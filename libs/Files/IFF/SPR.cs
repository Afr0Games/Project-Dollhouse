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
    public class SPR : IFFChunk, iSprite, IDisposable
    {
        public ushort Version1 = 0, Version2 = 0;
        public uint Version = 0;
        private List<uint> m_OffsetTable = new List<uint>();
        private uint m_PaletteID;
        public uint FrameCount = 0;
        private List<SPRFrame> m_Frames = new List<SPRFrame>();

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
            //m_Data = null; DON'T DO THIS - this data is used for decompression by GetSpriteFrame()
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

            SPRFrame Frame = new SPRFrame(Reader, m_Device, m_Parent.GetPalette((ushort)m_PaletteID), Version);
            m_Frames.Add(Frame); //Keep track of it, so it can be disposed.

            return Frame;
        }

        ~SPR()
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
                foreach(SPRFrame Frame in m_Frames)
                {
                    if (Frame != null)
                        Frame.Dispose();
                }

                // Prevent the finalizer from calling ~SPRFrame, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("SPRFrame not explicitly disposed!");
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

            bool quit = false;
            byte Clr = 0;
            Color Transparent;
            int CurrentRow = 0, CurrentColumn = 0;

            byte PixCommand, PixCount = 0;

            while (quit == false)
            {
                byte RowCommand = Reader.ReadByte();
                byte RowCount = Reader.ReadByte();

                switch (RowCommand)
                {
                    case 0x00: //Start marker; the count byte is ignored.
                        break;
                    //Fill this row with pixel data that directly follows; the count byte of the row command denotes the 
                    //size in bytes of the row and pixel data.
                    case 0x04:
                        RowCount -= 2;
                        CurrentColumn = 0;

                        while (RowCount > 0)
                        {
                            PixCommand = Reader.ReadByte();
                            PixCount = Reader.ReadByte();
                            RowCount -= 2;

                            switch (PixCommand)
                            {
                                case 1: //Leave the next pixel count pixels as transparent.
                                    for (int j = CurrentColumn; j < (CurrentColumn + PixCount); j++)
                                        SetPixel(j, CurrentRow, Color.Transparent);

                                    CurrentColumn += PixCount;

                                    break;
                                case 2: //Fill the next pixel count pixels with a palette color.
                                    //The pixel data is two bytes: the first byte denotes the palette color index, and the 
                                    //second byte is padding (which is always equal to the first byte but is ignored).
                                    Clr = Reader.ReadByte();
                                    Reader.ReadByte(); //Padding
                                    RowCount -= 2;

                                    for (int j = CurrentColumn; j < (CurrentColumn + PixCount); j++)
                                        SetPixel(j, CurrentRow, Palette[Clr]);

                                    CurrentColumn += PixCount;

                                    break;
                                case 3: //Set the next pixel count pixels to the palette color indices defined by the 
                                    //pixel data provided directly after this command.

                                    byte Padding = (byte)(PixCount % 2);

                                    if (Padding != 0)
                                        RowCount -= (byte)(PixCount + Padding);
                                    else
                                        RowCount -= PixCount;

                                    for (int j = CurrentColumn; j < (CurrentColumn + PixCount); j++)
                                        SetPixel(j, CurrentRow, Palette[Reader.ReadByte()]);

                                    CurrentColumn += PixCount;

                                    if (Padding != 0)
                                        Reader.ReadByte();

                                    break;
                            }
                        }

                        CurrentRow++;

                        break;
                    case 0x05: //End marker. The count byte is always 0, but may be ignored.

                        //Some sprites don't have these, so read them using ReadBytes(), which
                        //simply returns an empty array if the stream couldn't be read.
                        Reader.ReadBytes(2); //PixCommand and PixCount.

                        quit = true;
                        break;
                    case 0x09: //Leave the next count rows as transparent.
                        PixCommand = Reader.ReadByte();
                        PixCount = Reader.ReadByte();

                        for (int i = 0; i < RowCount; i++)
                        {
                            for (int j = CurrentColumn; j < Width; j++)
                                SetPixel(j, CurrentRow, Color.Transparent);

                            CurrentRow++;
                        }

                        break;
                    case 0x10: //Start marker, equivalent to 0x00; the count byte is ignored.
                        break;
                }
            }
        }

        /// <summary>
        /// Sets a pixel at a specified position of the texture.
        /// </summary>
        /// <param name="x">X coordinate of pixel.</param>
        /// <param name="y">Y coordinate of pixel.</param>
        /// <param name="c">The Color of the pixel.</param>
        private void SetPixel(int x, int y, Color c)
        {
            Rectangle r = new Rectangle((x < Width) ? x : x - 1, (y < Height) ? y : y - 1, 1, 1);
            Color[] color = new Color[1];
            color[0] = c;

            m_Texture.SetData<Color>(0, r, color, 0, 1);
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
