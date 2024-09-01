using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using AutoMapper;
using DAL;
using DAL.Models;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class ImageFileController : BaseController
    {

        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public ImageFileController(IUnitOfWork unitOfWork, ILogger<DepartmentController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("upload"), DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Images", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave)) Directory.CreateDirectory(pathToSave);

                var exts = Task.Run(async () => await _unitOfWork.MediaExtensions.GetMediaExtensionsLoadRelatedAsync(-1, -1, null)).Result;

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    fileName = Guid.NewGuid().ToString() + fileName;

                    var ext = Path.GetExtension(fileName);
                    var extobj = exts.FirstOrDefault(e => ext.EndsWith(e.Extension) && e.IsActive);
                    if (extobj == null)
                    {
                        AddErrors(new string[] { "File extension is not supported, please contact administrator for more information." });
                        return BadRequest(ModelState);
                    }

                    var sizeInKB = file.Length / 1024;

                    if(extobj.min != null && sizeInKB < extobj.min)
                    {
                        AddErrors(new string[] { "File size subceeded minimum limit for this extension, please contact administrator for more information." });
                        return BadRequest(ModelState);
                    }
                    
                    if (extobj.max != null && sizeInKB > extobj.max)
                    {
                        AddErrors(new string[] { "File size exceeded maximum limit for this extension, please contact administrator for more information." });
                        return BadRequest(ModelState);
                    }


                    var fullPath = Path.Combine(pathToSave, fileName);
                    Debug.WriteLine("fullpath:", fullPath);
                    var dbPath = Path.Combine(folderName, fileName);
                    Debug.WriteLine("dbpath:", dbPath);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    return Ok(new { dbPath, fileName });
                }
                else
                {
                    AddErrors(new string[] { "File cannot empty." });
                    return BadRequest(ModelState);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("")]
        [ProducesResponseType(201, Type = typeof(ImageViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateImage([FromBody] ImageViewModel image)
        {
            if (ModelState.IsValid)
            {
                if (image == null)
                    return BadRequest($"{nameof(image)} cannot be null");


                var type = _mapper.Map<ImageFile>(image);

                var appSetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("PATH_BASE_MEDIA_FOLDER_DIRECTORY").ConfigureAwait(false);

                var result = await _unitOfWork.Images.CreateAsync(type, appSetting?.Value);
                if (result.IsSuccess)
                {
                    ImageViewModel imageVM = _mapper.Map<ImageViewModel>(result.Data);
                    return CreatedAtAction("GetImageById", new { id = imageVM.Id }, imageVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(201, Type = typeof(ImageViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateImage(string id, [FromBody] ImageViewModel image)
        {
            if (ModelState.IsValid)
            {
                if (image == null)
                    return BadRequest($"{nameof(image)} cannot be null");

                var imageObj = await this._unitOfWork.Images.GetByIdAsync(image.Id);

                ImageViewModel imageVM = _mapper.Map<ImageViewModel>(imageObj);
                if (imageVM == null)
                    return NotFound(id);

                var type = _mapper.Map<ImageFile>(image);

                var appSetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("PATH_BASE_MEDIA_FOLDER_DIRECTORY").ConfigureAwait(false);

                var result = await _unitOfWork.Images.UpdateAsync(type, appSetting?.Value);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [HttpGet("images/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<ImageViewModel>))]
        public async Task<IActionResult> GetImages(int? institutionId = null, string institutionCode = null, int? userId =  null)
        {
            return await GetImages(-1, -1, institutionId, institutionCode, userId);
        }


        [HttpGet("images/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<DepartmentViewModel>))]
        public async Task<IActionResult> GetImages(int pageNumber, int pageSize, int? institutionId = null, string institutionCode = null, int? userId = null)
        {
            var images = await _unitOfWork.Images.GetImagesLoadRelatedAsync(pageNumber, pageSize, institutionId, institutionCode, userId);
            return Ok(_mapper.Map<List<ImageViewModel>>(images));
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200, Type = typeof(ImageViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await this._unitOfWork.Images.GetByIdAsync(id);

            ImageViewModel imageVM = _mapper.Map<ImageViewModel>(image);
            if (imageVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Images.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting image: " + string.Join(", ", result.Message));


            return Ok(imageVM);
        }
    }
}