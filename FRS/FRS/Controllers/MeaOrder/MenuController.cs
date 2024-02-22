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
    public class MenuController : BaseController
    {
        private IMenuService _service;
        readonly ILogger _logger;


        public MenuController(IMenuService service, ILogger<MenuController> logger)
        {
            _service = service;
            _logger = logger;
        }

        #region Menus

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("menus/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllMenusPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type  = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMenus(BaseFilter filter)
        {
            var results = await this._service.GetMenusAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<MenuDTO>>(results));
        }

        [ApiKeyAuthorize]
        [HttpGet("mealtypedishes/get/{id}/{pageNumber}/{pageSize}")]
        //[Authorize(Authorization.Policies.ViewAllMenusPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(MenuDishMealTypeDTO))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMenuDishMealTypesByIdAsync(int id, int pageNumber, int pageSize)
        {
            var results = await this._service.GetMenuDishMealTypesByIdAsync(id, pageNumber, pageSize);
            return Ok(results);
        }

        #endregion

        [HttpPost("menus")]
        //[Authorize(Authorization.Policies.ManageAllMenusPolicy)]
        [ProducesResponseType(201, Type  = typeof(MenuDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMenu([FromBody] MenuDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateMenuAsync(dto);
                if (result.IsSuccess)
                {
                    MenuDTO vm = Mapper.Map<MenuDTO>(result.Data);
                    return CreatedAtAction("GetMenuById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("menus/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMenusPolicy)]
        [ProducesResponseType(200, Type  = typeof(MenuDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var dto = await this._service.GetMenuByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteMenuAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("menus/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMenusPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMenu(string id, [FromBody] MenuDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetMenuByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateMenuAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Menu Cycles

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("menucycles/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMenuCycles(BaseFilter filter)
        {
            var results = await this._service.GetMenuCyclesAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<MenuCycleDTO>>(results));
        }

        #endregion

        [HttpPost("menucycles")]
        //[Authorize(Authorization.Policies.ManageAllMenuCyclesPolicy)]
        [ProducesResponseType(201, Type = typeof(MenuCycleDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMenuCycle([FromBody] MenuCycleDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateMenuCycleAsync(dto);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("menucycles/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMenuCyclesPolicy)]
        [ProducesResponseType(200, Type = typeof(MenuCycleDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMenuCycle(int id)
        {
            var dto = await this._service.GetMenuCycleByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteMenuCycleAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("menucycles/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMenuCyclesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMenuCycle(string id, [FromBody] MenuCycleDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetMenuCycleByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateMenuCycleAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpPost("menucycles/blockunblockdate")]
        //[Authorize(Authorization.Policies.ManageAllMenuCyclesPolicy)]
        [ProducesResponseType(201, Type = typeof(MenuCycleDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> BlockMenuCycleDate([FromBody] BlockUnblockDateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");


                var result =  model.IsUnblock ? await this._service.UnblockMenuCycleDate(model.BlockedDates) : await this._service.BlockMenuCycleDate(model.BlockedDates);
                if (result.IsSuccess)
                {
                    return Ok();
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [ApiKeyAuthorize]
        [HttpGet("menucycles/scheduleperiods/list/{menuCycleId}/{day}")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MenuCycleSchedulePeriodDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMenuCycleSchedulePeriods(int menuCycleId, int day)
        {
            var results = await this._service.GetMenuCycleSchedulePeriods(menuCycleId, day);
            return Ok(Mapper.Map<List<MenuCycleSchedulePeriodDTO>>(results));
        }

        [ApiKeyAuthorize]
        [HttpPost("menucycles/scheduleperiods")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MenuCycleSchedulePeriodDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateSchedulePeriodMenus([FromBody] List<MenuCycleSchedulePeriodDTO> models)
        {
            var results = await this._service.CreateSchedulePeriodMenus(models);
            return Ok(results);
        }
        #endregion

        #region Outlet Menu Cycle

        [ApiKeyAuthorize]
        [HttpGet("outlets/list")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MenuCycleDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutletMenuCyclesAsync(int outletId, int catererId)
        {
            var results = await this._service.GetOutletMenuCyclesAsync(outletId, catererId);
            return Ok(Mapper.Map<List<MenuCycleDTO>>(results));
        }

        [HttpPost("outlets/blockunblockdate")]
        //[Authorize(Authorization.Policies.ManageAllMenuCyclesPolicy)]
        [ProducesResponseType(201, Type = typeof(MenuCycleDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> BlockOutletDate([FromBody] BlockUnblockOutletDateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");


                var result = model.IsUnblock ? await this._service.UnblockOutletDate(model.BlockedDates) : await this._service.BlockOutletDate(model.BlockedDates);
                if (result.IsSuccess)
                {
                    return Ok();
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [ApiKeyAuthorize]
        [HttpGet("outlets/scheduleperiods/list/{outletId}/{catererId}/{menuCycleId}/{day}")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MenuCycleSchedulePeriodDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutletMenuCycleSchedulePeriods(int outletId, int catererId, int menuCycleId, int day)
        {
            var results = await this._service.GetOutletMenuCycleSchedulePeriods(outletId, catererId, menuCycleId, day);
            return Ok(Mapper.Map<List<MenuCycleSchedulePeriodDTO>>(results));
        } 

        [ApiKeyAuthorize]
        [HttpPost("outlets/scheduleperiods")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MenuCycleSchedulePeriodDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateOutletSchedulePeriodMenus([FromBody] List<MenuCycleSchedulePeriodDTO> models)
        {
            var results = await this._service.CreateOutletSchedulePeriodMenus(models);
            return Ok(results);
        }
        #endregion

        #region Student Menu Cycle
        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("students/list")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<OutletClassRosterDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStudentMenuCyclesAsync(int studentId, int outletId, int catererId)
        {
            var results = await this._service.GetStudentMenuCyclesAsync(studentId, outletId, catererId);
            return Ok(results);
        }


        //[ApiKeyAuthorize]
        [HttpGet("students/mealsessions")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MealSessionDetailDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStudentSessions(int studentId, DateTime orderDate)
        {
            var results = await this._service.GetStudentSessions(studentId, orderDate);
            return Ok(results);
        }

        //[ApiKeyAuthorize]
        //[HttpGet("outlets/mealsessions")]
        ////[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        //[ProducesResponseType(200, Type = typeof(List<MealSessionDetailDTO>))]
        //[ProducesResponseType(403)]
        //public async Task<IActionResult> GetOutletMealSessions(int outletId, DateTime? startDate, DateTime? endDate)
        //{
        //    var results = await this._service.GetOutletMealSessions(outletId, startDate, endDate);
        //    return Ok(results);
        //}


        [ApiKeyAuthorize]
        [HttpGet("outlets/mealsessions")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MealSessionDetailDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutletMealSessions(int outletId, DateTime orderDate, DateTime? orderDateTo = null)
        {
            var results = await this._service.GetOutletMealSessions(outletId, orderDate, null, null, orderDateTo);
            return Ok(results);
        }
        #endregion

        #region Menu Cycle Calendars

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("menucyclecalendars/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllMenuCycleCalendarsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMenuCycleCalendars(BaseFilter filter)
        {
            var results = await this._service.GetMenuCycleCalendarsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<MenuCycleCalendarDTO>>(results));
        }

        #endregion

        [HttpPost("menucyclecalendars")]
        //[Authorize(Authorization.Policies.ManageAllMenuCycleCalendarsPolicy)]
        [ProducesResponseType(201, Type = typeof(MenuCycleCalendarDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMenuCycleCalendar([FromBody] MenuCycleCalendarDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateMenuCycleCalendarAsync(dto);
                if (result.IsSuccess)
                {
                    MenuCycleCalendarDTO vm = Mapper.Map<MenuCycleCalendarDTO>(result.Data);
                    return CreatedAtAction("GetMenuCycleCalendarById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("menucyclecalendars/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMenuCycleCalendarsPolicy)]
        [ProducesResponseType(200, Type = typeof(MenuCycleCalendarDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMenuCycleCalendar(int id)
        {
            var dto = await this._service.GetMenuCycleCalendarByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteMenuCycleCalendarAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("menucyclecalendars/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMenuCycleCalendarsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMenuCycleCalendar(string id, [FromBody] MenuCycleCalendarDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetMenuCycleCalendarByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateMenuCycleCalendarAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Menu Group

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("menugroups/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllMenusPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMenuGroups(BaseFilter filter)
        {
            var results = await this._service.GetMenuGroupsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<MenuGroupDTO>>(results));
        }

        #endregion

        [HttpGet("menugroups/dishcycles")]
        //[Authorize(Authorization.Policies.ViewAllMenusPolicy)]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<DishCycleDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMenuGroupDishCyclesAsync(int studentId, DateTime orderDate)
        {
            var results = await this._service.GetMenuGroupDishCyclesAsync(studentId, orderDate);
            return Ok(Mapper.Map<List<DishCycleDTO>>(results));
        }

        [HttpGet("menugroups/activedishcycles")]
        //[Authorize(Authorization.Policies.ViewAllMenusPolicy)]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MenuGroupDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetActiveMenuGroupDishCyclesAsync(int studentId)
        {
            var results = await this._service.GetActiveMenuGroupDishCyclesAsync(studentId);
            return Ok(results);
        }

        [HttpGet("menugroups/dishesbydate")]
        //[Authorize(Authorization.Policies.ViewAllMenusPolicy)]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<DishByDateDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMenuGroupDishesByDateAsync(int studentId, DateTime startDate, DateTime endDate)
        {
            var results = await this._service.GetMenuGroupDishesByDateAsync(studentId, startDate, endDate);
            return Ok(Mapper.Map<List<DishByDateDTO>>(results));
        }

        [HttpPost("menugroups")]
        //[Authorize(Authorization.Policies.ManageAllMenusPolicy)]
        [ProducesResponseType(201, Type = typeof(MenuGroupDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMenuGroup([FromBody] MenuGroupDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateMenuGroupAsync(dto);
                if (result.IsSuccess)
                {
                    MenuGroupDTO vm = Mapper.Map<MenuGroupDTO>(result.Data);
                    return CreatedAtAction("GetMenuGroupById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("menugroups/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMenusPolicy)]
        [ProducesResponseType(200, Type = typeof(MenuGroupDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMenuGroup(int id)
        {
            var dto = await this._service.GetMenuGroupByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteMenuGroupAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("menugroups/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMenusPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMenuGroup(string id, [FromBody] MenuGroupDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetMenuGroupByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateMenuGroupAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
        #endregion
    }
}