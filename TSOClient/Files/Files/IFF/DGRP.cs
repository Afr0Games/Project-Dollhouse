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
using System.Text;
using System.IO;

namespace Files.IFF
{
    /// <summary>
    /// This chunk type collects SPR# and SPR2 resources into a "drawing group" which can be used to display one tile of 
    /// an object from all directions and zoom levels. Objects which span across multiple tiles have a separate DGRP 
    /// chunk for each tile. A DGRP chunk always consists of 12 images (one for every direction/zoom level combination), 
    /// which in turn contain info about one or more sprites.
    /// </summary>
    public class DGRP : IFFChunk
    {
        private ushort m_Version;
        public uint ImageCount;
        public List<DGRPImg> Images = new List<DGRPImg>();

        public DGRP(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            m_Version = Reader.ReadUShort();

            switch(m_Version)
            {
                case 20000:
                    ImageCount = Reader.ReadUShort();
                    break;
                case 20001:
                    ImageCount = Reader.ReadUShort();
                    break;
                case 20003:
                    ImageCount = Reader.ReadUInt32();
                    break;
                case 20004:
                    ImageCount = Reader.ReadUInt32();
                    break;
            }

            for(int i = 0; i < ImageCount; i++)
            {
                Images.Add(new DGRPImg(Reader, m_Version));
            }

            Reader.Close();
            m_Data = null;
        }
    }

    public class DGRPImg
    {
        public uint SpriteCount;
        public uint DirectionFlags;
        public uint ZoomLevel;
        public List<SpriteInfo> Info = new List<SpriteInfo>();

        public DGRPImg(FileReader Reader, uint Version)
        {
            if (Version == 20000 || Version == 20001)
            {
                SpriteCount = Reader.ReadUShort();
                DirectionFlags = Reader.ReadByte();
                ZoomLevel = Reader.ReadByte();

                for (int i = 0; i < SpriteCount; i++)
                    Info.Add(new SpriteInfo(Reader, Version));
            }
            else
            {
                DirectionFlags = Reader.ReadUInt32();
                ZoomLevel = Reader.ReadUInt32();
                SpriteCount = Reader.ReadUInt32();

                for (int i = 0; i < SpriteCount; i++)
                    Info.Add(new SpriteInfo(Reader, Version));
            }
        }
    }

    public class SpriteInfo
    {
        public short Type = 0;
        public int SPRChunkID;
        public int SPRFrameNum;
        public int Flags = 0;
        public int SpriteXOffset, SpriteYOffset;
        public float ObjectXOffset, ObjectYOffset, ObjectZOffset;

        public SpriteInfo(FileReader Reader, uint Version)
        {
            if(Version == 20000 || Version == 20001)
            {
                Type = Reader.ReadInt16();
                SPRChunkID = Reader.ReadInt16();
                SPRFrameNum = Reader.ReadInt16();
                Flags = Reader.ReadInt16();
                SpriteXOffset = Reader.ReadInt16();
                SpriteYOffset = Reader.ReadInt16();
                ObjectZOffset = Reader.ReadFloat();
            }
            else
            {
                SPRChunkID = Reader.ReadInt32();
                SPRFrameNum = Reader.ReadInt32();
                SpriteXOffset = Reader.ReadInt32();
                SpriteYOffset = Reader.ReadInt32();
                ObjectZOffset = Reader.ReadFloat();
                Flags = Reader.ReadInt32();

                if (Version == 20004)
                {
                    ObjectXOffset = Reader.ReadFloat();
                    ObjectYOffset = Reader.ReadFloat();
                }
            }
        }
    }
}
