using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces.MealOrder;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Helpers;
using DAL.Filters;
using FRS.Attributes;
using FRS.ViewModels;
using FRS.ViewModels.MealOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class DishController : BaseController
    {
        private IDishService _service;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public DishController(IDishService service, ILogger<DishController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
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
            return Ok(_mapper.Map<PagedEntityViewModel<DishTypeDTO>>(results));
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
                    DishTypeDTO vm = _mapper.Map<DishTypeDTO>(result.Data);
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

        [HttpGet("dishtypes/by-active-dishcycles")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<DishTypeDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishTypesByActiveDishCycles(int outletId, DateTime deliveryDate, DateTime deliveryDateTo)
        {
            var results = await this._service.GetDishTypesByActiveDishCyclesAsync(outletId, deliveryDate, deliveryDateTo);
            return Ok(results);
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
            return Ok(_mapper.Map<PagedEntityViewModel<DishDTO>>(results));
        }

        [HttpGet("dishes/sieve/list-lite")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishesLite(BaseFilter filter)
        {
            var results = await this._service.GetDishesLiteAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<DishLiteDTO>>(results));
        }

        [HttpPost("dishes/sieve/list/export")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetDishesExport(BaseFilter filter)
        {
            var xls = await _service.GenerateDishXlsx(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_DishList.xlsx";

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
            return Ok(_mapper.Map<List<DishSimple>>(results));
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
                    DishDTO vm = _mapper.Map<DishDTO>(result.Data);
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


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("dishes/import"), DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<IActionResult> ImportDish()
        {
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length <= 0) return BadRequest("No File Imported");

                string catererIdParam = Request.Form["catererId"];
                int catererId = 0;
                bool isValidCatererId = int.TryParse(catererIdParam, out catererId);

                if (string.IsNullOrEmpty(catererIdParam) && !isValidCatererId)
                    return BadRequest("CatererId Id is missing.");

                string userIdParam = Request.Form["userId"];
                int userId = 0;
                bool isValidUserId = int.TryParse(userIdParam, out userId);

                if (string.IsNullOrEmpty(userIdParam) && !isValidUserId)
                    return BadRequest("User Id is missing.");

                var folderName = Path.Combine("Resources", "Excel");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                Directory.CreateDirectory(pathToSave);

                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                fileName = Guid.NewGuid().ToString() + fileName;
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                IWorkbook wb = new XSSFWorkbook(new FileStream(fullPath, FileMode.Open));
                var studentIds = new List<int>();
                int startRow = 1;
                var valueExcel = new List<DishImportInputDTO>();
                for (int x = 0; x < wb.NumberOfSheets; x++)
                {
                    var sheet = (XSSFSheet)wb.GetSheetAt(x);
                    int reqNumCells = startRow - 1;
                    int colCount = sheet.GetRow(reqNumCells).PhysicalNumberOfCells;

                    int rowCount = sheet.PhysicalNumberOfRows;
                    for (int i = startRow; ExcelUtility.GetRowWithNonEmptyCell(sheet, i) != null; i++)
                    {
                        var fRow = sheet.GetRow(i);
                        if (fRow == null) continue;

                        var dto = new DishImportInputDTO();
                        int c = 0;

                        var label = fRow.GetCell(c++);
                        dto.Label = label != null ? label.ToString().Trim() : "";

                        var productDescription = fRow.GetCell(c++);
                        dto.ProductionDescription = productDescription != null ? productDescription.ToString().Trim() : "";

                        var extraNote = fRow.GetCell(c++);
                        dto.ExtraNote = extraNote != null ? extraNote.ToString().Trim() : "";

                        var rrpValue = fRow.GetCell(c++);
                        if (rrpValue != null && double.TryParse(rrpValue.ToString(), out double rrp))
                            dto.RPP = rrp;
                        else
                            dto.RPP = 0;

                        var costValue = fRow.GetCell(c++);
                        if (costValue != null && double.TryParse(costValue.ToString(), out double cost))
                            dto.Cost = cost;
                        else
                            dto.Cost = 0;

                        var dishTypeName = fRow.GetCell(c++);
                        dto.DishTypeName = dishTypeName != null ? dishTypeName.ToString().Trim() : "";

                        var bentoBoxTypeName = fRow.GetCell(c++);
                        dto.BentoBoxTypeName = bentoBoxTypeName != null ? bentoBoxTypeName.ToString().Trim() : "";

                        var cuisineName = fRow.GetCell(c++);
                        dto.CuisineName = cuisineName != null ? cuisineName.ToString().Trim() : "";

                        var kitchenName = fRow.GetCell(c++);
                        dto.KicthenName = kitchenName != null ? kitchenName.ToString().Trim() : "";

                        var sapCode = fRow.GetCell(c++);
                        dto.SapCode = sapCode != null ? sapCode.ToString().Trim() : "";

                        var proteinValue = fRow.GetCell(c++);
                        if (proteinValue != null && float.TryParse(proteinValue.ToString(), out float protein))
                            dto.Protein = protein;
                        else
                            dto.Protein = 0;

                        var sugarValue = fRow.GetCell(c++);
                        if (sugarValue != null && float.TryParse(sugarValue.ToString(), out float sugar))
                            dto.Sugar = sugar;
                        else
                            dto.Sugar = 0;

                        var totalFatValue = fRow.GetCell(c++);
                        if (totalFatValue != null && float.TryParse(totalFatValue.ToString(), out float totalFat))
                            dto.TotalFat = totalFat;
                        else
                            dto.TotalFat = 0;

                        var totalCarbValue = fRow.GetCell(c++);
                        if (totalCarbValue != null && float.TryParse(totalCarbValue.ToString(), out float totalCarb))
                            dto.TotalCarb = totalCarb;
                        else
                            dto.TotalCarb = 0;

                        var caloriesValue = fRow.GetCell(c++);
                        if (caloriesValue != null && float.TryParse(caloriesValue.ToString(), out float calories))
                            dto.Calories = calories;
                        else
                            dto.Calories = 0;

                        var restrictionNames = fRow.GetCell(c++);
                        string restrictionString = restrictionNames != null ? restrictionNames.ToString().Trim() : "";
                        List<string> dataRestriction = new List<string>();
                        if (!string.IsNullOrEmpty(restrictionString))
                            dataRestriction = restrictionString.Split(',').ToList();
                        dto.Restrictions = dataRestriction;

                        var enabledValue = fRow.GetCell(c++);
                        dto.IsEnabled = string.Equals(enabledValue?.ToString()?.Trim(), "yes", StringComparison.OrdinalIgnoreCase);

                        valueExcel.Add(dto);

                    }
                }
                var responseData = await _service.DishImport(catererId, valueExcel,userId);
                return Ok(responseData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ImportDish : {ex.Message}", ex);
                _logger.LogError($"Error ImportDish : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }

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
            return Ok(_mapper.Map<PagedEntityViewModel<CuisineDTO>>(results));
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
                    CuisineDTO vm = _mapper.Map<CuisineDTO>(result.Data);
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

        [HttpGet("dishcycles/simple/sieve/list")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishCyclesSimple(BaseFilter filter)
        {
            var results = await this._service.GetDishCyclesSimpleAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<DishCycleSimpleDTO>>(results));
        }

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
            return Ok(_mapper.Map<PagedEntityViewModel<DishCycleDTO>>(results));
        }

        #endregion

        [HttpGet("dishcycles/get/id/{id}")]
        [ProducesResponseType(200, Type = typeof(DishCycleDTO))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDishCycleById(int id)
        {
            var results = await this._service.GetDishCycleByIdAsync(id);
            return Ok(results);
        }

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
                    DishCycleDTO vm = _mapper.Map<DishCycleDTO>(result.Data);
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
            var results = await this._service.GetOutletStudentDishCyclesAsync(id, orderDate, mealSessionId);
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
            return Ok(_mapper.Map<PagedEntityViewModel<DishCycleCalendarDTO>>(results));
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
            return Ok(_mapper.Map<List<DishCycleScheduleSetDTO>>(results));
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
            return Ok(_mapper.Map<List<DishCycleDTO>>(results));
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
            return Ok(_mapper.Map<List<DishCyclePeriodDTO>>(results));
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