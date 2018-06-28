using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo
{
    /// <summary>
    /// Represents a line of renderable text.
    /// </summary>
    internal class RenderableText2
    {
        public string Text = "";
        public Vector2 Position;
    }

    /// <summary>
    /// Represents a renderable character.
    /// </summary>
    internal class RenderableCharacter
    {
        public string Char = "";
        public Vector2 Position;
    }

    /// <summary>
    /// Responsible for rendering text for UITextEdit.
    /// </summary>
    public class TextRenderer
    {
        /// <summary>
        /// Is this renderer for a single or multi line textbox?
        /// </summary>
        bool m_MultiLine = false;

        private Color m_TextColor;
        private SpriteFont m_Font;
        private Vector2 m_CurrentTextPosition = new Vector2(); //Position of the character currently being added.
        private float m_LineHeight;
        private Vector2 m_TextboxPosition;
        private Vector2 m_TextboxSize;
        private int m_ScrollFactor = 0;
        private List<RenderableText2> m_RenderableLines = new List<RenderableText2>();
        private GapBuffer<RenderableCharacter> m_CurrentLine = new GapBuffer<RenderableCharacter>();
        private int m_NumLines = 0;

        /// <summary>
        /// The contents of the GapBuffer containing the current line.
        /// </summary>
        /// <returns>The contents of the GapBuffer containing the current line.</returns>
        private string GetCurrentLine()
        {
            string Output = "";

            foreach (RenderableCharacter Char in m_CurrentLine)
                Output += Char.Char;

            return Output;
        }

        /// <summary>
        /// Constructs a new TextRenderer instance.
        /// </summary>
        /// <param name="TextboxPosition">Position of textbox using this renderer.</param>
        /// <param name="TextboxSize">Size of textbox using this renderer.</param>
        /// <param name="ScrollFactor">Scrolling factor of textbox using this renderer.</param>
        /// <param name="LineHeight">The height of a line of text.</param>
        /// <param name="Font">The font used to render the text.</param>
        /// <param name="NumLines">The maximum number of lines supported by this control.</param>
        public TextRenderer(bool MultiLine, Vector2 TextboxPosition, Vector2 TextboxSize, 
            int ScrollFactor, float LineHeight, SpriteFont Font, Color TxtColor, int NumLines)
        {
            m_MultiLine = MultiLine;
            m_TextboxPosition = TextboxPosition;
            m_CurrentTextPosition = m_TextboxPosition;
            m_TextboxSize = TextboxSize;
            m_ScrollFactor = ScrollFactor;
            m_LineHeight = LineHeight;
            m_Font = Font;
            m_TextColor = TxtColor;
            m_NumLines = NumLines;
        }

        /// <summary>
        /// Scrolls the current line of text to the right.
        /// </summary>
        public void ScrollTextRight()
        {
            for (int i = 0; i < m_CurrentLine.Count; i++)
                m_CurrentLine[i].Position.X += m_ScrollFactor;
        }

        /// <summary>
        /// Scrolls the current line of text to the left.
        /// </summary>
        /// <param name="Index">An optional index. If it's set, only the text from the index onwards will scroll.</param>
        public void ScrollTextLeft(int Index = 0)
        {
            for (int i = Index; i < m_CurrentLine.Count; i++)
                m_CurrentLine[i].Position.X -= m_ScrollFactor;
        }

        /// <summary>
        /// Scrolls all text up.
        /// </summary>
        /*public void ScrollTextUp()
        {
            foreach (RenderableText2 Txt in m_RenderableLines)
                Txt.Position.Y -= m_LineHeight;

            m_RenderableLines[m_VisibilityIndex].Visible = false;
            m_VisibilityIndex++;
        }*/

        /// <summary>
        /// Removes a character of renderable text from the input at the specified index.
        /// </summary>
        /// <param name="Index">The index at which to remove a renderable character.</param>
        public void RemoveAt(int Index)
        {
            if (Index > 0 && m_CurrentLine.Count >= 1)
            {
                m_CurrentTextPosition.X -= m_Font.MeasureString(m_CurrentLine[Index].Char).X;

                RenderableCharacter RenderChar = m_CurrentLine[Index];
                RenderChar.Char = "";

                m_CurrentLine.RemoveAt(Index);
                m_CurrentLine.Insert(Index, RenderChar);
            }
        }

        /// <summary>
        /// Inserts a character or a line of text into this TextRenderer instance.
        /// </summary>
        /// <param name="Index">The index at which to insert text.</param>
        /// <param name="Char">The character to insert.</param>
        public void Insert(int Index, string Char)
        {
            if (m_MultiLine)
            {
                if ((m_CurrentTextPosition.X < m_TextboxSize.X) && Char.Length == 1)
                {
                    m_CurrentTextPosition.X += m_Font.MeasureString(Char).X;
                    RenderableCharacter RenderChar = new RenderableCharacter();
                    RenderChar.Position = m_CurrentTextPosition;
                    RenderChar.Char = Char;

                    if (m_CurrentLine.Capacity <= Index)
                        m_CurrentLine.Capacity++;

                    m_CurrentLine.Insert(Index, RenderChar);
                }
                else
                {
                    RenderableText2 Line = new RenderableText2();
                    Line.Text = GetCurrentLine();
                    Line.Position = new Vector2(m_TextboxPosition.X + 2, m_CurrentTextPosition.Y);

                    m_RenderableLines.Add(Line);

                    if (m_CurrentTextPosition.Y <= m_TextboxPosition.Y + ((m_NumLines - 2) * m_Font.LineSpacing))
                        m_CurrentTextPosition.Y += m_Font.LineSpacing;

                    m_CurrentLine.Clear();

                    m_CurrentTextPosition.X = m_TextboxPosition.X;
                    RenderableCharacter RenderChar = new RenderableCharacter();
                    RenderChar.Position = m_CurrentTextPosition;
                    RenderChar.Char = Char;

                    if (m_CurrentTextPosition.Y >= m_TextboxPosition.Y + ((m_NumLines - 2) * m_Font.LineSpacing))
                    {
                        foreach (RenderableText2 Txt in m_RenderableLines)
                            Txt.Position.Y -= m_Font.LineSpacing;
                    }

                    m_CurrentLine.Insert(0, RenderChar);
                }
            }
            else
            {
                if (m_CurrentTextPosition.X < m_TextboxSize.X)
                    m_CurrentTextPosition.X += m_Font.MeasureString(Char).X;
                else
                    ScrollTextLeft();

                RenderableCharacter RenderChar = new RenderableCharacter();
                RenderChar.Position = m_CurrentTextPosition;
                RenderChar.Char = Char;

                m_CurrentLine.Insert(Index, RenderChar);
            }
        }

        /// <summary>
        /// Renders the text stored by this TextRenderer instance.
        /// </summary>
        /// <param name="SBatch">A SpriteBatch instance for drawing the text.</param>
        /// <param name="Depth">The depth at which to draw the text.</param>
        public void DrawText(SpriteBatch SBatch, float Depth)
        {
            if (m_MultiLine)
            {
                foreach (RenderableCharacter Txt in m_CurrentLine)
                {
                    SBatch.DrawString(m_Font, Txt.Char, new Vector2(Txt.Position.X, Txt.Position.Y),
                        m_TextColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, Depth + 0.1f);
                }

                foreach (RenderableText2 Txt in m_RenderableLines)
                {
                    SBatch.DrawString(m_Font, Txt.Text, new Vector2(Txt.Position.X, Txt.Position.Y),
                        m_TextColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, Depth + 0.1f);
                }
            }
            else
            {
                foreach (RenderableCharacter Txt in m_CurrentLine)
                {
                    SBatch.DrawString(m_Font, Txt.Char, new Vector2(Txt.Position.X, Txt.Position.Y), 
                        m_TextColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, Depth + 0.1f);
                }
            }
        }

        private bool m_MaxScrollup = false;

        /// <summary>
        /// Can the text be scrolled up any further?
        /// </summary>
        public bool MaxScrollup
        {
            get { return m_MaxScrollup; }
        }


        private bool m_MaxScrolldown = false;

        /// <summary>
        /// Can the text be scrolled down any further?
        /// </summary>
        public bool MaxScrolldown
        {
            get { return m_MaxScrolldown; }
        }

        /// <summary>
        /// Can the text in this TextRenderer be scrolled?
        /// </summary>
        public bool CanScrollDown()
        {
            if (m_RenderableLines.Count >= 1)
            {
                if ((m_RenderableLines[m_RenderableLines.Count - 1].Position.Y - m_TextboxPosition.Y) 
                    > (m_TextboxPosition.Y - m_TextboxSize.Y))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Scrolls the text in the textbox downwards.
        /// Called when clicking a scrolldown button for this textbox.
        /// </summary>
        /// <returns>True if the text can still be scrolled down, false otherwise.</returns>
        public bool ScrollDown()
        {
            ScrollTextDown();

            if(m_CurrentLine.Count != 0)
            {
                RenderableText2 Line = new RenderableText2();
                Line.Text = GetCurrentLine();
                Line.Position = new Vector2(m_TextboxPosition.X + 2, m_CurrentTextPosition.Y + m_Font.LineSpacing);

                m_RenderableLines.Add(Line);
                m_CurrentLine.Clear();
            }

            if (!MaxScrolldown)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Performs the memory movement of scrolling text down.
        /// </summary>
        private void ScrollTextDown()
        {
            for (int i = 0; i < m_RenderableLines.Count; i++)
            {
                //TODO: Fix this!
                if (m_RenderableLines[m_RenderableLines.Count - 1].Position.Y == 
                    (m_TextboxPosition.Y - m_TextboxSize.Y))
                    m_MaxScrolldown = true;

                if (!MaxScrolldown)
                {
                    m_RenderableLines[i].Position.Y -= m_Font.LineSpacing;

                    if (MaxScrollup == true)
                        m_MaxScrollup = false;
                }
            }
        }

        /// <summary>
        /// Can the text in this TextRenderer be scrolled?
        /// </summary>
        public bool CanScrollUp()
        {
            if (m_RenderableLines.Count >= 1)
            {
                if (m_RenderableLines[0].Position.Y < m_TextboxPosition.Y)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Scrolls the text in the textbox upwards.
        /// Called when clicking a scrollup button for this textbox.
        /// </summary>
        /// <returns>True if the text can still be scrolled up, false otherwise.</returns>
        public bool ScrollUp()
        {
            ScrollTextUp();

            if (!MaxScrollup)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Performs the memory movement of scrolling text up.
        /// </summary>
        private void ScrollTextUp()
        {
            for (int i = 0; i < m_RenderableLines.Count; i++)
            {
                if (m_RenderableLines[0].Position.Y >= m_TextboxPosition.Y)
                    m_MaxScrollup = true;

                if (!MaxScrollup)
                {
                    m_RenderableLines[i].Position.Y += m_Font.LineSpacing;

                    if (MaxScrolldown == true)
                        m_MaxScrolldown = false;
                }
            }
        }
    }
}
