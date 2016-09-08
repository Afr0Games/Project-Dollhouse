using System;
using System.Collections.Generic;
using System.Text;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Elements
{
    public class UILabel : UIElement
    {
        public string Caption = "";
        private Color m_TextColor;
        private TextAlignment m_Alignment;

        public UILabel(AddTextNode Node, ParserState State, UIScreen Screen) : base(Screen)
        {
            m_Name = Node.Name;
            m_ID = Node.ID;
            Position = new Vector2(Node.TextPosition.Numbers[0], Node.TextPosition.Numbers[1]) + Screen.Position;

            if (Node.Size != null)
                m_Size = new Vector2(Node.Size.Numbers[0], Node.Size.Numbers[1]);
            else
                m_Size = State.Size;

            if (Node.Color != null)
            {
                m_TextColor = new Color();
                m_TextColor.A = 255; //Ignore opacity, The Sims Online doesn't support transparent text.
                m_TextColor.R = (byte)Node.Color.Numbers[0];
                m_TextColor.G = (byte)Node.Color.Numbers[1];
                m_TextColor.B = (byte)Node.Color.Numbers[2];
            }
            else
            {
                m_TextColor = State.Color;
                m_TextColor.A = 255; //Ignore opacity, The Sims Online doesn't support transparent text.
            }

            if (Node.Alignment != 0)
                m_Alignment = Node.Alignment;
            else
                m_Alignment = (TextAlignment)State.Alignment;

            int Font = 0;
            if (Node.Font != null)
            {
                if (Node.Font != 0)
                    Font = (int)Node.Font;
            }
            else
                Font = State.Font;

            switch (Font)
            {
                    case 7:
                        m_Font = Screen.Font10px; //TODO: Fixme.
                    break;
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

            if (Node.Text != "")
                Caption = m_Screen.GetString(Node.Text);
            else
            {
                if(State.Caption != "") 
                    //Sometimes labels will not have pre-defined text, as they will hold text
                    //generated in-game.
                    Caption = m_Screen.GetString(State.Caption);
            }

            AlignText();
        }

        private void AlignText()
        {
            Vector2 Measurement = m_Font.MeasureString(Caption);
            Vector2 LocalCopy = Position;

            switch (m_Alignment)
            {
                case TextAlignment.Left_Top:
                    //Is there a need to modify position at all here??
                    break;
                case TextAlignment.Left_Center:
                    LocalCopy.Y += (m_Size.Y / 2) - (Measurement.Y / 2);
                    Position = LocalCopy;
                    break;
                case TextAlignment.Center_Top:
                    LocalCopy.X += (m_Size.X / 2) - (Measurement.X / 2);
                    Position = LocalCopy;
                    break;
                case TextAlignment.Center_Center:
                    LocalCopy.X += (m_Size.X / 2) - (Measurement.X / 2);
                    LocalCopy.Y += (m_Size.Y / 2) - (Measurement.Y / 2);
                    Position = LocalCopy;
                    break;
                case TextAlignment.Right_Top:
                    LocalCopy.X += m_Size.X - Measurement.X;
                    Position = LocalCopy;
                    break;
                case TextAlignment.Right_Center:
                    LocalCopy.X += m_Size.X - Measurement.X;
                    LocalCopy.Y += (m_Size.Y / 2) - (Measurement.Y / 2);
                    Position = LocalCopy;
                    break;
            }
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 1.0f;

            if (Caption != "")
            {
                if (Visible)
                {
                    SBatch.DrawString(m_Font, Caption, Position, m_TextColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f,
                        SpriteEffects.None, Depth);
                }
            }
        }
    }
}
