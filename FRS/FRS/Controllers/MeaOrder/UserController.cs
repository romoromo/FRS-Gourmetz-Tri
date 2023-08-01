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
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private IUserService _service;
        readonly ILogger _logger;


        public UserController(IUserService service, ILogger<UserController> logger)
        {
            _service = service;
            _logger = logger;
        }

        #region User Types

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("stafftypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStaffTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStaffTypes(BaseFilter filter)
        {
            var results = await this._service.GetStaffTypesAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<StaffTypeDTO>>(results));
        }

        #endregion

        [HttpPost("stafftypes")]
        //[Authorize(Authorization.Policies.ManageAllStaffTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(StaffTypeDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateStaffType([FromBody] StaffTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateStaffTypeAsync(dto);
                if (result.IsSuccess)
                {
                    StaffTypeDTO vm = Mapper.Map<StaffTypeDTO>(result.Data);
                    return CreatedAtAction("GetStaffTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("stafftypes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllStaffTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(StaffTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStaffType(int id)
        {
            var dto = await this._service.GetStaffTypeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteStaffTypeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("stafftypes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllStaffTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStaffType(string id, [FromBody] StaffTypeDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetStaffTypeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateStaffTypeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion
    }
}