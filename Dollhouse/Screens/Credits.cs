using System.Collections.Generic;
using UI;
using UI.Dialogs;
using UI.Elements;
using Files.Manager;
using Files.IFF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Glide;
using Sound;
using ResolutionBuddy;

namespace GonzoTest
{
    public class CreditsScreen : UIScreen
    {
        //All variables that exists in the script shares their name between the script and the code!
        private UIBackgroundImage BackgroundImg;
        private UIImage TSOLogoImage, BackButtonIndentImage, WillImage;
        private UIButton MaxisButton, BackButton, ExitButton;

        private Iff m_Credits;
        private List<UILabel> m_CreditsStrings = new List<UILabel>();
        private UILabel m_TitleLabel;
        private UIControl m_CreditsArea;
        private float m_CreditsY = 0;       //Upwards position of credits text.
        private float m_CreditsCenterX = 0; //Center of credits area.
        private Tweener m_Tween;

        private WillWrightDiag m_WillWrightDiag;

        public CreditsScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "Credits", SBatch, 
            new Vector2(0, 0), new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight), 
            GlobalSettings.Default.StartupPath + "\\" + "gamedata\\uiscripts\\credits.uis")
        {
            BackgroundImg = (UIBackgroundImage)m_PResult.Elements["\"BackgroundImage\""];
            Children.Add(BackgroundImg);
            
            TSOLogoImage = m_PResult.Controls["\"TSOLogoImage\""].Image;

            Children.Add(TSOLogoImage);
            BackButtonIndentImage = m_PResult.Controls["\"BackButtonIndentImage\""].Image;
            Children.Add(BackButtonIndentImage);

            m_TitleLabel = (UILabel)m_PResult.Elements["\"TitleLabel\""];
            Children.Add(m_TitleLabel); ;

            WillImage = (UIImage)m_PResult.Elements["\"WillImage\""];
            WillImage.Position = new Vector2(/*22*/0, /*42*/0);

            MaxisButton = (UIButton)m_PResult.Elements["\"MaxisButton\""];
            MaxisButton.OnButtonClicked += MaxisButton_OnButtonClicked;
            Children.Add(MaxisButton);

            BackButton = (UIButton)m_PResult.Elements["\"BackButton\""];
            BackButton.ZIndex = (int)DrawOrderEnum.UI;
            Children.Add(BackButton);

            ExitButton = (UIButton)m_PResult.Elements["\"ExitButton\""];
            ExitButton.ZIndex = (int)DrawOrderEnum.UI;
            Children.Add(ExitButton);

            m_WillWrightDiag = new WillWrightDiag(WillImage, this, new Vector2(100, 100));
            m_WillWrightDiag.Visible = false;
            Children.Add(m_WillWrightDiag);

            m_Credits = FileManager.Instance.GetIFF("credits.iff");
            m_CreditsArea = (UIControl)m_PResult.Controls["\"CreditsArea\""];
            m_CreditsY = m_CreditsArea.Size.Y;
            Children.Add(m_CreditsArea);

            int StrID = 0;
            float Separation = 1.0f;

            foreach(TranslatedString TStr in m_Credits.GetSTR(163).GetStringList(LanguageCodes.EngUS))
            {
                foreach (string Str in TStr.TranslatedStr.Split('\n'))
                {
                    UILabel CStrLabel = new UILabel(Str, StrID++, new Vector2(m_CreditsArea.Position.X +
                        m_CreditsCenterX, m_CreditsY + Separation), Manager.Font12px.MeasureString(Str),
                        Color.Wheat, 12, this)
                    { Visible = false, ZIndex = (int)DrawOrderEnum.Game };
                    m_CreditsStrings.Add(CStrLabel);
                    Separation += 15.0f;
                    Children.Add(CStrLabel);
                }
            }

            m_Tween = new Tweener();
            Separation = 0.0f - (m_CreditsStrings.Count * 15.0f);

            foreach (UILabel Lbl in m_CreditsStrings)
            {
                m_Tween.Tween(Lbl, new { YPosition = Separation }, 1500);
                Separation += 15.0f;
            }

            HitVM.PlayEvent("bkground_credits");
        }

        #region EventHandlers


        private void MaxisButton_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_WillWrightDiag.Visible = true;
        }

        #endregion

        public override void Update(InputHelper Input, GameTime GTime)
        {
            base.Update(Input, GTime);

            m_Tween.Update(0.3f); //Set this to a lower value if text scrolls by too fast.

            foreach (UILabel Lbl in m_CreditsStrings)
            {
                //TODO: Clip the credits area!
                if (Lbl.YPosition > m_CreditsArea.Position.Y && (Lbl.YPosition < m_CreditsArea.Size.Y))
                    Lbl.Visible = true;
                else Lbl.Visible = false;
            }
        }

        public override void Draw()
        {
            m_SBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, 
                RasterizerState.CullCounterClockwise, null, Resolution.TransformationMatrix());

            base.Draw();

            Manager.Device.Clear(Color.Black);

            m_SBatch.End();
        }
    }
}
