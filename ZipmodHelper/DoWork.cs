using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ScrewLib;

namespace ZipmodHelper
{
    internal class DoWork
    {
        public static async Task StartAsync(string inputFolder, string outputFolder, string tempFolder, CancellationToken cancellationToken)
        {
            try
            {
                await CreateDirectories(inputFolder, outputFolder, tempFolder);
            }
            catch (Exception e)
            {
                Logger.Writer(e.ToString());
                Logger.Writer("Creating directories failed. Cancelling.");
                return;
            }

            List<string> fileList = PrepareList(inputFolder);
            ProgressModel report = new ProgressModel();
            List<FileModel> output = new List<FileModel>();

            Logger.Writer($"{fileList.Count} file(s) found in {inputFolder}, starting processing.");

            foreach (string file in fileList)
            {
                
                string fileExt = Path.GetExtension(file).Remove(0,1);

                switch (fileExt)
                {
                    case "zipmod":
                    case "zip":
                        await WorkZIP(Path.GetDirectoryName(file), Path.GetFileName(file), outputFolder);
                        break;
                    case "png":
                    case "jpg":
                    case "jpeg":
                        await WorkImage(Path.GetDirectoryName(file), Path.GetFileName(file), outputFolder);
                        break;
                    default:
                        break;
                }
            }
        }

        private static async Task CreateDirectories(string inputFolder, string outputFolder, string tempFolder)
        {
            if (!Directory.Exists(inputFolder)) Directory.CreateDirectory(inputFolder);
            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);
        }

        private static List<string> PrepareList(string path)
        {
            List<string> output = new List<string>();
            foreach (var currentWorkingFile in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
            {
                output.Add(currentWorkingFile);
            }

            return output;
        }

        private static async Task WorkImage(string path, string file, string outPath)
        {
            string ImagesOut = $@"{outPath}\Images\LooseFiles";
            string CompleteFile = $@"{path}\{file}";
            if (!Directory.Exists(ImagesOut)) Directory.CreateDirectory(ImagesOut);
            await Task.Run(() => File.Copy(CompleteFile, $@"{ImagesOut}\{file}"));
        }

        private static async Task WorkZIP(string path, string file, string outPath)
        {
            
        }
    }
}
