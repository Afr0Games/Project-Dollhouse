/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the GonzoNet.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GonzoNet.Encryption;
using GonzoNet.Packets;

namespace GonzoNet
{
	public delegate void NetworkErrorDelegate(SocketException Exception);
	public delegate void ReceivedPacketDelegate(NetworkClient Sender, Packet P);
	public delegate void OnConnectedDelegate(LoginArgsContainer LoginArgs);
    public delegate void ClientDisconnectedDelegate(NetworkClient Sender);
	public delegate void ServerDisconnectedDelegate(NetworkClient Sender);

    public class NetworkClient : IDisposable
	{
		protected Listener m_Listener;
        private readonly SemaphoreSlim m_SocketSemaphore = new SemaphoreSlim(1, 1);
        private Socket m_Sock;
		private string m_IP;
		private int m_Port;

        private readonly CancellationTokenSource m_ReceiveCancellationTokenSource = new CancellationTokenSource();

        private readonly object m_ConnectedLock = new object();
        private bool m_Connected = false;

        /// <summary>
        /// Is this client connected to a server?
        /// </summary>
        public bool IsConnected
		{ 
			get { return m_Connected; } 
		}

		/// <summary>
		/// This client's SessionID. Ensures that a client is unique even if 
		/// multiple clients are trying to connect using the same IP.
		/// </summary>
        public Guid SessionId { get; } = Guid.NewGuid();

        private byte[] m_RecvBuf;
		PacketHandler m_Handler;

		ProcessingBuffer m_ProcessingBuffer = new ProcessingBuffer();

		private EncryptionMode m_EMode;
		private object m_EncryptorLock = new object();
		private Encryptor m_ClientEncryptor;

		public Encryptor ClientEncryptor
		{
			get
			{
				if (m_ClientEncryptor == null)
				{
					switch (m_EMode)
					{
						case EncryptionMode.AESCrypto:
							lock (m_EncryptorLock)
								m_ClientEncryptor = new AESEncryptor("");
							return m_ClientEncryptor;
						default: //Should never end up here, so doesn't really matter what we put...
							lock (m_EncryptorLock)
								m_ClientEncryptor = new AESEncryptor("");
							return m_ClientEncryptor;
					}
				}

				return m_ClientEncryptor;
			}

			set { m_ClientEncryptor = value; }
		}

		protected LoginArgsContainer m_LoginArgs;

		public event NetworkErrorDelegate OnNetworkError;
		public event ReceivedPacketDelegate OnReceivedData;
		public event OnConnectedDelegate OnConnected;
        public event ClientDisconnectedDelegate OnClientDisconnected;
		public event ServerDisconnectedDelegate OnServerDisconnected;

        /// <summary>
        /// Initializes a client for connecting to a remote server and listening to data.
        /// </summary>
        /// <param name="IP">The IP to connect to.</param>
        /// <param name="Port">The port to connect to.</param>
        /// <param name="EMode">The encryption mode to use!</param>
        /// <param name="KeepAlive">Should this connection be kept alive?</param>
        public NetworkClient(string IP, int Port, EncryptionMode EMode, bool KeepAlive)
		{
            if (IP == null)
                throw new ArgumentNullException("IP");

            if (IP == string.Empty)
                throw new ArgumentException("IP must be specified!");

			m_Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			if (KeepAlive)
				m_Sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

			m_IP = IP;
			m_Port = Port;

			m_EMode = EMode;

			//m_RecvBuf = new byte[11024];
			m_RecvBuf = new byte[ProcessingBuffer.MAX_PACKET_SIZE];

            m_ProcessingBuffer.OnProcessedPacket += M_ProcessingBuffer_OnProcessedPacket;
		}

