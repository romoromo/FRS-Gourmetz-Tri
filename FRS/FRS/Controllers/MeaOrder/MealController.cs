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
using DAL.Core.DTO;
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
    public class MealController : BaseController
    {
        private IMealService _service;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public MealController(IMealService service, ILogger<MealController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        #region Meal Types

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("mealtypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllMealTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMealTypes(BaseFilter filter)
        {
            var results = await this._service.GetMealTypesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<MealTypeDTO>>(results));
        }

        #endregion

        [HttpPost("mealtypes")]
        //[Authorize(Authorization.Policies.ManageAllMealTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(MealTypeDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMealType([FromBody] MealTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateMealTypeAsync(dto);
                if (result.IsSuccess)
                {
                    MealTypeDTO vm = _mapper.Map<MealTypeDTO>(result.Data);
                    return CreatedAtAction("GetMealTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("mealtypes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMealTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(MealTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMealType(int id)
        {
            var dto = await this._service.GetMealTypeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteMealTypeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("mealtypes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMealTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMealType(string id, [FromBody] MealTypeDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetMealTypeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateMealTypeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Meal Periods

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("mealperiods/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllMealPeriodsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMealPeriods(BaseFilter filter)
        {
            var results = await this._service.GetMealPeriodsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<MealPeriodDTO>>(results));
        }

        #endregion

        [HttpPost("mealperiods")]
        //[Authorize(Authorization.Policies.ManageAllMealPeriodsPolicy)]
        [ProducesResponseType(201, Type = typeof(MealPeriodDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMealPeriod([FromBody] MealPeriodDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateMealPeriodAsync(dto);
                if (result.IsSuccess)
                {
                    MealPeriodDTO vm = _mapper.Map<MealPeriodDTO>(result.Data);
                    return CreatedAtAction("GetMealPeriodById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("mealperiods/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMealPeriodsPolicy)]
        [ProducesResponseType(200, Type = typeof(MealPeriodDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMealPeriod(int id)
        {
            var dto = await this._service.GetMealPeriodByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteMealPeriodAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("mealperiods/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMealPeriodsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMealPeriod(string id, [FromBody] MealPeriodDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetMealPeriodByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateMealPeriodAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Meal Sessions

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("mealsessions/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllMealSessionsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMealSessions(BaseFilter filter)
        {
            var results = await this._service.GetMealSessionsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<MealSessionDTO>>(results));
        }

        #endregion


        [ApiKeyAuthorize]
        [HttpGet("mealsessions/get/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(MealSessionDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMealSession(int id)
        {
            var dto = await this._service.GetMealSessionByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [ApiKeyAuthorize]
        [HttpGet("mealsessions/mealperiod")]
        //[Authorize(Authorization.Policies.ViewAllMealSessionsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MealSessionMealPeriodDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMealSessionsByMealPeriod(int outletId, int catererId, int outletProfileId)
        {
            var results = await this._service.GetMealSessionsByMealPeriod(outletId, catererId, outletProfileId);
            return Ok(_mapper.Map<List<MealSessionMealPeriodDTO>>(results));
        }

        [ApiKeyAuthorize]
        [HttpGet("mealsessions/mealperiod/all")]
        //[Authorize(Authorization.Policies.ViewAllMealSessionsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MealSessionMealPeriodDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllMealSessionsByMealPeriod(int outletId, int catererId, int outletProfileId)
        {
            var results = await this._service.GetAllMealSessionsByMealPeriod(outletId, catererId, outletProfileId);
            return Ok(results);
        }

        [ApiKeyAuthorize]
        [HttpGet("mealsessions/mealperiod/outlet")]
        //[Authorize(Authorization.Policies.ViewAllMealSessionsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MealSessionMealPeriodDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMealSessionsByMealPeriodByOutlet(int outletId)
        {
            var results = await this._service.GetMealSessionsByMealPeriodByOutlet(outletId);
            return Ok(_mapper.Map<List<MealSessionMealPeriodDTO>>(results));
        }

        [ApiKeyAuthorize]
        [HttpGet("mealsessions/outlet-lite")]
        [ProducesResponseType(200, Type = typeof(List<MealSessionLiteDto>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMealSessionLiteByOutletId(int outletId)
        {
            var results = await this._service.GetMealSessionLiteByOutletId(outletId);
            return Ok(results);
        }

        [HttpPost("mealsessions")]
        //[Authorize(Authorization.Policies.ManageAllMealSessionsPolicy)]
        [ProducesResponseType(201, Type = typeof(MealSessionDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMealSession([FromBody] MealSessionDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateMealSessionAsync(dto);
                if (result.IsSuccess)
                {
                    MealSessionDTO vm = _mapper.Map<MealSessionDTO>(result.Data);
                    return CreatedAtAction("GetMealSessionById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("mealsessions/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMealSessionsPolicy)]
        [ProducesResponseType(200, Type = typeof(MealSessionDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMealSession(int id)
        {
            var dto = await this._service.GetMealSessionByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteMealSessionAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("mealsessions/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMealSessionsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMealSession(string id, [FromBody] MealSessionDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetMealSessionByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateMealSessionAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpPost("mealsessions/bulkcreate")]
        //[Authorize(Authorization.Policies.ManageAllMealSessionsPolicy)]
        [ProducesResponseType(201, Type = typeof(MealSessionDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> BulkCreateMealSession([FromBody] List<MealSessionDTO> dto)
        {
            if (dto == null)
                return BadRequest($"{nameof(dto)} cannot be null");


            var result = await this._service.BulkCreateMealSessionAsync(dto);
            if (result.IsSuccess)
            {
                return CreatedAtAction("GetMealSessionById", new { result });
            }

            AddErrors(new string[] { result.Message });

            return BadRequest(ModelState);
        }
        #endregion
    }
}