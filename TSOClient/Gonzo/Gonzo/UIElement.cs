using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo
{
    public class UIElement
    {
        protected Matrix m_Mat;
        protected UIElement m_Parent;
        protected Dictionary<string, UIElement> m_Elements = new Dictionary<string, UIElement>();
        protected string m_Name;
        protected int m_ID;

        public Matrix PositionMatrix { get { return m_Mat; } }

        protected List<CaretSeparatedText> m_StringTables = new List<CaretSeparatedText>();
        protected Dictionary<string, string> m_Strings = new Dictionary<string, string>();
        protected bool m_Opaque = false;
        protected Vector2 m_Size, m_Position;
        public Color TextColor;

        public string Tooltip { get; set; }
        public Texture2D Image;

        public int Tracking, Trigger;

        public virtual void Update(InputHelper Helper) { }
        public virtual void Draw(SpriteBatch SBatch) { }
        public virtual void Draw() { }

        public UIElement(string Name, Vector2 Position, Vector2 Size, UIElement Parent = null)
        {
            m_Name = Name;
            m_Mat = Matrix.CreateTranslation(Size.X, Size.Y, 0); //TODO: What happens if this element is a child?
            m_Size = Size;

            if (Parent != null)
            {
                m_Parent = Parent;
                m_Position = Vector2.Transform(new Vector2(Position.X, Position.Y), m_Parent.PositionMatrix);
            }
            else
                m_Position = Position;
        }

        public UIElement(UIElement Parent)
        {
            m_Parent = Parent;
        }

        /// <summary>
        /// Sets the position for this UIElement.
        /// </summary>
        /// <param name="X">The X position to set.</param>
        /// <param name="Y">The Y position to set.</param>
        public void SetPosition(int X, int Y)
        {
            m_Position.X = X;
            m_Position.Y = Y;
            Vector2 NewPosition = Vector2.Transform(new Vector2(X, Y), m_Mat);

            foreach (KeyValuePair<string, UIElement> KVP in m_Elements)
                m_Elements[KVP.Key].SetPosition((int)NewPosition.X, (int)NewPosition.Y);
        }

        /// <summary>
        /// Sets the size for this UIElement.
        /// </summary>
        /// <param name="Width">The width of this UIElement.</param>
        /// <param name="Height">THe height of this UIElement.</param>
        public void SetSize(int Width, int Height)
        {
            m_Size.X = Width;
            m_Size.Y = Height;

            //TODO: Scale children...
        }

        /// <summary>
        /// Gets a child from this UIElements Dictionary of children.
        /// </summary>
        /// <param name="Name">The name of the child to get.</param>
        /// <returns>A UIElement instance that can be cast.</returns>
        public UIElement GetChild(string Name)
        {
            try
            {
                return m_Elements[Name];
            }
            catch (Exception)
            {
                throw new Exception("Couldn't find child: " + Name + " in UIElement.cs");
            }
        }

        public string GetString(string Name)
        {
            try
            {
                return m_Strings[Name];
            }
            catch (Exception)
            {
                throw new Exception("Couldn't find string: " + Name + " in UIElement.cs");
            }
        }

        uint[] PixelData = new uint[1]; // Delare an Array of 1 just to store data for one pixel
        protected bool PixelCheck(InputHelper Input)
        {
            // Get Mouse position relative to top left of Texture
            Vector2 pixelPosition = Input.MousePosition - m_Position;

            // I know. I just checked this condition at OnMouseOver or we wouldn't be here
            // but just to be sure the spot we're checking is within the bounds of the texture...
            if (pixelPosition.X >= 0 && pixelPosition.X < Image.Width &&
                pixelPosition.Y >= 0 && pixelPosition.Y < Image.Height)
            {
                // Get the Texture Data within the Rectangle coords, in this case a 1 X 1 rectangle
                // Store the data in pixelData Array
                Image.GetData<uint>(0, new Rectangle((int)pixelPosition.X, (int)pixelPosition.Y, (1), (1)), PixelData, 0, 1);

                // Check if pixel in Array is non Alpha, give or take 20
                if (((PixelData[0] & 0xFF000000) >> 24) > 20)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the mouse is over this UIElement.
        /// Does NOT work for pixel-perfect collision.
        /// </summary>
        /// <param name="State">State of mouse.</param>
        /// <returns>True if mouse is over, false otherwise.</returns>
        public virtual bool IsMouseOver(InputHelper Input)
        {
            if (m_Size != null)
            {
                if (Input.MousePosition.X > m_Position.X && Input.MousePosition.X <= (m_Position.X + m_Size.X))
                {
                    if (Input.MousePosition.Y > m_Position.Y && Input.MousePosition.Y <= (m_Position.Y + m_Size.Y))
                        return true;
                }
            }
            else
            {
                if (Input.MousePosition.X > m_Position.X && Input.MousePosition.X <= (m_Position.X + Image.Width))
                {
                    if (Input.MousePosition.Y > m_Position.Y && Input.MousePosition.Y <= (m_Position.Y + Image.Height))
                        return true;
                }
            }

            return false;
        }
    }
}
