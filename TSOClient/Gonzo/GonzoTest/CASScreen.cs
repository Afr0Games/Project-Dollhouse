using Vitaboy;
using Shared;
using Files;
using Files.Manager;
using Gonzo;
using Gonzo.Elements;
using Gonzo.Dialogs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GonzoTest
{
    public class CASScreen : UIScreen
    {
        private UIImage m_BackgroundImg;/*, m_LeftArrowImg, m_RightArrowImg, m_BodySkinButtonImg,
            m_HeadSkinButtonImg, m_CancelButtonImg, m_FemaleButtonImg, m_MaleButtonImg, m_AcceptButtonImg,
            m_ExitButtonImg, m_SkinDarkButtonImg, m_SkinLightButtonImg, m_SkinMediumButtonImg, m_ScrollbarImg,
            m_ScrollDownButtonImg, m_ScrollUpButtonImg;*/
        private UIButton m_CancelBtn, m_AcceptBtn, m_DescriptionScrollUpBtn, m_DescriptionScrollDownBtn,
            m_ExitBtn, m_FemaleBtn, m_MaleBtn, m_SkinLightBtn, m_SkinMediumBtn, m_SkinDarkBtn;
        private UIHeadBrowser m_HeadSkinBrowser;
        private UIBodyBrowser m_BodySkinBrowser;
        private ExitDialog m_ExitDialog;

        private Sim m_Avatar;
        VitaboyScreen m_VitaboyScreen;

        //0 = light, 1 = medium, 2 = dark
        private int m_CurrentSkinType = 1;

        public CASScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "CAS", SBatch, 
            new Vector2(0, 0), 
            new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight),
            GlobalSettings.Default.StartupPath + "\\" + "gamedata\\uiscripts\\personselectionedit.uis")
        {
            m_BackgroundImg = (UIImage)m_Walker.Elements["\"BackgroundImage\""];

            m_CancelBtn = (UIButton)m_Walker.Elements["\"CancelButton\""];
            m_AcceptBtn = (UIButton)m_Walker.Elements["\"AcceptButton\""];
            m_DescriptionScrollUpBtn = (UIButton)m_Walker.Elements["\"DescriptionScrollUpButton\""];
            m_DescriptionScrollDownBtn = (UIButton)m_Walker.Elements["\"DescriptionScrollDownButton\""];

            m_ExitBtn = (UIButton)m_Walker.Elements["\"ExitButton\""];
            m_ExitBtn.OnButtonClicked += M_ExitBtn_OnButtonClicked;

            m_FemaleBtn = (UIButton)m_Walker.Elements["\"FemaleButton\""];
            m_FemaleBtn.OnButtonClicked += M_FemaleBtn_OnButtonClicked;

            m_MaleBtn = (UIButton)m_Walker.Elements["\"MaleButton\""];
            m_MaleBtn.OnButtonClicked += M_MaleBtn_OnButtonClicked;

            m_SkinLightBtn = (UIButton)m_Walker.Elements["\"SkinLightButton\""];
            m_SkinLightBtn.OnButtonClicked += M_SkinLightBtn_OnButtonClicked;

            m_SkinMediumBtn = (UIButton)m_Walker.Elements["\"SkinMediumButton\""];
            m_SkinMediumBtn.OnButtonClicked += M_SkinMediumBtn_OnButtonClicked;

            m_SkinDarkBtn = (UIButton)m_Walker.Elements["\"SkinDarkButton\""];
            m_SkinDarkBtn.OnButtonClicked += M_SkinDarkBtn_OnButtonClicked;

            m_HeadSkinBrowser = new UIHeadBrowser(this, m_Walker.Controls["\"HeadSkinBrowser\""], 1, AvatarSex.Female);
            m_HeadSkinBrowser.OnButtonClicked += M_HeadSkinBrowser_OnButtonClicked;
            m_BodySkinBrowser = new UIBodyBrowser(this, m_Walker.Controls["\"BodySkinBrowser\""], 1, AvatarSex.Female);
            m_BodySkinBrowser.OnButtonClicked += M_BodySkinBrowser_OnButtonClicked;

            AdultAvatar Avatar = new AdultAvatar(Manager.Device);
            Avatar.ChangeOutfit(FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fab001_sl__pjs4), Vitaboy.SkinType.Medium);
            Avatar.SetHead(FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fah002_mom), (Vitaboy.SkinType)m_CurrentSkinType);
            Avatar.ShouldRotate = true;

            m_Avatar = new Sim(Manager.Device, Avatar);
            m_Avatar.Camera.Origin = new Vector2(175, 100);
            m_Avatar.Camera.Zoom = 0.7f;

            m_VitaboyScreen = new VitaboyScreen(Manager, new Vector2(0, 0), 
                new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight));
            m_VitaboyScreen.AddSim(m_Avatar);

            m_ExitDialog = new ExitDialog(this, new Vector2(250, 250), m_Walker,
                GlobalSettings.Default.StartupPath + "\\" + "gamedata\\uiscripts\\exitdialog.uis");
            m_ExitDialog.Visible = false;

            Manager.AddScreen(m_VitaboyScreen);
        }

        #region EventHandlers

        private void M_ExitBtn_OnButtonClicked(object Sender)
        {
            m_ExitDialog.Visible = true;
        }

        private void M_MaleBtn_OnButtonClicked(object Sender)
        {
            m_HeadSkinBrowser.Sex = AvatarSex.Male;
            m_BodySkinBrowser.Sex = AvatarSex.Male;

            m_Avatar.ChangeOutfit(FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.mab000_robin), (Vitaboy.SkinType)m_CurrentSkinType);
            m_Avatar.Head(FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.mah003_antony), (Vitaboy.SkinType)m_CurrentSkinType);
        }

        private void M_FemaleBtn_OnButtonClicked(object Sender)
        {
            m_HeadSkinBrowser.Sex = AvatarSex.Female;
            m_BodySkinBrowser.Sex = AvatarSex.Female;

            m_Avatar.ChangeOutfit(FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fab001_sl__pjs4), (Vitaboy.SkinType)m_CurrentSkinType);
            m_Avatar.Head(FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fah002_mom), (Vitaboy.SkinType)m_CurrentSkinType);
        }

        /// <summary>
        /// The player clicked the SkinLightButton.
        /// This causes the browsers' skin type to change to light.
        /// </summary>
        private void M_SkinLightBtn_OnButtonClicked(object Sender)
        {
            m_HeadSkinBrowser.SkinType = 0;
            m_BodySkinBrowser.SkinType = 0;
            m_CurrentSkinType = 0;
        }

        /// <summary>
        /// The player clicked the SkinMediumButton.
        /// This causes the browsers' skin type to change to medium.
        /// </summary>
        private void M_SkinMediumBtn_OnButtonClicked(object Sender)
        {
            m_HeadSkinBrowser.SkinType = 1;
            m_BodySkinBrowser.SkinType = 1;
            m_CurrentSkinType = 1;
        }

        /// <summary>
        /// The player clicked the SkinDarkButton.
        /// This causes the browsers' skin type to change to dark.
        /// </summary>
        private void M_SkinDarkBtn_OnButtonClicked(object Sender)
        {
            m_HeadSkinBrowser.SkinType = 2;
            m_BodySkinBrowser.SkinType = 2;
            m_CurrentSkinType = 2;
        }

        private void M_HeadSkinBrowser_OnButtonClicked(object Sender, UISkinButtonClickedEventArgs EArgs)
        {
            m_Avatar.ChangeOutfit(EArgs.SelectedOutfit, (Vitaboy.SkinType)EArgs.SkinType);
        }

        private void M_BodySkinBrowser_OnButtonClicked(object Sender, UISkinButtonClickedEventArgs EArgs)
        {
            m_Avatar.ChangeOutfit(EArgs.SelectedOutfit, (Vitaboy.SkinType)EArgs.SkinType);
        }

        #endregion

        public override void Update(InputHelper Input, GameTime GTime)
        {
            m_HeadSkinBrowser.Update(Input, GTime);
            m_BodySkinBrowser.Update(Input, GTime);

            base.Update(Input, GTime);
        }

        public override void Draw()
        {
            m_BackgroundImg.Draw(m_SBatch, null, 0.0f);
            m_HeadSkinBrowser.Draw(m_SBatch, 0.9f);
            m_BodySkinBrowser.Draw(m_SBatch, 0.9f);

            base.Draw();
        }
    }
}
