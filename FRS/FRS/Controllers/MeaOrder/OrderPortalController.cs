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
using FRS.ViewModels.MealOrder;
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
    public class OrderPortalController : BaseController
    {
        private IOrderPortalService _service;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public OrderPortalController(IOrderPortalService service, ILogger<OrderPortalController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        #region Contents

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("contents/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllOrderPortalContentesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type  = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOrderPortalContentes(BaseFilter filter)
        {
            var results = await this._service.GetOrderPortalContentsAsync(filter);
            return Ok(results);
        }

        #endregion

        [HttpGet("contents/get/id/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(OrderPortalContentDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOrderPortalContent(int id)
        {
            var dto = await this._service.GetOrderPortalContentByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }


        [HttpGet("contents/first")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(OrderPortalContentDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOrderPortalContentFirst(int outletId)
        {
            var dto = await this._service.GetOrderPortalContentFirst(outletId);
            if (dto == null)
                return NotFound(outletId);

            dto.Banners = dto.Banners.OrderBy(e => e.Order).ThenBy(e => e.Id).ToList();
            return Ok(dto);
        }

        [HttpPost("contents")]
        //[Authorize(Authorization.Policies.ManageAllOrderPortalContentesPolicy)]
        [ProducesResponseType(201, Type  = typeof(OrderPortalContentDTO))]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateOrderPortalContent([FromBody] OrderPortalContentDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateOrderPortalContentAsync(dto);
                if (result.IsSuccess)
                {
                    OrderPortalContentDTO vm = _mapper.Map<OrderPortalContentDTO>(result.Data);
                    return CreatedAtAction("GetOrderPortalContentById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("contents/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOrderPortalContentesPolicy)]
        [ProducesResponseType(200, Type  = typeof(OrderPortalContentDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> DeleteOrderPortalContent(int id)
        {
            var dto = await this._service.GetOrderPortalContentByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteOrderPortalContentAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("contents/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOrderPortalContentesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> UpdateOrderPortalContent(string id, [FromBody] OrderPortalContentDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetOrderPortalContentByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateOrderPortalContentAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

    }
}