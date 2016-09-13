using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BuildBuddy
{
    public class PListEditor
    {
        private const string doctype =
            "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">";

        private readonly XmlDocument document;

        private readonly string filePath;

        public PListEditor(string path)
        {
            filePath = path + "/Info.plist";
            document = new XmlDocument();
            document.Load(filePath);
        }

        public void AddPListEntries(List<PlistEntry> entries)
        {
            foreach (var entry in entries)
            {
                entry.SerializeToPList(document, document.GetElementsByTagName("dict")[0]);
            }
            document.Save(filePath);
            //Bad hack to get around XmlDocument modifying doctype;
            var lines = File.ReadAllLines(filePath);
            lines[1] = doctype;
            File.WriteAllLines(filePath, lines);
        }
    }
}