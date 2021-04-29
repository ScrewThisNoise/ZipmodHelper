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
            if (!Directory.Exists($@"{OutputFolder}\{ResultFolder}\{currentWorkingFile.Remove(0, inputFolder.Length + 1)}"))
                Directory.CreateDirectory($@"{OutputFolder}\{ResultFolder}\{currentWorkingFile.Remove(0, inputFolder.Length + 1)}");
            string finalfilename =
                Misc.FileExists(
                    $@"{OutputFolder}\{ResultFolder}\", currentWorkingFile.Remove(0, inputFolder.Length + 1));
            File.Copy(currentWorkingFile, $@"{OutputFolder}\{ResultFolder}\{finalfilename}");
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
