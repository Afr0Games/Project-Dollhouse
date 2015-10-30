using System;
using System.Collections.Generic;
using System.Text;
using Files.Manager;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gonzo
{
    /// <summary>
    /// A clickable button that can trigger an event.
    /// A button is always graphically represented by four equally sized frames in a texture.
    /// </summary>
    public class UIButton : UIElement
    {
        private Vector2 m_SourcePosition;
        private bool m_IsMouseHovering = false;
        private bool m_IsTextHighlighted = false;
        private int m_FontSize;
        private bool m_IsTextButton = true;
        private string m_Text;

        public UIButton(AddButtonNode Node, ParserState State, UIElement Parent) : base(Parent)
        {
            m_Name = Node.Name;
            m_ID = Node.ID;

            m_Position = new Vector2();
            m_Position = Vector2.Transform(new Vector2(Node.ButtonPosition.Numbers[0], Node.ButtonPosition.Numbers[1]), 
                m_Parent.PositionMatrix);

            if (Node.ButtonSize != null)
            {
                m_Size = new Vector2();
                m_Size.X = Node.ButtonSize.Numbers[0];
                m_Size.Y = Node.ButtonSize.Numbers[1];

                m_SourcePosition = new Vector2(0.0f, 0.0f);
            }

            if (Node.TextHighlighted != null)
                m_IsTextHighlighted = (Node.TextHighlighted == 1) ? true : false;

            if (Node.Font != null)
                m_FontSize = (int)Node.Font;

            if (Node.TextColor != null)
            {
                TextColor = new Color();
                TextColor.R = (byte)Node.TextColor.Numbers[0];
                TextColor.G = (byte)Node.TextColor.Numbers[1];
                TextColor.B = (byte)Node.TextColor.Numbers[2];
            }

            if (Node.TextColorSelected != null)
            {
                TextColor = new Color();
                TextColor.R = (byte)Node.TextColorSelected.Numbers[0];
                TextColor.G = (byte)Node.TextColorSelected.Numbers[1];
                TextColor.B = (byte)Node.TextColorSelected.Numbers[2];
            }

            if (Node.TextColorHighlighted != null)
            {
                TextColor = new Color();
                TextColor.R = (byte)Node.TextColorHighlighted.Numbers[0];
                TextColor.G = (byte)Node.TextColorHighlighted.Numbers[1];
                TextColor.B = (byte)Node.TextColorHighlighted.Numbers[2];
            }

            if (Node.TextColorDisabled != null)
            {
                TextColor = new Color();
                TextColor.R = (byte)Node.TextColorDisabled.Numbers[0];
                TextColor.G = (byte)Node.TextColorDisabled.Numbers[1];
                TextColor.B = (byte)Node.TextColorDisabled.Numbers[2];
            }

            if(Node.TextButton != null)
                m_IsTextButton = (Node.TextButton == 1) ? true : false;

            if(m_IsTextButton)
                m_Text = m_Parent.GetString(State.Tooltip);

            if (Node.Image != null)
            {
                UIImage Img = (UIImage)m_Parent.GetChild(Node.Image);
                Image = Img.Image;
                m_SourcePosition = new Vector2(0.0f, 0.0f);
            }

            if (Node.Tooltip != "")
                Tooltip = m_Parent.GetString(Node.Tooltip);

            if (Node.Tracking != null)
                Tracking = (int)Node.Tracking;

            if(State.InSharedPropertiesGroup)
            {
                if(State.Image != "")
                {
                    UIImage Img = (UIImage)m_Parent.GetChild(State.Image);
                    Image = Img.Image;
                }

                if (State.Tooltip != "")
                    Tooltip = m_Parent.GetString(State.Tooltip);
            }
        }

        public override bool IsMouseOver(InputHelper Input)
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
                if (Input.MousePosition.X > m_Position.X && Input.MousePosition.X <= (m_Position.X + (Image.Width / 4)))
                {
                    if (Input.MousePosition.Y > m_Position.Y && Input.MousePosition.Y <= (m_Position.Y + Image.Height))
                        return true;
                }
            }

            return false;
        }

        public override void Update(InputHelper Input)
        {
            if(IsMouseOver(Input) || PixelCheck(Input))
            {
                if (!m_IsMouseHovering)
                {
                    m_SourcePosition.X += Image.Width;
                    m_IsMouseHovering = true;
                }
            }
            else
            {
                m_SourcePosition.X = 0;
                m_IsMouseHovering = false;
            }
        }

        public override void Draw(SpriteBatch SBatch)
        {
            if (Image != null)
            {
                //TODO: Fix color mask.
                SBatch.Draw(Image, new Rectangle((int)m_Position.X, (int)m_Position.Y, Image.Width, 
                    Image.Height), new Rectangle((int)m_SourcePosition.X, (int)m_SourcePosition.Y, 
                    Image.Width / 4, Image.Height), Color.White);
            }
        }
    }
}
