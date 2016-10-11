using Vitaboy;
using Shared;
using Files;
using Files.Manager;
using Files.Vitaboy;
using Gonzo;
using Gonzo.Elements;
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
        private UITextEdit m_DescriptionTextEdit;
        private UIHeadBrowser m_HeadSkinBrowser;
        private UIBodyBrowser m_BodySkinBrowser;

        private Sim m_Avatar;
        VitaboyScreen m_VitaboyScreen;

        public CASScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "CAS", SBatch, 
            new Vector2(0, 0), 
            new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight),
            GlobalSettings.Default.StartupPath + "\\" + "gamedata\\uiscripts\\personselectionedit.uis")
        {
            m_BackgroundImg = (UIImage)m_Elements["\"BackgroundImage\""];

            m_CancelBtn = (UIButton)m_Elements["\"CancelButton\""];
            m_AcceptBtn = (UIButton)m_Elements["\"AcceptButton\""];
            m_DescriptionScrollUpBtn = (UIButton)m_Elements["\"DescriptionScrollUpButton\""];
            m_DescriptionScrollDownBtn = (UIButton)m_Elements["\"DescriptionScrollDownButton\""];
            m_ExitBtn = (UIButton)m_Elements["\"ExitButton\""];

            m_FemaleBtn = (UIButton)m_Elements["\"FemaleButton\""];
            m_FemaleBtn.OnButtonClicked += M_FemaleBtn_OnButtonClicked;

            m_MaleBtn = (UIButton)m_Elements["\"MaleButton\""];
            m_MaleBtn.OnButtonClicked += M_MaleBtn_OnButtonClicked;

            m_SkinLightBtn = (UIButton)m_Elements["\"SkinLightButton\""];
            m_SkinLightBtn.OnButtonClicked += M_SkinLightBtn_OnButtonClicked;

            m_SkinMediumBtn = (UIButton)m_Elements["\"SkinMediumButton\""];
            m_SkinMediumBtn.OnButtonClicked += M_SkinMediumBtn_OnButtonClicked;

            m_SkinDarkBtn = (UIButton)m_Elements["\"SkinDarkButton\""];
            m_SkinDarkBtn.OnButtonClicked += M_SkinDarkBtn_OnButtonClicked;

            m_DescriptionTextEdit = (UITextEdit)m_Elements["\"DescriptionTextEdit\""];

            m_HeadSkinBrowser = new UIHeadBrowser(this, m_Controls["\"HeadSkinBrowser\""], 1, AvatarSex.Male);
            m_HeadSkinBrowser.OnButtonClicked += M_HeadSkinBrowser_OnButtonClicked;
            m_BodySkinBrowser = new UIBodyBrowser(this, m_Controls["\"BodySkinBrowser\""], 1, AvatarSex.Male);
            m_BodySkinBrowser.OnButtonClicked += M_BodySkinBrowser_OnButtonClicked;

            AdultAvatar Avatar = new AdultAvatar(Manager.Device);
            Avatar.ChangeOutfit(FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fab001_sl__pjs4), Vitaboy.SkinType.Medium);
            Avatar.Head = FileManager.GetAppearance((ulong)FileIDs.AppearancesFileIDs.fahm814_unleashedkim2);
            Avatar.ShouldRotate = true;

            m_Avatar = new Sim(Manager.Device, Avatar);
            m_Avatar.Camera.Origin = new Vector2(175, 100);
            m_Avatar.Camera.Zoom = 0.7f;

            m_VitaboyScreen = new VitaboyScreen(Manager, new Vector2(0, 0), 
                new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight));
            m_VitaboyScreen.AddSim(m_Avatar);

            Manager.AddScreen(m_VitaboyScreen);
        }

        private void M_MaleBtn_OnButtonClicked(UIButton ClickedButton)
        {
            m_HeadSkinBrowser.Sex = AvatarSex.Male;
            m_BodySkinBrowser.Sex = AvatarSex.Male;
        }

        private void M_FemaleBtn_OnButtonClicked(UIButton ClickedButton)
        {
            m_HeadSkinBrowser.Sex = AvatarSex.Female;
            m_BodySkinBrowser.Sex = AvatarSex.Female;
        }

        /// <summary>
        /// The player clicked the SkinLightButton.
        /// This causes the browsers' skin type to change to light.
        /// </summary>
        private void M_SkinLightBtn_OnButtonClicked(UIButton ClickedButton)
        {
            m_HeadSkinBrowser.SkinType = 0;
            m_BodySkinBrowser.SkinType = 0;
        }

        /// <summary>
        /// The player clicked the SkinMediumButton.
        /// This causes the browsers' skin type to change to medium.
        /// </summary>
        private void M_SkinMediumBtn_OnButtonClicked(UIButton ClickedButton)
        {
            m_HeadSkinBrowser.SkinType = 1;
            m_BodySkinBrowser.SkinType = 1;
        }

        /// <summary>
        /// The player clicked the SkinDarkButton.
        /// This causes the browsers' skin type to change to dark.
        /// </summary>
        private void M_SkinDarkBtn_OnButtonClicked(UIButton ClickedButton)
        {
            m_HeadSkinBrowser.SkinType = 2;
            m_BodySkinBrowser.SkinType = 2;
        }

        private void M_HeadSkinBrowser_OnButtonClicked(int SkinType, Outfit SelectedOutfit)
        {
            m_Avatar.ChangeOutfit(SelectedOutfit, (Vitaboy.SkinType)SkinType);
        }

        private void M_BodySkinBrowser_OnButtonClicked(int SkinType, Outfit SelectedOutfit)
        {
            m_Avatar.ChangeOutfit(SelectedOutfit, (Vitaboy.SkinType)SkinType);
        }

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
