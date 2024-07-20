/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the TSOProtocol library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Net;
using System.Collections.Concurrent;
using System.Diagnostics;
using Parlo;
using Parlo.Encryption;
using Parlo.Packets;
using SecureRemotePassword;
using TSOProtocol;
using TSOProtocol.Database;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Parlo.Docker
{
    internal class Program
    {
        private static BlockingCollection<NetworkClient> m_Clients = new BlockingCollection<NetworkClient>();

        private static SemaphoreSlim m_UserCacheSemaphore;
        private static UserCache m_UserCache;

        private static ConcurrentDictionary<NetworkClient, SClientData> m_SClientData = new ConcurrentDictionary<NetworkClient, SClientData>();
        private static ConcurrentDictionary<NetworkClient, ClientData> m_CClientData = new ConcurrentDictionary<NetworkClient, ClientData>();

        private static BlockingCollection<NetworkClient> m_AuthenticatedClients = new BlockingCollection<NetworkClient>();

        private static SrpClient m_SRPClient = new SrpClient();
        private static SrpServer m_SRPServer = new SrpServer();

        private const string m_USERNAME = "Mats", m_PASSWORD = "Test";

        private static bool m_Input = false;

        private static int m_NumClients = 0;

        private static int m_NumLogicalProcessors = Environment.ProcessorCount;
        private static int m_NumberOfCores = 0;

        private static int m_Port = 3077;

        static async Task Main(string[] args)
        {
            //Make sure the DB is fresh on startup, so that a new account is created every
            //the program starts. This should obviously not be done in a production scenario.
            if (File.Exists("/app/Users.db"))
                File.Delete("/app/Users.db");

            try
            {
                Database.CreateTables();
                //12C00000 = 300megs
                //10 minutes should be (more than?) enough - if a user logs in once, and has typed the
                //wrong password or username, it shouldn't take him 10 minutes to try again, UNLESS he
                //genuinely forgot it and needs to reset it, in which case it should take max 10 mins.
                m_UserCache = new UserCache("/app/Users.cache", 0x12C00000, TimeSpan.FromMinutes(10));
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            Task SRPServerTask = Task.Run(() => StartSRPServer());

            WebApplicationBuilder Builder = WebApplication.CreateBuilder(args);
            Builder.Services.AddControllers();
            WebApplication WebApp = Builder.Build();

            if (WebApp.Environment.IsDevelopment())
                WebApp.UseDeveloperExceptionPage();

            WebApp.MapGet("/", async context =>
            {
                context.Response.ContentType = "text/html";
                //Serve the HTML registration form
                await context.Response.SendFileAsync("register.html");
            });

            WebApp.MapPost("/register", async context =>
            {
                var Form = await context.Request.ReadFormAsync();
                var Username = Form["username"];
                var Password = Form["password"];

                var srpClient = new SrpClient();
                var salt = srpClient.GenerateSalt();
                var privateKey = srpClient.DerivePrivateKey(salt, Username, Password);
                var verifier = srpClient.DeriveVerifier(privateKey);

                User NewUser = new User
                {
                    Username = Username,
                    Salt = salt,
                    Verifier = verifier
                };

                try
                {
                    m_UserCache.AddUser(NewUser);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"status\":\"success\"}");
                }
                catch
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("{\"status\":\"error\"}");
                }
            });

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionTrapper);

            await Task.WhenAll(SRPServerTask, Task.Run(() => WebApp.RunAsync()));
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Unhandled exception:" + e.ExceptionObject.ToString());
        }

        /// <summary>
        /// Initializes the Parlo SRP server.
        /// </summary>
        private static async Task StartSRPServer()
        {
            string Salt = m_SRPClient.GenerateSalt();
            string PrivateKey = m_SRPClient.DerivePrivateKey(Salt, m_USERNAME, m_PASSWORD);
            string Verifier = m_SRPClient.DeriveVerifier(PrivateKey);

            m_NumberOfCores = ProcessorInfo.GetPhysicalCoreCount();

            m_UserCacheSemaphore = new SemaphoreSlim((m_NumberOfCores > m_NumLogicalProcessors) ?
                m_NumberOfCores : m_NumLogicalProcessors,
                (m_NumberOfCores > m_NumLogicalProcessors) ? m_NumberOfCores : m_NumLogicalProcessors);

            await m_UserCacheSemaphore.WaitAsync();
            //Add the user to the DB, this would normally be done when a client creates a new account.
            m_UserCache.AddUser(new User() { Salt = Salt, Username = m_USERNAME, Verifier = Verifier });
            m_UserCacheSemaphore.Release();

            Console.WriteLine("Parlo Docker container! \r\nListening...");

            ServerNetworkManager.Instance.OnNetworkError += ServerNetworkManager_OnNetworkError;
            ServerNetworkManager.Instance.OnPacketReceived += ServerNetworkManager_OnPacketReceived;
            //Subscribe to messages from GonzoNet so we can see if clients disconnected.
            Logger.OnMessageLogged += Logger_OnMessageLogged;

            _ = ServerNetworkManager.Instance.Listen("0.0.0.0", m_Port);
            Console.WriteLine("Using port: " + m_Port);
            string Cmd = "";

            while (true)
            {
                await Task.Yield();
            }
        }

        private static void Logger_OnMessageLogged(LogMessage Msg)
        {
            switch (Msg.Level)
            {
                //Tests the logger - messages flagged as info are "Client connected" and "Client disconnected!"
                case LogLevel.info:
                    Debug.WriteLine(Msg.Message);
                    Console.WriteLine(Msg.Message);
                    break;
                case LogLevel.error:
                    Debug.WriteLine(Msg.Message);
                    Console.WriteLine(Msg.Message);
                    break;
            }
        }

        private static async Task ServerNetworkManager_OnPacketReceived(IPacket P, byte ID, NetworkClient Sender)
        {
            AuthPacketIDs AuthID = (AuthPacketIDs)ID;
            byte[] PacketData = default;
            Packet PacketToSend = default;
            EncryptedPacket EncryptedPacketToSend = default;

            switch (AuthID)
            {
                case AuthPacketIDs.ClientInitialAuth:
                    Console.WriteLine("ClientInitialAuth: Received packet from client!");

                    ClientInitialAuth InitialAuthPacket = (ClientInitialAuth)P;
                    User U;

                    await m_UserCacheSemaphore.WaitAsync();
                        U = m_UserCache.GetUser(InitialAuthPacket.Username);
                    m_UserCacheSemaphore.Release();

                    SrpEphemeral Ephemeral = m_SRPServer.GenerateEphemeral(U.Verifier);

                    if (!m_SClientData.TryAdd(Sender, new SClientData()
                    {
                        Secret = Ephemeral.Secret,
                        PublicEphemeral = InitialAuthPacket.Ephemeral.TrimEnd('\0'),
                        Username = U.Username
                    }))
                    Console.WriteLine("ClientInitialAuth: Server couldn't add key because it already existed!");

                    ServerInitialAuthResponse ServerInitResponse = new ServerInitialAuthResponse(U.Salt, Ephemeral.Public);

                    PacketData = ServerNetworkManager.Instance.SerializePacket<ServerInitialAuthResponse>(ServerInitResponse);

                    PacketToSend = new Packet((byte)AuthPacketIDs.ServerInitialAuthResponse, PacketData, false);
                    await Sender.SendAsync(PacketToSend.BuildPacket());

                    break;
                case AuthPacketIDs.CAuthProof:
                    AuthProof CProofPacket = (AuthProof)P;

                    string ServerSecret = m_SClientData[Sender].Secret;
                    User ExistingUser;

                    await m_UserCacheSemaphore.WaitAsync();

                    ExistingUser = m_UserCache.GetUser(m_SClientData[Sender].Username);

                    m_UserCacheSemaphore.Release();

                    string PubEphemeral = m_SClientData[Sender].PublicEphemeral;
                    string Proof = CProofPacket.SessionProof.TrimEnd('\0');

                    try
                    {
                        SrpSession Session = m_SRPServer.DeriveSession(ServerSecret, PubEphemeral,
                            ExistingUser.Salt, ExistingUser.Username, ExistingUser.Verifier, Proof);

                        m_AuthenticatedClients.Add(Sender);
                        Console.WriteLine("Authenticated session: " + m_AuthenticatedClients.Count);

                        m_SClientData[Sender].EncryptionArgs = new EncryptionArgs()
                        {
                            Mode = EncryptionMode.AES,
                            Salt = ExistingUser.Salt,
                            Key = Session.Key
                        };

                        AuthProof SProofPacket = new AuthProof(Session.Proof);

                        PacketData = ServerNetworkManager.Instance.SerializePacket(SProofPacket);
                        EncryptedPacketToSend = new EncryptedPacket(m_SClientData[Sender].EncryptionArgs,
                            (byte)AuthPacketIDs.SAuthProof, PacketData);
                        await Sender.SendAsync(EncryptedPacketToSend.BuildPacket());
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Server couldn't authenticate session: \r\n" + E.ToString());
                    }


                    break;
            }
        }

        #region Error handling

        /// <summary>
        /// Server-side network error.
        /// </summary>
        /// <param name="Exception">The exception that occured.</param>
        private static void ServerNetworkManager_OnNetworkError(System.Net.Sockets.SocketException Exception)
        {
            Console.WriteLine("Network error: " + Exception.ToString());
        }

        #endregion
    }
}