using System.Timers;
using System.Collections.Generic;
using System.Text;
using Files;
using Files.Manager;
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

    public class UITextEdit : UIElement
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
                if (m_NumLines > 1)
                {
                    //Check that text doesn't go beyond width of control...
                    if ((m_Font.MeasureString(m_Lines[m_Cursor.LineIndex].SBuilder.ToString()).X >= m_Size.X) && 
                        !m_RemovingTxt && !m_MovingCursor)
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
                    if (m_Font.MeasureString(m_Lines[m_Cursor.LineIndex].SBuilder.ToString()).X >= (m_Size.X - m_Font.MeasureString("a").X) && 
                        !m_RemovingTxt)
                    {
                        m_Lines.Add(new RenderableText() { SBuilder = new StringBuilder(), Position = m_TextPosition, Visible = false });

                        m_Cursor.Position.X = m_Size.X;
                    }
                }
            }

            m_VerticalTextBoundary = (int)(Position.X + m_Font.MeasureString(m_Lines[m_Cursor.LineIndex].SBuilder.ToString()).X);

            if(m_HasFocus)
            {
                bool Upper = false;
                Keys[] PressedKeys = Input.CurrentKeyboardState.GetPressedKeys();

                if (IsUpperCase(Input))
                    Upper = true;

                foreach(Keys K in PressedKeys)
                {
                    if (Input.IsNewPress(K))
                    {
                        switch (K)
                        {
                            case Keys.A:
                                switch (Input.InputRegion.LayoutName)
                                {
                                    case "Norwegian":
                                    case "Swedish":
                                    case "Danish":
                                    case "English":
                                    case "Estonian":
                                        if (Upper)
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("A");
                                            m_Cursor.Position.X += m_Font.MeasureString("A").X;
                                        }
                                        else
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("a");
                                            m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                        }
                                        break;
                                    case "French":
                                        if (Upper)
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("Q");
                                            m_Cursor.Position.X += m_Font.MeasureString("Q").X;
                                        }
                                        else
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("q");
                                            m_Cursor.Position.X += m_Font.MeasureString("q").X;
                                        }
                                        break;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.B:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("B");
                                    m_Cursor.Position.X += m_Font.MeasureString("B").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("b");
                                    m_Cursor.Position.X += m_Font.MeasureString("b").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.C:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("C");
                                    m_Cursor.Position.X += m_Font.MeasureString("C").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("c");
                                    m_Cursor.Position.X += m_Font.MeasureString("c").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.D:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("D");
                                    m_Cursor.Position.X += m_Font.MeasureString("D").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("d");
                                    m_Cursor.Position.X += m_Font.MeasureString("d").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.E:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("E");
                                    m_Cursor.Position.X += m_Font.MeasureString("E").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("e");
                                    m_Cursor.Position.X += m_Font.MeasureString("e").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.F:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("F");
                                    m_Cursor.Position.X += m_Font.MeasureString("F").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("f");
                                    m_Cursor.Position.X += m_Font.MeasureString("f").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.G:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("G");
                                    m_Cursor.Position.X += m_Font.MeasureString("G").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("g");
                                    m_Cursor.Position.X += m_Font.MeasureString("g").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.H:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("H");
                                    m_Cursor.Position.X += m_Font.MeasureString("H").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("h");
                                    m_Cursor.Position.X += m_Font.MeasureString("h").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.I:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("I");
                                    m_Cursor.Position.X += m_Font.MeasureString("I").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("i");
                                    m_Cursor.Position.X += m_Font.MeasureString("i").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.J:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("J");
                                    m_Cursor.Position.X += m_Font.MeasureString("J").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("j");
                                    m_Cursor.Position.X += m_Font.MeasureString("j").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.K:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("K");
                                    m_Cursor.Position.X += m_Font.MeasureString("K").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("k");
                                    m_Cursor.Position.X += m_Font.MeasureString("k").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.L:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("L");
                                    m_Cursor.Position.X += m_Font.MeasureString("L").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("l");
                                    m_Cursor.Position.X += m_Font.MeasureString("l").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.M:
                                switch (Input.InputRegion.LayoutName)
                                {
                                    case "English":
                                    case "Norwegian":
                                    case "Swedish":
                                    case "Danish":
                                    case "Estonian":
                                        if (Upper)
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("M");
                                            m_Cursor.Position.X += m_Font.MeasureString("M").X;
                                        }
                                        else
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("m");
                                            m_Cursor.Position.X += m_Font.MeasureString("m").X;
                                        }
                                        break;
                                    case "French":
                                        m_Lines[m_Cursor.LineIndex].SBuilder.Append("?");
                                        m_Cursor.Position.X += m_Font.MeasureString("?").X;
                                        break;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.N:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("N");
                                    m_Cursor.Position.X += m_Font.MeasureString("N").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("n");
                                    m_Cursor.Position.X += m_Font.MeasureString("n").X;
                                }

                                m_RemovingTxt = false;
                                break;
                            case Keys.O:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("O");
                                    m_Cursor.Position.X += m_Font.MeasureString("O").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("o");
                                    m_Cursor.Position.X += m_Font.MeasureString("o").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.P:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("P");
                                    m_Cursor.Position.X += m_Font.MeasureString("P").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("p");
                                    m_Cursor.Position.X += m_Font.MeasureString("p").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.Q:
                                switch (Input.InputRegion.LayoutName)
                                {
                                    case "English":
                                    case "Norwegian":
                                    case "Swedish":
                                    case "Danish":
                                    case "Estonian":
                                        if (Upper)
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("Q");
                                            m_Cursor.Position.X += m_Font.MeasureString("Q").X;
                                        }
                                        else
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("q");
                                            m_Cursor.Position.X += m_Font.MeasureString("q").X;
                                        }
                                        break;
                                    case "French":
                                        if (Upper)
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("A");
                                            m_Cursor.Position.X += m_Font.MeasureString("A").X;
                                        }
                                        else
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("a");
                                            m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                        }
                                        break;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.S:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("S");
                                    m_Cursor.Position.X += m_Font.MeasureString("S").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("s");
                                    m_Cursor.Position.X += m_Font.MeasureString("s").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.T:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("T");
                                    m_Cursor.Position.X += m_Font.MeasureString("T").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("t");
                                    m_Cursor.Position.X += m_Font.MeasureString("t").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.U:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("U");
                                    m_Cursor.Position.X += m_Font.MeasureString("U").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("u");
                                    m_Cursor.Position.X += m_Font.MeasureString("u").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.V:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("V");
                                    m_Cursor.Position.X += m_Font.MeasureString("V").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("v");
                                    m_Cursor.Position.X += m_Font.MeasureString("v").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.W:
                                switch (Input.InputRegion.LayoutName)
                                {
                                    case "English":
                                    case "Norwegian":
                                    case "Swedish":
                                    case "Danish":
                                    case "Estonian":
                                        if (Upper)
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("W");
                                            m_Cursor.Position.X += m_Font.MeasureString("W").X;
                                        }
                                        else
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("w");
                                            m_Cursor.Position.X += m_Font.MeasureString("w").X;
                                        }
                                        break;
                                    case "French":
                                        if (Upper)
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("Z");
                                            m_Cursor.Position.X += m_Font.MeasureString("Z").X;
                                        }
                                        else
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("z");
                                            m_Cursor.Position.X += m_Font.MeasureString("z").X;
                                        }
                                        break;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.X:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("X");
                                    m_Cursor.Position.X += m_Font.MeasureString("X").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("x");
                                    m_Cursor.Position.X += m_Font.MeasureString("x").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.Y:
                                if (Upper)
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("Y");
                                    m_Cursor.Position.X += m_Font.MeasureString("Y").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append("y");
                                    m_Cursor.Position.X += m_Font.MeasureString("y").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.Z:
                                switch (Input.InputRegion.LayoutName)
                                {
                                    case "English":
                                    case "Norwegian":
                                    case "Swedish":
                                    case "Danish":
                                    case "Estonian":
                                        if (Upper)
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("Z");
                                            m_Cursor.Position.X += m_Font.MeasureString("Z").X;
                                        }
                                        else
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("z");
                                            m_Cursor.Position.X += m_Font.MeasureString("z").X;
                                        }
                                        break;
                                    case "French":
                                        if (Upper)
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("W");
                                            m_Cursor.Position.X += m_Font.MeasureString("W").X;
                                        }
                                        else
                                        {
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("w");
                                            m_Cursor.Position.X += m_Font.MeasureString("w").X;
                                        }
                                        break;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.Divide:
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
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append(";");
                                            m_Cursor.Position.X += m_Font.MeasureString(";").X;
                                            break;
                                        case "Norwegian":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("Ø");
                                                m_Cursor.Position.X += m_Font.MeasureString("Ø").X;
                                            }
                                            else
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("ø");
                                                m_Cursor.Position.X += m_Font.MeasureString("ø").X;
                                            }

                                            break;
                                        case "French":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("M");
                                                m_Cursor.Position.X += m_Font.MeasureString("M").X;
                                            }
                                            else
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("m");
                                                m_Cursor.Position.X += m_Font.MeasureString("m").X;
                                            }

                                            break;
                                        case "Swedish":
                                        case "Finnish":
                                        case "Estonian":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("Ö");
                                                m_Cursor.Position.X += m_Font.MeasureString("Ö").X;
                                            }
                                            else
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("ö");
                                                m_Cursor.Position.X += m_Font.MeasureString("ö").X;
                                            }

                                            break;
                                        case "Danish":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("Æ");
                                                m_Cursor.Position.X += m_Font.MeasureString("Æ").X;
                                            }
                                            else
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("æ");
                                                m_Cursor.Position.X += m_Font.MeasureString("æ").X;
                                            }

                                            break;
                                    }
                                }

                                m_Cursor.CharacterIndex++;
                                m_RemovingTxt = false;
                                break;
                            case Keys.OemQuotes:
                                if (Input.InputRegion != null)
                                {
                                    switch (Input.InputRegion.LayoutName)
                                    {
                                        case "English": //TODO: Should this be double quote if upper??
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("'");
                                            m_Cursor.Position.X += m_Font.MeasureString("'").X;
                                            m_Cursor.CharacterIndex++;
                                            break;
                                        case "Norwegian":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("Æ");
                                                m_Cursor.Position.X += m_Font.MeasureString("Æ").X;
                                            }
                                            else
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("æ");
                                                m_Cursor.Position.X += m_Font.MeasureString("æ").X;
                                            }

                                            m_Cursor.CharacterIndex++;
                                            break;
                                        case "Swedish":
                                        case "Finnish":
                                        case "Estonian":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("Ä");
                                                m_Cursor.Position.X += m_Font.MeasureString("Ä").X;
                                            }
                                            else
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("ä");
                                                m_Cursor.Position.X += m_Font.MeasureString("ä").X;
                                            }

                                            m_Cursor.CharacterIndex++;
                                            break;
                                        case "Danish":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("Ø");
                                                m_Cursor.Position.X += m_Font.MeasureString("Ø").X;
                                            }
                                            else
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("ø");
                                                m_Cursor.Position.X += m_Font.MeasureString("ø").X;
                                            }

                                            m_Cursor.CharacterIndex++;
                                            break;
                                    }
                                }

                                m_RemovingTxt = false;
                                break;
                            case Keys.OemOpenBrackets:
                                if (Input.InputRegion != null)
                                {
                                    switch (Input.InputRegion.LayoutName)
                                    {
                                        case "English":
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("{");
                                            m_Cursor.Position.X += m_Font.MeasureString("{").X;
                                            m_Cursor.CharacterIndex++;
                                            break;
                                        case "Norwegian":
                                        case "Swedish":
                                        case "Finnish":
                                        case "Danish":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("Å");
                                                m_Cursor.Position.X += m_Font.MeasureString("Å").X;
                                            }
                                            else
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("å");
                                                m_Cursor.Position.X += m_Font.MeasureString("å").X;
                                            }

                                            m_Cursor.CharacterIndex++;
                                            break;
                                        case "Estonian":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("Ü");
                                                m_Cursor.Position.X += m_Font.MeasureString("Ü").X;
                                            }
                                            else
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("ü");
                                                m_Cursor.Position.X += m_Font.MeasureString("ü").X;
                                            }

                                            m_Cursor.CharacterIndex++;
                                            break;
                                    }
                                }

                                m_RemovingTxt = false;
                                break;
                            case Keys.OemCloseBrackets:
                                if (Input.InputRegion != null)
                                {
                                    switch (Input.InputRegion.LayoutName)
                                    {
                                        case "English":
                                            m_Lines[m_Cursor.LineIndex].SBuilder.Append("}");
                                            m_Cursor.Position.X += m_Font.MeasureString("}").X;
                                            m_Cursor.CharacterIndex++;
                                            break;
                                        case "Norwegian":
                                        case "Swedish":
                                        case "Finnish":
                                        case "Danish":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("^");
                                                m_Cursor.Position.X += m_Font.MeasureString("^").X;
                                            }
                                            else if(Input.IsCurPress(Keys.RightAlt))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("~");
                                                m_Cursor.Position.X += m_Font.MeasureString("~").X;
                                            }

                                            m_Cursor.CharacterIndex++;
                                            break;
                                        case "Estonian":
                                            if (IsUpperCase(Input))
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("Õ");
                                                m_Cursor.Position.X += m_Font.MeasureString("Õ").X;
                                            }
                                            else
                                            {
                                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("õ");
                                                m_Cursor.Position.X += m_Font.MeasureString("õ").X;
                                            }

                                            m_Cursor.CharacterIndex++;
                                            break;
                                    }
                                }

                                m_RemovingTxt = false;
                                break;
                            case Keys.OemPlus:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("+");
                                m_Cursor.Position.X += m_Font.MeasureString("+").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.OemComma:
                                if (IsUpperCase(Input))
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append(";");
                                    m_Cursor.Position.X += m_Font.MeasureString(";").X;
                                }
                                else
                                {
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Append(",");
                                    m_Cursor.Position.X += m_Font.MeasureString(",").X;
                                }

                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.OemPeriod:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append(".");
                                m_Cursor.Position.X += m_Font.MeasureString(".").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.Space:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append(" ");
                                m_Cursor.Position.X += m_Font.MeasureString(" ").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.Tab:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("    ");
                                m_Cursor.Position.X += m_Font.MeasureString("   ").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.Subtract:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("-");
                                m_Cursor.Position.X += m_Font.MeasureString("-").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.NumPad0:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("0");
                                m_Cursor.Position.X += m_Font.MeasureString("0").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.NumPad1:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("1");
                                m_Cursor.Position.X += m_Font.MeasureString("1").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.NumPad2:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("2");
                                m_Cursor.Position.X += m_Font.MeasureString("2").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.NumPad3:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("3");
                                m_Cursor.Position.X += m_Font.MeasureString("3").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.NumPad4:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("4");
                                m_Cursor.Position.X += m_Font.MeasureString("4").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.NumPad5:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("5");
                                m_Cursor.Position.X += m_Font.MeasureString("5").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.NumPad6:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("6");
                                m_Cursor.Position.X += m_Font.MeasureString("6").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.NumPad7:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("7");
                                m_Cursor.Position.X += m_Font.MeasureString("7").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.NumPad8:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("8");
                                m_Cursor.Position.X += m_Font.MeasureString("8").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.NumPad9:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("9");
                                m_Cursor.Position.X += m_Font.MeasureString("9").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.Multiply:
                                m_Lines[m_Cursor.LineIndex].SBuilder.Append("*");
                                m_Cursor.Position.X += m_Font.MeasureString("*").X;
                                m_Cursor.CharacterIndex++;

                                m_RemovingTxt = false;
                                break;
                            case Keys.Enter:
                                m_Lines.Add(new RenderableText() { SBuilder = new StringBuilder(), Position = m_TextPosition });
                                m_TextPosition.Y += m_Font.LineSpacing;

                                m_Cursor.Position.Y += m_Font.LineSpacing;
                                m_Cursor.LineIndex += 1;
                                m_Cursor.CharacterIndex = 0;

                                m_RemovingTxt = false;
                                break;
                            case Keys.Back:
                                m_RemovingTxt = true;

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

                                if (m_Lines[m_Cursor.LineIndex].SBuilder.Length == 0)
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
                                    m_Lines[m_Cursor.LineIndex].SBuilder.Remove((int)(m_Cursor.CharacterIndex - 1), 1);
                                    m_Cursor.CharacterIndex--;
                                    m_Cursor.Position.X -= m_Font.MeasureString("a").X;
                                }

                                //Cursor moved to the beginning of a line.
                                if(m_Cursor.Position.X <= Position.X)
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
                                }

                                m_MovingCursor = true;
                                m_RemovingTxt = false;
                                break;
                            case Keys.Right:
                                if (m_Cursor.Position.X < (Position.X + m_Size.X))
                                {
                                    if (m_Lines[m_Cursor.LineIndex].SBuilder.Length > 0 &&
                                        m_Cursor.CharacterIndex < m_Lines[m_Cursor.LineIndex].SBuilder.Length)
                                    {
                                        m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                        m_Cursor.CharacterIndex++;
                                    }
                                }

                                m_MovingCursor = true;
                                m_RemovingTxt = false;
                                break;
                            case Keys.Up:
                                if (m_Cursor.Position.Y > Position.Y)
                                {
                                    m_Cursor.Position.Y -= m_Font.LineSpacing;
                                    m_Cursor.LineIndex--;
                                }

                                m_MovingCursor = true;
                                m_RemovingTxt = false;
                                break;
                            case Keys.Down:
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
                                break;
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

                if (m_Cursor.Visible == true)
                {
                    SBatch.DrawString(m_Font, " " + m_Cursor.Cursor, new Vector2(m_Cursor.Position.X,
                        m_Cursor.Position.Y), TextColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, 
                        SpriteEffects.None, Depth + 0.1f);
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
    }
}
