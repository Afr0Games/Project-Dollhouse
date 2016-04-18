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
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
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
    public class FileManager
    {
        public const uint CACHE_SIZE = 350000000; //~350mb
        private static uint m_BytesLoaded = 0;

        private static Dictionary<ulong, Asset> m_Assets = new Dictionary<ulong, Asset>();
        private static Dictionary<byte[], Asset> m_FAR1Assets = new Dictionary<byte[], Asset>();

        public static event ThirtyThreePercentCompletedDelegate OnThirtyThreePercentCompleted;
        public static event SixtysixPercentCompletedDelegate OnSixtysixPercentCompleted;
        public static event HundredPercentCompletedDelegate OnHundredPercentCompleted;

        private static List<FAR3Archive> m_FAR3Archives = new List<FAR3Archive>();
        private static List<FAR1Archive> m_FAR1Archives = new List<FAR1Archive>();
        private static List<DBPFArchive> m_DBPFArchives = new List<DBPFArchive>();

        private static IEnumerable<string> m_FAR3Paths;
        private static IEnumerable<string> m_FAR1Paths;
        private static IEnumerable<string> m_IFFPaths;
        private static IEnumerable<string> m_DBPFPaths;

        //Stores hashes of paths to IFFs outside of archives.
        private static Dictionary<byte[], string> m_IFFHashes = new Dictionary<byte[], string>();

        private static Game m_Game;

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

            m_FAR3Paths = GetFileList("*.dat", StartupDir);
            m_FAR1Paths = GetFileList("*.far", StartupDir);
            m_IFFPaths = GetFileList("*.iff", StartupDir);
            m_DBPFPaths = GetFileList("*.dat", StartupDir);

            //Always precompute hashes...
            foreach (string Fle in m_IFFPaths)
                m_IFFHashes.Add(GenerateHash(Path.GetFileName(Fle)), Fle.Replace("\\\\", "\\"));

            Task LoadTask = new Task(new Action(LoadAllArchives));
            LoadTask.Start();
        }

        /// <summary>
        /// Loads all archives into memory.
        /// </summary>
        private static void LoadAllArchives()
        {
            foreach (string Path in m_FAR3Paths)
            {
                //This should be ignored.
                if (!Path.Contains("packingslips.dat"))
                {
                    FAR3Archive Archive = new FAR3Archive(Path);
                    if(Archive.ReadArchive(false))
                        m_FAR3Archives.Add(Archive);
                }
            }

            if(OnThirtyThreePercentCompleted != null)
                OnThirtyThreePercentCompleted();

            foreach (string Path in m_FAR1Paths)
            {
                FAR1Archive Archive = new FAR1Archive(Path);
                Archive.ReadArchive(false);
                m_FAR1Archives.Add(Archive);
            }

            if(OnSixtysixPercentCompleted != null)
                OnSixtysixPercentCompleted();

            foreach (string Path in m_DBPFPaths)
            {
                DBPFArchive Archive = new DBPFArchive(Path);
                if(Archive.ReadArchive(false))
                    m_DBPFArchives.Add(Archive);
            }

            if(OnHundredPercentCompleted != null)
                OnHundredPercentCompleted();
        }

        #region Grabbing

        /// <summary>
        /// Gets an Texture2D instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the texture to get.</param>
        /// <returns>A Texture2D instance.</returns>
        public static Texture2D GetTexture(ulong AssetID)
        {
            Stream Data = GrabItem(AssetID);
            Stream PNGStream = new MemoryStream();

            if (Data == null)
            {
                Debug.WriteLine("Tried to load null data! Stack: \r\n" + System.Environment.StackTrace);
                return null;
            }

            if (IsBMP(Data))
            {
                Bitmap BMap = new Bitmap(Data);
                BMap.MakeTransparent(System.Drawing.Color.FromArgb(255, 0, 255));
                BMap.MakeTransparent(System.Drawing.Color.FromArgb(255, 1, 255));
                BMap.MakeTransparent(System.Drawing.Color.FromArgb(254, 2, 254));
                BMap.Save(PNGStream, System.Drawing.Imaging.ImageFormat.Png);
                PNGStream.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                try
                {
                    try
                    {
                        Bitmap BMap = new Bitmap(Data);
                        BMap.MakeTransparent(System.Drawing.Color.FromArgb(255, 0, 255));
                        BMap.MakeTransparent(System.Drawing.Color.FromArgb(255, 1, 255));
                        BMap.MakeTransparent(System.Drawing.Color.FromArgb(254, 2, 254));
                        BMap.Save(PNGStream, System.Drawing.Imaging.ImageFormat.Png);
                        PNGStream.Seek(0, SeekOrigin.Begin);
                    }
                    catch
                    {
                        return Texture2D.FromStream(m_Game.GraphicsDevice, Data);
                    }
                }
                catch
                {
                    Paloma.TargaImage TGA = new Paloma.TargaImage(Data);
                    TGA.Image.Save(PNGStream, System.Drawing.Imaging.ImageFormat.Png);
                    PNGStream.Seek(0, SeekOrigin.Begin);
                }
            }

            return Texture2D.FromStream(m_Game.GraphicsDevice, PNGStream);
        }

        public static Texture2D GetTexture(Stream Data)
        {
            Stream PNGStream = new MemoryStream();

            if (Data == null)
            {
                Debug.WriteLine("Tried to load null data! Stack: \r\n" + System.Environment.StackTrace);
                return null;
            }

            if (IsBMP(Data))
            {
                Bitmap BMap = new Bitmap(Data);
                BMap.MakeTransparent(System.Drawing.Color.FromArgb(255, 0, 255));
                BMap.MakeTransparent(System.Drawing.Color.FromArgb(255, 1, 255));
                BMap.MakeTransparent(System.Drawing.Color.FromArgb(254, 2, 254));
                BMap.Save(PNGStream, System.Drawing.Imaging.ImageFormat.Png);
                PNGStream.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                try
                {
                    return Texture2D.FromStream(m_Game.GraphicsDevice, Data);
                }
                catch
                {
                    Paloma.TargaImage TGA = new Paloma.TargaImage(Data);
                    TGA.Image.Save(PNGStream, System.Drawing.Imaging.ImageFormat.Png);
                    PNGStream.Seek(0, SeekOrigin.Begin);
                }
            }

            return Texture2D.FromStream(m_Game.GraphicsDevice, PNGStream);
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
            byte[] magic = new byte[] { (byte)'B', (byte)'M' };
            return data.SequenceEqual(magic);
        }

        public static Outfit GetOutfit(ulong AssetID)
        {
            return new Outfit(GrabItem(AssetID));
        }

        /// <summary>
        /// Gets an Skeleton instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the skeleton to get.</param>
        /// <returns>A Skeleton instance.</returns>
        public static Skeleton GetSkeleton(ulong AssetID)
        {
            return new Skeleton(GrabItem(AssetID));
        }

        /// <summary>
        /// Gets an Handgroup instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the Handgroup to get.</param>
        /// <returns>A Handgroup instance.</returns>
        public static HandGroup GetHandgroup(ulong AssetID)
        {
            return new HandGroup(GrabItem(AssetID));
        }

        /// <summary>
        /// Gets an Appearance instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the Appearance to get.</param>
        /// <returns>An Appearance instance.</returns>
        public static Appearance GetAppearance(ulong AssetID)
        {
            return new Appearance(GrabItem(AssetID));
        }

        /// <summary>
        /// Gets an Binding instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the Binding to get.</param>
        /// <returns>An Binding instance.</returns>
        public static Binding GetBinding(ulong AssetID)
        {
            return new Binding(GrabItem(AssetID));
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
        /// Gets a Mesh instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the mesh to get.</param>
        /// <returns>A Mesh instance.</returns>
        public static Mesh GetMesh(ulong AssetID)
        {
            return new Mesh(GrabItem(AssetID));
        }

        /// <summary>
        /// Gets a Anim instance from the FileManager.
        /// </summary>
        /// <param name="AssetID">The FileID/InstanceID of the animation to get.</param>
        /// <returns>A Anim instance.</returns>
        public static Anim GetAnimation(ulong AssetID)
        {
            return new Anim(GrabItem(AssetID));
        }

        /// <summary>
        /// Gets an XA instance from the FileManager.
        /// </summary>
        /// <param name="ID">The FileID/InstanceID of the xa to get.</param>
        /// <returns>A new XAFile instance.</returns>
        public static XAFile GetXA(uint ID)
        {
            return new XAFile(GrabItem(ID));
        }

        /// <summary>
        /// Gets an TRK instance from the FileManager.
        /// </summary>
        /// <param name="ID">The FileID/InstanceID of the track to get.</param>
        /// <returns>A new TRK instance.</returns>
        public static TRK GetTRK(uint ID)
        {
            return new TRK(GrabItem(ID));
        }

        /// <summary>
        /// Gets an HLS instance from the FileManager.
        /// </summary>
        /// <param name="ID">The FileID/InstanceID of the hitlist to get.</param>
        /// <returns>A new HLS instance.</returns>
        public static HLS GetHLS(uint ID)
        {
            return new HLS(GrabItem(ID));
        }

        /// <summary>
        /// Gets an IFF instance from the FileManager.
        /// </summary>
        /// <param name="Filename">The FileID/InstanceID of the IFF to get.</param>
        /// <returns>A new Iff instance.</returns>
        public static Iff GetIFF(string Filename)
        {
            return new Iff(GrabItem(Filename));
        }

        /// <summary>
        /// Returns a Stream instance with data from the specified item.
        /// </summary>
        /// <param name="ID">ID of the item to grab.</param>
        /// <returns>A Stream instance with data from the specified item.</returns>
        private static Stream GrabItem(uint ID)
        {
            lock (m_Assets)
            {
                if (m_Assets.ContainsKey(ID))
                    return m_Assets[ID].AssetData;
            }

            foreach (DBPFArchive Archive in m_DBPFArchives)
            {
                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.tsov2)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.tsov2);

                    Stream Data = Archive.GrabEntry(UniqueID);
                    AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, Data));
                    return Data;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.Samples)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.tsov2);

                    Stream Data = Archive.GrabEntry(UniqueID);
                    AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, Data));
                    return Data;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.EP2)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.tsov2);

                    Stream Data = Archive.GrabEntry(UniqueID);
                    AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, Data));
                    return Data;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.EP5Samps)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.tsov2);

                    Stream Data = Archive.GrabEntry(UniqueID);
                    AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, Data));
                    return Data;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeIDs.TRK, ID, (uint)GroupIDs.Tracks)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.tsov2);

                    Stream Data = Archive.GrabEntry(UniqueID);
                    AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, Data));
                    return Data;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeIDs.HLS, ID, (uint)GroupIDs.HitLists)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.tsov2);

                    Stream Data = Archive.GrabEntry(UniqueID);
                    AddItem(UniqueID.UniqueID, new Asset(UniqueID.UniqueID, Data));
                    return Data;
                }

                if (Archive.ContainsEntry(new UniqueFileID((uint)TypeIDs.HLS, ID, (uint)GroupIDs.HitListsTemp)))
                {
                    UniqueFileID UniqueID = new UniqueFileID((uint)TypeIDs.Sounds, ID, (uint)GroupIDs.tsov2);

                    Stream Data = Archive.GrabEntry(UniqueID);
                    AddItem(ID, new Asset(ID, Data));
                    return Data;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a Stream instance with data from the specified item.
        /// </summary>
        /// <param name="ID">ID of the item to grab.</param>
        /// <returns>A Stream instance with data from the specified item.</returns>
        private static Stream GrabItem(ulong ID)
        {
            lock (m_Assets)
            {
                if (m_Assets.ContainsKey(ID))
                    return m_Assets[ID].AssetData;
            }

            foreach (FAR3Archive Archive in m_FAR3Archives)
            {
                if (Archive.ContainsEntry(ID))
                {
                    Stream Data = Archive.GrabEntry(ID);
                    AddItem(ID, new Asset(ID, Data));
                    return Data;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a Stream instance with data from the specified item.
        /// </summary>
        /// <param name="Filename">Filename of item to grab.</param>
        /// <returns>A Stream instance with data from the specified item.</returns>
        private static Stream GrabItem(string Filename)
        {
            byte[] Hash = GenerateHash(Filename);

            foreach (KeyValuePair<byte[], Asset> KVP in m_FAR1Assets)
            {
                if(KVP.Key.SequenceEqual(Hash))
                    return m_FAR1Assets[Hash].AssetData;
            }

            foreach (FAR1Archive Archive in m_FAR1Archives)
            {
                if (Archive.ContainsEntry(Filename))
                {
                    Stream Data = Archive.GrabEntry(Filename);
                    AddItem(Filename, new Asset(Hash, Data));
                    return Archive.GrabEntry(Filename);
                }
            }

            foreach (KeyValuePair<byte[], string> KVP in m_IFFHashes)
            {
                if (KVP.Key.SequenceEqual(Hash))
                {
                    Stream Data = File.Open(KVP.Value, FileMode.Open, FileAccess.Read, FileShare.Read);
                    AddItem(Path.GetFileName(KVP.Value), new Asset(Hash, Data));
                    return Data;
                }
            }

            return null;
        }

        #endregion

        #region Adding

        private static void AddItem(ulong ID, Asset Item)
        {
            if ((m_BytesLoaded + Item.AssetData.Length) < CACHE_SIZE)
            {
                m_Assets.Add(ID, Item);
                m_BytesLoaded += (uint)Item.AssetData.Length;
            }
            else //Remove oldest and add new entry.
            {
                List<Asset> Assets = m_Assets.Values.ToList();
                Assets.Sort((x, y) => DateTime.Compare(x.LastAccessed, y.LastAccessed));
                m_Assets.Remove(Assets[0].AssetID);
                m_Assets.Add(ID, Item);
            }
        }

        private static void AddItem(string Filename, Asset Item)
        {
            if((m_BytesLoaded + Item.AssetData.Length) < CACHE_SIZE)
            {
                m_FAR1Assets.Add(GenerateHash(Filename), Item);
                m_BytesLoaded += (uint)Item.AssetData.Length;
            }
            else //Remove oldest and add new entry.
            {
                List<Asset> Assets = m_FAR1Assets.Values.ToList();
                Assets.Sort((x, y) => DateTime.Compare(x.LastAccessed, y.LastAccessed));
                m_FAR1Assets.Remove(Assets[0].FilenameHash);
                m_FAR1Assets.Add(GenerateHash(Filename), Item);
            }
        }

        #endregion

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
    }
}
