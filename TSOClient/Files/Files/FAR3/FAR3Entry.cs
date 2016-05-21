/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the SimsLib.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.FAR3
{
    public enum FAR3TypeIDs
    {
        BMP = 1,
        TGA = 2,
        SKEL = 5,
        ANIM = 7,
        MESH = 9,
        BND = 11,
        APR = 12,
        OFT = 13,
        PNG = 14,
        PO = 15,
        COL = 16,
        HAG = 18,
        JPG = 20,
        PackedPNG = 24 //Compressed
    }

    public class FAR3Entry
    {
        public uint DecompressedDataSize;
        public uint CompressedDataSize;
        public uint DataOffset;
        public ushort Flags;
        public ushort FileNameLength;
        public uint TypeID;
        public uint FileID;
        public string Filename;
    }
}
