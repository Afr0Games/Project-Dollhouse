using System;
using System.Timers;
using System.Reflection;
using UIParser;
using UIParser.Nodes;
using Files;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using log4net;

namespace Gonzo.Elements
{
    public class UITextEdit2 : UIElement, IDisposable
    {
        private GapBuffer<RenderableText2> m_Lines = new GapBuffer<RenderableText2>();
        private GapBuffer<string> m_CurrentLine = new GapBuffer<string>();

        private TextRenderer m_Renderer;

        /// <summary>
        /// The contents of the GapBuffer containing the current line.
        /// </summary>
        /// <returns>The contents of the GapBuffer containing the current line.</returns>
        private string GetCurrentLine()
        {
            string Output = "";

            foreach (string Char in m_CurrentLine)
                Output += Char;

            return Output;
        }

        public bool Transparent = false;
        private int m_NumLines = 0; //Maximum number of lines that this control can hold.
        private int m_MaxChars = 0;
        private TextEditAlignment m_Alignment = 0;
        private bool m_FlashOnEmpty = false;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        private bool m_DrawBackground = false;

        //For how long has a key been presseed?
        private DateTime m_DownSince = DateTime.Now;
        private float m_TimeUntilRepInMillis = 100f;
        private int m_RepsPerSec = 15;
        private DateTime m_LastRep = DateTime.Now;
        private Keys? m_RepChar; //A character currently being pressed (repeated).

        /// <summary>
        /// Gets or sets the font used by this UITextEdit.
        /// </summary>
        public SpriteFont Font
        {
            get { return m_Font; }
            set { m_Font = value; }
        }

        /// <summary>
        /// Current height of a line in this UITextEdit.
        /// </summary>
        public float Lineheight
        {
            get { return m_Font.MeasureString("a").Y; }
        }

        /// <summary>
        /// Current number of lines for this control.
        /// </summary>
        public int CurrentNumberOfLines
        {
            get
            {
                if (m_NumLines > 1)
                    return m_Lines.Count;
                else
                    return 1;
            }
        }

        /// <summary>
        /// Can the text in this UITextEdit instance scroll up?
        /// </summary>
        /// <returns>True if it can scroll up, false otherwise.</returns>
        public bool CanScrollUp()
        {
            return m_Renderer.CanScrollUp();
        }

        /// <summary>
        /// Can the text in this UITextEdit instance scroll down?
        /// </summary>
        /// <returns>True if it can scroll down, false otherwise.</returns>
        public bool CanScrollDown()
        {
            return m_Renderer.CanScrollDown();
        }

        /// <summary>
        /// Constructs a new UITextEdit control manually (I.E not from a UIScript).
        /// </summary>
        /// <param name="Name">The name of this UITextEdit control.</param>
        /// <param name="ID">The ID of this UITextEdit control.</param>
        /// <param name="DrawBackground">Should this UITextEdit's background be drawn?</param>
        /// <param name="NumLines">Number of lines that this UITextEdit control supports.</param>
        /// <param name="TextEditPosition">The position of this UITextEdit control.</param>
        /// <param name="TextEditSize">The size of this UITextEdit control.</param>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Tooltip">The tooltip associated with this UITextEdit control (optional).</param>
        public UITextEdit2(string Name, int ID, bool DrawBackground, int NumLines, 
            Vector2 TextEditPosition, Vector2 TextEditSize, int Font, UIScreen Screen, 
            string Tooltip = "") : base(Screen)
        {
            this.Name = Name;
            m_ID = ID;
            m_KeyboardInput = true; //UITextEdit needs to receive input from keyboard!
            m_DrawBackground = DrawBackground;
            m_NeedsClipping = true; //This control needs to clip all the text rendered outside of it!

            Position = TextEditPosition;
            m_TextPosition = Position;
            m_Size = TextEditSize;
            m_NumLines = NumLines;
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

            m_Renderer = new TextRenderer((m_NumLines > 1) ? true : false,
                Position, Size, m_ScrollFactor, Lineheight, m_Font, TextColor, m_NumLines);

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
        public UITextEdit2(AddTextEditNode Node, ParserState State, UIScreen Screen) :
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

            m_Renderer = new TextRenderer((m_NumLines > 1) ? true : false, 
                Position, Size, m_ScrollFactor, Lineheight, m_Font, TextColor, m_NumLines);

            m_Cursor.Position = Position;
            m_CursorVisibilityTimer = new Timer(100);
            m_CursorVisibilityTimer.Enabled = true;
            m_CursorVisibilityTimer.Elapsed += M_CursorVisibilityTimer_Elapsed;
            m_CursorVisibilityTimer.Start();

            m_Screen.Manager.OnTextInput += Manager_OnTextInput;
        }

