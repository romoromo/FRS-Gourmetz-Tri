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
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ImageReferenceTypeController : BaseController
    {
        private IImageReferenceTypeService _service;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public ImageReferenceTypeController(IImageReferenceTypeService service, ILogger<ImageReferenceTypeController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("imagereferencetypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllImageReferenceTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetImageReferenceTypes(BaseFilter filter)
        {
            var results = await this._service.GetImageReferenceTypesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<ImageReferenceTypeDTO>>(results));
        }

        #endregion

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllImageReferenceTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(ImageReferenceTypeDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateImageReferenceType([FromBody] ImageReferenceTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateAsync(dto);
                if (result.IsSuccess)
                {
                    ImageReferenceTypeDTO vm = _mapper.Map<ImageReferenceTypeDTO>(result.Data);
                    return CreatedAtAction("GetImageReferenceTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllImageReferenceTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(ImageReferenceTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteImageReferenceType(int id)
        {
            var dto = await this._service.GetByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting image reference type: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllImageReferenceTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateImageReferenceType(string id, [FromBody] ImageReferenceTypeDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


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