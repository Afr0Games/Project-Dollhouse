﻿using Parlo;
using Parlo.Packets;
using SecureRemotePassword;
using ZeroFormatter;

namespace TSOProtocol
{
    public delegate Task OnPacketReceivedDelegate(IPacket Packet, byte ID, NetworkClient Sender);

    public class ClientNetworkManager
    {
        private static Lazy<ClientNetworkManager> m_Instance = new Lazy<ClientNetworkManager>(() => new ClientNetworkManager());
        private static NetworkClient m_Client = default!;

        private static SrpClient m_SRPClient = new SrpClient();
        private static SrpEphemeral m_ClientEphemeral = default!;

        private static Task m_NetworkingTask = default!;

        private static bool m_HasBeenAuthenticated = false;

        /// <summary>
        /// Has the client been authenticated?
        /// This means it should start sending 
        /// encrypted packets to the server.
        /// </summary>
        public static bool HasBeenAuthenticated { get { return m_HasBeenAuthenticated; } }

        /// <summary>
        /// Event invoked when the client has connected.
        /// </summary>
        public static event OnConnectedDelegate OnConnected = default!;

        /// <summary>
        /// Event invoked when a network error occurred. 
        /// </summary>
        public static event NetworkErrorDelegate OnNetworkError = default!;

        /// <summary>
        /// Event invoked when the client received a packet.
        /// </summary>
        public static event OnPacketReceivedDelegate OnPacketReceived = default!;

        /// <summary>
        /// Gets an instance of this ClientNetworkManager.
        /// </summary>
        public static ClientNetworkManager Instance { get { return m_Instance.Value; } }

        /// <summary>
        /// Connects to a remote server.
        /// </summary>
        /// <param name="IP">The IP to connect to.</param>
        /// <param name="Port">The port to use.</param>
        /// <param name="Username">The player's username.</param>
        /// <param name="Password">The player's password.</param>
        public void Connect(string IP, int Port, string Username, string Password)
        {
            m_Client = new NetworkClient(new ParloSocket(true));
            m_Client.OnConnected += Client_OnConnected;
            m_Client.OnNetworkError += Client_OnNetworkError;
            m_Client.OnReceivedData += Client_OnReceivedData;

            LoginArgsContainer LoginArgs = new LoginArgsContainer();
            LoginArgs.Address = IP;
            LoginArgs.Port = Port;
            LoginArgs.Client = m_Client;
            LoginArgs.Username = Username;
            LoginArgs.Password = Password;

            if (PacketHandlers.Get((byte)AuthPacketIDs.ServerInitialAuthResponse) == null)
            {
                PacketHandlers.Register((byte)AuthPacketIDs.ServerInitialAuthResponse, false,
                    new OnPacketReceived(Client_OnReceivedData));
            }

            m_NetworkingTask = Task.Run(async () => { await m_Client.ConnectAsync(LoginArgs); });
        }

        /// <summary>
        /// Sends a packet to the server.
        /// </summary>
        /// <param name="Packet">The packet to send.</param>
        public Task SendAsync(Packet P)
        {
            return m_Client.SendAsync(P.BuildPacket());
        }

        /// <summary>
        /// Sends a encrypted packet to the server.
        /// </summary>
        /// <param name="Packet">The encrypted packet to send.</param>
        public Task SendAsync(EncryptedPacket P)
        {
            if (!m_HasBeenAuthenticated)
                throw new InvalidOperationException("Cannot send encrypted packets before authentication.");

            return m_Client.SendAsync(P.BuildPacket());
        }

        /// <summary>
        /// The client connected to the given server.
        /// This starts the SRP6 authentication process by sending an initial packet.
        /// </summary>
        /// <param name="LoginArgs">The LoginArgsContainer created based on the args passed to Connect().</param>
        private async Task Client_OnConnected(LoginArgsContainer LoginArgs)
        {
            await OnConnected?.Invoke(LoginArgs);

            ClientInitialAuth InitialAuth = new ClientInitialAuth();
            InitialAuth.Username = LoginArgs.Username;

            string Salt = m_SRPClient.GenerateSalt();
            string PrivateKey = m_SRPClient.DerivePrivateKey(Salt, LoginArgs.Username, LoginArgs.Password);
            string Verifier = m_SRPClient.DeriveVerifier(PrivateKey);

            m_ClientEphemeral = m_SRPClient.GenerateEphemeral();
            InitialAuth.Ephemeral = m_ClientEphemeral.Public;
            byte[] Data = ZeroFormatterSerializer.Serialize(InitialAuth);
        }

        /// <summary>
        /// A network error occurred!
        /// </summary>
        /// <param name="Exception">The SocketException that was thrown.</param>
        private void Client_OnNetworkError(System.Net.Sockets.SocketException Exception)
        {
            OnNetworkError?.Invoke(Exception);
        }

        /// <summary>
        /// The client received a packet!
        /// </summary>
        /// <param name="Packet">The packet that the client received.</param>
        private async Task Client_OnReceivedData(NetworkClient Sender, Packet P)
        {
            switch (P.ID)
            {
                case (byte)AuthPacketIDs.ServerInitialAuthResponse:
                    ServerInitialAuthResponse InitialAuthResponsePacket =
                        ZeroFormatterSerializer.Deserialize<ServerInitialAuthResponse>(P.Data);
                    await OnPacketReceived?.Invoke(InitialAuthResponsePacket, P.ID, Sender);
                    break;
                case (byte)AuthPacketIDs.SAuthProof:
                    m_HasBeenAuthenticated = true;
                    AuthProof AuthProofPacket = ZeroFormatterSerializer.Deserialize<AuthProof>(P.Data);
                    await OnPacketReceived?.Invoke(AuthProofPacket, P.ID, Sender);
                    break;
            }
        }
    }
}
