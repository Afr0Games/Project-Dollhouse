
using System.Collections.Generic;
using Shared;
using Gonzo;
using Gonzo.Elements;
using Gonzo.Dialogs;
using Files;
using Files.Manager;
using Sound;
using Vitaboy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;

namespace GonzoTest
{
    public class SASScreen : UIScreen
    {
        private UIBackgroundImage m_BackgroundImg;
        private UIImage TabBackgroundImg1, TabBackgroundImg2, TabBackgroundImg3, DescriptionTabBackgroundImg1, DescriptionTabBackgroundImg2, DescriptionTabBackgroundImg3, 
            DescriptionTabImage1, DescriptionTabImage2, DescriptionTabImage3, EnterTabImage1, EnterTabImage2, EnterTabImage3, EnterTabBackgroundImage1, EnterTabBackgroundImage2, EnterTabBackgroundImage3, /*DefaultHouseImg,*/ CreditsBackgroundImg,/*, CityThumbnailBusyImg*/
            CityButtonTemplateImage;
        private UIButton m_ExitButton, m_EnterTabBtn1, m_EnterTabBtn2, m_EnterTabBtn3, m_DescriptionTabBtn1, 
            m_DescriptionTabBtn2, m_DescriptionTabBtn3, m_AvatarButton1, m_AvatarButton2, m_AvatarButton3, m_CityButton1, 
            m_CityButton2, m_CityButton3, m_HouseButton1, m_HouseButton2, m_HouseButton3, m_NewAvatarButton1, m_NewAvatarButton2,
            m_NewAvatarButton3, m_DeleteAvatarButton1, m_DeleteAvatarButton2, m_DeleteAvatarButton3, m_PersonDescriptionScrollUpBtn1, 
            m_PersonDescriptionScrollUpBtn2, m_PersonDescriptionScrollUpBtn3, m_PersonDescriptionScrollDownBtn1, 
            m_PersonDescriptionScrollDownBtn2, m_PersonDescriptionScrollDownBtn3, m_CreditsButton;
        private ExitDialog m_ExitDialog;

        private UITextEdit m_PersonDescriptionText1, m_PersonDescriptionText2, m_PersonDescriptionText3;

        private UILabel m_CityNameText1, m_CityNameText2, m_CityNameText3, 
            m_HouseNameText1, m_HouseNameText2, m_HouseNameText3;

        //TODO: Move this to GameFacade...
        private List<Sim> m_Avatars = new List<Sim>();

