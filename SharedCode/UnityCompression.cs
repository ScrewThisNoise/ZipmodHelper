using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

// Utilizing code from https://github.com/IllusionMods/Unity3DCompressor
// For this code to work, SB3U needs to be in /Utils/SB3U

namespace ScrewLib
{
    class UnityCompression
    {
        public static bool CABRandomization { get; set; }
        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static void RunSingle(string file)
        {
            string tempfile = @"Utils\unitycomptemp.txt";
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("LoadPlugin(PluginDirectory+\"UnityPlugin.dll\")");
            sb.AppendLine();

            sb.Append(ProcessFile(file));

            File.WriteAllText(tempfile, sb.ToString());

            var SB3U = new ProcessStartInfo
            {
                Arguments = $"\"{tempfile}\"",
                CreateNoWindow = true,
                UseShellExecute = true,
                FileName = @"Utils\SB3U\SB3UtilityScript.exe",
                WindowStyle = ProcessWindowStyle.Hidden
            };


            using (Process exeProcess = Process.Start(SB3U)) exeProcess.WaitForExit();

            if(File.Exists(tempfile))
                File.Delete(tempfile);
        }

        private static string ProcessFile(string path)
        {
            StringBuilder sb = new StringBuilder();

            var rnbuf = new byte[16];
            rng.GetBytes(rnbuf);
            string CAB = "CAB-" + string.Concat(rnbuf.Select((x) => ((int)x).ToString("X2")).ToArray()).ToLower();

            sb.AppendLine($"unityParser4 = OpenUnity3d(path=\"{path}\")");
            sb.AppendLine("unityEditor4 = Unity3dEditor(parser=unityParser4)");
            sb.AppendLine("unityEditor4.GetAssetNames(filter=True)");
            if (CABRandomization)
                sb.AppendLine($"unityEditor4.RenameCabinet(cabinetIndex=0, name=\"{CAB}\")");
            sb.AppendLine("unityEditor4.SaveUnity3d(keepBackup=False, backupExtension=\".unit-y3d\", background=False, clearMainAsset=True, pathIDsMode=-1, compressionLevel=2, compressionBufferSize=262144)");
            sb.AppendLine();
            return sb.ToString();
        }

        public static bool FileIsAssetBundle(string path)
        {
            if (Path.GetExtension(path) == ".unity3d")
                return true;

            byte[] buffer = new byte[7];
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
            }
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length) == "UnityFS";
        }
    }
}
