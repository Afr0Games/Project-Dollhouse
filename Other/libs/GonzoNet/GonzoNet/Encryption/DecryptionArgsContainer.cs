/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace GonzoNet.Encryption
{
    public class ARC4DecryptionArgs
    {
        public ICryptoTransform Transformer;
    }

    public class AESDecryptionArgs
    {
        public byte[] IV;
        public byte[] NOnce;
		public byte[] Challenge;
        public ECDiffieHellmanCng PrivateKey;
    }

    public class DecryptionArgsContainer
    {
        public ushort UnencryptedLength;

        public ARC4DecryptionArgs ARC4DecryptArgs;
        public AESDecryptionArgs AESDecryptArgs;
    }
}