        public SASScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "SAS", SBatch,
            new Vector2(0, 0), new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight),
            GlobalSettings.Default.StartupPath + "\\" + "gamedata\\uiscripts\\personselection.uis")
        {
            m_BackgroundImg = (UIBackgroundImage)m_PResult.Elements["\"BackgroundImage\""];

            AdultAvatar Avatar = new AdultAvatar(Manager.Device, Manager.HeadShader);
            Avatar.ChangeOutfit(FileManager.Instance.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fab001_sl__pjs4), Vitaboy.SkinType.Medium);
            Avatar.SetHead(FileManager.Instance.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fah002_mom), Vitaboy.SkinType.Medium);
            AddAvatar(Avatar, 1);

            VitaboyScreen VScreen = new VitaboyScreen(Manager, new Vector2(0, 0),
                new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight));
            VScreen.AddSim(m_Avatars[0]);

            Manager.AddScreen(VScreen);

            TabBackgroundImg1 = m_PResult.Elements["\"TabBackgroundImage1\""].Image;
            TabBackgroundImg2 = m_PResult.Elements["\"TabBackgroundImage2\""].Image;
            TabBackgroundImg3 = m_PResult.Elements["\"TabBackgroundImage3\""].Image;

            DescriptionTabBackgroundImg1 = m_PResult.Elements["\"DescriptionTabBackgroundImage1\""].Image;
            DescriptionTabBackgroundImg1.Visible = m_Avatars.Count >= 1 ? true : false;
            DescriptionTabBackgroundImg2 = m_PResult.Elements["\"DescriptionTabBackgroundImage2\""].Image;
            DescriptionTabBackgroundImg2.Visible = m_Avatars.Count >= 2 ? true : false;
            DescriptionTabBackgroundImg3 = m_PResult.Elements["\"DescriptionTabBackgroundImage3\""].Image;
            DescriptionTabBackgroundImg3.Visible = m_Avatars.Count == 3 ? true : false;

            DescriptionTabImage1 = m_PResult.Elements["\"DescriptionTabImage1\""].Image;
            DescriptionTabImage1.Visible = m_Avatars.Count >= 1 ? true : false;
            DescriptionTabImage2 = m_PResult.Elements["\"DescriptionTabImage2\""].Image;
            DescriptionTabImage2.Visible = m_Avatars.Count >= 2 ? true : false;
            DescriptionTabImage3 = m_PResult.Elements["\"DescriptionTabImage3\""].Image;
            DescriptionTabImage3.Visible = m_Avatars.Count == 3 ? true : false;

            EnterTabImage1 = m_PResult.Elements["\"EnterTabImage1\""].Image;
            EnterTabImage1.Visible = false;
            EnterTabImage2 = m_PResult.Elements["\"EnterTabImage2\""].Image;
            EnterTabImage2.Visible = false;
            EnterTabImage3 = m_PResult.Elements["\"EnterTabImage3\""].Image;
            EnterTabImage3.Visible = false;

            EnterTabBackgroundImage1 = m_PResult.Elements["\"EnterTabBackgroundImage1\""].Image;
            EnterTabBackgroundImage1.Visible = false;
            EnterTabBackgroundImage2 = m_PResult.Elements["\"EnterTabBackgroundImage2\""].Image;
            EnterTabBackgroundImage2.Visible = false;
            EnterTabBackgroundImage3 = m_PResult.Elements["\"EnterTabBackgroundImage3\""].Image;
            EnterTabBackgroundImage3.Visible = false;

            //DefaultHouseImg = (UIImage)m_Elements["\"DefaultHouseImage\""].Image; //Not used??

            CreditsBackgroundImg = m_PResult.Controls["\"CreditsButtonBackgroundImage\""].Image;
            //CityThumbnailBusyImg = m_Controls["\"CityThumbnailBusyImage\""].Image; //Not used??

            CityButtonTemplateImage = (UIImage)m_PResult.Elements["\"CityButtonTemplateImage\""];

            m_ExitButton = (UIButton)m_PResult.Elements["\"ExitButton\""];
            m_ExitButton.OnButtonClicked += M_ExitButton_OnButtonClicked;
            m_ExitDialog = new ExitDialog(this, new Vector2(250, 250), m_Walker, 
                GlobalSettings.Default.StartupPath + "gamedata\\uiscripts\\exitdialog.uis");
            m_ExitDialog.Visible = false;

            m_EnterTabBtn1 = (UIButton)m_PResult.Elements["\"EnterTabButton1\""];
            m_EnterTabBtn1.OnButtonClicked += EnterTabBtn1_OnButtonClicked;
            m_EnterTabBtn1.Enabled = m_Avatars.Count >= 1 ? true : false;
            m_EnterTabBtn2 = (UIButton)m_PResult.Elements["\"EnterTabButton2\""];
            m_EnterTabBtn2.OnButtonClicked += EnterTabBtn2_OnButtonClicked;
            m_EnterTabBtn2.Enabled = m_Avatars.Count >= 2 ? true : false;
            m_EnterTabBtn3 = (UIButton)m_PResult.Elements["\"EnterTabButton3\""];
            m_EnterTabBtn3.OnButtonClicked += EnterTabBtn3_OnButtonClicked;
            m_EnterTabBtn3.Enabled = m_Avatars.Count == 3 ? true : false;

            m_DescriptionTabBtn1 = (UIButton)m_PResult.Elements["\"DescriptionTabButton1\""];
            m_DescriptionTabBtn1.OnButtonClicked += DescriptionTabBtn1_OnButtonClicked;
            m_DescriptionTabBtn1.Enabled = m_Avatars.Count >= 1 ? true : false;
            m_DescriptionTabBtn2 = (UIButton)m_PResult.Elements["\"DescriptionTabButton2\""];
            m_DescriptionTabBtn2.OnButtonClicked += DescriptionTabBtn2_OnButtonClicked;
            m_DescriptionTabBtn2.Enabled = m_Avatars.Count >= 2 ? true : false;
            m_DescriptionTabBtn3 = (UIButton)m_PResult.Elements["\"DescriptionTabButton3\""];
            m_DescriptionTabBtn3.OnButtonClicked += DescriptionTabBtn3_OnButtonClicked;
            m_DescriptionTabBtn3.Enabled = m_Avatars.Count == 3 ? true : false;

            m_AvatarButton1 = (UIButton)m_PResult.Elements["\"AvatarButton1\""];
            m_AvatarButton2 = (UIButton)m_PResult.Elements["\"AvatarButton2\""];
            m_AvatarButton3 = (UIButton)m_PResult.Elements["\"AvatarButton3\""];

            m_CityButton1 = (UIButton)m_PResult.Elements["\"CityButton1\""];
            m_CityButton1.AddImage(GetImage("\"CityButtonTemplateImage\"", true));
            m_CityButton1.Visible = false;
            m_CityButton2 = (UIButton)m_PResult.Elements["\"CityButton2\""];
            m_CityButton2.AddImage(GetImage("\"CityButtonTemplateImage\"", true));
            m_CityButton2.Visible = false;
            m_CityButton3 = (UIButton)m_PResult.Elements["\"CityButton3\""];
            m_CityButton3.AddImage(GetImage("\"CityButtonTemplateImage\"", true));
            m_CityButton3.Visible = false;

            m_HouseButton1 = (UIButton)m_PResult.Elements["\"HouseButton1\""];
            m_HouseButton1.AddImage(GetImage("\"HouseButtonTemplateImage\"", true));
            m_HouseButton1.Visible = false;
            m_HouseButton2 = (UIButton)m_PResult.Elements["\"HouseButton2\""];
            m_HouseButton2.AddImage(GetImage("\"HouseButtonTemplateImage\"", true));
            m_HouseButton2.Visible = false;
            m_HouseButton3 = (UIButton)m_PResult.Elements["\"HouseButton3\""];
            m_HouseButton3.AddImage(GetImage("\"HouseButtonTemplateImage\"", true));
            m_HouseButton3.Visible = false;

            m_NewAvatarButton1 = (UIButton)m_PResult.Elements["\"NewAvatarButton1\""];
            m_NewAvatarButton1.Visible = m_Avatars.Count >= 1 ? false : true;
            m_NewAvatarButton2 = (UIButton)m_PResult.Elements["\"NewAvatarButton2\""];
            m_NewAvatarButton2.Visible = m_Avatars.Count >= 2 ? false : true;
            m_NewAvatarButton3 = (UIButton)m_PResult.Elements["\"NewAvatarButton3\""];
            m_NewAvatarButton3.Visible = m_Avatars.Count == 3 ? false : true;

            m_DeleteAvatarButton1 = (UIButton)m_PResult.Elements["\"DeleteAvatarButton1\""];
            m_DeleteAvatarButton1.Visible = m_Avatars.Count >= 1 ? true : false;
            m_DeleteAvatarButton2 = (UIButton)m_PResult.Elements["\"DeleteAvatarButton2\""];
            m_DeleteAvatarButton2.Visible = m_Avatars.Count >= 2 ? true : false;
            m_DeleteAvatarButton3 = (UIButton)m_PResult.Elements["\"DeleteAvatarButton3\""];
            m_DeleteAvatarButton3.Visible = m_Avatars.Count == 3 ? true : false;

            m_PersonDescriptionText1 = (UITextEdit)m_PResult.Elements["\"PersonDescriptionText1\""];
            m_PersonDescriptionText1.Visible = m_Avatars.Count >= 1 ? true : false;
            m_PersonDescriptionText2 = (UITextEdit)m_PResult.Elements["\"PersonDescriptionText2\""];
            m_PersonDescriptionText2.Visible = m_Avatars.Count >= 2 ? true : false;
            m_PersonDescriptionText3 = (UITextEdit)m_PResult.Elements["\"PersonDescriptionText3\""];
            m_PersonDescriptionText3.Visible = m_Avatars.Count == 3 ? true : false;

            m_PersonDescriptionScrollUpBtn1 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollUpButton1\""];
            m_PersonDescriptionScrollUpBtn1.Visible = m_PersonDescriptionText1.Visible ? true : false;
            m_PersonDescriptionScrollUpBtn2 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollUpButton2\""];
            m_PersonDescriptionScrollUpBtn2.Visible = m_PersonDescriptionText2.Visible ? true : false;
            m_PersonDescriptionScrollUpBtn3 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollUpButton3\""];
            m_PersonDescriptionScrollUpBtn3.Visible = m_PersonDescriptionText3.Visible ? true : false;

            m_PersonDescriptionScrollDownBtn1 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollDownButton1\""];
            m_PersonDescriptionScrollDownBtn1.Visible = m_PersonDescriptionText1.Visible ? true : false;
            m_PersonDescriptionScrollDownBtn2 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollDownButton2\""];
            m_PersonDescriptionScrollDownBtn2.Visible = m_PersonDescriptionText2.Visible ? true : false;
            m_PersonDescriptionScrollDownBtn3 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollDownButton3\""];
            m_PersonDescriptionScrollDownBtn3.Visible = m_PersonDescriptionText3.Visible ? true : false;

            m_CityNameText1 = (UILabel)m_PResult.Elements["\"CityNameText1\""];
            m_CityNameText1.Visible = false;
            m_CityNameText2 = (UILabel)m_PResult.Elements["\"CityNameText2\""];
            m_CityNameText2.Visible = false;
            m_CityNameText3 = (UILabel)m_PResult.Elements["\"CityNameText3\""];
            m_CityNameText3.Visible = false;

            //TODO: Assign captions to these if a sim has a house...
            m_HouseNameText1 = (UILabel)m_PResult.Elements["\"HouseNameText1\""];
            m_HouseNameText2 = (UILabel)m_PResult.Elements["\"HouseNameText2\""];
            m_HouseNameText3 = (UILabel)m_PResult.Elements["\"HouseNameText3\""];

            m_CreditsButton = (UIButton)m_PResult.Elements["\"CreditsButton\""];
        }

        /// <summary>
        /// Adds an avatar to this instance of SASScreen.
        /// </summary>
        /// <param name="Avatar">An instance of AdultAvatar. 
        /// Assumed to already have a head and outfit.</param>
        /// <param name="ID">ID from 1 - 3, indicates which "box" this avatar will be rendered in.</param>
        public void AddAvatar(AdultAvatar Avatar, int ID)
        {
            switch(ID)
            {
                case 1:
                    if (m_Avatars.Count != 0)
                        m_Avatars[0] = new Sim(Manager.Device, Manager.RenderCamera, Avatar);
                    else
                        m_Avatars.Add(new Sim(Manager.Device, Manager.RenderCamera, Avatar));

                    m_Avatars[0].ShouldRotate = true;
                    break;
            }
        }

        #region Eventhandlers

        private void M_ExitButton_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_ExitDialog.Visible = true;
        }

        private void DescriptionTabBtn3_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_HouseButton3.Visible = false;
            m_CityButton3.Visible = false;

            m_PersonDescriptionText3.Visible = true;
            m_PersonDescriptionScrollDownBtn3.Visible = true;
            m_PersonDescriptionScrollUpBtn3.Visible = true;

            DescriptionTabBackgroundImg3.Visible = true;
            DescriptionTabImage3.Visible = true;

            EnterTabImage3.Visible = false;
            EnterTabBackgroundImage3.Visible = false;

            m_CityNameText3.Visible = false;

            //HitVM.PlayEvent("ui_nhood_click");
            HitVM.PlayEvent("vox_teasee_giggle");
        }

        private void DescriptionTabBtn2_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_HouseButton2.Visible = false;
            m_CityButton2.Visible = false;

            m_PersonDescriptionText2.Visible = true;
            m_PersonDescriptionScrollDownBtn2.Visible = true;
            m_PersonDescriptionScrollUpBtn2.Visible = true;

            DescriptionTabBackgroundImg2.Visible = true;
            DescriptionTabImage2.Visible = true;

            EnterTabImage2.Visible = false;
            EnterTabBackgroundImage2.Visible = false;

            m_CityNameText2.Visible = false;

            //HitVM.PlayEvent("ui_nhood_click");
            HitVM.PlayEvent("vox_teasee_giggle");
        }

        private void DescriptionTabBtn1_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_HouseButton1.Visible = false;
            m_CityButton1.Visible = false;

            m_PersonDescriptionText1.Visible = true;
            m_PersonDescriptionScrollDownBtn1.Visible = true;
            m_PersonDescriptionScrollUpBtn1.Visible = true;

            DescriptionTabBackgroundImg1.Visible = true;
            DescriptionTabImage1.Visible = true;

            EnterTabImage1.Visible = false;
            EnterTabBackgroundImage1.Visible = false;

            m_CityNameText1.Visible = false;

            //HitVM.PlayEvent("ui_nhood_click");
            HitVM.PlayEvent("vox_teasee_giggle");
        }

        private void EnterTabBtn3_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            if (m_Avatars.Count < 3)
            {
                m_HouseButton3.Visible = false;
                m_CityButton3.Visible = false;
            }
            else
            {
                if (m_Avatars[2].HasHouse)
                    m_HouseButton3.Visible = true;

                m_CityButton3.Visible = true;
                m_CityNameText3.Visible = true;
            }

            m_PersonDescriptionText3.Visible = false;
            m_PersonDescriptionScrollDownBtn3.Visible = false;
            m_PersonDescriptionScrollUpBtn3.Visible = false;

            DescriptionTabBackgroundImg3.Visible = false;
            DescriptionTabImage3.Visible = false;

            EnterTabImage3.Visible = true;
            EnterTabBackgroundImage3.Visible = true;

            //HitVM.PlayEvent("ui_nhood_click");
            HitVM.PlayEvent("vox_teasee_giggle");
        }

        private void EnterTabBtn2_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            if (m_Avatars.Count < 2)
            {
                m_HouseButton2.Visible = false;
                m_CityButton2.Visible = false;
            }
            else
            {
                if (m_Avatars[1].HasHouse)
                    m_HouseButton2.Visible = true;

                m_CityButton2.Visible = true;
                m_CityNameText2.Visible = true;
            }

            m_PersonDescriptionText2.Visible = false;
            m_PersonDescriptionScrollDownBtn2.Visible = false;
            m_PersonDescriptionScrollUpBtn2.Visible = false;

            DescriptionTabBackgroundImg2.Visible = false;
            DescriptionTabImage2.Visible = false;

            EnterTabImage2.Visible = true;
            EnterTabBackgroundImage2.Visible = true;

            //HitVM.PlayEvent("ui_nhood_click");
            HitVM.PlayEvent("vox_teasee_giggle");
        }

        private void EnterTabBtn1_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            if (m_Avatars.Count == 0)
            {
                m_HouseButton1.Visible = false;
                m_CityButton1.Visible = false;
            }
            else
            {
                if (m_Avatars[0].HasHouse)
                    m_HouseButton1.Visible = true;

                m_CityButton1.Visible = true;
                m_CityNameText1.Visible = true;
            }

            m_PersonDescriptionText1.Visible = false;
            m_PersonDescriptionScrollDownBtn1.Visible = false;
            m_PersonDescriptionScrollUpBtn1.Visible = false;

            DescriptionTabBackgroundImg1.Visible = false;
            DescriptionTabImage1.Visible = false;

            EnterTabImage1.Visible = true;
            EnterTabBackgroundImage1.Visible = true;

            //HitVM.PlayEvent("ui_nhood_click");
            HitVM.PlayEvent("vox_teasee_giggle");
        }

        #endregion

        public override void Update(InputHelper Input, GameTime GTime)
        {
            base.Update(Input, GTime);

            m_ExitDialog.Update(Input, GTime);
        }

        public override void Draw()
        {
            m_SBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, 
                RasterizerState.CullCounterClockwise, null, Resolution.TransformationMatrix());

            base.Draw();

            m_BackgroundImg.Draw(m_SBatch, null, 0.0f);

            TabBackgroundImg1.Draw(m_SBatch, null, 0.5f);
            TabBackgroundImg2.Draw(m_SBatch, null, 0.5f);
            TabBackgroundImg3.Draw(m_SBatch, null, 0.5f);

            DescriptionTabBackgroundImg1.Draw(m_SBatch, null, 0.7f);
            DescriptionTabImage1.Draw(m_SBatch, null, 0.7f);

            EnterTabBackgroundImage1.Draw(m_SBatch, null, 0.5f);
            EnterTabImage1.Draw(m_SBatch, null, 0.5f);

            DescriptionTabBackgroundImg2.Draw(m_SBatch, null, 0.7f);
            DescriptionTabImage2.Draw(m_SBatch, null, 0.7f);

            EnterTabBackgroundImage2.Draw(m_SBatch, null, 0.5f);
            EnterTabImage2.Draw(m_SBatch, null, 0.5f);

            DescriptionTabBackgroundImg3.Draw(m_SBatch, null, 0.7f);
            DescriptionTabImage3.Draw(m_SBatch, null, 0.7f);

            EnterTabBackgroundImage3.Draw(m_SBatch, null, 0.5f);
            EnterTabImage3.Draw(m_SBatch, null, 0.5f);

            //DefaultHouseImg.Draw(m_SBatch, null, 0.0f);

            CreditsBackgroundImg.Draw(m_SBatch, null, 0.0f);
            //CityThumbnailBusyImg.Draw(m_SBatch, null, 0.0f);

            m_ExitDialog.Draw(m_SBatch, 0.9f);

            m_SBatch.End();

            foreach (UIElement Element in m_PResult.Elements.Values)
            {
                if (Element.NeedsClipping)
                {
                    RasterizerState RasterState = new RasterizerState();
                    RasterState.ScissorTestEnable = true;
                    RasterState.CullMode = CullMode.CullCounterClockwiseFace;

                    m_SBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null,
                       RasterState, null, Resolution.TransformationMatrix());

                    Element.Draw(m_SBatch, 0.5f);

                    m_SBatch.End();
                }
            }
        }
    }
}
