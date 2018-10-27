using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using log4net;
using Files.Manager;
using Files;

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

    internal class Cursor
    {
        public int CharacterIndex = 0;
        public string Symbol = "|";
        public Vector2 Position = new Vector2(0, 0);
        public int LineIndex = 0;
        public bool Visible = true;
    }

    public class UITextEdit3 : UIElement, IDisposable
    {
        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool Transparent = false;
        private int m_NumLines = 0; //Maximum number of lines that this control can hold.
        private int m_MaxChars = 0;
        private TextEditAlignment m_Alignment = 0;
        private bool m_FlashOnEmpty = false;
        private Color m_BackColor, m_CursorColor, m_FrameColor;
        private bool m_ResizeForExactLineHeight = false;
        private bool m_EnableInputModeEditing = false;

        private GapBuffer<string> m_Text = new GapBuffer<string>();

        /// <summary>
        /// Defines whether this text edit control will receive a border on focus. 1=Yes, 0=No.Default is yes.
        /// </summary>
        private bool m_FrameOnFocus = true;

        private Dictionary<int, Vector2> m_CharacterPositions = new Dictionary<int, Vector2>();
        private List<Rectangle> m_HitBoxes = new List<Rectangle>();
        private bool m_UpdateCharPositions = true;

        private const int NUM_CHARS_IN_LINE = 30;
        private Timer m_CursorVisibilityTimer = new Timer();
        private Cursor m_Cursor = new Cursor();
        private int m_NumLinesInText = 1;

        private Texture2D m_ScrollbarImage;
        private int m_ScrollbarWidth = 0, m_ScrollbarHeight = 0;
        private ScrollbarType m_ScrollbarType;

        private TextEditMode m_Mode;

        private bool m_MultiLine = true;
        private bool m_CapitalLetters = false;

        //For how long has a key been presseed?
        private DateTime m_DownSince = DateTime.Now;
        private float m_TimeUntilRepInMillis = 100f;
        private int m_RepsPerSec = 15;
        private DateTime m_LastRep = DateTime.Now;
        private Keys? m_RepChar; //A character currently being pressed (repeated).

        private bool m_DrawBackground = false;

        private Vector2 m_TextPosition = new Vector2(0, 0); //Coordinate for anchoring the text.

        /// <summary>
        /// Constructs a new UITextEdit element.
        /// </summary>
        /// <param name="Name">The name of this UITextElement instance.</param>
        /// <param name="ID">The ID of this UITextEdlement instance.</param>
        /// <param name="DrawBackground">Should this UITextElement instance draw a background?</param>
        /// <param name="TextEditPosition">The position of this UITextEdit instance.</param>
        /// <param name="Font">The size of the font to use, from 9-16.</param>
        /// <param name="Size">The size of this UITextEdit instance.</param>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Tooltip">The tooltip associated with this UITextEdit control (optional).</param>
        public UITextEdit3(string Name, int ID, bool DrawBackground, Vector2 TextEditPosition, Vector2 Size, 
            int Font, UIScreen Screen, string Tooltip = "") : base(Name, TextEditPosition, Size, Screen, null)
        {
            this.Name = Name;
            m_ID = ID;
            m_KeyboardInput = true; //UITextEdit needs to receive input from keyboard!
            m_DrawBackground = DrawBackground;
            m_NeedsClipping = true; //This control needs to clip all the text rendered outside of it!

            Position = TextEditPosition;
            m_TextPosition = Position;
            m_Size = Size * Resolution.getVirtualAspectRatio();
            TextColor = m_Screen.StandardTxtColor;

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
            }

            m_Cursor.Position = Position;
            m_CursorVisibilityTimer = new Timer(100);
            m_CursorVisibilityTimer.Enabled = true;
            m_CursorVisibilityTimer.Elapsed += CursorVisibilityTimer_Elapsed;
            m_CursorVisibilityTimer.Start();

            m_Screen.Manager.OnTextInput += Window_TextInput;

            m_TextPosition = Position;

            if (DrawBackground)
            {
                Image = new UIImage(FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_textboxbackground), m_Screen);

                if (Position != null)
                    Image.Position = Position;

                Image.Slicer = new NineSlicer(new Vector2(0, 0), (int)Image.Texture.Width, (int)Image.Texture.Width, 15, 15, 15, 15);
                Image.SetSize((int)Size.X, (int)Size.Y);
            }
        }

        /// <summary>
        /// Constructs a new UITextEdit control from a parsed UIScript.
        /// </summary>
        /// <param name="Node">The AddTextEditNode that defines this UITextEdit control.</param>
        /// <param name="State">The ParserState returned when parsing the UIScript.</param>
        /// <param name="Screen">A UIScreen instance.</param>
        public UITextEdit3(AddTextEditNode Node, ParserState State, UIScreen Screen) :
            base(Screen)
        {
            Name = Node.Name;
            m_ID = Node.ID;
            m_KeyboardInput = true; //UITextEdit needs to receive input from keyboard!
            m_NeedsClipping = true; //This control needs to clip all the text rendered outside of it!

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
                {
                    //In the original game, this was most likely used to limit the size of the bio for the sake
                    //of the network protocol. This is no longer important.
                    m_NumLines = (int)Node.Lines;

                    if (m_NumLines > 1)
                        m_MultiLine = true;
                    else
                        m_MultiLine = false;
                }

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
                    m_BackColor = new Color(Node.Color.Numbers[0], Node.Color.Numbers[1], Node.Color.Numbers[2]);
                else
                    m_BackColor = new Color(57, 81, 110, 255);

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
                }
                if (State.Tooltip != "")
                    Tooltip = State.Tooltip;
            }

            int Font = 0;
            if (Node.Font != 0)
                Font = Node.Font;
            else
                Font = State.Font;

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
            }

            m_Cursor.Position = Position;
            m_CursorVisibilityTimer = new Timer(100);
            m_CursorVisibilityTimer.Enabled = true;
            m_CursorVisibilityTimer.Elapsed += CursorVisibilityTimer_Elapsed;
            m_CursorVisibilityTimer.Start();

            m_Screen.Manager.OnTextInput += Window_TextInput;
        }

        private void CursorVisibilityTimer_Elapsed(object sender, ElapsedEventArgs e)
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
        /// The text in this TextEditor instance, containing "\n".
        /// </summary>
        private string TextWithLBreaks
        {
            get
            {
                string Text = "";

                foreach (string Str in m_Text)
                    Text += Str;

                return Text;
            }
        }

        /// <summary>
        /// Returns a line of text.
        /// </summary>
        /// <param name="LineIndex">The index of the line of text to get.</param>
        /// <returns>The line of text, as indicated by the line index.</returns>
        private string GetLine(int LineIndex)
        {
            try
            {
                if (TextWithLBreaks.Contains("\n"))
                    return TextWithLBreaks.Split("\n".ToCharArray())[LineIndex];
            }
            catch (Exception)
            {
                return "";
            }

            return TextWithLBreaks;
        }

        /// <summary>
        /// Make sure cursor's index is valid and in range.
        /// </summary>
        private void FixCursorIndex()
        {
            if (m_Cursor.CharacterIndex < 0)
                m_Cursor.CharacterIndex = 0;
            if (m_Cursor.CharacterIndex == m_Text.Count)
                m_Cursor.CharacterIndex = m_Text.Count - 1;
        }

        /// <summary>
        /// Can the text scroll up?
        /// </summary>
        /// <returns>Returns true if it can, false otherwise.</returns>
        public bool CanScrollUp()
        {
            if ((m_TextPosition.Y + TextSize().Y) > (Position.Y + Size.Y))
                return true;

            return false;
        }

        /// <summary>
        /// Can the text scroll down?
        /// </summary>
        /// <returns>Returns true if it can, false otherwise.</returns>
        public bool CanScrollDown()
        {
            if (m_TextPosition.Y < Position.Y)
                return true;

            return false;
        }

        /// <summary>
        /// Scrolls the text down.
        /// </summary>
        /// <returns>True if the text could be scrolled down, false otherwise.</returns>
        public bool ScrollDown()
        {
            if (m_TextPosition.Y < Position.Y)
            {
                m_TextPosition.Y += m_Font.LineSpacing;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Scrolls the text up.
        /// </summary>
        /// <returns>True if the text could be scrolled down, false otherwise.</returns>
        public bool ScrollUp()
        {
            if ((m_TextPosition.Y + TextSize().Y) > (Position.Y + Size.Y))
            {
                m_TextPosition.Y -= m_Font.LineSpacing;
                return true;
            }

            return false;
        }

        private void ScrollLeft()
        {
            if(m_TextPosition.X < Position.X)
            {
                float Diff = (Position.X - m_TextPosition.X);
                if ((TextSize().X - Diff) >= (Position.X + Size.X))
                    m_TextPosition.X -= m_Font.LineSpacing;
            }
            else
            {
                if (TextSize().X > (Position.X + Size.X))
                    m_TextPosition.X -= m_Font.LineSpacing;
            }
        }

        private void ScrollRight()
        {
            if (m_TextPosition.X < Position.X)
            {
                float Diff = (m_TextPosition.X - Size.X);
                if ((TextSize().X - Diff) >= (Position.X + Size.X))
                    m_TextPosition.X += m_Font.LineSpacing;
            }
            else
            {
                if (TextSize().X > (Position.X + Size.X))
                {
                    if(m_TextPosition.X < Position.X)
                        m_TextPosition.X += m_Font.LineSpacing;
                }
            }
        }

        /// <summary>
        /// Make sure cursor's position is valid and in range.
        /// </summary>
        private void FixCursorPosition()
        {
            if (m_Cursor.Position.X < Position.X)
                m_Cursor.Position.X = Position.X;

            m_UpdateCharPositions = true;
            UpdateCharacterPositions();
            UpdateHitboxes();

            //Find the curor's real character index.
            int RealCharIndex = m_Cursor.CharacterIndex;

            RealCharIndex = (RealCharIndex < m_CharacterPositions.Count) ?  //Make sure it doesn't overflow.
                RealCharIndex : m_CharacterPositions.Count - 1;

            if (RealCharIndex < 0)
                RealCharIndex = 0; //Make sure it doesn't underflow.

            //Adjust the character's position based on the real character index.
            if (m_Text.Count > 0)
            {
                Vector2 IndexPosition = m_CharacterPositions[(RealCharIndex > 0) ? RealCharIndex : 0];

                if (m_Cursor.Position.X < IndexPosition.X)
                    m_Cursor.Position.X = IndexPosition.X;
                if (m_Cursor.Position.Y != IndexPosition.Y)
                    m_Cursor.Position.Y = IndexPosition.Y;
            }
        }

        /// <summary>
        /// The text in this TextEditor instance, without \n 
        /// (except for those explicitly added by pressing backspace).
        /// </summary>
        public string Text
        {
            get
            {
                string Text = "";

                foreach (string Str in m_Text)
                    Text += Str;

                return Text.Replace("\n", "");
            }
        }

        /// <summary>
        /// Returns the width of a character in this font.
        /// </summary>
        /// <returns>The width of the character in floating point numbers.</returns>
        private float CharacterWidth
        {
            get { return m_Font.MeasureString("a").X; }
        }

        /// <summary>
        /// Returns the width of a capitalized character in this font.
        /// </summary>
        /// <returns>The width of the capitalized character in floating point numbers.</returns>
        private float CapitalCharacterWidth
        {
            get { return m_Font.MeasureString("A").X; }
        }

        /// <summary>
        /// Returns the height of a character in this font.
        /// </summary>
        /// <returns>The height of the character in floating point numbers.</returns>
        private float CharacterHeight
        {
            get { return m_Font.MeasureString("a").Y; }
        }

        /// <summary>
        /// Returns the height of a capitalized character in this font.
        /// </summary>
        /// <returns>The height of the capitalized character in floating point numbers.</returns>
        private float CapitalCharacterHeight
        {
            get { return m_Font.MeasureString("A").Y; }
        }

        /// <summary>
        /// Returns the last line of text in the gap buffer.
        /// </summary>
        /// <returns></returns>
        private string CurrentLine
        {
            get
            {
                if (m_Text.Count > 1)
                {
                    if (TextWithLBreaks.Contains("\n"))
                    {
                        string[] Lines = TextWithLBreaks.Split("\n".ToCharArray());

                        return Lines[Lines.Length - 1];
                    }
                    else
                        return TextWithLBreaks;
                }

                if (m_Text.Count > 0)
                    return m_Text[0];
                else
                    return "";
            }
        }

        /// <summary>
        /// The control received text input.
        /// </summary>
        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (m_HasFocus)
            {
                if (e.Character != (char)Keys.Back)
                {
                    int Index = TextWithLBreaks.LastIndexOf("\n", m_Cursor.CharacterIndex);
                    if (Index == -1) //No occurence was found!!
                    {
                        if (m_MultiLine)
                        {
                            if (Text.Length <= NUM_CHARS_IN_LINE)
                            {
                                AddText((m_CapitalLetters == true) ? e.Character.ToString().ToUpper() :
                                    e.Character.ToString());
                                m_CapitalLetters = false;
                                m_UpdateCharPositions = true;
                                return;
                            }
                            else
                            {
                                AddNewline();
                                return;
                            }
                        }
                        else
                        {
                            AddText((m_CapitalLetters == true) ? e.Character.ToString().ToUpper() : 
                                e.Character.ToString());
                            m_CapitalLetters = false;
                            m_UpdateCharPositions = true;
                            return;
                        }
                    }

                    if ((m_Cursor.CharacterIndex - Index) <= NUM_CHARS_IN_LINE)
                    {
                        //If the cursor has moved away from the end of the text...
                        if (m_Cursor.CharacterIndex < (m_Text.Count - (1 + m_NumLinesInText)))
                        {
                            //... insert it at the cursor's position.
                            m_Text.Insert(m_Cursor.CharacterIndex, (m_CapitalLetters == true) ? e.Character.ToString().ToUpper() :
                                e.Character.ToString());
                            m_CapitalLetters = false;
                            m_UpdateCharPositions = true;
                        }
                        else
                        {
                            AddText((m_CapitalLetters == true) ? e.Character.ToString().ToUpper() :
                                e.Character.ToString()); //... just add the text as usual.
                            m_CapitalLetters = false;
                            m_UpdateCharPositions = true;
                        }
                    }
                    else
                    {
                        if (m_MultiLine)
                            AddNewline();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a string to m_Text, and updates the cursor.
        /// </summary>
        /// <param name="Text">The string to add.</param>
        private void AddText(string Text)
        {
            m_Text.Add(Text);
            m_Cursor.CharacterIndex++;

            if((Position.X + m_Cursor.Position.X) < (Position.X + Size.X))
                m_Cursor.Position.X += CharacterWidth;

            if (!m_MultiLine)
                ScrollLeft();
        }

        //Can the cursor move further down or has it reached the end of the textbox?
        private bool m_CanMoveCursorDown = true;

        /// <summary>
        /// Adds a newline to m_Text, and updates the cursor.
        /// </summary>
        private void AddNewline()
        {
            m_Text.Add("\n");
            m_Cursor.CharacterIndex++;
            m_Cursor.Position.X = Position.X;

            m_Cursor.LineIndex++;

            //Scroll the text up if it's gone beyond the borders of the control.
            if ((Position.Y - TextSize().Y) < (Position.Y - Size.Y))
            {
                m_TextPosition.Y -= CapitalCharacterHeight;
                m_CanMoveCursorDown = false;
                m_UpdateCharPositions = true;
            }

            if (m_CanMoveCursorDown)
                m_Cursor.Position.Y += CapitalCharacterHeight;

            m_NumLinesInText++;
        }

        /// <summary>
        /// Removes text from m_Text.
        /// </summary>
        private void RemoveText()
        {
            FixCursorIndex();
            FixCursorPosition();

            if (m_Cursor.Position.X > Position.X)
            {
                m_Text.RemoveAt(m_Cursor.CharacterIndex);
                m_Cursor.CharacterIndex--;
                m_Cursor.Position.X -= CharacterWidth;
            }

            if (m_Cursor.Position.X <= Position.X)
            {
                if (m_Cursor.LineIndex != 0)
                {
                    m_Cursor.Position.X = Position.X +
                        m_Font.MeasureString(GetLine(m_Cursor.LineIndex - 1)).X;

                    if (m_MultiLine)
                    {
                        m_Cursor.Position.Y -= CapitalCharacterHeight;
                        m_Cursor.LineIndex--;

                        m_NumLinesInText--;

                        if (m_TextPosition.Y < Position.Y)
                            m_TextPosition.Y += m_Font.LineSpacing;
                    }
                }
            }
        }

        /// <summary>
        /// Moves m_Cursor left.
        /// </summary>
        private void MoveCursorLeft()
        {
            if (m_MultiLine)
            {
                if (m_Cursor.Position.X > Position.X)
                {
                    m_Cursor.CharacterIndex -= (((NUM_CHARS_IN_LINE + 1) -
                        GetLine(m_Cursor.LineIndex).Length) + GetLine(m_Cursor.LineIndex).Length);

                    m_Cursor.Position.X -= CharacterWidth;
                }

                //Scroll the text right if the cursor is at the beginning of the control.
                if (m_Cursor.Position.X == Position.X)
                {
                    if (m_TextPosition.X > Position.X)
                        m_TextPosition.X -= m_Font.LineSpacing;
                }
            }
            else
            {
                if (m_Cursor.Position.X > Position.X)
                {
                    m_Cursor.CharacterIndex -= 1;

                    m_Cursor.Position.X -= CharacterWidth;
                }

                if (m_Cursor.Position.X == Position.X)
                    ScrollRight();
            }
        }


        /// <summary>
        /// Moves m_Cursor right.
        /// </summary>
        private void MoveCursorRight()
        {
            if (m_MultiLine)
            {
                if (m_Cursor.Position.X < (Position.X + Size.X))
                {
                    m_Cursor.CharacterIndex += (((NUM_CHARS_IN_LINE + 1) -
                        GetLine(m_Cursor.LineIndex).Length) + GetLine(m_Cursor.LineIndex).Length);

                    m_Cursor.Position.X += CharacterWidth;
                }

                //Scroll the text right if the cursor is at the beginning of the control.
                if (m_Cursor.Position.X == Position.X)
                {
                    if (m_TextPosition.X < Position.X)
                        m_TextPosition.X += m_Font.LineSpacing;
                }
            }
            else
            {
                if (m_Cursor.Position.X < (Position.X + Size.X))
                {
                    m_Cursor.CharacterIndex += 1;

                    m_Cursor.Position.X += CharacterWidth;
                }

                if (m_Cursor.Position.X >= (Position.X + Size.X))
                    ScrollLeft();
            }
        }

        /// <summary>
        /// Moves m_Cursor up.
        /// </summary>
        private void MoveCursorUp()
        {
            if (m_Cursor.Position.Y > Position.Y)
            {
                m_Cursor.LineIndex--;
                m_Cursor.CharacterIndex -= (((NUM_CHARS_IN_LINE + 1) -
                    GetLine(m_Cursor.LineIndex).Length) + GetLine(m_Cursor.LineIndex).Length);

                m_Cursor.Position.Y -= CapitalCharacterHeight;

                m_CanMoveCursorDown = true;
            }

            //Scroll the text down if the cursor is at the top of the control.
            if (m_Cursor.Position.Y == Position.Y)
            {
                if (m_TextPosition.Y < Position.Y)
                    m_TextPosition.Y += m_Font.LineSpacing;
            }
        }

        /// <summary>
        /// Moves m_Cursor down.
        /// </summary>
        private void MoveCursorDown()
        {
            if (m_Cursor.Position.Y < (Position.Y + Size.Y))
            {
                m_Cursor.LineIndex++;
                m_Cursor.CharacterIndex += (((NUM_CHARS_IN_LINE + 1) -
                    GetLine(m_Cursor.LineIndex).Length) + GetLine(m_Cursor.LineIndex).Length);

                m_Cursor.Position.Y += CapitalCharacterHeight;
            }
            else //Scroll the text up if the cursor is at the bottom of the control.
            {
                if ((m_TextPosition.Y + TextSize().Y) > (Position.Y + Size.Y))
                    m_TextPosition.Y -= m_Font.LineSpacing;
            }
        }

        /// <summary>
        /// Calculates the size of all the text in the textbox.
        /// </summary>
        /// <returns>A Vector2 containing the width and height of the text.</returns>
        private Vector2 TextSize()
        {
            float Width = 0.0f, Height = 0.0f;

            if (m_MultiLine)
            {
                foreach (string Str in TextWithLBreaks.Split("\n".ToCharArray()))
                {
                    Vector2 Size = m_Font.MeasureString(Str);
                    Width = Size.X;
                    Height += Size.Y;
                }
            }
            else
                return m_Font.MeasureString(Text);

            return new Vector2(Width, Height);
        }

        /// <summary>
        /// Update the hitboxes for the characters in the textbox.
        /// The hitboxes are used to detect collision(s) with the mouse cursor.
        /// </summary>
        private void UpdateHitboxes()
        {
            if (m_UpdateCharPositions)
            {
                int Height = 0;

                m_HitBoxes.Clear();

                //Make sure it doesn't go out of bounds...
                if (m_Text.Count >= 1)
                {
                    for (int i = 0; i < m_CharacterPositions.Count; i++)
                    {
                        //Make sure it doesn't go out of bounds...
                        Height = (int)(m_Font.MeasureString(m_Text[i < m_Text.Count ? i : m_Text.Count - 1]).Y);

                        //Create a hitbox for each character if the character isn't the last one.
                        if (i != m_CharacterPositions.Count - 1)
                        {
                            Rectangle Hitbox = new Rectangle((int)m_CharacterPositions[i].X, (int)m_CharacterPositions[i].Y,
                                (int)(m_CharacterPositions[i + 1].X - m_CharacterPositions[i].X), Height);
                            m_HitBoxes.Add(Hitbox);
                        }
                    }
                }

                m_UpdateCharPositions = false;
            }
        }

        /// <summary>
        /// Updates the positions of the characters.
        /// Called when a character is added or deleted from the textbox.
        /// </summary>
        private void UpdateCharacterPositions()
        {
            Vector2 Position = this.Position;
            float XPosition = 0;
            if (m_UpdateCharPositions)
            {
                m_CharacterPositions.Clear();

                int CharIndex = 0;

                foreach (string Str in TextWithLBreaks.Split("\n".ToCharArray()))
                {
                    XPosition = 0;

                    for (int i = 0; i < Str.Length; i++)
                    {
                        float CharWidth = m_Font.MeasureString(Str.Substring(i, 1)).X * Resolution.getVirtualAspectRatio();
                        XPosition += CharWidth;

                        m_CharacterPositions.Add(CharIndex, new Vector2(XPosition + this.Position.X, Position.Y + 
                            (m_TextPosition.Y - this.Position.Y))); //Remove the component's position so the text starts out at 0.
                        CharIndex++;
                    }

                    Position.Y += CapitalCharacterHeight * Resolution.getVirtualAspectRatio();
                }

                ///This shouldn't be set here, because it is set in UpdateHitboxes();
                //m_UpdateCharPositions = false;
            }
        }

        public override void Update(InputHelper Input, GameTime GTime)
        {
            UpdateCharacterPositions();
            UpdateHitboxes();

            if (m_HasFocus)
            {
                foreach (Rectangle Hitbox in m_HitBoxes)
                {
                    if (Hitbox.Contains(new Vector2(Input.MousePosition.X, Input.MousePosition.Y)) &&
                        (Input.IsNewPress(MouseButtons.LeftButton) || Input.IsOldPress(MouseButtons.LeftButton)))
                    {
                        m_Cursor.Position = new Vector2(Hitbox.X, Hitbox.Y);

                        int CharIndex = m_CharacterPositions.FirstOrDefault(x => x.Value == m_Cursor.Position).Key;
                        if (CharIndex != -1)
                            m_Cursor.CharacterIndex = CharIndex;
                    }
                }

                foreach (Keys Key in (Keys[])Enum.GetValues(typeof(Keys)))
                {
                    if (Input.IsNewPress(Key))
                    {
                        m_DownSince = DateTime.Now;
                        m_RepChar = Key;
                    }
                    else if (Input.IsOldPress(Key))
                    {
                        if (m_RepChar == Key)
                            m_RepChar = null;
                    }

                    if (m_RepChar != null && m_RepChar == Key && Input.CurrentKeyboardState.IsKeyDown(Key))
                    {
                        DateTime Now = DateTime.Now;
                        TimeSpan DownFor = Now.Subtract(m_DownSince);
                        if (DownFor.CompareTo(TimeSpan.FromMilliseconds(m_TimeUntilRepInMillis)) > 0)
                        {
                            // Should repeat since the wait time is over now.
                            TimeSpan repeatSince = Now.Subtract(m_LastRep);
                            if (repeatSince.CompareTo(TimeSpan.FromMilliseconds(1000f / m_RepsPerSec)) > 0)
                                // Time for another key-stroke.
                                m_LastRep = Now;
                        }
                    }
                }

                Keys[] PressedKeys = Input.CurrentKeyboardState.GetPressedKeys();

                //Are these keys being held down since the last update?
                if (m_RepChar == Keys.Back && m_LastRep == DateTime.Now)
                    RemoveText();
                if (m_RepChar == Keys.Up && m_LastRep == DateTime.Now)
                {
                    if (m_MultiLine)
                        MoveCursorUp();
                }
                if (m_RepChar == Keys.Down && m_LastRep == DateTime.Now)
                {
                    if (m_MultiLine)
                        MoveCursorDown();
                }

                foreach (Keys K in PressedKeys)
                {
                    if (Input.IsNewPress(K))
                    {
                        switch (K)
                        {
                            case Keys.Up:
                                if (m_RepChar != Keys.Up || m_LastRep != DateTime.Now)
                                {
                                    if (m_MultiLine)
                                        MoveCursorUp();
                                }
                                break;
                            case Keys.Down:
                                if (m_RepChar != Keys.Down || m_LastRep != DateTime.Now)
                                {
                                    if (m_MultiLine)
                                        MoveCursorDown();
                                }
                                break;
                            case Keys.Left:
                                MoveCursorLeft();
                                break;
                            case Keys.Right:
                                MoveCursorRight();
                                break;
                            case Keys.Back:
                                if (m_RepChar != Keys.Back || m_LastRep != DateTime.Now)
                                    RemoveText();
                                break;
                            case Keys.LeftShift:
                                m_CapitalLetters = true;
                                break;
                            case Keys.RightShift:
                                m_CapitalLetters = true;
                                break;
                            case Keys.Enter:
                                AddNewline();
                                break;
                        }
                    }
                }
            }

            base.Update(Input, GTime);
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            base.Draw(SBatch, LayerDepth);

            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.5f;

            if (m_DrawBackground)
            {
                Image.DrawTextureTo(SBatch, null, Image.Slicer.TLeft, Image.Position + Vector2.Zero, Depth);
                Image.DrawTextureTo(SBatch, Image.Slicer.TCenter_Scale, Image.Slicer.TCenter, Image.Position + new Vector2(Image.Slicer.LeftPadding, 0), Depth);
                Image.DrawTextureTo(SBatch, null, Image.Slicer.TRight, Image.Position + new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, 0), Depth);

                Image.DrawTextureTo(SBatch, Image.Slicer.CLeft_Scale, Image.Slicer.CLeft, Image.Position + new Vector2(0, Image.Slicer.TopPadding), null);
                Image.DrawTextureTo(SBatch, Image.Slicer.CCenter_Scale, Image.Slicer.CCenter, Image.Position + new Vector2(Image.Slicer.LeftPadding, Image.Slicer.TopPadding), Depth);
                Image.DrawTextureTo(SBatch, Image.Slicer.CRight_Scale, Image.Slicer.CRight, Image.Position + new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, Image.Slicer.TopPadding), Depth);

                int BottomY = Image.Slicer.Height - Image.Slicer.BottomPadding;
                Image.DrawTextureTo(SBatch, null, Image.Slicer.BLeft, Image.Position + new Vector2(0, BottomY), null);
                Image.DrawTextureTo(SBatch, Image.Slicer.BCenter_Scale, Image.Slicer.BCenter, Image.Position + new Vector2(Image.Slicer.LeftPadding, BottomY), Depth);
                Image.DrawTextureTo(SBatch, null, Image.Slicer.BRight, Image.Position + new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, BottomY), Depth);
            }

            if (m_ScrollbarImage != null)
                SBatch.Draw(m_ScrollbarImage, new Vector2(m_Size.X - m_ScrollbarWidth, 0), null, Color.White, 0.0f,
                    new Vector2(0.0f, 0.0f), new Vector2(0.0f, m_ScrollbarHeight), SpriteEffects.None, Depth);

            //Cut off the width of the scissor to make it look better.
            SBatch.GraphicsDevice.ScissorRectangle = new Rectangle((int)this.Position.X,
                (int)this.Position.Y, (int)(Size.X - m_Font.LineSpacing), (int)Size.Y);

            Vector2 Position = m_TextPosition;

            if (m_HasFocus)
            {
                if (m_Cursor.Visible)
                    SBatch.DrawString(m_Font, m_Cursor.Symbol, m_Cursor.Position, Color.White);
            }

            if (m_MultiLine)
            {
                foreach (string Str in TextWithLBreaks.Split("\n".ToCharArray()))
                {
                    SBatch.DrawString(m_Font, Str, Position, TextColor);
                    Position.Y += CapitalCharacterHeight;
                }
            }
            else
                SBatch.DrawString(m_Font, Text, Position, TextColor);
        }

        ~UITextEdit3()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this UITextEdit instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this UITextEdit instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                m_CursorVisibilityTimer.Dispose();

                // Prevent the finalizer from calling ~UITextEdit, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("UITextEdit not explicitly disposed!");
        }
    }
}
