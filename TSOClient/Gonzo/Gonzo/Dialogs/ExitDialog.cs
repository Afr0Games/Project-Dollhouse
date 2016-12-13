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
            Walker.Initialize(ScriptPath);
            m_Elements = Walker.Elements;
            m_Controls = Walker.Controls;

            m_ReloginButton = (UIButton)m_Elements["ReLoginButton"];
            m_ExitButton = (UIButton)m_Elements["ExitButton"];
            m_CancelButton = (UIButton)m_Elements["CancelButton"];

            m_TitleText = (UILabel)m_Elements["TitleText"];
            m_MessageText = (UILabel)m_Elements["MessageText"];
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
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
