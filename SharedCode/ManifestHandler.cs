using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ScrewLib
{
    class ManifestHandler
    {
        public static List<string> CheckIntegrity(string TempFolder)
        {
            var guid = "";
            var name = "Unknown";
            var version = "Unknown";
            var author = "Unknown";
            var game = "Unknown";

            var manifestDocument = new XmlDocument();
            manifestDocument.Load($"{TempFolder}\\manifest.xml");
            foreach (XmlNode node in manifestDocument.DocumentElement.ChildNodes)
            foreach (XmlNode locNode in node)
                switch (node.Name)
                {
                    case "guid":
                        guid = locNode.Value;
                        Logger.Writer($"GUID: {guid}");
                        break;
                    case "name":
                        name = locNode.Value;
                        Logger.Writer($"Name: {name}");
                        break;
                    case "version":
                        version = locNode.Value;
                        if (version.Remove(1, version.Length - 1).ToLower() != "v")
                            version = $"v{version}";
                        if (version == "vUnknown")
                            version = "(noVer)";
                        Logger.Writer($"Version: {version}");
                        break;
                    case "author":
                        author = locNode.Value;
                        Logger.Writer($"Author: {author}");
                        break;
                    case "game":
                        game = locNode.Value;
                        Logger.Writer($"Game: {game}");
                        break;
                    default:
                        continue;
                }
            List<string> resultStrings = new List<string>
            {
                guid,
                name,
                version,
                author,
                game
            };

            return resultStrings;
        }
    }
}
