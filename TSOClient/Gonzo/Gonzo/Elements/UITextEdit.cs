/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Timers;
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

    /// <summary>
    /// Represents a line of renderable text.
    /// </summary>
    internal class RenderableText
    {
        public StringBuilder SBuilder = new StringBuilder();
        public Vector2 Position;
        public bool Visible = true;
    }

    internal class TextCursor
    {
        public string Cursor = "|";
        public Vector2 Position;
        public bool Visible = false;
        public int LineIndex = 0;       //Which line is the cursor on?
        public int CharacterIndex = 0;  //Which character is the cursor next to?
    }

    public class UITextEdit : UIElement, IDisposable
    {
        public bool Transparent = false;
        private int m_NumLines = 0;
        private int m_MaxChars = 0;
        private TextEditAlignment m_Alignment = 0;
        private bool m_FlashOnEmpty = false;

        //This is used to index the current line of text to turn invisible when text is scrolling.
        private int m_VisibilityIndex = 0;

        private Timer m_CursorVisibilityTimer = new Timer();
        private TextCursor m_Cursor = new TextCursor();
        
        /// <summary>
        /// Defines whether this text edit control will receive a border on focus. 1=Yes, 0=No.Default is yes.
        /// </summary>
        private bool m_FrameOnFocus = true;

        private Vector2 m_TextPosition = new Vector2(0.0f, 0.0f);
        private Color m_BackColor, m_CursorColor, m_FrameColor;
        private TextEditMode m_Mode;
        private Texture2D m_ScrollbarImage;
        private int m_ScrollbarWidth = 0, m_ScrollbarHeight = 0;
        private ScrollbarType m_ScrollbarType;
        private bool m_ResizeForExactLineHeight = false;
        private bool m_EnableInputModeEditing = false;
        private bool m_MovingCursor = false; //Is cursor being moved?

        //Is the user removing text (I.E pressing backspace)?
        private bool m_RemovingTxt = false;
        //The length of the current line of text being written.
        private int m_VerticalTextBoundary = 0;

        private bool m_IsUpperCase = false; //Is shift currently being pressed?

        //Factor by which to scroll characters vertically.
        private int m_ScrollFactor = 3;

        private List<RenderableText> m_Lines = new List<RenderableText>();

        public UITextEdit(AddTextEditNode Node, ParserState State, UIScreen Screen) : 
            base(Screen)
        {
            Name = Node.Name;
            m_ID = Node.ID;
            m_KeyboardInput = true; //UITextEdit needs to receive input from keyboard!

            if (!State.InSharedPropertiesGroup)
            {
                if (Node.TextEditPosition.Numbers.Count > 0)
                {
                    Position = new Vector2(Node.TextEditPosition.Numbers[0], Node.TextEditPosition.Numbers[1]) + Screen.Position;
                    m_TextPosition = Position;
                }

                if (State.InSharedPropertiesGroup)
                    m_Size = State.Size;
                else
                {
                    m_Size = new Vector2();
                    m_Size.X = Node.Size.Numbers[0];
                    m_Size.Y = Node.Size.Numbers[1];
                }

                if (Node.Tooltip != "")
                    Tooltip = m_Screen.GetString(Node.Tooltip);

                Transparent = (Node.Transparent == 1) ? true : false;

                if (Node.Lines != null)
                    m_NumLines = (int)Node.Lines;

                if (Node.Capacity != null)
                    m_MaxChars = (int)Node.Capacity;

                if (Node.Alignment != null)
                    m_Alignment = (TextEditAlignment)Node.Alignment;

                if (Node.FlashOnEmpty != null)
                    m_FlashOnEmpty = (Node.FlashOnEmpty == 1) ? true : false;

                if (Node.FrameOnFocus != null)
                    m_FrameOnFocus = (Node.FrameOnFocus == 1) ? true : false;

                if (State.InSharedPropertiesGroup)
                    TextColor = State.Color;
                else
                    TextColor = new Color(Node.Color.Numbers[0], Node.Color.Numbers[1], Node.Color.Numbers[2]);

                if (Node.BackColor != null)
                {
                    m_BackColor = new Color(Node.Color.Numbers[0], Node.Color.Numbers[1], Node.Color.Numbers[2]);
                    /*Image = new UIImage(FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_textboxbackground), m_Screen);
                    if(Position != null)
                        Image.Position = Position;
                    Image.Slicer = new NineSlicer(new Vector2(0, 0), (int)Image.Texture.Width, (int)Image.Texture.Width, 15, 15, 15, 15);
                    Image.SetSize((int)Size.X, (int)Size.Y);*/
                }
                else
                {
                    m_BackColor = new Color(57, 81, 110, 255);
                    /*Image = new UIImage(FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_textboxbackground), m_Screen);
                    if(Position != null)
                        Image.Position = Position;
                    Image.Slicer = new NineSlicer(new Vector2(0, 0), (int)Image.Texture.Width, Image.Texture.Height, 15, 15, 15, 15);
                    Image.SetSize((int)Size.X, (int)Size.Y);*/
                }

                if (Node.Mode != null)
                    m_Mode = (Node.Mode == "kReadOnly") ? TextEditMode.ReadOnly : TextEditMode.Insert;

                if (Node.ScrollbarImage != string.Empty)
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
            }
            else
            {
                if (State.BackColor != null)
                    m_BackColor = State.BackColor;
                if (State.Color != null)
                    TextColor = State.Color;
                if (State.CursorColor != null)
                    m_CursorColor = State.CursorColor;
                if (State.Position != null)
                {
                    Position = new Vector2(State.Position[0], State.Position[1]) + Screen.Position;
                    m_TextPosition = Position;
                    //Image.Position = Position;
                }
                if (State.Tooltip != "")
                    Tooltip = State.Tooltip;
            }

            m_Lines.Add(new RenderableText()
            {
                SBuilder = new StringBuilder(),
                Position = m_TextPosition,
                Visible = true
            });

            int Font = 0;
            if (Node.Font != 0)
                Font = Node.Font;
            else
                Font = State.Font;
            
            switch(Font)
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

            m_Cursor.Position = Position;
            m_CursorVisibilityTimer = new Timer(100);
            m_CursorVisibilityTimer.Enabled = true;
            m_CursorVisibilityTimer.Elapsed += M_CursorVisibilityTimer_Elapsed;
            m_CursorVisibilityTimer.Start();

            m_Screen.Manager.OnTextInput += Manager_OnTextInput;
        }

        private void Manager_OnTextInput(object sender, TextInputEventArgs e)
        {
            if (m_HasFocus)
            {
                if (m_Mode != TextEditMode.ReadOnly)
                {
                    if (m_NumLines > 1)
                    {
                        //Check that text doesn't go beyond width of control...
                        if ((m_Font.MeasureString(m_Lines[m_Cursor.LineIndex].SBuilder.ToString()).X >= 
                            m_Size.X) && !m_RemovingTxt && !m_MovingCursor)
                        {
                            if (m_TextPosition.Y <= Position.Y + ((m_NumLines - 2) * m_Font.LineSpacing))
                            {
                                m_TextPosition.Y += m_Font.LineSpacing;
                                m_Lines.Add(new RenderableText() { SBuilder = new StringBuilder(), Position = m_TextPosition });

                                m_Cursor.Position.Y += m_Font.LineSpacing;
                                m_Cursor.LineIndex++;
                                m_Cursor.CharacterIndex = 0;
                            }
                            else //Text went beyond the borders of the control...
                            {
                                foreach (RenderableText Txt in m_Lines)
                                    Txt.Position.Y -= m_Font.LineSpacing;

                                m_Lines.Add(new RenderableText() { SBuilder = new StringBuilder(), Position = m_TextPosition });
                                m_ScrollbarHeight -= m_Font.LineSpacing; //TODO: Resize scrollbar...

                                m_Cursor.LineIndex++;
                                m_Cursor.CharacterIndex = 0;

                                m_Lines[m_VisibilityIndex].Visible = false;
                                m_VisibilityIndex++;
                            }

                            m_Cursor.Position.X = Position.X;
                        }
                    }
                    else
                    {
                        //Text went beyond the borders of the control...
                        if (m_Font.MeasureString(CurrentInput).X >= (m_Size.X - 
                            m_Font.MeasureString(e.Character.ToString()).X) && !m_RemovingTxt)
                        {
                            m_Lines.Add(new RenderableText() { SBuilder = new StringBuilder(), Position = m_Cursor.Position, Visible = true });
                            m_Cursor.Position.X = m_Size.X;
                            //In a single line control, each "line" will hold one character.
                            m_Cursor.LineIndex++;

                            foreach (RenderableText Txt in m_Lines)
                            {
                                Txt.Position.X -= m_Font.MeasureString(e.Character.ToString()).X;

                                if (Txt.Position.X < Position.X)
                                    Txt.Visible = false;
                            }
                        }
                        else
                        {
                            m_Lines.Add(new RenderableText() { SBuilder = new StringBuilder(), Position = m_Cursor.Position, Visible = true});
                            //In a single line control, each "line" will hold one character.
                            m_Cursor.LineIndex++;
                        }
                    }
                }

                if (!m_IsUpperCase)
                {
                    //If the cursor is in the middle of a line, replace the character.
                    if (m_NumLines > 1)
                    {
                        if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                            m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = e.Character;
                        else
                            m_Lines[m_Cursor.LineIndex].SBuilder.Append(e.Character);
                    }
                    else      
                    {
                        if (m_Cursor.CharacterIndex < CurrentInput.Length)
                            m_Lines[m_Cursor.LineIndex].SBuilder[0] = e.Character;
                        else
                        {
                            RenderableText Txt = new RenderableText();
                            Txt.SBuilder = new StringBuilder(e.Character.ToString());
                            Txt.Position = m_Cursor.Position;
                            Txt.Visible = true;
                            m_Lines.Insert(m_Cursor.LineIndex, Txt);
                        }
                    }
                }
                else
                {
                    if (m_NumLines > 1)
                    {
                        //If the cursor is in the middle of a line, replace the character.
                        if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                            m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = e.Character.ToString().ToUpper().ToCharArray()[0];
                        else
                            m_Lines[m_Cursor.LineIndex].SBuilder.Append(e.Character.ToString().ToUpper());
                    }
                    else
                    {
                        if ((m_Cursor.CharacterIndex < CurrentInput.Length) && m_MovingCursor)
                            m_Lines[m_Cursor.LineIndex].SBuilder[0] = e.Character;
                        else
                        {
                            RenderableText Txt = new RenderableText();
                            Txt.SBuilder = new StringBuilder(e.Character.ToString().ToUpper());
                            Txt.Position = m_Cursor.Position;
                            Txt.Visible = true;
                            m_Lines.Insert(m_Cursor.LineIndex, Txt);
                        }
                    }
                }

                m_Cursor.CharacterIndex++;
                m_RemovingTxt = false;
                m_MovingCursor = false;
                m_Cursor.Position.X += m_Font.MeasureString(e.Character.ToString()).X;
            }
        }

        private void M_CursorVisibilityTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_HasFocus)
            {
                if (m_Cursor.Visible)
                    m_Cursor.Visible = false;
                else
                    m_Cursor.Visible = true;
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
                foreach (RenderableText Txt in m_Lines)
                    InputStr += Txt.SBuilder.ToString();

                return InputStr;
            }
        }

        public override void Update(InputHelper Input, GameTime GTime)
        {
            base.Update(Input, GTime);

            if (m_Mode != TextEditMode.ReadOnly)
            {
                m_VerticalTextBoundary = (int)(Position.X + m_Font.MeasureString(m_Lines[m_Cursor.LineIndex].SBuilder.ToString()).X);
                m_IsUpperCase = IsUpperCase(Input);

                if (m_HasFocus)
                {
                    Keys[] PressedKeys = Input.CurrentKeyboardState.GetPressedKeys();

                    foreach (Keys K in PressedKeys)
                    {
                        if (Input.IsNewPress(K))
                        {
                            switch (K)
                            {
                                case Keys.Divide:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '/';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("/");

                                    m_Cursor.Position.X += m_Font.MeasureString("/").X;
                                    m_Cursor.CharacterIndex++;

                                    m_RemovingTxt = false;
                                    break;
                                case Keys.OemSemicolon:
                                    if (Input.InputRegion != null)
                                    {
                                        switch (Input.InputRegion.LayoutName)
                                        {
                                            case "English":
                                                if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                                    m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = ';';
                                                else
                                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append(";");

                                                m_Cursor.CharacterIndex++;
                                                m_MovingCursor = false;
                                                m_RemovingTxt = false;
                                                m_Cursor.Position.X += m_Font.MeasureString(";").X;
                                                break;
                                        }
                                    }
                                    break;
                                case Keys.OemQuotes:
                                    if (Input.InputRegion != null)
                                    {
                                        switch (Input.InputRegion.LayoutName)
                                        {
                                            case "English": //TODO: Should this be double quote if upper??
                                                if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                                    m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '\'';
                                                else
                                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("'");

                                                m_Cursor.CharacterIndex++;
                                                m_MovingCursor = false;
                                                m_RemovingTxt = false;
                                                m_Cursor.Position.X += m_Font.MeasureString("'").X;
                                                break;
                                        }
                                    }
                                    break;
                                case Keys.OemCloseBrackets:
                                    if (Input.InputRegion != null)
                                    {
                                        switch (Input.InputRegion.LayoutName)
                                        {
                                            case "English":
                                                if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                                    m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '}';
                                                else
                                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("}");

                                                m_Cursor.Position.X += m_Font.MeasureString("}").X;
                                                break;
                                            case "Norwegian":
                                            case "Swedish":
                                            case "Finnish":
                                            case "Danish":
                                                if (IsUpperCase(Input))
                                                {
                                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '^';
                                                    else
                                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("^");

                                                    m_Cursor.Position.X += m_Font.MeasureString("^").X;
                                                }
                                                else if (Input.IsCurPress(Keys.RightAlt))
                                                {
                                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '~';
                                                    else
                                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("~");

                                                    m_Cursor.Position.X += m_Font.MeasureString("~").X;
                                                }
                                                break;
                                        }
                                    }

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.OemPlus:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '+';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("+");

                                    m_Cursor.Position.X += m_Font.MeasureString("+").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.OemComma:
                                    if (IsUpperCase(Input))
                                    {
                                        if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                            m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = ';';
                                        else
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append(";");

                                        m_Cursor.Position.X += m_Font.MeasureString(";").X;
                                    }
                                    else
                                    {
                                        if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                            m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = ',';
                                        else
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append(",");

                                        m_Cursor.Position.X += m_Font.MeasureString(",").X;
                                    }

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.OemPeriod:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '.';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append(".");

                                    m_Cursor.Position.X += m_Font.MeasureString(".").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.Space:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = ' ';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append(" ");

                                    m_Cursor.Position.X += m_Font.MeasureString(" ").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.Tab:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("   "); //TODO: How to insert tab??
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("   ");

                                    m_Cursor.Position.X += m_Font.MeasureString("   ").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.Subtract:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '-';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("-");

                                    m_Cursor.Position.X += m_Font.MeasureString("-").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.NumPad0:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '0';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("0");

                                    m_Cursor.Position.X += m_Font.MeasureString("0").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.NumPad1:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '1';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("1");

                                    m_Cursor.Position.X += m_Font.MeasureString("1").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.NumPad2:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '2';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("2");

                                    m_Cursor.Position.X += m_Font.MeasureString("2").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.NumPad3:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '3';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("3");

                                    m_Cursor.Position.X += m_Font.MeasureString("3").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.NumPad4:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '4';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("4");

                                    m_Cursor.Position.X += m_Font.MeasureString("4").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.NumPad5:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '5';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("5");

                                    m_Cursor.Position.X += m_Font.MeasureString("5").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.NumPad6:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '6';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("6");

                                    m_Cursor.Position.X += m_Font.MeasureString("6").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.NumPad7:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '7';
                                    m_Cursor.Position.X += m_Font.MeasureString("7").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.NumPad8:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '8';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("8");

                                    m_Cursor.Position.X += m_Font.MeasureString("8").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.NumPad9:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '9';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("9");

                                    m_Cursor.Position.X += m_Font.MeasureString("9").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.Multiply:
                                    if (m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                        m_Lines[m_Cursor.LineIndex].SBuilder[m_Cursor.CharacterIndex] = '*';
                                    else
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("*");

                                    m_Cursor.Position.X += m_Font.MeasureString("*").X;

                                    m_Cursor.CharacterIndex++;
                                    m_MovingCursor = false;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.Enter:
                                    if (m_NumLines > 1)
                                    {
                                        m_TextPosition.X = Position.X;
                                        m_TextPosition.Y += m_Font.LineSpacing;
                                        m_Lines.Add(new RenderableText() { SBuilder = new StringBuilder(), Position = m_TextPosition });

                                        m_Cursor.Position.X = Position.X;
                                        m_Cursor.Position.Y += m_Font.LineSpacing;
                                        m_Cursor.LineIndex += 1;
                                        m_Cursor.CharacterIndex = 0;

                                        m_MovingCursor = false;
                                        m_RemovingTxt = false;
                                    }
                                    break;
                                case Keys.Back:
                                    m_RemovingTxt = true;

                                    //Cursor hasn't been moved.
                                    if (!m_MovingCursor)
                                    {
                                        if (m_Cursor.Position.X <= m_VerticalTextBoundary)
                                            m_Cursor.Position.X = m_VerticalTextBoundary;
                                        else
                                        {
                                            if (m_Cursor.Position.X > Position.X)
                                                m_Cursor.Position.X -= m_Font.MeasureString("a").X;
                                        }
                                    }

                                    //If the current line is empty, move the cursor up.
                                    if (m_Lines[m_Cursor.LineIndex].SBuilder.Length == 0)
                                    {
                                        if (m_NumLines > 1)
                                        {
                                            m_TextPosition.Y -= m_Font.LineSpacing;

                                            m_Cursor.Position.X = Position.X + m_Size.X;
                                            m_Cursor.Position.Y -= m_Font.LineSpacing;

                                            if (m_Cursor.LineIndex > 0)
                                            {
                                                m_Cursor.LineIndex--;
                                                m_Cursor.CharacterIndex = m_Lines[m_Cursor.LineIndex].SBuilder.Length;
                                            }
                                        }
                                        else
                                        {
                                            if (m_Cursor.LineIndex > 0)
                                                m_Cursor.LineIndex--;
                                        }
                                    }
                                    else
                                    {
                                        if (m_NumLines > 1)
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Remove((int)(m_Cursor.CharacterIndex - 1), 1);
                                            m_Cursor.CharacterIndex--;
                                            m_Cursor.Position.X -= m_Font.MeasureString("a").X;
                                        }
                                        else
                                        {
                                            m_Lines.Remove(m_Lines[m_Cursor.CharacterIndex]);
                                            m_Cursor.CharacterIndex--;
                                            m_Cursor.LineIndex--;
                                            m_Cursor.Position.X -= m_Font.MeasureString("a").X;
                                        }
                                    }

                                    //Cursor moved to the beginning of a line.
                                    if (m_Cursor.Position.X <= Position.X)
                                    {
                                        m_TextPosition.Y -= m_Font.LineSpacing;

                                        m_Cursor.Position.X = Position.X + m_Size.X;
                                        m_Cursor.Position.Y -= m_Font.LineSpacing;

                                        if (m_Cursor.LineIndex > 0)
                                        {
                                            m_Cursor.LineIndex--;
                                            m_Cursor.CharacterIndex = m_Lines[m_Cursor.LineIndex].SBuilder.Length;
                                        }
                                    }
                                    break;
                                case Keys.Left:
                                    if (m_Cursor.Position.X > Position.X)
                                    {
                                        m_Cursor.Position.X -= m_Font.MeasureString("a").X;
                                        m_Cursor.CharacterIndex--;

                                        if (m_NumLines == 1)
                                            m_Cursor.LineIndex--;
                                    }
                                    else if (m_Cursor.Position.X <= Position.X)
                                    {
                                        if (m_NumLines == 1)
                                        {
                                            for (int i = 0; i < m_Lines.Count; i++)
                                            {
                                                //Don't know why Line[0] doesn't work here...
                                                if ((m_Lines[1].Position.X < Position.X))
                                                    m_Lines[i].Position.X += m_ScrollFactor;

                                                if (m_Lines[i].Position.X > (Position.X + Size.X))
                                                    m_Lines[i].Visible = false;

                                                if (m_Lines[i].Position.X > Position.X && m_Lines[i].Position.X < (Position.X + Size.X))
                                                    m_Lines[i].Visible = true;
                                            }
                                        }
                                    }

                                    m_MovingCursor = true;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.Right:
                                    if (m_Cursor.Position.X < (Position.X + m_Size.X))
                                    {
                                        if (m_NumLines > 1)
                                        {
                                            if (m_Lines[m_Cursor.LineIndex].SBuilder.Length > 0 &&
                                                m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                            {
                                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                                m_Cursor.CharacterIndex++;
                                            }
                                        }
                                        else //Single-line control, simple.
                                        {
                                            m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                            m_Cursor.CharacterIndex++;
                                            m_Cursor.LineIndex++;
                                        }
                                    }
                                    else if (m_Cursor.Position.X >= (Position.X + m_Size.X))
                                    {
                                        if (m_NumLines == 1)
                                        {
                                            for (int i = 0; i < m_Lines.Count; i++)
                                            {
                                                if ((m_Lines[m_Lines.Count - 1].Position.X >= (Position.X + m_Size.X)))
                                                    m_Lines[i].Position.X -= m_ScrollFactor;

                                                if (m_Lines[i].Position.X < (Position.X + Size.X))
                                                    m_Lines[i].Visible = true;

                                                if (m_Lines[i].Position.X < (Position.X + Size.X) &&
                                                    m_Lines[i].Position.X < Position.X)
                                                    m_Lines[i].Visible = false;
                                            }
                                        }
                                    }

                                    m_MovingCursor = true;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.Up:
                                    if (m_NumLines > 1)
                                    {
                                        if (m_Cursor.Position.Y > Position.Y)
                                        {
                                            m_Cursor.Position.Y -= m_Font.LineSpacing;
                                            m_Cursor.LineIndex--;

                                            //Part of a line was most likely deleted, so readjust the cursor accordingly.
                                            if (m_Cursor.CharacterIndex > m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                            {
                                                m_Cursor.CharacterIndex = m_Lines[m_Cursor.LineIndex].SBuilder.Length;
                                                m_Cursor.Position.X = Position.X +
                                                    m_Font.MeasureString(m_Lines[m_Cursor.LineIndex].SBuilder.ToString()).X;
                                            }
                                        }

                                        m_MovingCursor = true;
                                        m_RemovingTxt = false;
                                    }
                                    break;
                                case Keys.Down:
                                    if (m_NumLines > 1)
                                    {
                                        if (m_Cursor.Position.Y < (Position.Y + m_Size.Y))
                                        {
                                            if (m_Lines.Count >= 2)
                                            {
                                                if (m_Cursor.Position.Y < m_Lines[m_Lines.Count - 1].Position.Y)
                                                {
                                                    m_Cursor.Position.Y += m_Font.LineSpacing;
                                                    m_Cursor.LineIndex++;

                                                    //Part of a line was most likely deleted, so readjust the cursor accordingly.
                                                    if (m_Cursor.CharacterIndex > m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                                    {
                                                        m_Cursor.CharacterIndex = m_Lines[m_Cursor.LineIndex].SBuilder.Length;
                                                        m_Cursor.Position.X = Position.X +
                                                            m_Font.MeasureString(m_Lines[m_Cursor.LineIndex].SBuilder.ToString()).X;
                                                    }
                                                }
                                            }
                                        }

                                        m_MovingCursor = true;
                                        m_RemovingTxt = false;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            if (IsMouseOver(Input))
            {
                if (Input.IsNewPress(MouseButtons.LeftButton) || Input.IsCurPress(MouseButtons.LeftButton))
                {
                    if (m_NumLines > 1)
                    {
                        if (m_Lines.Count > (Input.MousePosition.Y - Position.Y))
                        {
                            if (m_Lines[(int)(Input.MousePosition.Y - Position.Y)].SBuilder.ToString() != "")
                            {
                                m_Cursor.Position = Input.MousePosition;
                                m_Cursor.CharacterIndex = (int)((Input.MousePosition.X - Position.X) - 1);
                                m_Cursor.LineIndex = (int)((Input.MousePosition.Y - Position.Y) - 1);
                                m_MovingCursor = true;
                            }
                        }
                    }
                    else
                    {
                        if ((Position.X + m_Font.MeasureString(CurrentInput).X) > Input.MousePosition.X)
                        {
                            int ApproxChars = (int)(m_Font.MeasureString(CurrentInput).X / m_Lines.Count);
                            int Index = (int)Input.MousePosition.X / ApproxChars;

                            //TODO: Figure out how to access an index in the array based on mouse position.
                            if (m_Lines[Index - 1].SBuilder.ToString() != "")
                            {
                                m_Cursor.Position.X = Input.MousePosition.X;
                                m_Cursor.CharacterIndex = (int)(Index - 1);
                                m_Cursor.LineIndex = (int)(Index);
                                m_MovingCursor = true;
                            }
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.5f;

            int DrawingHeight = (int)m_TextPosition.Y;

            if (Visible)
            {
                /*Image.DrawTextureTo(SBatch, null, Image.Slicer.TLeft, Image.Position + Vector2.Zero, Depth);
                Image.DrawTextureTo(SBatch, Image.Slicer.TCenter_Scale, Image.Slicer.TCenter, Image.Position + new Vector2(Image.Slicer.LeftPadding, 0), Depth);
                Image.DrawTextureTo(SBatch, null, Image.Slicer.TRight, Image.Position + new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, 0), Depth);

                Image.DrawTextureTo(SBatch, Image.Slicer.CLeft_Scale, Image.Slicer.CLeft, Image.Position + new Vector2(0, Image.Slicer.TopPadding), null);
                Image.DrawTextureTo(SBatch, Image.Slicer.CCenter_Scale, Image.Slicer.CCenter, Image.Position + new Vector2(Image.Slicer.LeftPadding, Image.Slicer.TopPadding), Depth);
                Image.DrawTextureTo(SBatch, Image.Slicer.CRight_Scale, Image.Slicer.CRight, Image.Position + new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, Image.Slicer.TopPadding), Depth);

                int BottomY = Image.Slicer.Height - Image.Slicer.BottomPadding;
                Image.DrawTextureTo(SBatch, null, Image.Slicer.BLeft, Image.Position + new Vector2(0, BottomY), null);
                Image.DrawTextureTo(SBatch, Image.Slicer.BCenter_Scale, Image.Slicer.BCenter, Image.Position + new Vector2(Image.Slicer.LeftPadding, BottomY), Depth);
                Image.DrawTextureTo(SBatch, null, Image.Slicer.BRight, Image.Position + new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, BottomY), Depth);*/

                if (m_ScrollbarImage != null)
                    SBatch.Draw(m_ScrollbarImage, new Vector2(m_Size.X - m_ScrollbarWidth, 0), null, Color.White, 0.0f,
                        new Vector2(0.0f, 0.0f), new Vector2(0.0f, m_ScrollbarHeight), SpriteEffects.None, Depth);

                foreach (RenderableText Txt in m_Lines)
                {
                    if (Txt.Visible)
                    {
                        SBatch.DrawString(m_Font, Txt.SBuilder.ToString(), new Vector2(Txt.Position.X, Txt.Position.Y),
                            TextColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, Depth + 0.1f);
                        DrawingHeight += m_Font.LineSpacing;
                    }
                }

                if (m_HasFocus)
                {
                    if (m_Cursor.Visible == true)
                    {
                        SBatch.DrawString(m_Font, " " + m_Cursor.Cursor, new Vector2(m_Cursor.Position.X,
                            m_Cursor.Position.Y), TextColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f,
                            SpriteEffects.None, Depth + 0.1f);
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if shift is currently being pressed.
        /// </summary>
        /// <param name="PressedKeys">A list of keys currently being pressed.</param>
        /// <returns>True if it is being pressed, false otherwise.</returns>
        private bool IsUpperCase(InputHelper Helper)
        {
            if (Helper.IsCurPress(Keys.LeftShift) || Helper.IsCurPress(Keys.RightShift))
                return true;

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool CleanUpManagedResources)
        {
            if(CleanUpManagedResources)
            {
                m_CursorVisibilityTimer.Dispose();
            }
        }
    }
}
