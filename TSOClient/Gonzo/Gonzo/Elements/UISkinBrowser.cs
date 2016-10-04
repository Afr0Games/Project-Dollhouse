using System.Collections.Generic;
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

    internal class OutfitContainer
    {
        /// <summary>
        /// Container for grouping together outfits and appearances.
        /// </summary>
        /// <param name="OFT">An outfit from which to create this container.</param>
        public OutfitContainer(Outfit OFT)
        {
            Oft = OFT;
            Appearance Apr = FileManager.GetAppearance(Oft.LightAppearance.UniqueID);

            if (Apr.ThumbnailID.TypeID != 0)
                LightAppearance = Apr;
            Apr = FileManager.GetAppearance(Oft.MediumAppearance.UniqueID);
            if (Apr.ThumbnailID.TypeID != 0)
                MediumAppearance = Apr;
            Apr = FileManager.GetAppearance(Oft.DarkAppearance.UniqueID);
            if (Apr.ThumbnailID.TypeID != 0)
                DarkAppearance = Apr;
        }

        public Outfit Oft;
        public Appearance LightAppearance, MediumAppearance, DarkAppearance;
    }

    /// <summary>
    /// Container for button textures and info needed to display them.
    /// </summary>
    public class SkinBtnContainer
    {
        public Texture2D BtnTex;
        public Vector2 SourcePosition; //Which part of the button image to draw.
        public bool IsButtonClicked = false; //Has this button been clicked?
        public bool IsMouseHovering = false; //Is the mouse cursor hovering over this button?
    }

    /// <summary>
    /// Raised whenever a button in the UISkinBrowser is clicked.
    /// </summary>
    /// <param name="SkinType">The type of skin; 0 means light, 1 means medium, 2 means dark.</param>
    /// <param name="SelectedOutfit">The selected outfit represented by the button clicked.</param>
    public delegate void UISkinButtonClicked(int SkinType, Outfit SelectedOutfit);

    /// <summary>
    /// A skin browser is used to browse through thumbnails of heads and bodies.
    /// </summary>
    public class UISkinBrowser : UIControl
    {
        private SkinType m_SelectedSkintype;
        private bool m_HeadBrowser; //Is this a head or body browser?

        private List<OutfitContainer> m_LightAppearances = new List<OutfitContainer>();
        private List<OutfitContainer> m_MediumAppearances = new List<OutfitContainer>();
        private List<OutfitContainer> m_DarkAppearances = new List<OutfitContainer>();
        private List<Collection> m_Collections = new List<Collection>();

        private List<SkinBtnContainer> m_SkinBtns = new List<SkinBtnContainer>();
        private Texture2D m_EditHeadSkinBtnTex, m_EditBodySkinBtnTex;
        private Vector2 m_ButtonSize;

        public event UISkinButtonClicked OnButtonClicked;

        private UIButton m_SkinBrowserArrowLeft, m_SkinBrowserArrowRight;

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
            Position = Position + Screen.Position;

            m_EditHeadSkinBtnTex = FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_headskinbtn);
            m_EditBodySkinBtnTex = FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_bodyskinbtn);

            m_SelectedSkintype = (Elements.SkinType)SkinType;
            m_HeadBrowser = HeadBrowser;

            m_ButtonSize = new Vector2();
            m_ButtonSize.X = m_EditHeadSkinBtnTex.Width / 4;
            m_ButtonSize.Y = m_EditBodySkinBtnTex.Height;

            m_SkinBrowserArrowLeft = new UIButton("SkinBrowserArrowLeft",
                FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skinbrowserarrowleft),
                Position + new Vector2(-35, Size.Y - 80), Screen);
            m_SkinBrowserArrowLeft.OnButtonClicked += M_SkinBrowserArrowLeft_OnButtonClicked;
            m_SkinBrowserArrowRight = new UIButton("SkinBrowserArrowRight", 
                FileManager.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skinbrowserarrowright), 
                Position + new Vector2(Size.X - 35, Size.Y - 80), Screen);
            m_SkinBrowserArrowRight.OnButtonClicked += M_SkinBrowserArrowRight_OnButtonClicked;

            if (HeadBrowser)
            {
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male_heads));
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female_heads));
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternalheads_unisex));

                OutfitContainer Container;

                //Get all the thumbnails.
                foreach (Collection Col in m_Collections)
                {
                    foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                    {
                        Container = new OutfitContainer(FileManager.GetOutfit(
                            FileManager.GetPurchasableOutfit(PO.UniqueID).OutfitID.UniqueID));

                        switch (m_SelectedSkintype)
                        {
                            case Elements.SkinType.Light:
                                if (Container.LightAppearance != null) m_LightAppearances.Add(Container);
                                break;
                            case Elements.SkinType.Medium:
                                if (Container.MediumAppearance != null) m_MediumAppearances.Add(Container);
                                break;
                            case Elements.SkinType.Dark:
                                if (Container.DarkAppearance != null) m_DarkAppearances.Add(Container);
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

                OutfitContainer Container;

                //Get all the thumbnails.
                foreach (Collection Col in m_Collections)
                {
                    foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                    {
                        Container = new OutfitContainer(FileManager.GetOutfit(
                            FileManager.GetPurchasableOutfit(PO.UniqueID).OutfitID.UniqueID));

                        switch (m_SelectedSkintype)
                        {
                            case Elements.SkinType.Light:
                                if (Container.LightAppearance != null) m_LightAppearances.Add(Container);
                                break;
                            case Elements.SkinType.Medium:
                                if (Container.MediumAppearance != null) m_MediumAppearances.Add(Container);
                                break;
                            case Elements.SkinType.Dark:
                                if (Container.DarkAppearance != null) m_DarkAppearances.Add(Container);
                                break;
                        }
                    }
                }
            }

            switch(m_SelectedSkintype)
            {
                case Elements.SkinType.Light:
                    foreach(OutfitContainer Ctr in m_LightAppearances)
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
                    foreach (OutfitContainer Ctr in m_MediumAppearances)
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
                    foreach (OutfitContainer Ctr in m_DarkAppearances)
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

        private void M_SkinBrowserArrowLeft_OnButtonClicked(UIButton ClickedButton)
        {
            m_Index++;
        }

        private void M_SkinBrowserArrowRight_OnButtonClicked(UIButton ClickedButton)
        {
            if(Index >= 1)
                m_Index--;
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

        /// <summary>
        /// Checks if the mouse cursor is over a skin button.
        /// </summary>
        /// <param name="Input">The InputHelper instance used to get the mouse curor's position.</param>
        /// <param name="Button">A SkinBtnContainer representing the button to check.</param>
        /// <param name="BtnPosition">Position of the button.</param>
        /// <returns></returns>
        private bool IsMouseOverButton(InputHelper Input, SkinBtnContainer Button, Vector2 BtnPosition)
        {
            if (Input.MousePosition.X > BtnPosition.X && Input.MousePosition.X <= (BtnPosition.X + (Button.BtnTex.Width / 4)))
            {
                if (Input.MousePosition.Y > BtnPosition.Y && Input.MousePosition.Y <= (BtnPosition.Y + Button.BtnTex.Height))
                    return true;
            }

            return false;
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            m_Counter = m_Index;

            for (int X = 0; X <= Map.GetUpperBound(1); X++)
            {
                for (int Y = 0; Y <= Map.GetUpperBound(0); Y++)
                {
                    int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                    Vector2 TexturePosition = m_HeadBrowser == true ?
                        new Vector2(X * (HeadTileSize.X), Y * (HeadTileSize.Y)) :
                         new Vector2(X * (BodyTileSize.X), Y * (BodyTileSize.Y));
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

                    if (m_Counter < NumberOfSkinsToDisplay)
                        m_Counter++;
                }
            }

            m_SkinBrowserArrowLeft.Update(Helper, GTime);
            m_SkinBrowserArrowRight.Update(Helper, GTime);

            base.Update(Helper, GTime);
        }

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
                            m_Thumb = FileManager.GetTexture(m_LightAppearances[Y + m_Counter].LightAppearance.
                                ThumbnailID.UniqueID);
                            int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            int BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = m_HeadBrowser == true ? 
                                new Vector2(X * (HeadTileSize.X + 10), Y * (HeadTileSize.Y)) :
                                 new Vector2(X * (BodyTileSize.X + 10), Y * (BodyTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 2, TexturePosition.Y - 5);

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
                            m_Thumb = FileManager.GetTexture(m_MediumAppearances[Y + m_Counter].MediumAppearance.
                                ThumbnailID.UniqueID);
                            int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            int BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = m_HeadBrowser == true ?
                                new Vector2(X * (HeadTileSize.X + 10), Y * (HeadTileSize.Y)) :
                                 new Vector2(X * (BodyTileSize.X + 10), Y * (BodyTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 2, TexturePosition.Y - 5);

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
                            m_Thumb = FileManager.GetTexture(m_DarkAppearances[Y + m_Counter].DarkAppearance.
                                ThumbnailID.UniqueID);
                            int BtnWidth = m_SkinBtns[m_Counter + Y].BtnTex.Width / 4;
                            int BtnHeight = m_SkinBtns[m_Counter + Y].BtnTex.Height;

                            Vector2 TexturePosition = m_HeadBrowser == true ?
                                new Vector2(X * (HeadTileSize.X + 10), Y * (HeadTileSize.Y)) :
                                 new Vector2(X * (BodyTileSize.X + 10), Y * (BodyTileSize.Y));
                            Vector2 ButtonPosition = new Vector2(TexturePosition.X - 2, TexturePosition.Y - 5);

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

            m_SkinBrowserArrowLeft.Draw(SBatch, Depth);
            m_SkinBrowserArrowRight.Draw(SBatch, Depth);

            base.Draw(SBatch, LayerDepth);
        }
    }
}
