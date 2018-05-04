/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files.Test.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Files.FAR1;
using Files.FAR3;
using Files.DBPF;

namespace Files.Tests
{
    [TestClass]
    public class FileReaderTest
    {
        /// <summary>
        /// Gets a value indicating if platform is Linux.
        /// </summary>
        /// <value><c>true</c> if is Linux; otherwise, <c>false</c>.</value>
        private bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        /// <summary>
        /// Tests FAR1 parsing by attempting to open a FAR1 as a FAR3 archive.
        /// Currently hardcoded for Windows because test methods can't take parameters.
        /// </summary>
        [TestMethod]
        public void TestIncorrectFAR1Parsing()
        {
            string GameDir = "C:\\Program Files\\Maxis\\The Sims Online\\TSOClient\\";
            string Delimiter = (IsLinux) ? "//" : "\\";

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
            string GameDir = "C:\\Program Files\\Maxis\\The Sims Online\\TSOClient\\";

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
            string GameDir = "C:\\Program Files\\Maxis\\The Sims Online\\TSOClient\\";
            string Delimiter = (IsLinux) ? "//" : "\\";

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
            string GameDir = "C:\\Program Files\\Maxis\\The Sims Online\\TSOClient\\";
            string Delimiter = (IsLinux) ? "//" : "\\";

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
            string GameDir = "C:\\Program Files\\Maxis\\The Sims Online\\TSOClient\\";
            string Delimiter = (IsLinux) ? "//" : "\\";

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
            string GameDir = "C:\\Program Files\\Maxis\\The Sims Online\\TSOClient\\";
            
            DBPFArchive Arch = new DBPFArchive(GameDir + "EP2.dat");

            Assert.IsTrue(Arch.ReadArchive(false));
        }
    }
}
