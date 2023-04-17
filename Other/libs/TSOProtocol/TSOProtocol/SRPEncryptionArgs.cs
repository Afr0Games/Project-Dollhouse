using GonzoNet.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSOProtocol
{
    /// <summary>
    /// Represents arguments needed to encrypt a packet
    /// based on an established SRP session.
    /// </summary>
    public class SRPEncryptionArgs
    {
        public EncryptionMode Mode;
        public string Session;
        public string Salt;
    }
}
