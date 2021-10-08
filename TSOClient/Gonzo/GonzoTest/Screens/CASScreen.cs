/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the GonzoTest.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Collections.Generic;
using Vitaboy;
using Shared;
using Files;
using Files.Manager;
using Sound;
using Gonzo;
using Gonzo.Elements;
using Gonzo.Dialogs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;
using MonoGame_Textbox;

namespace GonzoTest
{
    public class CASScreen : UIScreen
    {
        //All variables that exists in the script shares their name between the script and the code!
        private UIBackgroundImage m_BackgroundImg;
        private UIButton m_CancelBtn, m_AcceptBtn, m_DescriptionScrollUpBtn, m_DescriptionScrollDownBtn,
            m_ExitBtn, m_FemaleBtn, m_MaleBtn, m_SkinLightBtn, m_SkinMediumBtn, m_SkinDarkBtn;
        private UIHeadBrowser m_HeadSkinBrowser;
        private UIBodyBrowser m_BodySkinBrowser;
        private ExitDialog m_ExitDialog;
        private TextBox m_NameTextEdit;
        private UITextEdit m_DescriptionTextEdit;

        private Sim m_Avatar;
        VitaboyScreen m_VitaboyScreen;

        //0 = light, 1 = medium, 2 = dark
        private int m_CurrentSkinType = 1;

