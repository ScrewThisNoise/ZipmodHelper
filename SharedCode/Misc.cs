using System.IO;
using System.Text.RegularExpressions;

namespace ScrewLib
{
    public class Misc
    {
        public static string FileExists(string outputFolder, string author, string name, string version)
        {
            string outFile = $"[{author}] {name} {version}.zipmod";
            // string outFile = $"[{author}] {name} {version}.zipmod";
            if (File.Exists($@"{outputFolder}\{outFile}"))
            {
                var n = 0;
                string newName;
                do
                {
                    n++;
                    newName = $"[{author}] {name} {version} ({n}).zipmod";
                } while (File.Exists($@"{outputFolder}\{newName}"));

                outFile = newName;
            }
            else
            {
                outFile = $"[{author}] {name} {version}.zipmod";
            }

            return outFile;
        }

        public static string FileExists(string outputFolder, string outFile)
        {
            string fileext = outFile.Remove(0, outFile.Length - 4);
            var currentDir = Path.GetDirectoryName(outFile);
            string newName = $"{outFile.Remove(outFile.Length - 4, 4).Remove(0, currentDir.Length + 1)}{fileext}";
            if (File.Exists($@"{outputFolder}\{outFile.Remove(0, currentDir.Length + 1)}"))
            {
                var n = 0;
                do
                {
                    n++;
                    newName = $"{outFile.Remove(outFile.Length - 4, 4).Remove(0, currentDir.Length + 1)} ({n}){fileext}";
                } while (File.Exists($@"{outputFolder}\{newName}"));
            }

            return newName;
        }

        public static void RemoveFiles(string TempFolder, string OutputFolder, string[] fileExt)
        {
            var filenames =
                Directory.EnumerateFiles(TempFolder, "*.*", SearchOption.AllDirectories);
            foreach (var filename in filenames)
            {
                if (filename.Contains(".unit-y3d"))
                {
                    File.Delete(filename);
                    Logger.Writer($"Deleted junk file: {filename}");
                }

                foreach (var curFileExt in fileExt)
                {
                    // Let's just have a peek at if this is a Card or Scene and perserve that first...
                    if (!filename.Contains("abdata") && filename.Contains(".png"))
                    {
                        var possibleCard = new FileInfo(filename);
                        UserContentHandler.Identifier(possibleCard, filename, OutputFolder);
                    }
                    if (!filename.Contains("abdata") && filename.Contains(curFileExt))
                        File.Delete(filename);
                }
            }
        }

        public static string IllegalFilenameCheck(string filename)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            filename = r.Replace(filename, "_");
            return filename;
        }

        public static void PerformCompression(string TempFile, bool randomCAB, bool CABOverride)
        {
            
            UnityCompression.CABRandomization = CABOverride ? false : randomCAB;

            if (UnityCompression.FileIsAssetBundle(TempFile))
            {
                var currentDir = Path.GetDirectoryName(TempFile);
                var fileName = TempFile.Remove(0, currentDir.Length + 1);
                Logger.Writer($"Compressing {fileName}...");
                UnityCompression.RunSingle($"{Directory.GetCurrentDirectory()}\\{TempFile}");
            }
        }
    }
}