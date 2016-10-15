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
        private bool m_HasFocus = false;

        private StringBuilder m_SBuilder = new StringBuilder();

        private List<RenderableText> m_Lines = new List<RenderableText>();

        public UITextEdit(AddTextEditNode Node, ParserState State, UIScreen Screen) : base(Screen)
        {
            m_Name = Node.Name;
            m_ID = Node.ID;

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
                    Image = new UIImage(FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_textboxbackground), m_Screen);
                    if(Position != null)
                        Image.Position = Position;
                    Image.Slicer = new NineSlicer(new Vector2(0, 0), Image.Texture.Width, Image.Texture.Height, 20, 20, 20, 20);
                    Image.SetSize((int)Size.X, (int)Size.Y);
                }
                else
                {
                    m_BackColor = new Color(57, 81, 110, 255);
                    Image = new UIImage(FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_textboxbackground), m_Screen);
                    if(Position != null)
                        Image.Position = Position;
                    Image.Slicer = new NineSlicer(new Vector2(0, 0), Image.Texture.Width, Image.Texture.Height, 20, 20, 20, 20);
                    Image.SetSize((int)Size.X, (int)Size.Y);
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
                    Image.Position = Position;
                }
                if (State.Tooltip != "")
                    Tooltip = State.Tooltip;
            }

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
            if (m_Mode != TextEditMode.ReadOnly)
            {
                if (IsMouseOver(Input))
                {
                    if (Input.IsNewPress(MouseButtons.LeftButton))
                        m_HasFocus = true;
                }

                if (m_NumLines > 0)
                {
                    //Check that text doesn't go beyond width of control...
                    if (m_Font.MeasureString(m_SBuilder.ToString()).X >= m_Size.X)
                    {
                        if (m_TextPosition.Y <= Position.Y + ((m_NumLines - 2) * m_Font.LineSpacing))
                        {
                            m_Lines.Add(new RenderableText() { SBuilder = m_SBuilder, Position = m_TextPosition });
                            m_SBuilder = new StringBuilder();
                            m_TextPosition.Y += m_Font.LineSpacing;
                        }
                        else //Text went beyond the borders of the control...
                        {
                            foreach (RenderableText Txt in m_Lines)
                                Txt.Position.Y -= m_Font.LineSpacing;

                            m_Lines.Add(new RenderableText() { SBuilder = m_SBuilder, Position = m_TextPosition });
                            m_SBuilder = new StringBuilder();
                            m_ScrollbarHeight -= m_Font.LineSpacing;

                            m_Lines[m_VisibilityIndex].Visible = false;
                            m_VisibilityIndex++;
                        }

                        m_Cursor.Position.X = Position.X;
                    }
                }
                else
                {
                    //TODO: Scroll text backwards...
                }
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

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.B:
                                if (Upper)
                                    m_SBuilder.Append("B");
                                else
                                    m_SBuilder.Append("b");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.C:
                                if (Upper)
                                    m_SBuilder.Append("C");
                                else
                                    m_SBuilder.Append("c");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.D:
                                if (Upper)
                                    m_SBuilder.Append("D");
                                else
                                    m_SBuilder.Append("d");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.E:
                                if (Upper)
                                    m_SBuilder.Append("F");
                                else
                                    m_SBuilder.Append("f");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.F:
                                if (Upper)
                                    m_SBuilder.Append("F");
                                else
                                    m_SBuilder.Append("f");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.G:
                                if (Upper)
                                    m_SBuilder.Append("G");
                                else
                                    m_SBuilder.Append("g");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.H:
                                if (Upper)
                                    m_SBuilder.Append("H");
                                else
                                    m_SBuilder.Append("h");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.I:
                                if (Upper)
                                    m_SBuilder.Append("I");
                                else
                                    m_SBuilder.Append("i");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.J:
                                if (Upper)
                                    m_SBuilder.Append("J");
                                else
                                    m_SBuilder.Append("j");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.K:
                                if (Upper)
                                    m_SBuilder.Append("K");
                                else
                                    m_SBuilder.Append("k");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.L:
                                if (Upper)
                                    m_SBuilder.Append("L");
                                else
                                    m_SBuilder.Append("l");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.M:
                                if (Upper)
                                    m_SBuilder.Append("M");
                                else
                                    m_SBuilder.Append("m");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.N:
                                if (Upper)
                                    m_SBuilder.Append("N");
                                else
                                    m_SBuilder.Append("n");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.O:
                                if (Upper)
                                    m_SBuilder.Append("O");
                                else
                                    m_SBuilder.Append("o");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.P:
                                if (Upper)
                                    m_SBuilder.Append("P");
                                else
                                    m_SBuilder.Append("p");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.Q:
                                if (Upper)
                                    m_SBuilder.Append("R");
                                else
                                    m_SBuilder.Append("r");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.S:
                                if (Upper)
                                    m_SBuilder.Append("S");
                                else
                                    m_SBuilder.Append("s");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.T:
                                if (Upper)
                                    m_SBuilder.Append("T");
                                else
                                    m_SBuilder.Append("t");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.U:
                                if (Upper)
                                    m_SBuilder.Append("U");
                                else
                                    m_SBuilder.Append("u");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.V:
                                if (Upper)
                                    m_SBuilder.Append("V");
                                else
                                    m_SBuilder.Append("v");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.W:
                                if (Upper)
                                    m_SBuilder.Append("W");
                                else
                                    m_SBuilder.Append("w");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.X:
                                if (Upper)
                                    m_SBuilder.Append("X");
                                else
                                    m_SBuilder.Append("X");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.Y:
                                if (Upper)
                                    m_SBuilder.Append("Y");
                                else
                                    m_SBuilder.Append("y");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.Z:
                                if (Upper)
                                    m_SBuilder.Append("Z");
                                else
                                    m_SBuilder.Append("z");

                                m_Cursor.Position.X += m_Font.MeasureString("a").X;
                                break;
                            case Keys.Back:
                                if (m_SBuilder.Length > 0)
                                {
                                    m_SBuilder.Remove(m_SBuilder.Length - 1, 1);
                                    m_Cursor.Position.X -= m_Font.MeasureString("a").X;
                                }
                                else
                                {
                                    m_SBuilder.Append(m_Lines[m_Lines.Count - 1].SBuilder.ToString());
                                    m_Lines.Remove(m_Lines[m_Lines.Count - 1]);

                                    m_TextPosition.X = Position.X;
                                    m_TextPosition.Y -= m_Font.LineSpacing;

                                    m_Cursor.Position.X = Position.X + m_Size.X;
                                    m_Cursor.Position.Y -= m_Font.LineSpacing;
                                }
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

                SBatch.DrawString(m_Font, m_SBuilder.ToString(), new Vector2(m_TextPosition.X, 
                    (m_VisibilityIndex > 0) ? m_TextPosition.Y + m_Font.LineSpacing : m_TextPosition.Y), 
                    TextColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, Depth + 0.1f);

                if (m_Cursor.Visible == true)
                {
                    SBatch.DrawString(m_Font, " " + m_Cursor.Cursor, new Vector2(m_Cursor.Position.X,
                        (m_VisibilityIndex > 0) ? m_TextPosition.Y + m_Font.LineSpacing : m_TextPosition.Y), 
                        TextColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, Depth + 0.1f);
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
