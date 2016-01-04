using System;
using System.Collections.Generic;
using System.Text;
using UIParser.Nodes;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Elements
{
    public class UIImage : UIElement
    {
        public Texture2D Texture;
        private bool m_Loaded = false;
        public bool Loaded { get { return m_Loaded; } }

        /// <summary>
        /// Initialize this to get nine rectangles with which to draw the texture.
        /// </summary>
        public NineSlicer Slicer;

        public UIImage(DefineImageNode Node, UIScreen Screen) : base(Screen)
        {
            m_Name = Node.Name;
            Texture = FileManager.GetTexture(ulong.Parse(Node.AssetID, System.Globalization.NumberStyles.HexNumber));

            m_Loaded = true;
        }

        public UIImage(Texture2D Tex, UIScreen Screen, UIElement Parent = null) : base(Screen, Parent)
        {
            Texture = Tex;

            m_Loaded = true;
        }

        /// <summary>
        /// Renders this UIImage to the screen.
        /// </summary>
        /// <param name="SBatch">Spritebatch with wich to render.</param>
        /// <param name="SourceRect">A rectangle controlling which part of the image is rendered. Can be null.
        ///                         If Slicer has been initialized, SourceRect will also be added to this image's
        ///                         position to calculate the final position.</param>
        public override void Draw(SpriteBatch SBatch, Rectangle? SourceRect)
        {
            if (Slicer != null)
            {
                Vector2 Pos = new Vector2(Position.X + SourceRect.Value.X, 
                    Position.Y + SourceRect.Value.Y);
                Pos *= m_Screen.Scale;
                SBatch.Draw(Texture, Pos, SourceRect, Color.White, 0.0f,
                    new Vector2(0.0f, 0.0f), m_Screen.Scale, SpriteEffects.None, 0.0f);
            }
            else
            {
                SBatch.Draw(Texture, Position, SourceRect, Color.White, 0.0f, 
                    new Vector2(0.0f, 0.0f), m_Screen.Scale, SpriteEffects.None, 0.0f);
            }
        }
    }

    public class NineSlicer
    {
        public Rectangle TLeft, TCenter, TRight;
        public Rectangle CLeft, CCenter, CRight;
        public Rectangle BLeft, BCenter, BRight;
        private int m_Width, m_Height;

        public NineSlicer(Vector2 Position, int TextureWidth, int TextureHeight)
        {
            m_Width = TextureWidth;
            m_Height = TextureHeight;
            Calculate();
        }

        public void Calculate()
        {
            int TileWidth = (int)(m_Width / 3);
            int TileHeight = (int)(m_Height / 3);

            TLeft = new Rectangle(0, 0, TileWidth, TileHeight);
            TCenter = new Rectangle(TLeft.Right, 0, TileWidth, TileHeight);
            TRight = new Rectangle(TCenter.Right, 0, TileWidth, TileHeight);

            CLeft = new Rectangle(0, TileHeight, TileWidth, TileHeight);
            CCenter = new Rectangle(CLeft.Right, TileHeight, TileWidth, TileHeight);
            CRight = new Rectangle(CCenter.Right, TileHeight, TileWidth, TileHeight);

            BLeft = new Rectangle(0, (TileHeight * 2), TileWidth, TileHeight);
            BCenter = new Rectangle(BLeft.Right, (TileHeight * 2), TileWidth, TileHeight);
            BRight = new Rectangle(BCenter.Right, (TileHeight * 2), TileWidth, TileHeight);
        }
    }
}
