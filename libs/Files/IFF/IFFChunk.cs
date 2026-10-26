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
            byte[] name = Reader.ReadBytes(4);
            if (name.Length != 4) 
                throw new EndOfStreamException("Truncated IFF chunk type");

            m_RawType = Encoding.ASCII.GetString(name);
            m_NameString = m_RawType;
            string key = m_RawType.Replace("#", "").TrimEnd('\0');

            if (!Enum.TryParse(key, out IFFChunkTypes type) || !Enum.IsDefined(typeof(IFFChunkTypes), type))
                type = IFFChunkTypes.Unknown;

            Type = type;
            Size = Reader.ReadUInt32();

            if (Size < 76 || (long)Size - 76 > Reader.StreamLength - Reader.Position - 68)
                throw new InvalidDataException("Invalid or truncated IFF chunk size for " + m_RawType);

            ID = Reader.ReadUShort();
            m_OriginalID = ID;
            m_OriginalType = Type;
            m_Flags = Reader.ReadUShort();
            m_Label = Reader.ReadBytes(64);

            if (m_Label.Length != 64) 
                throw new EndOfStreamException("Truncated IFF label");

            m_Data = Reader.ReadBytes(checked((int)Size - 76));

            if (m_Data.Length != Size - 76) 
                throw new EndOfStreamException("Truncated IFF payload");
        }

            m_Data = Reader.ReadBytes((int)(Size - 76));

            if (!Endian.IsBigEndian)
                Array.Reverse(m_Data); //Data is Little Endian, so needs to be reversed.
        /// <summary>Writes the header followed by the correctly sized, unswapped payload.</summary>
        public virtual Stream ToStream()
        {
            byte[] payload = GetPayloadForWriting();
            var output = new MemoryStream();
            var writer = new BinaryWriter(output, Encoding.ASCII, true);
            string typeCode = m_RawType;
            if (Type != m_OriginalType)
            {
                if (Type == IFFChunkTypes.Unknown)
                    throw new InvalidOperationException("Use the original raw type for unknown chunks");
                typeCode = Type == IFFChunkTypes.STR ? "STR#" :
                           Type == IFFChunkTypes.SPR ? "SPR#" :
                           Type == IFFChunkTypes.CST ? "CST\0" : Type.ToString();
                if (typeCode.Length != 4)
                    throw new InvalidOperationException("Chunk type must encode as four ASCII bytes");
            }
            writer.Write(Encoding.ASCII.GetBytes(typeCode));
            uint chunkSize = checked((uint)payload.Length + 76U);
            // IFF container headers use big-endian fields.
            writer.Write(new byte[] { (byte)(chunkSize >> 24), (byte)(chunkSize >> 16), (byte)(chunkSize >> 8), (byte)chunkSize });
            writer.Write((byte)(ID >> 8)); writer.Write((byte)ID);
            writer.Write((byte)(m_Flags >> 8)); writer.Write((byte)m_Flags);
            writer.Write(m_Label);
            writer.Write(payload);
            writer.Flush();
            output.Position = 0;
            return output;
        }
    }
}