        /// <summary>
        /// Initializes a client that listens for data.
        /// </summary>
        /// <param name="ClientSocket">The client's socket.</param>
        /// <param name="Server">The Listener instance calling this constructor.</param>
		/// <param name="EMode">The encryption mode to use!</param>
        public NetworkClient(Socket ClientSocket, Listener Server, EncryptionMode EMode)
		{
            if (ClientSocket == null || Server == null)
                throw new ArgumentNullException("ClientSocket or Server!");

			m_Sock = ClientSocket;
			m_Listener = Server;
            m_RecvBuf = new byte[ProcessingBuffer.MAX_PACKET_SIZE];

            m_EMode = EMode;

            m_ProcessingBuffer.OnProcessedPacket += M_ProcessingBuffer_OnProcessedPacket;

			lock (m_ConnectedLock)
				m_Connected = true;

            _ = ReceiveAsync(); // Start the BeginReceive task without awaiting it
        }

        /// <summary>
        /// Connects to a server.
        /// </summary>
        /// <param name="LoginArgs">Arguments used for login. Can be null.</param>
        public async Task ConnectAsync(LoginArgsContainer LoginArgs)
        {
            m_LoginArgs = LoginArgs;

            if (LoginArgs != null)
            {
                if (m_EMode == EncryptionMode.AESCrypto)
                {
                    lock (m_EncryptorLock)
                    {
                        m_ClientEncryptor = LoginArgs.Enc;
                        m_ClientEncryptor.Username = LoginArgs.Username;
                    }
                }
            }
            //Making sure that the client is not already connecting to the login server.
            if (!m_Sock.Connected)
            {
                try
                {
                    await m_Sock.ConnectAsync(IPAddress.Parse(m_IP), m_Port);

                    lock (m_ConnectedLock)
                        m_Connected = true;

                    _ = ReceiveAsync();

                    OnConnected?.Invoke(m_LoginArgs);
                }
                catch (SocketException e)
                {
                    //Hopefully all classes inheriting from NetworkedUIElement will subscribe to this...
                    OnNetworkError?.Invoke(e);
                }
            }
        }

        public async Task SendAsync(byte[] Data)
		{
			if (Data == null || Data.Length < 1)
				throw new ArgumentNullException("Data");

            try
            {
                await m_Sock.SendAsync(new ArraySegment<byte>(Data), SocketFlags.None);
            }
            catch (Exception ex)
            {
                Logger.Log("Error sending data: " + ex.Message, LogLevel.error);

                //Disconnect without sending the disconnect message to prevent recursion.
                await DisconnectAsync(false);
            }
        }

        /// <summary>
        /// We processed a packet, hurray!
        /// </summary>
        /// <param name="Packet">The packet that was processed.</param>
        private void M_ProcessingBuffer_OnProcessedPacket(Packet P)
        {
			if(P.ID == (byte)GonzoNetIDs.SGoodbye)
				OnServerDisconnected?.Invoke(this);
			if(P.ID == (byte)GonzoNetIDs.CGoodbye) //Client notified server of disconnection.
				OnClientDisconnected?.Invoke(this);

            OnReceivedData?.Invoke(this, P);
        }

        private async Task ReceiveAsync()
        {
            while (m_Connected)
            {
                if (m_ReceiveCancellationTokenSource.Token.IsCancellationRequested)
                    return;

                if (m_Sock == null || !m_Sock.Connected)
                    return;

                try
                {
                    int bytesRead = await m_Sock.ReceiveAsync(new ArraySegment<byte>(m_RecvBuf), SocketFlags.None);

                    if (bytesRead > 0)
                    {
                        byte[] TmpBuf = new byte[bytesRead];
                        Buffer.BlockCopy(m_RecvBuf, 0, TmpBuf, 0, bytesRead);
                        //Clear, to make sure this buffer is always fresh.
                        m_RecvBuf = new byte[ProcessingBuffer.MAX_PACKET_SIZE];

                        //Keep shoveling shit into the buffer as fast as we can.
                        m_ProcessingBuffer.AddData(TmpBuf); //Hence the Shoveling Shit Algorithm (SSA).
                    }
                    else //Can't do anything with this!
                    {
                        await DisconnectAsync(false);
                        return;
                    }

                    await Task.Delay(10); //STOP HOGGING THE PROCESSOR!
                }
                catch (SocketException)
                {
                    await DisconnectAsync(false);
                    return;
                }
            }
        }

