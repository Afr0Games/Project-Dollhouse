using System;
using Microsoft.Xna.Framework;
using Gonzo.Elements;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Dialogs
{
    public class LoginDialog : UIDialog
    {
        private UILabel m_LblTitle, m_LblUsername, m_LblPassword;
        private UITextEdit m_TxtUsername, m_TxtPassword;
        private CaretSeparatedText m_Cst;

        public LoginDialog(UIScreen Screen, Vector2 Pos) : base(Screen, Pos, false, true, false)
        {
            m_Font = m_Screen.Font11px;

            m_Cst = StringManager.StrTable(209);
            m_LblTitle = new UILabel(m_Cst[1], 1, Pos, m_Font.MeasureString(m_Cst[1]),
                m_Screen.StandardTxtColor, 11, m_Screen, UIParser.Nodes.TextAlignment.Center_Center);
            m_LblUsername = new UILabel(m_Cst[4], 2, new Vector2(Pos.X + 20, Pos.Y - 50), m_Font.MeasureString(m_Cst[4]), 
                m_Screen.StandardTxtColor, 11, m_Screen, UIParser.Nodes.TextAlignment.Center_Center);
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            base.Update(Helper, GTime);

            if(Visible)
            {
                if(m_DoDrag)
                {
                    Vector2 OffsetFromMouse = Position;
                    m_LblTitle.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                    OffsetFromMouse = new Vector2(Position.X + 20, Position.Y - 50);
                    m_LblUsername.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                }
            }
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.10f;

            m_LblTitle.Draw(SBatch, Depth + 0.1f);
            m_LblUsername.Draw(SBatch, Depth + 0.1f);
            //m_LblPassword.Draw(SBatch, Depth + 0.1f);

            base.Draw(SBatch, LayerDepth);
        }
    }
}
