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
using Files;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hoodtest
{
    /// <summary>
    /// Retrieves the 32 original road/roadcorner TGAs via the shared FileManager.
    /// Creates a transient two-row GPU atlas at startup; no artwork is written to disk.
    /// FileManager owns the source Texture2D cache, so source textures are not disposed.
    /// </summary>
    public static class MaxisRoadTextures
    {
        public sealed class RoadAtlas : IDisposable
        {
            public Texture2D Texture { get; internal set; }
            public Texture2D Sizes { get; internal set; }
            public Vector2 CellSize { get; internal set; }
            public void Dispose() { Texture?.Dispose(); Sizes?.Dispose(); }
        }

        private const int Padding = 2;
        private const int Variants = 16;
        private const int Banks = 2;

        public static RoadAtlas Load(GraphicsDevice Device)
        {
            if (Device == null) throw new ArgumentNullException(nameof(Device));

            Color[][] SourcePixels = new Color[Banks * Variants][];
            int[] Widths = new int[Banks * Variants];
            int[] Heights = new int[Banks * Variants];
            int LargestWidth = 0, LargestHeight = 0;

            for (int bank = 0; bank < Banks; ++bank)
            {
                for (int index = 0; index < Variants; ++index)
                {
                    string Name = (bank == 0 ? "road" : "roadcorner") + index.ToString("00");
                    ulong AssetID = (ulong)Enum.Parse<FileIDs.TerrainFileIDs>(Name);
                    Texture2D Original = FileManager.Instance.GetTexture(AssetID, IsTGA: true);

                    if (Original == null) 
                        throw new FileNotFoundException("Missing original road texture " + Name + ".tga");
                    if (Original.Width < 1 || Original.Height < 1 || Original.Width > 2048 || Original.Height > 2048)
                        throw new InvalidDataException("Unexpected road image size: " + Name);

                    int Slot = bank * Variants + index;
                    Widths[Slot] = Original.Width;
                    Heights[Slot] = Original.Height;
                    Color[] Pixels = new Color[checked(Original.Width * Original.Height)];
                    Original.GetData(Pixels);
                    SourcePixels[Slot] = Pixels;
                    LargestWidth = Math.Max(LargestWidth, Original.Width);
                    LargestHeight = Math.Max(LargestHeight, Original.Height);
                }
            }

            int CellWidth = LargestWidth + Padding * 2;
            int CellHeight = LargestHeight + Padding * 2;
            int TextureWidth = checked(CellWidth * Variants);
            int TextureHeight = checked(CellHeight * Banks);
            Color[] AtlasPixels = new Color[checked(TextureWidth * TextureHeight)];

            for (int Bank = 0; Bank < Banks; ++Bank)
            for (int Index = 0; Index < Variants; ++Index)
            {
                int slot = Bank * Variants + Index;
                int SourceWidth = Widths[slot], SourceHeight = Heights[slot];
                Color[] src = SourcePixels[slot];
                for (int cy = 0; cy < SourceHeight + Padding * 2; ++cy)
                {
                    int sy = Math.Max(0, Math.Min(SourceHeight - 1, cy - Padding));
                    int DestY = Bank * CellHeight + cy;
                    int row = DestY * TextureWidth + Index * CellWidth;

                    for (int cx = 0; cx < SourceWidth + Padding * 2; ++cx)
                    {
                        int sx = Math.Max(0, Math.Min(SourceWidth - 1, cx - Padding));
                        AtlasPixels[row + cx] = src[sy * SourceWidth + sx];
                    }
                }
            }

            // A 16x2 size table preserves each original image's dimensions.
            // No rescaling of original artwork, including mixed-size road sprites.
            Color[] Dimensions = new Color[Banks * Variants];
            for (int i = 0; i < Dimensions.Length; ++i)
            {
                int w = Widths[i], h = Heights[i];
                Dimensions[i] = new Color((byte)(w & 255), (byte)(w >> 8),
                                          (byte)(h & 255), (byte)(h >> 8));
            }

            Texture2D Atlas = new Texture2D(Device, TextureWidth, TextureHeight, false, SurfaceFormat.Color);
            Texture2D Sizes = new Texture2D(Device, Variants, Banks, false, SurfaceFormat.Color);
            Atlas.SetData(AtlasPixels);
            Sizes.SetData(Dimensions);

            return new RoadAtlas
            {
                Texture = Atlas,
                Sizes = Sizes,
                CellSize = new Vector2(CellWidth, CellHeight)
            };
        }
    }
}
