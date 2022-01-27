using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipmodHelper
{
    public class FileModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string NewName { get; set; }
        public string Path { get; set; }
        public string MD5 { get; set; }
        public string[] Game { get; set; }
    }
}
