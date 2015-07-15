/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Iffinator.Iff
{
    public class IffChunk
    {
        public long Offset;
        public string Type;
        public long Size;
        public short TypeNum;
        public short ID;
        public char[] Label; //Always padded to 64 bytes!
        public byte[] Data; //Equal to Size - 76 (header is 76 bytes).
    }
}
