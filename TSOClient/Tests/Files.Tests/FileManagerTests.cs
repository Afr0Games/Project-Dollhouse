using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Files.Tests
{
    [TestClass]
    public class FileManagerTests
    {
        [TestMethod]
        public void TestGetFileList()
        {
            string GameDir = CommonMethods.GetInstallDir();
            string Delimiter = (CommonMethods.IsLinux) ? "//" : "\\";

            IEnumerable<string> FAR3Archives = CommonMethods.GetFileList("*.dat", GameDir);

            //There's at least one element in the collection, so the method found the archives.
            Assert.IsTrue(FAR3Archives.GetEnumerator().MoveNext());
        }
    }
}
