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
    /// Creates an ORIGINAL, procedural stand-in for the city's terrain transition art.
    /// No TGA, PNG or Maxis pixel data is embedded, read, copied, or redistributed.
    ///
    /// There are 16 four-edge masks, two families (A: narrow/irregular;
    /// B: broad/soft) and two deterministic variants per family.
    /// Mask bits from ViewerClient: L=4, U=2, R=1, D=8;
    /// a CLEAR bit means the secondary texture enters from that edge.
    /// </summary>
    public static class ProceduralTransitionMasks
    {
        public const int MaskSize = 64;
        private const int Padding = 2;
        public const int CellSize = MaskSize + Padding * 2;
        public const int Columns = 16;
        public const int Rows = 4;

        public static Texture2D CreateTexture(GraphicsDevice device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            int width = Columns * CellSize;
            int height = Rows * CellSize;
            var pixels = new Color[width * height];

            for (int row = 0; row < Rows; ++row)
            {
                bool broad = row >= 2; // Procedural analogue of transb
                int variant = row & 1;
                for (int mask = 0; mask < Columns; ++mask)
                {
                    //Fill both the mask and its duplicated border to avoid
                    //bilinear cross-talk between adjacent atlas cells.
                    for (int py = 0; py < CellSize; ++py)
                    {
                        int y = Clamp(py - Padding, 0, MaskSize - 1);
                        for (int px = 0; px < CellSize; ++px)
                        {
                            int x = Clamp(px - Padding, 0, MaskSize - 1);
                            float u = (x + 0.5f) / MaskSize;
                            float v = (y + 0.5f) / MaskSize;
                            float blend = Evaluate(mask, broad, variant, u, v);
                            byte grey = (byte)Math.Max(0, Math.Min(255,
                                (int)Math.Round(blend * 255f)));
                            pixels[(row * CellSize + py) * width + mask * CellSize + px] =
                                new Color(grey, grey, grey, (byte)255);
                        }
                    }
                }
            }

            Texture2D atlas = new Texture2D(device, width, height, false, SurfaceFormat.Color);
            atlas.SetData(pixels);
            return atlas;
        }

        /// <summary>
        /// Analytic distance-to-edge blend.
        /// </summary>
        public static float Evaluate(int mask, bool broad, int variant, float u, float v)
        {
            //Fully enclosed by primary terrain: no secondary material.
            if ((mask & 15) == 15) return 0f;

            float distance = 2f;
            if ((mask & 4) == 0) distance = Math.Min(distance, u);
            if ((mask & 2) == 0) distance = Math.Min(distance, v);
            if ((mask & 1) == 0) distance = Math.Min(distance, 1f - u);
            if ((mask & 8) == 0) distance = Math.Min(distance, 1f - v);

            //Two spatial frequencies add some natural-looking irregularity.
            //Variant changes the deterministic seed instead of mirroring the
            //pattern; the neighboring tile can use a different variant.
            float wave = 0.55f * ValueNoise(u * 13f, v * 13f, 23 + variant * 109 + mask * 7)
                       + 0.45f * ValueNoise(u * 29f, v * 29f, 61 + variant * 73 + mask * 11);
            distance += (wave - 0.5f) * (broad ? 0.034f : 0.055f);

            float start = broad ? 0.015f : 0.045f;
            float end = broad ? 0.36f : 0.19f;
            float t = Clamp01((distance - start) / (end - start));
            return 1f - t * t * (3f - 2f * t);
        }

        private static float ValueNoise(float x, float y, int seed)
        {
            int ix = (int)Math.Floor(x), iy = (int)Math.Floor(y);
            float fx = x - ix, fy = y - iy;
            fx = fx * fx * (3f - 2f * fx);
            fy = fy * fy * (3f - 2f * fy);

            float a = Hash(ix, iy, seed), b = Hash(ix + 1, iy, seed);
            float c = Hash(ix, iy + 1, seed), d = Hash(ix + 1, iy + 1, seed);
            return Lerp(Lerp(a, b, fx), Lerp(c, d, fx), fy);
        }

        //Deterministic integer hash, no System.Random or external resources.
        private static float Hash(int x, int y, int seed)
        {
            //TODO: Replace this with something safe.
            unchecked
            {
                uint h = (uint)x * 374761393u + (uint)y * 668265263u + (uint)seed * 1442695041u;
                h = (h ^ (h >> 13)) * 1274126177u;
                h ^= h >> 16;
                return (h & 0x00FFFFFFu) / 16777215f;
            }
        }

        private static float Lerp(float a, float b, float t) { return a + (b - a) * t; }
        private static float Clamp01(float x) { return Math.Max(0f, Math.Min(x, 1f)); }
        private static int Clamp(int x, int min, int max) { return Math.Max(min, Math.Min(max, x)); }
    }
}
