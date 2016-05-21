/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the SimsLib.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.IO;

namespace Files.Manager
{
    /// <summary>
    /// An in-memory asset. Loaded and managed by FileManager.
    /// </summary>
    public class Asset
    {
        public DateTime LastAccessed = DateTime.Now, Cached = DateTime.Now;
        private uint m_Size;
        private ulong m_AssetID = 0;
        private byte[] m_AssetFilename;
        private object m_AssetData;

        /// <summary>
        /// Get the ID of this asset. May be null.
        /// </summary>
        public ulong AssetID
        {
            get { return m_AssetID; }
        }

        /// <summary>
        /// Get the hash of this asset's filename. May be null.
        /// </summary>
        public byte[] FilenameHash
        {
            get { return m_AssetFilename; }
        }

        /// <summary>
        /// Gets the size of this asset in bytes.
        /// </summary>
        public uint Size
        {
            get { return m_Size; }
        }

        /// <summary>
        /// Gets the data of this asset.
        /// </summary>
        public object AssetData
        {
            get
            {
                LastAccessed = DateTime.Now;
                return m_AssetData;
            }
        }

        /// <summary>
        /// Creates a new Asset instance.
        /// </summary>
        /// <param name="AssetID">ID of this asset.</param>
        /// <param name="Data">Data of asset.</param>
        public Asset(ulong AssetID, uint Size, object Data)
        {
            m_AssetID = AssetID;
            m_AssetData = Data;
            m_Size = Size;
            LastAccessed = DateTime.Now;
        }

        /// <summary>
        /// Creates a new Asset instance.
        /// </summary>
        /// <param name="AssetID">Hash of this asset's filename.</param>
        /// <param name="Data">Data of asset.</param>
        public Asset(byte[] AssetID, uint Size, object Data)
        {
            m_AssetFilename = AssetID;
            m_AssetData = Data;
            m_Size = Size;
            LastAccessed = DateTime.Now;
        }
    }
}
