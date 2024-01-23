using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using DAL.Core;
using FRS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class FileController : BaseController
    {
        private readonly IConfiguration _configuration;

        public FileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(string subdir = null)
        {
            try
            {
                var file = Request.Form.Files[0];
                string useOriginalFilenameVal = Request.Form["useOriginalFilename"];
                bool useOriginalFilename = false;
                bool.TryParse(useOriginalFilenameVal, out useOriginalFilename);
                var folderName = Path.Combine("Resources", "Images");
                if (!string.IsNullOrEmpty(subdir))
                {
                    folderName = Path.Combine(folderName, subdir);
                }

                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);

                if (file.Length > 0)
                {
                    string originalFileName;
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    originalFileName = fileName;

                    if(!useOriginalFilename)
                    {
                        fileName = Guid.NewGuid().ToString() + fileName;
                    }
                    
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return Ok(new { dbPath, fileName, originalFileName });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error - " + ex.StackTrace);
            }
        }

        [AllowAnonymous]
        [HttpGet("emos/zipresources")]
        public async Task<IActionResult> ZipEmosResources(string path = null, string fname = null, DateTime? from = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                string source = _configuration["AppSettings:MOS_ResourcesFolder"];
                string folderToZip = source;
                string destinationFile = _configuration["AppSettings:MOS_ZipResourceFilePath"];
                string defaultOutputFilename = "products.zip";

                if (!string.IsNullOrEmpty(path))
                {
                    source = Path.Combine(_configuration["AppSettings:MOS_BaseResourcesFolder"], path);
                    var folderName = DateTime.Now.ToString("yyyyMMddHHmm");
                    folderToZip = Path.Combine(_configuration["AppSettings:MOS_BaseResourcesFolder"], "Sync", "ToZip", fname, folderName);

                    if (!Directory.Exists(folderToZip))
                    {
                        Directory.CreateDirectory(folderToZip);
                    }

                    Utilities.TraverseCopyDirectory(source, from ?? DateTime.Now.Date, folderToZip);

                    var outputFilename = string.Format("{0}{1}.zip", fname, folderName);
                    destinationFile = Path.Combine(_configuration["AppSettings:MOS_BaseResourcesFolder"], "Sync", "Zipped", outputFilename);
                    defaultOutputFilename = outputFilename;
                }

                if (System.IO.File.Exists(destinationFile))
                {
                    System.IO.File.Delete(destinationFile);
                }

                ZipFile.CreateFromDirectory(folderToZip, destinationFile);
                response.IsSuccess = true;
                response.Data = defaultOutputFilename;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
            }

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("emos/downloadzip")]
        public async Task<IActionResult> DownloadEmosResourcesAsync(string folder = null, string filename = null, bool latest = false)
        {
            string zippedFilePath = _configuration["AppSettings:MOS_ZipResourceFilePath"];

            if (latest)
            {
                // just get the latest zip file
                zippedFilePath = Path.Combine(_configuration["AppSettings:MOS_BaseResourcesFolder"], folder, filename);
                //DirectoryInfo di = new DirectoryInfo(zipDirectory);
                //zippedFilePath = di.GetFiles().OrderBy(p => p.CreationTime)
                //                        .FirstOrDefault(p => p.Name.StartsWith(filename))?.Name;

            }

            if (!System.IO.File.Exists(zippedFilePath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(zippedFilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(zippedFilePath, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(memory, contentType);
        }
    }
}