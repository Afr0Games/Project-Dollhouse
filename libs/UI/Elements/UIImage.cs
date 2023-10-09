﻿/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Reflection;
using UIParser.Nodes;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using log4net;

namespace UI.Elements
{
    public class UIImage : UIElement, IDisposable, IBasicDrawable
    {
        public Texture2D Texture;
        private bool m_Loaded = false;
        public bool Loaded { get { return m_Loaded; } }
        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private float m_Opacity;

        /// <summary>
        /// Initialize this to get nine rectangles with which to draw the texture.
        /// </summary>
        public NineSlicer Slicer;

        /// <summary>
        /// Constructs a new UIImage instance.
        /// </summary>
        /// <param name="Node">A DefineImageNode instance, as generated by an Abstract Syntax Tree (see UIScreen.cs).</param>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Opacity">Opacity of this UIImage. Defaults to 255.</param>
        public UIImage(DefineImageNode Node, UIScreen Screen, float Opacity = 255f) : base(Screen)
        {
            Name = Node.Name;
            Texture = FileManager.Instance.GetTexture(ulong.Parse(Node.AssetID, System.Globalization.NumberStyles.HexNumber));
            m_Opacity = Opacity;
            m_Loaded = true;
            DrawOrder = (int)DrawOrderEnum.Game; //Default
        }

        /// <summary>
        /// Constructs a new UIImage instance.
        /// </summary>
        /// <param name="Tex">Texture to create image from.</param>
        /// <param name="Screen">UIScreen instance.</param>
        /// <param name="Parent">Parent of this UIImage instance, may be null.</param>
        /// <param name="Opacity">Opacity of this UIElement. Defaults to 255.</param>
        public UIImage(Texture2D Tex, Vector2 Pos, UIScreen Screen, UIElement Parent = null, float Opacity = 255f) 
            : base(Screen, Parent)
        {
            Texture = Tex;
            Position = Pos;
            m_Opacity = Opacity;
            m_Loaded = true;
            DrawOrder = (int)DrawOrderEnum.Game; //Default
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="Image">A UIImage instance to copy.</param>
        public UIImage(UIImage Image) : base(Image.m_Screen)
        {
            Name = Image.Name;
            Texture = Image.Texture;
            Position = Image.Position;
            m_Loaded = true;
            DrawOrder = (int)DrawOrderEnum.Game; //Default
        }

        /// <summary>
        /// Sets the size of this UIImage instance.
        /// </summary>
        /// <param name="Width">The width of this UIImage instance.</param>
        /// <param name="Height">The height of this UIImage instance.</param>
        public void SetSize(float Width, float Height)
        {
            m_Size.X = Width;
            m_Size.Y = Height;

            if (Slicer != null)
            {
                Slicer.ReScale(Width, Height);
            }
        }

        public override void Draw(SpriteBatch SBatch)
        {
            if (Visible)
                SBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height), Color.White);
        }

