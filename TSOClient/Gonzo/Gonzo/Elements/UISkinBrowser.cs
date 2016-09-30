using System;

using System.Collections.Generic;
using System.Text;
using Files;
using Files.Manager;
using Files.Vitaboy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Elements
{
    internal enum SkinType
    {
        Light = 0x0,
        Medium = 0x1,
        Dark = 0x2
    }

    /// <summary>
    /// Container for Texture2D instances returned from GetThumbnails().
    /// These instances may be null.
    /// </summary>
    internal class AppearanceContainer
    {
        public Appearance LightAppearance, MediumAppearance, DarkAppearance;
    }

    /// <summary>
    /// Container for button textures and info needed to display them.
    /// </summary>
    internal class SkinBtnContainer
    {
        public Texture2D BtnTex;
        public Vector2 SourcePosition; //Which part of the button image to draw.
    }

    /// <summary>
    /// A skin browser is used to browse through thumbnails of heads and bodies.
    /// </summary>
    public class UISkinBrowser : UIControl
    {
        private SkinType m_SelectedSkintype;
        private bool m_HeadBrowser; //Is this a head or body browser?

        private List<Appearance> m_LightAppearances = new List<Appearance>();
        private List<Appearance> m_MediumAppearances = new List<Appearance>();
        private List<Appearance> m_DarkAppearances = new List<Appearance>();
        private List<Collection> m_Collections = new List<Collection>();

        private List<SkinBtnContainer> m_SkinBtns = new List<SkinBtnContainer>();
        private Texture2D m_EditHeadSkinBtnTex, m_EditBodySkinBtnTex;
        private Vector2 m_ButtonSize;

        /// <summary>
        /// Sets the skintype currently visible in this UISkinBrowser.
        /// 0 = light, 1 = medium, 2 = dark.
        /// </summary>
        public int SkinType
        {
            set
            {
                switch(value)
                {
                    case 0:
                        m_SelectedSkintype = Elements.SkinType.Light;
                        break;
                    case 1:
                        m_SelectedSkintype = Elements.SkinType.Medium;
                        break;
                    case 2:
                        m_SelectedSkintype = Elements.SkinType.Dark;
                        break;
                }
            }
        }

        /// <summary>
        /// Index which controls which rows of skins are visible.
        /// </summary>
        public int Index
        {
            //TODO: Find upper bound...
            get { return m_Index; }
            set { m_Index = value; }
        }

        /// <summary>
        /// Constructs a new instance of UISkinBrowser.
        /// </summary>
        /// <param name="Screen">A UIScreen instance that this UISkinBrowser belongs to.</param>
        /// <param name="Ctrl">A UIControl instance that this UISkinBrowser should be created from.</param>
        /// <param name="SkinType">The type of skin initially displayed by this UISkinBrowser. 0 = light, 1 = medium, 2 = dark.</param>
        /// <param name="HeadBrowser">Is this browser supposed to browse head skins?</param>
        public UISkinBrowser(UIScreen Screen, UIControl Ctrl, int SkinType, bool HeadBrowser) : base(Ctrl, Screen)
        {
            m_EditHeadSkinBtnTex = FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_headskinbtn);
            m_EditBodySkinBtnTex = FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_bodyskinbtn);

            m_SelectedSkintype = (Elements.SkinType)SkinType;
            m_HeadBrowser = HeadBrowser;

            m_ButtonSize = new Vector2();
            m_ButtonSize.X = m_EditHeadSkinBtnTex.Width / 4;
            m_ButtonSize.Y = m_EditBodySkinBtnTex.Height;

            if (HeadBrowser)
            {
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male_heads));
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female_heads));
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternalheads_unisex));

                AppearanceContainer Container;

                //Get all the thumbnails.
                foreach(Collection Col in m_Collections)
                {
                    foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                    {
                        Container = GetThumbnails(FileManager.GetPurchasableOutfit(PO.UniqueID));

                        switch (m_SelectedSkintype)
                        {
                            case Elements.SkinType.Light:
                                if (Container.LightAppearance != null) m_LightAppearances.Add(Container.LightAppearance);
                                break;
                            case Elements.SkinType.Medium:
                                if (Container.MediumAppearance != null) m_MediumAppearances.Add(Container.MediumAppearance);
                                break;
                            case Elements.SkinType.Dark:
                                if (Container.DarkAppearance != null) m_DarkAppearances.Add(Container.DarkAppearance);
                                break;
                        }
                    }
                }
            }
            else //Bodybrowser.
            {
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male));
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female));
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternal_unisex));

                AppearanceContainer Container;

                //Get all the thumbnails.
                foreach (Collection Col in m_Collections)
                {
                    foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                    {
                        Container = GetThumbnails(FileManager.GetPurchasableOutfit(PO.UniqueID));

                        switch (m_SelectedSkintype)
                        {
                            case Elements.SkinType.Light:
                                if (Container.LightAppearance != null) m_LightAppearances.Add(Container.LightAppearance);
                                break;
                            case Elements.SkinType.Medium:
                                if (Container.MediumAppearance != null) m_MediumAppearances.Add(Container.MediumAppearance);
                                break;
                            case Elements.SkinType.Dark:
                                if (Container.DarkAppearance != null) m_DarkAppearances.Add(Container.DarkAppearance);
                                break;
                        }
                    }
                }
            }

            switch(m_SelectedSkintype)
            {
                case Elements.SkinType.Light:
                    foreach(Appearance Apr in m_LightAppearances)
                    {
                        SkinBtnContainer Container = new SkinBtnContainer();
                        Container.BtnTex = HeadBrowser == true ? m_EditHeadSkinBtnTex : m_EditBodySkinBtnTex;
                        Container.SourcePosition = HeadBrowser == true ?
                            //Initialize to second frame in image.
                            new Vector2((m_EditHeadSkinBtnTex.Width / (4)) * 2, 0.0f) :
                            new Vector2((m_EditBodySkinBtnTex.Width / (4)) * 2, 0.0f);
                        m_SkinBtns.Add(Container);
                    }

                    break;
                case Elements.SkinType.Medium:
                    foreach (Appearance Apr in m_MediumAppearances)
                    {
                        SkinBtnContainer Container = new SkinBtnContainer();
                        Container.BtnTex = HeadBrowser == true ? m_EditHeadSkinBtnTex : m_EditBodySkinBtnTex;
                        Container.SourcePosition = HeadBrowser == true ?
                            //Initialize to second frame in image.
                            new Vector2((m_EditHeadSkinBtnTex.Width / (4)) * 2, 0.0f) :
                            new Vector2((m_EditBodySkinBtnTex.Width / (4)) * 2, 0.0f);
                        m_SkinBtns.Add(Container);
                    }

                    break;
                case Elements.SkinType.Dark:
                    foreach (Appearance Apr in m_DarkAppearances)
                    {
                        SkinBtnContainer Container = new SkinBtnContainer();
                        Container.BtnTex = HeadBrowser == true ? m_EditHeadSkinBtnTex : m_EditBodySkinBtnTex;
                        Container.SourcePosition = HeadBrowser == true ?
                            //Initialize to second frame in image.
                            new Vector2((m_EditHeadSkinBtnTex.Width / (4)) * 2, 0.0f) :
                            new Vector2((m_EditBodySkinBtnTex.Width / (4)) * 2, 0.0f);
                        m_SkinBtns.Add(Container);
                    }

                    break;
            }
        }

        /// <summary>
        /// Returns a AppearanceContainer for a PurchasableOutfit instance.
        /// </summary>
        /// <param name="PO">A PurchasableOutfit instance.</param>
        /// <returns>A AppearanceContainer instance.</returns>
        private AppearanceContainer GetThumbnails(PurchasableOutfit PO)
        {
            Outfit Oft = FileManager.GetOutfit(PO.OutfitID.UniqueID);
            Appearance Apr;
            AppearanceContainer Container = new AppearanceContainer();

            Apr = FileManager.GetAppearance(Oft.LightAppearance.UniqueID);
            if (Apr.ThumbnailID.TypeID != 0)
                Container.LightAppearance = Apr;
            Apr = FileManager.GetAppearance(Oft.MediumAppearance.UniqueID);
            if (Apr.ThumbnailID.TypeID != 0)
                Container.MediumAppearance = Apr;
            Apr = FileManager.GetAppearance(Oft.DarkAppearance.UniqueID);
            if (Apr.ThumbnailID.TypeID != 0)
                Container.DarkAppearance = Apr;

            return Container;
        }

        private Texture2D m_Thumb;
        private int m_Counter = 0, m_Index = 0, NumberOfSkinsToDisplay = 21;
        float Depth = 0.0f;

        Vector2 HeadTileSize = new Vector2(37, 42);
        Vector2 BodyTileSize = new Vector2(37, 76);

        private int[,] Map = new int[,]
        {
         {0, 0, 0, 0, 0, 0, 0},
         {0, 0, 0, 0, 0, 0, 0},
         {0, 0, 0, 0, 0, 0, 0},
        };

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            m_Counter = m_Index;

            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.9f;

            switch(m_SelectedSkintype)
            {
                case Elements.SkinType.Light:
                    for (int X = 0; X <= Map.GetUpperBound(1); X++)
                    {
                        for (int Y = 0; Y <= Map.GetUpperBound(0); Y++)
                        {
                            m_Thumb = FileManager.GetTexture(m_LightAppearances[Y + m_Counter].ThumbnailID.UniqueID);
                            int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            int BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = m_HeadBrowser == true ? 
                                new Vector2(X * (HeadTileSize.X), Y * (HeadTileSize.Y)) :
                                 new Vector2(X * (BodyTileSize.X), Y * (BodyTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 3, TexturePosition.Y - 5);

                            SBatch.Draw(m_SkinBtns[Y + m_Counter].BtnTex, new Rectangle((int)(Position.X + 
                                ButtonPosition.X), (int)(Position.Y + ButtonPosition.Y), BtnWidth, BtnHeight),
                                new Rectangle((int)m_SkinBtns[m_Counter + Y].SourcePosition.X, 
                                (int)m_SkinBtns[m_Counter + Y].SourcePosition.Y, BtnWidth, BtnHeight), 
                                Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, Depth - 0.1f);

                            SBatch.Draw(m_Thumb, new Rectangle((int)(Position.X + TexturePosition.X), 
                                (int)(Position.Y + TexturePosition.Y), m_Thumb.Width, m_Thumb.Height), 
                                null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, Depth);

                            if (m_Counter < NumberOfSkinsToDisplay)
                                m_Counter++;
                        }
                    }

                    break;
                case Elements.SkinType.Medium:
                    for (int X = 0; X <= Map.GetUpperBound(1); X++)
                    {
                        for (int Y = 0; Y <= Map.GetUpperBound(0); Y++)
                        {
                            m_Thumb = FileManager.GetTexture(m_MediumAppearances[Y + m_Counter].ThumbnailID.UniqueID);
                            int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            int BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = m_HeadBrowser == true ?
                                new Vector2(X * (HeadTileSize.X), Y * (HeadTileSize.Y)) :
                                 new Vector2(X * (BodyTileSize.X), Y * (BodyTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 3, TexturePosition.Y - 5);

                            SBatch.Draw(m_SkinBtns[Y + m_Counter].BtnTex, new Rectangle((int)(Position.X +
                                ButtonPosition.X), (int)(Position.Y + ButtonPosition.Y), BtnWidth, BtnHeight),
                                new Rectangle((int)m_SkinBtns[m_Counter + Y].SourcePosition.X,
                                (int)m_SkinBtns[m_Counter + Y].SourcePosition.Y, BtnWidth, BtnHeight),
                                Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, Depth - 0.1f);

                            SBatch.Draw(m_Thumb, new Rectangle((int)(Position.X + TexturePosition.X),
                                (int)(Position.Y + TexturePosition.Y), m_Thumb.Width, m_Thumb.Height),
                                null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, Depth);

                            if (m_Counter < NumberOfSkinsToDisplay)
                                m_Counter++;
                        }
                    }

                    break;
                case Elements.SkinType.Dark:
                    for (int X = 0; X <= Map.GetUpperBound(1); X++)
                    {
                        for (int Y = 0; Y <= Map.GetUpperBound(0); Y++)
                        {
                            m_Thumb = FileManager.GetTexture(m_DarkAppearances[Y + m_Counter].ThumbnailID.UniqueID);
                            int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            int BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = m_HeadBrowser == true ?
                                new Vector2(X * (HeadTileSize.X), Y * (HeadTileSize.Y)) :
                                 new Vector2(X * (BodyTileSize.X), Y * (BodyTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 3, TexturePosition.Y - 5);

                            SBatch.Draw(m_SkinBtns[Y + m_Counter].BtnTex, new Rectangle((int)(Position.X +
                                ButtonPosition.X), (int)(Position.Y + ButtonPosition.Y), BtnWidth, BtnHeight),
                                new Rectangle((int)m_SkinBtns[m_Counter + Y].SourcePosition.X,
                                (int)m_SkinBtns[m_Counter + Y].SourcePosition.Y, BtnWidth, BtnHeight),
                                Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, Depth - 0.1f);

                            SBatch.Draw(m_Thumb, new Rectangle((int)(Position.X + TexturePosition.X),
                                (int)(Position.Y + TexturePosition.Y), m_Thumb.Width, m_Thumb.Height),
                                null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, Depth);

                            if (m_Counter < NumberOfSkinsToDisplay)
                                m_Counter++;
                        }
                    }

                    break;
            }

            base.Draw(SBatch, LayerDepth);
        }
    }
}
