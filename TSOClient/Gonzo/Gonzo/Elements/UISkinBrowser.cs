using System;
using System.Collections.Generic;
using System.Text;
using Files;
using Files.Manager;
using Files.Vitaboy;
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
    internal class ThumbnailsContainer
    {
        public Texture2D LightThumbnail, MediumThumbnail, DarkThumbnail;
    }

    /// <summary>
    /// A skin browser is used to browse through thumbnails of heads and bodies.
    /// </summary>
    public class UISkinBrowser : UIControl
    {
        private List<Texture2D> m_LightThumbnails = new List<Texture2D>();
        private List<Texture2D> m_MediumThumbnails = new List<Texture2D>();
        private List<Texture2D> m_DarkThumbnails = new List<Texture2D>();
        private List<Collection> m_Collections = new List<Collection>();

        /// <summary>
        /// Constructs a new instance of UISkinBrowser.
        /// </summary>
        /// <param name="Screen">A UIScreen instance that this UISkinBrowser belongs to.</param>
        /// <param name="Ctrl">A UIControl instance that this UISkinBrowser should be created from.</param>
        /// <param name="HeadBrowser">Is this browser supposed to browse head skins?</param>
        public UISkinBrowser(UIScreen Screen, UIControl Ctrl, bool HeadBrowser) : base(Ctrl, Screen)
        {
            if(HeadBrowser)
            {
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male_heads));
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female_heads));
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternalheads_unisex));

                ThumbnailsContainer Container;

                //Get all the thumbnails.
                foreach(Collection Col in m_Collections)
                {
                    foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                    {
                        Container = GetThumbnails(FileManager.GetPurchasableOutfit(PO.UniqueID));

                        if (Container.LightThumbnail != null) m_LightThumbnails.Add(Container.LightThumbnail);
                        if (Container.MediumThumbnail != null) m_MediumThumbnails.Add(Container.MediumThumbnail);
                        if (Container.DarkThumbnail != null) m_DarkThumbnails.Add(Container.DarkThumbnail);
                    }
                }
            }
            else //Bodybrowser.
            {
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_male));
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.ea_female));
                m_Collections.Add(FileManager.GetCollection((ulong)FileIDs.CollectionsFileIDs.eainternal_unisex));

                ThumbnailsContainer Container;

                //Get all the thumbnails.
                foreach (Collection Col in m_Collections)
                {
                    foreach (UniqueFileID PO in Col.PurchasableOutfitIDs)
                    {
                        Container = GetThumbnails(FileManager.GetPurchasableOutfit(PO.UniqueID));

                        if (Container.LightThumbnail != null) m_LightThumbnails.Add(Container.LightThumbnail);
                        if (Container.MediumThumbnail != null) m_MediumThumbnails.Add(Container.MediumThumbnail);
                        if (Container.DarkThumbnail != null) m_DarkThumbnails.Add(Container.DarkThumbnail);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a ThumbnailsContainer for a PurchasableOutfit instance.
        /// </summary>
        /// <param name="PO">A PurchasableOutfit instance.</param>
        /// <returns>A ThumbnailsContainer instance.</returns>
        private ThumbnailsContainer GetThumbnails(PurchasableOutfit PO)
        {
            Outfit Oft = FileManager.GetOutfit(PO.OutfitID.UniqueID);
            Appearance Apr;
            ThumbnailsContainer Container = new ThumbnailsContainer();

            Apr = FileManager.GetAppearance(Oft.LightAppearance.UniqueID);
            if(Apr.ThumbnailID.TypeID != 0)
                Container.LightThumbnail = FileManager.GetTexture(Apr.ThumbnailID.UniqueID);
            Apr = FileManager.GetAppearance(Oft.MediumAppearance.UniqueID);
            if (Apr.ThumbnailID.TypeID != 0)
                Container.MediumThumbnail = FileManager.GetTexture(Apr.ThumbnailID.UniqueID);
            Apr = FileManager.GetAppearance(Oft.DarkAppearance.UniqueID);
            if (Apr.ThumbnailID.TypeID != 0)
                Container.DarkThumbnail = FileManager.GetTexture(Apr.ThumbnailID.UniqueID);

            return Container;
        }
    }
}
