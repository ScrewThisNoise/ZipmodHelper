using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipmodHelper
{
    public class ProgressModel
    {
        public int PercentageComplete { get; set; } = 0;
        public List<FileModel> FilesProcessed { get; set; } = new List<FileModel>();
    }
}
