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
    public class ClassController : BaseController
    {
        private IClassService _service;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public ClassController(IClassService service, ILogger<ClassController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }
        #region Class Batches

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("classbatches/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllClassBatchesPolicy)]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetClassBatches(BaseFilter filter)
        {
            var results = await this._service.GetClassBatchesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<ClassBatchDTO>>(results));
        }

        #endregion

        [HttpPost("classbatches")]
        //[Authorize(Authorization.Policies.ManageAllClassBatchesPolicy)]
        [ProducesResponseType(201, Type = typeof(ClassBatchDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateClassBatch([FromBody] ClassBatchDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateClassBatchAsync(dto);
                if (result.IsSuccess)
                {
                    ClassBatchDTO vm = _mapper.Map<ClassBatchDTO>(result.Data);
                    return CreatedAtAction("GetClassBatchById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("classbatches/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllClassBatchesPolicy)]
        [ProducesResponseType(200, Type = typeof(ClassBatchDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteClassBatch(int id)
        {
            var dto = await this._service.GetClassBatchByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteClassBatchAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("classbatches/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllClassBatchesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateClassBatch(string id, [FromBody] ClassBatchDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetClassBatchByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateClassBatchAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Class Levels

        //#region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("classlevels/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllClassLevelsPolicy)]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetClassLevels(BaseFilter filter)
        {
            var results = await this._service.GetClassLevelsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<ClassLevelDTO>>(results));
        }

        #endregion

        [HttpPost("classlevels")]
        //[Authorize(Authorization.Policies.ManageAllClassLevelsPolicy)]
        [ProducesResponseType(201, Type = typeof(ClassLevelDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateClassLevel([FromBody] ClassLevelDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateClassLevelAsync(dto);
                if (result.IsSuccess)
                {
                    ClassLevelDTO vm = _mapper.Map<ClassLevelDTO>(result.Data);
                    return CreatedAtAction("GetClassLevelById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("classlevels/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllClassLevelsPolicy)]
        [ProducesResponseType(200, Type = typeof(ClassLevelDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteClassLevel(int id)
        {
            var dto = await this._service.GetClassLevelByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteClassLevelAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("classlevels/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllClassLevelsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateClassLevel(string id, [FromBody] ClassLevelDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetClassLevelByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateClassLevelAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        //[ApiKeyAuthorize]
        [HttpGet("classlevels/periodmealsession")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MealSessionDetailDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutletPeriodMealSessions(int outletId)
        {
            var results = await this._service.GetOutletPeriodMealSessions(outletId);
            return Ok(results);
        }

        //#endregion

        #region Classes

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("classes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllClasssPolicy)]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetClasss(BaseFilter filter)
        {
            var results = await this._service.GetClassesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<ClassDTO>>(results));
        }

        #endregion

        [HttpPost("classes")]
        //[Authorize(Authorization.Policies.ManageAllClasssPolicy)]
        [ProducesResponseType(201, Type = typeof(ClassDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateClass([FromBody] ClassDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateClassAsync(dto);
                if (result.IsSuccess)
                {
                    ClassDTO vm = _mapper.Map<ClassDTO>(result.Data);
                    return CreatedAtAction("GetClassById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("classes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllClasssPolicy)]
        [ProducesResponseType(200, Type = typeof(ClassDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var dto = await this._service.GetClassByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteClassAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("classes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllClasssPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateClass(string id, [FromBody] ClassDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetClassByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateClassAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Class Rostering

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("classrosters/sieve/list")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetClassRosters(ClassRosterFilter rosterFilter)
        {
            var results = await this._service.GetOutletClassRostersAsync(rosterFilter);
            return Ok(_mapper.Map<PagedEntityViewModel<OutletClassRosterDTO>>(results));
        }

        #endregion

        [ApiKeyAuthorize]
        [HttpGet("classrosters/get/id/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(OutletClassRosterDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetClassRosterById(int id)
        {
            var dto = await this._service.GetOutletClassRosterByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [ApiKeyAuthorize]
        [HttpGet("classrosters/{outletId}/{catererId}/{mealSessionId}")]
        //[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<OutletClassRosterDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetClassRosterByCatererOutlet(int outletId, int catererId, int mealSessionId)
        {
            var results = await this._service.GetOutletClassRosterByIdAsync(outletId, catererId, mealSessionId);
            return Ok(_mapper.Map<OutletClassRosterDTO>(results));
        }

        [HttpPost("classrosters")]
        //[Authorize(Authorization.Policies.ManageAllMenuCyclesPolicy)]
        [ProducesResponseType(201, Type = typeof(OutletClassRosterDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateClassRoster([FromBody] OutletClassRosterDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateOutletClassRosterAsync(dto);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [HttpPut("classrosters/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllClassRostersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateClassRoster(string id, [FromBody] OutletClassRosterDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetOutletClassRosterByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateOutletClassRosterAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpDelete("classrosters/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(MenuCycleDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteOutletClassRoster(int id)
        {
            var dto = await this._service.GetOutletClassRosterByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteOutletClassRosterAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPost("classrosters/export")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateClassRostersXls(ClassRosterFilter filter)
        {
            var xls = await _service.GenerateClassRostersXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_ClassRosters.xlsx";

            if (xls == null || xls.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: xls,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: reportName
            );
        }
        //[HttpPut("menucycles/update/{id}")]
        ////[Authorize(Authorization.Policies.ManageAllMenuCyclesPolicy)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> UpdateMenuCycle(string id, [FromBody] MenuCycleDTO model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (model == null)
        //            return BadRequest($"{nameof(model)} cannot be null");

        //        if (model.Id == 0)
        //            return BadRequest("Conflicting type id in parameter and model data");


        //        var dto = await this._service.GetMenuCycleByIdAsync(model.Id);

        //        if (dto == null)
        //            return NotFound(id);

        //        var result = await this._service.UpdateMenuCycleAsync(model);
        //        if (result.IsSuccess)
        //            return NoContent();

        //        AddErrors(new string[] { result.Message });

        //    }

        //    return BadRequest(ModelState);
        //}

        //[ApiKeyAuthorize]
        //[HttpGet("menucycles/scheduleperiods/list/{menuCycleId}/{day}")]
        ////[Authorize(Authorization.Policies.ViewAllMenuCyclesPolicy)]
        //[AllowAnonymous]
        //[ProducesResponseType(200, Type = typeof(List<MenuCycleSchedulePeriodDTO>))]
        //[ProducesResponseType(403)]
        //public async Task<IActionResult> GetMenuCycleSchedulePeriods(int menuCycleId, int day)
        //{
        //    var results = await this._service.GetMenuCycleSchedulePeriods(menuCycleId, day);
        //    return Ok(_mapper.Map<List<MenuCycleSchedulePeriodDTO>>(results));
        //}

        #endregion

        #region Dispenser Outlets

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("dispenseroutlets/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllClassLevelsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDispenserOutlets(BaseFilter filter)
        {
            var results = await this._service.GetDispenserOutletsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<DispenserOutletDTO>>(results));
        }

        #endregion

        [HttpPost("dispenseroutlets")]
        //[Authorize(Authorization.Policies.ManageAllClassLevelsPolicy)]
        [ProducesResponseType(201, Type = typeof(DispenserOutletDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDispenserOutlet([FromBody] DispenserOutletDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateDispenserOutletAsync(dto);
                if (result.IsSuccess)
                {
                    DispenserOutletDTO vm = _mapper.Map<DispenserOutletDTO>(result.Data);
                    return CreatedAtAction("GetDispenserOutletById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("dispenseroutlets/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllClassLevelsPolicy)]
        [ProducesResponseType(200, Type = typeof(DispenserOutletDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDispenserOutlet(int id)
        {
            var dto = await this._service.GetDispenserOutletByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteDispenserOutletAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("dispenseroutlets/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllClassLevelsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDispenserOutlet(string id, [FromBody] DispenserOutletDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetDispenserOutletByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateDispenserOutletAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region PLC

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("plc/sieve/list")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetPLCs(BaseFilter filter)
        {
            var results = await this._service.GetPLCPagedAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<PLCDTO>>(results));
        }

        #endregion

        [HttpPost("plc")]
        //[Authorize(Authorization.Policies.ManageAllClassLevelsPolicy)]
        [ProducesResponseType(201, Type = typeof(PLCDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePLC([FromBody] PLCDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreatePLCAsync(dto);
                if (result.IsSuccess)
                {
                    PLCDTO vm = _mapper.Map<PLCDTO>(result.Data);
                    return CreatedAtAction("GetPLCById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("plc/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(DispenserOutletDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePLC(int id)
        {
            var dto = await this._service.GetPLCByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeletePLCAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("plc/update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePLC(string id, [FromBody] PLCDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetPLCByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdatePLCAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region CLassLevelSchedule
        [HttpGet("classlevelschedule/sieve/list")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetClassLevelSchedule(BaseFilter filter)
        {
            return Ok(new PagedEntityViewModel<ClassLevelScheduleDTO>());
        }

        [HttpPost("classlevelschedule")]
        [ProducesResponseType(201, Type = typeof(ClassLevelDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateClassLevelSchedule([FromBody] ClassLevelScheduleDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                return CreatedAtAction("GetClassLevelScheduleById", new { id = dto.Id }, dto);
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("classlevelschedule/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(ClassLevelDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteClassLevelSchedule(int id)
        {
             return Ok(new ClassLevelScheduleDTO());
        }

        [HttpPut("classlevelschedule/update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateClassLevelSchedule(string id, [FromBody] ClassLevelDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                return NoContent();

            }

            return BadRequest(ModelState);
        }
        #endregion
    }
}