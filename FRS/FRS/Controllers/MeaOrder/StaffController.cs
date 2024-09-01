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
using DAL.Core.Interfaces;
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
    public class StaffController : BaseController
    {
        private IStaffService _service;
        readonly ILogger _logger;
        private readonly IAccountManager _accountManager;
        private readonly IMapper _mapper;

        public StaffController(IStaffService service, ILogger<StaffController> logger, IAccountManager accountManager, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _accountManager = accountManager;
            _mapper = mapper;
        }

        #region Staffs

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("staffs/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStaffsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStaffs(BaseFilter filter)
        {
            var results = await this._service.GetStaffsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<StaffDTO>>(results));
        }

        #endregion

        [HttpPost("staffs")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStaffsPolicy)]
        [ProducesResponseType(201, Type = typeof(StaffDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateStaff([FromBody] StaffDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateStaffAsync(dto);
                if (result.IsSuccess)
                {
                    StaffDTO vm = _mapper.Map<StaffDTO>(result.Data);
                    return CreatedAtAction("GetStaffById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("staffs/get/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStaffsPolicy)]
        [ProducesResponseType(200, Type = typeof(StaffDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStaff(int id)
        {
            var dto = await this._service.GetStaffByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpDelete("staffs/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStaffsPolicy)]
        [ProducesResponseType(200, Type = typeof(StaffDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var dto = await this._service.GetStaffByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteStaffAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("staffs/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStaffsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStaff(string id, [FromBody] StaffDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetStaffByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateStaffAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

    }
}