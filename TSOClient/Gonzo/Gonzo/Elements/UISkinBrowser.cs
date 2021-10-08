/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using Files;
using Files.Manager;
using Files.Vitaboy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Elements
{
    public enum SkinType
    {
        Light = 0x0,
        Medium = 0x1,
        Dark = 0x2
    }

    public enum AvatarSex
    {
        Male = 0x0,
        Female = 0x1
    }

    public class OutfitContainer
    {
        /// <summary>
        /// Container for grouping together outfits and appearances.
        /// </summary>
        /// <param name="OFT">An outfit from which to create this container.</param>
        public OutfitContainer(Outfit OFT)
        {
            Oft = OFT;
            Appearance Apr = FileManager.Instance.GetAppearance(Oft.LightAppearance.UniqueID);

            if (Apr.ThumbnailID.TypeID != 0)
                LightAppearance = Apr;
            Apr = FileManager.Instance.GetAppearance(Oft.MediumAppearance.UniqueID);
            if (Apr.ThumbnailID.TypeID != 0)
                MediumAppearance = Apr;
            Apr = FileManager.Instance.GetAppearance(Oft.DarkAppearance.UniqueID);
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

    public class UISkinButtonClickedEventArgs : EventArgs
    {
        public int SkinType;
        public Outfit SelectedOutfit;
    }

    /// <summary>
    /// Raised whenever a button in the UISkinBrowser is clicked.
    /// </summary>
    /// <param name="SkinType">The type of skin; 0 means light, 1 means medium, 2 means dark.</param>
    /// <param name="SelectedOutfit">The selected outfit represented by the button clicked.</param>
    public delegate void UISkinButtonClicked(object Sender, UISkinButtonClickedEventArgs E);

    /// <summary>
    /// A skin browser is used to browse through thumbnails of heads and bodies.
    /// This is the base class for UIHeadBrowser and UIBodyBrowser.
    /// </summary>
    public class UISkinBrowser : UIControl
    {
        protected SkinType m_SelectedSkintype;
        protected AvatarSex m_Sex;

        protected List<OutfitContainer> m_LightAppearances = new List<OutfitContainer>();
        protected List<OutfitContainer> m_LightFemaleAppearances = new List<OutfitContainer>();
        protected List<OutfitContainer> m_MediumAppearances = new List<OutfitContainer>();
        protected List<OutfitContainer> m_MediumFemaleAppearances = new List<OutfitContainer>();
        protected List<OutfitContainer> m_DarkAppearances = new List<OutfitContainer>();
        protected List<OutfitContainer> m_DarkFemaleAppearances = new List<OutfitContainer>();
        protected List<Collection> m_Collections = new List<Collection>(), m_FemaleCollections = new List<Collection>();

        protected List<SkinBtnContainer> m_SkinBtns = new List<SkinBtnContainer>();

        protected UIButton m_SkinBrowserArrowLeft, m_SkinBrowserArrowRight;
        protected int m_BtnWidth, m_BtnHeight; //Width and height of a button in the browser.

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
        /// Sets the sex of the skins shown by this SkinBrowser.
        /// </summary>
        public AvatarSex Sex
        {
            set
            {
                m_Sex = value;
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
        public UISkinBrowser(UIScreen Screen, UIControl Ctrl, int SkinType, AvatarSex Sex) : base(Ctrl, Screen)
        {
            m_KeyboardInput = false;

            Position = Position + Screen.Position;

            m_SelectedSkintype = (SkinType)SkinType;
            m_Sex = Sex;

            //DrawOrder = (int)DrawOrderEnum.Game; //Default

            m_SkinBrowserArrowLeft = new UIButton("SkinBrowserArrowLeft",
                Position + new Vector2(5, Size.Y - 70), Screen,
                FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skinbrowserarrowleft));
            m_SkinBrowserArrowLeft.DrawOrder = (int)DrawOrderEnum.UI;
            m_SkinBrowserArrowLeft.OnButtonClicked += M_SkinBrowserArrowLeft_OnButtonClicked;
            m_SkinBrowserArrowRight = new UIButton("SkinBrowserArrowRight", 
                Position + new Vector2(Size.X - 45, Size.Y - 70), Screen,
                FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.person_edit_skinbrowserarrowright));
            m_SkinBrowserArrowRight.DrawOrder = (int)DrawOrderEnum.UI;
            m_SkinBrowserArrowRight.OnButtonClicked += M_SkinBrowserArrowRight_OnButtonClicked;
        }

        private void M_SkinBrowserArrowLeft_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            m_Index++;
        }

        private void M_SkinBrowserArrowRight_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            if(Index >= 1)
                m_Index--;
        }

        protected Texture2D m_Thumb;
        protected int m_Counter = 0, m_Index = 0, m_NumberOfSkinsToDisplay = 21;
        protected float m_Depth = 0.0f;

        protected int[,] m_Map = new int[,]
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
        protected bool IsMouseOverButton(InputHelper Input, SkinBtnContainer Button, Vector2 BtnPosition)
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
            m_SkinBrowserArrowLeft.Update(Helper, GTime);
            m_SkinBrowserArrowRight.Update(Helper, GTime);

            base.Update(Helper, GTime);
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            m_SkinBrowserArrowLeft.Draw(SBatch, m_Depth);
            m_SkinBrowserArrowRight.Draw(SBatch, m_Depth);

            base.Draw(SBatch, LayerDepth);
        }
    }
}
