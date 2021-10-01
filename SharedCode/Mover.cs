using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScrewLib
{
    class Mover
    {
        private static void MoveMod(string currentWorkingFile, string inputFolder, string OutputFolder, string ResultFolder)
        {
            var currentDir = Path.GetDirectoryName(currentWorkingFile);
            var currentFile = currentWorkingFile.Remove(0, currentDir.Length + 1);

            var test = currentWorkingFile.Remove(currentDir.Length, currentFile.Length + 1);
            test = currentWorkingFile.Remove(0, inputFolder.Length + 1);

            if (inputFolder == test) test = string.Empty;
            else test = currentWorkingFile.Remove(0, inputFolder.Length + 1);

            var testdir =
                $@"{OutputFolder}\{ResultFolder}\";
            if (!Directory.Exists(testdir))
            {
                Directory.CreateDirectory(testdir);
            }
            string finalfilename =
                Misc.FileExists(
                    $@"{OutputFolder}\{ResultFolder}\", currentWorkingFile.Remove(0, inputFolder.Length + 1));
            File.Copy(currentWorkingFile, $@"{testdir}\{finalfilename}");
        }

        public static void InproperMod(string currentWorkingFile, string inputFolder, string OutputFolder)
        {
            Logger.Writer($@"Current file doesn't contain a proper mod... Putting mod in {OutputFolder}\ManualQueue.");
            MoveMod(currentWorkingFile, inputFolder, OutputFolder, "ManualQueue");
        }

        public static void MalformedManifest(string currentWorkingFile, string inputFolder, string OutputFolder)
        {
            Logger.Writer($@"Current file Has a bad manifest file... Putting mod in {OutputFolder}\MalformedManifest.");
            MoveMod(currentWorkingFile, inputFolder, OutputFolder, "MalformedManifest");
        }
    }
}
