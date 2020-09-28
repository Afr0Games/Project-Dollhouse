/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Files.IFF
{
    /// <summary>
    /// This chunk type holds a number of paletted sprites that share a common color palette and lack z-buffers and alpha buffers. 
    /// SPR# chunks can be either big-endian or little-endian, which must be determined by comparing the first two bytes to zero 
    /// (since no version number uses more than two bytes).
    /// </summary>
    public class SPR : IFFChunk, iSprite
    {
        public ushort Version1 = 0, Version2 = 0;
        public uint Version = 0;
        private List<uint> m_OffsetTable = new List<uint>();
        private uint m_PaletteID;
        public uint FrameCount = 0;

        public SPR(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            Version1 = Reader.ReadUShort();
            Version2 = Reader.ReadUShort();

            if(Version1 == 0)
            {
                Version = Version2;
                Reader = new FileReader(new MemoryStream(Reader.ReadToEnd()), true);
            }
            else
                Version = Version1;

            FrameCount = Reader.ReadUInt32();
            m_PaletteID = Reader.ReadUInt32();

            if (Version >= 502 && Version <= 505)
            {
                //TODO: Should this be stored?
                for (int i = 0; i < FrameCount; i++)
                    m_OffsetTable.Add(Reader.ReadUInt32());
            }
            else
                m_OffsetTable.Add((uint)Reader.Position);

            Reader.Close();
            //m_Data = null; //DON'T DO THIS!
        }

        /// <summary>
        /// Provides basic type safety.
        /// </summary>
        /// <returns>The sprite type, either SPR or SPR2.</returns>
        public SPRType GetSPRType()
        {
            return SPRType.SPR;
        }

        public iSpriteFrame GetFrame(int ID)
        {
            FileReader Reader;

            if (Version1 == 0)
                Reader = new FileReader(new MemoryStream(m_Data), true);
            else
                Reader = new FileReader(new MemoryStream(m_Data), false);

            Reader.Seek(m_OffsetTable[ID]);

            return new SPRFrame(Reader, m_Device, m_Parent.GetPalette((ushort)m_PaletteID), Version);
        }
    }

    /// <summary>
    /// Represents a compressed sprite stored in a SPR# chunk.
    /// </summary>
    public class SPRFrame : IDisposable, iSpriteFrame
    {
        public uint Version = 0;
        public uint Size = 0;
        public ushort Height = 0, Width = 0;
        private Texture2D m_Texture;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// A 2-byte signed integer specifying the offset to add to the X direction 
        /// in order to align this sprite correctly, in the case of multi-tile objects.
        /// SPR does not support multi tile objects, so this is always 0.
        /// </summary>
        public ushort XLocation { get; set; }

        /// <summary>
        /// A 2-byte signed integer specifying the offset to add to the Y direction 
        /// in order to align this sprite correctly, in the case of multi-tile objects.
        /// SPR does not support multi tile objects, so this is always 0.
        /// </summary>
        public ushort YLocation { get; set; }

        /// <summary>
        /// This sprite as a texture.
        /// </summary>
        /// <returns>The texture - will be null if this SPR has been initialied using GDI+.</returns>
        public Texture2D GetTexture()
        {
            return m_Texture;
        }

        public SPRFrame(FileReader Reader, GraphicsDevice Device, PALT Palette, uint SPRVersion)
        {
            if (Version == 1001)
            {
                Version = Reader.ReadUInt32();
                Size = Reader.ReadUInt32();
            }

            Reader.ReadUInt32(); //Reserved
            Height = Reader.ReadUShort();
            Width = Reader.ReadUShort();

            m_Texture = new Texture2D(Device, Width, Height);

            for (ushort i = 0; i < Height; i++)
            {
                byte Cmd = Reader.ReadByte();
                byte Count = Reader.ReadByte();

                switch (Cmd)
                {
                    case 0x04:
                        for (byte j = 0; j < Count; j++)
                        {
                            byte PxCmd = Reader.ReadByte();
                            byte PxCount = Reader.ReadByte();
                            Color[] Pixels;

                            switch (PxCmd)
                            {
                                case 0x01:
                                    //Leave the next pixel count pixels as transparent. This pixel command has no pixel data.
                                    Pixels = new Color[Count];
                                    for (int k = 0; k < Count; k++)
                                        Pixels[k] = Color.Transparent;

                                    m_Texture.SetData(Pixels, 0, Count);
                                    break;
                                case 0x02:
                                    //Fill the next pixel count pixels with a single palette color. 
                                    //The pixel data is two bytes: the first byte denotes the palette color 
                                    //index, and the second byte is padding (which is always equal to the 
                                    //first byte but is ignored).
                                    Pixels = new Color[Count];
                                    byte ColorIndex = Reader.ReadByte();

                                    for (int k = 0; k < Count; k++)
                                        Pixels[k] = Palette[ColorIndex];

                                    m_Texture.SetData(Pixels, 0, Count);
                                    break;
                                case 0x03:
                                    //Set the next pixel count pixels to the palette color indices defined by 
                                    //the pixel data provided directly after this command. Each byte in the pixel data, 
                                    //minus the padding byte at the very end (if it exists), is a color index value to 
                                    //be copied to the row.
                                    Pixels = new Color[Count];

                                    for (int k = 0; k < Count; k++)
                                        Pixels[k] = Palette[Reader.ReadByte()];

                                    m_Texture.SetData(Pixels, 0, Count);
                                    break;
                                case 0x09:
                                    //Leave the next count rows as transparent.
                                    for (int k = 0; k < Count; k++)
                                    {
                                        Pixels = new Color[Width];
                                        for (int l = 0; l < Width; l++)
                                            Pixels[l] = Color.Transparent;

                                        m_Texture.SetData(Pixels, 0, Width);
                                    }
                                    break;
                            }
                        }

                        break;
                }
            }
        }

        ~SPRFrame()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this SPRFrame instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this SPRFrame instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Texture != null)
                    m_Texture.Dispose();

                // Prevent the finalizer from calling ~SPRFrame, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("SPRFrame not explicitly disposed!");
        }
    }
}
