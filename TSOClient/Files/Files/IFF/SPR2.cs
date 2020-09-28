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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using log4net;

namespace Files.IFF
{
    /// <summary>
    /// This chunk type holds a number of paletted sprites that may have z-buffer and/or alpha channels.
    /// </summary>
    public class SPR2 : IFFChunk, iSprite
    {
        public uint Version = 0;
        private List<uint> m_OffsetTable = new List<uint>();
        private uint m_PaletteID;
        public uint FrameCount = 0;

        public SPR2(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            Version = Reader.ReadUInt32();
            
            if(Version == 1000)
            {
                FrameCount = Reader.ReadUInt32();
                m_PaletteID = Reader.ReadUInt32();

                for (int i = 0; i < FrameCount; i++)
                    m_OffsetTable.Add(Reader.ReadUInt32());
            }
            else
            {
                m_PaletteID = Reader.ReadUInt32();
                Reader.ReadUInt32(); //FrameCount is blank in this version.
                float Offset = 0;

                //Read the first sprite's header.
                Reader.ReadUInt32(); //SpriteVersion
                uint Size = Reader.ReadUInt32();
                Offset = Reader.Position;
                m_OffsetTable.Add((uint)Offset);
                Offset += Size;

                for (int i = 0; m_OffsetTable.Count <= 24; i++)
                {
                    Reader.ReadUInt32(); //SpriteVersion
                    Size = Reader.ReadUInt32();
                    Offset = Reader.Position;
                    Offset += Size;
                    m_OffsetTable.Add((uint)Offset);
                }
            }

            Reader.Close();
            //m_Data = null; //Don't do this here!
        }

        /// <summary>
        /// Provides basic type safety.
        /// </summary>
        /// <returns>The sprite type, either SPR or SPR2.</returns>
        public SPRType GetSPRType()
        {
            return SPRType.SPR2;
        }

        public iSpriteFrame GetFrame(int ID)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            Reader.Seek(m_OffsetTable[ID]);

