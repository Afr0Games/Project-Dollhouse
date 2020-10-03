/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Collections.Concurrent;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using log4net;
using System.Linq;
using System.Drawing;
using Files.FAR3;
using Files.FAR1;
using Files.DBPF;
using Files.Vitaboy;
using Files.AudioFiles;
using Files.AudioLogic;
using Files.IFF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Files.Manager
{
    public delegate void ThirtyThreePercentCompletedDelegate();
    public delegate void SixtysixPercentCompletedDelegate();
    public delegate void HundredPercentCompletedDelegate();

    /// <summary>
    /// Provides access to all of the game's files.
    /// Also takes care of loading archives.
    /// </summary>
    public class FileManager : IDisposable
    {
        public const uint CACHE_SIZE = 350000000; //~350mb
        private static uint m_BytesLoaded = 0;

        private static readonly ILog m_Logger = 
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static ConcurrentDictionary<ulong, Asset> m_Assets = new ConcurrentDictionary<ulong, Asset>();
        private static ManualResetEvent m_AssetsResetEvent = new ManualResetEvent(false);
        private static ConcurrentDictionary<byte[], Asset> m_FAR1Assets = new ConcurrentDictionary<byte[], Asset>(new ByteArrayComparer());
        private static ManualResetEvent m_FAR1AssetsResetEvent = new ManualResetEvent(false);
        private static ConcurrentDictionary<byte[], Asset> m_MusicAssets = new ConcurrentDictionary<byte[], Asset>(new ByteArrayComparer());

        public static event ThirtyThreePercentCompletedDelegate OnThirtyThreePercentCompleted;
        public static event SixtysixPercentCompletedDelegate OnSixtysixPercentCompleted;
        public static event HundredPercentCompletedDelegate OnHundredPercentCompleted;

        private static ManualResetEvent m_StillLoading = new ManualResetEvent(false);

        private static ConcurrentBag<FAR3Archive> m_FAR3Archives = new ConcurrentBag<FAR3Archive>();
        private static ConcurrentBag<FAR1Archive> m_FAR1Archives = new ConcurrentBag<FAR1Archive>();
        private static ConcurrentBag<DBPFArchive> m_DBPFArchives = new ConcurrentBag<DBPFArchive>();

        private static IEnumerable<string> m_FAR3Paths;
        private static IEnumerable<string> m_FAR1Paths;
        private static IEnumerable<string> m_IFFPaths;
        private static IEnumerable<string> m_DBPFPaths;

        //Stores hashed filenames of IFFs outside of archives.
        private static ConcurrentDictionary<byte[], string> m_IFFHashes = new ConcurrentDictionary<byte[], string>();
        //Stores hashed filenames of music files outside of archives.
        private static ConcurrentDictionary<byte[], string> m_MusicHashes = new ConcurrentDictionary<byte[], string>();

        private static Game m_Game;
        private static string m_BaseDir = "";

        /// <summary>
        /// Has Initialize() been called? If not, loading files WILL NOT BE POSSIBLE!
        /// </summary>
        public static bool IsInitialized = false;

        /// <summary>
        /// The directory the game is currently running from.
        /// </summary>
        public static string BaseDirectory
        {
            get { return m_BaseDir; }
        }

        static FileManager()
        {
        }

        /// <summary>
        /// Initializes the FileManager.
        /// </summary>
        /// <param name="G">A Game instance.</param>
        /// <param name="StartupDir">Path of the directory where game has its resources.</param>
        public static void Initialize(Game G, string StartupDir)
        {
            m_Game = G;
            m_BaseDir = StartupDir;

            m_FAR3Paths = GetFileList("*.dat", StartupDir);
            m_FAR1Paths = GetFileList("*.far", StartupDir);
            m_IFFPaths = GetFileList("*.iff", StartupDir);
            m_DBPFPaths = GetFileList("*.dat", StartupDir);

            //Always precompute hashes...
            foreach (string Fle in m_IFFPaths)
			{
				if(IsLinux)
					m_IFFHashes.TryAdd(GenerateHash(Path.GetFileName(Fle)), Fle.Replace("//", "/"));
				else
                	m_IFFHashes.TryAdd(GenerateHash(Path.GetFileName(Fle)), Fle.Replace("\\\\", "\\"));
			}

            //Always precompute hashes...
            foreach (string Fle in GetFileList("*.mp3", StartupDir))
            {
                if (IsLinux)
                    m_MusicHashes.TryAdd(GenerateHash(Path.GetFileName(Fle)), Fle.Replace("//", "/"));
                else
                    m_MusicHashes.TryAdd(GenerateHash(Path.GetFileName(Fle)), Fle.Replace("\\\\", "\\"));
            }

            IsInitialized = true;

            Task LoadTask = new Task(new Action(LoadAllArchives));
            LoadTask.Start();
        }

		/// <summary>
		/// Gets a value indicating if platform is linux.
		/// </summary>
		/// <value><c>true</c> if is linux; otherwise, <c>false</c>.</value>
		public static bool IsLinux
		{
			get
			{
				int p = (int) Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}

        /// <summary>
        /// Loads all archives into memory.
        /// </summary>
        private static void LoadAllArchives()
        {
            m_StillLoading.Reset();

            foreach (string Path in m_FAR3Paths)
            {
                //This should be ignored.
                if (!Path.Contains("packingslips.dat"))
                {
                    FAR3Archive Archive = new FAR3Archive(Path);
                    if (Archive.ReadArchive(false))
                        m_FAR3Archives.Add(Archive);
                    else
                        Archive.Dispose();
                }
            }

            OnThirtyThreePercentCompleted?.Invoke();

            foreach (string Path in m_FAR1Paths)
            {
                FAR1Archive Archive = new FAR1Archive(Path);
                Archive.ReadArchive(false);
                m_FAR1Archives.Add(Archive);
            }

            OnSixtysixPercentCompleted?.Invoke();

            foreach (string Path in m_DBPFPaths)
            {
                DBPFArchive Archive = new DBPFArchive(Path);
                if (Archive.ReadArchive(false))
                    m_DBPFArchives.Add(Archive);
                else
                    Archive.Dispose();
            }

            m_StillLoading.Set();

            OnHundredPercentCompleted?.Invoke();
        }

        #region Grabbing

        /// <summary>
        /// Gets an Texture2D instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the texture to get.</param>
        /// <param name="IsTGA">Do you know ahead of time that the resource is a TGA?
        /// Set this to true to avoid a lot of try/catch branches.</param>
        /// <returns>A Texture2D instance.</returns>
        public static Texture2D GetTexture(ulong AssetID, bool IsTGA = false)
        {
            if (!IsTGA)
            {
                Stream Data = (Stream)GrabItem(AssetID, FAR3TypeIDs.JPG);

                if (Data == null)
                    Data = GrabItem(AssetID, FAR3TypeIDs.BMP);
                if (Data == null)
                    Data = GrabItem(AssetID, FAR3TypeIDs.PNG);
                if (Data == null)
                    Data = GrabItem(AssetID, FAR3TypeIDs.PackedPNG);
                if (Data == null)
                    Data = GrabItem(AssetID, FAR3TypeIDs.TGA);
                if(Data == null) //Asset most likely existed outside of an archive.
                {
                }

                if (Data == null)
                {
                    Debug.WriteLine("Tried to load null data! Stack: \r\n" + Environment.StackTrace);
                    return null;
                }
            }
            else
                GrabItem(AssetID, FAR3TypeIDs.TGA);

            m_AssetsResetEvent.WaitOne();
            return (Texture2D)m_Assets[AssetID].AssetData;
        }

        /// <summary>
        /// Checks if the supplied data is a BMP.
        /// </summary>
        /// <param name="Data">The data as a Stream.</param>
        /// <returns>True if data was BMP, false otherwise.</returns>
        private static bool IsBMP(Stream Data)
        {
            if (Data == null)
                return false;

            BinaryReader Reader = new BinaryReader(Data, Encoding.UTF8, true);
            byte[] data = Reader.ReadBytes(2);
            Reader.Dispose();
            byte[] magic = new byte[] { (byte)'B', (byte)'M' };
            return data.SequenceEqual(magic);
        }

        /// <summary>
        /// Gets an Outfit instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">ID of the outfit to get.</param>
        /// <returns>An Outfit instance.</returns>
        public static Outfit GetOutfit(ulong AssetID)
        {
            if (m_Assets.ContainsKey(AssetID))
            {
                m_AssetsResetEvent.WaitOne();
                return (Outfit)m_Assets[AssetID].AssetData;
            }

            Stream Data = GrabItem(AssetID, FAR3TypeIDs.OFT);

            m_AssetsResetEvent.WaitOne();
            return (Outfit)m_Assets[AssetID].AssetData;
        }

        /// <summary>
        /// Gets an PurchasableOutfit instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">ID of the outfit to get.</param>
        /// <returns>An PurchasableOutfit instance.</returns>
        public static PurchasableOutfit GetPurchasableOutfit(ulong AssetID)
        {
            if (m_Assets.ContainsKey(AssetID))
            {
                m_AssetsResetEvent.WaitOne();
                return (PurchasableOutfit)m_Assets[AssetID].AssetData;
            }

            Stream Data = GrabItem(AssetID, FAR3TypeIDs.PO);

            m_AssetsResetEvent.WaitOne();
            return (PurchasableOutfit)m_Assets[AssetID].AssetData;
        }

        /// <summary>
        /// Gets an Skeleton instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the skeleton to get.</param>
        /// <returns>A Skeleton instance.</returns>
        public static Skeleton GetSkeleton(ulong AssetID)
        {
            if (m_Assets.ContainsKey(AssetID))
            {
                m_AssetsResetEvent.WaitOne();
                return (Skeleton)m_Assets[AssetID].AssetData;
            }

            Stream Data = GrabItem(AssetID, FAR3TypeIDs.SKEL);

            m_AssetsResetEvent.WaitOne();
            return (Skeleton)m_Assets[AssetID].AssetData;
        }

        /// <summary>
        /// Gets an Handgroup instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the Handgroup to get.</param>
        /// <returns>A Handgroup instance.</returns>
        public static HandGroup GetHandgroup(ulong AssetID)
        {
            if (m_Assets.ContainsKey(AssetID))
            {
                m_AssetsResetEvent.WaitOne();
                return (HandGroup)m_Assets[AssetID].AssetData;
            }

            Stream Data = GrabItem(AssetID, FAR3TypeIDs.HAG);

            m_AssetsResetEvent.WaitOne();
            return (HandGroup)m_Assets[AssetID].AssetData;
        }

        /// <summary>
        /// Gets an Appearance instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the Appearance to get.</param>
        /// <returns>An Appearance instance.</returns>
        public static Appearance GetAppearance(ulong AssetID)
        {
            if (m_Assets.ContainsKey(AssetID))
            {
                m_AssetsResetEvent.WaitOne();
                return (Appearance)m_Assets[AssetID].AssetData;
            }

            Stream Data = GrabItem(AssetID, FAR3TypeIDs.APR);

            m_AssetsResetEvent.WaitOne();
            return (Appearance)m_Assets[AssetID].AssetData;
        }

        /// <summary>
        /// Gets an Binding instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the Binding to get.</param>
        /// <returns>An Binding instance.</returns>
        public static Binding GetBinding(ulong AssetID)
        {
            if (m_Assets.ContainsKey(AssetID))
            {
                m_AssetsResetEvent.WaitOne();
                return (Binding)m_Assets[AssetID].AssetData;
            }

            Stream Data = GrabItem(AssetID, FAR3TypeIDs.BND);

            return new Binding(GrabItem(AssetID, FAR3TypeIDs.BND));
        }

        /// <summary>
        /// Gets an array of Binding instances.
        /// </summary>
        /// <param name="BindingIDs">A List<> of IDs of bindings.</param>
        /// <returns>An array of Binding instances.</returns>
        public static Binding[] GetBindings(List<UniqueFileID> BindingIDs)
        {
            Binding[] Bindings = new Binding[BindingIDs.Count];

            for (int i = 0; i < BindingIDs.Count; i++)
                Bindings[i] = GetBinding(BindingIDs[i].UniqueID);

            return Bindings;
        }

        /// <summary>
        /// Gets an Collection instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the Collection to get.</param>
        /// <returns>An Collection instance.</returns>
        public static Collection GetCollection(ulong AssetID)
        {
            if (m_Assets.ContainsKey(AssetID))
            {
                m_AssetsResetEvent.WaitOne();
                return (Collection)m_Assets[AssetID].AssetData;
            }

            Stream Data = GrabItem(AssetID, FAR3TypeIDs.COL);

            m_AssetsResetEvent.WaitOne();
            return (Collection)m_Assets[AssetID].AssetData;
        }

        /// <summary>
        /// Gets a Mesh instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the mesh to get.</param>
        /// <returns>A Mesh instance.</returns>
        public static Mesh GetMesh(ulong AssetID)
        {
            if (m_Assets.ContainsKey(AssetID))
            {
                m_AssetsResetEvent.WaitOne();
                return (Mesh)m_Assets[AssetID].AssetData;
            }

            Stream Data = GrabItem(AssetID, FAR3TypeIDs.MESH);

            m_AssetsResetEvent.WaitOne();
            return (Mesh)m_Assets[AssetID].AssetData;
        }

        /// <summary>
        /// Gets a Anim instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the animation to get.</param>
        /// <returns>A Anim instance.</returns>
        public static Anim GetAnimation(ulong AssetID)
        {
            if (m_Assets.ContainsKey(AssetID))
            {
                m_AssetsResetEvent.WaitOne();
                return (Anim)m_Assets[AssetID].AssetData;
            }

            Stream Data = GrabItem(AssetID, FAR3TypeIDs.ANIM);

            m_AssetsResetEvent.WaitOne();
            return (Anim)m_Assets[AssetID].AssetData;
        }

        /// <summary>
        /// Gets a sound (XA, WAV or UTK) from the FileManager.
        /// </summary>
        /// <param name="ID">The FileID/InstanceID of the sound to get.</param>
        /// <returns>A new ISoundCodec instance.</returns>
        public static ISoundCodec GetSound(uint ID)
        {
            UniqueFileID UID = new UniqueFileID((uint)TypeIDs.UTK, ID);

            if (m_Assets.ContainsKey(UID.UniqueID))
            {
                m_AssetsResetEvent.WaitOne();
                return (ISoundCodec)m_Assets[UID.UniqueID].AssetData;
            }

            UID = new UniqueFileID((uint)TypeIDs.XA, ID);

            if (m_Assets.ContainsKey(UID.UniqueID))
            {
                m_AssetsResetEvent.WaitOne();
                return (ISoundCodec)m_Assets[UID.UniqueID].AssetData;
            }

            UID = new UniqueFileID((uint)TypeIDs.WAV, ID);

            if (m_Assets.ContainsKey(UID.UniqueID))
            {
                m_AssetsResetEvent.WaitOne();
                return (ISoundCodec)m_Assets[UID.UniqueID].AssetData;
            }

            UID = new UniqueFileID((uint)TypeIDs.SoundFX, ID);

            if (m_Assets.ContainsKey(UID.UniqueID))
            {
                m_AssetsResetEvent.WaitOne();
                return (ISoundCodec)m_Assets[UID.UniqueID].AssetData;
            }

            ISoundCodec Data = (ISoundCodec)GrabItem(ID, TypeIDs.UTK);

            if (Data == null)
                Data = (ISoundCodec)GrabItem(ID, TypeIDs.XA);
            if(Data == null)
                Data = (ISoundCodec)GrabItem(ID, TypeIDs.SoundFX);
            if (Data == null)
                Data = (ISoundCodec)GrabItem(ID, TypeIDs.WAV);

            return Data;
        }

        /// <summary>
        /// Gets music (MP3) from the FileManager.
        /// </summary>
        /// <param name="Name">The name of the music to get (only for sounds existing outside of archives).</param>
        /// <returns>A new ISoundCodec instance.</returns>
        public static ISoundCodec GetMusic(string FileName)
        {
            return (ISoundCodec)GrabItem(FileName);
        }

        /// <summary>
        /// Checks if the supplied data is a UTK.
        /// </summary>
        /// <param name="Data">The data as a Stream.</param>
        /// <returns>True if data was UTK, false otherwise.</returns>
        private static bool IsUTK(Stream Data)
        {
            if (Data == null)
                return false;

            BinaryReader Reader = new BinaryReader(Data, Encoding.UTF8, true);
            byte[] data = Reader.ReadBytes(4);
            Reader.Dispose();
            byte[] magic = new byte[] { (byte)'U', (byte)'T', (byte)'M', (byte)'0' };
            return data.SequenceEqual(magic);
        }

        /// <summary>
        /// Checks if the supplied data is a UTK.
        /// </summary>
        /// <param name="Data">The data as a Stream.</param>
        /// <returns>True if data was XA, false otherwise.</returns>
        private static bool IsXA(Stream Data)
        {
            if (Data == null)
                return false;

            BinaryReader Reader = new BinaryReader(Data, Encoding.UTF8, true);
            byte[] data = Reader.ReadBytes(4);
            Reader.Dispose();
            byte[] magic = new byte[] { (byte)'X', (byte)'A', (byte)'I', (byte)'0' };
            byte[] magic2 = new byte[] { (byte)'X', (byte)'A', (byte)'J', (byte)'0' };

            if (data.SequenceEqual(magic) || data.SequenceEqual(magic2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets an TRK instance from the FileManager.
        /// </summary>
        /// <param name="ID">The FileID/InstanceID of the track to get.</param>
        /// <returns>A new TRK instance.</returns>
        public static TRK GetTRK(uint ID)
        {
            UniqueFileID UID = new UniqueFileID((uint)TypeIDs.TRK, ID);

            if (m_Assets.ContainsKey(UID.UniqueID))
            {
                m_AssetsResetEvent.WaitOne();
                return (TRK)m_Assets[UID.UniqueID].AssetData;
            }

            return (TRK)GrabItem(UID.FileID, TypeIDs.TRK);
        }

        /// <summary>
        /// Attempts to figure out if a track exists in the FileManager.
        /// </summary>
        /// <param name="ID">ID of the track.</param>
        /// <returns>True if found, false otherwise.</returns>
        public static bool TrackExists(uint ID)
        {
            TRK Track = GetTRK(ID);

            if (Track != null)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets an HLS instance from the FileManager.
        /// </summary>
        /// <param name="ID">The FileID/InstanceID of the hitlist to get.</param>
        /// <returns>A new HLS instance.</returns>
        public static HLS GetHLS(uint ID)
        {
            UniqueFileID UID = new UniqueFileID((uint)TypeIDs.HIT, ID);

            if (m_Assets.ContainsKey(UID.UniqueID))
            {
                m_AssetsResetEvent.WaitOne();
                return (HLS)m_Assets[UID.UniqueID].AssetData;
            }

            return (HLS)GrabItem(ID, TypeIDs.HIT);
        }

        /// <summary>
        /// Gets an IFF instance from the FileManager.
        /// </summary>
        /// <param name="Filename">The name of the IFF to get.</param>
        /// <returns>A new Iff instance.</returns>
        public static Iff GetIFF(string Filename)
        {
            Iff Item = new Iff();
            Item.Init((Stream)GrabItem(Filename), true);

            return Item;
        }

        /// <summary>
        /// Returns a Stream instance with data from the specified item.
        /// </summary>
        /// <param name="ID">ID of the item to grab.</param>
        /// <param name="TypeID">TypeID of the the item to grab.</param>
        /// <returns>An object that can be casted to the instance corresponding to the TypeID.</returns>
        private static object GrabItem(uint ID, TypeIDs TypeID)
        {
            Stream Data;

            m_StillLoading.WaitOne();

            foreach (DBPFArchive Archive in m_DBPFArchives)
            {
                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.Custom)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.Custom);
                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                    {
                        if (IsUTK(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new UTKFile2(Data)));
                        else if(IsXA(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new XAFile(Data)));
                        else //TODO: Check for more file formats!
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new WavFile(Data)));
                    }

                    return m_Assets[UniqueID.UniqueID].AssetData;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.CustomTrks)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.CustomTrks);

                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                        AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new TRK(Data)));

                    return Archive.GrabEntry(UniqueID);
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.EP2)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.EP2);
                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                    {
                        if (IsUTK(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new UTKFile2(Data)));
                        else if (IsXA(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new XAFile(Data)));
                        else //TODO: Check for more file formats!
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new WavFile(Data)));
                    }

                    return Archive.GrabEntry(UniqueID);
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.EP5Samps)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.EP5Samps);
                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                    {
                        if (IsUTK(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new UTKFile2(Data)));
                        else if (IsXA(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new XAFile(Data)));
                        else //TODO: Check for more file formats!
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new WavFile(Data)));
                    }

                    return m_Assets[UniqueID.UniqueID].AssetData;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.HitLists)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.HitLists);

                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                        AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new HLS(Data)));

                    return m_Assets[UniqueID.UniqueID].AssetData;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.HitListsTemp)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.HitListsTemp);

                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                        AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new HLS(Data)));

                    return m_Assets[UniqueID.UniqueID].AssetData;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.Multiplayer)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.Multiplayer);
                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                    {
                        if (IsUTK(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new UTKFile2(Data)));
                        else if (IsXA(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new XAFile(Data)));
                        else //TODO: Check for more file formats!
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new WavFile(Data)));
                    }

                    return m_Assets[UniqueID.UniqueID].AssetData;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.Samples)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.Samples);
                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                    {
                        if (IsUTK(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new UTKFile2(Data)));
                        else if (IsXA(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new XAFile(Data)));
                        else //TODO: Check for more file formats!
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new WavFile(Data)));
                    }

                    return m_Assets[UniqueID.UniqueID].AssetData;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.TrackDefs)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.TrackDefs);
                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                        AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new TRK(Data)));

                    return m_Assets[UniqueID.UniqueID].AssetData;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.Tracks)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.Tracks);
                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                        AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new TRK(Data)));

                    return m_Assets[UniqueID.UniqueID].AssetData;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.tsov2)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.tsov2);
                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                    {
                        if (IsUTK(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new UTKFile2(Data)));
                        else if (IsXA(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new XAFile(Data)));
                        else //TODO: Check for more file formats!
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new WavFile(Data)));
                    }

                    return m_Assets[UniqueID.UniqueID].AssetData;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.Stings)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeID, ID, (uint)GroupIDs.Stings);
                    Data = Archive.GrabEntry(UniqueID);

                    if (!m_Assets.ContainsKey(UniqueID.UniqueID))
                    {
                        if (IsUTK(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new UTKFile2(Data)));
                        else if (IsXA(Data))
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new XAFile(Data)));
                        else //TODO: Check for more file formats!
                            AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, (uint)Data.Length, new WavFile(Data)));
                    }

                    return m_Assets[UniqueID.UniqueID].AssetData;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a Stream instance with data from the specified item.
        /// </summary>
        /// <param name="ID">ID of the item to grab.</param>
        /// <returns>A Stream instance with data from the specified item.</returns>
        private static Stream GrabItem(ulong ID, FAR3TypeIDs TypeID)
        {
            MemoryStream MemStream = new MemoryStream();
            object TheLock = new object();
            Bitmap BMap = new Bitmap(1, 1);

            m_StillLoading.WaitOne();

            foreach (FAR3Archive Archive in m_FAR3Archives)
            {
                if (Archive.ContainsEntry(ID))
                {
                    Stream Data = Archive.GrabEntry(ID);

                    if (!m_Assets.ContainsKey(ID))
                    {
                        switch(TypeID)
                        {
                            case FAR3TypeIDs.ANIM:
                                AddItem(ID, new Asset(ID, (uint)Data.Length, new Anim(Data)));
                                break;
                            case FAR3TypeIDs.APR:
                                AddItem(ID, new Asset(ID, (uint)Data.Length, new Appearance(Data)));
                                break;
                            case FAR3TypeIDs.BND:
                                AddItem(ID, new Asset(ID, (uint)Data.Length, new Binding(Data)));
                                break;
                            case FAR3TypeIDs.COL:
                                AddItem(ID, new Asset(ID, (uint)Data.Length, new Collection(Data)));
                                break;
                            case FAR3TypeIDs.HAG:
                                AddItem(ID, new Asset(ID, (uint)Data.Length, new HandGroup(Data)));
                                break;
                            case FAR3TypeIDs.MESH:
                                AddItem(ID, new Asset(ID, (uint)Data.Length, new Mesh(Data)));
                                break;
                            case FAR3TypeIDs.OFT:
                                AddItem(ID, new Asset(ID, (uint)Data.Length, new Outfit(Data)));
                                break;
                            case FAR3TypeIDs.PO:
                                AddItem(ID, new Asset(ID, (uint)Data.Length, new PurchasableOutfit(Data)));
                                break;
                            case FAR3TypeIDs.SKEL:
                                AddItem(ID, new Asset(ID, (uint)Data.Length, new Skeleton(Data)));
                                break;
                            case FAR3TypeIDs.TGA:
                                lock (TheLock)
                                {
                                    using (Paloma.TargaImage TGA = new Paloma.TargaImage(Data))
                                    {
                                        TGA.Image.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
                                        MemStream.Seek(0, SeekOrigin.Begin);
                                        AddItem(ID, new Asset(ID, (uint)MemStream.Length,
                                            Texture2D.FromStream(m_Game.GraphicsDevice, MemStream)));
                                    }
                                }
                                break;
                            case FAR3TypeIDs.PNG:
                            case FAR3TypeIDs.PackedPNG:
                                AddItem(ID, new Asset(ID, (uint)Data.Length, 
                                    Texture2D.FromStream(m_Game.GraphicsDevice, Data)));
                                break;
                            case FAR3TypeIDs.JPG:
                                try
                                {
                                    using (BMap = new Bitmap(Data))
                                    {
                                        BMap.MakeTransparent(System.Drawing.Color.FromArgb(255, 0, 255));
                                        BMap.MakeTransparent(System.Drawing.Color.FromArgb(255, 1, 255));
                                        BMap.MakeTransparent(System.Drawing.Color.FromArgb(254, 2, 254));
                                        BMap.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
                                        MemStream.Seek(0, SeekOrigin.Begin);
                                    }

                                    AddItem(ID, new Asset(ID, (uint)MemStream.Length,
                                        Texture2D.FromStream(m_Game.GraphicsDevice, MemStream)));
                                }
                                catch
                                {
                                    try
                                    {
                                        AddItem(ID, new Asset(ID, (uint)Data.Length,
                                            Texture2D.FromStream(m_Game.GraphicsDevice, Data)));
                                    }
                                    catch(Exception) //Most likely a TGA, sigh.
                                    {
                                        MemStream = new MemoryStream();

                                        using (Paloma.TargaImage TGA = new Paloma.TargaImage(Data))
                                        {
                                            TGA.Image.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
                                            MemStream.Seek(0, SeekOrigin.Begin);
                                            AddItem(ID, new Asset(ID, (uint)MemStream.Length,
                                                Texture2D.FromStream(m_Game.GraphicsDevice, MemStream)));
                                        }
                                    }
                                }
                                break;
                            case FAR3TypeIDs.BMP:
                                if (IsBMP(Data))
                                {
                                    lock (TheLock)
                                    {
                                        try
                                        {
                                            using (BMap = new Bitmap(Data))
                                            {
                                                BMap.MakeTransparent(System.Drawing.Color.FromArgb(255, 0, 255));
                                                BMap.MakeTransparent(System.Drawing.Color.FromArgb(255, 1, 255));
                                                BMap.MakeTransparent(System.Drawing.Color.FromArgb(254, 2, 254));
                                                BMap.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
                                                MemStream.Seek(0, SeekOrigin.Begin);
                                            }

                                            AddItem(ID, new Asset(ID, (uint)MemStream.Length,
                                                Texture2D.FromStream(m_Game.GraphicsDevice, MemStream)));
                                        }
                                        catch (Exception)
                                        {
                                            AddItem(ID, new Asset(ID, (uint)Data.Length,
                                                Texture2D.FromStream(m_Game.GraphicsDevice, Data)));
                                        }
                                    }
                                }
                                else
                                {
                                    AddItem(ID, new Asset(ID, (uint)Data.Length, 
                                        Texture2D.FromStream(m_Game.GraphicsDevice, Data)));
                                }
                                break;
                        }
                    }

                    MemStream.Dispose();

                    return Data;
                }
            }

            //If we reached this point, asset most likely existed outside of an archive...
            if (Enum.IsDefined(typeof(FileIDs.TerrainFileIDs), ID))
            {
                if (Enum.GetName(typeof(FileIDs.TerrainFileIDs), ID).Contains("road"))
                {
                    using (FileStream FS = File.Open(m_BaseDir + (IsLinux ? "gamedata/terrain/" : "gamedata\\terrain\\") +
                        Enum.GetName(typeof(FileIDs.TerrainFileIDs), ID) + ".tga", FileMode.Open, FileAccess.Read,
                        FileShare.ReadWrite))
                    {
                        using (Paloma.TargaImage TGA = new Paloma.TargaImage(FS))
                        {
                            TGA.Image.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
                            MemStream.Seek(0, SeekOrigin.Begin);
                            AddItem(ID, new Asset(ID, (uint)MemStream.Length,
                                Texture2D.FromStream(m_Game.GraphicsDevice, MemStream)));
                        }
                    }
                }
                else
                {
                    using (FileStream FS = File.Open(m_BaseDir + (IsLinux ? "gamedata/terrain/newformat/" : 
                        "gamedata\\terrain\\newformat\\") + Enum.GetName(typeof(FileIDs.TerrainFileIDs), ID) + ".tga", 
                        FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (Paloma.TargaImage TGA = new Paloma.TargaImage(FS))
                        {
                            TGA.Image.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
                            MemStream.Seek(0, SeekOrigin.Begin);
                            try
                            {
                                AddItem(ID, new Asset(ID, (uint)MemStream.Length,
                                    Texture2D.FromStream(m_Game.GraphicsDevice, MemStream)));
                            }
                            catch(Exception E)
                            {
                                m_Logger.Error("FileManager.GrabItem():\n" +
                                    "Failed to load TGA from 'gamedata\\terrain\\newformat\\':\n" + 
                                    E.ToString());
                            }
                        }
                    }
                }

                return MemStream;
            }
            else if (Enum.IsDefined(typeof(FileIDs.CitiesFileIDs), ID))
            {
                string[] Split = Enum.GetName(typeof(FileIDs.CitiesFileIDs), ID).Split("_".ToCharArray());
                using (FileStream FS = File.Open(m_BaseDir + (IsLinux ? "cities/" : "cities\\") +
                    Split[0] + "_" + Split[1] + (IsLinux ? "/" : "\\") + Split[2] + ".bmp",
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BMap = new Bitmap(FS))
                    {
                        BMap.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
                        MemStream.Seek(0, SeekOrigin.Begin);
                    }

                    try
                    {
                        AddItem(ID, new Asset(ID, (uint)MemStream.Length,
                            Texture2D.FromStream(m_Game.GraphicsDevice, MemStream)));
                    }
                    catch (Exception E)
                    {
                        m_Logger.Error("FileManager.GrabItem():\n" +
                            "Failed to load BMP from 'cities\\':\n" +
                            E.ToString());
                    }

                    return MemStream;
                }
            }
            else if(Enum.IsDefined(typeof(FileIDs.UIFileIDs), ID))
            {
                using (FileStream FS = File.Open(m_BaseDir +
                    (IsLinux ? "uigraphics/holiday/" : "uigraphics\\holiday\\") +
                    Enum.GetName(typeof(FileIDs.UIFileIDs), ID) + 
                    GetExtension(Enum.GetName(typeof(FileIDs.UIFileIDs), ID)), 
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BMap = new Bitmap(FS))
                    {
                        BMap.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
                        MemStream.Seek(0, SeekOrigin.Begin);
                    }

                    AddItem(ID, new Asset(ID, (uint)MemStream.Length,
                        Texture2D.FromStream(m_Game.GraphicsDevice, MemStream)));
                    return MemStream;
                }
            }
            else if (Enum.IsDefined(typeof(FileIDs.HintsFileIDs), ID))
            {
                using (FileStream FS = File.Open(m_BaseDir +
                    (IsLinux ? "uigraphics/hints/" : "uigraphics\\hints\\") + 
                    Enum.GetName(typeof(FileIDs.HintsFileIDs), ID) + ".bmp", 
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BMap = new Bitmap(FS))
                    {
                        BMap.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
                        MemStream.Seek(0, SeekOrigin.Begin);
                    }

                    AddItem(ID, new Asset(ID, (uint)MemStream.Length,
                        Texture2D.FromStream(m_Game.GraphicsDevice, MemStream)));
                    return MemStream;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a Stream instance with data from the specified item.
        /// </summary>
        /// <param name="Filename">Filename of item to grab.</param>
        /// <returns>An object instance with data from the specified item.</returns>
        private static object GrabItem(string Filename)
        {
            byte[] Hash;

            if (!IsLinux)
            {
                if (!Filename.Contains("\\")) //Make sure the hash is only of the filename.
                    Hash = GenerateHash(Filename);
                else
                    Hash = GenerateHash(Path.GetFileName(Filename));
            }
            else
            {
                if (!Filename.Contains("/")) //Make sure the hash is only of the filename.
                    Hash = GenerateHash(Filename);
                else
                    Hash = GenerateHash(Path.GetFileName(Filename));
            }

            m_StillLoading.WaitOne();

            /*foreach (KeyValuePair<byte[], Asset> KVP in m_FAR1Assets)
            {
                if(KVP.Key.SequenceEqual(Hash))
                    return m_FAR1Assets[Hash].AssetData;
            }*/

            //TODO: Verify that hashed filenames work for FAR1 archives.
            foreach (FAR1Archive Archive in m_FAR1Archives)
            {
                if (Archive.ContainsEntry(Hash))
                {
                    Stream Data = Archive.GrabEntry(Hash);
                    AddItem(Filename, new Asset(Hash, (uint)Data.Length, Data));
                    return Archive.GrabEntry(Hash);
                }
            }

            foreach (KeyValuePair<byte[], string> KVP in m_IFFHashes)
            {
                if (KVP.Key.SequenceEqual(Hash))
                {
                    Stream Data = File.Open(KVP.Value, FileMode.Open, FileAccess.Read, FileShare.Read);
                    AddItem(Path.GetFileName(KVP.Value), new Asset(Hash, (uint)Data.Length, Data));
                    return Data;
                }
            }

            //If we reached this point, we're looking for musical assets outside of archives...
            foreach (KeyValuePair<byte[], string> KVP in m_MusicHashes)
            {
                if (KVP.Key.SequenceEqual(Hash))
                {
                    Stream Data = File.Open(KVP.Value, FileMode.Open, FileAccess.Read, FileShare.Read);
                    MP3File MP3Fle;
                    XAFile XA;

                    if (Filename.Contains(".mp3"))
                    {
                        MP3Fle = new MP3File(Data);
                        AddMusic(Filename, new Asset(Hash, (uint)Data.Length, MP3Fle));
                    }
                    else if(Filename.Contains("'.xa"))
                    {
                        XA = new XAFile(Data);
                        AddMusic(Filename, new Asset(Hash, (uint)Data.Length, XA));
                    }

                    return m_MusicAssets[Hash].AssetData;
                }
            }

            return null;
        }

        #endregion

        #region Adding

        private static void AddItem(ulong ID, Asset Item)
        {
            m_AssetsResetEvent.Reset();

            if ((m_BytesLoaded + Item.Size) <= CACHE_SIZE)
            {
                m_Assets.AddOrUpdate(ID, Item, (Key, ExistingValue) => ExistingValue = Item);
                m_BytesLoaded += (uint)Item.Size;
            }
            else //Remove oldest and add new entry.
            {
                List<Asset> Assets = m_Assets.Values.ToList();
                Assets.Sort((x, y) => DateTime.Compare(x.LastAccessed, y.LastAccessed));
                Asset Oldest = Assets[0];
                m_Assets.TryRemove(Oldest.AssetID, out Oldest);
                m_Assets.AddOrUpdate(Assets[0].AssetID, Item, (Key, ExistingValue) => ExistingValue = Item);
            }

            m_AssetsResetEvent.Set();
        }

        /// <summary>
        /// Adds an asset to the FileManager's cache.
        /// This method should not be used directly.
        /// </summary>
        /// <param name="Filename">The filename of the asset to add.</param>
        /// <param name="Item">The asset to add.</param>
        private static void AddItem(string Filename, Asset Item)
        {
            m_FAR1AssetsResetEvent.Reset();

            if((m_BytesLoaded + Item.Size) <= CACHE_SIZE)
            {
                m_FAR1Assets.AddOrUpdate(GenerateHash(Filename), Item, (Key, ExistingValue) => ExistingValue = Item);
                m_BytesLoaded += (uint)Item.Size;
            }
            else //Remove oldest and add new entry.
            {
                List<Asset> Assets = m_FAR1Assets.Values.ToList();
                Assets.Sort((x, y) => DateTime.Compare(x.LastAccessed, y.LastAccessed));
                Asset Oldest = Assets[0];
                m_FAR1Assets.TryRemove(Oldest.FilenameHash, out Oldest);
                m_FAR1Assets.AddOrUpdate(GenerateHash(Filename), Item, (Key, ExistingValue) => ExistingValue = Item);
            }

            m_FAR1AssetsResetEvent.Set();
        }

        /// <summary>
        /// Adds a musical asset to the FileManager's cache.
        /// This method should not be used directly.
        /// </summary>
        /// <param name="Filename">The filename of the musical asset to add.</param>
        /// <param name="Item">The asset to add.</param>
        private static void AddMusic(string Filename, Asset Item)
        {
            if ((m_BytesLoaded + Item.Size) <= CACHE_SIZE)
            {
                m_MusicAssets.AddOrUpdate(GenerateHash(Filename), Item, (Key, ExistingValue) => ExistingValue = Item);
                m_BytesLoaded += (uint)Item.Size;
            }
            else //Remove oldest and add new entry.
            {
                List<Asset> Assets = m_MusicAssets.Values.ToList();
                Assets.Sort((x, y) => DateTime.Compare(x.LastAccessed, y.LastAccessed));
                Asset Oldest = Assets[0];
                m_MusicAssets.TryRemove(Oldest.FilenameHash, out Oldest);
                m_MusicAssets.AddOrUpdate(GenerateHash(Filename), Item, (Key, ExistingValue) => ExistingValue = Item);
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Sigh. Gets the extension based on a filename in uigraphics\\holiday, because Maxis
        /// couldn't decide on the type of file to use.
        /// </summary>
        /// <param name="Filename">Filename without extension.</param>
        /// <returns>A string indicating the filename.</returns>
        private static string GetExtension(string Filename)
        {
            if (Filename.Contains("paddys_day"))
                return ".png";
            if (Filename.Contains("valentine"))
                return ".png";
            if (Filename.Contains("halloween"))
                return ".bmp";
            if (Filename.Contains("thanksgiving"))
                return ".bmp";
            if (Filename.Contains("xmas"))
                return ".png";
            if (Filename.Contains("thanksgiving"))
                return ".bmp";

            return "";
        }

        /// <summary>
        /// Generates a list of strings containing all the files having the supplied extension in the specified path.
        /// </summary>
        /// <param name="fileSearchPattern">The extension to search for.</param>
        /// <param name="rootFolderPath">The path to search.</param>
        /// <returns>An IEnumerable instance containing a list of strings with the qualified path to the file
        /// corresponding to the <paramref name="fileSearchPattern"/> in the <paramref name="rootFolderPath"/></returns>
        private static IEnumerable<string> GetFileList(string fileSearchPattern, string rootFolderPath)
        {
            Queue<string> pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                rootFolderPath = pending.Dequeue();
                tmp = Directory.GetFiles(rootFolderPath, fileSearchPattern);
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                tmp = Directory.GetDirectories(rootFolderPath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    pending.Enqueue(tmp[i]);
                }
            }
        }

        private static byte[] GenerateHash(string Filename)
        {
            byte[] tmpSource;
            byte[] Hash;
            //Create a byte array from source data.
            tmpSource = Encoding.UTF8.GetBytes(Filename);
            Hash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

            return Hash;
        }

        #endregion

        ~FileManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this FileManager instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this FAR3Archive instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                foreach(FAR3Archive Archive in m_FAR3Archives)
                {
                    if (Archive != null)
                        Archive.Dispose();
                }

                foreach (FAR1Archive Archive in m_FAR1Archives)
                {
                    if (Archive != null)
                        Archive.Dispose();
                }

                foreach (DBPFArchive Archive in m_DBPFArchives)
                {
                    if (Archive != null)
                        Archive.Dispose();
                }

                // Prevent the finalizer from calling ~FileManager, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("FileManager not explicitly disposed!");
        }
    }
}
