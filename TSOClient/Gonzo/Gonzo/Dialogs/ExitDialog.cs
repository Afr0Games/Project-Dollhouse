using System.Collections.Generic;
using Gonzo.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Dialogs
{
    public class ExitDialog : UIDialog
    {
        protected List<CaretSeparatedText> m_StringTables = new List<CaretSeparatedText>();
        protected Dictionary<string, string> m_Strings = new Dictionary<string, string>();

        private UIButton m_ReloginButton, m_ExitButton, m_CancelButton;
        private UILabel m_TitleText, m_MessageText; 

        /// <summary>
        /// Constructs a new ExitDialog instance.
        /// </summary>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Position">The position of this ExitDialog.</param>
        /// <param name="Walker">A TreeWalker instance.</param>
        /// <param name="ScriptPath">The path to the script which defines this ExitDialog.</param>
        public ExitDialog(UIScreen Screen, Vector2 Position, TreeWalker Walker, string ScriptPath) : 
            base(Screen, Position, true, true, true)
        {
            ParseResult Result = new ParseResult();

            Walker.Initialize(ScriptPath, ref Result);

            Vector2 RelativePosition = new Vector2(30, 120);
            m_ReloginButton = (UIButton)Result.Elements["\"ReLoginButton\""];
            m_ReloginButton.Position = Position;
            m_ReloginButton.Position += RelativePosition;
            m_ReloginButton.DrawOrder = (int)DrawOrderEnum.UI;
            m_ReloginButton.AddParent(this);

            RelativePosition = new Vector2(160, 120);
            m_ExitButton = (UIButton)Result.Elements["\"ExitButton\""];
            m_ExitButton.Position = Position;
            m_ExitButton.Position += RelativePosition;
            m_ExitButton.DrawOrder = (int)DrawOrderEnum.UI;
            m_ExitButton.AddParent(this);

            RelativePosition = new Vector2(350, 120);
            m_CancelButton = (UIButton)Result.Elements["\"CancelButton\""];
            m_CancelButton.Position = Position;
            m_CancelButton.Position += RelativePosition;
            m_CancelButton.DrawOrder = (int)DrawOrderEnum.UI;
            m_ExitButton.AddParent(this);

            RelativePosition = new Vector2(175, Position.Y);
            m_TitleText = (UILabel)Result.Elements["\"TitleText\""];
            m_TitleText.Position += RelativePosition;
            m_TitleText.AddParent(this);

            m_MessageText = (UILabel)Result.Elements["\"MessageText\""];
            m_MessageText.Position += Position;
            m_MessageText.AddParent(this);

            m_Font = m_Screen.Manager.Font9px; //Needs to be set for debug purposes.

            UIControl DialogSize = Result.Controls["\"DialogSize\""];

            /*if (Size.X != 0 && Size.Y != 0)
            {
                SetSize((Size.X) * m_Screen.Manager.Resolution.ScalingRatio,
                    (Size.Y) * m_Screen.Manager.Resolution.ScalingRatio);
            }
            else
                SetSize((DialogSize.Size.X * m_Screen.Manager.Resolution.ScalingRatio), 
                    (DialogSize.Size.Y) * m_Screen.Manager.Resolution.ScalingRatio);*/
            SetSize((m_Screen.Font10px.MeasureString(m_TitleText.Caption).X + 40), 
                m_DefaultSize.Y);
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            base.Update(Helper, GTime);

            if (Visible)
            {
                m_MessageText.Update(Helper, GTime);
                m_TitleText.Update(Helper, GTime);

                m_ReloginButton.Update(Helper, GTime);
                m_ExitButton.Update(Helper, GTime);
                m_CancelButton.Update(Helper, GTime);

                if(m_DoDrag)
                {
                    Vector2 OffsetFromMouse = new Vector2(30, 120);
                    m_ReloginButton.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                    OffsetFromMouse = new Vector2(160, 120);
                    m_ExitButton.Position = (Helper.MousePosition + OffsetFromMouse)  - m_DragOffset;
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
                    m_ReloginButton.Draw(SBatch, (float)(Depth + 0.1));
                    m_ExitButton.Draw(SBatch, (float)(Depth + 0.1));
                    m_CancelButton.Draw(SBatch, (float)(Depth + 0.1));

                    m_TitleText.Draw(SBatch, (float)(Depth + 0.1));
                    m_MessageText.Draw(SBatch, (float)(Depth + 0.1));
                }

                base.Draw(SBatch, LayerDepth);
            }
        }
    }
}
