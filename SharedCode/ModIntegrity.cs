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
                    if (CheckFile.Contains(@"Map_") && CheckFile.Contains(@"abdata\map"))
                        return "StudioMap";
                    if (CheckFile.Contains(@"abdata\studio\info"))
                        return "Studio";
                    if (CheckFile.Contains(@"abdata\map\list"))
                        return "GameMap";
                }

            return "Unknown";
        }

        public static bool CABOverride(string WorkingDIR)
        {
            foreach (var CheckFile in Directory.EnumerateFiles(WorkingDIR, "*.*", SearchOption.AllDirectories))
                if (CheckFile.Contains(".csv"))
                {
                    if (CheckFile.Contains(@"abdata\map\list\mapinfo")) // HS2 Map
                        return true;
                    if (CheckFile.Contains(@"abdata\studio\info\kPlug")) // Studio map
                        return true;
                    if (CheckFile.Contains(@"data_prefab_")) // Hooh modding tool
                        return true;
                }

            return false;
        }
    }
}
