using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrewLib;

namespace ZipmodHelper
{
    internal class MiscFunc
    {
        public static async Task<string> CheckDupeFileAsync(string path, string file)
        {
            string fileExt = Path.GetExtension(file);
            string newName = file;
            if (File.Exists($@"{path}\{newName}"))
            {
                var n = 0;
                do
                {
                    n++;
                    newName = $"{file.Remove(file.Length - fileExt.Length, fileExt.Length)} ({n}){fileExt}";
                } while (File.Exists($@"{path}\{newName}"));
            }
            return newName;
        }

        public static async Task CopyFile(string InFolder, string OutFolder, string FileName)
        {
            string CompleteFile = $@"{InFolder}\{FileName}";
            string OutFile = await MiscFunc.CheckDupeFileAsync(OutFolder, FileName);
            await Task.Run(() => File.Copy(CompleteFile, $@"{OutFolder}\{OutFile}"));
            Logger.Writer($@"File [{InFolder}\{FileName}] copied to [{OutFolder}\{OutFile}]");
        }

        public static async Task CopyFile(string InFolder, string OutFolder, string FileName, string Add)
        {
            string origFile = FileName;
            string fileExt = Path.GetExtension(FileName);
            FileName = $"{FileName.Remove(FileName.Length - fileExt.Length, fileExt.Length)}{Add}{fileExt}";
            string CompleteFile = $@"{InFolder}\{origFile}";
            string OutFile = await MiscFunc.CheckDupeFileAsync(OutFolder, FileName);
            await Task.Run(() => File.Copy(CompleteFile, $@"{OutFolder}\{OutFile}"));
            Logger.Writer($@"File [{InFolder}\{origFile}] copied to [{OutFolder}\{OutFile}]");
        }

        public static async Task CreateDirectories(string Folder)
        {
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
        }
    }
}
