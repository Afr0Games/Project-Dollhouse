/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.IO;

namespace Files.IFF
{
    /// <summary>
    /// Type of OBJD.
    /// </summary>
    [Serializable()]
    public enum OBJDType
    {
        Unknown = 0,
        //Character or NPC
        Person = 2,
        //Buyable objects
        Normal = 4,
        //Roaches, Stoves2, TrClownGen, AnimTester, HelpSystem, JobFinder, NPCController, Stoves, 
        //Tutorial, VisitGenerator, phonecall, unsnacker, CCPhonePlugin, EStove
        SimType = 7,
        //Stairs, doors, pool diving board & ladder, windows(?)
        Portal = 8,
        Cursor = 9,
        PrizeToken = 10,
        //Temporary location for drop or shoo
        Internal = 11,
        Food = 34
    }

    /// <summary>
    /// This is an object definition, the main chunk for an object and the first loaded by the VM. 
    /// There can be multiple master OBJDs in an IFF, meaning that one IFF file can define multiple objects. 
    /// </summary>
    public class OBJD : IFFChunk 
    {
        private byte m_NumFields = 0;

        public uint Version = 0;
        public ushort InitialStackSize = 0;
        public ushort BaseGraphicID = 0;
        public ushort NumGraphics = 0;
        
        /// <summary>
        /// The ID of the BHAV that contains the code making up the main function for this object.
        /// A value of less than 4096 refers to a global subroutine, less than 8192 means local,
        /// anything above that means semi-global.
        /// </summary>
        public ushort MainID = 0;

        public ushort GardeningID;
        public ushort TTABID;
        public ushort InteractionGroup;
        public OBJDType ObjectType;
        public ushort MasterID = 0;
        public short SubIndex = 0;
        public ushort WashHandsID = 0;
        public ushort AnimTableID = 0;
        public uint GUID = 0;
        public ushort Disabled = 0;
        public ushort Portal = 0;
        public ushort Price = 0;
        public ushort BodyStringsID = 0;

        public ushort SLOTID = 0;
        public ushort AllowIntersection = 0;
        public ushort UsesFnTable = 0;
        public ushort Bitfield1 = 0;
        public ushort PrepareFoodID = 0;
        public ushort CookFoodID = 0;
        public ushort PlaceOnSurfaceID = 0;
        public ushort DisposeID = 0;
        public ushort EatFoodID = 0;
        public ushort PickupFromSLOTID = 0;
        public ushort WashDishID = 0;
        public ushort EatingSurfaceID = 0;
        public ushort Sit = 0;
        public ushort Stand = 0;
        public ushort SalePrice = 0;
        public ushort InitialDepreciation = 0;
        public ushort DailyDepreciation = 0;
        public ushort SelfDepreciating = 0;
        public ushort DepreciationLimit = 0;
        public ushort RoomFlags = 0;
        public ushort FunctionFlags = 0;
        public ushort CatalogStringsID = 0;

        public ushort Global = 0;
        public ushort BHAV_Init = 0;
        public ushort BHAV_Place = 0;
        public ushort BHAV_UserPickup = 0;
        public ushort WallStyle = 0;
        public ushort BHAV_Load = 0;
        public ushort BHAV_UserPlace = 0;
        public ushort ObjectVersion = 0;
        public ushort BHAV_RoomChange = 0;
        public ushort MotiveEffectsID = 0;
        public ushort BHAV_Cleanup = 0;
        public ushort BHAV_LevelInfo = 0;
        public ushort CatalogID = 0;
        public ushort BHAV_ServingSurface = 0;
        public ushort LevelOffset = 0;
        public ushort Shadow = 0;

        public ushort NumAttributes = 0;
        public ushort BHAV_Clean = 0;
        public ushort BHAV_QueueSkipped = 0;
        public ushort FrontDirection = 0;
        public ushort BHAV_WallAdjacencyChanged = 0;
        public ushort MyLeadObject = 0;
        public ushort DynamicSpriteBaseId = 0;
        public ushort NumDynamicSprites = 0;

        public ushort ChairEntryFlags = 0;
        public ushort TileWidth = 0;
        public ushort InhibitSuitCopying = 0;
        public ushort BuildModeType = 0;
        public ushort OriginalGUID1 = 0;
        public ushort OriginalGUID2 = 0;
        public ushort SuitGUID1 = 0;
        public ushort SuitGUID2 = 0;
        public ushort BHAV_Pickup = 0;
        public ushort ThumbnailGraphic = 0;
        public ushort ShadowFlags = 0;
        public ushort FootprintMask = 0;
        public ushort BHAV_DynamicMultiTileUpdate = 0;
        public ushort ShadowBrightness = 0;
        public ushort BHAV_Repair = 0;

        public ushort WallStyleSpriteID = 0;
        public ushort RatingHunger = 0;
        public ushort RatingComfort = 0;
        public ushort RatingHygiene = 0;
        public ushort RatingBladder = 0;
        public ushort RatingEnergy = 0;
        public ushort RatingFun = 0;
        public ushort RatingRoom = 0;
        public ushort RatingSkillFlags = 0;

        /// <summary>
        /// Is this the master OBJD of a multi tile object?
        /// </summary>
        public bool IsMaster
        {
            get
            {
                return (SubIndex == -1) ? true : false;
            }
        }

