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

namespace Files.DBPF
{
    public enum GroupIDs : uint
    {
        Multiplayer = 0x29dd0888,
        Custom = 0x29daa4a6,
        CustomTrks = 0x29d9359d,
        Tracks = 0xa9c6c89a,
        TrackDefs = 0xfdbdbf87,
        tsov2 = 0x69c6c943,
        Samples = 0x9dbdbf89,
        HitLists = 0x9dbdbf74,
        HitListsTemp = 0xc9c6c9b3,
        EP2 = 0xdde8f5c6,
        EP5Samps = 0x8a6fcc30
    }

    public enum TypeIDs : uint
    {
        HLS = 0x7b1acfcd,
        TLO = 0x9d796db4,
        TRK = 0x5d73a611,
        Sounds = 0x2026960b //WAV, UTK and XA
    }

    public class DBPFArchive
    {
        private Dictionary<UniqueFileID, DBPFEntry> m_Entries = new Dictionary<UniqueFileID, DBPFEntry>();
        private string m_Path;
        private FileReader m_Reader;
        public uint IndexEntryCount;
        public uint IndexOffset;
        public uint IndexSize;

        public DBPFArchive(string Path)
        {
            m_Path = Path;
        }

        /// <summary>
        /// Reads all entries in this archive into memory.
        /// </summary>
        /// <param name="ThrowException">Wether or not to throw an exception if the archive was not a DBPF. If false, function will return.</param>
        public bool ReadArchive(bool ThrowException)
        {
            if (m_Reader == null)
            {
                try
                {
                    m_Reader = new FileReader(m_Path, false);
                }
                //This will be thrown because of file access priviledges or because an archive is being tentatively opened twice.
                catch (Exception e)
                {
                    if (ThrowException)
                        throw e;
                    else
                        return false;
                }
            }

            lock (m_Reader)
            {
                ASCIIEncoding Enc = new ASCIIEncoding();
                string MagicNumber = Enc.GetString(m_Reader.ReadBytes(4));

                if (!MagicNumber.Equals("DBPF", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ThrowException)
                        throw new DBPFException("MagicNumber was wrong - DBPFArchive.cs!");
                    else
                    {
                        m_Reader.Close();
                        return false;
                    }
                }

                m_Reader.ReadUInt32();  //MajorVersion
                m_Reader.ReadUInt32();  //MinorVersion
                m_Reader.ReadBytes(12); //Reserved.
                m_Reader.ReadBytes(4);  //Date created.
                m_Reader.ReadBytes(4);  //Date modified.
                m_Reader.ReadUInt32();  //Index major version.
                IndexEntryCount = m_Reader.ReadUInt32();
                IndexOffset = m_Reader.ReadUInt32();
                IndexSize = m_Reader.ReadUInt32();

                m_Reader.Seek(IndexOffset);

                for(int i = 0; i < IndexEntryCount; i++)
                {
                    DBPFEntry Entry = new DBPFEntry();
                    Entry.TypeID = m_Reader.ReadUInt32();
                    Entry.GroupID = m_Reader.ReadUInt32();
                    Entry.InstanceID = m_Reader.ReadUInt32();
                    Entry.FileOffset = m_Reader.ReadUInt32();
                    Entry.FileSize = m_Reader.ReadUInt32();

                    UniqueFileID ID = new UniqueFileID(Entry.TypeID, Entry.InstanceID, Entry.GroupID);
                    Entry.EntryID = ID;
                    m_Entries.Add(ID, Entry);
                }
            }

            return true;
        }

        /// <summary>
        /// Returns an entry in this archive as a Stream instance.
        /// Throws a DBPFException if entry was not found.
        /// </summary>
        /// <param name="ID">ID of the entry to grab from archive.</param>
        /// <returns>The entry's data as a Stream instance.</returns>
        public Stream GrabEntry(UniqueFileID ID)
        {
            if (!ContainsEntry(ID))
                throw new DBPFException("Couldn't find entry - DBPFArchive.cs!");

            DBPFEntry Entry = m_Entries[ID];

            if(m_Reader == null)
                m_Reader = new FileReader(File.Open(m_Path, FileMode.Open, FileAccess.Read, FileShare.Read), false);

            m_Reader.Seek(Entry.FileOffset);

            MemoryStream Data = new MemoryStream(m_Reader.ReadBytes((int)Entry.FileSize));

            return Data;
        }

        public List<DBPFEntry> GrabEntriesForTypeID(uint TypeID)
        {
            List<DBPFEntry> ReturnedEntries = new List<DBPFEntry>();

            foreach(KeyValuePair<UniqueFileID, DBPFEntry> Entry in m_Entries)
            {
                if (Entry.Key.TypeID == TypeID)
                    ReturnedEntries.Add(Entry.Value);
            }

            return ReturnedEntries;
        }

        /// <summary>
        /// Does this archive contain the specified entry?
        /// </summary>
        /// <param name="ID">ID of the entry to search for.</param>
        /// <returns>True if entry was found, false otherwise.</returns>
        public bool ContainsEntry(UniqueFileID ID)
        {
            return m_Entries.ContainsKey(ID);
        }
    }
}
