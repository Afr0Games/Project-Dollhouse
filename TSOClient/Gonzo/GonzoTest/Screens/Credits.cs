using System.Collections.Generic;
using Gonzo;
using Gonzo.Dialogs;
using Gonzo.Elements;
using Files.Manager;
using Files.IFF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Glide;
using Sound;

namespace GonzoTest
{
    public class CreditsScreen : UIScreen
    {
        private UIImage BackgroundImg, TSOLogoImage, BackButtonIndentImage, WillImage;
        private UIButton MaxisButton;

        private Iff m_Credits;
        private List<UILabel> m_CreditsStrings = new List<UILabel>();
        private UIControl m_CreditsArea;
        private float m_CreditsY = 0;       //Upwards position of credits text.
        private float m_CreditsCenterX = 0; //Center of credits area.
        private Tweener m_Tween;

        private WillWrightDiag m_WillWrightDiag;

        public CreditsScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "Credits", SBatch, 
            new Vector2(0, 0), new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight), 
            GlobalSettings.Default.StartupPath + "\\" + "gamedata\\uiscripts\\credits.uis")
        {
            BackgroundImg = (UIImage)m_PResult.Elements["\"BackgroundImage\""];
            TSOLogoImage = m_PResult.Controls["\"TSOLogoImage\""].Image;
            BackButtonIndentImage = m_PResult.Controls["\"BackButtonIndentImage\""].Image;
            WillImage = (UIImage)m_PResult.Elements["\"WillImage\""];

            MaxisButton = (UIButton)m_PResult.Elements["\"MaxisButton\""];
            MaxisButton.OnButtonClicked += MaxisButton_OnButtonClicked;

            m_WillWrightDiag = new WillWrightDiag(WillImage, this, new Vector2(100, 100));
            m_WillWrightDiag.Visible = false;
            //m_PResult.Elements.Add("WillWrightDiag", m_WillWrightDiag);

            m_Credits = FileManager.GetIFF("credits.iff");
            m_CreditsArea = (UIControl)m_PResult.Controls["\"CreditsArea\""];
            m_CreditsY = m_CreditsArea.Size.Y;

            int StrID = 0;
            float Separation = 1.0f;

            foreach(TranslatedString TStr in m_Credits.GetSTR(163).GetStringList(LanguageCodes.EngUS))
            {
                foreach (string Str in TStr.TranslatedStr.Split('\n'))
                {
                    m_CreditsStrings.Add(new UILabel(Str, StrID++, new Vector2(m_CreditsArea.Position.X +
                        m_CreditsCenterX, m_CreditsY + Separation), Manager.Font12px.MeasureString(Str), 
                        Color.Wheat, 12, this));
                    Separation += 15.0f;
                }
            }

            m_Tween = new Tweener();
            Separation = 0.0f - (m_CreditsStrings.Count * 15.0f);

            foreach (UILabel Lbl in m_CreditsStrings)
            {
                m_Tween.Tween(Lbl, new { YPosition = Separation }, 1000);
                Separation += 15.0f;
            }

            HitVM.PlayEvent("bkground_credits");
        }

        #region EventHandlers


        private void MaxisButton_OnButtonClicked(object Sender)
        {
            m_WillWrightDiag.Visible = true;
        }

        #endregion

        public override void Update(InputHelper Input, GameTime GTime)
        {
            base.Update(Input, GTime);

            m_WillWrightDiag.Update(Input, GTime);
            m_Tween.Update(0.3f); //Set this to a lower value if text scrolls by too fast.
        }

        public override void Draw()
        {
            BackgroundImg.Draw(m_SBatch, null, 0.0f);
            TSOLogoImage.Draw(m_SBatch, null, 0.0f);
            BackButtonIndentImage.Draw(m_SBatch, null, 0.0f);

            foreach (UILabel Lbl in m_CreditsStrings)
            {
                //TODO: Figure out how to stop lines from displaying outside of the screen.
                if(Lbl.YPosition > m_CreditsArea.Position.Y && (Lbl.YPosition < m_CreditsArea.Size.Y))
                    Lbl.Draw(m_SBatch, 0.3f);
            }

            m_WillWrightDiag.Draw(m_SBatch, 0.4f);

            base.Draw();
        }
    }
}
