
// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Textbox
{
    public class Cursor
    {
        public Color Color { get; set; }
        public Color Selection { get; set; }
        public Rectangle Icon { get; set; }

        public bool Active { get; set; }

        private bool visible;
        private readonly int ticksPerBlink;
        private int ticks;

        /// <summary>
        ///     The current location of the cursor in the array
        /// </summary>
        public int TextCursor
        {
            get { return textCursor; }
            set { textCursor = value.Clamp(0, textBox.Text.Length); }
        }

        /// <summary>
        ///     All characters between SelectedChar and the TextCursor are selected
        ///     when SelectedChar != null. Cannot be the same as the TextCursor value.
        /// </summary>
        public int? SelectedChar
        {
            get { return selectedChar; }
            set
            {
                if (value.HasValue)
                {
                    if (value.Value != TextCursor)
                    {
                        selectedChar = (short)(value.Value.Clamp(0, textBox.Text.Length));
                    }
                }
                else
                {
                    selectedChar = null;
                }
            }
        }

        private readonly TextBox textBox;

        private int textCursor;
        private int? selectedChar;

        public Cursor(TextBox textBox, Color color, Color selection, Rectangle icon, int ticksPerBlink)
        {
            this.textBox = textBox;
            Color = color;
            Selection = selection;
            Icon = icon;
            Active = true;
            visible = false;
            this.ticksPerBlink = ticksPerBlink;
            ticks = 0;
        }

        public void Update()
        {
            ticks++;

            if (ticks <= ticksPerBlink)
            {
                return;
            }

            visible = !visible;
            ticks = 0;
        }

        public void Draw(SpriteBatch spriteBatch/*, float? LayerDepth*/)
        {
            /*float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.9f;*/ //Cursors are always drawn on top.

            // Top left corner of the text area.
            int x = textBox.Renderer.Area.X;
            int y = textBox.Renderer.Area.Y;

            Point cp = GetPosition(x, y, TextCursor);
            if (selectedChar.HasValue)
            {
                Point sc = GetPosition(x, y, selectedChar.Value);
                if (sc.X > cp.X)
                {
                    spriteBatch.Draw(spriteBatch.GetWhitePixel(),
                        new Rectangle(cp.X, cp.Y, sc.X - cp.X, textBox.Renderer.Font.LineSpacing), Icon, Selection,
                        0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, 0/*Depth*/);
                }
                else
                {
                    spriteBatch.Draw(spriteBatch.GetWhitePixel(),
                        new Rectangle(sc.X, sc.Y, cp.X - sc.X, textBox.Renderer.Font.LineSpacing), Icon, Selection,
                        0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, 0 /*/ Depth */);
                }
            }

            if (!visible)
            {
                return;
            }

            spriteBatch.Draw(spriteBatch.GetWhitePixel(),
                new Rectangle(cp.X, cp.Y, Icon.Width, textBox.Renderer.Font.LineSpacing), Icon, Color,
                0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, 0/*Depth*/);
        }

        private Point GetPosition(int x, int y, int pos)
        {
            if (pos > 0)
            {
                if (textBox.Text.Characters[pos - 1] == '\n'
                    || textBox.Text.Characters[pos - 1] == '\r')
                {
                    // Beginning of next line.
                    y += textBox.Renderer.Y[pos - 1] + textBox.Renderer.Font.LineSpacing;
                }
                else if (pos == textBox.Text.Length)
                {
                    // After last character.
                    x += textBox.Renderer.X[pos - 1] + textBox.Renderer.Width[pos - 1];
                    y += textBox.Renderer.Y[pos - 1];
                }
                else
                {
                    // Beginning of current character.
                    x += textBox.Renderer.X[pos];
                    y += textBox.Renderer.Y[pos];
                }
            }
            return new Point(x, y);
        }
    }
}