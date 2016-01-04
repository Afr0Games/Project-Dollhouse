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
            Position *= m_Screen.Scale;
            m_Size = new Vector2(Node.Size.Numbers[0], Node.Size.Numbers[1]) * m_Screen.Scale;
            m_TextColor = Color.FromNonPremultiplied(Node.Color.Numbers[0], Node.Color.Numbers[1], 
                Node.Color.Numbers[2], 255);
            m_Alignment = Node.Alignment;

            if (Node.Font != null)
            {
                switch (Node.Font)
                {
                    case 7:
                        m_Font = Screen.Font10px; //TODO Fixme.
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
            }
            else
                m_Font = Screen.Font12px;

            if (Node.Text != "")
                Caption = m_Screen.GetString(Node.Text);
            else
                Caption = State.Caption;

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

        public override void Draw(SpriteBatch SBatch)
        {
            if(Caption != "")
                SBatch.DrawString(m_Font, Caption, Position, m_TextColor, 0.0f, new Vector2(0.0f, 0.0f), 
                    m_Screen.Scale, SpriteEffects.None, 0.0f);
        }
    }
}
