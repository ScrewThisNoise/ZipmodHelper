using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScrewLib
{
    class ModIntegrity
    {
        public static bool Check(string workingDIR)
        {
            return File.Exists($"{workingDIR}\\manifest.xml");
        }

        public static string ModType(string WorkingDIR)
        {
            foreach (var CheckFile in Directory.EnumerateFiles(WorkingDIR, "*.*", SearchOption.AllDirectories))
                if (CheckFile.Contains(".csv"))
                {
                    if (CheckFile.Contains(@"abdata\list\characustom"))
                        return "Clothing";
                    if (CheckFile.Contains(@"abdata\studio\info"))
                        return "Studio";
                    if (CheckFile.Contains(@"abdata\map\list"))
                        return "GameMap";
                    if (CheckFile.Contains(@"Map_") && CheckFile.Contains(@"abdata\map"))
                        return "StudioMap";
                }

            return "Unknown";
        }
    }
}
