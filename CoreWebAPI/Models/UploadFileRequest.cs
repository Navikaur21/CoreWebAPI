using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPI.Models
{
   
    public class UploadFileRequest
    {
        public string FilePath { get; set; }

        public string FileName { get; set; }
    }
}
