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
                //Pos *= m_Screen.Scale;
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

        public NineSlicer(Vector2 Position, int TextureWidth, int TextureHeight, int LeftPadding, int RightPadding, 
            int TopPadding, int BottomPadding)
        {
            m_Width = TextureWidth;
            m_Height = TextureHeight;
            Calculate(Position.X, Position.Y, LeftPadding, RightPadding, TopPadding, BottomPadding);
        }

        /// <summary>
        /// Calculates 9 patches of texture (from: 
        /// http://gamedev.stackexchange.com/questions/115684/monogame-scale-resizing-texture-without-stretching-the-borders)
        /// </summary>
        /// <param name="X">X-position of texture.</param>
        /// <param name="Y">Y-position of texture.</param>
        /// <param name="LeftPadding">Amount of padding for left side.</param>
        /// <param name="RightPadding">Amount of padding for right side.</param>
        /// <param name="TopPadding">Amount of padding for top.</param>
        /// <param name="BottomPadding">Amount of padding for bottom.</param>
        public void Calculate(float X, float Y, int LeftPadding, int RightPadding, int TopPadding, int BottomPadding)
        {
            int MiddleWidth = m_Width - LeftPadding - RightPadding;
            int MiddleHeight = m_Height - TopPadding - BottomPadding;
            int BottomY = (int)(Y + m_Height - BottomPadding);
            int RightX = (int)(X + m_Width - RightPadding);
            int LeftX = (int)(X + LeftPadding);
            int TopY = (int)(Y + TopPadding);

            TLeft = new Rectangle((int)X, (int)Y, LeftPadding, TopPadding);
            TCenter = new Rectangle(LeftX, (int)Y, MiddleWidth, TopPadding);
            TRight = new Rectangle(RightX, (int)Y, RightPadding, TopPadding);

            CLeft = new Rectangle((int)X, TopY, LeftPadding, MiddleHeight);
            CCenter = new Rectangle(LeftX, TopY, MiddleWidth, MiddleHeight);
            CRight = new Rectangle(RightX, TopY, RightPadding, MiddleHeight);

            BLeft = new Rectangle((int)X, BottomY, LeftPadding, BottomPadding);
            BCenter = new Rectangle(LeftX, BottomY, MiddleWidth, BottomPadding);
            BRight = new Rectangle(RightX, BottomY, RightPadding, BottomPadding);
        }
    }
}
