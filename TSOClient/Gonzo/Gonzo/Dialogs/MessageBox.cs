using System.Collections.Generic;
using Gonzo.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Dialogs
{
    public class MessageBox : UIDialog
    {
        private Dictionary<string, UIElement> m_Elements = new Dictionary<string, UIElement>();
        private Dictionary<string, UIControl> m_Controls = new Dictionary<string, UIControl>();

        protected List<CaretSeparatedText> m_StringTables = new List<CaretSeparatedText>();
        protected Dictionary<string, string> m_Strings = new Dictionary<string, string>();

        private UIButton m_OKButton, m_CancelButton;
        private UILabel m_TitleText, m_MessageText;

        /// <summary>
        /// Constructs a new MessageBox instance.
        /// </summary>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Position">The position of this MessageBox.</param>
        public MessageBox(UIScreen Screen, Vector2 Position, string Message = "", string Title = "") :
            base(Screen, Position, true, true, true)
        {
            Vector2 RelativePosition = new Vector2(160, 120);
            m_OKButton = new UIButton("OKButton", Position + RelativePosition, Screen, null, "OK", 9, true, this);
            m_OKButton.Position = Position;
            m_OKButton.Position += RelativePosition;
            m_OKButton.AddParent(this);

            m_Font = m_Screen.Manager.Font9px; //Needs to be set for debug purposes.

            RelativePosition = new Vector2(175, Position.Y);
            m_TitleText = new UILabel(Title, 1, Position + RelativePosition, m_Font.MeasureString(Title), 
                Color.Wheat, 9, Screen, this);
            m_TitleText.Position += RelativePosition;
            m_TitleText.AddParent(this);

            RelativePosition = new Vector2(((Size.X / 2) - (m_Font.MeasureString(Message).X / 2)), Size.Y / 2);
            m_MessageText = new UILabel(Message, 1, Position + RelativePosition, m_Font.MeasureString(Message),
                Color.Wheat, 9, Screen, this);
            m_MessageText.Position += Position + RelativePosition;
            m_MessageText.AddParent(this);

            SetSize((m_Screen.Font10px.MeasureString(m_TitleText.Caption).X + 40), 
                m_DefaultSize.Y);
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            base.Update(Helper, GTime);

            if (Visible)
            {
                m_OKButton.Update(Helper, GTime);
                m_CancelButton.Update(Helper, GTime);

                if (m_DoDrag)
                {
                    Vector2 OffsetFromMouse = new Vector2(160, 120);
                    m_OKButton.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                    OffsetFromMouse = new Vector2(350, 120);
                    m_CancelButton.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;

                    OffsetFromMouse = new Vector2(60, 6);
                    m_TitleText.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                    OffsetFromMouse = new Vector2(60, 48);
                    m_MessageText.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                }
            }
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            if (Visible)
            {
                float Depth;
                if (LayerDepth != null)
                    Depth = (float)LayerDepth;
                else
                    Depth = 0.10f;

                if (Visible)
                {
                    m_OKButton.Draw(SBatch, (float)(Depth + 0.1));
                    m_CancelButton.Draw(SBatch, (float)(Depth + 0.1));

                    m_TitleText.Draw(SBatch, (float)(Depth + 0.1));
                    m_MessageText.Draw(SBatch, (float)(Depth + 0.1));
                }

                base.Draw(SBatch, LayerDepth);
            }
        }
    }
}
