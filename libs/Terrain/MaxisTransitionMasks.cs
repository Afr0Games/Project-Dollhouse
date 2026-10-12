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

namespace Terrain
{
    /// <summary>
    /// Loads the original Maxis transition masks as 60 separate TGA files.
    /// Packs them only into a temporary GPU texture for efficient sampling.
    /// Never creates, converts, or writes an image on disk.
    /// </summary>
    
    public static class MaxisTransitionMasks
    {
        private const int MaskSize = 64;
        private const int Padding = 2;
        private const int CellSize = MaskSize + Padding * 2;
        private const int Columns = 16; //Column 15 is the unused all-black mask.
        private const int Rows = 4;     //transa?a, transa?b, transb?a, transb?b.

        public static Texture2D Load(GraphicsDevice device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            int atlasWidth = Columns * CellSize;
            int atlasHeight = Rows * CellSize;
            Color[] atlasPixels = new Color[atlasWidth * atlasHeight];

            //Initialize the missing mask-15 column as fully opaque black.
            for (int i = 0; i < atlasPixels.Length; ++i)
                atlasPixels[i] = Color.Black;

            for (int row = 0; row < Rows; ++row)
            {
                char family = row < 2 ? 'a' : 'b';
                char variant = (row & 1) == 0 ? 'a' : 'b';

                for (int mask = 0; mask < 15; ++mask)
                {
                    string filename = string.Format("trans{0}{1:00}{2}", family, mask, variant);
                    ulong assetID = (ulong)Enum.Parse<FileIDs.TerrainFileIDs>(filename);

                    Color[] sourcePixels = new Color[MaskSize * MaskSize];
                    using (Texture2D source = FileManager.Instance.GetTexture(assetID, true))
                    {
                        if (source.Width != MaskSize || source.Height != MaskSize)
                            throw new InvalidDataException(filename + " must be 64x64 pixels.");

                        source.GetData(sourcePixels);
                    }

                    //Two pixels of replicated edges stop bilinear sampling from
                    //leaking neighbouring masks at atlas cell boundaries.
                    int destinationX = mask * CellSize;
                    int destinationY = row * CellSize;
                    for (int y = 0; y < CellSize; ++y)
                    {
                        int sourceY = Clamp(y - Padding, 0, MaskSize - 1);
                        int destinationRow = (destinationY + y) * atlasWidth + destinationX;
                        int sourceRow = sourceY * MaskSize;
                        for (int x = 0; x < CellSize; ++x)
                        {
                            int sourceX = Clamp(x - Padding, 0, MaskSize - 1);
                            atlasPixels[destinationRow + x] = sourcePixels[sourceRow + sourceX];
                        }
                    }
                }
            }

            Texture2D result = new Texture2D(device, atlasWidth, atlasHeight,
                false, SurfaceFormat.Color);
            result.SetData(atlasPixels);
            return result;
        }

        private static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(value, max));
        }
    }
}
