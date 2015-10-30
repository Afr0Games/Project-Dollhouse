using System;
using System.Collections.Generic;
using System.Text;
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
            m_Data = BaseChunk.m_Data;
            Size = BaseChunk.Size;
            ID = BaseChunk.ID;
            Type = BaseChunk.Type;
        }

        private void ReadHeader(FileReader Reader)
        {
            Type = (IFFChunkTypes)Enum.Parse(typeof(IFFChunkTypes), Reader.ReadString(4).Replace("#", "").Replace("\0", ""));
            Size = Reader.ReadUInt32();
            ID = Reader.ReadUShort();
            Reader.ReadUShort();  //Flags
            Reader.ReadBytes(64); //Label
            //System.Diagnostics.Debug.WriteLine(Reader.ReadString(64));

            m_Data = Reader.ReadBytes((int)(Size - 76));

            if (!Endian.IsBigEndian)
                Array.Reverse(m_Data); //Data is Little Endian, so needs to be reversed.
        }
    }
}
