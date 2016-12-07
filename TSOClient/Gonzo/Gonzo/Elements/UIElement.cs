/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Elements
{
    public enum LayerDepth
    {
        Default,
        ImageLayer,
        ButtonLayer,
        DialogLayer,
        TextLayer
    }

    /// <summary>
    /// UIElement is the base class for all UI related elements (UIButton, UIControl, 
    /// UIDialog, UIImage, UILabel, UISlider, UITextEdit).
    /// </summary>
    public class UIElement
    {
        protected UIElement m_Parent;
        protected UIScreen m_Screen;
        protected Dictionary<string, UIElement> m_Elements = new Dictionary<string, UIElement>();
        public string Name;
        protected int m_ID;

        protected bool m_HasFocus = false; //Does this UIElement have focus so it can receive keyboard input?
        protected bool m_KeyboardInput = false; //Should this UIElement receive keyboard input?

        /// <summary>
        /// Gets or sets whether this UIElement has focus so it can receive keyboard input.
        /// </summary>
        public bool HasFocus { get { return m_HasFocus; } set { m_HasFocus = value; } }

        /// <summary>
        /// Can this UIElement receive keyboard input?
        /// </summary>
        public bool ListensToKeyboard { get { return m_KeyboardInput; } }

        protected bool m_Opaque = false;
        protected Vector2 m_Size;

        private Vector2 m_Position;

        //TextDrawingColor stores the color used for drawing (can be any of the following values).
        public Color TextDrawingColor, TextColor, TextColorSelected, TextColorHighlighted, 
            TextColorDisabled;

        public string Tooltip { get; set; }
        public UIImage Image;

        protected SpriteFont m_Font;

        public int Tracking, Trigger;

        public bool Visible = true;

        /// <summary>
        /// Mouse interacted with this UIElement.
        /// </summary>
        /// <param name="Helper">InputHelper instance for input data.</param>
        public virtual void MouseEvents(InputHelper Helper) { }

        /// <summary>
        /// Handles update logic for this UIElement.
        /// </summary>
        /// <param name="Helper">InputHelper instance for input data.</param>
        public virtual void Update(InputHelper Helper, GameTime GTime)
        {
            if (IsMouseOver(Helper))
            {
                if (Helper.IsNewPress(MouseButtons.LeftButton))
                {
                    m_HasFocus = true;
                    m_Screen.OverrideFocus(this);
                }
            }
        }

        /// <summary>
        /// Handles drawing logic for this UIElement.
        /// </summary>
        /// <param name="SBatch">A SpriteBatch instance.</param>
        /// <param name="LayerDepth">Depth at which to draw, may be null.</param>
        public virtual void Draw(SpriteBatch SBatch, float? LayerDepth) { }

        /// <summary>
        /// Handles drawing logic for this UIElement.
        /// </summary>
        /// <param name="SBatch">A SpriteBatch instance.</param>
        /// <param name="SourceRect">A source rectangle, for controlling which part of this elenent's texture is drawn.</param>
        /// <param name="LayerDepth">Depth at which to draw, may be null.</param>
        public virtual void Draw(SpriteBatch SBatch, Rectangle? SourceRect, float? LayerDepth) { }

        /// <summary>
        /// Handles drawing logic for this UIElement.
        /// </summary>
        /// <param name="SBatch">A SpriteBatch instance.</param>
        /// <param name="SourceRect">A source rectangle, for controlling which part of this elenent's texture is drawn.</param>
        /// <param name="LayerDepth">Depth at which to draw, may be null.</param>
        /// <param name="ScaleFactor">Scale at which to draw, may be null.</param>
        public virtual void Draw(SpriteBatch SBatch, Rectangle? SourceRect, float? LayerDepth, Vector2? ScaleFactor) { }

        /// <summary>
        /// Gets the value of the corresponding layer depth.
        /// </summary>
        /// <param name="Depth">The depth for which to retrieve a value.</param>
        /// <returns>The value of the specified depth.</returns>
        public static float GetLayerDepth(LayerDepth Depth)
        {
            switch(Depth)
            {
                case LayerDepth.Default:
                    return 0.0f;
                case LayerDepth.ImageLayer:
                    return 0.8f;
                case LayerDepth.ButtonLayer:
                    return 0.9f;
                case LayerDepth.DialogLayer:
                    return 0.10f;
                case LayerDepth.TextLayer:
                    return 0.11f;
                default:
                    return 0.0f;
            }
        }

        public UIElement(string Name, Vector2 Position, Vector2 Size, UIScreen Screen, UIElement Parent = null)
        {
            this.Name = Name;
            m_Size = Size;

            if (Parent != null)
            {
                m_Parent = Parent;
                m_Position += Parent.m_Position;
            }
            else
                m_Position = Position;

            m_Screen = Screen;
        }

        /// <summary>
        /// Constructs a UIElement from a Screen instance and an optional UIElement that acts as a parent.
        /// </summary>
        /// <param name="Screen">A Screen instance.</param>
        /// <param name="Parent">(Optional) UIElement that acts as a parent.</param>
        public UIElement(UIScreen Screen, UIElement Parent = null)
        {
            m_Screen = Screen;

            if (Parent != null)
                m_Parent = Parent;
        }

        /// <summary>
        /// Gets the size of this element (width and height).
        /// </summary>
        public Vector2 Size
        {
            get { return m_Size; }
        }

        /// <summary>
        /// Gets or sets the position for this UIElement.
        /// </summary>
        public Vector2 Position
        {
            get { return m_Position; }
            set
            {
                m_Position.X = value.X;
                m_Position.Y = value.Y;

                if (Image != null)
                    Image.Position = m_Position;
            }
        }

        /// <summary>
        /// Gets a child from this UIElements Dictionary of children.
        /// </summary>
        /// <param name="Name">The name of the child to get.</param>
        /// <returns>A UIElement instance that can be cast.</returns>
        public UIElement GetChild(string Name)
        {
            try
            {
                return m_Elements[Name];
            }
            catch (Exception)
            {
                throw new Exception("Couldn't find child: " + Name + " in UIElement.cs");
            }
        }

        uint[] PixelData = new uint[1]; // Delare an Array of 1 just to store data for one pixel
        protected bool PixelCheck(InputHelper Input, int Width)
        {
            if (Image == null || !Image.Loaded)
                return false;

            // Get Mouse position relative to top left of Texture
            Vector2 pixelPosition = Input.MousePosition - m_Position;

            // I know. I just checked this condition at OnMouseOver or we wouldn't be here
            // but just to be sure the spot we're checking is within the bounds of the texture...
            if (pixelPosition.X >= 0 && pixelPosition.X < Width &&
                pixelPosition.Y >= 0 && pixelPosition.Y < Image.Texture.Height)
            {
                // Get the Texture Data within the Rectangle coords, in this case a 1 X 1 rectangle
                // Store the data in pixelData Array
                Image.Texture.GetData<uint>(0, new Rectangle((int)pixelPosition.X, (int)pixelPosition.Y, 
                    (1), (1)), PixelData, 0, 1);

                // Check if pixel in Array is non Alpha, give or take 20
                if (((PixelData[0] & 0xFF000000) >> 24) > 20)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the mouse is over this UIElement.
        /// Does NOT work for pixel-perfect collision.
        /// </summary>
        /// <param name="State">State of mouse.</param>
        /// <returns>True if mouse is over, false otherwise.</returns>
        public virtual bool IsMouseOver(InputHelper Input)
        {
            if (m_Size != null)
            {
                if (Input.MousePosition.X > m_Position.X && Input.MousePosition.X <= (m_Position.X + m_Size.X))
                {
                    if (Input.MousePosition.Y > m_Position.Y && Input.MousePosition.Y <= (m_Position.Y + m_Size.Y))
                        return true;
                }
            }
            else
            {
                if (Input.MousePosition.X > m_Position.X && Input.MousePosition.X <= (m_Position.X + Image.Texture.Width))
                {
                    if (Input.MousePosition.Y > m_Position.Y && Input.MousePosition.Y <= (m_Position.Y + Image.Texture.Height))
                        return true;
                }
            }

            return false;
        }
    }
}
