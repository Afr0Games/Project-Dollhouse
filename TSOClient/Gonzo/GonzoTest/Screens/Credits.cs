
using System.Timers;
using System.Collections.Generic;
using Gonzo;
using Gonzo.Dialogs;
using Gonzo.Elements;
using Files.Manager;
using Files.IFF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GonzoTest
{
    public class CreditsScreen : UIScreen
    {
        private UIImage BackgroundImg, TSOLogoImage, BackButtonIndentImage, WillImage;
        private UIButton MaxisButton;

        private Iff m_Credits;
        private List<string> m_CreditsStrings = new List<string>();
        private UIControl m_CreditsArea;
        private float m_CreditsY = 0;       //Upwards position of credits text.
        private float m_CreditsCenterX = 0; //Center of credits area.
        private Timer m_CreditsTimer;       //Timer for controlling text scroll.

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
            m_PResult.Elements.Add("WillWrightDiag", m_WillWrightDiag);

            m_Credits = FileManager.GetIFF("credits.iff");
            m_CreditsArea = (UIControl)m_PResult.Controls["\"CreditsArea\""];
            m_CreditsY = m_CreditsArea.Size.Y;

            //TODO: Convert all strings to UILabels and use Glide to scroll them.
            foreach(TranslatedString TStr in m_Credits.GetSTR(163).GetStringList(LanguageCodes.EngUS))
            {
                foreach (string Str in TStr.TranslatedStr.Split('\n'))
                    m_CreditsStrings.Add(Str);
            }

            m_CreditsTimer = new Timer(300);
            m_CreditsTimer.Elapsed += M_CreditsTimer_Elapsed;
            m_CreditsTimer.Start();
        }

        #region EventHandlers

        private void M_CreditsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_CreditsY -= 3.5f;
        }

        private void MaxisButton_OnButtonClicked(object Sender)
        {
            m_WillWrightDiag.Visible = true;
        }

        #endregion

        public override void Update(InputHelper Input, GameTime GTime)
        {
            m_WillWrightDiag.Update(Input, GTime);

            base.Update(Input, GTime);
        }

        public override void Draw()
        {
            BackgroundImg.Draw(m_SBatch, null, 0.0f);
            TSOLogoImage.Draw(m_SBatch, null, 0.0f);
            BackButtonIndentImage.Draw(m_SBatch, null, 0.0f);

            float Separation = 1.0f;

            foreach (string Str in m_CreditsStrings)
            {
                m_CreditsCenterX = (m_CreditsArea.Size.X / 2) - (Manager.Font12px.MeasureString(Str).X / 2);

                if ((m_CreditsY + Separation) > m_CreditsArea.Position.Y && (m_CreditsY + Separation) < m_CreditsArea.Size.Y)
                {
                    m_SBatch.DrawString(Manager.Font12px, Str, new Vector2(m_CreditsArea.Position.X + 
                        m_CreditsCenterX, m_CreditsY + Separation), Color.Wheat);
                }

                Separation += 15.0f;
            }

            base.Draw();
        }
    }
}
