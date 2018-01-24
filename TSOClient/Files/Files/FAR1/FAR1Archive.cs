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
using System.Reflection;
using log4net;

namespace Files.FAR1
{
    public class FAR1Archive : IDisposable
    {
        private EntryContainer m_Entries = new EntryContainer();
        private string m_Path;
        private FileReader m_Reader;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public FAR1Archive(string Path)
        {
            m_Path = Path;
        }

        /// <summary>
        /// Reads all entries in the archive into memory.
        /// </summary>
        /// <param name="ThrowException">Wether or not to throw an exception if the archive was not a FAR. If false, function will return.</param>
        public void ReadArchive(bool ThrowException)
        {
            if (m_Reader == null)
                m_Reader = new FileReader(m_Path, false);

            lock (m_Reader)
            {
                ASCIIEncoding Enc = new ASCIIEncoding();
                string MagicNumber = Enc.GetString(m_Reader.ReadBytes(8));

                if (!MagicNumber.Equals("FAR!byAZ", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ThrowException)
                        throw new FAR1Exception("MagicNumber was wrong - FAR1Archive.cs!");
                    else
                    {
                        m_Reader.Close();
                        return;
                    }
                }

                m_Reader.ReadUInt32(); //Version.
                m_Reader.Seek(m_Reader.ReadUInt32());

                uint NumFiles = m_Reader.ReadUInt32();

                for (int i = 0; i < NumFiles; i++)
                {
                    FAR1Entry Entry = new FAR1Entry();
                    Entry.CompressedDataSize = m_Reader.ReadUInt32();
                    Entry.DecompressedDataSize = m_Reader.ReadUInt32();
                    Entry.DataOffset = m_Reader.ReadUInt32();
                    Entry.FilenameLength = m_Reader.ReadUShort();
                    Entry.FilenameHash = FileUtilities.GenerateHash(Enc.GetString(m_Reader.ReadBytes(Entry.FilenameLength)));

                    m_Entries.Add(Entry);
                }
            }
        }

        /// <summary>
        /// Returns an entry in this archive as a Stream instance.
        /// Throws a FAR3Exception if entry was not found.
        /// </summary>
        /// <param name="ID">ID of the entry to grab from archive.</param>
        /// <returns>The entry's data as a Stream instance.</returns>
        public Stream GrabEntry(byte[] FilenameHash)
        {
            if (!ContainsEntry(FilenameHash))
                throw new FAR1Exception("Couldn't find entry - FAR1Archive.cs!");

            FAR1Entry Entry = m_Entries[FilenameHash];

            if(m_Reader == null)
                m_Reader = new FileReader(File.Open(m_Path, FileMode.Open), false);

            m_Reader.Seek(Entry.DataOffset);

            MemoryStream Data = new MemoryStream(m_Reader.ReadBytes((int)Entry.DecompressedDataSize));
            return Data;
        }

        /// <summary>
        /// Returns the given entries as a List of Stream instances.
        /// Throws a FAR3Exception if an entry wasn't found.
        /// </summary>
        /// <param name="Entries">A List of UniqueFileID of entries to grab.</param>
        /// <returns>The entries as a List of Stream instances.</returns>
        public List<Stream> GrabEntries(List<byte[]> Entries)
        {
            List<Stream> GrabbedEntries = new List<Stream>();

            for (int i = 0; i < Entries.Count; i++)
            {
                if (!m_Entries.Contains(Entries[i]))
                    throw new FAR1Exception("Couldn't find entry - FAR1Archive.cs!");
            }

            for (int i = 0; i < Entries.Count; i++)
                GrabbedEntries.Add(GrabEntry(Entries[i]));

            return GrabbedEntries;
        }

        /// <summary>
        /// Returns all entries.
        /// </summary>
        /// <returns>The entries as a List of FAR1Entries.</returns>
        public List<FAR1Entry> GrabAllEntries()
        {
            List<FAR1Entry> GrabbedEntries = new List<FAR1Entry>();

            foreach (FAR1Entry Entry in m_Entries)
                GrabbedEntries.Add(Entry);

            return GrabbedEntries;
        }

        /// <summary>
        /// Does this archive contain the specified entry?
        /// </summary>
        /// <param name="FilenameHash">The hashed filename of the entry to search for.</param>
        /// <returns>True if entry was found, false otherwise.</returns>
        public bool ContainsEntry(byte[] FilenameHash)
        {
            return m_Entries.Contains(FilenameHash);
        }

        ~FAR1Archive()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this FAR1Archive instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this FAR1Archive instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Reader != null)
                    m_Reader.Close();

                // Prevent the finalizer from calling ~FAR1Archive, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("FAR1Archive not explicitly disposed!");
        }
    }
}
