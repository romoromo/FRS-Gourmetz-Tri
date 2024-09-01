using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Models;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class MediaExtensionController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public MediaExtensionController(IUnitOfWork unitOfWork, ILogger<MediaExtensionController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("list")]
        //[Authorize(Authorization.Policies.ManageAllMediaExtensionsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<MediaExtensionViewModel>))]
        public async Task<IActionResult> GetMediaExtensions(int? institutionId = null)
        {
            return await GetMediaExtensions(-1, -1, institutionId);
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ManageAllMediaExtensionsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<MediaExtensionViewModel>))]
        public async Task<IActionResult> GetMediaExtensions(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.MediaExtensions.GetMediaExtensionsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(_mapper.Map<List<MediaExtensionViewModel>>(result));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllMediaExtensionsPolicy)]
        [ProducesResponseType(201, Type = typeof(MediaExtensionViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMediaExtension([FromBody] MediaExtensionViewModel mediaExtension)
        {
            if (ModelState.IsValid)
            {
                if (mediaExtension == null)
                    return BadRequest($"{nameof(mediaExtension)} cannot be null");


                var type = _mapper.Map<MediaExtension>(mediaExtension);

                var result = await _unitOfWork.MediaExtensions.CreateAsync(type);
                if (result.IsSuccess)
                {
                    MediaExtensionViewModel mediaExtensionVM = _mapper.Map<MediaExtensionViewModel>(result.Data);
                    return CreatedAtAction("GetMediaExtensionById", new { id = mediaExtensionVM.Id }, mediaExtensionVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMediaExtensionsPolicy)]
        [ProducesResponseType(200, Type = typeof(MediaExtensionViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMediaExtension(int id)
        {
            var mediaExtension = await this._unitOfWork.MediaExtensions.GetByIdAsync(id);

            MediaExtensionViewModel mediaExtensionVM = _mapper.Map<MediaExtensionViewModel>(mediaExtension);
            if (mediaExtensionVM == null)
                return NotFound(id);

            var result = await _unitOfWork.MediaExtensions.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(mediaExtensionVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMediaExtensionsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMediaExtension(string id, [FromBody] MediaExtensionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var mediaExtension = await this._unitOfWork.MediaExtensions.GetByIdAsync(model.Id);

                MediaExtensionViewModel mediaExtensionVM = _mapper.Map<MediaExtensionViewModel>(mediaExtension);
                if (mediaExtensionVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<MediaExtension>(model);
                var result = await _unitOfWork.MediaExtensions.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}