        /// <summary>
        /// Is this object multi tile?
        /// </summary>
        public bool IsMultiTile
        {
            get
            {
                return MasterID != 0;
            }
        }

        public OBJD(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            Version = Reader.ReadUInt32();

            switch(Version)
            {
                case 136:
                    m_NumFields = 80;
                    break;
                case 138:
                    m_NumFields = 95;
                    break;
                case 139:
                    m_NumFields = 96;
                    break;
                case 140:
                    m_NumFields = 97;
                    break;
                case 141:
                    m_NumFields = 97;
                    break;
                case 142:
                    m_NumFields = 105;
                    break;
            }

            InitialStackSize = Reader.ReadUShort();
            BaseGraphicID = Reader.ReadUShort();
            NumGraphics = Reader.ReadUShort();
            MainID = Reader.ReadUShort();
            GardeningID = Reader.ReadUShort();
            TTABID = Reader.ReadUShort();
            InteractionGroup = Reader.ReadUShort();
            ObjectType = (OBJDType)Reader.ReadUShort();
            MasterID = Reader.ReadUShort();
            SubIndex = Reader.ReadInt16();
            WashHandsID = Reader.ReadUShort();
            AnimTableID = Reader.ReadUShort();
            GUID = Reader.ReadUInt32();
            Disabled = Reader.ReadUShort();
            Portal = Reader.ReadUShort();
            Price = Reader.ReadUShort();
            BodyStringsID = Reader.ReadUShort();
            SLOTID = Reader.ReadUShort();
            AllowIntersection = Reader.ReadUShort();
            UsesFnTable = Reader.ReadUShort();
            Bitfield1 = Reader.ReadUShort();
            PrepareFoodID = Reader.ReadUShort();
            CookFoodID = Reader.ReadUShort();
            PlaceOnSurfaceID = Reader.ReadUShort();
            DisposeID = Reader.ReadUShort();
            EatFoodID = Reader.ReadUShort();
            PickupFromSLOTID = Reader.ReadUShort();
            WashDishID = Reader.ReadUShort();
            EatingSurfaceID = Reader.ReadUShort();
            Sit = Reader.ReadUShort();
            Stand = Reader.ReadUShort();
            SalePrice = Reader.ReadUShort();
            InitialDepreciation = Reader.ReadUShort();
            DailyDepreciation = Reader.ReadUShort();
            SelfDepreciating = Reader.ReadUShort();
            DepreciationLimit = Reader.ReadUShort();
            RoomFlags = Reader.ReadUShort();
            FunctionFlags = Reader.ReadUShort();
            CatalogStringsID = Reader.ReadUShort();

            Global = Reader.ReadUShort();
            BHAV_Init = Reader.ReadUShort();
            BHAV_Place = Reader.ReadUShort();
            BHAV_UserPickup = Reader.ReadUShort();
            WallStyle = Reader.ReadUShort();
            BHAV_Load = Reader.ReadUShort();
            BHAV_UserPlace = Reader.ReadUShort();
            ObjectVersion = Reader.ReadUShort();
            BHAV_RoomChange = Reader.ReadUShort();
            MotiveEffectsID = Reader.ReadUShort();
            BHAV_Cleanup = Reader.ReadUShort();
            BHAV_LevelInfo = Reader.ReadUShort();
            CatalogID = Reader.ReadUShort();
            BHAV_ServingSurface = Reader.ReadUShort();
            LevelOffset = Reader.ReadUShort();
            Shadow = Reader.ReadUShort();
            NumAttributes = Reader.ReadUShort();

            BHAV_Clean = Reader.ReadUShort();
            BHAV_QueueSkipped = Reader.ReadUShort();
            FrontDirection = Reader.ReadUShort();
            BHAV_WallAdjacencyChanged = Reader.ReadUShort();
            MyLeadObject = Reader.ReadUShort();
            DynamicSpriteBaseId = Reader.ReadUShort();
            NumDynamicSprites = Reader.ReadUShort();

            ChairEntryFlags = Reader.ReadUShort();
            TileWidth = Reader.ReadUShort();
            InhibitSuitCopying = Reader.ReadUShort();
            BuildModeType = Reader.ReadUShort();
            OriginalGUID1 = Reader.ReadUShort();
            OriginalGUID2 = Reader.ReadUShort();
            SuitGUID1 = Reader.ReadUShort();
            SuitGUID2 = Reader.ReadUShort();
            BHAV_Pickup = Reader.ReadUShort();
            ThumbnailGraphic = Reader.ReadUShort();
            ShadowFlags = Reader.ReadUShort();
            FootprintMask = Reader.ReadUShort();
            BHAV_DynamicMultiTileUpdate = Reader.ReadUShort();
            ShadowBrightness = Reader.ReadUShort();
            BHAV_Repair = Reader.ReadUShort();

            if (m_NumFields > 80)
            {
                WallStyleSpriteID = Reader.ReadUShort();
                RatingHunger = Reader.ReadUShort();
                RatingComfort = Reader.ReadUShort();
                RatingHygiene = Reader.ReadUShort();
                RatingBladder = Reader.ReadUShort();
                RatingEnergy = Reader.ReadUShort();
                RatingFun = Reader.ReadUShort();
                RatingRoom = Reader.ReadUShort();
                RatingSkillFlags = Reader.ReadUShort();
            }

            m_Data = null;
        }
    }
}
