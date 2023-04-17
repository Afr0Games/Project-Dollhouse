/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the GonzoNet.
The Original Code is the Parlo Library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using GonzoNet.Packets;
using GonzoNet;
using System.Linq.Expressions;

namespace GonzoNet
{
    /// <summary>
    /// Occurs when a network error happened.
    /// </summary>
    /// <param name="Exception">The SocketException that was thrown.</param>
	public delegate void NetworkErrorDelegate(SocketException Exception);

    /// <summary>
    /// Occurs when a packet was received.
    /// </summary>
    /// <param name="Sender">The NetworkClient instance that sent or received the packet.</param>
    /// <param name="P">The Packet that was received.</param>
	public delegate void ReceivedPacketDelegate(NetworkClient Sender, Packet P);

    /// <summary>
    /// Occurs when a client connected to a server.
    /// </summary>
    /// <param name="LoginArgs">The arguments that were used to establish the connection.</param>
	public delegate void OnConnectedDelegate(LoginArgsContainer LoginArgs);

    /// <summary>
    /// Occurs when a client a client disconnected.
    /// </summary>
    /// <param name="Sender">The NetworkClient instance that disconnected.</param>
    public delegate void ClientDisconnectedDelegate(NetworkClient Sender);

    /// <summary>
    /// Occurs when a server sent a packet saying it's about to disconnect.
    /// </summary>
    /// <param name="Sender">The NetworkClient instance used to connect to the server.</param>
	public delegate void ServerDisconnectedDelegate(NetworkClient Sender);

    /// <summary>
    /// Occurs when a client missed too many heartbeats.
    /// </summary>
    /// <param name="Sender">The client that lost too many heartbeats.</param>
    public delegate void OnConnectionLostDelegate(NetworkClient Sender);

    public class NetworkClient : IDisposable, IAsyncDisposable
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
        /// Is this client still alive,
        /// I.E has it not missed too 
        /// many heartbeats?
        /// </summary>
        public bool IsAlive
        {
            get { return m_IsAlive; }
        }

        /// <summary>
        /// This client's SessionID. Ensures that a client is unique even if 
        /// multiple clients are trying to connect using the same IP.
        /// </summary>
        public Guid SessionId { get; } = Guid.NewGuid();

        private byte[] m_RecvBuf;
        PacketHandler m_Handler;

        private DateTime m_LastHeartbeatSent;
        private bool m_IsAlive = true;
        private object m_IsAliveLock = new object();

        private readonly object m_MissedHeartbeatsLock = new object();
        private int m_MissedHeartbeats = 0;

        private int m_MaxMissedHeartbeats = 6;
        private int m_HeartbeatInterval = 30; //In seconds.

        private CancellationTokenSource m_SendHeartbeatCTS = new CancellationTokenSource();
        private CancellationTokenSource m_CheckMissedHeartbeatsCTS = new CancellationTokenSource();

        /// <summary>
        /// Gets or sets the number of missed heartbeats.
        /// Defaults to 6. If this number is reached, 
        /// the client will be assumed to be disconnected
        /// and will be disposed.
        /// </summary>
        public int MaxMissedHeartbeats
        {
            get { return m_MaxMissedHeartbeats; }
            set { m_MaxMissedHeartbeats = value; }
        }

        /// <summary>
        /// The number of missed heartbeats.
        /// </summary>
        public int MissedHeartbeats
        {
            get { return m_MissedHeartbeats; }
        }

        /// <summary>
        /// Gets or sets how many seconds should pass before a heartbeat is sent.
        /// Defaults to 30 seconds.
        /// </summary>
        public int HeartbeatInterval
        {
            get { return m_HeartbeatInterval; }
            set { m_HeartbeatInterval = value; }
        }

        private ProcessingBuffer m_ProcessingBuffer = new ProcessingBuffer();

        protected LoginArgsContainer m_LoginArgs;

        /// <summary>
        /// Fired when a network error occured.
        /// </summary>
		public event NetworkErrorDelegate OnNetworkError;

        /// <summary>
        /// Fired when this NetworkClient instance received data.
        /// </summary>
		public event ReceivedPacketDelegate OnReceivedData;

