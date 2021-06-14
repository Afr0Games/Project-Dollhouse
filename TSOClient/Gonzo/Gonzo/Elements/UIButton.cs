/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Reflection;
using Files;
using Files.Manager;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gonzo.Dialogs;
using log4net;

namespace Gonzo.Elements
{
    public delegate void ButtonClickDelegate(object Sender, ButtonClickEventArgs E);

    /// <summary>
    /// A class inherited from EventArgs that contains information about a button click.
    /// </summary>
    public class ButtonClickEventArgs : EventArgs
    {
        public MouseButtons WhichButtonWasClicked;

        public ButtonClickEventArgs(MouseButtons MButton) : base()
        {
            WhichButtonWasClicked = MButton;
        }
    }

    /// <summary>
    /// A clickable button that can trigger an event.
    /// A button is always graphically represented by four equally sized frames in a texture.
    /// </summary>
    public class UIButton : UIElement, IDisposable
    {
        private Vector2 m_SourcePosition;
        private bool m_IsMouseHovering = false;
        private bool m_IsTextHighlighted = false;

        private bool m_IsTextButton = false;
        private string m_Text;
        private float m_XScale = 1.0f; //Used to scale buttons to fit text.
        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Vector2 m_TextPosition = new Vector2(0, 0);

        /// <summary>
        /// Is this button enabled (I.E not greyed out?)
        /// </summary>
        public bool Enabled = true;

        public bool m_IsButtonClicked = false;
        public event ButtonClickDelegate OnButtonClicked;

        public UIButton(AddButtonNode Node, ParseResult Result, UIScreen Screen) : base(Screen)
        {
            Name = Node.Name;
            m_ID = Node.ID;
            m_Screen = Screen;

            if (!Result.State.InSharedPropertiesGroup)
            {
                if (Node.Image != null)
                {
                    Image = m_Screen.GetImage(Node.Image, false);
                    //Initialize to second frame in the image.
                    m_SourcePosition = new Vector2((Image.Texture.Width / (4)) * 2, 0.0f);

                    m_Size = new Vector2();
                    m_Size.X = (Image.Texture.Width) / (4);
                    m_Size.Y = Image.Texture.Height;

                    Position = new Vector2(Node.ButtonPosition.Numbers[0], Node.ButtonPosition.Numbers[1]) + m_Screen.Position;
                }
                else
                {
                    Image = new UIImage(FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.buttontiledialog), 
                        new Vector2(0, 0), m_Screen);
                    //Initialize to second frame in the image.
                    m_SourcePosition = new Vector2((Image.Texture.Width / 4) * 2, 0.0f);

                    m_Size = new Vector2();
                    m_Size.X = (Image.Texture.Width) / (4);
                    m_Size.Y = Image.Texture.Height;

                    Position = new Vector2(Node.ButtonPosition.Numbers[0], Node.ButtonPosition.Numbers[1]) + m_Screen.Position;
                }
            }
            else
            {
                if (Result.State.Image != "")
                {
                    Image = m_Screen.GetImage(Result.State.Image, true);
                    //Initialize to second frame in the image.
                    m_SourcePosition = new Vector2((Image.Texture.Width / 4) * 2, 0.0f);

                    m_Size = new Vector2();
                    m_Size.X = (Image.Texture.Width) / (4);
                    m_Size.Y = Image.Texture.Height;
                }
                else
                {
                    if (Result.State.TextButton)
                    {
                        m_Text = Result.State.Caption;
                        //Text buttons always use this image.
                        Image = new UIImage(FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.buttontiledialog), 
                            new Vector2(0, 0), m_Screen);
                        //Initialize to second frame in the image.
                        if(Result.State.Size == null)
                            m_SourcePosition = new Vector2((Image.Texture.Width / 4) * 2, 0.0f);

                        m_Size = new Vector2();
                        m_Size.X = Image.Texture.Width / 4;
                        m_Size.Y = Image.Texture.Height;
                    }
                    else
                    {
                        if(Node.Image != null)
                            Image = m_Screen.GetImage(Node.Image, false);
                        else
                            Image = new UIImage(FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.buttontiledialog), 
                                new Vector2(0, 0), m_Screen);
                        //Initialize to second frame in the image.
                        m_SourcePosition = new Vector2((Image.Texture.Width / (4)) * 2, 0.0f);

                        m_Size = new Vector2();
                        m_Size.X = (Image.Texture.Width) / (4);
                        m_Size.Y = Image.Texture.Height;
                    }
                }

