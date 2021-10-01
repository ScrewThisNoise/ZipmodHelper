using System;
using System.IO;
using System.Windows.Markup;

namespace ScrewLib
{
    public class UserContentHandler
    {
        // Heavily relying on code borrowed from KKManager (https://github.com/IllusionMods/KKManager/)
        private static long SearchForSequence(Stream stream, byte[] sequence)
        {
            const int bufferSize = 4096;
            var origPos = stream.Position;

            var buffer = new byte[bufferSize];
            int read;

            var scanByte = sequence[0];

            while ((read = stream.Read(buffer, 0, bufferSize)) > 0)
            {
                for (var i = 0; i < read; i++)
                {
                    if (buffer[i] != scanByte)
                        continue;

                    var flag = true;

                    for (var x = 1; x < sequence.Length; x++)
                    {
                        i++;

                        if (i >= bufferSize)
                        {
                            if ((read = stream.Read(buffer, 0, bufferSize)) < bufferSize)
                                return -1;

                            i = 0;
                        }

                        if (buffer[i] != sequence[x])
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        var result = (stream.Position + 1) - (bufferSize - i) - sequence.Length;
                        stream.Position = origPos;
                        return result;
                    }
                }
            }

            return -1;
        }

        private static readonly byte[] _pngEndChunk = { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };
        private static long SearchForPngEnd(Stream stream)
        {
            var result = SearchForSequence(stream, _pngEndChunk);
            if (result >= 0) result += _pngEndChunk.Length;
            return result;
        }

        private static string GetContentType(string marker, string currentFile)
        {
            bool firstrun = true;
            restart:
            switch (marker)
            {
                case string a when a.Contains("【KStudio】"):
                    return "KKStudio";
                case string a when a.Contains("StudioNEOV2"):
                    return "AISStudio";
                case string a when a.Contains("【KoiKatuChara】"):
                    return "KKChara";
                case string a when a.Contains("【KoiKatuCharaS】"):
                    return "KKPChara";
                case string a when a.Contains("【KoiKatuCharaSP】"):
                    return "KKPEroChara";
                case string a when a.Contains("【EroMakeChara】"):
                    return "ECChara";
                case string a when a.Contains("【AIS_Chara】"):
                    return "AISChara";
                default:
                    if (firstrun)
                    {
                        firstrun = false;
                        marker = File.ReadAllText(currentFile);
                        goto restart;
                    }
                    return "Unknown";
            }
        }

        private static void MoveIt(string currentFile, string outDir, string Game, string type)
        {
            var userdatafolder = Game == "Unknown" ? "UnknownImage" : $"UserData{Game}";

            if (!Directory.Exists($@"{outDir}\{userdatafolder}\{type}\"))
                Directory.CreateDirectory($@"{outDir}\{userdatafolder}\{type}\");
            outDir = $@"{outDir}\{userdatafolder}\{type}\";
            string FileName = Misc.FileExists(outDir, currentFile);
            File.Copy(currentFile, $@"{outDir}\{FileName}");
        }

        public static bool Identifier(FileInfo filename, string currentFile, string outDir)
        {

            using (var stream = filename.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
                var pngEnd = SearchForPngEnd(stream);
                if (pngEnd == -1 || pngEnd >= stream.Length)
                    return false;

                stream.Position = pngEnd;

                try
                {
                    var marker = reader.ReadString();
                    var contentType = GetContentType(marker, currentFile);
                    string game = "";
                    string type = "";

                    switch (contentType)
                    {
                        case "KKChara":
                            game = "KK";
                            type = "card";
                            break;
                        case "KKPChara":
                            game = "KK";
                            type = "card";
                            break;
                        case "KKPEroChara":
                            game = "KK";
                            type = "card";
                            break;
                        case "ECChara":
                            game = "EC";
                            type = "card";
                            break;
                        case "AISChara":
                            game = "AIS";
                            type = "card";
                            break;
                        case "KKStudio":
                            game = "KK";
                            type = "studio";
                            break;
                        case "AISStudio":
                            game = "AIS";
                            type = "studio";
                            break;
                        default:
                            break;
                    }
                    MoveIt(currentFile, outDir, game, type);
                }
                catch (Exception e)
                {
                    Logger.Writer($"Unknown image {filename}!");
                    MoveIt(currentFile, outDir, "Unknown", "");
                    throw;
                }

                return true;
            }
        }
    }
}