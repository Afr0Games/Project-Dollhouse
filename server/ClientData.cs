using Parlo.Encryption;
using SecureRemotePassword;

namespace Parlo.Docker
{
    internal class ClientData
    {
        public ClientData(SrpClient C, SrpEphemeral Ephemeral)
        {
            Client = C;
            ClientEphemeral = Ephemeral;
        }

        public EncryptionArgs EncryptionArgs { get; set; }

        public SrpSession Session { get; set; }
        public string Username { get; set; }

        public SrpEphemeral ClientEphemeral { get; }

        public SrpClient Client { get; }

        public override bool Equals(Object Obj)
        {
            //Check for null and compare run-time types.
            if ((Obj == null) || !this.GetType().Equals(Obj.GetType()))
                return false;
            else
            {
                ClientData C = (ClientData)Obj;

                //When the client instantiates a new ClientData class, it adds these to it. Therefore, compare them.
                return (ClientEphemeral == C.ClientEphemeral) && (Client == C.Client);
            }
        }
    }

    /// <summary>
    /// Clientdata useed by clients that are handled by the server.
    /// </summary>
    internal class SClientData
    {
        public string Secret { get; set; }
        public string PublicEphemeral { get; set; }
        public SrpSession Session { get; set; }
        public string Username { get; set; }

        public EncryptionArgs EncryptionArgs { get; set; }

        public override bool Equals(Object Obj)
        {
            //Check for null and compare run-time types.
            if ((Obj == null) || !this.GetType().Equals(Obj.GetType()))
                return false;
            else
            {
                SClientData C = (SClientData)Obj;

                //When the server instantiates a new SClientData class, it adds these to it. Therefore, compare them.
                return (Secret == C.Secret) && (PublicEphemeral == C.PublicEphemeral) && (Username == C.Username);
            }
        }
    }
}