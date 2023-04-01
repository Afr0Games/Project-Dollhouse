/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the GonzoNet.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GonzoNet.Encryption;
using GonzoNet.Packets;

namespace GonzoNet
{
    public delegate void OnReceivedDelegate(Packet P, NetworkClient Client);
    public delegate void OnDisconnectedDelegate(NetworkClient Client);

    /// <summary>
    /// Represents a listener that listens for incoming clients.
    /// </summary>
    public class Listener : IDisposable
    {
		private BlockingCollection<NetworkClient> m_NetworkClients;
        private Socket m_ListenerSock;
        private IPEndPoint m_LocalEP;
        private readonly CancellationTokenSource m_ShutdownDelayCTS = new CancellationTokenSource();

        private EncryptionMode m_EMode;

        public event OnDisconnectedDelegate OnDisconnected;
		public event OnDisconnectedDelegate OnConnected;

		public BlockingCollection<NetworkClient> Clients
        {
            get { return m_NetworkClients; }
        }

        /// <summary>
        /// The local endpoint that this listener is listening to.
        /// </summary>
        public IPEndPoint LocalEP
        {
            get { return m_LocalEP; }
        }

        /// <summary>
        /// Initializes a new instance of Listener.
        /// </summary>
        public Listener(EncryptionMode Mode)
        {
            m_ListenerSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			m_NetworkClients = new BlockingCollection<NetworkClient>();

            m_EMode = Mode;
            /*switch (Mode)
            {
                case EncryptionMode.AESCrypto:
                    m_AesCryptoService = new AesCryptoServiceProvider();
                    m_AesCryptoService.GenerateIV();
                    m_AesCryptoService.GenerateKey();
                    break;
            }*/
        }

        /// <summary>
        /// Initializes Listener. Throws SocketException if something went haywire.
        /// </summary>
        /// <param name="LocalEP">The endpoint to listen on.</param>
        public virtual async Task InitializeAsync(IPEndPoint LocalEP)
        {
            m_LocalEP = LocalEP ?? throw new ArgumentNullException("LocalEP!");

            try
            {
                m_ListenerSock.Bind(LocalEP);
                m_ListenerSock.Listen(10000);
            }
            catch (SocketException E)
            {
                Logger.Log("Exception occured in Listener.InitializeAsync(): " + E.ToString(), LogLevel.error);
            }

            await AcceptAsync();
        }

        /// <summary>
        /// Asynchronously accepts clients.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        private async Task AcceptAsync()
        {
            while (true)
            {
                Socket AcceptedSocket = await m_ListenerSock.AcceptAsync();

                if (AcceptedSocket != null)
                {
                    Logger.Log("\nNew client connected!\r\n", LogLevel.info);

                    AcceptedSocket.LingerState = new LingerOption(true, 5);
                    NetworkClient NewClient = new NetworkClient(AcceptedSocket, this, m_EMode);
                    NewClient.OnClientDisconnected += NewClient_OnClientDisconnected;

                    switch (m_EMode)
                    {
                        case EncryptionMode.AESCrypto:
                            NewClient.ClientEncryptor = new AESEncryptor("");
                            break;
                    }

                    m_NetworkClients.Add(NewClient);
                    if (OnConnected != null) OnConnected(NewClient);
                }
            }
        }

        /// <summary>
        /// Gets a connected client based on its remote IP and remote port.
        /// </summary>
        /// <param name="RemoteIP">The remote IP of the client.</param>
        /// <param name="RemotePort">The remote port of the client.</param>
        /// <returns>A NetworkClient instance. Null if not found.</returns>
        public NetworkClient GetClient(string RemoteIP, int RemotePort)
		{
            if(RemoteIP == null)
                throw new ArgumentNullException("RemoteIP!");
            if (RemoteIP == string.Empty)
                throw new ArgumentException("RemoteIP must be specified!");

			lock (Clients)
			{
				foreach (NetworkClient PlayersClient in Clients)
				{
					if(RemoteIP.Equals(PlayersClient.RemoteIP, StringComparison.CurrentCultureIgnoreCase))
					{
						if(RemotePort == PlayersClient.RemotePort)
							return PlayersClient;
					}
				}
			}

			return null;
		}

        /// <summary>
        /// A connected client disconnected from the Listener.
        /// </summary>
        /// <param name="Sender">The NetworkClient instance that disconnected.</param>
        private void NewClient_OnClientDisconnected(NetworkClient Sender)
        {
            Logger.Log("Client disconnected!", LogLevel.info);
            Sender.Dispose();
            //m_NetworkClients.Remove(Sender);
        }

        /// <summary>
        /// The number of clients that are connected to this Listener instance.
        /// </summary>
        public int NumConnectedClients
        {
            get { return m_NetworkClients.Count; }
        }

        ~Listener()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this Listener instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this Listener instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual async void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                // First, we wait for all clients to disconnect.
                var DisconnectTasks = new List<Task>();
                foreach (NetworkClient Client in m_NetworkClients)
                    DisconnectTasks.Add(Client.DisconnectAsync());

                if (DisconnectTasks.Count > 0)
                {
                    await Task.WhenAll(DisconnectTasks);
                    await Task.Delay(TimeSpan.FromSeconds((double)GonzoDefaultTimeouts.Server));
                }

                // After all clients have disconnected, shutdown the listener socket.
                m_ListenerSock.Shutdown(SocketShutdown.Both);
                m_ListenerSock.Close();
                m_ListenerSock.Dispose();

                // Dispose of the clients.
                foreach (NetworkClient Client in m_NetworkClients)
                    Client.Dispose();

                m_NetworkClients.Dispose();

                // Prevent the finalizer from calling ~Listener, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                Logger.Log("Listener not explicitly disposed!", LogLevel.error);
        }
    }
}