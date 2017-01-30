using System.Collections.Generic;
using Gonzo.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Dialogs
{
    public class ExitDialog : UIDialog
    {
        private Dictionary<string, UIElement> m_Elements = new Dictionary<string, UIElement>();
        private Dictionary<string, UIControl> m_Controls = new Dictionary<string, UIControl>();

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

            m_ReloginButton = (UIButton)Result.Elements["\"ReLoginButton\""];
            if (Resolution.getVirtualAspectRatio() > 1.3) //Only adjust if we're above 800x600
            {
                m_ReloginButton.Position = Position;
                m_ReloginButton.Position += new Vector2(30, 120);
            }
            m_ExitButton = (UIButton)Result.Elements["\"ExitButton\""];
            if (Resolution.getVirtualAspectRatio() > 1.3)
            {
                m_ExitButton.Position = Position;
                m_ExitButton.Position += new Vector2(160, 120);
            }
            m_CancelButton = (UIButton)Result.Elements["\"CancelButton\""];
            if (Resolution.getVirtualAspectRatio() > 1.3)
            {
                m_CancelButton.Position = Position;
                m_CancelButton.Position += new Vector2(350, 120);
            }

            m_TitleText = (UILabel)Result.Elements["\"TitleText\""];
            if (Resolution.getVirtualAspectRatio() > 1.3)
            {
                m_TitleText.Position += Position;
            }
            m_MessageText = (UILabel)Result.Elements["\"MessageText\""];
            if (Resolution.getVirtualAspectRatio() > 1.3)
            {
                m_MessageText.Position += Position;
            }

            UIControl DialogSize = Result.Controls["\"DialogSize\""];

            if (Size.X != 0 && Size.Y != 0)
                SetSize((int)(Size.X * Resolution.getVirtualAspectRatio()), (int)(Size.Y * Resolution.getVirtualAspectRatio()));
            else
                SetSize((int)(DialogSize.Size.X * Resolution.getVirtualAspectRatio()), (int)(DialogSize.Size.Y * Resolution.getVirtualAspectRatio()));
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            base.Update(Helper, GTime);

            if (IsDrawn)
            {
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

                if (IsDrawn)
                {
                    m_ReloginButton.Draw(SBatch, (float)(LayerDepth + 0.1));
                    m_ExitButton.Draw(SBatch, (float)(LayerDepth + 0.1));
                    m_CancelButton.Draw(SBatch, (float)(LayerDepth + 0.1));

                    m_TitleText.Draw(SBatch, (float)(LayerDepth + 0.1));
                    m_MessageText.Draw(SBatch, (float)(LayerDepth + 0.1));
                }

                base.Draw(SBatch, LayerDepth);
            }
        }
    }
}
