using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ScrewLib
{
    class MD5Calc
    {
        public static string CalculateMD5(string inputFile)
        {

            using (var md5Instance = MD5.Create())
            {
                using (var stream = File.OpenRead(inputFile))
                {
                    var hashResult = md5Instance.ComputeHash(stream);
                    return BitConverter.ToString(hashResult).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
