using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO.Packaging;
using System.IO;
using Newtonsoft.Json;

//https://community.powerbi.com/t5/Desktop/Modifying-the-Layout-file-that-is-embedded-in-a-pbix-file/td-p/415562

namespace PBIRenamer
{
    public class PBIRenamer
    {
        static void Main(string[] args)
        {

        }

        public void Save(string Filepath, string _layoutModel)
        {
            //Serializes string object back to JSON and strips byte order marker from Layout File
            var layoutFile = JsonConvert.SerializeObject(_layoutModel);
            File.WriteAllText(Filepath, layoutFile, Encoding.Unicode);

            IEnumerable<byte> layoutAsBytes = File.ReadAllBytes(Filepath);
            layoutAsBytes = StripUnicodeByteOrder(layoutAsBytes);
            File.WriteAllBytes(Filepath, layoutAsBytes.ToArray());
        }

        public static IEnumerable<Byte> StripUnicodeByteOrder(IEnumerable<byte> bytes)
        {
            //Strips byte order unicode marker
            if (bytes.ElementAt(0) == 0xFF && bytes.ElementAt(1) == 0xFE)
                bytes = bytes.Skip(2);

            return bytes;
        }

        public string RenamePBIFolder(string pbixFile)
        {
            //renames powerbi file to zip rile and returns string path
            var zipPath = Path.ChangeExtension(pbixFile, ".zip");

            File.Copy(pbixFile, zipPath);

            return zipPath;

        }

        public string GetLayoutAsJson(string zipPath)
        {

            var layout = "Layout";


            string text = new string(
                (new System.IO.StreamReader(
                        ZipFile.OpenRead(zipPath)
                            .Entries
                            .FirstOrDefault(x => x.Name.Equals(layout,
                                StringComparison.InvariantCulture))
                            .Open(), Encoding.UTF8)
                    .ReadToEnd())
                .ToArray());

            return text;
        }

    }


}
