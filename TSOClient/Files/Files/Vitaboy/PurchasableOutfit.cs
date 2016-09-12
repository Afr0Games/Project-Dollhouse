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
    public class PurchasableOutfit
    {
        private FileReader m_Reader;
        public uint OutfitType;
        public UniqueFileID OutfitID;

        /// <summary>
        /// Constructs a new instance of the PurchasableOutfit class.
        /// </summary>
        /// <param name="Data">A Stream of data retrieved from a FAR3 archive.</param>
        public PurchasableOutfit(Stream Data)
        {
            m_Reader = new FileReader(Data, true);

            m_Reader.ReadUInt32(); //Version
            m_Reader.ReadUInt32(); //Unknown
            OutfitType = m_Reader.ReadUInt32();

            if(OutfitType != 0)
            {
                //A 4-byte unsigned integer specifying the type of data that follows; should be 0xA96F6D42 for cAssetKey
                m_Reader.ReadUInt32();
                uint FileID = m_Reader.ReadUInt32();
                uint TypeID = m_Reader.ReadUInt32();
                OutfitID = new UniqueFileID(TypeID, FileID);
            }

            m_Reader.ReadUInt32(); //Unknown.

            m_Reader.Close();
        }
    }
}
