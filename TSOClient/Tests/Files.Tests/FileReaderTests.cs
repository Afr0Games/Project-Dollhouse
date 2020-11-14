/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files.Test.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Files.Manager;
using Files.FAR1;
using Files.FAR3;
using Files.DBPF;
using Files.IFF;
using Files.AudioLogic;
using Files.AudioFiles;
using Sound;

namespace Files.Tests
{
    [TestClass]
    public class FileReaderTests
    {
        /// <summary>
        /// Tests FAR1 parsing by attempting to open a FAR1 as a FAR3 archive.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestIncorrectFAR1Parsing()
        {
            string GameDir = CommonMethods.GetInstallDir();
            string Delimiter = (CommonMethods.IsLinux) ? "//" : "\\";

            FAR1Archive Arch = new FAR1Archive(GameDir + "uigraphics" + Delimiter + "credits" + Delimiter +
                "credits.dat");

            Assert.IsFalse(Arch.ReadArchive(false));
        }

        /// <summary>
        /// Tests FAR3 parsing by attempting to open a FAR3 as a DBPF archive.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestIncorrectFAR3Parsing()
        {
            string GameDir = CommonMethods.GetInstallDir();

            FAR3Archive Arch = new FAR3Archive(GameDir + "EP2.dat");

            Assert.IsFalse(Arch.ReadArchive(false));
        }

        /// <summary>
        /// Tests DBPF parsing by attempting to open a DBPF as a FAR3 archive.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestIncorrectDBPFParsing()
        {
            string GameDir = CommonMethods.GetInstallDir();
            string Delimiter = (CommonMethods.IsLinux) ? "//" : "\\";

            DBPFArchive Arch = new DBPFArchive(GameDir + "uigraphics" + Delimiter + "credits" + Delimiter + 
                "credits.dat");

            Assert.IsFalse(Arch.ReadArchive(false));
        }

        /// <summary>
        /// Tests FAR1 parsing by attempting to open a FAR1 archive.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestCorrectFAR1Parsing()
        {
            string GameDir = CommonMethods.GetInstallDir();
            string Delimiter = (CommonMethods.IsLinux) ? "//" : "\\";

            FAR1Archive Arch = new FAR1Archive(GameDir + "objectdata" + Delimiter + "objects" + Delimiter +
                "objiff.far");

            Assert.IsTrue(Arch.ReadArchive(false));
        }

        /// <summary>
        /// Tests FAR3 parsing by attempting to open a FAR3 archive.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestCorrectFAR3Parsing()
        {
            string GameDir = CommonMethods.GetInstallDir();
            string Delimiter = (CommonMethods.IsLinux) ? "//" : "\\";

            FAR3Archive Arch = new FAR3Archive(GameDir + "uigraphics" + Delimiter + "credits" + Delimiter +
                "credits.dat");

            Assert.IsTrue(Arch.ReadArchive(false));
        }

        /// <summary>
        /// Tests DBPF parsing by attempting to open a DBPF archive.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestCorrectDBPFParsing()
        {
            string GameDir = CommonMethods.GetInstallDir();
            
            DBPFArchive Arch = new DBPFArchive(GameDir + "EP2.dat");

            Assert.IsTrue(Arch.ReadArchive(false));
        }

        /// <summary>
        /// Tests IFF parsing by attempting to open a IFF.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestCorrectIFFParsing()
        {
            string GameDir = CommonMethods.GetInstallDir();
            string Delimiter = (CommonMethods.IsLinux) ? "//" : "\\";

            FAR1Archive Arch = new FAR1Archive(GameDir + "objectdata" + Delimiter + "objects" + Delimiter +
                "objiff.far");
            Arch.ReadArchive(false);

            Iff Obj = new Iff();
            Assert.IsTrue(Obj.Init(Arch.GrabEntry(FileUtilities.GenerateHash("anniversary.iff")), false));
        }

        /// <summary>
        /// Tests HIT parsing by attempting to open a HIT.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestCorrectHITParsing()
        {
            string GameDir = CommonMethods.GetInstallDir();
            string Delimiter = (CommonMethods.IsLinux) ? "//" : "\\";

            try
            {
                Hit Snd = new Hit(File.Open(GameDir + "sounddata" + Delimiter + "newmain.hit",
                    FileMode.Open, FileAccess.ReadWrite));
            }
            catch(HitException)
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Tests XA parsing by attempting to open a XA.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestCorrectXAParsing()
        {
            string GameDir = CommonMethods.GetInstallDir();
            string Delimiter = (CommonMethods.IsLinux) ? "//" : "\\";

            try
            {
                XAFile Snd = new XAFile(File.Open(GameDir + "sounddata" + Delimiter + "tvstations" + Delimiter + 
                    "tv_romance" + Delimiter +  "tv_r1.xa", FileMode.Open, FileAccess.ReadWrite));
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.ToString());
                Assert.Fail();
            }
        }

        /// <summary>
        /// Tests MP3 parsing by attempting to open a MP3.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        /*[TestMethod]
        public void TestCorrectMP3Parsing()
        {
            string GameDir = CommonMethods.GetInstallDir();
            string Delimiter = (CommonMethods.IsLinux) ? "//" : "\\";

            if (FileManager.IsLinux)
            {
                try
                {
                    MP3File Snd = new MP3File(File.Open(GameDir + "music" + Delimiter + "modes" + Delimiter +
                        "map" + Delimiter + "tsomap4_v1.mp3", FileMode.Open, FileAccess.ReadWrite));
                    Snd.DecompressedWav();
                }
                catch (Exception E)
                {
                    Debug.WriteLine(E.ToString());
                    Assert.Fail();
                }
            }
            else
            {
                try
                {
                    SoundPlayer Player = new SoundPlayer(GameDir + "music" + Delimiter + "modes" + Delimiter +
                            "map" + Delimiter + "tsobuild3.mp3");
                    Player.PlaySound(false, true);
                }
                catch (Exception E)
                {
                    Debug.WriteLine(E.ToString());
                    Assert.Fail();
                }
            }
        }*/

        /// <summary>
        /// Tests Ini parsing by attempting to open a Ini.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestCorrectIniParsing()
        {
            string GameDir = CommonMethods.GetInstallDir();
            string Delimiter = (CommonMethods.IsLinux) ? "//" : "\\";

            try
            {
                Ini IniFile = new Ini(File.Open(GameDir + "sys" + Delimiter + "radio.ini",
                    FileMode.Open, FileAccess.ReadWrite));
            }
            catch (FileNotFoundException)
            {
                Assert.Fail();
            }
        }
    }
}
