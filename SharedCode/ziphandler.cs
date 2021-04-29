using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using Ionic.Zlib;

namespace ScrewLib
{
    class ZipHandler
    {
        public static void Extract(string inputFile, string outputFolder)
        {
            if(Directory.Exists(outputFolder))
                Directory.Delete(outputFolder, true);

            Directory.CreateDirectory(outputFolder);
            Logger.Writer($"Extracting {inputFile}...");
            try
            {
                using (var zip = ZipFile.Read(inputFile))
                {
                    zip.ExtractAll(outputFolder, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch (Exception e)
            {
                Logger.Writer(e.ToString());
                throw;
            }
        }

        public static void Seal(string tempFolder, string outputFolder, string filename)
        {
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            try
            {
                using (var zip = new ZipFile())
                {
                    zip.CompressionLevel = CompressionLevel.Level0;
                    zip.CompressionMethod = CompressionMethod.None;
                    Logger.Writer("Writing ZIPMOD...");
                    zip.AddDirectory(tempFolder);
                    zip.Save($"{outputFolder}\\{filename}");
                }
            }
            catch (Exception e)
            {
                Logger.Writer(e.ToString());
                throw;
            }
        }
    }
}
