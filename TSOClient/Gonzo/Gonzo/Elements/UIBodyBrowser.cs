using System;
using System.Collections.Generic;
using Files;
using Files.Vitaboy;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Elements
{
    /// <summary>
    /// A UI control for browsing avatar bodies.
    /// </summary>
    public class UIBodyBrowser : UISkinBrowser
    {
        public event UISkinButtonClicked OnButtonClicked;
        private Vector2 BodyTileSize = new Vector2(37, 76);
        private Texture2D m_EditBodySkinBtnTex;

        public UIBodyBrowser(UIScreen Screen, UIControl Ctrl, int SkinType) : base(Screen, Ctrl, SkinType)
        {
            m_EditBodySkinBtnTex = FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_bodyskinbtn);

            m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male));
            m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female));
            m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternal_unisex));

            OutfitContainer OftContainer;

            m_Map = new int[,]
            {
                {0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0},
            };

            //Get all the thumbnails.
            foreach (Collection Col in m_Collections)
            {
                foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                {
                    OftContainer = new OutfitContainer(FileManager.GetOutfit(
                        FileManager.GetPurchasableOutfit(PO.UniqueID).OutfitID.UniqueID));

                    switch (m_SelectedSkintype)
                    {
                        case Elements.SkinType.Light:
                            if (OftContainer.LightAppearance != null) m_LightAppearances.Add(OftContainer);
                            break;
                        case Elements.SkinType.Medium:
                            if (OftContainer.MediumAppearance != null) m_MediumAppearances.Add(OftContainer);
                            break;
                        case Elements.SkinType.Dark:
                            if (OftContainer.DarkAppearance != null) m_DarkAppearances.Add(OftContainer);
                            break;
                    }
                }
            }

            switch (m_SelectedSkintype)
            {
                case Elements.SkinType.Light:
                    foreach (OutfitContainer Ctr in m_LightAppearances)
                    {
                        SkinBtnContainer Container = new SkinBtnContainer();
                        Container.BtnTex = m_EditBodySkinBtnTex;
                        Container.SourcePosition = 
                            //Initialize to second frame in image.
                            new Vector2((m_EditBodySkinBtnTex.Width / (4)) * 2, 0.0f);
                        m_SkinBtns.Add(Container);
                    }

                    break;
                case Elements.SkinType.Medium:
                    foreach (OutfitContainer Ctr in m_MediumAppearances)
                    {
                        SkinBtnContainer Container = new SkinBtnContainer();
                        Container.BtnTex = m_EditBodySkinBtnTex;
                        Container.SourcePosition = 
                            //Initialize to second frame in image.
                            new Vector2((m_EditBodySkinBtnTex.Width / (4)) * 2, 0.0f);
                        m_SkinBtns.Add(Container);
                    }

                    break;
                case Elements.SkinType.Dark:
                    foreach (OutfitContainer Ctr in m_DarkAppearances)
                    {
                        SkinBtnContainer Container = new SkinBtnContainer();
                        Container.BtnTex = m_EditBodySkinBtnTex;
                        Container.SourcePosition =
                            //Initialize to second frame in image.
                            new Vector2((m_EditBodySkinBtnTex.Width / (4)) * 2, 0.0f);
                        m_SkinBtns.Add(Container);
                    }

                    break;
            }
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            m_Counter = m_Index;

            for (int X = 0; X <= m_Map.GetUpperBound(1); X++)
            {
                for (int Y = 0; Y <= m_Map.GetUpperBound(0); Y++)
                {
                    int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                    Vector2 TexturePosition = new Vector2(X * (BodyTileSize.X + 10), Y * (BodyTileSize.Y));
                    Vector2 ButtonPosition = new Vector2(TexturePosition.X - 3, TexturePosition.Y - 5);

                    if(IsMouseOverButton(Helper, m_SkinBtns[Y + m_Counter], Position + ButtonPosition))
                    {
                        if (Helper.IsNewPress(MouseButtons.LeftButton))
                        {
                            if (!m_SkinBtns[Y + m_Counter].IsButtonClicked)
                            {
                                m_SkinBtns[Y + m_Counter].SourcePosition.X += BtnWidth;

                                if (OnButtonClicked != null)
                                {
                                    switch (m_SelectedSkintype)
                                    {
                                        case Elements.SkinType.Light:
                                            OnButtonClicked(0, m_LightAppearances[Y + m_Counter].Oft);
                                            break;
                                        case Elements.SkinType.Medium:
                                            OnButtonClicked(1, m_MediumAppearances[Y + m_Counter].Oft);
                                            break;
                                        case Elements.SkinType.Dark:
                                            OnButtonClicked(2, m_DarkAppearances[Y + m_Counter].Oft);
                                            break;
                                    }
                                }

                                m_SkinBtns[Y + m_Counter].IsButtonClicked = true;
                            }
                        }
                        else
                        {
                            if (m_SkinBtns[Y + m_Counter].IsButtonClicked)
                                m_SkinBtns[Y + m_Counter].SourcePosition.X -= BtnWidth;

                            m_SkinBtns[Y + m_Counter].IsButtonClicked = false;
                        }

                        if (!m_SkinBtns[Y + m_Counter].IsMouseHovering)
                        {
                            m_SkinBtns[Y + m_Counter].SourcePosition.X -= BtnWidth;
                            m_SkinBtns[Y + m_Counter].IsMouseHovering = true;
                        }
                    }
                    else
                    {
                        m_SkinBtns[Y + m_Counter].SourcePosition.X = (BtnWidth * 2);
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

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            m_Counter = m_Index;

            if (LayerDepth != null)
                m_Depth = (float)LayerDepth;
            else
                m_Depth = 0.9f;

            switch (m_SelectedSkintype)
            {
                case Elements.SkinType.Light:
                    for (int X = 0; X <= m_Map.GetUpperBound(1); X++)
                    {
                        for (int Y = 0; Y <= m_Map.GetUpperBound(0); Y++)
                        {
                            m_Thumb = FileManager.GetTexture(m_LightAppearances[Y + m_Counter].LightAppearance.
                                ThumbnailID.UniqueID);
                            int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            int BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = new Vector2(X * (BodyTileSize.X + 16), Y * (BodyTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 3, TexturePosition.Y - 5);

                            SBatch.Draw(m_SkinBtns[Y + m_Counter].BtnTex, new Rectangle((int)(Position.X +
                                ButtonPosition.X), (int)(Position.Y + ButtonPosition.Y), BtnWidth, BtnHeight),
                                new Rectangle((int)m_SkinBtns[m_Counter + Y].SourcePosition.X,
                                (int)m_SkinBtns[m_Counter + Y].SourcePosition.Y, BtnWidth, BtnHeight),
                                Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, m_Depth - 0.1f);

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
                            m_Thumb = FileManager.GetTexture(m_MediumAppearances[Y + m_Counter].MediumAppearance.
                                ThumbnailID.UniqueID);
                            int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            int BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = new Vector2(X * (BodyTileSize.X + 16), Y * (BodyTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 3, TexturePosition.Y - 5);

                            SBatch.Draw(m_SkinBtns[Y + m_Counter].BtnTex, new Rectangle((int)(Position.X +
                                ButtonPosition.X), (int)(Position.Y + ButtonPosition.Y), BtnWidth, BtnHeight),
                                new Rectangle((int)m_SkinBtns[m_Counter + Y].SourcePosition.X,
                                (int)m_SkinBtns[m_Counter + Y].SourcePosition.Y, BtnWidth, BtnHeight),
                                Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, m_Depth - 0.1f);

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
                            m_Thumb = FileManager.GetTexture(m_DarkAppearances[Y + m_Counter].DarkAppearance.
                                ThumbnailID.UniqueID);
                            int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            int BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = new Vector2(X * (BodyTileSize.X + 16), Y * (BodyTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 3, TexturePosition.Y - 5);

                            SBatch.Draw(m_SkinBtns[Y + m_Counter].BtnTex, new Rectangle((int)(Position.X +
                                ButtonPosition.X), (int)(Position.Y + ButtonPosition.Y), BtnWidth, BtnHeight),
                                new Rectangle((int)m_SkinBtns[m_Counter + Y].SourcePosition.X,
                                (int)m_SkinBtns[m_Counter + Y].SourcePosition.Y, BtnWidth, BtnHeight),
                                Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, m_Depth - 0.1f);

                            SBatch.Draw(m_Thumb, new Rectangle((int)(Position.X + TexturePosition.X),
                                (int)(Position.Y + TexturePosition.Y), m_Thumb.Width, m_Thumb.Height),
                                null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, m_Depth);

                            if (m_Counter < m_NumberOfSkinsToDisplay)
                                m_Counter++;
                        }
                    }

                    break;
            }

            base.Draw(SBatch, LayerDepth);
        }
    }
}
