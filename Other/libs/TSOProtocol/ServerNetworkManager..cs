using System;
using GonzoNet.Encryption;
using System.Threading.Tasks;
using ZeroFormatter;
using SecureRemotePassword;
using System.Net;
using System.Collections.Concurrent;
using GonzoNet;
using GonzoNet.Packets;

namespace TSOProtocol
{
    public class ServerNetworkManager
    {
        private static readonly Lazy<ServerNetworkManager> m_Instance = new(() => new ServerNetworkManager());
        private Listener m_Listener = default!;

        private SrpServer m_SRPServer = new SrpServer();
        private SrpEphemeral m_ServerEphemeral = new SrpEphemeral();

        private Task m_NetworkingTask = default!;

        private ConcurrentDictionary<Guid, NetworkClient> m_Clients = new ConcurrentDictionary<Guid, NetworkClient>();

        /// <summary>
        /// Event invoked when a network error occurred. 
        /// </summary>
        public event NetworkErrorDelegate OnNetworkError = default!;

        /// <summary>
        /// Gets an instance of this ServerNetworkManager.
        /// </summary>
        public static ServerNetworkManager Instance { get { return m_Instance.Value; } }

        /// <summary>
        /// Event invoked when a client received a packet.
        /// </summary>
        public event OnPacketReceivedDelegate OnPacketReceived = default!;

        public void Listen(string IP, int Port)
        {
            m_Listener = new Listener(); //Change this in release
            m_Listener.OnConnected += M_Listener_OnConnected;
            m_Listener.OnDisconnected += M_Listener_OnDisconnected;

            IPEndPoint Endpoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            PacketHandlers.Register((byte)AuthPacketIDs.ClientInitialAuth, false, 0,
                new OnPacketReceive(Client_OnReceivedData));
            PacketHandlers.Register((byte)AuthPacketIDs.CAuthProof, false, 0,
                new OnPacketReceive(Client_OnReceivedData));

            _ = m_Listener.InitializeAsync(Endpoint); 
        }

        /// <summary>
        /// A new client connected!
        /// </summary>
        /// <param name="Client">The client that connected.</param>
        private void M_Listener_OnConnected(NetworkClient Client)
        {
            Guid NewClientGuid = Guid.NewGuid();

            //False means the key already existed which should be impossible.
            if (!m_Clients.TryAdd(NewClientGuid, Client)) 
            {
                throw new NetworkException("ServerNetworkManager: Key already existed!");
            }

            Client.OnReceivedData += Client_OnReceivedData;
        }

        /// <summary>
        /// Received a packet from a client.
        /// </summary>
        /// <param name="Packet">The packet that was received.</param>
        private void Client_OnReceivedData(NetworkClient Sender, Packet P)
        {
            //PacketStream PStream = (PacketStream)Packet;

            switch (P.ID)
            {
                case (byte)AuthPacketIDs.ClientSignup:
                    ClientSignup SignUpPacket = ZeroFormatterSerializer.Deserialize<ClientSignup>(P.Data);
                    OnPacketReceived?.Invoke(SignUpPacket, P.ID, Sender);
                    break;
                case (byte)AuthPacketIDs.ClientInitialAuth:
                    ClientInitialAuth InitialAuthPacket = ZeroFormatterSerializer.Deserialize<ClientInitialAuth>(P.Data);
                    OnPacketReceived?.Invoke(InitialAuthPacket, P.ID, Sender);
                    break;
                case (byte)AuthPacketIDs.CAuthProof:
                    AuthProof AuthProofPacket = ZeroFormatterSerializer.Deserialize<AuthProof>(P.Data);
                    OnPacketReceived?.Invoke(AuthProofPacket, P.ID, Sender);
                    break;
            }
        }

        /// <summary>
        /// A client disconnected!
        /// </summary>
        /// <param name="Client">The client that disconnected.</param>
        private void M_Listener_OnDisconnected(NetworkClient Client)
        {
        }
    }
}
