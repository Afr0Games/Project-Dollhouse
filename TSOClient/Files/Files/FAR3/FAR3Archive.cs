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
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Files.FAR3
{
    /// <summary>
    /// Represents a FAR version 3 archive.
    /// </summary>
    public class FAR3Archive
    {
        private Dictionary<ulong, FAR3Entry> m_Entries = new Dictionary<ulong, FAR3Entry>();
        private string m_Path;
        private FileReader m_Reader;

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
            if (m_Reader == null)
                m_Reader = new FileReader(m_Path, false);

            lock(m_Reader)
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
                    Entry.DataType = m_Reader.ReadByte();
                    Entry.DataOffset = m_Reader.ReadUInt32();
                    Entry.Flags = m_Reader.ReadUShort();
                    Entry.FileNameLength = m_Reader.ReadUShort();
                    Entry.TypeID = m_Reader.ReadUInt32();
                    Entry.FileID = m_Reader.ReadUInt32();
                    Entry.Filename = Enc.GetString(m_Reader.ReadBytes(Entry.FileNameLength));

                    UniqueFileID ID = new UniqueFileID(Entry.TypeID, Entry.FileID);

                    if(!m_Entries.ContainsKey(ID.UniqueID))
                        m_Entries.Add(ID.UniqueID, Entry);
                }
            }

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
            if (!ContainsEntry(ID))
                throw new FAR3Exception("Couldn't find entry - FAR3Archive.cs!");

            FAR3Entry Entry = m_Entries[ID];

            if(m_Reader == null)
                m_Reader = new FileReader(File.Open(m_Path, FileMode.Open, FileAccess.Read, FileShare.Read), false);

            m_Reader.Seek((long)Entry.DataOffset);

            if (Entry.DecompressedDataSize > Entry.CompressedDataSize)
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

                    return new MemoryStream(DecompressedData);
                }
            }

            return new MemoryStream(m_Reader.ReadBytes((int)Entry.DecompressedDataSize));
        }

        /// <summary>
        /// Returns all entries.
        /// Throws a FAR3Exception if an entry wasn't found.
        /// </summary>
        /// <returns>The entries as a List of FAR3Entries.</returns>
        public List<FAR3Entry> GrabAllEntries()
        {
            List<FAR3Entry> GrabbedEntries = new List<FAR3Entry>();

            foreach (KeyValuePair<ulong, FAR3Entry> KVP in m_Entries)
                GrabbedEntries.Add(KVP.Value);

            return GrabbedEntries;
        }

        /// <summary>
        /// Does this archive contain the specified entry?
        /// </summary>
        /// <param name="ID">ID of the entry to search for.</param>
        /// <returns>True if entry was found, false otherwise.</returns>
        public bool ContainsEntry(ulong ID)
        {
            return m_Entries.ContainsKey(ID);
        }
    }
}
