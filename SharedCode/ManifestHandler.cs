using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ScrewLib
{
    class ManifestHandler
    {
        public static List<string> CheckIntegrity(string TempFolder, List<string> tagList)
        {
            var guid = String.Empty;
            var name = "Unknown";
            var version = "Unknown";
            var author = "Unknown";
            var description = string.Empty;
            var website = string.Empty;
            var game = "Unknown";
            var originalgame = string.Empty;
            var game2 = String.Empty;
            var originalgame2 = string.Empty;

            var manifestDocument = new XmlDocument();
            manifestDocument.Load($"{TempFolder}\\manifest.xml");
            foreach (XmlNode node in manifestDocument.DocumentElement.ChildNodes)
            foreach (XmlNode locNode in node)
                switch (node.Name)
                {
                    case "guid":
                        guid = locNode.Value;
                        break;
                    case "name":
                        name = locNode.Value;
                        break;
                    case "version":
                        version = locNode.Value;
                        if (version.Remove(1, version.Length - 1).ToLower() != "v")
                            version = $"v{version}";
                        if (version == "vUnknown")
                            version = "(noVer)";
                        break;
                    case "author":
                        author = locNode.Value;
                        break;
                    case "description":
                        description = locNode.Value;
                        break;
                    case "website":
                        website = locNode.Value;
                        break;
                    case "game":
                        if (game == "Unknown")
                        {
                            game = locNode.Value;
                            originalgame = locNode.Value;
                        }
                        else
                        {
                            game2 = locNode.Value;
                            originalgame2 = locNode.Value;
                        }
                        break;
                    default:
                        continue;
                }

            //let's mess with the data
            try
            {
                if (tagList[0] == "RemoveAll")
                {
                    game = String.Empty;
                    game2 = String.Empty;
                }
                else
                {
                    game = tagList[0];
                    game2 = tagList[1];
                }
                
            }
            catch (Exception e)
            {
            }

            // List out the final data

            Logger.Writer($"GUID: {guid}");
            Logger.Writer($"Name: {name}");
            Logger.Writer($"Version: {version}");
            Logger.Writer($"Author: {author}");
            Logger.Writer($"Description: {description}");
            Logger.Writer($"Website: {website}");
            Logger.Writer($"Original Game: [{originalgame}], [{originalgame2}]");
            Logger.Writer($"Game: [{game}], [{game2}]");

            List<string> resultStrings = new List<string>
            {
                guid,
                name,
                version,
                author,
                game,
                game2
            };

            // lets write to the file I guess
            File.Delete($"{TempFolder}\\manifest.xml");
            XmlTextWriter textWriter = new XmlTextWriter($"{TempFolder}\\manifest.xml", Encoding.UTF8) {Formatting = Formatting.Indented};
            textWriter.WriteStartDocument();

            textWriter.WriteComment("Generated with BetterRepack ZipmodHelper");
            textWriter.WriteStartElement("manifest");
            textWriter.WriteAttributeString("schema-ver", null, "1");
            textWriter.WriteElementString("guid", guid);
            textWriter.WriteElementString("name", name);
            textWriter.WriteElementString("version", version);
            textWriter.WriteElementString("author", author);
            textWriter.WriteElementString("description", description);
            textWriter.WriteElementString("website", website);
            textWriter.WriteElementString("game", game);
            textWriter.WriteElementString("game", game2);
            textWriter.WriteEndElement();
            textWriter.WriteEndDocument();
            textWriter.Close();

            return resultStrings;
        }
    }
}
