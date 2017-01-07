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
            m_ReloginButton.Position += Position;
            m_ExitButton = (UIButton)Result.Elements["\"ExitButton\""];
            m_ExitButton.Position += Position;
            m_CancelButton = (UIButton)Result.Elements["\"CancelButton\""];
            m_CancelButton.Position += Position;

            m_TitleText = (UILabel)Result.Elements["\"TitleText\""];
            m_TitleText.Position += Position;
            m_MessageText = (UILabel)Result.Elements["\"MessageText\""];
            m_MessageText.Position += Position;

            UIControl DialogSize = Result.Controls["\"DialogSize\""];

            if (Size.X != 0 && Size.Y != 0)
                SetSize((int)(Size.X * Resolution.getVirtualAspectRatio()), (int)(Size.Y * Resolution.getVirtualAspectRatio()));
            else
                SetSize((int)(DialogSize.Size.X * Resolution.getVirtualAspectRatio()), (int)(DialogSize.Size.Y * Resolution.getVirtualAspectRatio()));
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            if (IsDrawn)
            {
                m_ReloginButton.Update(Helper, GTime);
                m_ExitButton.Update(Helper, GTime);
                m_CancelButton.Update(Helper, GTime);

                if(m_DoDrag)
                {
                    m_ReloginButton.Position = (Position - new Vector2(-30, -120)) * Resolution.getVirtualAspectRatio();
                    m_ExitButton.Position = (Position - new Vector2(-140, -120))  * Resolution.getVirtualAspectRatio();
                    m_CancelButton.Position = (Position - new Vector2(-251, -120)) * Resolution.getVirtualAspectRatio();

                    m_TitleText.Position = ((Position - new Vector2(-10, -6)) * Resolution.getVirtualAspectRatio());
                    m_MessageText.Position = ((Position - new Vector2(-10, -48)) * Resolution.getVirtualAspectRatio());
                }
            }

            base.Update(Helper, GTime);
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
