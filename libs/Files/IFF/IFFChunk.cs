/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Files.IFF
{
    public enum IFFChunkTypes : int
    {
        BMP_ = 0,
        FBMP = 1,
        FWAV = 2,
        BCON = 3,
        DGRP = 4,
        OBJD = 5,
        BHAV = 6,
        CST = 7,
        CTSS = 8,
        GLOB = 9,
        FCNS = 10,
        TTAs = 11,
        TTAB = 12,
        TPRP = 13,
        STR =  14,
        OBJf = 15,
        SLOT = 16,
        SPR = 17,
        SPR2 = 18,
        PALT = 19,
        XXXX = 20, //WTF?!
        rsmp = 21
    }

    public class IFFChunk
    {
        protected Iff m_Parent;
        protected GraphicsDevice m_Device;
        public IFFChunkTypes Type;
        public uint Size;
        public ushort ID;
        protected byte[] m_Data;

#if DEBUG
        public string NameString = "";
#endif

        public IFFChunk(FileReader Reader, GraphicsDevice Device, Iff Parent)
        {
            m_Parent = Parent;
            m_Device = Device;

            ReadHeader(Reader);
        }

        public IFFChunk(FileReader Reader, Iff Parent)
        {
            m_Parent = Parent;
            ReadHeader(Reader);
        }

        public IFFChunk(IFFChunk BaseChunk)
        {
            m_Parent = BaseChunk.m_Parent;
            m_Device = BaseChunk.m_Parent.Device;
            m_Data = BaseChunk.m_Data;
            Size = BaseChunk.Size;
            ID = BaseChunk.ID;
            Type = BaseChunk.Type;
        }

        private void ReadHeader(FileReader Reader)
        {
            if (Debugger.IsAttached)
                Type = (IFFChunkTypes)Enum.Parse(typeof(IFFChunkTypes), Reader.ReadString(4).Replace("#", "").Replace("\0", ""));
            else
            {
                NameString = Reader.ReadString(4);
                Type = (IFFChunkTypes)Enum.Parse(typeof(IFFChunkTypes), NameString.Replace("#", "").Replace("\0", ""));
            }
            
            Size = Reader.ReadUInt32();
            ID = Reader.ReadUShort();
            Reader.ReadUShort();  //Flags
            Reader.ReadBytes(64); //Label

            m_Data = Reader.ReadBytes((int)(Size - 76));

            if (!Endian.IsBigEndian)
                Array.Reverse(m_Data); //Data is Little Endian, so needs to be reversed.
        }
    }
}
