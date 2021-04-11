using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FtpServer.Controllers
{
    public class UploadedFile
    {
        public byte[] FileData { get; set; }
        public string FileName { get; set; }
        public string FileContentType { get; set; }
    }
    
    [ApiController]
    [Route("[controller]")]
    public class FTPController : ControllerBase
    {
        private static Dictionary<Guid, UploadedFile> _files = new Dictionary<Guid, UploadedFile>();
        
        
        [HttpPost]
        [Route("upload")]
        public ActionResult Upload(IFormFile file)
        {
            var id = Guid.NewGuid();
            
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                
                _files.TryAdd(id, new UploadedFile
                {
                
                    FileData = fileBytes,
                    FileName = file.Name,
                    FileContentType = file.ContentType
                });
            }


            return Ok(id);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            _files.Remove(id);
            return Ok();
        }

        [HttpPatch]
        public IActionResult Update(Guid id, IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                
                var b = new UploadedFile
                {

                    FileData = fileBytes,
                    FileName = file.Name,
                    FileContentType = file.ContentType
                };
                
                _files[id] = b;
            }
            

            return Ok();
        }

        [HttpGet]
        public FileResult GetFile(Guid id)
        {
            return File(_files[id].FileData, _files[id].FileContentType, _files[id].FileName);
        }
    }
}