using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Files;
using Files.FAR1;
using Files.IFF;

namespace Files.Tests
{
    [TestClass]
    public class FileWriterTests
    {
        /// <summary>
        /// Tests IFF writing by attempting to write and subsequently open a IFF.
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

            string TempFile = /*Path.GetTempFileName();*/ "C:\\anniversary.iff";
            byte[] EntryHash = FileUtilities.GenerateHash("anniversary.iff");

            Obj.Init(Arch.GrabEntry(EntryHash), false);
            BinaryWriter Writer = new BinaryWriter(File.Open(TempFile, FileMode.OpenOrCreate));
            MemoryStream ObjStream = (MemoryStream)Obj.ToStream();
            Writer.Write(ObjStream.ToArray());
            Writer.Flush();
            Writer.Close();

            Assert.IsTrue(Obj.Init(File.Open(TempFile, FileMode.Open), true));
        }
    }
}