            return new SPR2Frame(Reader, m_Device, m_Parent.GetPalette((ushort)m_PaletteID), Version);
        }
    }

    /// <summary>
    /// A frame stored by a SPR2 instance.
    /// </summary>
    public class SPR2Frame : IDisposable, iSpriteFrame
    {
        public ushort Width, Height;

        /// <summary>
        /// A 2-byte unsigned integer specifying the color index in this sprite's color palette 
        /// that was used for pixels that are now fully transparent; furthermore, any pixels in 
        /// the color channel defined with this color that were not given an alpha value must be 
        /// assigned an opacity of zero and a z-buffer value of 255 (if the z-buffer channel is specified).
        /// </summary>
        public Color TransparentColor;

        /// <summary>
        /// A 2-byte signed integer specifying the offset to add to the X direction 
        /// in order to align this sprite correctly, in the case of multi-tile objects.
        /// </summary>
        public ushort XLocation { get; set; }

        /// <summary>
        /// A 2-byte signed integer specifying the offset to add to the Y direction 
        /// in order to align this sprite correctly, in the case of multi-tile objects.
        /// </summary>
        public ushort YLocation { get; set; }

        private Texture2D m_Texture;
        public byte[,] ZBuffer; //Stores the z-value for each pixel (https://www.computerhope.com/jargon/z/zbuffering.htm)

        public bool HasZBuffer, HasAlphaChannel;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The texture for this sprite.
        /// </summary>
        /// <returns>A Texture2D instance. Will be null if this SPR2 has been initialized using GDI+.</returns>
        public Texture2D GetTexture()
        {
            return m_Texture;
        }

        public SPR2Frame(FileReader Reader, GraphicsDevice Device, PALT Palette, uint SpriteVersion)
        {
            if(SpriteVersion == 1001)
            {
                Reader.ReadUInt32(); //Version
                Reader.ReadUInt32(); //Size
            }

            Width = Reader.ReadUShort();
            Height = Reader.ReadUShort();

            m_Texture = new Texture2D(Device, Width, Height);

            ZBuffer = new byte[Width, Height];

            uint Flags = Reader.ReadUInt32();
            Reader.ReadUShort(); //Palette.

            TransparentColor = Palette[Reader.ReadUShort()];

            HasZBuffer = (Flags & 0x02) == 0x02;
            HasAlphaChannel = (Flags & 0x04) == 0x04;

            YLocation = Reader.ReadUShort();
            XLocation = Reader.ReadUShort();

            bool EndMarker = false;

            int CurrentRow = 0, CurrentColumn = 0;
            int Padding = 0;
            Color Clr; //The current color.

            while (!EndMarker)
            {
                ushort Marker = Reader.ReadUShort();
                int Command = Marker >> 13;
                int Count = Marker & 0x1FFF;

                switch(Command)
                {
                    //Fill this row with pixel data that directly follows; the count byte of the row command denotes 
                    //the size in bytes of the row's command/count bytes together with the supplied pixel data. In 
                    //the pixel data, each pixel command consists of a 3-bit/13-bit command/count header followed by a 
                    //block of pixel data padded to a multiple of 2 bytes. If the row is not filled completely, the 
                    //remainder is transparent. The pixel commands are:
                    case 0x00:
                        Count -= 2; //Row command + count bytes.

                        while(Count > 0)
                        {
                            ushort PxMarker = Reader.ReadUShort();
                            var PxCommand = PxMarker >> 13;
                            var PxCount = PxMarker & 0x1FFF;
                            Count -= 2;

                            switch (PxCommand)
                            {
                                //Set the next pixel count pixels in the z-buffer and color channels to the values defined 
                                //by the pixel data provided directly after this command. Every group of 2 bytes in the pixel 
                                //data provides a luminosity (z-buffer) or color index (color) value to be copied to the row 
                                //for the z-buffer channel and color channel, respectively, in that order, using the full 
                                //opacity value of 255 for each pixel that is not the transparent color.
                                case 0x01:
                                    Count -= PxCount * 2;

                                    while (PxCount > 0)
                                    {
                                        ZBuffer[CurrentColumn, CurrentRow] = Reader.ReadByte();

                                        Clr = Palette[Reader.ReadByte()];
                                        if (Clr != Color.Transparent)
                                            SetPixel(CurrentColumn, CurrentRow, Clr);
                                        else
                                            SetPixel(CurrentColumn, CurrentRow, Color.Transparent);

                                        PxCount--;
                                        CurrentColumn++;
                                    }

                                    break;
                                    //Set the next pixel count pixels in the z-buffer, color, and alpha channels to the values 
                                    //defined by the pixel data provided directly after this command. Every group of 3 bytes in 
                                    //the pixel data, minus the padding byte at the very end (if it exists), provides a luminosity 
                                    //(z-buffer and alpha) or color index (color) value to be copied to the row for the z-buffer, 
                                    //color, and alpha channels, respectively, in that order. The alpha channel data is grayscale 
                                    //in the range 0-31, and the z buffer is in range 0-255.
                                case 0x02:
                                    Padding = PxCount % 2;
                                    Count -= (PxCount * 3) + Padding;

                                    while (PxCount > 0)
                                    {
                                        ZBuffer[CurrentColumn, CurrentRow] = Reader.ReadByte();
                                        Clr = Palette[Reader.ReadByte()];

                                        //Read the alpha.
                                        Clr.A = Reader.ReadByte();

                                        SetPixel(CurrentColumn, CurrentRow, Clr);

                                        PxCount--;
                                        CurrentColumn++;
                                    }

                                    if (Padding != 0)
                                        Reader.ReadByte();
                                    break;
                                //Leave the next pixel count pixels in the color channel filled with the transparent color, 
                                //in the z-buffer channel filled with 255, and in the alpha channel filled with 0. This pixel 
                                //command has no pixel data.
                                case 0x03:
                                    while (PxCount > 0)
                                    {
                                        //This is completely transparent regardless of whether the frame
                                        //supports alpha.
                                        SetPixel(CurrentColumn, CurrentRow, new Color(0, 0, 0, 0));

                                        if (HasZBuffer)
                                            ZBuffer[CurrentColumn, CurrentRow] = 255;

                                        PxCount--;
                                        CurrentColumn++;
                                    }
                                    break;
                                //Set the next pixel count pixels in the color channel to the palette color indices defined by 
                                //the pixel data provided directly after this command. Every byte in the pixel data, minus the 
                                //padding byte at the very end(if it exists), provides a color index value to be copied to the 
                                //row for the color channel using the full opacity value of 255 and the closest z-buffer value 
                                //of 0 if the pixel is not the transparent color, or otherwise the no opacity value of 0 and the 
                                //farthest z-buffer value of 255.
                                case 0x06:
                                    Padding = PxCount % 2;
                                    Count -= PxCount + Padding;

                                    while (PxCount > 0)
                                    {
                                        Clr = Palette[Reader.ReadByte()];
                                        if (Clr != Color.Transparent)
                                            SetPixel(CurrentColumn, CurrentRow, Clr);
                                        else
                                            SetPixel(CurrentColumn, CurrentRow, Color.Transparent);

                                        /*if (HasZBuffer)
                                        {
                                            if (Clr != Color.Transparent)
                                                Frame.ZBuffer.SetPixel(new Point(CurrentColumn, CurrentRow),
                                                    Color.FromArgb(255, 1, 1, 1));
                                            else
                                                Frame.BitmapData.SetPixel(new Point(CurrentColumn, CurrentRow),
                                                    Color.FromArgb(255, 255, 255, 255));
                                        }*/

                                        PxCount--;
                                        CurrentColumn++;
                                    }

                                    if (Padding != 0)
                                        Reader.ReadByte();
                                    break;
                            }
                        }

                        CurrentRow++;
                        CurrentColumn = 0;

                        break;
                    //Leave the next count rows in the color channel filled with the transparent color, 
                    //in the z-buffer channel filled with 255, and in the alpha channel filled with 0.
                    case 0x04:
                        for (int i = 0; i < Count; i++)
                        {
                            for (int j = 0; j < Width - 1; j++)
                            {
                                SetPixel(CurrentColumn, CurrentRow, new Color(0, 0, 0, 0));

                                if (HasZBuffer)
                                    ZBuffer[CurrentColumn, CurrentRow] = 255;

                                CurrentColumn++;
                            }

                            CurrentColumn = 0;
                            CurrentRow++;
                        }

                        CurrentColumn = 0;
                        break;
                    case 0x05:
                        EndMarker = true;
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
            Rectangle r = new Rectangle((x < Width) ? x : x-1, (y < Height) ? y : y - 1, 1, 1);
            Color[] color = new Color[1];
            color[0] = c;

            m_Texture.SetData<Color>(0, r, color, 0, 1);
        }

        ~SPR2Frame()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this SPR2Frame instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this SPR2Frame instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Texture != null)
                    m_Texture.Dispose();

                // Prevent the finalizer from calling ~SPR2Frame, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("SPR2Frame not explicitly disposed!");
        }
    }
}
