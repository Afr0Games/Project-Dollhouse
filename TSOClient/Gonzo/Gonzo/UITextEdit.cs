using System;
using System.Collections.Generic;
using System.Text;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gonzo
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

        private Color m_Color, m_BackColor, m_CursorColor, m_FrameColor;
        private Vector2 m_CursorPosition = new Vector2(0.0f, 0.0f);
        private int m_FontSize = 0;
        private TextEditMode m_Mode;
        private Texture2D m_ScrollbarImage;
        private int m_ScrollbarWidth = 0;
        private ScrollbarType m_ScrollbarType;
        private bool m_ResizeForExactLineHeight = false;
        private bool m_EnableInputModeEditing = false;

        private StringBuilder m_SBuilder = new StringBuilder();

        public UITextEdit(AddTextEditNode Node, ParserState State, UIElement Parent) : base(Parent)
        {
            m_Name = Node.Name;
            m_ID = Node.ID;
            m_Position = new Vector2();
            m_Position = Vector2.Transform(new Vector2(Node.TextEditPosition.Numbers[0], Node.TextEditPosition.Numbers[1]),
                m_Parent.PositionMatrix);

            m_Size = new Vector2();
            m_Size.X = Node.Size.Numbers[0];
            m_Size.Y = Node.Size.Numbers[1];

            if (Node.Tooltip != "")
                Tooltip = m_Parent.GetString(Node.Tooltip);

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

            m_Color = new Color(Node.Color.Numbers[0], Node.Color.Numbers[1], Node.Color.Numbers[2]);

            if (Node.BackColor != null)
                m_BackColor = new Color(Node.Color.Numbers[0], Node.Color.Numbers[1], Node.Color.Numbers[2]);

            if (Node.Mode != null)
                m_Mode = (Node.Mode == "kInsert") ? TextEditMode.Insert : TextEditMode.ReadOnly;

            if (Node.ScrollbarImage != null)
                m_ScrollbarImage = m_Parent.GetChild(Node.ScrollbarImage).Image;

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

            m_FontSize = (int)Node.Font;

            if (State != null)
            {
                if (State.BackColor != null)
                    m_BackColor = State.BackColor;
                if (State.Color != null)
                    m_Color = State.Color;
                if (State.CursorColor != null)
                    m_CursorColor = State.CursorColor;
                if (State.Position != null)
                    m_Position = new Vector2(State.Position[0], State.Position[1]);
                if (State.Tooltip != "")
                    Tooltip = State.Tooltip;
            }
        }

        public override void Update(InputHelper Input)
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
