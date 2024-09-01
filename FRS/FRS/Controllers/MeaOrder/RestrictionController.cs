using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using BAL.Services.MealOrder;
using DAL;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class RestrictionController : BaseController
    {
        private IRestrictionService _service;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public RestrictionController(IRestrictionService service, ILogger<RestrictionController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        #region Restriction Types

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("restrictiontypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllRestrictionTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRestrictionTypes(BaseFilter filter)
        {
            var results = await this._service.GetRestrictionTypesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<RestrictionTypeDTO>>(results));
        }

        #endregion

        [HttpPost("restrictiontypes")]
        //[Authorize(Authorization.Policies.ManageAllRestrictionTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(RestrictionTypeDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRestrictionType([FromBody] RestrictionTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateRestrictionTypeAsync(dto);
                if (result.IsSuccess)
                {
                    RestrictionTypeDTO vm = _mapper.Map<RestrictionTypeDTO>(result.Data);
                    return CreatedAtAction("GetRestrictionTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("restrictiontypes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllRestrictionTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(RestrictionTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRestrictionType(int id)
        {
            var dto = await this._service.GetRestrictionTypeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteRestrictionTypeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("restrictiontypes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllRestrictionTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRestrictionType(string id, [FromBody] RestrictionTypeDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetRestrictionTypeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateRestrictionTypeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Restrictiones

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("restrictions/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllRestrictionsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRestrictions(BaseFilter filter)
        {
            var results = await this._service.GetRestrictionsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<RestrictionDTO>>(results));
        }

        #endregion

        [HttpPost("restrictions")]
        //[Authorize(Authorization.Policies.ManageAllRestrictionsPolicy)]
        [ProducesResponseType(201, Type = typeof(RestrictionDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRestriction([FromBody] RestrictionDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateRestrictionAsync(dto);
                if (result.IsSuccess)
                {
                    RestrictionDTO vm = _mapper.Map<RestrictionDTO>(result.Data);
                    return CreatedAtAction("GetRestrictionById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("restrictions/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllRestrictionsPolicy)]
        [ProducesResponseType(200, Type = typeof(RestrictionDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRestriction(int id)
        {
            var dto = await this._service.GetRestrictionByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteRestrictionAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("restrictions/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllRestrictionsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRestriction(string id, [FromBody] RestrictionDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetRestrictionByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateRestrictionAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion
    }
}