                Position = new Vector2(Node.ButtonPosition.Numbers[0], Node.ButtonPosition.Numbers[1]) + m_Screen.Position;

                if (Result.State.Tooltip != "")
                    Tooltip = m_Screen.GetString(Result.State.Tooltip);
            }

            if (Node.TextHighlighted != null)
                m_IsTextHighlighted = (Node.TextHighlighted == 1) ? true : false;

            if (Node.Font != null)
            {
                int FontSize = (int)Node.Font;

                switch (FontSize)
                {
                    case 9:
                        m_Font = Screen.Font9px;
                        break;
                    case 10:
                        m_Font = Screen.Font10px;
                        break;
                    case 12:
                        m_Font = Screen.Font12px;
                        break;
                    case 14:
                        m_Font = Screen.Font14px;
                        break;
                    case 16:
                        m_Font = Screen.Font16px;
                        break;
                }
            }
            else if (Result.State.Font != 0)
            {
                switch (Result.State.Font)
                {
                    case 9:
                        m_Font = Screen.Font9px;
                        break;
                    case 10:
                        m_Font = Screen.Font10px;
                        break;
                    case 12:
                        m_Font = Screen.Font12px;
                        break;
                    case 14:
                        m_Font = Screen.Font14px;
                        break;
                    case 16:
                        m_Font = Screen.Font16px;
                        break;
                    default:
                        m_Font = Screen.Font12px;
                        break;
                }
            }

            if (Node.TextColor != null)
            {
                TextColor = new Color();
                TextColor.A = 255;
                TextColor.R = (byte)Node.TextColor.Numbers[0];
                TextColor.G = (byte)Node.TextColor.Numbers[1];
                TextColor.B = (byte)Node.TextColor.Numbers[2];
            }
            else
            {
                TextColor = Result.State.TextColor;
                TextColor.A = 255;
            }

            if (Node.TextColorSelected != null)
            {
                TextColorSelected = new Color();
                TextColorSelected.A = 255;
                TextColorSelected.R = (byte)Node.TextColorSelected.Numbers[0];
                TextColorSelected.G = (byte)Node.TextColorSelected.Numbers[1];
                TextColorSelected.B = (byte)Node.TextColorSelected.Numbers[2];
            }
            else
            {
                TextColorSelected = Result.State.TextColorSelected;
                TextColorSelected.A = 255;
            }

            if (Node.TextColorHighlighted != null)
            {
                TextColorHighlighted = new Color();
                TextColorHighlighted.A = 255;
                TextColorHighlighted.R = (byte)Node.TextColorHighlighted.Numbers[0];
                TextColorHighlighted.G = (byte)Node.TextColorHighlighted.Numbers[1];
                TextColorHighlighted.B = (byte)Node.TextColorHighlighted.Numbers[2];
            }
            else
            {
                TextColorHighlighted = Result.State.TextColorHighlighted;
                TextColorHighlighted.A = 255;
            }

            if (Node.TextColorDisabled != null)
            {
                TextColorDisabled = new Color();
                TextColorDisabled.A = 255;
                TextColorDisabled.R = (byte)Node.TextColorDisabled.Numbers[0];
                TextColorDisabled.G = (byte)Node.TextColorDisabled.Numbers[1];
                TextColorDisabled.B = (byte)Node.TextColorDisabled.Numbers[2];
            }
            else
            {
                TextColorDisabled = Result.State.TextColorDisabled;
                TextColorDisabled.A = 255;
            }

            if (Node.TextButton != null)
                m_IsTextButton = (Node.TextButton == 1) ? true : false;
            else
                m_IsTextButton = m_IsTextButton = Result.State.TextButton;


            if (m_IsTextButton)
            {
                if (Node.Text != string.Empty)
                    m_Text = Result.Strings[Node.Text];
                else
                    m_Text = Result.Strings[Result.State.Caption];

                if (m_Size.X != 0)
                    ScaleToText();
            }

            if (Node.Tooltip != "")
                Tooltip = Result.Strings[Node.Tooltip];

            if (Node.Tracking != null)
                Tracking = (int)Node.Tracking;
        }

        /// <summary>
        /// Constructs a new UIButton instance.
        /// </summary>
        /// <param name="Name">Name of button.</param>
        /// <param name="Position">Button's position.</param>
        /// <param name="Screen">This button's screen.</param>
        /// <param name="Caption">The caption for this button (optional).</param>
        /// <param name="Font">The size of the caption's font (optional).</param>
        /// <param name="Tex">Texture used to display this button (optional).</param>
        /// <param name="ScaleToText">Should the button be scaled to fit the caption?</param>
        /// <param name="Parent">The parent of this UIButton.</param>
        public UIButton(string Name, Vector2 Pos, UIScreen Screen, Texture2D Tex = null,
            string Caption = "", int Font = 9, bool ScaleToText = true, UIElement Parent = null) : base(Screen, Parent)
        {
            base.Name = Name;
            Position = Pos;

            if (Parent != null)
            {
                //Would a text edit ever be attached to anything but a UIDialog instance? Probably not.
                UIDialog Dialog = (UIDialog)Parent;
                Dialog.OnDragged += Dialog_OnDragged;
            }

            if (Tex != null)
                Image = new UIImage(Tex, new Vector2(Pos.X, Pos.Y), Screen, null);
            else
                Image = new UIImage(FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.buttontiledialog), new Vector2(Pos.X, Pos.Y), 
                    m_Screen);

            //Initialize to second frame in the image.
            m_SourcePosition = new Vector2((Image.Texture.Width / 4) * 2, 0.0f);

            if (Caption != "")
            {
                m_IsTextButton = true;
                m_Text = Caption;

                TextColor = m_Screen.StandardTxtColor; //TODO: Find out how to pass optional color as a parameter.

                switch (Font)
                {
                    case 9:
                        m_Font = Screen.Font9px;
                        break;
                    case 10:
                        m_Font = Screen.Font10px;
                        break;
                    case 12:
                        m_Font = Screen.Font12px;
                        break;
                    case 14:
                        m_Font = Screen.Font14px;
                        break;
                    case 16:
                        m_Font = Screen.Font16px;
                        break;
                    default:
                        m_Font = Screen.Font12px;
                        break;
                }
            }

            m_Size = new Vector2();
            m_Size.X = Image.Texture.Width / 4;
            m_Size.Y = Image.Texture.Height;
        }

        /// <summary>
        /// Scales a button to the size of the button's text.
        /// Also repositions a button's text according to the new size.
        /// </summary>
        private void ScaleToText()
        {
            if (m_Font.MeasureString(m_Text).X > m_Size.X)
                m_XScale = m_Font.MeasureString(m_Text).X / m_Size.X;
            else if(m_Font.MeasureString(m_Text).X <= m_Size.X)
                m_XScale = m_Size.X / m_Font.MeasureString(m_Text).X;

            m_XScale += 0.5f; //Text margin.

            float HalfX = m_Size.X / 2;
            float HalfY = m_Size.Y / 2; 
            m_TextPosition.X += (HalfX * m_XScale) - (m_Font.MeasureString(m_Text).X / 2);
            m_TextPosition.Y += HalfY - (m_Font.MeasureString(m_Text).Y / 2);
        }

        /// <summary>
        /// Is the mouse cursor over this UIButton instance?
        /// </summary>
        /// <param name="Input">A InputHelper instance that provides the mouse cursors' position.</param>
        /// <returns></returns>
        public override bool IsMouseOver(InputHelper Input)
        {
            if (Input.MousePosition.X > Position.X && Input.MousePosition.X <= (Position.X + (m_Size.X * m_XScale)))
            {
                if (Input.MousePosition.Y > Position.Y && Input.MousePosition.Y <= (Position.Y + m_Size.Y))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Adds an image to this button, because (you guessed it) sometimes
        /// buttons will be defined without images...
        /// </summary>
        /// <param name="Img">The image to add.</param>
        public void AddImage(UIImage Img)
        {
            Image = Img;
            Image.Position = Position;

            m_Size = new Vector2();
            m_Size.X = Image.Texture.Width / 4;
            m_Size.Y = Image.Texture.Height;

            //Initialize to second frame in the image.
            m_SourcePosition = new Vector2((Image.Texture.Width / 4) * 2, 0.0f);

            if (m_Text != null)
                ScaleToText();
        }

        /// <summary>
        /// This UIButton instance is attached to a dialog, and the dialog is being dragged.
        /// </summary>
        /// <param name="MousePosition">The mouse position.</param>
        /// <param name="DragOffset">The dialog's drag offset.</param>
        private void Dialog_OnDragged(Vector2 MousePosition, Vector2 DragOffset)
        {
            Vector2 RelativePosition = Position - m_Parent.Position;

            Position = (MousePosition + RelativePosition) - DragOffset;
        }

        public override void Update(InputHelper Input, GameTime GTime)
        {
            if (m_IsTextButton)
            {
                //Make the text stick to this button if/when the button is moved.
                m_TextPosition = Position;

                float HalfX = m_Size.X / 2;
                float HalfY = m_Size.Y / 2;
                m_TextPosition.X += (HalfX * m_XScale) - (m_Font.MeasureString(m_Text).X / 2);
                m_TextPosition.Y += HalfY - (m_Font.MeasureString(m_Text).Y / 2);
            }

            if(IsMouseOver(Input) || PixelCheck(Input, (int)m_Size.X))
            {
                if (Input.IsNewPress(MouseButtons.LeftButton))
                {
                    if (!m_IsButtonClicked && Enabled)
                    {
                        TextDrawingColor = TextColorHighlighted;
                        m_SourcePosition.X += m_Size.X;

                        OnButtonClicked?.Invoke(this, new ButtonClickEventArgs(MouseButtons.LeftButton));

                        m_IsButtonClicked = true;
                    }
                    else if(Enabled == false)
                        m_SourcePosition.X = m_Size.X * 3;
                }
                else
                {
                    if (m_IsButtonClicked)
                    {
                        TextDrawingColor = TextColorSelected;
                        m_SourcePosition.X -= m_Size.X;
                    }

                    m_IsButtonClicked = false;
                }

                if (!m_IsMouseHovering && Enabled)
                {
                    TextDrawingColor = TextColorSelected;
                    m_SourcePosition.X -= m_Size.X;
                    m_IsMouseHovering = true;
                }
            }
            else
            {
                if (Enabled)
                {
                    TextDrawingColor = TextColor;
                    m_SourcePosition.X = (m_Size.X * 2);
                    m_IsMouseHovering = false;
                }
                else
                    m_SourcePosition.X = (m_Size.X * 3);
            }

            base.Update(Input, GTime);
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            if (Visible)
            {
                float Depth;
                if (LayerDepth != null)
                    Depth = (float)LayerDepth;
                else
                    Depth = 0.9f; //Buttons are always drawn on top.

                if (Image != null && Image.Loaded)
                {
                    Image.Draw(SBatch, new Rectangle((int)m_SourcePosition.X, 
                        (int)m_SourcePosition.Y, (int)m_Size.X, (int)m_Size.Y), 
                        Depth, new Vector2(m_XScale, 1.0f));
                }

                if (m_IsTextButton)
                {
                    //TODO: What depth should be used to have the text draw on top?
                    SBatch.DrawString(m_Font, m_Text, m_TextPosition, TextDrawingColor, 0.0f,
                        new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, Depth + 0.1f);
                }
            }
        }

        ~UIButton()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this UIButton instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this UIButton instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (Image != null)
                    Image.Dispose();

                // Prevent the finalizer from calling ~UIButton, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("UIButton not explicitly disposed!");
        }
    }
}
