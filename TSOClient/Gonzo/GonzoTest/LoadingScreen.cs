﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Files;
using Files.Vitaboy;
using Files.Manager;
using Gonzo;
using Gonzo.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GonzoTest
{
    public class LoadingScreen : UIScreen
    {
        private UIImage m_BackgroundImg;
        private UILabel m_LblExtrudingTerrainWeb, m_LblCalculatingDomesticCoefficients, 
            m_LblReadjustingCareerLadder, m_LblAccessingMoneySupply, m_LblHackingTheSocialNetwork,
            m_LblDownloadingReticulatedSplines, m_LblAdjustingEmotionalWeights, 
            m_LblCalibratingPersonalityMatrix, m_LblSettingUpPersonfinder;
        private CaretSeparatedText m_Txt;

        public LoadingScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "LoadingScreen", 
            SBatch, new Vector2(0, 0), 
            new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight))
        {
            m_BackgroundImg = new UIImage(FileManager.GetTexture((ulong)FileIDs.UIFileIDs.setup, false), this);
            m_Txt = StringManager.StrTable(155);

            Task LoadTask = new Task(new Action(CacheResources));
            LoadTask.Start();
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

            OnFinishedExtrudingTerrainWeb();

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

            OnFinishedCalculatingDomesticCoefficients();

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint1);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint2);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint3);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint4);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint5);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint6);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint7);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_hints_hint8);

            OnFinishedReadjusticCareerLadder();

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_holiday_setup_halloween);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_holiday_setup_paddys_day);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_holiday_setup_thanksgiving);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_holiday_setup_valentine);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.uigraphics_holiday_setup_xmas);

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.buttontiledialog);

            OnFinishedAccessingMoneySupply();

            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_creditsbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_creditsindent);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_proxycity);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_proxyhouse);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_templatecity);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_creditsbtn);
            FileManager.GetTexture((ulong)FileIDs.UIFileIDs.cas_sas_templatehouse);

            OnFinishedHackingTheSocialNetwork();

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

            OnFinishedDownloadingReticulatedSplines();

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

            OnFinishedAdjustingEmotionalWeights();

            //Cache collections.

            List<Collection> Collections = new List<Collection>();
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male));
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female));
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternal_unisex));
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male_heads));
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female_heads));
            Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternalheads_unisex));
            OutfitContainer Container;
            
            OnFinishedCalibratingPersonalityMatrix();

            //Cache outfits and thumbnails.

            foreach (Collection Col in Collections)
            {
                foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                {
                    Container = new OutfitContainer(
                        FileManager.GetOutfit(FileManager.GetPurchasableOutfit(PO.UniqueID).OutfitID.UniqueID));
                    FileManager.GetTexture(Container.LightAppearance.ThumbnailID.UniqueID);
                    FileManager.GetTexture(Container.MediumAppearance.ThumbnailID.UniqueID);
                    FileManager.GetTexture(Container.DarkAppearance.ThumbnailID.UniqueID);
                }
            }

            OnFinishedSettingUpPersonFinder();
        }

        private void OnFinishedExtrudingTerrainWeb()
        {

        }

        private void OnFinishedCalculatingDomesticCoefficients()
        {

        }

        public void OnFinishedReadjusticCareerLadder()
        {

        }

        public void OnFinishedAccessingMoneySupply()
        {

        }

        public void OnFinishedHackingTheSocialNetwork()
        {

        }

        private void OnFinishedDownloadingReticulatedSplines()
        {

        }

        private void OnFinishedAdjustingEmotionalWeights()
        {

        }

        private void OnFinishedCalibratingPersonalityMatrix()
        {

        }

        private void OnFinishedSettingUpPersonFinder()
        {

        }

        public override void Draw()
        {
            base.Draw();

            m_BackgroundImg.Draw(m_SBatch, null, 0.0f);
        }
    }
}