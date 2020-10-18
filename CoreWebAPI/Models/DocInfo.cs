using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPI.Models
{
      public class DocInfo
        {
            public DocInfo(Stream content, string contentType)
            {
                Content = content;
                ContentType = contentType;
            }

            public Stream Content { get; }
            public string ContentType { get; }
        }
    
}
