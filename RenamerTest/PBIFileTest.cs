using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBIRenamer;

namespace RenamerTest
{
    [TestClass]
    public class PBIFileTest
    {
        

        
        public string pbixFile = "test_files/Sales.pbix";
        public string zipFile = "test_files/Sales.zip";
        private readonly PbiFile _pbiFile;
        private JObject _jObject;

        public PBIFileTest()
        {
            _pbiFile = new PbiFile();
        }



        [TestMethod]
        public void TestRenamePBIFile()
        {
            File.Delete(zipFile);

            Debug.WriteLine(this.pbixFile);
            var new_file_path = _pbiFile.RenamePbiFolder(this.pbixFile);
            Assert.IsTrue(File.Exists(new_file_path));
        }

        [TestMethod]
        public void TestConvertToJson()
        {
            var Json = _pbiFile.GetLayoutAsJson(zipFile);
            Debug.WriteLine(JObject.Parse(JsonConvert.SerializeObject(Json)));
            

        }

        [TestMethod]
        public void TestSave()
        {
            var Json = _pbiFile.GetLayoutAsJson(zipFile);
            _pbiFile.Save(zipFile,Json);
        }
    }
    
}
