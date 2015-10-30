/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the FileTests.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Text;
using Files;
using Files.FAR1;
using Files.FAR3;
using Files.DBPF;
using Files.AudioFiles;
using Files.AudioLogic;
using Files.Manager;
using Files.IFF;
using Files.Vitaboy;

namespace FileTests
{
    class Program
    {
        private static string StartupPath;

        /// <summary>
        /// Runs tests for the various file classes defined in the File library.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string Software = "";

            if ((is64BitOperatingSystem == false) && (is64BitProcess == false))
                Software = "SOFTWARE";
            else
                Software = "SOFTWARE\\Wow6432Node";

            //Find the path to TSO on the user's system.
            RegistryKey softwareKey = Registry.LocalMachine.OpenSubKey(Software);

            if (Array.Exists(softwareKey.GetSubKeyNames(), delegate (string s) { return s.Equals("Maxis", StringComparison.InvariantCultureIgnoreCase); }))
            {
                RegistryKey maxisKey = softwareKey.OpenSubKey("Maxis");
                if (Array.Exists(maxisKey.GetSubKeyNames(), delegate (string s) { return s.Equals("The Sims Online", StringComparison.InvariantCultureIgnoreCase); }))
                {
                    RegistryKey tsoKey = maxisKey.OpenSubKey("The Sims Online");
                    string installDir = (string)tsoKey.GetValue("InstallDir");
                    installDir += "\\TSOClient\\";
                    StartupPath = installDir;
                }
                else
                {
                    Console.WriteLine("Error TSO was not found on your system.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
            else
            {
                Console.WriteLine("Error: No Maxis products were found on your system.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            FileManager.OnThirtyThreePercentCompleted += FileManager_OnThirtyThreePercentCompleted;
            FileManager.OnSixtysixPercentCompleted += FileManager_OnSixtysixPercentCompleted;
            FileManager.OnHundredPercentCompleted += FileManager_OnHundredPercentCompleted;
            FileManager.Initialize(null, StartupPath);

            Console.ReadLine();
        }

        #region FileManager

        private static void FileManager_OnHundredPercentCompleted()
        {
            Console.WriteLine("Loading: 100% done!");

            FAR3Test();
            FAR1Test();
            DBPFTest();
        }

        private static void FileManager_OnSixtysixPercentCompleted()
        {
            Console.WriteLine("Loading: 66% done...");
        }

        private static void FileManager_OnThirtyThreePercentCompleted()
        {
            Console.WriteLine("Loading: 33% done...");
        }

        #endregion

        /// <summary>
        /// Tests the FAR3Archive class.
        /// </summary>
        private static void FAR3Test()
        {
            Console.WriteLine("Attempting to parse Mesh...");
            FileManager.GetMesh((ulong)FileIDs.MeshFileIDs.uaab023dog_pinkpoodle_head_dogbody_head);

            Console.WriteLine("Attempting to parse Skeleton...");
            FileManager.GetSkeleton(0x100000005);

            Console.WriteLine("Attempting to parse Outfit...");
            Outfit Oft = FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fab002_police);

            Console.WriteLine("Attempting to parse Handgroup...");
            HandGroup Hag = FileManager.GetHandgroup(Oft.HandgroupID.UniqueID);

            Console.WriteLine("Attempting to parse Appearance...");
            Appearance Apr = FileManager.GetAppearance(Hag.Light.Left.Idle.AppearanceID.UniqueID);

            Console.WriteLine("Attempting to parse Animation...");
            Anim Animation = FileManager.GetAnimation(0x100000007);
        }

        /// <summary>
        /// Tests the FAR1Archive class.
        /// </summary>
        private static void FAR1Test()
        {
            Console.WriteLine("Attempting to parse IFF...");
            //Testcases: djbooth.iff, trashcanvacation.iff, stair2.iff, stereos.iff, maid.iff, mask.iff, medicinecabinet.iff
            Iff IffObject = FileManager.GetIFF("medicinecabinet.iff");
            OBJD Master = IffObject.Master;
        }

        /// <summary>
        /// Tests the DBPFArchive class.
        /// </summary>
        public static void DBPFTest()
        {
            Console.WriteLine("Attempting to parse XA...");


            Console.WriteLine("Attempting to parse EVT...");
            EVT Event = new EVT(File.Open(StartupPath + "\\sounddata\\tsov2.evt", FileMode.Open, FileAccess.Read, FileShare.Read));

            Console.WriteLine("Attempting to parse TRK...");
            TRK Track = FileManager.GetTRK(Event.Events[2].TrackID);

            Console.WriteLine("Attempting to parse HIT...");
            Hit Hit = new Hit(File.Open(StartupPath + "\\sounddata\\tsoep5.hit", FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public static IEnumerable<string> GetFileList(string fileSearchPattern, string rootFolderPath)
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

        private static bool is64BitProcess = (IntPtr.Size == 8);
        private static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        /// <summary>
        /// Determines if this process is run on a 64bit OS.
        /// </summary>
        /// <returns>True if it is, false otherwise.</returns>
        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
