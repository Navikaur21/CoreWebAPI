using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPI.Controllers
{
    public class DisplayDocument
    {
        public DisplayDocument(Models.DocumentLog data)
        {
            Name = data.Name;
            Location = data.FilePath;
            FileSize = data.FileSize;
            Position = data.Position;
        }
        public string Name { get; set; }
        public string Location { get; set; }
        public long? FileSize { get; set; }
        public int? Position { get; set; }


    }
}
