/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the TSOClient.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using GonzoNet.Encryption;

namespace GonzoNet
{
	public delegate void NetworkErrorDelegate(SocketException Exception);
	public delegate void ReceivedPacketDelegate(NetworkClient Sender, Packet P);
	public delegate void OnConnectedDelegate(LoginArgsContainer LoginArgs);

	public class NetworkClient
	{
		protected Listener m_Listener;
		private Socket m_Sock;
		private string m_IP;
		private int m_Port;

		private object m_ConnectedLock = new object();
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

		/// <summary>
		/// Initializes a client for connecting to a remote server and listening to data.
		/// </summary>
		/// <param name="IP">The IP to connect to.</param>
		/// <param name="Port">The port to connect to.</param>
		/// <param name="EMode">The encryption mode to use!</param>
		/// <param name="KeepAlive">Should this connection be kept alive?</param>
		public NetworkClient(string IP, int Port, EncryptionMode EMode, bool KeepAlive)
		{
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
			m_Sock = ClientSocket;
			m_Listener = Server;
            m_RecvBuf = new byte[ProcessingBuffer.MAX_PACKET_SIZE];

            m_EMode = EMode;

            m_ProcessingBuffer.OnProcessedPacket += M_ProcessingBuffer_OnProcessedPacket;

			lock (m_ConnectedLock)
				m_Connected = true;

            m_Sock.BeginReceive(m_RecvBuf, 0, m_RecvBuf.Length, SocketFlags.None,
				new AsyncCallback(ReceiveCallback), m_Sock);
		}

		/// <summary>
		/// Connects to the login server.
		/// </summary>
		/// <param name="LoginArgs">Arguments used for login. Can be null.</param>
		public void Connect(LoginArgsContainer LoginArgs)
		{
			m_LoginArgs = LoginArgs;

			if (LoginArgs != null)
			{
				if(m_EMode == EncryptionMode.AESCrypto)
				{
					lock (m_EncryptorLock)
					{
						m_ClientEncryptor = LoginArgs.Enc;
						m_ClientEncryptor.Username = LoginArgs.Username;
					}
				}
			}
			//Making sure that the client is not already connecting to the loginserver.
			if (!m_Sock.Connected)
			{
				m_Sock.BeginConnect(IPAddress.Parse(m_IP), m_Port, new AsyncCallback(ConnectCallback), m_Sock);
			}
		}

		public void Send(byte[] Data)
		{
			if (Data == null || Data.Length < 1)
				throw new SocketException();

			try
			{
				m_Sock.BeginSend(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnSend), m_Sock);
			}
			catch (SocketException)
			{
				//TODO: Reconnect?
				Disconnect();
			}
		}

		/// <summary>
		/// Sends an encrypted packet to the server.
		/// Automatically appends the length of the packet after the ID, as 
		/// the encrypted data can be smaller or longer than that of the
		/// unencrypted data.
		/// </summary>
		/// <param name="PacketID">The ID of the packet (will remain unencrypted).</param>
		/// <param name="Data">The data that will be encrypted.</param>
		public void SendEncrypted(byte PacketID, byte[] Data)
		{
			if (!m_Connected) return;
			byte[] EncryptedData;

			lock (m_EncryptorLock)
				EncryptedData = m_ClientEncryptor.FinalizePacket(PacketID, Data);

			try
			{
				m_Sock.BeginSend(EncryptedData, 0, EncryptedData.Length, SocketFlags.None,
					new AsyncCallback(OnSend), m_Sock);
			}
			catch (SocketException)
			{
				Disconnect();
			}
		}

		protected virtual void OnSend(IAsyncResult AR)
		{
			Socket ClientSock = (Socket)AR.AsyncState;
			int NumBytesSent = ClientSock.EndSend(AR);
		}

		private void BeginReceive(/*object State*/)
		{
			m_Sock.BeginReceive(m_RecvBuf, 0, m_RecvBuf.Length, SocketFlags.None, 
				new AsyncCallback(ReceiveCallback), m_Sock);
		}

		private void ConnectCallback(IAsyncResult AR)
		{
			try
			{
				Socket Sock = (Socket)AR.AsyncState;
				Sock.EndConnect(AR);

				lock(m_ConnectedLock)
					m_Connected = true;

				BeginReceive();

				if (OnConnected != null)
					OnConnected(m_LoginArgs);
			}
			catch (SocketException E)
			{
				//Hopefully all classes inheriting from NetworkedUIElement will subscribe to this...
				if (OnNetworkError != null)
					OnNetworkError(E);
			}
		}

        /// <summary>
        /// We processed a packet, hurray!
        /// </summary>
        /// <param name="Packet">The packet that was processed.</param>
        private void M_ProcessingBuffer_OnProcessedPacket(Packet P)
        {
            OnReceivedData(this, P);
        }

		protected virtual void ReceiveCallback(IAsyncResult AR)
		{
			try
			{
				Socket Sock = (Socket)AR.AsyncState;
				int NumBytesRead = Sock.EndReceive(AR);

				/** Can't do anything with this! **/
				if (NumBytesRead == 0) { return; }

				byte[] TmpBuf = new byte[NumBytesRead];
				Buffer.BlockCopy(m_RecvBuf, 0, TmpBuf, 0, NumBytesRead);
				//m_RecvBuf = new byte[11024]; //Clear, to make sure this buffer is always fresh.
				m_RecvBuf = new byte[ProcessingBuffer.MAX_PACKET_SIZE];

                //Keep shoveling shit into the buffer as fast as we can.
                m_ProcessingBuffer.AddData(TmpBuf); //Hence the Shoveling Shit Algorithm (SSA).

				m_Sock.BeginReceive(m_RecvBuf, 0, m_RecvBuf.Length, SocketFlags.None,
					new AsyncCallback(ReceiveCallback), m_Sock);
			}
			catch (SocketException)
			{
				Disconnect();
			}
		}

		/// <summary>
		/// This socket's remote IP. Will return null if the socket is not connected remotely.
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
		public void Disconnect()
		{
			try
			{
				m_Sock.Shutdown(SocketShutdown.Both);
				m_Sock.Disconnect(true);

				lock (m_ConnectedLock)
					m_Connected = false;
			}
			catch
			{
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
    }
}