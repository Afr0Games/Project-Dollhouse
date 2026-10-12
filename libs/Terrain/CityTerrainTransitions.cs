/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terrain
{
    /// <summary>
    /// Builds a per-tile type/mask map once, rather than
    /// querying four neighbouring texels for every rendered pixel.
    /// Visual transition masks are generated procedurally at runtime; no client artwork is embedded.
    ///
    /// Neighbour priority and bit assignments follow ViewerClient's
    /// FUN_10006AAD. The mapping from Terrain's colour BMP to terrain ranks
    /// below is provisional: the original game's six numerical terrain IDs
    /// have not yet been matched conclusively to these five colours.
    /// </summary>
    public static class CityTerrainTransitions
    {
        private const byte Grass = 0;
        private const byte Water = 1;
        private const byte Sand  = 2;
        private const byte Rock  = 3;
        private const byte Snow  = 4;
        private const byte Invalid = 255;

        public static Texture2D CreateTexture(GraphicsDevice device, Texture2D terrainType)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (terrainType == null) throw new ArgumentNullException(nameof(terrainType));

            int width = terrainType.Width;
            int height = terrainType.Height;
            Color[] pixels = new Color[width * height];
            Color[] encoded = new Color[pixels.Length];
            terrainType.GetData(pixels);

            byte[] types = new byte[pixels.Length];
            for (int i = 0; i < pixels.Length; ++i)
                types[i] = GetTerrainType(pixels[i]);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = y * width + x;
                    byte center = types[index];
                    if (center == Invalid)
                    {
                        encoded[index] = new Color(Invalid, Invalid, (byte)15, (byte)0);
                        continue;
                    }

                    byte secondary = center;
                    byte mask = 0;
                    //L, U, R, D. Every comparison is against CENTER, and
                    //LAST greater neighbour wins. Do not use Math.Max here.
                    Consider(Sample(types, width, height, x - 1, y, center), center, 4,
                        ref secondary, ref mask);
                    Consider(Sample(types, width, height, x, y - 1, center), center, 2,
                        ref secondary, ref mask);
                    Consider(Sample(types, width, height, x + 1, y, center), center, 1,
                        ref secondary, ref mask);
                    Consider(Sample(types, width, height, x, y + 1, center), center, 8,
                        ref secondary, ref mask);

                    //RGBA = current type, secondary type, transition mask, parity.
                    //Each is an integer byte (shader multiplies samples by 255).
                    encoded[index] = new Color(center, secondary, mask, (byte)((x + y) & 1));
                }
            }

            Texture2D result = new Texture2D(device, width, height, false, SurfaceFormat.Color);
            result.SetData(encoded);
            return result;
        }

        private static byte Sample(byte[] types, int width, int height,
            int x, int y, byte fallback)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return fallback;
            byte terrain = types[y * width + x];
            return terrain == Invalid ? fallback : terrain;
        }

        private static void Consider(byte neighbor, byte center, byte bit,
            ref byte secondary, ref byte mask)
        {
            if (neighbor <= center)
                mask |= bit;
            else
                secondary = neighbor;
        }

        private static byte GetTerrainType(Color color)
        {
            //Actual terraintype.bmp water is (12, 0, 255), not pure blue.
            if (color.B > 220 && color.R < 32 && color.G < 32) return Water;
            if (color.G > 220 && color.R < 32 && color.B < 32) return Grass;
            if (color.R > 220 && color.G > 220 && color.B > 220) return Snow;
            if (color.R > 220 && color.G > 220 && color.B < 32) return Sand;
            if (color.R > 220 && color.G < 32 && color.B < 32) return Rock;
            return Invalid; //Black/unmapped = no transitions into invalid terrain.
        }
    }
}
