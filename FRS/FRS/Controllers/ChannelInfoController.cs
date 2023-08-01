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
    public class ChannelInfoController : BaseController
    {
        private IChannelInfoService _service;
        readonly ILogger _logger;


        public ChannelInfoController(IChannelInfoService service, ILogger<ChannelInfoController> logger)
        {
            _service = service;
            _logger = logger;
        }

        #region Device Types

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("channelinfos/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllChannelInfosPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetChannelInfos(BaseFilter filter)
        {
            var results = await this._service.GetChannelInfosAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<ChannelInfoDTO>>(results));
        }

        #endregion

        [HttpPost("channelinfos")]
        //[Authorize(Authorization.Policies.ManageAllChannelInfosPolicy)]
        [ProducesResponseType(201, Type = typeof(ChannelInfoDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateChannelInfo([FromBody] ChannelInfoDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateChannelInfoAsync(dto);
                if (result.IsSuccess)
                {
                    ChannelInfoDTO vm = Mapper.Map<ChannelInfoDTO>(result.Data);
                    return CreatedAtAction("GetChannelInfoById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("channelinfos/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllChannelInfosPolicy)]
        [ProducesResponseType(200, Type = typeof(ChannelInfoDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteChannelInfo(int id)
        {
            var dto = await this._service.GetChannelInfoByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteChannelInfoAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("channelinfos/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllChannelInfosPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateChannelInfo(string id, [FromBody] ChannelInfoDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetChannelInfoByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateChannelInfoAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

    }
}