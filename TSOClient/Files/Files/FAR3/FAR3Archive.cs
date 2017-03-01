/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Files.FAR3
{
    /// <summary>
    /// Represents a FAR version 3 archive.
    /// </summary>
    public class FAR3Archive : IDisposable
    {
        private ConcurrentDictionary<ulong, FAR3Entry> m_Entries = new ConcurrentDictionary<ulong, FAR3Entry>();
        private string m_Path;
        private FileReader m_Reader;
        private ManualResetEvent m_FinishedReading = new ManualResetEvent(false);

        public FAR3Archive(string Path)
        {
            m_Path = Path;
        }

        /// <summary>
        /// Reads all the entries in the archive into memory.
        /// </summary>
        /// <param name="ThrowException">Wether or not to throw an exception if the archive was not a FAR3. If false, function will return.</param>
        public bool ReadArchive(bool ThrowException)
        {
            m_FinishedReading.Reset();

            if (m_Reader == null)
                m_Reader = new FileReader(m_Path, false);

            lock (m_Reader)
            {
                ASCIIEncoding Enc = new ASCIIEncoding();
                string MagicNumber = Enc.GetString(m_Reader.ReadBytes(8));

                if (!MagicNumber.Equals("FAR!byAZ", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ThrowException)
                        throw new FAR3Exception("MagicNumber was wrong - FAR3Archive.cs!");
                    else
                    {
                        m_Reader.Close();
                        return false;
                    }
                }

                m_Reader.ReadUInt32(); //Version.
                m_Reader.Seek((long)m_Reader.ReadUInt32());

                uint NumFiles = m_Reader.ReadUInt32();

                for (int i = 0; i < NumFiles; i++)
                {
                    FAR3Entry Entry = new FAR3Entry();
                    Entry.DecompressedDataSize = m_Reader.ReadUInt32();
                    byte[] Dummy = m_Reader.ReadBytes(3);
                    Entry.CompressedDataSize = (uint)((Dummy[0] << 0) | (Dummy[1] << 8) | (Dummy[2]) << 16);
                    m_Reader.ReadByte(); //Unknown.
                    Entry.DataOffset = m_Reader.ReadUInt32();
                    Entry.Flags = m_Reader.ReadUShort();
                    Entry.FileNameLength = m_Reader.ReadUShort();
                    Entry.TypeID = m_Reader.ReadUInt32();
                    Entry.FileID = m_Reader.ReadUInt32();
                    Entry.Filename = Enc.GetString(m_Reader.ReadBytes(Entry.FileNameLength));

                    UniqueFileID ID = new UniqueFileID(Entry.TypeID, Entry.FileID);

                    if (!m_Entries.ContainsKey(ID.UniqueID))
                        m_Entries.AddOrUpdate(ID.UniqueID, Entry, (Key, ExistingValue) => ExistingValue = Entry);
                }
            }

            m_FinishedReading.Set();

            return true;
        }

        /// <summary>
        /// Returns an entry in this archive as a Stream instance.
        /// Throws a FAR3Exception if entry was not found.
        /// </summary>
        /// <param name="ID">ID of the entry to grab from archive.</param>
        /// <returns>The entry's data as a Stream instance.</returns>
        public Stream GrabEntry(ulong ID)
        {
            m_FinishedReading.WaitOne();

            if (!ContainsEntry(ID))
                throw new FAR3Exception("Couldn't find entry - FAR3Archive.cs!");

            FAR3Entry Entry = m_Entries[ID];

            lock (m_Reader)
            {
                m_Reader.Seek((long)Entry.DataOffset);

                switch (Entry.TypeID)
                {
                    case 1: //BMP
                    case 2: //TGA
                    case 5: //SKEL
                    case 7: //ANIM
                    case 9: //MESH
                    case 11: //BND
                    case 12: //APR
                    case 13: //OFT
                    case 15: //PO
                    case 16: //COL
                    case 18: //HAG
                    case 20: //JPG
                    case 24: //PNG
                        return Decompress(Entry);
                    case 14: //PNG, uncompressed
                    default:
                        MemoryStream MemStream = new MemoryStream(m_Reader.ReadBytes((int)Entry.DecompressedDataSize));
                        MemStream.Seek(0, SeekOrigin.Begin);
                        return MemStream;
                }
            }
        }

        /// <summary>
        /// Returns all entries.
        /// Throws a FAR3Exception if an entry wasn't found.
        /// THIS METHOD IS USED BY MR. SHIPPER - DO NOT DELETE!
        /// </summary>
        /// <returns>The entries as a List of FAR3Entries.</returns>
        public List<FAR3Entry> GrabAllEntries()
        {
            List<FAR3Entry> GrabbedEntries = new List<FAR3Entry>();
            foreach (KeyValuePair<ulong, FAR3Entry> KVP in m_Entries)
                GrabbedEntries.Add(KVP.Value);

            return GrabbedEntries;
        }

        private Stream Decompress(FAR3Entry Entry)
        {
            m_Reader.ReadBytes(9);
            uint CompressedSize = m_Reader.ReadUInt32();
            ushort CompressionID = m_Reader.ReadUShort();

            if (CompressionID == 0xFB10)
            {
                byte[] Dummy = m_Reader.ReadBytes(3);
                uint DecompressedSize = (uint)((Dummy[0] << 0x10) | (Dummy[1] << 0x08) | +Dummy[2]);

                Decompresser Dec = new Decompresser();
                Dec.CompressedSize = CompressedSize;
                Dec.DecompressedSize = DecompressedSize;

                byte[] DecompressedData = Dec.Decompress(m_Reader.ReadBytes((int)CompressedSize));

                MemoryStream MemStream = new MemoryStream(DecompressedData);
                MemStream.Seek(0, SeekOrigin.Begin);

                return MemStream;
            }
            else
            {
                m_Reader.Seek(m_Reader.Position - 15);
                return new MemoryStream(m_Reader.ReadBytes((int)Entry.DecompressedDataSize));
            }
        }

        /// <summary>
        /// Does this archive contain the specified entry?
        /// </summary>
        /// <param name="ID">ID of the entry to search for.</param>
        /// <returns>True if entry was found, false otherwise.</returns>
        public bool ContainsEntry(ulong ID)
        {
            m_FinishedReading.WaitOne();

            return m_Entries.ContainsKey(ID);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool CleanUpNativeAndManagedResources)
        {
            if (CleanUpNativeAndManagedResources)
            {
                if (m_Reader != null)
                    m_Reader.Close();
                if (m_FinishedReading != null)
                    m_FinishedReading.Close();
            }
        }
    }
}
