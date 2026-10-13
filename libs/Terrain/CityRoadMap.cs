/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Files.Manager;

namespace Terrain
{
    /// <summary>
    /// Loads the actual 8-bit palette INDEX from the original city RoadMap.bmp.
    /// The BMP palette RGB is intentionally ignored: bit 0..3 selects road##,
    /// and bits 4..7 select roadcorner## in ViewerClient's city-map channel 2.
    /// </summary>
    public static class CityRoadMap
    {
        public static Texture2D Load(GraphicsDevice device, int expectedWidth, int expectedHeight)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            string path = LocateFile();
            int width, height;
            byte[] flags = ReadFlags(path, out width, out height);
            if (width != expectedWidth || height != expectedHeight)
                throw new InvalidDataException($"RoadMap is {width}x{height}; terrain map is {expectedWidth}x{expectedHeight}. " +
                    "All city layers must come from the same city. RoadMap path: " + path);

            Color[] encoded = new Color[flags.Length];
            for (int i = 0; i < flags.Length; ++i)
            {
                byte road = (byte)(flags[i] & 0x0f);
                byte corner = (byte)(flags[i] >> 4);
                // Two independently sampled 0..15 selectors, encoded as exact bytes.
                encoded[i] = new Color(road, corner, (byte)0, (byte)255);
            }

            Texture2D texture = new Texture2D(device, width, height, false, SurfaceFormat.Color);
            texture.SetData(encoded);
            return texture;
        }

        private static string LocateFile()
        {
            // Optional override, useful if Hoodtest's loose BMPs are from city other than 0001.
            string directPath = Environment.GetEnvironmentVariable("HOODTEST_ROADMAP_PATH");
            if (!string.IsNullOrWhiteSpace(directPath))
            {
                string absolute = Path.GetFullPath(directPath);
                if (!File.Exists(absolute)) throw new FileNotFoundException("RoadMap override does not exist", absolute);
                return absolute;
            }

            string local = Path.Combine(Environment.CurrentDirectory, "roadmap.bmp");
            if (File.Exists(local)) return local;
            local = Path.Combine(AppContext.BaseDirectory, "roadmap.bmp");
            if (File.Exists(local)) return local;

            int cityID = 1;
            string configuredCity = Environment.GetEnvironmentVariable("HOODTEST_CITY_ID");
            if (!string.IsNullOrWhiteSpace(configuredCity) &&
                (!int.TryParse(configuredCity, out cityID) || cityID < 1 || cityID > 9999))
                throw new InvalidDataException("HOODTEST_CITY_ID must be an integer from 1 through 9999.");

            string clientDirectory = FileManager.Instance.BaseDirectory;
            if (!string.IsNullOrWhiteSpace(clientDirectory))
            {
                string clientPath = Path.Combine(clientDirectory, "cities", $"city_{cityID:0000}", "roadmap.bmp");
                if (File.Exists(clientPath)) return clientPath;
            }

            throw new FileNotFoundException(
                "Can't find the original city RoadMap.bmp. Place 'roadmap.bmp' next to 'elevation.bmp', " +
                "set HOODTEST_ROADMAP_PATH to its complete filename, or configure FileManager's " +
                "game directory plus HOODTEST_CITY_ID. All map layers must be from the same city.");
        }

        /// <summary>
        /// Reads BI_RGB and BI_RLE8 8-bit BMPs without replacing palette indices
        /// with palette RGB. Rows are returned top-to-bottom, matching Texture2D.
        /// </summary>
        public static byte[] ReadFlags(string filename, out int width, out int height)
        {
            using (FileStream stream = File.OpenRead(filename))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                if (reader.ReadUInt16() != 0x4d42) throw new InvalidDataException("RoadMap must be a BMP.");
                reader.ReadUInt32(); // BMP file size
                reader.ReadUInt16(); // reserved
                reader.ReadUInt16(); // reserved
                uint pixelOffset = reader.ReadUInt32();
                uint dibSize = reader.ReadUInt32();
                if (dibSize < 40 || dibSize > 124)
                    throw new InvalidDataException("RoadMap needs a Windows BITMAPINFOHEADER-compatible BMP.");

                width = reader.ReadInt32();
                int signedHeight = reader.ReadInt32();
                if (width <= 0 || signedHeight == 0 || width > 8192 || signedHeight == int.MinValue ||
                    Math.Abs((long)signedHeight) > 8192)
                    throw new InvalidDataException("Invalid RoadMap dimensions.");
                height = Math.Abs(signedHeight);
                bool topDown = signedHeight < 0;
                ushort planes = reader.ReadUInt16();
                ushort bitsPerPixel = reader.ReadUInt16();
                uint compression = reader.ReadUInt32();
                if (planes != 1 || bitsPerPixel != 8 || compression > 1)
                    throw new InvalidDataException("RoadMap requires an 8-bit indexed BMP (BI_RGB or BI_RLE8).");
                if (pixelOffset >= stream.Length || pixelOffset < 14 + dibSize)
                    throw new InvalidDataException("Invalid RoadMap pixel-data offset.");

                byte[] flags = new byte[checked(width * height)];
                stream.Position = pixelOffset;
                if (compression == 0)
                {
                    int stride = checked((width + 3) & ~3);
                    byte[] row = new byte[stride];
                    for (int storedY = 0; storedY < height; ++storedY)
                    {
                        if (reader.Read(row, 0, stride) != stride)
                            throw new EndOfStreamException("RoadMap BMP has truncated pixel rows.");
                        int y = topDown ? storedY : (height - 1 - storedY);
                        Buffer.BlockCopy(row, 0, flags, y * width, width);
                    }
                }
                else
                {
                    // RLE8 commands: encoded run, EOL, EOF, delta, absolute run.
                    int x = 0, storedY = 0;
                    bool ended = false;
                    while (stream.Position < stream.Length && !ended)
                    {
                        int count = reader.ReadByte();
                        int value = reader.ReadByte();
                        if (count != 0)
                        {
                            for (int i = 0; i < count; ++i)
                                Put(flags, width, height, topDown, x++, storedY, (byte)value);
                        }
                        else if (value == 0)
                        {
                            x = 0;
                            ++storedY;
                        }
                        else if (value == 1) ended = true;
                        else if (value == 2)
                        {
                            x += reader.ReadByte();
                            storedY += reader.ReadByte();
                        }
                        else
                        {
                            int n = value;
                            for (int i = 0; i < n; ++i)
                                Put(flags, width, height, topDown, x++, storedY, reader.ReadByte());
                            if ((n & 1) != 0) reader.ReadByte(); // Word-aligned absolute run
                        }
                        if (x > width || storedY > height)
                            throw new InvalidDataException("RoadMap RLE8 run exceeds bitmap dimensions.");
                    }
                    if (!ended) throw new InvalidDataException("RoadMap RLE8 is truncated (missing end marker).");
                }
                return flags;
            }
        }

        private static void Put(byte[] data, int width, int height, bool topDown, int x, int storedY, byte index)
        {
            if (x < 0 || x >= width || storedY < 0 || storedY >= height)
                throw new InvalidDataException("RoadMap RLE8 pixel outside the map.");
            int y = topDown ? storedY : (height - 1 - storedY);
            data[y * width + x] = index;
        }
    }
}
