/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Files;
using Files.Manager;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Elements
{
    public delegate void ButtonClickDelegate(object Sender);

    /// <summary>
    /// A clickable button that can trigger an event.
    /// A button is always graphically represented by four equally sized frames in a texture.
    /// </summary>
    public class UIButton : UIElement
    {
        private Vector2 m_SourcePosition;
        private bool m_IsMouseHovering = false;
        private bool m_IsTextHighlighted = false;

        private bool m_IsTextButton = false;
        private string m_Text;
        private Vector2 m_TextPosition;
        private float m_XScale = 1.0f; //Used to scale buttons to fit text.

        /// <summary>
        /// Is this button enabled (I.E not greyed out?)
        /// </summary>
        public bool Enabled = true;

        public bool m_IsButtonClicked = false;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
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
                    Image = new UIImage(FileManager.GetTexture((ulong)FileIDs.UIFileIDs.buttontiledialog), m_Screen);
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
                        Image = new UIImage(FileManager.GetTexture((ulong)FileIDs.UIFileIDs.buttontiledialog), m_Screen);
                        //Initialize to second frame in the image.
                        if(Result.State.Size == null)
                            m_SourcePosition = new Vector2((Image.Texture.Width / 4) * 2, 0.0f);

                        m_Size = new Vector2();
                        m_Size.X = Image.Texture.Width / 4;
                        m_Size.Y = Image.Texture.Height;
                    }
                    else
                    {
                        Image = m_Screen.GetImage(Node.Image, false);
                        //Initialize to second frame in the image.
                        m_SourcePosition = new Vector2((Image.Texture.Width / (4)) * 2, 0.0f);

                        m_Size = new Vector2();
                        m_Size.X = (Image.Texture.Width) / (4);
                        m_Size.Y = Image.Texture.Height;
                    }

                    Position = new Vector2(Node.ButtonPosition.Numbers[0], Node.ButtonPosition.Numbers[1]) + m_Screen.Position;
                }

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

                m_TextPosition = Position;

                if (m_Size.X != 0)
                {
                    //if (Result.State.Size == null)
                        ScaleToText();
                }
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
        /// <param name="Tex">Texture used to display this button.</param>
        /// <param name="Position">Button's position.</param>
        /// <param name="Screen">This button's screen.</param>
        public UIButton(string Name, Texture2D Tex, Vector2 Pos, UIScreen Screen, UIElement Parent = null) : base(Screen, Parent)
        {
            base.Name = Name;
            Position = Pos;

            Image = new UIImage(Tex, Screen, null);
            Image.Position = new Vector2(Pos.X, Pos.Y);
            //Initialize to second frame in the image.
            m_SourcePosition = new Vector2((Tex.Width / 4) * 2, 0.0f);

            m_Size = new Vector2();
            m_Size.X = Tex.Width / 4;
            m_Size.Y = Tex.Height;
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
        /// Draws a border around this button, for debugging purposes.
        /// </summary>
        /// <param name="SBatch">A Spritebatch to draw with.</param>
        /// <param name="rectangleToDraw">A rectangle that will make up the border.</param>
        /// <param name="thicknessOfBorder">Thickness of border to be drawn.</param>
        /// <param name="borderColor">Color of border.</param>
        public void DrawBorder(SpriteBatch SBatch, Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // At the top of your class:
            Texture2D pixel;

            // Somewhere in your LoadContent() method:
            pixel = new Texture2D(m_Screen.Manager.Graphics, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White }); // so that we can draw whatever color we want on top of it

            // Draw top line
            SBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            SBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            SBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            SBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }

        public override void Update(InputHelper Input, GameTime GTime)
        {
            if(m_IsTextButton)
            {
                m_TextPosition = Position;

                if (m_Size.X != 0)
                    ScaleToText();
            }

            if(IsMouseOver(Input) || PixelCheck(Input, (int)m_Size.X))
            {
                if (Input.IsNewPress(MouseButtons.LeftButton))
                {
                    if (!m_IsButtonClicked && Enabled)
                    {
                        TextDrawingColor = TextColorHighlighted;
                        m_SourcePosition.X += m_Size.X;

                        OnButtonClicked?.Invoke(this);

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
                        new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, Depth);
                }
            }
        }
    }
}
