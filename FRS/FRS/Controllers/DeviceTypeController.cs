using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.Services.Interfaces;
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
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class DeviceTypeController : BaseController
    {
        private IDeviceService _service;
        readonly ILogger _logger;


        public DeviceTypeController(IDeviceService service, ILogger<DeviceTypeController> logger)
        {
            _service = service;
            _logger = logger;
        }

        #region Device Types

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("devicetypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDeviceTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDeviceTypes(BaseFilter filter)
        {
            var results = await this._service.GetDeviceTypesAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<DeviceTypeDTO>>(results));
        }

        #endregion

        [HttpPost("devicetypes")]
        //[Authorize(Authorization.Policies.ManageAllDeviceTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(DeviceTypeDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDeviceType([FromBody] DeviceTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateDeviceTypeAsync(dto);
                if (result.IsSuccess)
                {
                    DeviceTypeDTO vm = Mapper.Map<DeviceTypeDTO>(result.Data);
                    return CreatedAtAction("GetDeviceTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("devicetypes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDeviceTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDeviceType(int id)
        {
            var dto = await this._service.GetDeviceTypeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteDeviceTypeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("devicetypes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDeviceTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDeviceType(string id, [FromBody] DeviceTypeDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetDeviceTypeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateDeviceTypeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

    }
}