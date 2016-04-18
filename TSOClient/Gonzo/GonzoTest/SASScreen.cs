using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gonzo;
using Gonzo.Elements;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GonzoTest
{
    public class SASScreen : UIScreen
    {
        private UIImage BackgroundImg, TabBackgroundImg1, TabBackgroundImg2, TabBackgroundImg3, DescriptionTabBackgroundImg1, DescriptionTabBackgroundImg2, DescriptionTabBackgroundImg3, 
            DescriptionTabImage1, DescriptionTabImage2, DescriptionTabImage3, EnterTabImage1, EnterTabImage2, EnterTabImage3, EnterTabBackgroundImage1, EnterTabBackgroundImage2, EnterTabBackgroundImage3, /*DefaultHouseImg,*/ CreditsBackgroundImg/*, CityThumbnailBusyImg*/;
        private UIButton m_ExitButton, m_EnterTabBtn1, m_EnterTabBtn2, m_EnterTabBtn3, m_DescriptionTabBtn1, 
            m_DescriptionTabBtn2, m_DescriptionTabBtn3, m_AvatarButton1, m_AvatarButton2, m_AvatarButton3, m_CityButton1, 
            m_CityButton2, m_CityButton3, m_HouseButton1, m_HouseButton2, m_HouseButton3, m_NewAvatarButton1, m_NewAvatarButton2,
            m_NewAvatarButton3, m_DeleteAvatarButton1, m_DeleteAvatarButton2, m_DeleteAvatarButton3, 
            m_PersonDescriptionScrollUpBtn1, m_PersonDescriptionScrollUpBtn2, m_PersonDescriptionScrollUpBtn3, 
            m_PersonDescriptionScrollDownBtn1, m_PersonDescriptionScrollDownBtn2, m_PersonDescriptionScrollDownBtn3, 
            m_CreditsButton;

        private bool m_InDescriptionTab1 = true, m_InDescriptionTab2 = true, m_InDescriptionTab3 = true,
            m_InEnterTab1 = false, m_InEnterTab2 = false, m_InEnterTab3 = false;

        public SASScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "SAS", SBatch,
            new Vector2(0, 0), new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight),
            GlobalSettings.Default.StartupPath + "\\" + "gamedata\\uiscripts\\personselection.uis")
        {
            BackgroundImg = (UIImage)m_Elements["\"BackgroundImage\""];

            TabBackgroundImg1 = m_Elements["\"TabBackgroundImage1\""].Image;
            TabBackgroundImg2 = m_Elements["\"TabBackgroundImage2\""].Image;
            TabBackgroundImg3 = m_Elements["\"TabBackgroundImage3\""].Image;

            DescriptionTabBackgroundImg1 = m_Elements["\"DescriptionTabBackgroundImage1\""].Image;
            DescriptionTabBackgroundImg2 = m_Elements["\"DescriptionTabBackgroundImage2\""].Image;
            DescriptionTabBackgroundImg3 = m_Elements["\"DescriptionTabBackgroundImage3\""].Image;

            DescriptionTabImage1 = m_Elements["\"DescriptionTabImage1\""].Image;
            DescriptionTabImage2 = m_Elements["\"DescriptionTabImage1\""].Image;
            DescriptionTabImage3 = m_Elements["\"DescriptionTabImage1\""].Image;

            EnterTabImage1 = m_Elements["\"EnterTabImage1\""].Image;
            EnterTabImage2 = m_Elements["\"EnterTabImage2\""].Image;
            EnterTabImage3 = m_Elements["\"EnterTabImage3\""].Image;

            EnterTabBackgroundImage1 = m_Elements["\"EnterTabBackgroundImage1\""].Image;
            EnterTabBackgroundImage2 = m_Elements["\"EnterTabBackgroundImage2\""].Image;
            EnterTabBackgroundImage3 = m_Elements["\"EnterTabBackgroundImage3\""].Image;

            //DefaultHouseImg = (UIImage)m_Elements["\"DefaultHouseImage\""].Image; //Not used??

            CreditsBackgroundImg = m_Controls["\"CreditsButtonBackgroundImage\""].Image;
            //CityThumbnailBusyImg = m_Controls["\"CityThumbnailBusyImage\""].Image; //Not used??

            m_ExitButton = (UIButton)m_Elements["\"ExitButton\""];

            m_EnterTabBtn1 = (UIButton)m_Elements["\"EnterTabButton1\""];
            m_EnterTabBtn1.OnButtonClicked += EnterTabBtn1_OnButtonClicked;
            m_EnterTabBtn2 = (UIButton)m_Elements["\"EnterTabButton2\""];
            m_EnterTabBtn2.OnButtonClicked += EnterTabBtn2_OnButtonClicked;
            m_EnterTabBtn3 = (UIButton)m_Elements["\"EnterTabButton3\""];
            m_EnterTabBtn3.OnButtonClicked += EnterTabBtn3_OnButtonClicked;

            m_DescriptionTabBtn1 = (UIButton)m_Elements["\"DescriptionTabButton1\""];
            m_DescriptionTabBtn1.OnButtonClicked += DescriptionTabBtn1_OnButtonClicked;
            m_DescriptionTabBtn2 = (UIButton)m_Elements["\"DescriptionTabButton2\""];
            m_DescriptionTabBtn2.OnButtonClicked += DescriptionTabBtn2_OnButtonClicked;
            m_DescriptionTabBtn3 = (UIButton)m_Elements["\"DescriptionTabButton3\""];
            m_DescriptionTabBtn3.OnButtonClicked += DescriptionTabBtn3_OnButtonClicked;

            m_AvatarButton1 = (UIButton)m_Elements["\"AvatarButton1\""];
            m_AvatarButton2 = (UIButton)m_Elements["\"AvatarButton2\""];
            m_AvatarButton3 = (UIButton)m_Elements["\"AvatarButton3\""];

            m_CityButton1 = (UIButton)m_Elements["\"CityButton1\""];
            m_CityButton2 = (UIButton)m_Elements["\"CityButton2\""];
            m_CityButton3 = (UIButton)m_Elements["\"CityButton3\""];

            m_HouseButton1 = (UIButton)m_Elements["\"HouseButton1\""];
            m_HouseButton2 = (UIButton)m_Elements["\"HouseButton2\""];
            m_HouseButton3 = (UIButton)m_Elements["\"HouseButton3\""];

            m_NewAvatarButton1 = (UIButton)m_Elements["\"NewAvatarButton1\""];
            m_NewAvatarButton2 = (UIButton)m_Elements["\"NewAvatarButton2\""];
            m_NewAvatarButton3 = (UIButton)m_Elements["\"NewAvatarButton3\""];

            m_DeleteAvatarButton1 = (UIButton)m_Elements["\"DeleteAvatarButton1\""];
            m_DeleteAvatarButton2 = (UIButton)m_Elements["\"DeleteAvatarButton2\""];
            m_DeleteAvatarButton3 = (UIButton)m_Elements["\"DeleteAvatarButton3\""];

            m_PersonDescriptionScrollUpBtn1 = (UIButton)m_Elements["\"PersonDescriptionScrollUpButton1\""];
            m_PersonDescriptionScrollUpBtn2 = (UIButton)m_Elements["\"PersonDescriptionScrollUpButton2\""];
            m_PersonDescriptionScrollUpBtn3 = (UIButton)m_Elements["\"PersonDescriptionScrollUpButton3\""];

            m_PersonDescriptionScrollDownBtn1 = (UIButton)m_Elements["\"PersonDescriptionScrollDownButton1\""];
            m_PersonDescriptionScrollDownBtn2 = (UIButton)m_Elements["\"PersonDescriptionScrollDownButton2\""];
            m_PersonDescriptionScrollDownBtn3 = (UIButton)m_Elements["\"PersonDescriptionScrollDownButton3\""];

            m_CreditsButton = (UIButton)m_Elements["\"CreditsButton\""];
        }

        #region Eventhandlers

        private void DescriptionTabBtn3_OnButtonClicked(UIButton ClickedButton)
        {
            m_InDescriptionTab3 = true;
            m_InEnterTab3 = false;
        }

        private void DescriptionTabBtn2_OnButtonClicked(UIButton ClickedButton)
        {
            m_InDescriptionTab2 = true;
            m_InEnterTab2 = false;
        }

        private void DescriptionTabBtn1_OnButtonClicked(UIButton ClickedButton)
        {
            m_InDescriptionTab1 = true;
            m_InEnterTab1 = false;
        }

        private void EnterTabBtn3_OnButtonClicked(UIButton ClickedButton)
        {
            m_InEnterTab3 = true;
            m_InDescriptionTab3 = false;
        }

        private void EnterTabBtn2_OnButtonClicked(UIButton ClickedButton)
        {
            m_InEnterTab2 = true;
            m_InDescriptionTab2 = false;
        }

        private void EnterTabBtn1_OnButtonClicked(UIButton ClickedButton)
        {
            m_InEnterTab1 = true;
            m_InDescriptionTab1 = false;
        }

        #endregion

        public override void Update(InputHelper Input)
        {
            base.Update(Input);
        }

        public override void Draw()
        {
            BackgroundImg.Draw(m_SBatch, null, 0.0f);

            TabBackgroundImg1.Draw(m_SBatch, null, 0.0f);
            TabBackgroundImg2.Draw(m_SBatch, null, 0.0f);
            TabBackgroundImg3.Draw(m_SBatch, null, 0.0f);

            if (m_InDescriptionTab1)
            {
                DescriptionTabBackgroundImg1.Draw(m_SBatch, null, 0.0f);
                DescriptionTabImage1.Draw(m_SBatch, null, 0.0f);
            }
            else if (m_InEnterTab1)
            {
                EnterTabBackgroundImage1.Draw(m_SBatch, null, 0.0f);
                EnterTabImage1.Draw(m_SBatch, null, 0.0f);
            }

            if (m_InDescriptionTab2)
            {
                DescriptionTabBackgroundImg2.Draw(m_SBatch, null, 0.0f);
                DescriptionTabImage2.Draw(m_SBatch, null, 0.0f);
            }
            else if (m_InEnterTab2)
            {
                EnterTabBackgroundImage2.Draw(m_SBatch, null, 0.0f);
                EnterTabImage2.Draw(m_SBatch, null, 0.0f);
            }

            if (m_InDescriptionTab3)
            {
                DescriptionTabBackgroundImg3.Draw(m_SBatch, null, 0.0f);
                DescriptionTabImage3.Draw(m_SBatch, null, 0.0f);
            }
            else if (m_InEnterTab3)
            {
                EnterTabBackgroundImage3.Draw(m_SBatch, null, 0.0f);
                EnterTabImage3.Draw(m_SBatch, null, 0.0f);
            }

            //DefaultHouseImg.Draw(m_SBatch, null, 0.0f);

            CreditsBackgroundImg.Draw(m_SBatch, null, 0.0f);
            //CityThumbnailBusyImg.Draw(m_SBatch, null, 0.0f);

            base.Draw();
        }
    }
}
