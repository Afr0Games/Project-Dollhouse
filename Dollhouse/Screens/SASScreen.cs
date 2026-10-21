using System.Collections.Generic;
using Shared;
using UI;
using UI.Elements;
using UI.Dialogs;
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
        //All variables that exists in the script shares their name between the script and the code!
        private UIBackgroundImage m_BackgroundImg;
        private UIImage TabBackgroundImg1, TabBackgroundImg2, TabBackgroundImg3, DescriptionTabBackgroundImg1, DescriptionTabBackgroundImg2, DescriptionTabBackgroundImg3, 
            SimCreateButtonImage, SimSelectButtonImage, DescriptionTabImage1, DescriptionTabImage2, DescriptionTabImage3, EnterTabImage1, EnterTabImage2, EnterTabImage3, 
            EnterTabBackgroundImage1, EnterTabBackgroundImage2, EnterTabBackgroundImage3, /*DefaultHouseImg,*/ CreditsBackgroundImg,/*, CityThumbnailBusyImg*/
            CityButtonTemplateImage;
        private UIButton m_ExitButton, m_EnterTabBtn1, m_EnterTabBtn2, m_EnterTabBtn3, m_DescriptionTabBtn1, 
            m_DescriptionTabBtn2, m_DescriptionTabBtn3, m_AvatarButton1, m_AvatarButton2, m_AvatarButton3, m_CityButton1, 
            m_CityButton2, m_CityButton3, m_HouseButton1, m_HouseButton2, m_HouseButton3, m_NewAvatarButton1, m_NewAvatarButton2,
            m_NewAvatarButton3, m_DeleteAvatarButton1, m_DeleteAvatarButton2, m_DeleteAvatarButton3, m_PersonDescriptionScrollUpBtn1, 
            m_PersonDescriptionScrollUpBtn2, m_PersonDescriptionScrollUpBtn3, m_PersonDescriptionScrollDownBtn1, 
            m_PersonDescriptionScrollDownBtn2, m_PersonDescriptionScrollDownBtn3, m_CreditsButton;
        private ExitDialog m_ExitDialog;

        private UITextEdit m_PersonDescriptionText1, m_PersonDescriptionText2, m_PersonDescriptionText3;

        private UILabel m_TitleLabel, m_CityNameText1, m_CityNameText2, m_CityNameText3,
            m_HouseNameText1, m_HouseNameText2, m_HouseNameText3,
            m_PersonNameText1, m_PersonNameText2, m_PersonNameText3;
        private UIButton m_FamilyButton;
        private UILabel m_FamilyButtonText;

        //TODO: Move this to GameFacade...
        private List<Sim> m_Avatars = new List<Sim>();

        public SASScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "SAS", SBatch,
            new Vector2(0, 0), new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight),
            GlobalSettings.Default.StartupPath + "\\" + "gamedata\\uiscripts\\personselection.uis")
        {
            m_BackgroundImg = (UIBackgroundImage)m_PResult.Elements["\"BackgroundImage\""];
            Children.Add(m_BackgroundImg);

            m_TitleLabel = (UILabel)m_PResult.Elements["\"TitleLabel\""];
            Children.Add(m_TitleLabel);

            //The .uis defines one avatar name per slot, but the original code
            //never registered these labels, making every name invisible.
            m_PersonNameText1 = (UILabel)m_PResult.Elements["\"PersonNameText1\""];
            m_PersonNameText2 = (UILabel)m_PResult.Elements["\"PersonNameText2\""];
            m_PersonNameText3 = (UILabel)m_PResult.Elements["\"PersonNameText3\""];
            Children.Add(m_PersonNameText1);
            Children.Add(m_PersonNameText2);
            Children.Add(m_PersonNameText3);

            AdultAvatar Avatar = new AdultAvatar(Manager.Device, Manager.HeadShader);
            Avatar.ChangeOutfit(FileManager.Instance.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fab001_sl__pjs4), Vitaboy.SkinType.Medium);
            Avatar.SetHead(FileManager.Instance.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fah002_mom), Vitaboy.SkinType.Medium);
            AddAvatar(Avatar, 1);

            VitaboyScreen VScreen = new VitaboyScreen(Manager, new Vector2(0, 0),
                new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight));
            VScreen.AddSim(m_Avatars[0]);

            Manager.AddScreen(VScreen);

            TabBackgroundImg1 = GetVisibleScriptImage("\"TabBackgroundImage1\"");
            TabBackgroundImg2 = GetVisibleScriptImage("\"TabBackgroundImage2\"");
            TabBackgroundImg3 = GetVisibleScriptImage("\"TabBackgroundImage3\"");
            //SetControlProperties already positions these images via TreeWalker.
            //They must also be in Children or they never get drawn.
            TabBackgroundImg1.Visible = m_Avatars.Count >= 1;
            TabBackgroundImg2.Visible = m_Avatars.Count >= 2;
            TabBackgroundImg3.Visible = m_Avatars.Count >= 3;
            Children.Add(TabBackgroundImg1);
            Children.Add(TabBackgroundImg2);
            Children.Add(TabBackgroundImg3);

            SimCreateButtonImage = (UIImage)m_PResult.Elements["\"SimCreateButtonImage\""];
            SimSelectButtonImage = (UIImage)m_PResult.Elements["\"SimSelectButtonImage\""];

            DescriptionTabBackgroundImg1 = GetVisibleScriptImage("\"DescriptionTabBackgroundImage1\"");
            DescriptionTabBackgroundImg1.Visible = m_Avatars.Count >= 1 ? true : false;
            DescriptionTabBackgroundImg2 = GetVisibleScriptImage("\"DescriptionTabBackgroundImage2\"");
            DescriptionTabBackgroundImg2.Visible = m_Avatars.Count >= 2 ? true : false;
            DescriptionTabBackgroundImg3 = GetVisibleScriptImage("\"DescriptionTabBackgroundImage3\"");
            DescriptionTabBackgroundImg3.Visible = m_Avatars.Count == 3 ? true : false;
            Children.Add(DescriptionTabBackgroundImg1);
            Children.Add(DescriptionTabBackgroundImg2);
            Children.Add(DescriptionTabBackgroundImg3);

            DescriptionTabImage1 = GetVisibleScriptImage("\"DescriptionTabImage1\"");
            DescriptionTabImage1.Visible = m_Avatars.Count >= 1 ? true : false;
            DescriptionTabImage2 = GetVisibleScriptImage("\"DescriptionTabImage2\"");
            DescriptionTabImage2.Visible = m_Avatars.Count >= 2 ? true : false;
            DescriptionTabImage3 = GetVisibleScriptImage("\"DescriptionTabImage3\"");
            DescriptionTabImage3.Visible = m_Avatars.Count == 3 ? true : false;
            Children.Add(DescriptionTabImage1);
            Children.Add(DescriptionTabImage2);
            Children.Add(DescriptionTabImage3);

            EnterTabImage1 = GetVisibleScriptImage("\"EnterTabImage1\"");
            EnterTabImage1.Visible = false;
            EnterTabImage2 = GetVisibleScriptImage("\"EnterTabImage2\"");
            EnterTabImage2.Visible = false;
            EnterTabImage3 = GetVisibleScriptImage("\"EnterTabImage3\"");
            EnterTabImage3.Visible = false;
            Children.Add(EnterTabImage1);
            Children.Add(EnterTabImage2);
            Children.Add(EnterTabImage3);

            EnterTabBackgroundImage1 = GetVisibleScriptImage("\"EnterTabBackgroundImage1\"");
            EnterTabBackgroundImage1.Visible = false;
            EnterTabBackgroundImage2 = GetVisibleScriptImage("\"EnterTabBackgroundImage2\"");
            EnterTabBackgroundImage2.Visible = false;
            EnterTabBackgroundImage3 = GetVisibleScriptImage("\"EnterTabBackgroundImage3\"");
            EnterTabBackgroundImage3.Visible = false;
            Children.Add(EnterTabBackgroundImage1);
            Children.Add(EnterTabBackgroundImage2);
            Children.Add(EnterTabBackgroundImage3);

            //DefaultHouseImg = (UIImage)m_Elements["\"DefaultHouseImage\""].Image; //Not used??

            //Standalone SetControlProperties creates a UIControl, not an
            //entry in Elements. Its image is not automatically positioned.
            var creditsIndent = m_PResult.Controls["\"CreditsButtonBackgroundImage\""];
            CreditsBackgroundImg = new UIImage(creditsIndent.Image.Texture,
                creditsIndent.Position, this);
            Children.Add(CreditsBackgroundImg);
            //CityThumbnailBusyImg = m_Controls["\"CityThumbnailBusyImage\""].Image; //Not used??

            CityButtonTemplateImage = (UIImage)m_PResult.Elements["\"CityButtonTemplateImage\""];

            m_ExitButton = (UIButton)m_PResult.Elements["\"ExitButton\""];
            m_ExitButton.OnButtonClicked += M_ExitButton_OnButtonClicked;
            Children.Add(m_ExitButton);
            m_ExitDialog = new ExitDialog(this, new Vector2(250, 250), m_Walker, 
                GlobalSettings.Default.StartupPath + "gamedata\\uiscripts\\exitdialog.uis");
            m_ExitDialog.Visible = false;
            Children.Add(m_ExitDialog);

            m_EnterTabBtn1 = (UIButton)m_PResult.Elements["\"EnterTabButton1\""];
            m_EnterTabBtn1.OnButtonClicked += EnterTabBtn1_OnButtonClicked;
            m_EnterTabBtn1.Enabled = m_Avatars.Count >= 1 ? true : false;
            Children.Add(m_EnterTabBtn1);
            m_EnterTabBtn2 = (UIButton)m_PResult.Elements["\"EnterTabButton2\""];
            m_EnterTabBtn2.OnButtonClicked += EnterTabBtn2_OnButtonClicked;
            m_EnterTabBtn2.Enabled = m_Avatars.Count >= 2 ? true : false;
            Children.Add(m_EnterTabBtn2);
            m_EnterTabBtn3 = (UIButton)m_PResult.Elements["\"EnterTabButton3\""];
            m_EnterTabBtn3.OnButtonClicked += EnterTabBtn3_OnButtonClicked;
            m_EnterTabBtn3.Enabled = m_Avatars.Count == 3 ? true : false;
            Children.Add(m_EnterTabBtn3);

            m_DescriptionTabBtn1 = (UIButton)m_PResult.Elements["\"DescriptionTabButton1\""];
            m_DescriptionTabBtn1.OnButtonClicked += DescriptionTabBtn1_OnButtonClicked;
            m_DescriptionTabBtn1.Enabled = m_Avatars.Count >= 1 ? true : false;
            Children.Add(m_DescriptionTabBtn1);
            m_DescriptionTabBtn2 = (UIButton)m_PResult.Elements["\"DescriptionTabButton2\""];
            m_DescriptionTabBtn2.OnButtonClicked += DescriptionTabBtn2_OnButtonClicked;
            m_DescriptionTabBtn2.Enabled = m_Avatars.Count >= 2 ? true : false;
            Children.Add(m_DescriptionTabBtn2);
            m_DescriptionTabBtn3 = (UIButton)m_PResult.Elements["\"DescriptionTabButton3\""];
            m_DescriptionTabBtn3.OnButtonClicked += DescriptionTabBtn3_OnButtonClicked;
            m_DescriptionTabBtn3.Enabled = m_Avatars.Count == 3 ? true : false;
            Children.Add(m_DescriptionTabBtn3);

            //These controls are transparent portrait hit areas in the original
            //script (MouseTransparent=1). The generic UIButton constructor gives
            //each one a fallback blue capsule, which should NOT be rendered.
            //Retain the UIButton instances for input/event compatibility.
            m_AvatarButton1 = (UIButton)m_PResult.Elements["\"AvatarButton1\""];
            //The occupied slot renders its actual 3D Sim rather than an empty portrait.
            m_AvatarButton1.Image.Visible = false;
            Children.Add(m_AvatarButton1);

            m_AvatarButton2 = (UIButton)m_PResult.Elements["\"AvatarButton2\""];
            //SimCreateButtonImage is the four-frame EMPTY PORTRAIT image.
            //It belongs in the avatar hit area (script Y=80), not the
            //bottom Create-a-Sim button (script Y=536).
            m_AvatarButton2.AddImage(new UIImage(SimCreateButtonImage.Texture, Vector2.Zero, this));
            m_AvatarButton2.Image.Visible = m_Avatars.Count < 2;
            Children.Add(m_AvatarButton2);

            m_AvatarButton3 = (UIButton)m_PResult.Elements["\"AvatarButton3\""];
            m_AvatarButton3.AddImage(new UIImage(SimCreateButtonImage.Texture, Vector2.Zero, this));
            m_AvatarButton3.Image.Visible = m_Avatars.Count < 3;
            Children.Add(m_AvatarButton3);

            m_CityButton1 = (UIButton)m_PResult.Elements["\"CityButton1\""];
            m_CityButton1.AddImage(new UIImage(GetImage("\"CityButtonTemplateImage\"").Texture, Vector2.Zero, this));
            m_CityButton1.Visible = false;
            Children.Add(m_CityButton1);
            m_CityButton2 = (UIButton)m_PResult.Elements["\"CityButton2\""];
            m_CityButton2.AddImage(new UIImage(GetImage("\"CityButtonTemplateImage\"").Texture, Vector2.Zero, this));
            m_CityButton2.Visible = false;
            Children.Add(m_CityButton2);
            m_CityButton3 = (UIButton)m_PResult.Elements["\"CityButton3\""];
            m_CityButton3.AddImage(new UIImage(GetImage("\"CityButtonTemplateImage\"").Texture, Vector2.Zero, this));
            m_CityButton3.Visible = false;
            Children.Add(m_CityButton3);

            m_HouseButton1 = (UIButton)m_PResult.Elements["\"HouseButton1\""];
            m_HouseButton1.AddImage(new UIImage(GetImage("\"HouseButtonTemplateImage\"").Texture, Vector2.Zero, this));
            m_HouseButton1.Visible = false;
            Children.Add(m_HouseButton1);
            m_HouseButton2 = (UIButton)m_PResult.Elements["\"HouseButton2\""];
            m_HouseButton2.AddImage(new UIImage(GetImage("\"HouseButtonTemplateImage\"").Texture, Vector2.Zero, this));
            m_HouseButton2.Visible = false;
            Children.Add(m_HouseButton2);
            m_HouseButton3 = (UIButton)m_PResult.Elements["\"HouseButton3\""];
            m_HouseButton3.AddImage(new UIImage(GetImage("\"HouseButtonTemplateImage\"").Texture, Vector2.Zero, this));
            m_HouseButton3.Visible = false;
            Children.Add(m_HouseButton3);

            //These are the SMALL caption buttons at Y=536, distinct from the
            //EMPTY PORTRAIT graphics drawn at Y=80 above. Keep their existing
            //UIButton fallback art instead of binding the large portrait sheet.
            m_NewAvatarButton1 = (UIButton)m_PResult.Elements["\"NewAvatarButton1\""];
            m_NewAvatarButton1.Visible = m_Avatars.Count < 1;
            Children.Add(m_NewAvatarButton1);
            m_NewAvatarButton2 = (UIButton)m_PResult.Elements["\"NewAvatarButton2\""];
            m_NewAvatarButton2.Visible = m_Avatars.Count < 2;
            Children.Add(m_NewAvatarButton2);
            m_NewAvatarButton3 = (UIButton)m_PResult.Elements["\"NewAvatarButton3\""];
            m_NewAvatarButton3.Visible = m_Avatars.Count < 3;
            Children.Add(m_NewAvatarButton3);

            m_DeleteAvatarButton1 = (UIButton)m_PResult.Elements["\"DeleteAvatarButton1\""];
            //No delete action/dialog is implemented yet; don't display a
            //generic, nonfunctional button in the occupied avatar slot.
            m_DeleteAvatarButton1.Visible = false;
            Children.Add(m_DeleteAvatarButton1);
            m_DeleteAvatarButton2 = (UIButton)m_PResult.Elements["\"DeleteAvatarButton2\""];
            m_DeleteAvatarButton2.Visible = false;
            Children.Add(m_DeleteAvatarButton2);
            m_DeleteAvatarButton3 = (UIButton)m_PResult.Elements["\"DeleteAvatarButton3\""];
            m_DeleteAvatarButton3.Visible = false;
            Children.Add(m_DeleteAvatarButton3);

            m_PersonDescriptionText1 = (UITextEdit)m_PResult.Elements["\"PersonDescriptionText1\""];
            m_PersonDescriptionText1.Visible = m_Avatars.Count >= 1 ? true : false;
            Children.Add(m_PersonDescriptionText1);
            m_PersonDescriptionText2 = (UITextEdit)m_PResult.Elements["\"PersonDescriptionText2\""];
            m_PersonDescriptionText2.Visible = m_Avatars.Count >= 2 ? true : false;
            Children.Add(m_PersonDescriptionText2);
            m_PersonDescriptionText3 = (UITextEdit)m_PResult.Elements["\"PersonDescriptionText3\""];
            m_PersonDescriptionText3.Visible = m_Avatars.Count == 3 ? true : false;
            Children.Add(m_PersonDescriptionText3);

            m_PersonDescriptionScrollUpBtn1 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollUpButton1\""];
            m_PersonDescriptionScrollUpBtn1.Visible = m_PersonDescriptionText1.Visible ? true : false;
            Children.Add(m_PersonDescriptionScrollUpBtn1);
            m_PersonDescriptionScrollUpBtn2 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollUpButton2\""];
            m_PersonDescriptionScrollUpBtn2.Visible = m_PersonDescriptionText2.Visible ? true : false;
            Children.Add(m_PersonDescriptionScrollUpBtn2);
            m_PersonDescriptionScrollUpBtn3 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollUpButton3\""];
            m_PersonDescriptionScrollUpBtn3.Visible = m_PersonDescriptionText3.Visible ? true : false;
            Children.Add(m_PersonDescriptionScrollUpBtn3);

            m_PersonDescriptionScrollDownBtn1 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollDownButton1\""];
            m_PersonDescriptionScrollDownBtn1.Visible = m_PersonDescriptionText1.Visible ? true : false;
            Children.Add(m_PersonDescriptionScrollDownBtn1);
            m_PersonDescriptionScrollDownBtn2 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollDownButton2\""];
            m_PersonDescriptionScrollDownBtn2.Visible = m_PersonDescriptionText2.Visible ? true : false;
            Children.Add(m_PersonDescriptionScrollDownBtn2);
            m_PersonDescriptionScrollDownBtn3 = (UIButton)m_PResult.Elements["\"PersonDescriptionScrollDownButton3\""];
            m_PersonDescriptionScrollDownBtn3.Visible = m_PersonDescriptionText3.Visible ? true : false;
            Children.Add(m_PersonDescriptionScrollDownBtn3);

            m_CityNameText1 = (UILabel)m_PResult.Elements["\"CityNameText1\""];
            m_CityNameText1.Visible = false;
            Children.Add(m_CityNameText1);
            m_CityNameText2 = (UILabel)m_PResult.Elements["\"CityNameText2\""];
            m_CityNameText2.Visible = false;
            Children.Add(m_CityNameText2);
            m_CityNameText3 = (UILabel)m_PResult.Elements["\"CityNameText3\""];
            m_CityNameText3.Visible = false;
            Children.Add(m_CityNameText3);

            //The .uis uses SelectAvatarTooltip as a placeholder for these
            //dynamic labels. Clear it until we receive actual city records.
            m_CityNameText1.Caption = string.Empty;
            m_CityNameText2.Caption = string.Empty;
            m_CityNameText3.Caption = string.Empty;

            //TODO: Assign captions to these if a sim has a house...
            m_HouseNameText1 = (UILabel)m_PResult.Elements["\"HouseNameText1\""];
            m_HouseNameText1.Visible = false;
            Children.Add(m_HouseNameText1);
            m_HouseNameText2 = (UILabel)m_PResult.Elements["\"HouseNameText2\""];
            m_HouseNameText2.Visible = false;
            Children.Add(m_HouseNameText2);
            m_HouseNameText3 = (UILabel)m_PResult.Elements["\"HouseNameText3\""];
            m_HouseNameText3.Visible = false;
            Children.Add(m_HouseNameText3);

            m_CreditsButton = (UIButton)m_PResult.Elements["\"CreditsButton\""];
            Children.Add(m_CreditsButton);

            //Free Will is declared in personselection.uis, but the previous
            //screen didn't register it. Its script uses textButton=0, so add a
            //centered label without modifying the shared button/parser logic.
            m_FamilyButton = (UIButton)m_PResult.Elements["\"FamilyButton\""];
            Children.Add(m_FamilyButton);
            m_FamilyButtonText = new UILabel(GetString("\"FamilyButtonText\""), 47,
                new Vector2(644f, 11f), new Vector2(75f, 25f), StandardTxtColor,
                10, this, null, UIParser.Nodes.TextAlignment.Center_Center);
            Children.Add(m_FamilyButtonText);

            //Prototype currently has one locally-created Sim, with no city DB.
            //Use an explicit test name, not a fabricated city/lot assignment.
            SetAvatarName(1, "Test Sim");
            InitializeOccupiedSlotTabs();

            //Keep modal dialogs above controls rather than behind them.
            m_ExitDialog.ZIndex = (int)DrawOrderEnum.MessageBoxes;
            HitVM.PlayEvent("bkground_selectasim");
        }

        /// <summary>
        /// The shared UIImage copy constructor currently drops its alpha value.
        /// A copied UIControl image is therefore fully transparent. Recreate
        /// only this screen's scripted overlays with the original texture and
        /// coordinates, keeping the shared UI renderer unchanged.
        /// </summary>
        private UIImage GetVisibleScriptImage(string scriptName)
        {
            UIImage image = m_PResult.Elements[scriptName].Image;
            return new UIImage(image.Texture, image.Position, this);
        }

        /// <summary>
        /// Apply an avatar's name when account/city data becomes available.
        /// The screen does not invent server-side avatar identity.
        /// </summary>
        public void SetAvatarName(int slot, string name)
        {
            UILabel label = slot switch
            {
                1 => m_PersonNameText1,
                2 => m_PersonNameText2,
                3 => m_PersonNameText3,
                _ => throw new System.ArgumentOutOfRangeException(nameof(slot))
            };

            label.Caption = name ?? string.Empty;
            //UILabel aligns only once in its constructor. Recenter updated
            //runtime captions within the original 235x17 scripted name box.
            float NameCenterX = (slot == 1 ? 22f : slot == 2 ? 282f : 542f) + 117.5f;
            Vector2 NameSize = Font9px.MeasureString(label.Caption);
            label.Position = new Vector2(NameCenterX - NameSize.X / 2f,
                52f + (17f - NameSize.Y) / 2f);
            label.Visible = slot <= m_Avatars.Count;
        }

        /// <summary>
        /// Supply optional, real city/house labels when those records are
        /// available. Passing null/empty keeps them hidden.
        /// </summary>
        public void SetAvatarLocation(int Slot, string CityName, string HouseName = null)
        {
            UILabel City = Slot switch
            {
                1 => m_CityNameText1,
                2 => m_CityNameText2,
                3 => m_CityNameText3,
                _ => throw new System.ArgumentOutOfRangeException(nameof(Slot))
            };

            UILabel House = Slot switch
            {
                1 => m_HouseNameText1,
                2 => m_HouseNameText2,
                3 => m_HouseNameText3,
                _ => throw new System.ArgumentOutOfRangeException(nameof(Slot))
            };

            City.Caption = CityName ?? string.Empty;
            House.Caption = HouseName ?? string.Empty;

            //Re-align captions loaded after script parsing (see UILabel).
            float LabelX = (Slot == 1 ? 135f : Slot == 2 ? 396f : 656f) + 60f;
            Vector2 CityTextSize = Font9px.MeasureString(City.Caption);
            Vector2 HouseTextSize = Font9px.MeasureString(House.Caption);

            City.Position = new Vector2(LabelX - CityTextSize.X / 2f, 382f + (60f - CityTextSize.Y) / 2f);
            House.Position = new Vector2(LabelX - HouseTextSize.X / 2f, 472f + (60f - HouseTextSize.Y) / 2f);
            //Only the Enter tab displays location information.
            bool ShowingEnter = Slot == 1 ? EnterTabImage1.Visible : Slot == 2 ? EnterTabImage2.Visible : EnterTabImage3.Visible;
            City.Visible = ShowingEnter && !string.IsNullOrWhiteSpace(CityName);
            House.Visible = ShowingEnter && !string.IsNullOrWhiteSpace(HouseName);
        }

        /// <summary>
        /// Existing Sims default to the city/house tab, as in the original
        /// Select-a-Sim screen. Description text and scrollbars remain available
        /// via the existing tab-button handlers.
        /// </summary>
        private void InitializeOccupiedSlotTabs()
        {
            bool first = m_Avatars.Count >= 1;
            bool second = m_Avatars.Count >= 2;
            bool third = m_Avatars.Count >= 3;

            EnterTabImage1.Visible = EnterTabBackgroundImage1.Visible = first;
            EnterTabImage2.Visible = EnterTabBackgroundImage2.Visible = second;
            EnterTabImage3.Visible = EnterTabBackgroundImage3.Visible = third;

            DescriptionTabImage1.Visible = DescriptionTabBackgroundImg1.Visible = false;
            DescriptionTabImage2.Visible = DescriptionTabBackgroundImg2.Visible = false;
            DescriptionTabImage3.Visible = DescriptionTabBackgroundImg3.Visible = false;

            m_PersonDescriptionText1.Visible = m_PersonDescriptionScrollUpBtn1.Visible =
                m_PersonDescriptionScrollDownBtn1.Visible = false;
            m_PersonDescriptionText2.Visible = m_PersonDescriptionScrollUpBtn2.Visible =
                m_PersonDescriptionScrollDownBtn2.Visible = false;
            m_PersonDescriptionText3.Visible = m_PersonDescriptionScrollUpBtn3.Visible =
                m_PersonDescriptionScrollDownBtn3.Visible = false;

            m_CityButton1.Visible = first;
            m_CityButton2.Visible = second;
            m_CityButton3.Visible = third;
            m_HouseButton1.Visible = first && m_Avatars[0].HasHouse;
            m_HouseButton2.Visible = second && m_Avatars[1].HasHouse;
            m_HouseButton3.Visible = third && m_Avatars[2].HasHouse;

            m_CityNameText1.Visible = first && !string.IsNullOrWhiteSpace(m_CityNameText1.Caption);
            m_CityNameText2.Visible = second && !string.IsNullOrWhiteSpace(m_CityNameText2.Caption);
            m_CityNameText3.Visible = third && !string.IsNullOrWhiteSpace(m_CityNameText3.Caption);
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
            m_HouseNameText3.Visible = false;

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
            m_HouseNameText2.Visible = false;

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
            m_HouseNameText1.Visible = false;

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
                m_CityNameText3.Visible = !string.IsNullOrWhiteSpace(m_CityNameText3.Caption);
                m_HouseNameText3.Visible = m_Avatars[2].HasHouse &&
                    !string.IsNullOrWhiteSpace(m_HouseNameText3.Caption);
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
                m_CityNameText2.Visible = !string.IsNullOrWhiteSpace(m_CityNameText2.Caption);
                m_HouseNameText2.Visible = m_Avatars[1].HasHouse &&
                    !string.IsNullOrWhiteSpace(m_HouseNameText2.Caption);
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
                m_CityNameText1.Visible = !string.IsNullOrWhiteSpace(m_CityNameText1.Caption);
                m_HouseNameText1.Visible = m_Avatars[0].HasHouse &&
                    !string.IsNullOrWhiteSpace(m_HouseNameText1.Caption);
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
        }

        public override void Draw()
        {
            //Used to be SpriteSortMode.FrontToBack.
            m_SBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, 
                RasterizerState.CullCounterClockwise, null, Resolution.TransformationMatrix());

            base.Draw();

            m_SBatch.End();
        }
    }
}