        /// <summary>
        /// Draws this UIImage to the screen.
        /// </summary>
        /// <param name="SBatch">A SpriteBatch instance to draw with.</param>
        /// <param name="DestinationRect">The drawing bounds on screen.</param>
        /// <param name="SourceRect">A rectangle controlling which part of the image to draw. May be null.</param>
        /// <param name="LayerDepth">The depth at which this UIImage will be drawn.</param>
        public override void Draw(SpriteBatch SBatch, Rectangle? DestinationRect, Rectangle? SourceRect)
        {
            if (Visible)
            {
                Rectangle DrawRect = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
                Color Clr = new Color(Color.White.R, Color.White.G, Color.White.B, m_Opacity);

                if (SourceRect != null)
                {
                    if (DestinationRect != null)
                    {
                        SBatch.Draw(Texture, (Rectangle)DestinationRect, SourceRect, Clr, 0.0f, new Vector2(0.0f, 0.0f),
                            SpriteEffects.None, 0);
                    }
                    else
                    {
                        SBatch.Draw(Texture, DrawRect, SourceRect, Clr, 0.0f, new Vector2(0.0f, 0.0f),
                            SpriteEffects.None, 0);
                    }
                }
                else
                {
                    if (DestinationRect != null)
                    {
                        SBatch.Draw(Texture, (Rectangle)DestinationRect, null, Clr, 0.0f, new Vector2(0.0f, 0.0f),
                            SpriteEffects.None, 0);
                    }
                    else
                    {
                        SBatch.Draw(Texture, DrawRect, null, Clr, 0.0f, new Vector2(0.0f, 0.0f),
                            SpriteEffects.None, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Draws this UIImage to the screen.
        /// </summary>
        /// <param name="SBatch">A SpriteBatch instance to draw with.</param>
        /// <param name="SourceRect">A rectangle controlling which part of the image to draw. May be null.</param>
        /// <param name="LayerDepth">Depth at which to draw, may be null.</param>
        /// <param name="ScaleFactor">Scale at which to draw, may be null.</param>
        public override void Draw(SpriteBatch SBatch, Rectangle? SourceRect, float? LayerDepth, Vector2? ScaleFactor)
        {
            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.0f;

            Vector2 Scale;
            if (ScaleFactor != null)
                Scale = (Vector2)ScaleFactor;
            else
                Scale = new Vector2(1.0f, 1.0f);

            if (Visible)
            {
                Color Clr = new Color(Color.White.R, Color.White.G, Color.White.B, m_Opacity);

                SBatch.Draw(Texture, Position, SourceRect, Clr, 0.0f, 
                    new Vector2(0.0f, 0.0f), Scale, SpriteEffects.None, Depth);
            }
        }

        /// <summary>
        /// Draws a part of this image's texture to a specific point in screen space.
        /// Used for drawing dialogs (see UIDialog.cs).
        /// </summary>
        /// <param name="SBatch">The SpriteBatch used for drawing.</param>
        /// <param name="Scale">The scale at which to draw.</param>
        /// <param name="From">A source rectangle that controls which part of the texture is drawn.</param>
        /// <param name="To">Point in screen space to draw to.</param>
        /// <param name="LayerDepth">Depth at which to render, from 0-1. Set to 0.10f by default.</param>
        /// <param name="Opacity">The opacity at which to render the texture.</param>
        public void DrawTextureTo(SpriteBatch SBatch, Vector2? Scale, Rectangle From, Vector2 To)
        {
            Vector2 Scl = Scale == null ? new Vector2(1.0f, 1.0f) : (Vector2)Scale;
            Color Clr = new Color(Color.White.R, Color.White.G, Color.White.B, m_Opacity);

            SBatch.Draw(Texture, To, From, Clr, 0.0f, 
                new Vector2(0.0f, 0.0f), Scl, SpriteEffects.None, 0);
        }

        ~UIImage()
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
                if (Texture != null)
                    Texture.Dispose();

                // Prevent the finalizer from calling ~UIImage, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("UIImage not explicitly disposed!");
        }
    }

    /// <summary>
    /// A utility class for slicing an image into nine pieces (nineslicing).
    /// </summary>
    public class NineSlicer
    {
        public Rectangle TLeft, TCenter, TRight;
        public Rectangle CLeft, CCenter, CRight;
        public Rectangle BLeft, BCenter, BRight;
        public int Width, Height;
        public int LeftPadding, RightPadding, TopPadding, BottomPadding;

        public Vector2 TCenter_Scale, CCenter_Scale, BCenter_Scale, CLeft_Scale, CRight_Scale;

        /// <summary>
        /// Constructs a new NineSlicer instance.
        /// </summary>
        /// <param name="Position">Position of where to begin slicing the texture.</param>
        /// <param name="TextureWidth">Width of texture.</param>
        /// <param name="TextureHeight">Height of texture.</param>
        /// <param name="LeftPadding">Padding on the left.</param>
        /// <param name="RightPadding">Padding on the right.</param>
        /// <param name="TopPadding">Padding at the top.</param>
        /// <param name="BottomPadding">Padding at the bottom.</param>
        public NineSlicer(Vector2 Position, int TextureWidth, int TextureHeight, int LeftPadding, int RightPadding, 
            int TopPadding, int BottomPadding)
        {
            Width = TextureWidth;
            Height = TextureHeight;
            Calculate(Position.X, Position.Y, LeftPadding, RightPadding, TopPadding, BottomPadding);
            ReScale(Width, Height);
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

        /// <summary>
        /// Rescales this NineSlicer to the specified width and height.
        /// </summary>
        /// <param name="width">Width to scale to.</param>
        /// <param name="height">Height to scale to.</param>
        public void ReScale(float width, float height)
        {
            Width = (int)width;
            Height = (int)height;

            TCenter_Scale = new Vector2((width - (LeftPadding + RightPadding)) / (TCenter.Width), 1);
            CCenter_Scale = new Vector2((width - (LeftPadding + RightPadding)) / (CCenter.Width), 
                (height - (TopPadding + BottomPadding)) / (CCenter.Height));
            BCenter_Scale = new Vector2((width - (LeftPadding + RightPadding)) / (BCenter.Width), 1);

            CLeft_Scale = new Vector2(1, (height - (TopPadding + BottomPadding)) / (CLeft.Height));
            CRight_Scale = new Vector2(1, (height - (TopPadding + BottomPadding)) / (CRight.Height));
        }
    }
}
