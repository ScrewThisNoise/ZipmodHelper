using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrewLib;

namespace ZipmodHelper
{
    internal class DoWork
    {
        public static async Task StartAsync(string inputFolder, string outputFolder, string tempFolder)
        {
            await CreateDirectories(inputFolder, outputFolder, tempFolder);
            List<string> fileList = PrepareList(inputFolder);
            Logger.Writer($"{fileList.Count} files found in {inputFolder}, starting processing.");
            foreach (string file in fileList)
            {

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

        private static void WorkImage(string path, string file)
        {
            throw new NotImplementedException();
        }

        private static void WorkZIP(string path, string file)
        {
            throw new NotImplementedException();
        }
    }
}
