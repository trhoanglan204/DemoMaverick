using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Models.File
{
    public class FileUploadModel
    {
        public string? FileName { get; set; }
        public byte[]? FileData { get; set; }
    }
}
