using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CoreWebAPI.Extensions;
using CoreWebAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Services
{
    public class DocService :IDocService
    {
       
            private readonly BlobServiceClient _docServiceClient;

            public DocService(BlobServiceClient docServiceClient)
            {
                _docServiceClient = docServiceClient;
            }

            public async Task<DocInfo> GetDocAsync(string name)
            {
                var containerClient = _docServiceClient.GetBlobContainerClient("navdal-container");
                var blobClient = containerClient.GetBlobClient(name);
                var blobDownloadInfo = await blobClient.DownloadAsync();
                return new DocInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
            }

            //public  Task<IEnumerable<string>> ListDocAsync()
            //{
            //    var containerClient = _docServiceClient.GetBlobContainerClient("youtube");
            //    var items = new List<string>();
            //    foreach (var blobItem in containerClient.GetBlobsAsync())
            //    {
            //        items.Add(blobItem.Name);
            //    }

            //    return items;
            //}

            public async Task UploadFileDocAsync(string filePath, string fileName)
            {
                var containerClient = _docServiceClient.GetBlobContainerClient("navdal-container");
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(filePath, new BlobHttpHeaders { ContentType = filePath.GetContentType() });
            }

            public async Task<string> UploadContentDocAsync(Stream content, string fileName)
            {
                var containerClient = _docServiceClient.GetBlobContainerClient("navdal-container");
                var blobClient = containerClient.GetBlobClient(fileName);
          
                  //var bytes = Encoding.UTF8.GetBytes(content);
                  //var memoryStream = new MemoryStream(bytes);
                  await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = fileName.GetContentType() });
                 string result = blobClient.Uri.AbsoluteUri;
                 return result;
        }

            public async Task DeleteDocAsync(string fileName)
            {
                var containerClient = _docServiceClient.GetBlobContainerClient("navdal-container");
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync();
            }
        }
    
}
