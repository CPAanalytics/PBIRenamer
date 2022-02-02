using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace PBIRenamer
{
    public class PbiFile
    {
        private const string LayoutFileName = "Layout";
        private const string SecurityFileName = "SecurityBindings";
        private List<string> convertBacktoJtoken = new List<string>();
        public string zipFile { get; private set; }
        public JObject LayoutObject { get; set; }


        public PbiFile(string pbiFilePath)
        {
            this.zipFile = RenamePbiFolder(pbiFilePath);
            this.LayoutObject = GetLayoutAsJson(zipFile);
        }

        public void Save()
        {
            //Serializes JObject and strips byte order marker from Layout File
            var tempFile = "tempfile";
            File.Delete(tempFile);

            var layoutModel = ConvertBacktoJtoken(this.LayoutObject);

            var layoutFile = JsonConvert.SerializeObject(layoutModel);
            File.WriteAllText(tempFile, layoutFile, Encoding.Unicode);
            IEnumerable<byte> layoutAsBytes = File.ReadAllBytes(tempFile);
            File.Delete(tempFile);
            layoutAsBytes = StripUnicodeByteOrder(layoutAsBytes);

            var zipPbix = ZipFile.Open(this.zipFile, ZipArchiveMode.Update);

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
            File.Delete(zipPath);
            File.Copy(pbixFile, zipPath);

            return zipPath;
        }

        public JObject GetLayoutAsJson(string zipPath)
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

            return (JObject)IterateJObjectandParseJSON(JObject.Parse(text));
        }

        public dynamic IterateJObjectandParseJSON(dynamic variable)
        //https://medium.com/codex/iterating-through-a-dynamic-object-in-c-a3c604141569
        //Powerbi stores Layoutfile as Json Object with certain dynamic objects stored as string values.  This function recursively iterates the JObject
        //Locates and replaces these string object.  Location is stored in a convertBacktoJtoken list.
        {

            if (variable.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
            {

                foreach (var property in variable)
                {

                    IterateJObjectandParseJSON(property.Value);
                }
            }
            else if (variable.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
            {

                foreach (var item in variable)
                {
                    IterateJObjectandParseJSON(item);
                }


            }

            else if (variable.GetType() == typeof(Newtonsoft.Json.Linq.JValue))
            {
                JValue Jvariable = (JValue)variable;
                var character = Jvariable.Value.ToString()[0];
                switch (Jvariable.Value.ToString()[0])
                {
                    case '[':
                        this.convertBacktoJtoken.Add(Jvariable.Path);
                        Jvariable.Replace(JArray.Parse(Jvariable.Value.ToString()));
                        break;
                    case '{':
                        this.convertBacktoJtoken.Add(Jvariable.Path);
                        Jvariable.Replace(JObject.Parse(Jvariable.Value.ToString()));
                        break;
                    default:
                        break;

                }


            }

            return variable;
        }

        public JObject ConvertBacktoJtoken(JObject jObject)
        {
            //Uses list generated by IterateJObject and reserializes Objects back to string before final serialization.
            //If this is not done when the file is opened in powerbi will throw cannot convert JObject to Jtoken error.
            foreach (var location in convertBacktoJtoken)
            {
                var token = jObject.SelectToken(location);
                token.Replace(JsonConvert.SerializeObject(token));
            }

            return jObject;
        }
    }
}