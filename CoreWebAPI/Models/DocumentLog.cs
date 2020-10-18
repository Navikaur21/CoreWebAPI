using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPI.Models
{
    public class DocumentLog
    {
        [Key]
        public int DocumentID { get; set; }
        public string FilePath { get; set; }
        public string Name { get; set; }
        public long? FileSize { get; set; }
        public DateTime? UploadedDateTime { get; set; }
        public int? Position { get; set; }
       



    }
}
