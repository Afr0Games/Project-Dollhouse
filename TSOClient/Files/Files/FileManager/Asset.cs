using System;
using System.Collections.Generic;
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
        private ulong m_AssetID = 0;
        private byte[] m_AssetFilename;
        private Stream m_AssetData;

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
        /// Gets the data of this asset.
        /// </summary>
        public Stream AssetData
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
        public Asset(ulong AssetID, Stream Data)
        {
            m_AssetID = AssetID;
            m_AssetData = Data;
            LastAccessed = DateTime.Now;
        }

        /// <summary>
        /// Creates a new Asset instance.
        /// </summary>
        /// <param name="AssetID">Hash of this asset's filename.</param>
        /// <param name="Data">Data of asset.</param>
        public Asset(byte[] AssetID, Stream Data)
        {
            m_AssetFilename = AssetID;
            m_AssetData = Data;
            LastAccessed = DateTime.Now;
        }
    }
}
