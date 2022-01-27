using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace RenamerTest
{
    [TestClass]
    public class UnitTest1
    {
        public string pbixFile = "test_files/Sales.pbix";
        public string zipFile = "test_files/Sales.zip";
        
        [TestMethod]
        public void TestRenamePBIFile()
        {
            if (File.Exists(zipFile))
            {
                File.Delete(zipFile);
            }
            Debug.WriteLine(this.pbixFile);
            var filepath = new PBIRenamer.PBIRenamer();
            var new_file_path = filepath.RenamePBIFolder(this.pbixFile);
            Assert.IsTrue(File.Exists(new_file_path));
        }

        public void Test

    }
}
