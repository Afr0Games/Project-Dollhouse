/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the TSOProtocol.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/
using System.Threading.Tasks;
using Parlo;
using Parlo.Encryption;
using Parlo.Packets;
using SecureRemotePassword;
using System.Diagnostics;
using ZeroFormatter;
using System.Net;

namespace TSOProtocol
{
    public delegate Task OnPacketReceivedDelegate(IPacket Packet, byte ID, NetworkClient Sender);

    public class ClientNetworkManager
    {
        private static Lazy<ClientNetworkManager> m_Instance = new Lazy<ClientNetworkManager>(() => new ClientNetworkManager());
        private static NetworkClient m_Client = default!;
        private static EncryptionArgs m_EncryptionArgs = new EncryptionArgs();

        private static SrpClient m_SRPClient = new SrpClient();
        private static SrpEphemeral m_ClientEphemeral = default!;
        private static SrpSession m_Session = default!;

        private static Task m_NetworkingTask = default!;

        private static bool m_HasBeenAuthenticated = false;

        private static LoginArgsContainer m_LoginArgs = default!;

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
        public async Task Connect(string IP, int Port, string Username, string Password)
        {
            try
            {
                m_Client = new NetworkClient(new ParloSocket(true));
            }
            catch(Exception ex) { throw ex; }
            m_Client.OnConnected += Client_OnConnected;
            m_Client.OnNetworkError += Client_OnNetworkError;
            m_Client.OnReceivedData += Client_OnReceivedData;

            m_LoginArgs = new LoginArgsContainer();
            m_LoginArgs.Address = IP;
            m_LoginArgs.Port = Port;
            m_LoginArgs.Client = m_Client;
            m_LoginArgs.Username = Username;
            m_LoginArgs.Password = Password;

            if (PacketHandlers.Get((byte)AuthPacketIDs.ServerInitialAuthResponse) == null)
            {
                PacketHandlers.Register((byte)AuthPacketIDs.ServerInitialAuthResponse, false,
                    new OnPacketReceived(Client_OnReceivedData));
            }
            if (PacketHandlers.Get((byte)AuthPacketIDs.SAuthProof) == null)
            {
                PacketHandlers.Register((byte)AuthPacketIDs.SAuthProof, false,
                    new OnPacketReceived(Client_OnReceivedData));
            }

            //m_NetworkingTask = Task.Run(async () => { await m_Client.ConnectAsync(m_LoginArgs); });
            await m_Client.ConnectAsync(m_LoginArgs);
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

            m_ClientEphemeral = m_SRPClient.GenerateEphemeral();
            ClientInitialAuth InitialAuth = new ClientInitialAuth(LoginArgs.Username, m_ClientEphemeral.Public);

            byte[] Data = ZeroFormatterSerializer.Serialize(InitialAuth);
            Packet PacketToSend = new Packet((byte)AuthPacketIDs.ClientInitialAuth, Data, false);
            await m_Client.SendAsync(PacketToSend.BuildPacket());
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
            byte[] PacketData;

            switch (P.ID)
            {
                case (byte)AuthPacketIDs.ServerInitialAuthResponse:
                        ServerInitialAuthResponse InitialAuthResponsePacket =
                        ZeroFormatterSerializer.Deserialize<ServerInitialAuthResponse>(P.Data);
                    string Salt = InitialAuthResponsePacket.Salt.TrimEnd('\0');
                    string PrivateKey = m_SRPClient.DerivePrivateKey(Salt, m_LoginArgs.Username, m_LoginArgs.Password);

                    try
                    {
                        m_Session = m_SRPClient.DeriveSession(m_ClientEphemeral.Secret, 
                            InitialAuthResponsePacket.PublicEphemeral, Salt, m_LoginArgs.Username, PrivateKey);

                        m_EncryptionArgs = new EncryptionArgs()
                        {
                            Mode = EncryptionMode.AES,
                            Salt = Salt,
                            Key = m_Session.Key
                        };
                    }
                    catch(Exception E)
                    {
                        Debug.WriteLine("Client couldn't derive session: \r\n" + E.ToString());
                        OnNetworkError((System.Net.Sockets.SocketException)E);
                    }

                    AuthProof CProofPacket = new AuthProof(m_Session.Proof);

                    ///What's wrong here???
                    await OnPacketReceived?.Invoke(InitialAuthResponsePacket, P.ID, Sender);

                    PacketData = ZeroFormatterSerializer.Serialize(CProofPacket);
                    Packet PacketToSend = new Packet((byte)AuthPacketIDs.CAuthProof, PacketData, false);
                    await Sender.SendAsync(PacketToSend.BuildPacket());
                    break;
                case (byte)AuthPacketIDs.SAuthProof:
                    m_HasBeenAuthenticated = true;
                    EncryptedPacket EncPacket = EncryptedPacket.FromPacket(m_EncryptionArgs, P);
                    AuthProof AuthProofPacket = ZeroFormatterSerializer.Deserialize<AuthProof>(EncPacket.DecryptPacket());

                    string SessionProof = AuthProofPacket.SessionProof.TrimEnd('\0');

                    try
                    {
                        m_SRPClient.VerifySession(m_ClientEphemeral.Public,
                            m_Session, AuthProofPacket.SessionProof);
                    }
                    catch (Exception E)
                    {
                        Debug.WriteLine("Client couldn't derive session: \r\n" + E.ToString());
                        OnNetworkError((System.Net.Sockets.SocketException)E);
                    }

                    await OnPacketReceived?.Invoke(AuthProofPacket, P.ID, Sender);
                    break;
            }
        }
    }
}
