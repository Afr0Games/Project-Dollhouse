﻿/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Files;
using Files.Vitaboy;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Elements
{
    /// <summary>
    /// A UI control for browsing avatar heads.
    /// </summary>
    public class UIHeadBrowser : UISkinBrowser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event UISkinButtonClicked OnButtonClicked;
        private Vector2 HeadTileSize = new Vector2(37, 42);
        private Texture2D m_EditHeadSkinBtnTex;
        private int m_NumberOfHeads = 0;

        public UIHeadBrowser(UIScreen Screen, UIControl Ctrl, int SkinType, AvatarSex Sex) :
            base(Screen, Ctrl, SkinType, Sex)
        {
            m_EditHeadSkinBtnTex = FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_headskinbtn);

            m_Collections.Add(FileManager.Instance.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male_heads));
            m_FemaleCollections.Add(FileManager.Instance.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female_heads));
            m_Collections.Add(FileManager.Instance.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternalheads_unisex));

            OutfitContainer OftContainer;

            //Get all the thumbnails.
            foreach (Collection Col in m_Collections)
            {
                foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                {
                    OftContainer = new OutfitContainer(FileManager.Instance.GetOutfit(
                        FileManager.Instance.GetPurchasableOutfit(PO.UniqueID).OutfitID.UniqueID));

                    //Load all appearances where available, if the player wishes to switch skin type (see CASScreen.cs)
                    if (OftContainer.LightAppearance != null) m_LightAppearances.Add(OftContainer);
                    if (OftContainer.MediumAppearance != null) m_MediumAppearances.Add(OftContainer);
                    if (OftContainer.DarkAppearance != null) m_DarkAppearances.Add(OftContainer);
                }
            }

            //Get all the thumbnails.
            foreach (Collection Col in m_FemaleCollections)
            {
                foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                {
                    m_NumberOfHeads++;

                    OftContainer = new OutfitContainer(FileManager.Instance.GetOutfit(
                        FileManager.Instance.GetPurchasableOutfit(PO.UniqueID).OutfitID.UniqueID));

                    //Load all appearances where available, if the player wishes to switch skin type (see CASScreen.cs)
                    if (OftContainer.LightAppearance != null) m_LightFemaleAppearances.Add(OftContainer);
                    if (OftContainer.MediumAppearance != null) m_MediumFemaleAppearances.Add(OftContainer);
                    if (OftContainer.DarkAppearance != null) m_DarkFemaleAppearances.Add(OftContainer);
                }
            }

            foreach (OutfitContainer Ctr in m_LightAppearances)
            {
                SkinBtnContainer Container = new SkinBtnContainer();
                Container.BtnTex = m_EditHeadSkinBtnTex;
                Container.SourcePosition = 
                    //Initialize to second frame in image.
                    new Vector2((m_EditHeadSkinBtnTex.Width / (4)) * 2, 0.0f);
                m_SkinBtns.Add(Container);
            }

            foreach (OutfitContainer Ctr in m_MediumAppearances)
            {
                SkinBtnContainer Container = new SkinBtnContainer();
                Container.BtnTex = m_EditHeadSkinBtnTex;
                Container.SourcePosition =
                    //Initialize to second frame in image.
                    new Vector2((m_EditHeadSkinBtnTex.Width / (4)) * 2, 0.0f);
                m_SkinBtns.Add(Container);
            }

            foreach (OutfitContainer Ctr in m_DarkAppearances)
            {
                SkinBtnContainer Container = new SkinBtnContainer();
                Container.BtnTex = m_EditHeadSkinBtnTex;
                Container.SourcePosition =
                    //Initialize to second frame in image.
                    new Vector2((m_EditHeadSkinBtnTex.Width / (4)) * 2, 0.0f);
                m_SkinBtns.Add(Container);
            }
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            m_Counter = m_Index;

            for (int X = 0; X <= m_Map.GetUpperBound(1); X++)
            {
                for (int Y = 0; Y <= m_Map.GetUpperBound(0); Y++)
                {
                    m_BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                    Vector2 TexturePosition = new Vector2(X * (HeadTileSize.X + 10), Y * (HeadTileSize.Y));
                    Vector2 ButtonPosition = new Vector2(TexturePosition.X - 3, TexturePosition.Y - 5);

                    if (IsMouseOverButton(Helper, m_SkinBtns[Y + m_Counter], Position + ButtonPosition))
                    {
                        if (Helper.IsNewPress(MouseButtons.LeftButton))
                        {
                            if (!m_SkinBtns[Y + m_Counter].IsButtonClicked)
                            {
                                m_SkinBtns[Y + m_Counter].SourcePosition.X += m_BtnWidth;

                                if (OnButtonClicked != null)
                                {
                                    UISkinButtonClickedEventArgs EArgs = new UISkinButtonClickedEventArgs();

                                    switch (m_SelectedSkintype)
                                    {
                                        case Elements.SkinType.Light:
                                            EArgs.SkinType = 0;
                                            EArgs.SelectedOutfit = (m_Sex == AvatarSex.Male ?
                                                m_LightAppearances[(Y * X) + m_Counter].Oft :
                                                m_LightFemaleAppearances[(Y * X) + m_Counter].Oft);

                                            OnButtonClicked(this, EArgs);
                                            break;
                                        case Elements.SkinType.Medium:
                                            EArgs.SkinType = 1;
                                            EArgs.SelectedOutfit = (m_Sex == AvatarSex.Male ?
                                                m_MediumAppearances[(Y * X) + m_Counter].Oft :
                                                m_MediumFemaleAppearances[(Y * X) + m_Counter].Oft);
                                            OnButtonClicked(this, EArgs);
                                            break;
                                        case Elements.SkinType.Dark:
                                            EArgs.SkinType = 2;
                                            EArgs.SelectedOutfit = (m_Sex == AvatarSex.Male ?
                                                m_DarkAppearances[(Y * X) + m_Counter].Oft :
                                                m_DarkFemaleAppearances[(Y * X) + m_Counter].Oft);

                                            OnButtonClicked(this, EArgs);
                                            break;
                                    }
                                }

                                m_SkinBtns[Y + m_Counter].IsButtonClicked = true;
                            }
                        }
                        else
                        {
                            if (m_SkinBtns[Y + m_Counter].IsButtonClicked)
                                m_SkinBtns[Y + m_Counter].SourcePosition.X -= m_BtnWidth;

                            m_SkinBtns[Y + m_Counter].IsButtonClicked = false;
                        }

                        if (!m_SkinBtns[Y + m_Counter].IsMouseHovering)
                        {
                            m_SkinBtns[Y + m_Counter].SourcePosition.X -= m_BtnWidth;
                            m_SkinBtns[Y + m_Counter].IsMouseHovering = true;
                        }
                    }
                    else
                    {
                        m_SkinBtns[Y + m_Counter].SourcePosition.X = (m_BtnWidth * 2);
                        m_SkinBtns[Y + m_Counter].IsMouseHovering = false;
                    }

                    if (m_Counter < m_NumberOfSkinsToDisplay)
                        m_Counter++;
                }
            }

            if (Index == 0)
                m_SkinBrowserArrowRight.Enabled = false;
            else
                m_SkinBrowserArrowRight.Enabled = true;

            base.Update(Helper, GTime);
        }

        public override void Draw(SpriteBatch SBatch)
        {
            m_Counter = m_Index;

            SBatch.DrawString(m_Screen.Font9px, "Heads: " + m_NumberOfHeads, 
                new Vector2(Position.X + (m_Size.X / 2) - m_Screen.Font9px.MeasureString("Heads: ").X, 
                Position.Y + (m_Size.Y - 70)), new Color(255, 249, 157), 0.0f, new Vector2(0, 0), 1.0f, 
                SpriteEffects.None, m_Depth);

            switch (m_SelectedSkintype)
            {
                case Elements.SkinType.Light:
                    for (int X = 0; X <= m_Map.GetUpperBound(1); X++)
                    {
                        for (int Y = 0; Y <= m_Map.GetUpperBound(0); Y++)
                        {
                            m_Thumb = FileManager.Instance.GetTexture(m_Sex == AvatarSex.Male ? 
                                m_LightAppearances[(Y * X) + m_Counter].LightAppearance.ThumbnailID.UniqueID :
                                m_LightFemaleAppearances[(Y * X) + m_Counter].LightAppearance.ThumbnailID.UniqueID);
                            m_BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            m_BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = 
                                new Vector2(X * (HeadTileSize.X + 10), Y * (HeadTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 2, TexturePosition.Y - 5);

                            SBatch.Draw(m_SkinBtns[Y + m_Counter].BtnTex, new Rectangle((int)(Position.X +
                                ButtonPosition.X), (int)(Position.Y + ButtonPosition.Y), m_BtnWidth, m_BtnHeight),
                                new Rectangle((int)m_SkinBtns[m_Counter + Y].SourcePosition.X,
                                (int)m_SkinBtns[m_Counter + Y].SourcePosition.Y, m_BtnWidth, m_BtnHeight),
                                Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, m_Depth);

                            SBatch.Draw(m_Thumb, new Rectangle((int)(Position.X + TexturePosition.X),
                                (int)(Position.Y + TexturePosition.Y), m_Thumb.Width, m_Thumb.Height),
                                null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, m_Depth);

                            if (m_Counter < m_NumberOfSkinsToDisplay)
                                m_Counter++;
                        }
                    }

                    break;
                case Elements.SkinType.Medium:
                    for (int X = 0; X <= m_Map.GetUpperBound(1); X++)
                    {
                        for (int Y = 0; Y <= m_Map.GetUpperBound(0); Y++)
                        {
                            m_Thumb = FileManager.Instance.GetTexture(m_Sex == AvatarSex.Male ?
                                m_MediumAppearances[(Y * X) + m_Counter].MediumAppearance.ThumbnailID.UniqueID :
                                m_MediumFemaleAppearances[(Y * X) + m_Counter].MediumAppearance.ThumbnailID.UniqueID);
                            m_BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            m_BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = new Vector2(X * (HeadTileSize.X + 10), Y * (HeadTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 2, TexturePosition.Y - 5);

                            SBatch.Draw(m_SkinBtns[Y + m_Counter].BtnTex, new Rectangle((int)(Position.X +
                                ButtonPosition.X), (int)(Position.Y + ButtonPosition.Y), m_BtnWidth, m_BtnHeight),
                                new Rectangle((int)m_SkinBtns[m_Counter + Y].SourcePosition.X,
                                (int)m_SkinBtns[m_Counter + Y].SourcePosition.Y, m_BtnWidth, m_BtnHeight),
                                Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, m_Depth);

                            SBatch.Draw(m_Thumb, new Rectangle((int)(Position.X + TexturePosition.X),
                                (int)(Position.Y + TexturePosition.Y), m_Thumb.Width, m_Thumb.Height),
                                null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, m_Depth);

                            if (m_Counter < m_NumberOfSkinsToDisplay)
                                m_Counter++;
                        }
                    }

                    break;
                case Elements.SkinType.Dark:
                    for (int X = 0; X <= m_Map.GetUpperBound(1); X++)
                    {
                        for (int Y = 0; Y <= m_Map.GetUpperBound(0); Y++)
                        {
                            m_Thumb = FileManager.Instance.GetTexture(m_Sex == AvatarSex.Male ?
                                m_DarkAppearances[(Y * X) + m_Counter].DarkAppearance.ThumbnailID.UniqueID :
                                m_DarkFemaleAppearances[(Y * X) + m_Counter].DarkAppearance.ThumbnailID.UniqueID);
                            m_BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            m_BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = new Vector2(X * (HeadTileSize.X + 10), Y * (HeadTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 2, TexturePosition.Y - 5);

                            SBatch.Draw(m_SkinBtns[Y + m_Counter].BtnTex, new Rectangle((int)(Position.X +
                                ButtonPosition.X), (int)(Position.Y + ButtonPosition.Y), m_BtnWidth, m_BtnHeight),
                                new Rectangle((int)m_SkinBtns[m_Counter + Y].SourcePosition.X,
                                (int)m_SkinBtns[m_Counter + Y].SourcePosition.Y, m_BtnWidth, m_BtnHeight),
                                Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, m_Depth);

                            SBatch.Draw(m_Thumb, new Rectangle((int)(Position.X + TexturePosition.X),
                                (int)(Position.Y + TexturePosition.Y), m_Thumb.Width, m_Thumb.Height),
                                null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, m_Depth);

                            if (m_Counter < m_NumberOfSkinsToDisplay)
                                m_Counter++;
                        }
                    }

                    break;
            }

            base.Draw(SBatch);
        }
    }
}