        public CASScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "CAS", SBatch, 
            new Vector2(0, 0), new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight),
            GlobalSettings.Default.StartupPath + "\\" + "gamedata\\uiscripts\\personselectionedit.uis")
        {
            m_BackgroundImg = (UIBackgroundImage)m_PResult.Elements["\"BackgroundImage\""];

            m_CancelBtn = (UIButton)m_PResult.Elements["\"CancelButton\""];
            m_AcceptBtn = (UIButton)m_PResult.Elements["\"AcceptButton\""];

            m_DescriptionScrollUpBtn = (UIButton)m_PResult.Elements["\"DescriptionScrollUpButton\""];
            m_DescriptionScrollUpBtn.OnButtonClicked += M_DescriptionScrollUpBtn_OnButtonClicked;
            m_DescriptionScrollUpBtn.Enabled = false;
            m_DescriptionScrollDownBtn = (UIButton)m_PResult.Elements["\"DescriptionScrollDownButton\""];
            m_DescriptionScrollDownBtn.OnButtonClicked += M_DescriptionScrollDownBtn_OnButtonClicked;
            m_DescriptionScrollDownBtn.Enabled = false;

            m_ExitBtn = (UIButton)m_PResult.Elements["\"ExitButton\""];
            m_ExitBtn.OnButtonClicked += M_ExitBtn_OnButtonClicked;

            m_FemaleBtn = (UIButton)m_PResult.Elements["\"FemaleButton\""];
            m_FemaleBtn.OnButtonClicked += M_FemaleBtn_OnButtonClicked;

            m_MaleBtn = (UIButton)m_PResult.Elements["\"MaleButton\""];
            m_MaleBtn.OnButtonClicked += M_MaleBtn_OnButtonClicked;

            m_SkinLightBtn = (UIButton)m_PResult.Elements["\"SkinLightButton\""];
            m_SkinLightBtn.OnButtonClicked += M_SkinLightBtn_OnButtonClicked;

            m_SkinMediumBtn = (UIButton)m_PResult.Elements["\"SkinMediumButton\""];
            m_SkinMediumBtn.OnButtonClicked += M_SkinMediumBtn_OnButtonClicked;

            m_SkinDarkBtn = (UIButton)m_PResult.Elements["\"SkinDarkButton\""];
            m_SkinDarkBtn.OnButtonClicked += M_SkinDarkBtn_OnButtonClicked;

            m_HeadSkinBrowser = new UIHeadBrowser(this, m_PResult.Controls["\"HeadSkinBrowser\""], 1, AvatarSex.Female);
            m_HeadSkinBrowser.OnButtonClicked += M_HeadSkinBrowser_OnButtonClicked;
            m_HeadSkinBrowser.DrawOrder = (int)DrawOrderEnum.UI;
            RegisterElement(m_HeadSkinBrowser);
            m_BodySkinBrowser = new UIBodyBrowser(this, m_PResult.Controls["\"BodySkinBrowser\""], 1, AvatarSex.Female);
            m_BodySkinBrowser.OnButtonClicked += M_BodySkinBrowser_OnButtonClicked;
            m_BodySkinBrowser.DrawOrder = (int)DrawOrderEnum.UI;
            RegisterElement(m_BodySkinBrowser);

            AdultAvatar Avatar = new AdultAvatar(Manager.Device, Manager.HeadShader);
            Avatar.ChangeOutfit(FileManager.Instance.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fab001_sl__pjs4), Vitaboy.SkinType.Medium);
            Avatar.SetHead(FileManager.Instance.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fah002_mom), (Vitaboy.SkinType)m_CurrentSkinType);
            Avatar.ShouldRotate = true;

            m_Avatar = new Sim(Manager.Device, Manager.RenderCamera, Avatar);

            m_VitaboyScreen = new VitaboyScreen(Manager, new Vector2(0, 0), 
                new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight));
            m_VitaboyScreen.AddSim(m_Avatar);

            m_ExitDialog = new ExitDialog(this, new Vector2(250, 250), m_Walker,
                GlobalSettings.Default.StartupPath + "gamedata\\uiscripts\\exitdialog.uis");
            m_ExitDialog.Visible = false;
            RegisterElement(m_ExitDialog);

            m_NameTextEdit = new TextBox(new Rectangle(22, 52, 230, 18), 64, "", Manager.Device, 9,
                StandardTxtColor, StandardTxtColor, 30, this, false);
            m_NameTextEdit.Name = "NameTextEdit"; //This should be set for all UIElements that need to receive input.
            m_NameTextEdit.DrawOrder = (int)DrawOrderEnum.UI;
            m_DescriptionTextEdit = (UITextEdit)m_PResult.Elements["\"DescriptionTextEdit\""];
            m_DescriptionTextEdit.DrawOrder = (int)DrawOrderEnum.UI;

            KeyboardInput.Initialize(Manager, 500f, 20);

            HitVM.PlayEvent("bkground_createasim");

            Manager.AddScreen(m_VitaboyScreen);

            foreach (KeyValuePair<string, UIElement> KVP in m_PResult.Elements)
                RegisterElement(KVP.Value);
        }

        #region EventHandlers

        /// <summary>
        /// User tried to scroll the text in the description textbox down.
        /// </summary>
        /// <param name="Sender"></param>
        private void M_DescriptionScrollDownBtn_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            if (!m_DescriptionTextEdit.ScrollDown())
                m_DescriptionScrollDownBtn.Enabled = false;
        }

        /// <summary>
        /// User tried to scroll the text in the description textbox up.
        /// </summary>
        private void M_DescriptionScrollUpBtn_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            if (!m_DescriptionTextEdit.ScrollUp())
                m_DescriptionScrollUpBtn.Enabled = false;
        }

        /// <summary>
        /// User wanted to exit the application.
        /// </summary>
        private void M_ExitBtn_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_ExitDialog.Visible = true;
        }

        /// <summary>
        /// User wanted to change the sex of the avatar to male.
        /// </summary>
        private void M_MaleBtn_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_HeadSkinBrowser.Sex = AvatarSex.Male;
            m_BodySkinBrowser.Sex = AvatarSex.Male;

            m_Avatar.ChangeOutfit(FileManager.Instance.GetOutfit((ulong)FileIDs.OutfitsFileIDs.mab000_robin), (Vitaboy.SkinType)m_CurrentSkinType);
            m_Avatar.Head(FileManager.Instance.GetOutfit((ulong)FileIDs.OutfitsFileIDs.mah003_antony), (Vitaboy.SkinType)m_CurrentSkinType);
        }

        /// <summary>
        /// User wanted to change the sex of the avatar to female.
        /// </summary>
        private void M_FemaleBtn_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_HeadSkinBrowser.Sex = AvatarSex.Female;
            m_BodySkinBrowser.Sex = AvatarSex.Female;

            m_Avatar.ChangeOutfit(FileManager.Instance.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fab001_sl__pjs4), (Vitaboy.SkinType)m_CurrentSkinType);
            m_Avatar.Head(FileManager.Instance.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fah002_mom), (Vitaboy.SkinType)m_CurrentSkinType);
        }

        /// <summary>
        /// The player clicked the SkinLightButton.
        /// This causes the browsers' skin type to change to light.
        /// </summary>
        private void M_SkinLightBtn_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_HeadSkinBrowser.SkinType = 0;
            m_BodySkinBrowser.SkinType = 0;
            m_CurrentSkinType = 0;
        }

        /// <summary>
        /// The player clicked the SkinMediumButton.
        /// This causes the browsers' skin type to change to medium.
        /// </summary>
        private void M_SkinMediumBtn_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_HeadSkinBrowser.SkinType = 1;
            m_BodySkinBrowser.SkinType = 1;
            m_CurrentSkinType = 1;
        }

        /// <summary>
        /// The player clicked the SkinDarkButton.
        /// This causes the browsers' skin type to change to dark.
        /// </summary>
        private void M_SkinDarkBtn_OnButtonClicked(object Sender, ButtonClickEventArgs E)
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

            m_ExitDialog.Update(Input, GTime);

            m_NameTextEdit.Update(Input, GTime);

            if (m_DescriptionTextEdit.CanScrollUp())
                m_DescriptionScrollUpBtn.Enabled = true;
            else
                m_DescriptionScrollUpBtn.Enabled = false;

            if (m_DescriptionTextEdit.CanScrollDown())
                m_DescriptionScrollDownBtn.Enabled = true;
            else
                m_DescriptionScrollDownBtn.Enabled = false;

            m_Avatar.Update(GTime);

            base.Update(Input, GTime);
        }

        public override void Draw()
        {
            //Used to be SpriteSortMode.FrontToBack.
            m_SBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, 
                RasterizerState.CullCounterClockwise, null, Resolution.TransformationMatrix());

            base.Draw(); //Needs to be drawn first for the ExitDialog to be drawn correctly.

            m_NameTextEdit.Draw(m_SBatch, null);

            //Manager.Device.Clear(Color.Black);

            m_SBatch.End();
        }
    }
}
