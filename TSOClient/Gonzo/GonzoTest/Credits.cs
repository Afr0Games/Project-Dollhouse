using System;
using System.Collections.Generic;
using System.Text;
using Gonzo;
using Gonzo.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GonzoTest
{
public class CreditsScreen : UIScreen
    {
        private UIImage BackgroundImg, TSOLogoImage, BackButtonIndentImage;
        private UIButton MaxisButton;

        private bool m_DrawDiag = false;
        private UIDialog m_WillWrightDiag;

        public CreditsScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "Credits", SBatch, 
            new Vector2(0, 0), new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight), 
            GlobalSettings.Default.StartupPath + "\\" + "gamedata\\uiscripts\\credits.uis")
        {
            BackgroundImg = (UIImage)m_Elements["\"BackgroundImage\""];
            TSOLogoImage = m_Controls["\"TSOLogoImage\""].Image;
            BackButtonIndentImage = m_Controls["\"BackButtonIndentImage\""].Image;

            MaxisButton = (UIButton)m_Elements["\"MaxisButton\""];
            MaxisButton.OnButtonClicked += MaxisButton_OnButtonClicked;

            m_WillWrightDiag = new UIDialog(this, new Vector2(100, 100), true, true, true);
        }

        private void MaxisButton_OnButtonClicked(UIButton ClickedButton)
        {
            m_DrawDiag = true;
        }

        public override void Update(InputHelper Input)
        {
            m_WillWrightDiag.Update(Input);

            base.Update(Input);
        }

        public override void Draw()
        {
            BackgroundImg.Draw(m_SBatch, null);
            TSOLogoImage.Draw(m_SBatch, null);
            BackButtonIndentImage.Draw(m_SBatch, null);

            if (m_DrawDiag)
                m_WillWrightDiag.Draw(m_SBatch);

            base.Draw();
        }
    }
}
