﻿/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the TSOClient.

The Initial Developer of the Original Code is
ddfczm. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TSO.Files.utils;

namespace TSO.Files.formats.iff.chunks
{
    public class OBJf : IffChunk
    {
        public OBJfFunctionEntry[] functions;
        public uint Version;

        public override void Read(Iff iff, Stream stream)
        {
            using (var io = IoBuffer.FromStream(stream, ByteOrder.LITTLE_ENDIAN))
            {
                io.ReadUInt32(); //pad
                Version = io.ReadUInt32();
                string magic = io.ReadCString(4);
                functions = new OBJfFunctionEntry[io.ReadUInt32()];
                for (int i=0; i<functions.Length; i++) {
                    var result = new OBJfFunctionEntry();
                    result.ConditionFunction = io.ReadUInt16();
                    result.ActionFunction = io.ReadUInt16();
                    functions[i] = result;
                }
            }
        }
    }

    public struct OBJfFunctionEntry {
        public ushort ConditionFunction;
        public ushort ActionFunction;
    }
}
