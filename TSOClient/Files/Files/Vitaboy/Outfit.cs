/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the SimsLib.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Linq;
using System.Text;
using System.IO;

namespace Files.Vitaboy
{
    public enum OutfitRegion : uint
    {
        Head = 0,
        Body = 18
    }

    /// <summary>
    /// Outfits collect together the light-, medium-, and dark-skinned versions of an appearance 
    /// and associate them collectively with a hand group and a body region (head or body).
    /// </summary>
    public class Outfit : IDisposable
    {
        private FileReader m_Reader;
        public UniqueFileID LightAppearance, MediumAppearance, DarkAppearance;
        private uint m_HandGroup;
        public OutfitRegion Region;

        public Outfit(Stream Data)
        {
            m_Reader = new FileReader(Data, true);

            uint Version = m_Reader.ReadUInt32(); //Version
            m_Reader.ReadUInt32(); //Unknown

            LightAppearance = ReadBackwardsID();
            MediumAppearance = ReadBackwardsID();
            DarkAppearance = ReadBackwardsID();
            m_HandGroup = m_Reader.ReadUInt32();
            Region = (OutfitRegion)Endian.SwapUInt32(m_Reader.ReadUInt32());
        }

        public UniqueFileID HandgroupID
        {
            get { return new UniqueFileID((uint)18, m_HandGroup); }
        }

        /// <summary>
        /// Some genious at Maxis decided to store these backwards from how they're stored in archives o_O
        /// </summary>
        private UniqueFileID ReadBackwardsID()
        {
            uint FileID = m_Reader.ReadUInt32();
            uint TypeID = m_Reader.ReadUInt32();

            return new UniqueFileID(TypeID, FileID);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool CleanUpManagedResources)
        {
            if (CleanUpManagedResources)
            {
                if(m_Reader != null)
                    m_Reader.Dispose();
            }
        }
    }
}
