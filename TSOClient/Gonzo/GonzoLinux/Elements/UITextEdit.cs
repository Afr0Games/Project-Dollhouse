using System;
using System.Collections.Generic;
using System.Text;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gonzo.Elements
{
    public enum TextEditAlignment
    {
        LeftTop = 0,
        LeftCenter = 1,
        CenterTop = 2,
        CenterCenter = 3,
        RightTop = 4,
        RightCenter = 5
    }

    public enum TextEditMode
    {
        Insert = 0,
        ReadOnly = 1
    }

    public enum ScrollbarType
    {
        Slider = 0,
        Scrollbar = 1
    }

    public class UITextEdit : UIElement
    {
        public bool Transparent = false;
        private int m_NumLines = 0;
        private int m_MaxChars = 0;
        private TextEditAlignment m_Alignment = 0;
        private bool m_FlashOnEmpty = false;
        
        /// <summary>
        /// Defines whether this text edit control will receive a border on focus. 1=Yes, 0=No.Default is yes.
        /// </summary>
        private bool m_FrameOnFocus = true;

        private Vector2 m_TextPosition = new Vector2(0.0f, 0.0f);
        private Color m_BackColor, m_CursorColor, m_FrameColor;
        private Vector2 m_CursorPosition = new Vector2(0.0f, 0.0f);
        private TextEditMode m_Mode;
        private Texture2D m_ScrollbarImage;
        private int m_ScrollbarWidth = 0, m_ScrollbarHeight = 0;
        private ScrollbarType m_ScrollbarType;
        private bool m_ResizeForExactLineHeight = false;
        private bool m_EnableInputModeEditing = false;
        private bool m_HasFocus = false;

        private StringBuilder m_SBuilder = new StringBuilder();

        private List<StringBuilder> m_Lines = new List<StringBuilder>();

        public UITextEdit(AddTextEditNode Node, ParserState State, UIScreen Screen) : base(Screen)
        {
            m_Name = Node.Name;
            m_ID = Node.ID;
            Position = new Vector2();
            Position = new Vector2(Node.TextEditPosition.Numbers[0], Node.TextEditPosition.Numbers[1]) + Screen.Position;

            m_Size = new Vector2();
            m_Size.X = Node.Size.Numbers[0];
            m_Size.Y = Node.Size.Numbers[1];
            m_Size *= m_Screen.Scale;

            if (Node.Tooltip != "")
                Tooltip = m_Screen.GetString(Node.Tooltip);

            Transparent = (Node.Transparent == 1) ? true : false;

            if (Node.Lines != null)
                m_NumLines = (int)Node.Lines;

            if (Node.Capacity != null)
                m_MaxChars = (int)Node.Capacity;

            if(Node.Alignment != null)
                m_Alignment = (TextEditAlignment)Node.Alignment;

            if (Node.FlashOnEmpty != null)
                m_FlashOnEmpty = (Node.FlashOnEmpty == 1) ? true : false;

            if (Node.FrameOnFocus != null)
                m_FrameOnFocus = (Node.FrameOnFocus == 1) ? true : false;

            TextColor = new Color(Node.Color.Numbers[0], Node.Color.Numbers[1], Node.Color.Numbers[2]);

            if (Node.BackColor != null)
            {
                m_BackColor = new Color(Node.Color.Numbers[0], Node.Color.Numbers[1], Node.Color.Numbers[2]);
                Image = new UIImage(TextureUtils.CreateRectangle(Screen.Manager.Graphics, 
                    (int)m_Size.X, (int)m_Size.Y, m_BackColor), m_Screen);
            }
            else
            {
                m_BackColor = new Color(57, 81, 110, 255);
                Image = new UIImage(TextureUtils.CreateRectangle(Screen.Manager.Graphics,
                    (int)m_Size.X, (int)m_Size.Y, m_BackColor), m_Screen);
            }

            if (Node.Mode != null)
                m_Mode = (Node.Mode == "kInsert") ? TextEditMode.Insert : TextEditMode.ReadOnly;

            if (Node.ScrollbarImage != null)
                m_ScrollbarImage = m_Screen.GetImage(Node.ScrollbarImage).Image.Texture;

            if (Node.ScrollbarGutter != null)
                m_ScrollbarWidth = (int)Node.ScrollbarGutter;

            if (Node.ScrollbarType != null)
                m_ScrollbarType = (ScrollbarType)Node.ScrollbarType;

            if (Node.ResizeForExactLineHeight != null)
                m_ResizeForExactLineHeight = (Node.ResizeForExactLineHeight == 1) ? true : false;

            if (Node.EnableIME != null)
                m_EnableInputModeEditing = (Node.EnableIME == 1) ? true : false;

            if (Node.CursorColor != null)
                m_CursorColor = new Color(Node.CursorColor.Numbers[0], Node.CursorColor.Numbers[1], Node.CursorColor.Numbers[2]);

            if (Node.FrameColor != null)
                m_FrameColor = new Color(Node.FrameColor.Numbers[0], Node.FrameColor.Numbers[1], Node.FrameColor.Numbers[2]);

            switch(Node.Font)
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

            if (State != null)
            {
                if (State.BackColor != null)
                    m_BackColor = State.BackColor;
                if (State.Color != null)
                    TextColor = State.Color;
                if (State.CursorColor != null)
                    m_CursorColor = State.CursorColor;
                if (State.Position != null)
                    Position = new Vector2(State.Position[0], State.Position[1]);
                if (State.Tooltip != "")
                    Tooltip = State.Tooltip;
            }
        }

        /// <summary>
        /// Gets the current text in this UITextEdit instance.
        /// </summary>
        public string CurrentInput
        {
            get
            {
                string InputStr = "";
                foreach (StringBuilder SBuilder in m_Lines)
                    InputStr += SBuilder.ToString();

                return InputStr;
            }
        }

        public override void Update(InputHelper Input)
        {
            if(IsMouseOver(Input))
            {
                if (Input.IsCurPress(MouseButtons.LeftButton))
                    m_HasFocus = true;
            }

            if (m_NumLines > 0)
            {
                //Check that text doesn't go beyond width of control...
                if (m_Font.MeasureString(m_SBuilder.ToString()).X >= m_Size.X)
                {
                    if ((m_Lines.Count <= m_NumLines) && (m_Lines.Count < m_Size.Y))
                    {
                        m_Lines.Add(m_SBuilder);
                        m_SBuilder = new StringBuilder();
                        m_TextPosition.Y += m_Font.LineSpacing;
                    }
                    else //Text went beyond the borders of the control...
                    {
                        m_Lines.Add(m_SBuilder);
                        m_SBuilder = new StringBuilder();
                        m_TextPosition.Y += m_Font.LineSpacing;
                        m_ScrollbarHeight += m_Font.LineSpacing;
                    }
                }
            }
            else
            {
                //TODO: Scroll text backwards...
            }

            if(m_HasFocus)
            {
                bool Upper = false;
                Keys[] PressedKeys = Input.CurrentKeyboardState.GetPressedKeys();

                if (IsUpperCase(PressedKeys))
                    Upper = true;

                foreach(Keys K in PressedKeys)
                {
                    if (Input.IsNewPress(K))
                    {
                        switch (K)
                        {
                            case Keys.A:
                                if (Upper)
                                    m_SBuilder.Append("A");
                                else
                                    m_SBuilder.Append("a");
                                break;
                            case Keys.B:
                                if (Upper)
                                    m_SBuilder.Append("B");
                                else
                                    m_SBuilder.Append("b");
                                break;
                            case Keys.C:
                                if (Upper)
                                    m_SBuilder.Append("C");
                                else
                                    m_SBuilder.Append("c");
                                break;
                            case Keys.D:
                                if (Upper)
                                    m_SBuilder.Append("D");
                                else
                                    m_SBuilder.Append("d");
                                break;
                            case Keys.E:
                                if (Upper)
                                    m_SBuilder.Append("F");
                                else
                                    m_SBuilder.Append("f");
                                break;
                            case Keys.F:
                                if (Upper)
                                    m_SBuilder.Append("F");
                                else
                                    m_SBuilder.Append("f");
                                break;
                            case Keys.G:
                                if (Upper)
                                    m_SBuilder.Append("G");
                                else
                                    m_SBuilder.Append("g");
                                break;
                            case Keys.H:
                                if (Upper)
                                    m_SBuilder.Append("H");
                                else
                                    m_SBuilder.Append("h");
                                break;
                            case Keys.I:
                                if (Upper)
                                    m_SBuilder.Append("I");
                                else
                                    m_SBuilder.Append("i");
                                break;
                            case Keys.J:
                                if (Upper)
                                    m_SBuilder.Append("J");
                                else
                                    m_SBuilder.Append("j");
                                break;
                            case Keys.K:
                                if (Upper)
                                    m_SBuilder.Append("K");
                                else
                                    m_SBuilder.Append("k");
                                break;
                            case Keys.L:
                                if (Upper)
                                    m_SBuilder.Append("L");
                                else
                                    m_SBuilder.Append("l");
                                break;
                            case Keys.M:
                                if (Upper)
                                    m_SBuilder.Append("M");
                                else
                                    m_SBuilder.Append("m");
                                break;
                            case Keys.N:
                                if (Upper)
                                    m_SBuilder.Append("N");
                                else
                                    m_SBuilder.Append("n");
                                break;
                            case Keys.O:
                                if (Upper)
                                    m_SBuilder.Append("O");
                                else
                                    m_SBuilder.Append("o");
                                break;
                            case Keys.P:
                                if (Upper)
                                    m_SBuilder.Append("P");
                                else
                                    m_SBuilder.Append("p");
                                break;
                            case Keys.Q:
                                if (Upper)
                                    m_SBuilder.Append("R");
                                else
                                    m_SBuilder.Append("r");
                                break;
                            case Keys.S:
                                if (Upper)
                                    m_SBuilder.Append("S");
                                else
                                    m_SBuilder.Append("S");
                                break;
                            case Keys.T:
                                if (Upper)
                                    m_SBuilder.Append("T");
                                else
                                    m_SBuilder.Append("T");
                                break;
                            case Keys.U:
                                if (Upper)
                                    m_SBuilder.Append("U");
                                else
                                    m_SBuilder.Append("u");
                                break;
                            case Keys.V:
                                if (Upper)
                                    m_SBuilder.Append("V");
                                else
                                    m_SBuilder.Append("v");
                                break;
                            case Keys.W:
                                if (Upper)
                                    m_SBuilder.Append("W");
                                else
                                    m_SBuilder.Append("w");
                                break;
                            case Keys.X:
                                if (Upper)
                                    m_SBuilder.Append("X");
                                else
                                    m_SBuilder.Append("X");
                                break;
                            case Keys.Y:
                                if (Upper)
                                    m_SBuilder.Append("Y");
                                else
                                    m_SBuilder.Append("y");
                                break;
                            case Keys.Z:
                                if (Upper)
                                    m_SBuilder.Append("Z");
                                else
                                    m_SBuilder.Append("z");
                                break;
                                //TODO: Support more keys?
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch SBatch)
        {
            int Height = (int)m_TextPosition.Y;

            Image.Draw(SBatch, null);

            if (m_ScrollbarImage != null)
                SBatch.Draw(m_ScrollbarImage,new Vector2(m_Size.X - m_ScrollbarWidth, 0), null, Color.White, 0.0f, 
                    new Vector2(0.0f, 0.0f), new Vector2(0.0f, m_ScrollbarHeight * m_Screen.Scale.Y), SpriteEffects.None, 0.0f);

            foreach(StringBuilder SBuilder in m_Lines)
            {
                SBatch.DrawString(m_Font, SBuilder.ToString(), new Vector2(m_TextPosition.X, Height), TextColor,
                    0.0f, new Vector2(0.0f, 0.0f), m_Screen.Scale, SpriteEffects.None, 0);
                Height += m_Font.LineSpacing;
            }

            SBatch.DrawString(m_Font, m_SBuilder.ToString(), new Vector2(m_TextPosition.Y, Height), TextColor);
        }

        /// <summary>
        /// Returns true if shift is currently being pressed.
        /// </summary>
        /// <param name="PressedKeys">A list of keys currently being pressed.</param>
        /// <returns>True if it is being pressed, false otherwise.</returns>
        private bool IsUpperCase(Keys[] PressedKeys)
        {
            foreach(Keys K in PressedKeys)
            {
                if (K == Keys.LeftShift || K == Keys.RightShift)
                    return true;
            }

            return false;
        }
    }
}