        private void Manager_OnTextInput(object sender, TextInputEventArgs e)
        {
            bool NewLine = false; //Was a new line created?

            if (m_HasFocus)
            {
                if (m_Mode != TextEditMode.ReadOnly)
                {
                    if (m_NumLines > 1)
                    {
                        Vector2 MeasuredString = m_Font.MeasureString(GetCurrentLine());
                        //Check that text doesn't go beyond width of control...
                        if (((Position.X + (MeasuredString.X + m_Renderer.FontSpacing)) >=
                            (m_Size.X - Position.X)) && !m_RemovingTxt && !m_MovingCursor)
                        {
                            if (m_TextPosition.Y <= Position.Y + ((m_NumLines - 2) * m_Font.LineSpacing))
                            {
                                m_TextPosition.Y += m_Font.LineSpacing;

                                AddCurrentLine();

                                m_Cursor.Position.Y += m_Font.LineSpacing;
                                m_Cursor.LineIndex++;
                                m_Cursor.CharacterIndex = 0;
                                m_Cursor.Position.X = Position.X;
                                NewLine = true;
                            }
                            else //Text went beyond the borders of the control...
                            {
                                AddCurrentLine();

                                m_ScrollbarHeight -= m_Font.LineSpacing; //TODO: Resize scrollbar...

                                m_Cursor.LineIndex++;
                                m_Cursor.CharacterIndex = 0;
                                m_Cursor.Position.X = Position.X;
                                NewLine = true;
                            }
                        }
                    }
                    else
                    {
                        //Text went beyond the borders of the control...
                        if (m_Font.MeasureString(Text).X >= (m_Size.X -
                            m_Font.MeasureString(e.Character.ToString()).X) && !m_RemovingTxt)
                        {
                            m_Cursor.Position.X = m_Size.X;

                            m_Renderer.ScrollTextLeft();
                        }
                    }

                    if (!m_IsUpperCase)
                    {
                        //If the cursor is in the middle of a line, replace the character.
                        if (m_Cursor.CharacterIndex < GetCurrentLine().Length)
                        {
                            m_CurrentLine.RemoveAt(m_Cursor.CharacterIndex);
                            m_Renderer.RemoveAt(m_Cursor.CharacterIndex);

                            m_CurrentLine.Insert(m_Cursor.CharacterIndex, e.Character.ToString());
                            m_Renderer.Insert(m_Cursor.CharacterIndex, e.Character.ToString());
                        }
                        else
                        {
                            m_CurrentLine.Add(e.Character.ToString());
                            m_Renderer.Insert(m_Cursor.CharacterIndex, e.Character.ToString());
                        }
                    }
                    else
                    {
                        //If the cursor is in the middle of a line, replace the character.
                        if (m_Cursor.CharacterIndex < GetCurrentLine().Length)
                        {
                            m_CurrentLine.RemoveAt(m_Cursor.CharacterIndex);
                            m_Renderer.RemoveAt(m_Cursor.CharacterIndex);

                            m_CurrentLine.Insert(m_Cursor.CharacterIndex, e.Character.ToString().ToUpper());
                            m_Renderer.Insert(m_Cursor.CharacterIndex, e.Character.ToString().ToUpper());
                        }
                        else
                        {
                            m_CurrentLine.Add(e.Character.ToString().ToUpper());
                            m_Renderer.Insert(m_Cursor.CharacterIndex, e.Character.ToString());
                        }
                    }
                }

                if (!NewLine)
                    m_Cursor.CharacterIndex++;

                m_RemovingTxt = false;
                m_MovingCursor = false;
                m_Cursor.Position.X += m_Renderer.FontSpacing;
            }
        }

