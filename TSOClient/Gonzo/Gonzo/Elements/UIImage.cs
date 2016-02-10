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
        /// Sets the size of this UIImage instance.
        /// </summary>
        /// <param name="Width">The width of this UIImage instance.</param>
        /// <param name="Height">The height of this UIImage instance.</param>
        public void SetSize(int Width, int Height)
        {
            m_Size.X = Width;
            m_Size.Y = Height;

            if (Slicer != null)
            {
                Slicer.CalculateScales(Width, Height);
            }
        }

        public override void Draw(SpriteBatch SBatch, Rectangle? SourceRect)
        {
            if (SourceRect != null)
                SBatch.Draw(Texture, Position, SourceRect, Color.White);
            else
                SBatch.Draw(Texture, Position, null, Color.White);
        }

        /// <summary>
        /// Draws a part of this image's texture to a specific point in screen space.
        /// Used for drawing dialogs (see UIDialog.cs).
        /// </summary>
        /// <param name="SBatch">The SpriteBatch used for drawing.</param>
        /// <param name="Scale">The scale at which to draw.</param>
        /// <param name="From">A source rectangle that controls which part of the texture is drawn.</param>
        /// <param name="To">Point in screen space to draw to.</param>
        public void DrawTextureTo(SpriteBatch SBatch, Vector2? Scale, Rectangle From, Vector2 To)
        {
            if (Scale != null)
                SBatch.Draw(Texture, To, null, From, new Vector2(0.0f, 0.0f), 0.0f, Scale, Color.White, SpriteEffects.None, 0.0f);
            else
                SBatch.Draw(Texture, To, From, Color.White);
        }
    }

    public class NineSlicer
    {
        public Rectangle TLeft, TCenter, TRight;
        public Rectangle CLeft, CCenter, CRight;
        public Rectangle BLeft, BCenter, BRight;
        public int Width, Height;
        public int LeftPadding, RightPadding, TopPadding, BottomPadding;

        public Vector2 TCenter_Scale, CCenter_Scale, BCenter_Scale, CLeft_Scale, CRight_Scale;

        public NineSlicer(Vector2 Position, int TextureWidth, int TextureHeight, int LeftPadding, int RightPadding, 
            int TopPadding, int BottomPadding)
        {
            Width = TextureWidth;
            Height = TextureHeight;
            Calculate(Position.X, Position.Y, LeftPadding, RightPadding, TopPadding, BottomPadding);
            CalculateScales(Width, Height);
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
            this.LeftPadding = LeftPadding;
            this.RightPadding = RightPadding;
            this.TopPadding = TopPadding;
            this.BottomPadding = BottomPadding;

            int MiddleWidth = Width - LeftPadding - RightPadding;
            int MiddleHeight = Height - TopPadding - BottomPadding;
            int BottomY = (int)(Y + Height - BottomPadding);
            int RightX = (int)(X + Width - RightPadding);
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

        public void CalculateScales(float width, float height)
        {
            Width = (int)width;
            Height = (int)height;

            TCenter_Scale = new Vector2((width - (LeftPadding + RightPadding)) / (TCenter.Width), 1);
            CCenter_Scale = new Vector2(
                            (width - (LeftPadding + RightPadding)) / (CCenter.Width),
                            (height - (TopPadding + BottomPadding)) / (CCenter.Height)
                       );
            BCenter_Scale = new Vector2((width - (LeftPadding + RightPadding)) / (BCenter.Width), 1);

            CLeft_Scale = new Vector2(1, (height - (TopPadding + BottomPadding)) / (CLeft.Height));
            CRight_Scale = new Vector2(1, (height - (TopPadding + BottomPadding)) / (CRight.Height));
        }
    }
}