		/// <summary>
		/// This NetworkClient's remote IP. Will return null if the NetworkClient's socket is not connected remotely.
		/// </summary>
		public string RemoteIP
		{
			get
			{
				IPEndPoint RemoteEP = (IPEndPoint)m_Sock.RemoteEndPoint;

				if (RemoteEP != null)
					return RemoteEP.Address.ToString();
				else
					return null;
			}
		}

		/// <summary>
		/// This socket's remote port. Will return 0 if the socket is not connected remotely.
		/// </summary>
		public int RemotePort
		{
			get
			{
				IPEndPoint RemoteEP = (IPEndPoint)m_Sock.RemoteEndPoint;

				if (RemoteEP != null)
					return RemoteEP.Port;
				else
					return 0;
			}
		}

        /// <summary>
        /// Disconnects this NetworkClient instance and stops
        /// all sending and receiving of data.
        /// </summary>
		/// <param name="SendDisconnectMessage">Should a DisconnectMessage be sent?</param>
        public async Task DisconnectAsync(bool SendDisconnectMessage = true)
        {
            try
            {
                await m_SocketSemaphore.WaitAsync();

				if (m_Connected && m_Sock != null)
				{
                    if (SendDisconnectMessage)
                    {
                        //Set the timeout to five seconds by default for clients, even though it's not really important for clients.
                        GoodbyePacket ByePacket = new GoodbyePacket((int)GonzoDefaultTimeouts.Client);
                        byte[] ByeData = ByePacket.ToByteArray();
                        Packet Goodbye = new Packet((byte)GonzoNetIDs.CGoodbye, (ushort)ByeData.Length, ByeData);
                        await SendAsync(Goodbye.BuildPacket());
                    }

                    if (m_Sock.Connected)
                    {
                        // Shutdown and disconnect the socket.
                        m_Sock.Shutdown(SocketShutdown.Both);
                        m_Sock.Disconnect(true);
                    }

					lock (m_ConnectedLock)
						m_Connected = false;
				}
            }
            catch(SocketException E)
            {
                Logger.Log("Exception happened during NetworkClient.DisconnectAsync():" + E.ToString(), LogLevel.error);
            }
            catch(ObjectDisposedException)
            {
                Logger.Log("NetworkClient.DisconnectAsync() tried to shutdown or disconnect socket that was already disposed", 
                    LogLevel.warn);
            }
            finally
            {
                m_SocketSemaphore.Release();
            }
        }

        private PacketHandler FindPacketHandler(byte ID)
		{
			PacketHandler Handler = PacketHandlers.Get(ID);

			if (Handler != null)
				return Handler;
			else return null;
		}

		/// <summary>
		/// Returns a unique hash code for this NetworkClient instance.
		/// Needs to be implemented for this class to be usable in a 
		/// Dictionary.
		/// </summary>
		/// <returns>A unique hash code.</returns>
        public override int GetHashCode()
        {
            return SessionId.GetHashCode();
        }

        ~NetworkClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this NetworkClient instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this NetworkClient instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual async void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if(m_Listener == null) //This is already called by the NetworkListener instance in its Dispose method...
				    await DisconnectAsync();

                m_ReceiveCancellationTokenSource.Cancel();

                m_Sock.Dispose();

                m_ProcessingBuffer.OnProcessedPacket -= M_ProcessingBuffer_OnProcessedPacket;
                m_ProcessingBuffer.Dispose();

                // Prevent the finalizer from calling ~NetworkClient, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                Logger.Log("NetworkClient not explicitly disposed!", LogLevel.error);
        }
    }
}