        /// <summary>
        /// Makes this UITextEdit's renderer scroll the text up.
        /// </summary>
        public bool ScrollUp()
        {
            return m_Renderer.ScrollUp();
        }

        /// <summary>
        /// Makes this UITextEdit's renderer scroll the text down.
        /// </summary>
        public bool ScrollDown()
        {
            return m_Renderer.ScrollDown();
        }

        /// <summary>
        /// Gets the current text in this UITextEdit instance.
        /// </summary>
        public string Text
        {
            get
            {
                if (m_NumLines > 1)
                {
                    string InputStr = "";
                    foreach (RenderableText2 Txt in m_Lines)
                        InputStr += Txt.Text;

                    return InputStr;
                }
                else
                {
                    string InputStr = "";
                    foreach (string Txt in m_CurrentLine)
                        InputStr += Txt;

                    return InputStr;
                }
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

        public override void Update(InputHelper Input, GameTime GTime)
        {
            base.Update(Input, GTime);

            if (m_Mode != TextEditMode.ReadOnly)
            {
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

                if (Text != "")
                    m_VerticalTextBoundary = (int)(Position.X + m_Font.MeasureString(Text).X);
                else //Keep the text one character away from the beginning of the control
                     //to make it look nice.
                    m_VerticalTextBoundary = (int)(Position.X + m_Font.MeasureString("a").X);

                m_IsUpperCase = IsUpperCase(Input);

                if (m_HasFocus)
                {
                    if (m_RepChar == Keys.Back && m_LastRep == DateTime.Now)
                        RemoveCharacters();

                    Keys[] PressedKeys = Input.CurrentKeyboardState.GetPressedKeys();

                    foreach (Keys K in PressedKeys)
                    {
                        if (Input.IsNewPress(K))
                        {
                            switch (K)
                            {
                                case Keys.Enter:
                                    if (m_NumLines > 1)
                                    {
                                        m_TextPosition.X = Position.X;
                                        m_TextPosition.Y += m_Font.LineSpacing;
                                        m_CurrentLine.Add("\r\n");
                                        m_Lines.Add(new RenderableText2() { Text = GetCurrentLine(), Position = m_TextPosition });
                                        m_CurrentLine.Clear();

                                        m_Cursor.Position.X = Position.X;
                                        m_Cursor.Position.Y += m_Font.LineSpacing;
                                        m_Cursor.LineIndex += 1;
                                        m_Cursor.CharacterIndex = 0;

                                        m_MovingCursor = false;
                                        m_RemovingTxt = false;
                                    }
                                    break;
                                case Keys.Left:
                                    if (m_Cursor.Position.X > Position.X && m_Cursor.CharacterIndex > 0)
                                    {
                                        m_Cursor.Position.X -= m_Renderer.FontSpacing;
                                        m_Cursor.CharacterIndex--;
                                    }
                                    else if (m_Cursor.Position.X <= Position.X)
                                    {
                                        if (m_NumLines == 1)
                                            m_Renderer.ScrollTextRight();
                                    }

                                    m_MovingCursor = true;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.Right:
                                    if (m_Cursor.Position.X < (Position.X + m_Size.X))
                                    {
                                        if (GetCurrentLine().Length > 0 &&
                                            m_Cursor.CharacterIndex < GetCurrentLine().Length)
                                        {
                                            m_Cursor.Position.X += m_Renderer.FontSpacing;
                                            m_Cursor.CharacterIndex++;
                                        }
                                    }
                                    else if (m_Cursor.Position.X >= (Position.X + m_Size.X))
                                    {
                                        if (m_NumLines == 1)
                                            m_Renderer.ScrollTextLeft();
                                    }

                                    m_MovingCursor = true;
                                    m_RemovingTxt = false;
                                    break;
                                case Keys.Up:
                                    if (m_NumLines > 1)
                                    {
                                        if (m_Cursor.Position.Y >= Position.Y)
                                        {
                                            //Never allow the cursor to go beyond the height of the control!
                                            if(m_Cursor.Position.Y > Position.Y)
                                                m_Cursor.Position.Y -= m_Font.LineSpacing;

                                            if(m_Cursor.LineIndex > 0)
                                                m_Cursor.LineIndex--;

                                            ReplaceCurrentLine(m_Lines[m_Cursor.LineIndex].Text);

                                            //Part of a line was most likely deleted, so readjust the cursor accordingly.
                                            if (m_Cursor.CharacterIndex > GetCurrentLine().Length)
                                            {
                                                m_Cursor.CharacterIndex = GetCurrentLine().Length;
                                                m_Cursor.Position.X = Position.X + m_Renderer.FontSpacing;
                                            }
                                        }

                                        if (m_Cursor.Position.Y == Position.Y)
                                            ScrollUp();

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
                                                m_Cursor.Position.Y += m_Font.LineSpacing;
                                                ReplaceCurrentLine(m_Lines[m_Cursor.LineIndex].Text);
                                                m_Cursor.LineIndex++;

                                                //Part of a line was most likely deleted, so readjust the cursor accordingly.
                                                if (m_Cursor.CharacterIndex > GetCurrentLine().Length)
                                                {
                                                    m_Cursor.CharacterIndex = GetCurrentLine().Length;
                                                    m_Cursor.Position.X = Position.X + m_Renderer.FontSpacing;
                                                }
                                            }
                                        }

                                        if (m_Cursor.Position.Y >= (Position.Y + Size.Y))
                                        {
                                            if (!string.IsNullOrEmpty(GetCurrentLine()))
                                            {
                                                //Why does this line fuck up text scrolling??!
                                                m_Renderer.Insert(m_Cursor.LineIndex, GetCurrentLine());
                                                AddCurrentLine();
                                            }

                                            ScrollDown();
                                        }

                                        m_MovingCursor = true;
                                        m_RemovingTxt = false;
                                    }
                                    break;
                                case Keys.Back:
                                    RemoveCharacters();
                                    break;
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

            if (Visible)
            {
                Rectangle ScreenRect = new Rectangle(0, 0, SBatch.GraphicsDevice.Viewport.Width, 
                    SBatch.GraphicsDevice.Viewport.Height);

                SBatch.GraphicsDevice.ScissorRectangle = new Rectangle((int)Position.X, (int)Position.Y, 
                    (int)Size.X, (int)Size.Y);

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

                m_Renderer.DrawText(SBatch, Depth);

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
        /// Removes characters from the input, I.E backspace was pressed.
        /// </summary>
        private void RemoveCharacters()
        {
            m_RemovingTxt = true;

            //Cursor hasn't been moved.
            if (!m_MovingCursor)
            {
                if (m_Cursor.Position.X > Position.X)
                {
                    if (m_NumLines > 1)
                    {
                        if (GetCurrentLine().Length != 0)
                            MoveCursorLeft();
                    }
                    else
                    {
                        if (Text != string.Empty)
                        {
                            if (GetCurrentLine().Length > 0 && m_VerticalTextBoundary <= (Position.X + Size.X))
                                m_Cursor.Position.X -= m_Font.MeasureString(GetCurrentLine()[0].ToString()).X;
                        }
                    }
                }
            }

            //If the current line is empty, move the cursor up.
            /*if (m_Lines.Count > 0)
            {*/
                if (GetCurrentLine().Length == 0)
                {
                    if (m_NumLines > 1)
                    {
                        if (m_TextPosition.Y > Position.Y)
                        {
                            m_TextPosition.Y -= m_Font.LineSpacing;

                            m_Cursor.Position.X = Position.X + m_Size.X;
                            m_Cursor.Position.Y -= m_Font.LineSpacing;
                        }

                        if (m_Cursor.LineIndex > 0)
                        {
                            m_Cursor.LineIndex--;
                            ReplaceCurrentLine(m_Lines[m_Cursor.LineIndex].Text);
                            m_Cursor.CharacterIndex = m_Lines[m_Cursor.LineIndex].Text.Length;
                        }
                    }
                }
                else
                {
                    if (m_Cursor.CharacterIndex > 0)
                    {
                        if (m_NumLines == 1)
                        {
                            if (m_CurrentLine.Count >= 2)
                                MoveCursorLeft();
                        }
                        else
                            MoveCursorLeft();

                        m_CurrentLine[m_Cursor.CharacterIndex - 1] = "";
                        m_Renderer.RemoveAt(m_Cursor.CharacterIndex - 1);
                        m_Renderer.ScrollTextLeft(m_Cursor.CharacterIndex);
                        m_Cursor.CharacterIndex--;
                    }
                }
            //}

            //Cursor moved to the beginning of a line.
            if (m_Cursor.Position.X <= Position.X)
            {
                if (m_TextPosition.Y > Position.Y)
                {
                    m_TextPosition.Y -= m_Font.LineSpacing;

                    m_Cursor.Position.X = Position.X + m_Size.X;
                    m_Cursor.Position.Y -= m_Font.LineSpacing;
                }

                if (m_Cursor.LineIndex > 0)
                {
                    m_Cursor.LineIndex--;
                    m_Cursor.CharacterIndex = m_CurrentLine[m_Cursor.LineIndex].ToString().Length;
                }
            }
        }

        /// <summary>
        /// Moves the cursor to the left by the number of pixels used by the last character in the current input.
        /// </summary>
        private void MoveCursorLeft()
        {
            if(m_Cursor.Position.X > Position.X)
                m_Cursor.Position.X -= m_Font.MeasureString(m_CurrentLine[m_Cursor.CharacterIndex - 1].ToString()).X;
        }

        /// <summary>
        /// Adds the current line of text (m_CurrentLine) to
        /// the existing lines of text (m_Lines), and clears
        /// the contents of the current line.
        /// </summary>
        private void AddCurrentLine()
        {
            RenderableText2 RenderTxt = new RenderableText2();
            RenderTxt.Position = m_TextPosition;
            RenderTxt.Text = GetCurrentLine();
            m_Lines.Add(RenderTxt);
            m_CurrentLine.Clear();
        }

        /// <summary>
        /// Replaces the contents of m_CurrentLine with the specified text.
        /// Called when the cursor is moved up or down to a different line 
        /// of text.
        /// </summary>
        /// <param name="Text">The text with which to replace the current line.</param>
        private void ReplaceCurrentLine(string Text)
        {
            m_CurrentLine.Clear();

            for(int i = 0; i < Text.Length; i++)
                m_CurrentLine.Add(Text[i].ToString());
        }

        /// <summary>
        /// Returns true if shift is currently being pressed.
        /// </summary>
        /// <param name="Helper">An InputHelper instance.</param>
        /// <returns>True if it is being pressed, false otherwise.</returns>
        private bool IsUpperCase(InputHelper Helper)
        {
            if (Helper.IsCurPress(Keys.LeftShift) || Helper.IsCurPress(Keys.RightShift))
                return true;

            return false;
        }

        ~UITextEdit2()
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
