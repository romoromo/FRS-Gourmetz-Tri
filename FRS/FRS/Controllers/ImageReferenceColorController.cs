using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.Services.Interfaces;
using DAL;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ImageReferenceColorController : BaseController
    {
        private IImageReferenceColorService _service;
        readonly ILogger _logger;


        public ImageReferenceColorController(IImageReferenceColorService service, ILogger<ImageReferenceColorController> logger)
        {
            _service = service;
            _logger = logger;
        }

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("imagereferencecolors/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllImageReferenceColorsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetImageReferenceColors(BaseFilter filter)
        {
            var results = await this._service.GetImageReferenceColorsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<ImageReferenceColorDTO>>(results));
        }

        #endregion

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllImageReferenceColorsPolicy)]
        [ProducesResponseType(201, Type = typeof(ImageReferenceColorDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateImageReferenceColor([FromBody] ImageReferenceColorDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateAsync(dto);
                if (result.IsSuccess)
                {
                    ImageReferenceColorDTO vm = Mapper.Map<ImageReferenceColorDTO>(result.Data);
                    return CreatedAtAction("GetImageReferenceColorById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllImageReferenceColorsPolicy)]
        [ProducesResponseType(200, Type = typeof(ImageReferenceColorDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteImageReferenceColor(int id)
        {
            var dto = await this._service.GetByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting image reference color: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllImageReferenceColorsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateImageReferenceColor(string id, [FromBody] ImageReferenceColorDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting color id in parameter and model data");


                var dto = await this._service.GetByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}