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
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class DishController : BaseController
    {
        private IDishService _service;
        readonly ILogger _logger;


        public DishController(IDishService service, ILogger<DishController> logger)
        {
            _service = service;
            _logger = logger;
        }

        #region Dish Types

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("dishtypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDishTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishTypes(BaseFilter filter)
        {
            var results = await this._service.GetDishTypesAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<DishTypeDTO>>(results));
        }

        #endregion

        [HttpPost("dishtypes")]
        //[Authorize(Authorization.Policies.ManageAllDishTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(DishTypeDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDishType([FromBody] DishTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateDishTypeAsync(dto);
                if (result.IsSuccess)
                {
                    DishTypeDTO vm = Mapper.Map<DishTypeDTO>(result.Data);
                    return CreatedAtAction("GetDishTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("dishtypes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDishTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(DishTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDishType(int id)
        {
            var dto = await this._service.GetDishTypeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteDishTypeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("dishtypes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDishTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDishType(string id, [FromBody] DishTypeDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetDishTypeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateDishTypeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Dishes

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("dishes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDishesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type  = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishes(BaseFilter filter)
        {
            var results = await this._service.GetDishesAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<DishDTO>>(results));
        }

        #endregion

        //[ApiKeyAuthorize]
        [HttpGet("dishes/changes")]
        //[Authorize(Authorization.Policies.ViewAllDishesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishChanges(DateTime updatedAfter, DateTime? updatedBefore)
        {
            var results = await this._service.GetDishChangesAsync(updatedAfter, updatedBefore);
            return Ok(Mapper.Map<List<DishSimple>>(results));
        }



        [ApiKeyAuthorize]
        [HttpGet("dishes/get/{outletid}")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(MealCreditSetDTO))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishesByMealType(int outletId, int catererId,int mealTypeId, DateTime orderDate, int? sessionId)
        {
            var results = await this._service.GetDishesByMealType(outletId, catererId, mealTypeId, orderDate, sessionId);
            return Ok(results);
        }


        [HttpGet("dishes/get/id/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(DishDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDish(int id)
        {
            var dto = await this._service.GetDishByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("dishes")]
        //[Authorize(Authorization.Policies.ManageAllDishesPolicy)]
        [ProducesResponseType(201, Type  = typeof(DishDTO))]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateDish([FromBody] DishDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateDishAsync(dto);
                if (result.IsSuccess)
                {
                    DishDTO vm = Mapper.Map<DishDTO>(result.Data);
                    return CreatedAtAction("GetDishById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("dishes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDishesPolicy)]
        [ProducesResponseType(200, Type  = typeof(DishDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dto = await this._service.GetDishByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteDishAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("dishes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDishesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> UpdateDish(string id, [FromBody] DishDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetDishByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateDishAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("dishes/generatecode")]
        //[Authorize(Authorization.Policies.ManageAllDishesPolicy)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        public async Task<IActionResult> GenerateCode(int catererId)
        {
            var code = await this._service.GenerateCode(catererId);
            return Ok(new { code });
        }

        #endregion

        #region Cuisines

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("cuisines/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllCuisinesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCuisines(BaseFilter filter)
        {
            var results = await this._service.GetCuisinesAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<CuisineDTO>>(results));
        }

        #endregion

        [HttpPost("cuisines")]
        //[Authorize(Authorization.Policies.ManageAllCuisinesPolicy)]
        [ProducesResponseType(201, Type = typeof(CuisineDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCuisine([FromBody] CuisineDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateCuisineAsync(dto);
                if (result.IsSuccess)
                {
                    CuisineDTO vm = Mapper.Map<CuisineDTO>(result.Data);
                    return CreatedAtAction("GetCuisineById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("cuisines/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllCuisinesPolicy)]
        [ProducesResponseType(200, Type = typeof(CuisineDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCuisine(int id)
        {
            var dto = await this._service.GetCuisineByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteCuisineAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("cuisines/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllCuisinesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCuisine(string id, [FromBody] CuisineDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetCuisineByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateCuisineAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Dish Cycles

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("dishcycles/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDishCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishCycles(BaseFilter filter)
        {
            var results = await this._service.GetDishCyclesAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<DishCycleDTO>>(results));
        }

        #endregion

        [HttpPost("dishcycles")]
        //[Authorize(Authorization.Policies.ManageAllDishCyclesPolicy)]
        [ProducesResponseType(201, Type = typeof(DishCycleDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDishCycle([FromBody] DishCycleDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateDishCycleAsync(dto);
                if (result.IsSuccess)
                {
                    DishCycleDTO vm = Mapper.Map<DishCycleDTO>(result.Data);
                    return CreatedAtAction("GetDishCycleById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("dishcycles/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDishCyclesPolicy)]
        [ProducesResponseType(200, Type = typeof(DishCycleDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDishCycle(int id)
        {
            var dto = await this._service.GetDishCycleByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteDishCycleAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("dishcycles/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDishCyclesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDishCycle(string id, [FromBody] DishCycleDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetDishCycleByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateDishCycleAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("dishcycles/student/{id}")]
        [ProducesResponseType(200, Type = typeof(List<DishCycleDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishCyclesByStudentId(int id, DateTime orderDate, int mealSessionId)
        {
            var results = await this._service.GetOutletDishCyclesAsync(id, orderDate, mealSessionId);
            return Ok(results);
        }
        #endregion

        #region Dish Cycle Calendars

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("dishcyclecalendars/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDishCycleCalendarsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishCycleCalendars(BaseFilter filter)
        {
            var results = await this._service.GetDishCycleCalendarsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<DishCycleCalendarDTO>>(results));
        }

        #endregion


        [HttpPost("dishcycles/blockunblockdate")]
        //[Authorize(Authorization.Policies.ManageAllDishCyclesPolicy)]
        [ProducesResponseType(201, Type = typeof(DishCycleDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> BlockDishCycleDate([FromBody] DishBlockUnblockDateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");


                var result = model.IsUnblock ? await this._service.UnblockDishCycleDate(model.BlockedDates) : await this._service.BlockDishCycleDate(model.BlockedDates);
                if (result.IsSuccess)
                {
                    return Ok();
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [ApiKeyAuthorize]
        [HttpGet("dishcycles/getScheduleSetDetails/{dishCycleId}/{day}")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<DishCycleScheduleSetDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishCycleScheduleSetMenus(int dishCycleId, int day, int? outletId)
        {
            var results = await this._service.GetDishCycleScheduleSetMenus(dishCycleId, day, outletId);
            return Ok(Mapper.Map<List<DishCycleScheduleSetDTO>>(results));
        }
        #endregion

        #region Outlet Dish Cycles

        [ApiKeyAuthorize]
        [HttpGet("dishcycles/outlets/list")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<DishCycleDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutletDishCyclesAsync(int outletId, int catererId)
        {
            var results = await this._service.GetOutletDishCyclesAsync(outletId, catererId);
            return Ok(Mapper.Map<List<DishCycleDTO>>(results));
        }

        [ApiKeyAuthorize]
        [HttpGet("dishcycles/outlets/periods/list")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<DishCyclePeriodDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutletDishCyclePeriodsAsync(int dishCycleId)
        {
            var results = await this._service.GetOutletDishCyclePeriodsAsync(dishCycleId);
            return Ok(Mapper.Map<List<DishCyclePeriodDTO>>(results));
        }

        [HttpPost("dishcycles/outlets/blockunblockdate")]
        //[Authorize(Authorization.Policies.ManageAllMenuCyclesPolicy)]
        [ProducesResponseType(201, Type = typeof(MenuCycleDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> BlockOutletDate([FromBody] BlockUnblockOutletDishDateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");


                var result = model.IsUnblock ? await this._service.DishUnblockOutletDate(model.BlockedDates) : await this._service.DishBlockOutletDate(model.BlockedDates);
                if (result.IsSuccess)
                {
                    return Ok();
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [ApiKeyAuthorize]
        [HttpPost("dishcycles/outlets/scheduleperiods")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateOutletDishCyclePeriodMenus([FromBody] OutletDishViewMenuDTO models)
        {
            var results = await this._service.CreateOutletDishCyclePeriodMenus(models);
            return Ok(results);
        }
        #endregion
    }
}