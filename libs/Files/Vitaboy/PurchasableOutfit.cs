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
using System.Reflection;
using log4net;

namespace Files.Vitaboy
{
    public class PurchasableOutfit : IDisposable
    {
        private FileReader m_Reader;
        public uint OutfitType;
        public UniqueFileID OutfitID;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        ~PurchasableOutfit()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this PurchasableOutfit instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this PurchasableOutfit instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Reader != null)
                    m_Reader.Dispose();

                // Prevent the finalizer from calling ~PurchasableOutfit, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("PurchasableOutfit not explicitly disposed!");
        }
    }
}
