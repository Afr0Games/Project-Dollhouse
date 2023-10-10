/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;
using log4net;

namespace Files.FAR1
{
    /// <summary>
    /// Class for reading and writing FAR1 archives.
    /// </summary>
    public class FAR1Archive : IDisposable
    {
        private Dictionary<byte[], FAR1Entry> m_Entries = new Dictionary<byte[], FAR1Entry>(new ByteArrayComparer());
        private string m_Path;
        private FileReader m_Reader;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The fully qualified path to this archive.
        /// Used by Iffinator.
        /// </summary>
        public string Path
        {
            get { return m_Path; }
        }

        public FAR1Archive(string Path)
        {
            m_Path = Path;
        }

        /// <summary>
        /// Reads all entries in the archive into memory.
        /// </summary>
        /// <param name="ThrowException">Wether or not to throw an exception if the archive was not a FAR. If false, function will return.</param>
        public bool ReadArchive(bool ThrowException)
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
                        return false;
                    }
                }

                if (m_Reader.ReadUInt32() != 1) //Version.
                    return false;

                m_Reader.Seek(m_Reader.ReadUInt32());

                uint NumFiles = m_Reader.ReadUInt32();

                for (int i = 0; i < NumFiles; i++)
                {
                    FAR1Entry Entry = new FAR1Entry();
                    Entry.CompressedDataSize = m_Reader.ReadUInt32();
                    Entry.DecompressedDataSize = m_Reader.ReadUInt32();
                    Entry.DataOffset = m_Reader.ReadUInt32();
                    Entry.FilenameLength = m_Reader.ReadUShort();

                    if (IsAssemblyDebugBuild(Assembly.GetExecutingAssembly()))
                    {
                        string Filename = Enc.GetString(m_Reader.ReadBytes(Entry.FilenameLength));
                        Entry.Filename = Filename;
                        Entry.EntryType = GetEntryType(Filename);
                        Entry.FilenameHash = FileUtilities.GenerateHash(Filename);
                    }
                    else
                        Entry.FilenameHash = FileUtilities.GenerateHash(Enc.GetString(m_Reader.ReadBytes(Entry.FilenameLength)));

                    m_Entries.Add(Entry.FilenameHash, Entry);
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the entry type for an entry. Only used in debug builds, because string search/parsing is SLOW.
        /// </summary>
        /// <param name="Filename">The filename to check.</param>
        /// <returns>The type of the entry, based on the filename.</returns>
        private FAR1EntryType GetEntryType(string Filename)
        {
            if (Filename.Contains(".iff"))
                return FAR1EntryType.IFF;
            else if (Filename.Contains(".otf"))
                return FAR1EntryType.OTF;
            else if (Filename.Contains(".spf"))
                return FAR1EntryType.SPF;
            else if (Filename.Contains(".flr"))
                return FAR1EntryType.FLR;
            else if (Filename.Contains(".wll"))
                return FAR1EntryType.WLL;
            else if (Filename.Contains(".bmp"))
                return FAR1EntryType.BMP;

            return FAR1EntryType.UNK; //Unknown
        }

        private bool IsAssemblyDebugBuild(Assembly assembly)
        {
            return assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled);
        }

        /// <summary>
        /// Returns an entry in this archive as a Stream instance.
        /// Throws a FAR1Exception if entry was not found.
        /// </summary>
        /// <param name="FilenameHash">The hash of the filename to grab from the archive.</param>
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
        /// Throws a FAR1Exception if an entry wasn't found.
        /// </summary>
        /// <param name="Entries">A List of filename hashes of entries to grab.</param>
        /// <returns>The entries as a List of Stream instances.</returns>
        public List<Stream> GrabEntries(List<byte[]> Entries)
        {
            List<Stream> GrabbedEntries = new List<Stream>();

            for (int i = 0; i < Entries.Count; i++)
            {
                if (!m_Entries.ContainsKey(Entries[i]))
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

            foreach (KeyValuePair<byte[], FAR1Entry> Entry in m_Entries)
                GrabbedEntries.Add(Entry.Value);

            return GrabbedEntries;
        }

        /// <summary>
        /// Does this archive contain the specified entry?
        /// </summary>
        /// <param name="FilenameHash">The hashed filename of the entry to search for.</param>
        /// <returns>True if entry was found, false otherwise.</returns>
        public bool ContainsEntry(byte[] FilenameHash)
        {
            return m_Entries.ContainsKey(FilenameHash);
        }

        public void AddEntry(FAR1Entry Entry, Stream EntryData, bool Backup = true)
        {
            m_Entries.Add(Entry.FilenameHash, Entry);

            List<Stream> DataOfEntries = GrabEntries(GetHashList());
            DataOfEntries.Add(EntryData);

            CreateNew(Path, DataOfEntries, Backup);
        }

        /// <summary>
        /// A helper function for getting all the hashes for all the entries in this archive.
        /// </summary>
        /// <returns>A list of hashes.</returns>
        public List<byte[]> GetHashList()
        {
            List<byte[]> Hashes = new List<byte[]>();

            foreach(KeyValuePair<byte[], FAR1Entry> KVP in m_Entries)
                Hashes.Add(KVP.Key);

            return Hashes;
        }

        /// <summary>
        /// Creates a new FAR archive at the designated path. The archive will contain all the entries in
        /// this FAR1Archive instance. If this function is not passed a list of byte arrays for the entries'
        /// data, it will use the data read from this FAR1Archive instance.
        /// </summary>
        /// <param name="ArchivePath">The fully qualified path to the archive to create.</param>
        /// <param name="Entries">The entries in the archive.</param>
        /// <param name="Data">(Optional) The data of the entries.</param>
        /// <param name="Backup">(Optional) Will the original archive be backed up if <paramref name="ArchivePath"/>
        /// coincides with an already existing archive? Set to true by default.</param>
        public void CreateNew(string ArchivePath, List<Stream> Data = null, bool Backup = true)
        {
            string RandomFile = System.IO.Path.GetTempFileName();

            //No need for other apps to access this file, so drop the FileAccess parameter.
            BinaryWriter Writer = new BinaryWriter(File.Open(RandomFile, FileMode.Open));
            //TODO: Will this work?
            Writer.Write(ASCIIEncoding.ASCII.GetBytes("FAR!byAZ"));
            Writer.Write((uint)1); //Version

            Writer.Write((uint)0); //Offset to first entry.
            //This isn't *actually* the first entry offset, but the offset of where to store that offset...
            int FirstEntryOffset = (int)Writer.BaseStream.Position;

            Writer.Write((uint)m_Entries.Count);
            //THIS is the *actual* first entry offset.
            int StartOfEntries = (int)Writer.BaseStream.Position;

            Writer.Seek(FirstEntryOffset, SeekOrigin.Begin);
            Writer.Write((uint)StartOfEntries);
            Writer.Seek(StartOfEntries, SeekOrigin.Begin);

            List<int> EntryOffsets = new List<int>();

            foreach (KeyValuePair<byte[], FAR1Entry> Entry in m_Entries)
            {
                Writer.Write(Entry.Value.CompressedDataSize);
                Writer.Write(Entry.Value.DecompressedDataSize);

                EntryOffsets.Add((int)Writer.BaseStream.Position);

                Writer.Write((uint)0); //Dataoffset.
                Writer.Write(Entry.Value.FilenameLength);
                Writer.Write(ASCIIEncoding.ASCII.GetBytes(Entry.Value.Filename)); //TODO: Will this work?
            }

            List<uint> DataOffsets = new List<uint>();
            int Index = 0;

            if (Data != null)
            {
                foreach (Stream DataEntry in Data)
                {
                    DataOffsets.Add((uint)Writer.BaseStream.Position);
                    MemoryStream MemStream = new MemoryStream();
                    DataEntry.CopyTo(MemStream);
                    Writer.Write(MemStream.ToArray());

                    Writer.Seek(EntryOffsets[Index], SeekOrigin.Begin);
                    Writer.Write(DataOffsets[Index]);
                    Writer.Seek((int)(DataOffsets[Index] + (DataEntry.Length - 1)), SeekOrigin.Begin);

                    Index++;
                }
            }
            else
            {
                foreach (Stream DataEntry in GrabEntries(GetHashList()))
                {
                    DataOffsets.Add((uint)Writer.BaseStream.Position);

                    MemoryStream MemStream = new MemoryStream();
                    DataEntry.CopyTo(MemStream);
                    Writer.Write(MemStream.ToArray());

                    Writer.Seek(EntryOffsets[Index], SeekOrigin.Begin);
                    Writer.Write(DataOffsets[Index]);
                    Writer.Seek((int)(DataOffsets[Index] + (DataEntry.Length - 1)), SeekOrigin.Begin);

                    Index++;
                }
            }

            Writer.Flush();
            Writer.Close();

            if (File.Exists(ArchivePath))
            {
                if(Backup)
                    File.Move(ArchivePath, ArchivePath + ".bak");
            }

            File.Move(RandomFile, ArchivePath);
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

    /// <summary>
    /// A simple ByteArrayComparer that can be passed to a Dictionary.
    /// </summary>
    public class ByteArrayComparer : EqualityComparer<byte[]>
    {
        public override bool Equals(byte[] first, byte[] second)
        {
            if (first == null || second == null)
            {
                // null == null returns true.
                // non-null == null returns false.
                return first == second;
            }
            if (ReferenceEquals(first, second))
            {
                return true;
            }
            if (first.Length != second.Length)
            {
                return false;
            }
            // Linq extension method is based on IEnumerable, must evaluate every item.
            return first.SequenceEqual(second);
        }

        /// <summary>
        /// Gets the hash code for a byte array. Only works with cryptographic hashes!
        /// </summary>
        /// <param name="obj">The byte array, a crypto hash.</param>
        /// <returns>The hash code.</returns>
        public override int GetHashCode(byte[] obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (obj.Length >= 4)
            {
                return BitConverter.ToInt32(obj, 0);
            }
            // Length occupies at most 2 bits. Might as well store them in the high order byte
            int value = obj.Length;
            foreach (var b in obj)
            {
                value <<= 8;
                value += b;
            }
            return value;
        }
    }
}