        /// <summary>
        /// Fired when this NetworkClient instance connected to a server.
        /// </summary>
		public event OnConnectedDelegate OnConnected;

        /// <summary>
        /// Fired when this NetworkClient instance disconnected.
        /// </summary>
        public event ClientDisconnectedDelegate OnClientDisconnected;

        /// <summary>
        /// Fired when this MetworkClient instance received a packet about the server's impending disconnection.
        /// </summary>
		public event ServerDisconnectedDelegate OnServerDisconnected;

        /// <summary>
        /// Fired when this NetworkClient instance missed too many heartbeats.
        /// </summary>
        public event OnConnectionLostDelegate OnConnectionLost;

        /// <summary>
        /// Initializes a client for connecting to a remote server and listening to data.
        /// </summary>
        /// <param name="IP">The IP to connect to.</param>
        /// <param name="Port">The port to connect to.</param>
        /// <param name="EMode">The encryption mode to use!</param>
        /// <param name="KeepAlive">Should this connection be kept alive?</param>
        public NetworkClient(string IP, int Port, bool KeepAlive)
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

            m_RecvBuf = new byte[ProcessingBuffer.MAX_PACKET_SIZE];

            m_ProcessingBuffer.OnProcessedPacket += M_ProcessingBuffer_OnProcessedPacket;
        }

        /// <summary>
        /// Initializes a client that listens for data.
        /// </summary>
        /// <param name="ClientSocket">The client's socket.</param>
        /// <param name="Server">The Listener instance calling this constructor.</param>
        /// <param name="EMode">The encryption mode to use!</param>
        public NetworkClient(Socket ClientSocket, Listener Server)
        {
            if (ClientSocket == null || Server == null)
                throw new ArgumentNullException("ClientSocket or Server!");

            m_Sock = ClientSocket;
            m_Listener = Server;
            m_RecvBuf = new byte[ProcessingBuffer.MAX_PACKET_SIZE];

            m_ProcessingBuffer.OnProcessedPacket += M_ProcessingBuffer_OnProcessedPacket;

            m_MissedHeartbeats = 0;
            _ = CheckforMissedHeartbeats();

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

                    _ = SendHeartbeatAsync();
                }
                catch (SocketException e)
                {
                    //Hopefully all classes inheriting from NetworkedUIElement will subscribe to this...
                    OnNetworkError?.Invoke(e);
                }
            }
        }

        /// <summary>
        /// Asynchronously sends data to a connected client or server.
        /// </summary>
        /// <param name="Data">The data to send.</param>
        /// <returns>A Task instance that can be await-ed.</returns>
        /// <exception cref="ArgumentNullException">Thrown if Data is null.</exception>
        /// <exception cref="SocketException">Thrown if the socket attempted to send data without being connected.</exception>
        public async Task SendAsync(byte[] Data)
        {
            if (Data == null || Data.Length < 1)
                throw new ArgumentNullException("Data");

            try
            {
                if (m_Connected)
                    await m_Sock.SendAsync(new ArraySegment<byte>(Data), SocketFlags.None);
                else
                    throw new SocketException((int)SocketError.NotConnected);
            }
            catch (SocketException E)
            {
                if (E.SocketErrorCode == SocketError.NotConnected)
                {
                    Logger.Log("Error sending data: Client is not connected.", LogLevel.warn);
                    throw E;
                }
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
            if (P.ID == (byte)GonzoNetIDs.SGoodbye)
            {
                OnServerDisconnected?.Invoke(this);
                return;
            }
            if (P.ID == (byte)GonzoNetIDs.CGoodbye) //Client notified server of disconnection.
            {
                OnClientDisconnected?.Invoke(this);
                return;
            }
            if (P.ID == (byte)GonzoNetIDs.Heartbeat)
            {
                lock (m_IsAliveLock)
                    m_IsAlive = true;

                lock(m_MissedHeartbeatsLock)
                    m_MissedHeartbeats = 0;

                return;
            }

            OnReceivedData?.Invoke(this, P);
        }

        /// <summary>
        /// Asynchronously receives data.
        /// </summary>
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
        /// Periodically sends a heartbeat to the server.
        /// How often is determined by HeartbeatInterval.
        /// </summary>
        private async Task SendHeartbeatAsync()
        {
            try
            {
                while (true)
                {
                    if (m_SendHeartbeatCTS.Token.IsCancellationRequested)
                        break;

                    try
                    {
                        HeartbeatPacket Heartbeat = new HeartbeatPacket((DateTime.Now > m_LastHeartbeatSent) ?
                            DateTime.Now - m_LastHeartbeatSent : m_LastHeartbeatSent - DateTime.Now);
                        m_LastHeartbeatSent = DateTime.Now;
                        byte[] HeartbeatData = Heartbeat.ToByteArray();
                        Packet Pulse = new Packet((byte)GonzoNetIDs.Heartbeat,
                            (ushort)(PacketHeaders.UNENCRYPTED + HeartbeatData.Length), HeartbeatData);
                        await SendAsync(Pulse.BuildPacket());
                    }
                    catch (Exception E)
                    {
                        Logger.Log("Error sending heartbeat: " + E.Message, LogLevel.error);
                    }

                    await Task.Delay(m_HeartbeatInterval * 1000, m_SendHeartbeatCTS.Token);
                }
            }
            catch(TaskCanceledException)
            {
                Logger.Log("SendHeartbeat task cancelled", LogLevel.info);
            }
        }

        /// <summary>
        /// Periodically checks for missed heartbeats.
        /// How often is determined by HeartbeatInterval.
        /// </summary>
        private async Task CheckforMissedHeartbeats()
        {
            try
            {
                while (true)
                {
                    if (m_CheckMissedHeartbeatsCTS.Token.IsCancellationRequested)
                        break;

                    lock (m_MissedHeartbeatsLock)
                        m_MissedHeartbeats++;

                    if (m_MissedHeartbeats > m_MaxMissedHeartbeats)
                    {
                        lock (m_IsAliveLock)
                            m_IsAlive = false;

                        OnConnectionLost?.Invoke(this);
                    }

                    await Task.Delay(m_HeartbeatInterval * 1000, m_CheckMissedHeartbeatsCTS.Token);
                }
            }
            catch(TaskCanceledException) 
            {
                Logger.Log("CheckforMissedHeartbeats task cancelled", LogLevel.info);
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
                        //Set the timeout to five seconds by default for clients,
                        //even though it's not really important for clients.
                        GoodbyePacket ByePacket = new GoodbyePacket((int)GonzoDefaultTimeouts.Client);
                        byte[] ByeData = ByePacket.ToByteArray();
                        Packet Goodbye = new Packet((byte)GonzoNetIDs.CGoodbye, 
                            (ushort)(PacketHeaders.UNENCRYPTED + ByeData.Length), ByeData);
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
            catch (SocketException E)
            {
                Logger.Log("Exception happened during NetworkClient.DisconnectAsync():" + E.ToString(), LogLevel.error);
            }
            catch (ObjectDisposedException)
            {
                Logger.Log("NetworkClient.DisconnectAsync() tried to shutdown or disconnect socket that was already disposed",
                    LogLevel.warn);
            }
            finally
            {
                m_SocketSemaphore.Release();
            }
        }

        /// <summary>
        /// Finds a PacketHandler instance based on the provided ID.
        /// </summary>
        /// <param name="ID">The ID of the PacketHandler to retrieve.</param>
        /// <returns>A PacketHandler instance if it was found, or null if it wasn't.</returns>
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

        /// <summary>
        /// Finalizer for NetworkClient.
        /// </summary>
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
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                m_ReceiveCancellationTokenSource.Cancel();

                if(m_Sock != null)
                    m_Sock.Dispose();

                m_ProcessingBuffer.OnProcessedPacket -= M_ProcessingBuffer_OnProcessedPacket;
                m_ProcessingBuffer.Dispose();

                // Prevent the finalizer from calling ~NetworkClient, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                Logger.Log("NetworkClient not explicitly disposed!", LogLevel.error);
        }

        /// <summary>
        /// Disposes of the async resources used by this NetworkClient instance.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (m_Listener == null) //This is already called by the NetworkListener instance in its Dispose method...
                await DisconnectAsync();

            m_CheckMissedHeartbeatsCTS.Cancel();
            m_SendHeartbeatCTS.Cancel();
        }
    }
}