using System;
using System.Collections.Generic;
using System.Text;
using Files.Manager;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gonzo.Elements
{
    public delegate void ButtonClickDelegate(UIButton ClickedButton);

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

        public bool m_IsButtonClicked = false;
        public event ButtonClickDelegate OnButtonClicked;

        public UIButton(AddButtonNode Node, ParserState State, UIScreen Screen) : base(Screen)
        {
            m_Name = Node.Name;
            m_ID = Node.ID;
            m_Screen = Screen;

            Position = new Vector2();
            Position = new Vector2(Node.ButtonPosition.Numbers[0], Node.ButtonPosition.Numbers[1]) + m_Screen.Position;
            Position *= m_Screen.Scale;

            if (Node.ButtonSize != null)
            {
                m_Size = new Vector2();
                m_Size.X = Node.ButtonSize.Numbers[0];
                m_Size.Y = Node.ButtonSize.Numbers[1];
                m_Size *= m_Screen.Scale;

                m_SourcePosition = new Vector2(m_Size.X * 2, 0.0f);
            }

            if (Node.Image != null)
            {
                Image = m_Screen.GetImage(Node.Image);
                m_SourcePosition = new Vector2((Image.Texture.Width / (4 * Screen.Scale.X)) * 2, 0.0f);

                m_Size = new Vector2();
                m_Size.X = (Image.Texture.Width * Screen.Scale.X) / (4 * Screen.Scale.X);
                m_Size.Y = Image.Texture.Height;

                Image.Position = new Vector2(Position.X, Position.Y);
                m_Elements.Add("Background", Image);
            }

            if (Node.TextHighlighted != null)
                m_IsTextHighlighted = (Node.TextHighlighted == 1) ? true : false;

            if (Node.Font != null)
            {
                int FontSize = (int)Node.Font;

                switch (FontSize)
                {
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
            else
                m_Font = Screen.Font12px;

            if (Node.TextColor != null)
            {
                TextColor = new Color();
                TextColor.R = (byte)Node.TextColor.Numbers[0];
                TextColor.G = (byte)Node.TextColor.Numbers[1];
                TextColor.B = (byte)Node.TextColor.Numbers[2];
            }
            else
                TextColor = Color.White;

            if (Node.TextColorSelected != null)
            {
                TextColor = new Color();
                TextColorSelected.R = (byte)Node.TextColorSelected.Numbers[0];
                TextColorSelected.G = (byte)Node.TextColorSelected.Numbers[1];
                TextColorSelected.B = (byte)Node.TextColorSelected.Numbers[2];
            }

            if (Node.TextColorHighlighted != null)
            {
                TextColor = new Color();
                TextColorHighlighted.R = (byte)Node.TextColorHighlighted.Numbers[0];
                TextColorHighlighted.G = (byte)Node.TextColorHighlighted.Numbers[1];
                TextColorHighlighted.B = (byte)Node.TextColorHighlighted.Numbers[2];
            }

            if (Node.TextColorDisabled != null)
            {
                TextColor = new Color();
                TextColor.R = (byte)Node.TextColorDisabled.Numbers[0];
                TextColor.G = (byte)Node.TextColorDisabled.Numbers[1];
                TextColor.B = (byte)Node.TextColorDisabled.Numbers[2];
            }

            if(Node.TextButton != null)
                m_IsTextButton = (Node.TextButton == 1) ? true : false;

            if (m_IsTextButton)
            {
                m_Text = m_Screen.GetString(Node.Text);
                m_TextPosition = Position;

                if (m_Size.X != 0)
                {
                    m_TextPosition.X += (m_Size.X / 2) - (m_Font.MeasureString(m_Text).X / 2);
                    m_TextPosition.Y += (m_Size.Y / 2) - (m_Font.MeasureString(m_Text).Y / 2);
                }
            }

            if (Node.Tooltip != "")
                Tooltip = m_Screen.GetString(Node.Tooltip);

            if (Node.Tracking != null)
                Tracking = (int)Node.Tracking;

            if(State.InSharedPropertiesGroup)
            {
                if (State.Image != "")
                {
                    Image = m_Screen.GetImage(State.Image);
                    Image.Position = new Vector2(Position.X, Position.Y);
                    m_SourcePosition = new Vector2((Image.Texture.Width / (4 * Screen.Scale.X)) * 2, 0.0f);
                }

                if (State.Tooltip != "")
                    Tooltip = m_Screen.GetString(State.Tooltip);
            }
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
            m_Name = Name;
            Position = Pos;

            Image = new UIImage(Tex, Screen, null);
            Image.Position = new Vector2(Pos.X, Pos.Y);
            m_SourcePosition = new Vector2((Tex.Width / (4 * Screen.Scale.X)) * 2, 0.0f);

            m_Elements.Add("Background", Image);

            m_Size = new Vector2();
            m_Size.X = (Tex.Width * Screen.Scale.X) / (4 * Screen.Scale.X);
            m_Size.Y = Tex.Height;
        }

        public override bool IsMouseOver(InputHelper Input)
        {
            if (Input.MousePosition.X > Position.X && Input.MousePosition.X <= (Position.X + m_Size.X))
            {
                if (Input.MousePosition.Y > Position.Y && Input.MousePosition.Y <= (Position.Y + m_Size.Y))
                    return true;
            }

            return false;
        }

        public override void Update(InputHelper Input)
        {
            if(IsMouseOver(Input) || PixelCheck(Input, (int)m_Size.X))
            {
                if (Input.IsNewPress(MouseButtons.LeftButton))
                {
                    if (!m_IsButtonClicked)
                    {
                        TextColor = TextColorSelected;
                        m_SourcePosition.X += m_Size.X;

                        if (OnButtonClicked != null)
                            OnButtonClicked(this);

                        m_IsButtonClicked = true;
                    }
                }
                else
                {
                    if (m_IsButtonClicked)
                    {
                        TextColor = TextColorHighlighted;
                        m_SourcePosition.X -= m_Size.X;
                    }

                    m_IsButtonClicked = false;
                }

                if (!m_IsMouseHovering)
                {
                    TextColor = TextColorSelected;
                    m_SourcePosition.X -= m_Size.X;
                    m_IsMouseHovering = true;
                }
            }
            else
            {
                TextColor = Color.Wheat;
                m_SourcePosition.X = (m_Size.X * 2);
                m_IsMouseHovering = false;
            }
        }

        public override void Draw(SpriteBatch SBatch)
        {
            if (Image != null && Image.Loaded)
            {
                Image.Draw(SBatch, new Rectangle((int)m_SourcePosition.X, (int)m_SourcePosition.Y,
                    (int)m_Size.X, (int)m_Size.Y));
            }

            if (m_IsTextButton)
            {
                SBatch.DrawString(m_Font, m_Text, m_TextPosition, TextColor, 0.0f, new Vector2(0.0f, 0.0f),
                    m_Screen.Scale, SpriteEffects.None, 0.0f);
            }
        }
    }
}
