/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

namespace Files.Vitaboy
{
    public class BoneBinding
    {
        public uint BoneIndex;
        public uint FirstRealVertexIndex;
        public uint RealVertexCount;
        public uint FirstBlendVertexIndex;
        public uint BlendVertexCount;

        public BoneBinding(FileReader Reader)
        {
            BoneIndex = Reader.ReadUInt32();
            FirstRealVertexIndex = Reader.ReadUInt32();
            RealVertexCount = Reader.ReadUInt32();
            FirstBlendVertexIndex = Reader.ReadUInt32();
            BlendVertexCount = Reader.ReadUInt32();
        }
    }
}
