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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using log4net;
using UIParser.Nodes;
using Files.Manager;

namespace UI.Elements
{
    /// <summary>
    /// Class for creating and drawing background images.
    /// </summary>
    public class UIBackgroundImage : UIElement, IDisposable
    {
        public Texture2D Texture;
        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private float m_Opacity;

        /// <summary>
        /// Initialize this to get nine rectangles with which to draw the texture.
        /// </summary>
        public NineSlicer Slicer;

        /// <summary>
        /// Constructs a new UIImage instance.
        /// </summary>
        /// <param name="Name">The name of this background.</param>
        /// <param name="Tex">Texture to create image from.</param>
        /// <param name="Screen">UIScreen instance.</param>
        /// <param name="Parent">Parent of this UIImage instance, may be null.</param>
        /// <param name="Opacity">Opacity of this UIElement. Defaults to 255.</param>
        public UIBackgroundImage(string Name, Texture2D Tex, UIScreen Screen, UIElement Parent = null, float Opacity = 255f) 
            : base(Name, new Vector2(0, 0), new Vector2(Tex.Width, Tex.Height), Screen, Parent)
        {
            Texture = Tex;
            m_Opacity = Opacity;
            DrawOrder = (int)DrawOrderEnum.Game;
            Visible = true;
        }

        /// <summary>
        /// Constructs a new UIBackgroundImage instance.
        /// </summary>
        /// <param name="Node">A DefineImageNode instance, as generated by an Abstract Syntax Tree (see UIScreen.cs).</param>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Opacity">Opacity of this UIImage. Defaults to 255.</param>
        public UIBackgroundImage(DefineImageNode Node, UIScreen Screen, float Opacity = 255f) : base(Screen)
        {
            Name = Node.Name;
            Texture = FileManager.Instance.GetTexture(ulong.Parse(Node.AssetID, System.Globalization.NumberStyles.HexNumber));
            m_Opacity = Opacity;
            DrawOrder = (int)DrawOrderEnum.Game;
            Visible = true;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="Image">A UIImage instance to copy.</param>
        public UIBackgroundImage(UIBackgroundImage Image) : base(Image.m_Screen)
        {
            Name = Image.Name;
            Texture = Image.Texture;
            Position = Image.Position;
            DrawOrder = (int)DrawOrderEnum.Game;
            Visible = true;
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

        public override void Draw(SpriteBatch SBatch/*, float? LayerDepth*/)
        {
            float Depth;

            /*if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.10f;*/

            SBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height), 
                null, Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0/*Depth*/);
        }

        ~UIBackgroundImage()
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
}
