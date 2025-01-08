/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Diagnostics;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;

namespace Terrain
{
    /// <summary>
    /// The five different types that a terrain can be.
    /// </summary>
    public enum TerrainTypes
    {
        Ground,
        Water,
        Sand,
        Rock,
        Snow
    }

    /// <summary>
    /// Represents a texture atlas, I.E a texture that contains multiple textures.
    /// </summary>
    public class TextureAtlas : IDisposable
    {
        Texture2D m_Atlas;

        /// <summary>
        /// The generated texture atlas.
        /// </summary>
        public Texture2D Atlas
        {
            get { return m_Atlas; }
        }

        /// <summary>
        /// How many textures (blocks) should be in the atlas horizontally.
        /// </summary>
        public int NumBlocksX = 2;

        /// <summary>
        /// How many textures (blocks) should be in the atlas vertically.
        /// </summary>
        public int NumBlocksY = 3;

        /// <summary>
        /// Creates a new TextureAtlas instance.
        /// </summary>
        /// <param name="Device">A GraphicsDevice instance.</param>
        /// <param name="Textures">The full paths to the textures that make up the atlas.</param>
        public TextureAtlas(GraphicsDevice Device, string[] Textures)
        {
            CreateTextureAtlas(Device, Textures);
        }

        private void CreateTextureAtlas(GraphicsDevice Device, string[] Textures)
        {
            List<Texture2D> LoadedTextures = new List<Texture2D>();
            SpriteBatch SBatch = new SpriteBatch(Device);

            foreach (string Tex in Textures)
                LoadedTextures.Add(Texture2D.FromStream(Device, LoadTGA(Tex)));

            int TexWidth = LoadedTextures[0].Width, TexHeight = LoadedTextures[0].Height;

            RenderTarget2D TextureAtlas = new RenderTarget2D(Device, TexWidth * NumBlocksX,
                TexHeight * NumBlocksY);

            Device.SetRenderTarget(TextureAtlas);
            Device.Clear(Microsoft.Xna.Framework.Color.Transparent);

            SBatch.Begin();
            SBatch.Draw(LoadedTextures[0], new Microsoft.Xna.Framework.Rectangle(0, 0, TexWidth, TexHeight), Microsoft.Xna.Framework.Color.White); //Ground
            SBatch.Draw(LoadedTextures[1], new Microsoft.Xna.Framework.Rectangle(TexWidth, 0, TexWidth, TexHeight), Microsoft.Xna.Framework.Color.White); //Water
            SBatch.Draw(LoadedTextures[2], new Microsoft.Xna.Framework.Rectangle(0, TexHeight, TexWidth, TexHeight), Microsoft.Xna.Framework.Color.White); //Sand
            SBatch.Draw(LoadedTextures[3], new Microsoft.Xna.Framework.Rectangle(TexWidth, TexHeight, TexWidth, TexHeight), Microsoft.Xna.Framework.Color.White); //Rock
            SBatch.Draw(LoadedTextures[4], new Microsoft.Xna.Framework.Rectangle(0, 2 * TexHeight, TexWidth, TexHeight), Microsoft.Xna.Framework.Color.White); //Snow
            SBatch.End();

            Device.SetRenderTarget(null);

            m_Atlas = TextureAtlas;
        }

        private Stream LoadTGA(string Path)
        {
            Image Img = Image.Load(Path);
            MemoryStream TGAStream = new MemoryStream();
            Img.SaveAsPng(TGAStream);
            TGAStream.Position = 0;
            return TGAStream;
        }

        public Vector2 TexColorToAtlasCoords(Microsoft.Xna.Framework.Color TexColor)
        {
            int NumCellsX = 2; //Number of cells in the X direction
            int NumCellsY = 3; //Number of cells in the Y direction
            float CellWidthUV = 1.0f / NumCellsX; //Width of each cell in UV space
            float CellHeightUV = 1.0f / NumCellsY; //Height of each cell in UV space

            float U = 0, V = 0;
            if (TexColor == Microsoft.Xna.Framework.Color.Green)
                { U = 0; V = 0; } //Ground
            else if (TexColor == Microsoft.Xna.Framework.Color.Blue)
                { U = CellWidthUV; V = 0; } //Water
            else if (TexColor == Microsoft.Xna.Framework.Color.Red)
                { U = CellWidthUV; V = CellHeightUV; } //Rock
            else if (TexColor == Microsoft.Xna.Framework.Color.White)
                { U = 0; V = 2 * CellHeightUV; } //Snow
            else if (TexColor == Microsoft.Xna.Framework.Color.Yellow)
                { U = 0; V = CellHeightUV; } //Sand

            return new Vector2(U, V);
        }

        ~TextureAtlas()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this UIImage instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this UIImage instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Atlas != null)
                    m_Atlas.Dispose();

                // Prevent the finalizer from calling ~UIImage, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                Debug.WriteLine("TextureAtlas not explicitly disposed!");
        }
    }
}
