using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBIRenamer;

namespace PBIRenamer
{
    public class PbiFile
    {
        private static readonly string LayoutFileName = "Layout";
        private static readonly string SecurityFileName = "SecurityBindings";

        public void Save(string zipFilepath, Rootobject layoutModel)
        {
            //Serializes JObject object back to string and strips byte order marker from Layout File
            var tempFile = "tempfile";
            File.Delete(tempFile);

            var layoutFile = JsonConvert.SerializeObject(layoutModel);
            File.WriteAllText(tempFile, layoutFile, Encoding.Unicode);
            IEnumerable<byte> layoutAsBytes = File.ReadAllBytes(tempFile);
            File.Delete(tempFile);
            layoutAsBytes = StripUnicodeByteOrder(layoutAsBytes);

            var zipPbix = ZipFile.Open(zipFilepath, ZipArchiveMode.Update);

            var zipPbixEntries = zipPbix.Entries;

            zipPbixEntries.FirstOrDefault(x => x.Name.Equals(SecurityFileName))?.Delete();

            foreach (var item in zipPbixEntries)

                if (item.Name.Equals(LayoutFileName))
                {
                    var layoutEntry = item.FullName;
                    item.Delete();
                    var newEntry = zipPbix.CreateEntry(layoutEntry);
                    var openEntry = newEntry.Open();
                    foreach (var b in layoutAsBytes.ToArray()) openEntry.WriteByte(b);
                    newEntry.LastWriteTime = DateTimeOffset.Now;
                    Debug.WriteLine("Layout File Replaced");
                    openEntry.Dispose();
                    break;
                }

            zipPbix.Dispose();
        }


        public static IEnumerable<byte> StripUnicodeByteOrder(IEnumerable<byte> bytes)
        {
            //Strips byte order unicode marker
            if (bytes.ElementAt(0) == 0xFF && bytes.ElementAt(1) == 0xFE)
                bytes = bytes.Skip(2);

            return bytes;
        }

        public string RenamePbiFolder(string pbixFile)
        {
            //renames powerbi file to zip rile and returns string path
            var zipPath = Path.ChangeExtension(pbixFile, ".zip");

            File.Copy(pbixFile, zipPath);

            return zipPath;
        }

        public Rootobject GetLayoutAsJson(string zipPath)
        {
            //IMPORTANT: Encoding in powerbi files needs to be set to Unicode
            var zipFile = ZipFile.OpenRead(zipPath);
            var streamReader = new StreamReader(zipFile
                .Entries
                .FirstOrDefault(x => x.Name.Equals(LayoutFileName,
                    StringComparison.InvariantCulture))
                .Open(), Encoding.Unicode);

            
            var text = new string(streamReader.ReadToEnd().ToArray());
            streamReader.Close();
            zipFile.Dispose();

            var Children = JObject.Parse(text).Children();

            return JsonConvert.DeserializeObject<Rootobject>(text);
        }
    }
}