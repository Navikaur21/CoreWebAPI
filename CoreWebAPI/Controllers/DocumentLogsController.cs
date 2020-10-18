using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreWebAPI.Data;
using CoreWebAPI.Models;
using System.IO;
using CoreWebAPI.Services;
using Microsoft.Extensions.Configuration;

namespace CoreWebAPI.Controllers
{
    [Route("Document")]
    [ApiController]
    public class DocumentLogsController : ControllerBase
    {
        private readonly DocumentContext _context;
        private readonly IDocService _docService;
        public IConfiguration _config { get; }
        private readonly long _fileSizeLimit;
        private string _permittedExtension;

        public DocumentLogsController(DocumentContext context, IDocService docService, IConfiguration configuration)
        {
            _context = context;//datacontext to save the metadata in SQL
            _docService = docService;//service to save the file in the Blob
            _config = configuration;
            _fileSizeLimit = _config.GetValue<long>("FileSizeLimit");
            _permittedExtension = _config.GetValue<string>("PermittedExtension");
        }

        // GET: 
        [HttpGet("GetAll")]
        public IEnumerable<DisplayDocument> GetDocumentList()
        {
            List<DisplayDocument> result = new List<DisplayDocument>();
            foreach (DocumentLog e in _context.DocumentList)
            {
                result.Add(new DisplayDocument(e));
            }
            if (result.Where(i => i.Position == 0).Count() > 0) //even if one of the position is zero
                return result.OrderBy(i => i.Name); //default order will follow
            else
                return result.OrderBy(i => i.Position); //After reordering it will be shown in The position Order 

        }
        [HttpGet("Download/{filename}")]
        public async Task<IActionResult> DownloadDocument([FromRoute] string filename)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(string.IsNullOrEmpty(filename))
            {
                ModelState.AddModelError("Download",$"Please give the filename.");
                return ValidationProblem(ModelState);
            }
            var documentLog = _context.DocumentList.FirstOrDefault(i => i.Name == filename || i.Name.Contains(filename));
            if (documentLog == null)
            {
                ModelState.AddModelError("File",$"This PDF does not exist.");
                return ValidationProblem(ModelState);
            }
            var fileblob = await _docService.GetDocAsync(documentLog.Name);
            return new FileStreamResult((Stream)fileblob.Content, fileblob.ContentType);
        }
        [HttpPost("Upload")]
        public async Task<IActionResult> UploadDocument()
        {
            try
            {
                #region validation
                if (Request.Form.Files.Count() == 0)
                {
                    ModelState.AddModelError("File",$"Please upload the file.");
                    return ValidationProblem(ModelState);
                }
                IFormFile fileToUpload = Request.Form.Files[0];
                if (fileToUpload.Length < 0)
                {
                    ModelState.AddModelError("File",$"File is Empty (Error 1).");
                    return ValidationProblem(ModelState);
                }
                var ext = Path.GetExtension(fileToUpload.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || !_permittedExtension.Contains(ext))
                {
                    // The extension is invalid ... discontinue processing the file
                    ModelState.AddModelError("File",$"Only PDF file Please! .");
                    return ValidationProblem(ModelState);
                }
                if (fileToUpload.Length > _fileSizeLimit)
                {
                    ModelState.AddModelError("File",
                        $"File Size is bigger than 5 MB(Max Limit).");
                    return ValidationProblem(ModelState);
                }
                #endregion
                Stream uploadFileStream = fileToUpload.OpenReadStream();
                string BlobFilePath = await _docService.UploadContentDocAsync(uploadFileStream, fileToUpload.FileName);
                var newdoc = new DocumentLog
                {
                    FilePath = BlobFilePath, //to be saved in the SQL DB
                    UploadedDateTime = DateTime.Now,
                    FileSize = fileToUpload.Length,
                    Name = fileToUpload.FileName,
                    Position = 0
                };
                _context.DocumentList.Add(newdoc);
                 await _context.SaveChangesAsync();
                return Ok("File has been uploaded");

            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("File",$"Please upload the file." + ex.InnerException.Message + "\n Message is  " + ex.Message);
                return ValidationProblem(ModelState);
            }
        }
        // DELETE: api/DocumentLogs/filename
        [HttpDelete("{filename}")]
        public async Task<IActionResult> DeleteDocument([FromRoute] string filename)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var documentLog = _context.DocumentList.Where(i => i.Name == filename).FirstOrDefault();
            if (documentLog == null)
            {
                ModelState.AddModelError("File", $"File does not exist.");
                return ValidationProblem(ModelState);
            }
            _context.DocumentList.Remove(documentLog);
            await _docService.DeleteDocAsync(filename);
            await _context.SaveChangesAsync();

            return Ok(filename + " has been deleted.");
        }
        [HttpPatch("Reorder")]
        public async Task<IActionResult> UpdateDocPositions([FromBody] List<ReorderingStruct> DocList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (ReorderingStruct ee in DocList)
            {
                DocumentLog docfound = _context.DocumentList.Where(i => i.Name == ee.Name).FirstOrDefault();
                if (docfound != null)
                {
                    docfound.Position = ee.Position;
                    _context.Entry(docfound).State = EntityState.Modified;
                }
                else
                {
                    ModelState.AddModelError(ee.Name, ee.Name + " not found ");
                }
            }
            await _context.SaveChangesAsync();
            string errorMessage = "";
            if (ModelState.Count() > 0)
            {
                errorMessage = "All the Docs have been reordered except ";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errorMessage = errorMessage + " " + error.ErrorMessage;
                    }
                }
               return NotFound(errorMessage);
            }
          return Ok("Reordering has been done");
            
        }
    }
}
