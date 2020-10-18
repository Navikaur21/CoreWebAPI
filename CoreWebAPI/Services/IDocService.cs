using CoreWebAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPI.Services
{
    public interface IDocService
    {
         Task<DocInfo> GetDocAsync(string name);

        // Task<IEnumerable<string>> ListDocAsync();

         Task UploadFileDocAsync(string filePath, string fileName);

         Task<string> UploadContentDocAsync(Stream content, string fileName);

         Task DeleteDocAsync(string blobName);
    }
}
