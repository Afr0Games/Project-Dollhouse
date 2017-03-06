using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Files;
using Files.Vitaboy;
using Files.Manager;
using Gonzo;
using Gonzo.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Glide;
using Sound;

namespace GonzoTest
{
    public class LoadingScreen : UIScreen
    {
        private UIImage m_BackgroundImg;
        private Tweener m_Tween;
        private Stopwatch m_Stopwatch;
        private UILabel m_LblExtrudingTerrainWeb, m_LblCalculatingDomesticCoefficients, 
            m_LblReadjustingCareerLadder, m_LblAccessingMoneySupply, m_LblHackingTheSocialNetwork,
            m_LblDownloadingReticulatedSplines, m_LblAdjustingEmotionalWeights, 
            m_LblCalibratingPersonalityMatrix, m_LblSettingUpPersonfinder;
        private CaretSeparatedText m_Txt;
        private Song m_LoadingSong;

        public LoadingScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "LoadingScreen", 
            SBatch, new Vector2(0, 0), 
            new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight))
        {
            m_BackgroundImg = new UIImage(FileManager.GetTexture((ulong)FileIDs.UIFileIDs.setup, false), this);
            m_Txt = StringManager.StrTable(155);

            m_LblExtrudingTerrainWeb = new UILabel(m_Txt[5], 1, new Vector2(GlobalSettings.Default.ScreenWidth,
                GlobalSettings.Default.ScreenHeight - 24), new Vector2(12, 12),
                Color.FromNonPremultiplied(255, 249, 157, 255), 12, this);
            m_LblExtrudingTerrainWeb.Visible = false;

            m_LblCalculatingDomesticCoefficients = new UILabel(m_Txt[6], 1, new Vector2(GlobalSettings.Default.ScreenWidth, 
                GlobalSettings.Default.ScreenHeight - 24), new Vector2(12, 12), 
                Color.FromNonPremultiplied(255, 249, 157, 255), 12, this);
            m_LblCalculatingDomesticCoefficients.Visible = false;

            m_LblReadjustingCareerLadder = new UILabel(m_Txt[7], 1, new Vector2(GlobalSettings.Default.ScreenWidth, 
                GlobalSettings.Default.ScreenHeight - 24), new Vector2(12, 12), 
                Color.FromNonPremultiplied(255, 249, 157, 255), 12, this);
            m_LblReadjustingCareerLadder.Visible = false;

            m_LblAccessingMoneySupply = new UILabel(m_Txt[8], 1, new Vector2(GlobalSettings.Default.ScreenWidth, 
                GlobalSettings.Default.ScreenHeight - 24), new Vector2(12, 12), 
                Color.FromNonPremultiplied(255, 249, 157, 255), 12, this);
            m_LblAccessingMoneySupply.Visible = false;

            m_LblHackingTheSocialNetwork = new UILabel(m_Txt[9], 1, new Vector2(GlobalSettings.Default.ScreenWidth,
                GlobalSettings.Default.ScreenHeight - 24), new Vector2(12, 12),
                Color.FromNonPremultiplied(255, 249, 157, 255), 12, this);
            m_LblHackingTheSocialNetwork.Visible = false;

            m_LblDownloadingReticulatedSplines = new UILabel(m_Txt[10], 1, new Vector2(GlobalSettings.Default.ScreenWidth,
                GlobalSettings.Default.ScreenHeight - 24), new Vector2(12, 12),
                Color.FromNonPremultiplied(255, 249, 157, 255), 12, this);
            m_LblDownloadingReticulatedSplines.Visible = false;

            m_LblAdjustingEmotionalWeights = new UILabel(m_Txt[11], 1, new Vector2(GlobalSettings.Default.ScreenWidth, 
                GlobalSettings.Default.ScreenHeight - 24), new Vector2(12, 12), 
                Color.FromNonPremultiplied(255, 249, 157, 255), 12, this);
            m_LblAdjustingEmotionalWeights.Visible = false;

            m_LblCalibratingPersonalityMatrix = new UILabel(m_Txt[12], 1, new Vector2(GlobalSettings.Default.ScreenWidth,
                GlobalSettings.Default.ScreenHeight - 24), new Vector2(12, 12),
                Color.FromNonPremultiplied(255, 249, 157, 255), 12, this);
            m_LblCalibratingPersonalityMatrix.Visible = false;

            m_LblSettingUpPersonfinder = new UILabel(m_Txt[13], 1, new Vector2(GlobalSettings.Default.ScreenWidth, 
                GlobalSettings.Default.ScreenHeight - 24), new Vector2(12, 12), 
                Color.FromNonPremultiplied(255, 249, 157, 255), 12, this);
            m_LblSettingUpPersonfinder.Visible = false;

            HitVM.PlayEvent("bkground_load");

            Task LoadTask = new Task(new Action(CacheResources));
            LoadTask.Start();

            m_Stopwatch = new Stopwatch();
            m_Stopwatch.Start();

            m_Tween = new Tweener();
            m_Tween.Tween(m_LblExtrudingTerrainWeb, 
                new { XPosition = (float)GlobalSettings.Default.ScreenWidth / 2 }, 7);
        }

        private void CacheResources()
        {
            //Cache textures.

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skinbrowserarrowleft);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skinbrowserarrowright);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_headskinbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_bodyskinbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_acceptbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_arrowdownbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_arrowupbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_acceptbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_background);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_cancelbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_closebtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_descriptionslider);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_femalebtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_malebtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skinbrowserarrowleft);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skinbrowserarrowright);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skindarkbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skinlightbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skinmediumbtn);

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_background);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_cityhouseiconalpha);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_cityiconbusy);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_descriptionback);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_descriptiontab);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_descriptiontabbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_entertabbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_exitbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_helpback);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_iconsindents);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_icontab);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_scrollbar);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_scrollbarnotch);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_scrollbarthumb);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_simcreatebtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_simselectbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_tab2tabback);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_tabsback);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_whosonlineback);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_whosonlinetab);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_select_whosonlinetabbtn);

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint1);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint2);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint3);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint4);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint5);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint6);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint7);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint8);

            FileManager.GetTexture((ulong)FileIDs.TerrainFileIDs.gr);
            FileManager.GetTexture((ulong)FileIDs.TerrainFileIDs.rk);
            FileManager.GetTexture((ulong)FileIDs.TerrainFileIDs.sn);
            FileManager.GetTexture((ulong)FileIDs.TerrainFileIDs.wt);

            //TODO: Apply these as backgrounds on the appropriate dates.
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_holiday_setup_halloween);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_holiday_setup_paddys_day);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_holiday_setup_thanksgiving);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_holiday_setup_valentine);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_holiday_setup_xmas);

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.buttontiledialog);

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_creditsbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_creditsindent);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_proxycity);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_proxyhouse);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_templatecity);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_creditsbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_templatehouse);

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cityselector_cityicon);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cityselector_sortbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cityselector_thumbnailalpha);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cityselector_thumbnailbackground);

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.creditscreen_backbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.creditscreen_backbtnindent);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.creditscreen_background);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.creditscreen_exitbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.creditscreen_maxisbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.creditscreen_tsologo_english);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.creditscreen_will);

            OnFinishedExtrudingTerrainWeb();

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtnbackgroundtall);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtnbackground);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_backgroundtemplatetall, true);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_backgroundtemplate, true);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_dwnrightcorner_wbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_iconselectionbutton);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_menuiconback);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_okcheckbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_progressbarback);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_progressbarfront);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_textboxbackground);

            //Cache some meshes.

            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb001fafat_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb001fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb002fafat_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb002fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb003fafat_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb003fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb003faskn_01_pelvis_body);

            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb004fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb006fafat_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb007fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb008fafat_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb008fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb008faskn_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb009fafat_01_pelvis_body);

            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb009fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb009faskn_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb010fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb011fafat_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb011fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb011faskn_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb012fafat_01_pelvis_body);

            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb012fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb012faskn_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb013fafat_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb013fafit_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb017fafat_01_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb017fafit_retro_pelvis_body);
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.fabb021fafit_jennifer_pelvis_body);

            //Cache collections.

            List<Collection> Collections = new List<Collection>();
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male));
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female));
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternal_unisex));
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male_heads));
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female_heads));
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternalheads_unisex));
            OutfitContainer Container;

            OnFinishedCalculatingDomesticCoefficients();

            //Cache outfits and thumbnails.
            int ColCounter = 0;

            foreach (Collection Col in Collections)
            {
                switch (ColCounter)
                {
                    case 1:
                        OnFinishedReadjusticCareerLadder();
                        break;
                    case 2:
                        OnFinishedAccessingMoneySupply();
                        break;
                    case 3:
                        OnFinishedHackingTheSocialNetwork();
                        break;
                    case 4:
                        OnFinishedDownloadingReticulatedSplines();
                        break;
                    case 5:
                        OnFinishedAdjustingEmotionalWeights();
                        break;
                    case 6:
                        OnFinishedCalibratingPersonalityMatrix();
                        break;
                }

                foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                {
                    Container = new OutfitContainer(
                        FileManager.GetOutfit(FileManager.GetPurchasableOutfit(PO.UniqueID).OutfitID.UniqueID));
                    FileManager.GetTexture(Container.LightAppearance.ThumbnailID.UniqueID);
                    FileManager.GetTexture(Container.MediumAppearance.ThumbnailID.UniqueID);
                    FileManager.GetTexture(Container.DarkAppearance.ThumbnailID.UniqueID);
                }

                ColCounter++;
            }

            OnFinishedSettingUpPersonFinder();
        }

        private void OnFinishedExtrudingTerrainWeb()
        {
            m_Tween.Tween(m_LblExtrudingTerrainWeb, 
                new { XPosition = (0 - Font12px.MeasureString(m_LblExtrudingTerrainWeb.Caption).X) }, 7);
            m_Tween.Tween(m_LblCalculatingDomesticCoefficients, 
                new { XPosition = (float)GlobalSettings.Default.ScreenWidth / 2 -
                (Font12px.MeasureString(m_LblCalculatingDomesticCoefficients.Caption).X / 2) }, 7);

            m_LblCalculatingDomesticCoefficients.Visible = true;
        }

        private void OnFinishedCalculatingDomesticCoefficients()
        {
            m_Tween.Tween(m_LblCalculatingDomesticCoefficients, 
                new { XPosition = (0 - Font12px.MeasureString(m_LblCalculatingDomesticCoefficients.Caption).X) }, 10);
            m_Tween.Tween(m_LblReadjustingCareerLadder, new { XPosition = GlobalSettings.Default.ScreenWidth / 2 -
                (Font12px.MeasureString(m_LblReadjustingCareerLadder.Caption).X / 2) }, 10);

            m_LblExtrudingTerrainWeb.Visible = false;
            m_LblReadjustingCareerLadder.Visible = true;
        }

        public void OnFinishedReadjusticCareerLadder()
        {
            m_Tween.Tween(m_LblReadjustingCareerLadder, 
                new { XPosition = (0 - Font12px.MeasureString(m_LblReadjustingCareerLadder.Caption).X) }, 13);
            m_Tween.Tween(m_LblAccessingMoneySupply, new { XPosition = GlobalSettings.Default.ScreenWidth / 2 -
                (Font12px.MeasureString(m_LblAccessingMoneySupply.Caption).X / 2) }, 13);

            m_LblCalculatingDomesticCoefficients.Visible = false;
            m_LblAccessingMoneySupply.Visible = true;
        }

        public void OnFinishedAccessingMoneySupply()
        {
            m_Tween.Tween(m_LblAccessingMoneySupply, 
                new { XPosition = (0 - Font12px.MeasureString(m_LblAccessingMoneySupply.Caption).X) }, 16);
            m_Tween.Tween(m_LblHackingTheSocialNetwork, 
                new { XPosition = (float)GlobalSettings.Default.ScreenWidth / 2 -
                (Font12px.MeasureString(m_LblHackingTheSocialNetwork.Caption).X / 2) }, 16);

            m_LblAccessingMoneySupply.Visible = false;
            m_LblHackingTheSocialNetwork.Visible = true;
        }

        public void OnFinishedHackingTheSocialNetwork()
        {
            m_Tween.Tween(m_LblHackingTheSocialNetwork, 
                new { XPosition = (0 - Font12px.MeasureString(m_LblHackingTheSocialNetwork.Caption).X) }, 16);
            m_Tween.Tween(m_LblDownloadingReticulatedSplines, 
                new { XPosition = (float)GlobalSettings.Default.ScreenWidth / 2 -
                (Font12px.MeasureString(m_LblDownloadingReticulatedSplines.Caption).X / 2) }, 16);

            m_LblReadjustingCareerLadder.Visible = false;
            m_LblDownloadingReticulatedSplines.Visible = true;
        }

        private void OnFinishedDownloadingReticulatedSplines()
        {
            m_Tween.Tween(m_LblDownloadingReticulatedSplines,
                new { XPosition = (0 - Font12px.MeasureString(m_LblDownloadingReticulatedSplines.Caption).X) }, 30);
            m_Tween.Tween(m_LblAdjustingEmotionalWeights, 
                new { XPosition = (float)GlobalSettings.Default.ScreenWidth / 2 -
                (Font12px.MeasureString(m_LblAdjustingEmotionalWeights.Caption).X / 2) }, 30);

            m_LblHackingTheSocialNetwork.Visible = false;
            m_LblAdjustingEmotionalWeights.Visible = true;
        }

        private void OnFinishedAdjustingEmotionalWeights()
        {
            m_Tween.Tween(m_LblAdjustingEmotionalWeights, 
                new { XPosition = (0 - Font12px.MeasureString(m_LblAdjustingEmotionalWeights.Caption).X) }, 55);
            m_Tween.Tween(m_LblCalibratingPersonalityMatrix, 
                new { XPosition = (float)GlobalSettings.Default.ScreenWidth / 2 -
                (Font12px.MeasureString(m_LblCalibratingPersonalityMatrix.Caption).X / 2) }, 55);

            m_LblDownloadingReticulatedSplines.Visible = false;
            m_LblCalibratingPersonalityMatrix.Visible = true;
        }

        private void OnFinishedCalibratingPersonalityMatrix()
        {
            m_Tween.Tween(m_LblCalibratingPersonalityMatrix, 
                new { XPosition = (0 - Font12px.MeasureString(m_LblCalibratingPersonalityMatrix.Caption).X) }, 55);
            m_Tween.Tween(m_LblSettingUpPersonfinder, 
                new { XPosition = (float)GlobalSettings.Default.ScreenWidth / 2 - 
                (Font12px.MeasureString(m_LblSettingUpPersonfinder.Caption).X / 2) }, 55);

            m_LblAdjustingEmotionalWeights.Visible = false;
            m_LblSettingUpPersonfinder.Visible = true;
        }

        private void OnFinishedSettingUpPersonFinder()
        {
            m_LblCalibratingPersonalityMatrix.Visible = false;
        }

        public override void Update(InputHelper Input, GameTime GTime)
        {
            base.Update(Input, GTime);

            m_Tween.Update(m_Stopwatch.Elapsed.Seconds);
        }

        public override void Draw()
        {
            base.Draw();

            m_BackgroundImg.Draw(m_SBatch, null, 0.0f);

            m_LblExtrudingTerrainWeb.Draw(m_SBatch, 0.3f);
            m_LblCalculatingDomesticCoefficients.Draw(m_SBatch, 0.3f);
            m_LblReadjustingCareerLadder.Draw(m_SBatch, 0.3f);
            m_LblAccessingMoneySupply.Draw(m_SBatch, 0.3f);
            m_LblHackingTheSocialNetwork.Draw(m_SBatch, 0.3f);
            m_LblDownloadingReticulatedSplines.Draw(m_SBatch, 0.3f);
            m_LblAdjustingEmotionalWeights.Draw(m_SBatch, 0.3f);
            m_LblCalibratingPersonalityMatrix.Draw(m_SBatch, 0.3f);
            m_LblSettingUpPersonfinder.Draw(m_SBatch, 0.3f);
        }
    }
}
