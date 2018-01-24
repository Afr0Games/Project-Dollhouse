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
    public class SPR2 : IFFChunk
    {
        public uint Version = 0;
        private List<uint> m_OffsetTable = new List<uint>();
        private uint m_PaletteID;

        /// <summary>
        /// Gets a specific sprite.
        /// </summary>
        /// <param name="Key">The ID of the sprite to get.</param>
        /// <returns>A SPRFrame instance.</returns>
        public SPR2Frame this[int Key]
        {
            get
            {
                if (Version == 1000)
                    return GetSprite(Key, m_Parent.GetPalette((ushort)m_PaletteID));
                else
                    return GetSprite(0, m_Parent.GetPalette((ushort)m_PaletteID));
            }
        }

        public SPR2(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            Version = Reader.ReadUInt32();
            uint SpriteCount = 0;
            
            if(Version == 1000)
            {
                SpriteCount = Reader.ReadUInt32();
                m_PaletteID = Reader.ReadUInt32();

                for (int i = 0; i < SpriteCount; i++)
                    m_OffsetTable.Add(Reader.ReadUInt32());
            }
            else
            {
                m_PaletteID = Reader.ReadUInt32();
                SpriteCount = Reader.ReadUInt32();

                m_OffsetTable.Add((uint)Reader.Position);
            }

            Reader.Close();
            //m_Data = null; //Don't do this here!
        }

        private SPR2Frame GetSprite(int ID, PALT Palette)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            Reader.Seek(m_OffsetTable[ID]);

            return new SPR2Frame(Reader, m_Device, Palette, Version);
        }
    }

    public class SPR2Frame : IDisposable
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
        /// A 2-byte signed integer specifying the offset to add to the Y direction 
        /// in order to align this sprite correctly, in the case of multi-tile objects.
        /// </summary>
        public ushort XLocation;

        /// <summary>
        /// A 2-byte signed integer specifying the offset to add to the Y direction 
        /// in order to align this sprite correctly, in the case of multi-tile objects.
        /// </summary>
        public ushort YLocation;

        public Texture2D Texture;
        public byte[] ZBuffer;

        public bool HasZBuffer, HasAlphaChannel;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SPR2Frame(FileReader Reader, GraphicsDevice Device, PALT Palette, uint SpriteVersion)
        {
            if(SpriteVersion == 1001)
            {
                Reader.ReadUInt32(); //Version
                Reader.ReadUInt32(); //Size
            }

            Width = Reader.ReadUShort();
            Height = Reader.ReadUShort();
            Texture = new Texture2D(Device, Width, Height);
            ZBuffer = new byte[Width * Height];

            uint Flags = Reader.ReadUInt32();
            Reader.ReadUShort(); //Palette.

            TransparentColor = Palette[Reader.ReadUShort()];

            HasZBuffer = (Flags & 0x02) == 0x02;
            HasAlphaChannel = (Flags & 0x04) == 0x04;

            XLocation = Reader.ReadUShort();
            YLocation = Reader.ReadUShort();

            bool EndMarker = false;

            while(!EndMarker)
            {
                ushort Marker = Reader.ReadUShort();
                var Command = Marker >> 13;
                var Count = Marker & 0x1FFF;

                switch(Command)
                {
                    //Fill this row with pixel data that directly follows; the count byte of the row command denotes 
                    //the size in bytes of the row's command/count bytes together with the supplied pixel data. In 
                    //the pixel data, each pixel command consists of a 3-bit/13-bit command/count header followed by a 
                    //block of pixel data padded to a multiple of 2 bytes. If the row is not filled completely, the 
                    //remainder is transparent. The pixel commands are:
                    case 0x00:
                        for(int i = 0; i < Count; i++)
                        {
                            ushort PxMarker = Reader.ReadUShort();
                            var PxCommand = PxMarker >> 13;
                            var PxCount = PxMarker & 0x1FFF;

                            Color[] Colors;

                            switch(PxCommand)
                            {
                                //Set the next pixel count pixels in the z-buffer and color channels to the values defined 
                                //by the pixel data provided directly after this command. Every group of 2 bytes in the pixel 
                                //data provides a luminosity (z-buffer) or color index (color) value to be copied to the row 
                                //for the z-buffer channel and color channel, respectively, in that order, using the full 
                                //opacity value of 255 for each pixel that is not the transparent color.
                                case 0x01:
                                    Colors = new Color[PxCount];

                                    for(int j = 0; j < PxCount; j++)
                                    {
                                        byte Luminosity = Reader.ReadByte();
                                        byte ColorIndex = Reader.ReadByte();
                                        Colors[j] = Palette[ColorIndex];
                                        ZBuffer[j] = Luminosity;
                                    }
                                    Texture.SetData<Color>(Colors, 0, Colors.Length);
                                    break;
                                    //Set the next pixel count pixels in the z-buffer, color, and alpha channels to the values 
                                    //defined by the pixel data provided directly after this command. Every group of 3 bytes in 
                                    //the pixel data, minus the padding byte at the very end (if it exists), provides a luminosity 
                                    //(z-buffer and alpha) or color index (color) value to be copied to the row for the z-buffer, 
                                    //color, and alpha channels, respectively, in that order. The alpha channel data is grayscale 
                                    //in the range 0-31, and the z buffer is in range 0-255.
                                case 0x02:
                                    Colors = new Color[PxCount];

                                    for (int j = 0; j < PxCount; j++)
                                    {
                                        byte Luminosity = Reader.ReadByte();
                                        byte ColorIndex = Reader.ReadByte();
                                        byte Alpha = (byte)(Reader.ReadByte() * 8.2258064516129032258064516129032);
                                        Colors[j] = Palette[ColorIndex];
                                        Colors[j].A = Alpha;
                                        ZBuffer[j] = Luminosity;
                                    }
                                    Texture.SetData<Color>(Colors, 0, Colors.Length);
                                    break;
                                //Leave the next pixel count pixels in the color channel filled with the transparent color, 
                                //in the z-buffer channel filled with 255, and in the alpha channel filled with 0. This pixel 
                                //command has no pixel data.
                                case 0x03:
                                    Colors = new Color[PxCount];

                                    for (int j = 0; j < PxCount; j++)
                                    {
                                        Colors[j] = Color.Transparent;
                                        Colors[j].A = 0;
                                        ZBuffer[j] = 255;
                                    }
                                    Texture.SetData<Color>(Colors, 0, Colors.Length);
                                    break;
                                //Set the next pixel count pixels in the color channel to the palette color indices defined by 
                                //the pixel data provided directly after this command.Every byte in the pixel data, minus the 
                                //padding byte at the very end(if it exists), provides a color index value to be copied to the 
                                //row for the color channel using the full opacity value of 255 and the closest z-buffer value 
                                //of 0 if the pixel is not the transparent color, or otherwise the no opacity value of 0 and the 
                                //farthest z-buffer value of 255.
                                case 0x06:
                                    Colors = new Color[PxCount];

                                    for (int j = 0; j < PxCount; j++)
                                    {
                                        byte ColorIndex = Reader.ReadByte();
                                        Colors[j] = Palette[ColorIndex];

                                        Colors[j].A = (Palette[ColorIndex] != Color.Transparent) ? (byte)255 : (byte)0;
                                        ZBuffer[j] = (Palette[ColorIndex] != Color.Transparent) ? (byte)0 : (byte)255;
                                    }
                                    Texture.SetData<Color>(Colors, 0, Colors.Length);
                                    break;
                            }
                        }
                        break;
                    //Leave the next count rows in the color channel filled with the transparent color, 
                    //in the z-buffer channel filled with 255, and in the alpha channel filled with 0.
                    case 0x04:
                        for (int j = 0; j < Count; j++)
                        {
                            Color[] Colors = new Color[Width];

                            for (int k = 0; k < Width; k++)
                            {
                                Colors[k] = Color.Transparent;
                                Colors[k].A = 0;
                                ZBuffer[k] = 255;
                            }

                            Texture.SetData<Color>(Colors, 0, Colors.Length);
                        }
                        break;
                    case 0x05:
                        EndMarker = true;
                        break;
                }
            }
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
                if (Texture != null)
                    Texture.Dispose();

                // Prevent the finalizer from calling ~SPR2Frame, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("SPR2Frame not explicitly disposed!");
        }
    }
}
