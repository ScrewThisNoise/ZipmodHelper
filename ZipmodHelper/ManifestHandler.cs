using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace ZipmodHelper
{
    internal class ManifestHandler
    {
        public static async Task Run(string ManifestFile)
        {
            //if (!File.Exists(ManifestFile)) throw new DoWork.ManifestNotFoundException();
            string fullFilePath = Path.GetFullPath(ManifestFile);
            ManifestModel ManifestData = await readManifest(fullFilePath);
            await MiscFunc.CopyFile(Path.GetDirectoryName(fullFilePath), Path.GetDirectoryName(fullFilePath), "manifest.xml", "_orig");
            await writeNewManifest(ManifestFile, ManifestData);
        }

        private static async Task<ManifestModel> readManifest(string ManifestFile)
        {
            ManifestModel model = new ManifestModel();
            var manifestDocument = new XmlDocument();
            manifestDocument.Load(ManifestFile);
            foreach (XmlNode node in manifestDocument.DocumentElement.ChildNodes)
            foreach (XmlNode locNode in node)
            {
                switch (node.Name)
                {
                    case "guid":
                        model.GUID = locNode.Value;
                        break;
                    case "name":
                        model.Name = locNode.Value;
                        break;
                    case "version":
                        model.Version = locNode.Value;
                        break;
                    case "author":
                        model.Author = locNode.Value;
                        break;
                    case "description":
                        model.Description = locNode.Value;
                        break;
                    case "website":
                        model.Website = locNode.Value;
                        break;
                    case "game":
                        if (model.Game1 == null) model.Game1 = locNode.Value;
                        else if (model.Game2 == null) model.Game2 = locNode.Value;
                        else if (model.Game3 == null) model.Game3 = locNode.Value;
                        else if (model.Game4 == null) model.Game4 = locNode.Value;
                        else if (model.Game5 == null) model.Game5 = locNode.Value;
                        break;
                    case "migrationInfo":
                        model.Migration = locNode.Value;
                        break;
                    case "MaterialEditor":
                        model.MaterialEditor = locNode.Value;
                        break;
                    case "KK_MaterialEditor":
                        model.KK_MaterialEditor = locNode.Value;
                        break;
                    case "KKS_MaterialEditor":
                        model.KKS_MaterialEditor = locNode.Value;
                        break;
                    case "EC_MaterialEditor":
                        model.EC_MaterialEditor = locNode.Value;
                        break;
                    case "AI_MaterialEditor":
                        model.AI_MaterialEditor = locNode.Value;
                        break;
                    case "HS2_MaterialEditor":
                        model.HS2_MaterialEditor = locNode.Value;
                        break;
                    case "AI_HeelsData":
                        model.AI_HeelsData = locNode.Value;
                        break;
                    case "HS2_HeelsData":
                        model.HS2_HeelsData = locNode.Value;
                        break;
                    default:
                        break;
                }
            }

            return model;
        }

        private static async Task writeNewManifest(string ManifestFile, ManifestModel model)
        {
            File.Delete(ManifestFile);
            bool GametagKK = (bool)((MainWindow)Application.Current.MainWindow).GametagKK.IsChecked;
            bool GametagKKS = (bool)((MainWindow)Application.Current.MainWindow).GametagKKS.IsChecked;
            bool GametagEC = (bool)((MainWindow)Application.Current.MainWindow).GametagEC.IsChecked;
            bool GametagAI = (bool)((MainWindow)Application.Current.MainWindow).GametagAI.IsChecked;
            bool GametagHS2 = (bool)((MainWindow)Application.Current.MainWindow).GametagHS2.IsChecked;
            bool GametagRemove = (bool)((MainWindow)Application.Current.MainWindow).GametagRemove.IsChecked;
            
            if (GametagKK || GametagKKS || GametagEC || GametagAI || GametagHS2 || GametagRemove)
            {
                model.Game1 = null;
                model.Game2 = null;
                model.Game3 = null;
                model.Game4 = null;
                model.Game5 = null;

                if (!GametagRemove)
                {
                    if (GametagKK) model = await addGame("KK", model);
                    if (GametagKKS) model = await addGame("KKS", model);
                    if (GametagEC) model = await addGame("EC", model);
                    if (GametagAI) model = await addGame("AI", model);
                    if (GametagHS2) model = await addGame("HS2", model);
                }
            }

            if (model.Version == null) model.Version = "vUnknown";
        }

        private static async Task<ManifestModel> addGame(string game, ManifestModel model)
        {
            if (model.Game1 == null) model.Game1 = game;
            if (model.Game2 == null) model.Game2 = game;
            if (model.Game3 == null) model.Game3 = game;
            if (model.Game4 == null) model.Game4 = game;
            if (model.Game5 == null) model.Game5 = game;

            return model;
        }
    }
}
