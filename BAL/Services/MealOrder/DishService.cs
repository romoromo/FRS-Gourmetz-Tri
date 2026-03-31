using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL;
using AutoMapper;
using System.IO;
using BAL.Services.Interfaces.MealOrder;
using BAL.DTO.MealOrder;
using DAL.Models.MealOrder;
using DAL.Core.Interfaces;
using DAL.Core.Logging;
using Microsoft.Extensions.Logging;
using DAL.Core.DTO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Linq;

namespace BAL.Services.MealOrder
{
    public class DishService : IDishService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private IAccountManager _accountManager;
        private ILogger _logger;
        private readonly IMapper _mapper;

        public DishService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
            _logger = Logger.CreateLogger<DishService>();
            _mapper = mapper;
        }

        #region Dish Type

        public async Task<PagedEntity<DishTypeDTO>> GetDishTypesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<DishTypeDTO>>(await this._uow.DishTypes.GetDishTypesAsync(filter));
            return result;
        }

        public async Task<DishTypeDTO> GetDishTypeByIdAsync(int id)
        {
            return _mapper.Map<DishTypeDTO>(await this._uow.DishTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDishTypeAsync(DishTypeDTO dto)
        {
            return await this._uow.DishTypes.CreateAsync(_mapper.Map<DishType>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDishTypeAsync(DishTypeDTO dto)
        {
            return await this._uow.DishTypes.UpdateAsync(_mapper.Map<DishType>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDishTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.DishTypes.DeleteAsync(id);
            return result;
        }

        public async Task<IEnumerable<DishTypeDTO>> GetDishTypesByActiveDishCyclesAsync(int outletId, DateTime deliveryDate, DateTime deliveryDateTo)
        {
            return _mapper.Map<IEnumerable<DishTypeDTO>>(await this._uow.DishTypes.GetDishTypesByActiveDishCyclesAsync(outletId, deliveryDate, deliveryDateTo));
        }

        #endregion

        #region Dish 

        public async Task<PagedEntity<DishDTO>> GetDishesAsync(BaseFilter filter)
        {
            var datas = await this._uow.Dishes.GetDishesAsync(filter);
            var result = _mapper.Map<PagedEntity<DishDTO>>(datas);
            return result;
        }

        public async Task<List<DishSimple>> GetDishChangesAsync(DateTime updatedAfter, DateTime? updatedBefore)
        {
            var result = _mapper.Map<List<DishSimple>>(await this._uow.Dishes.GetDishChangesAsync(updatedAfter, updatedBefore));
            return result;
        }

        public async Task<string> GenerateCode(int id)
        {
            return await this._uow.Dishes.GenerateCode(id);
        }

        public async Task<DishDTO> GetDishByIdAsync(int id)
        {
            return _mapper.Map<DishDTO>(await this._uow.Dishes.GetByIdAsync(id));
        }

        public async Task<DishDTO> GetDishByCodeAsync(string code)
        {
            return _mapper.Map<DishDTO>(await this._uow.Dishes.GetByCodeAsync(code));
        }

        public async Task<BaseOperationResponse> CreateDishAsync(DishDTO dto)
        {
            dto = GetDishWithFile(dto);
            return await this._uow.Dishes.CreateAsync(_mapper.Map<Dish>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDishAsync(DishDTO dto)
        {
            dto = GetDishWithFile(dto);
            return await this._uow.Dishes.UpdateAsync(_mapper.Map<Dish>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDishAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Dishes.DeleteAsync(id);
            return result;
        }

        private DishDTO GetDishWithFile(DishDTO dto)
        {
            if (!string.IsNullOrEmpty(dto.FilePath))
            {

                try
                {
                    string source = Path.Combine(Directory.GetCurrentDirectory(), dto.FilePath);
                    string ext = Path.GetExtension(source);
                    string fname = dto.Code + ext;
                    var relativePath = Path.Combine("Resources", "Images", "Dish");
                    string destinationFolder = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
                    if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
                    var relativeFilePath = Path.Combine(relativePath, fname);
                    var destination = Path.Combine(Directory.GetCurrentDirectory(), relativeFilePath);
                    var sourceFile = new FileInfo(source);
                    sourceFile.MoveTo(destination);
                    //System.IO.File.Copy(source, destination, true);

                    dto.FilePath = relativeFilePath;
                    dto.FileName = fname;
                }
                catch (Exception ex)
                {
                    _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, "An error occurred while getting the dish file file: " + dto.FilePath);
                }
            }

            if (!string.IsNullOrEmpty(dto.ProductionPicturePath))
            {

                try
                {
                    string source = Path.Combine(Directory.GetCurrentDirectory(), dto.ProductionPicturePath);
                    string ext = Path.GetExtension(source);
                    string fname = dto.Code + "_production" + ext;
                    var relativePath = Path.Combine("Resources", "Images", "Dish");
                    string destinationFolder = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
                    if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
                    var relativeFilePath = Path.Combine(relativePath, fname);
                    var destination = Path.Combine(Directory.GetCurrentDirectory(), relativeFilePath);
                    var sourceFile = new FileInfo(source);
                    sourceFile.MoveTo(destination);
                    //System.IO.File.Copy(source, destination, true);

                    dto.ProductionPicturePath = relativeFilePath;
                    dto.ProductionPictureName = fname;
                }
                catch (Exception ex)
                {
                    _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, "An error occurred while getting the dish file prod file: " + dto.ProductionPicturePath);
                }
            }

            return dto;
        }

        public async Task<PagedEntity<DishLiteDTO>> GetDishesLiteAsync(BaseFilter filter)
        {
            return await _uow.Dishes.GetDishesLiteAsync(filter);
        }

        public async Task<List<int?>> GetDishesIdsByCycleIdAsync(int dishCycleId)
        {
            return await _uow.Dishes.GetDishesIdsByCycleIdAsync(dishCycleId);
        }

        public async Task<byte[]> GenerateDishXlsx(BaseFilter filter)
        {
            var dishesPaged = await GetDishesLiteAsync(filter);
            var dishes = dishesPaged.PagedData.ToList();
            using (var stream = new MemoryStream())
            {
                var wb = new XSSFWorkbook();
                var rowCount = 0;
                var sheet = (XSSFSheet)wb.CreateSheet("Dishes");
                var headers = new string[]
                {
                    "Code","Label", "Production Description", "Menu Description", "RPP", "Cost",
                    "Dish Type Name", "Bento Box Type Name", "Cuisine Name", "Kitchen Name",
                    "SAP Code", "Protein", "Sugar", "Total Fat", "Total Carb", "Calories",
                    "Restriction Names", "Is Enabled"
                };
                #region Headers

                var headerStyle = wb.CreateCellStyle();
                var headerFont = wb.CreateFont();
                headerFont.Boldweight = (short)FontBoldWeight.Bold;
                headerStyle.SetFont(headerFont);
                headerStyle.Alignment = HorizontalAlignment.Center;
                var row = sheet.CreateRow(rowCount); var borderedHeaderStyle = wb.CreateCellStyle();
                borderedHeaderStyle.SetFont(headerFont);
                borderedHeaderStyle.Alignment = HorizontalAlignment.Center;
                borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                ICell cell;
                for (var i = 0; i < headers.Length; i++)
                {
                    cell = row.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = borderedHeaderStyle;
                }
                sheet.AutoSizeColumn(0);

                #endregion

                #region Content
                var contentStyle = wb.CreateCellStyle();
                contentStyle.BorderTop = BorderStyle.Thin;
                contentStyle.BorderBottom = BorderStyle.Thin;
                contentStyle.BorderLeft = BorderStyle.Thin;
                contentStyle.BorderRight = BorderStyle.Thin;
                contentStyle.VerticalAlignment = VerticalAlignment.Top;
                contentStyle.Alignment = HorizontalAlignment.Left;
                contentStyle.WrapText = true;
                var dataFormatCustom = wb.CreateDataFormat();
                dishes.ForEach(dt =>
                {
                    int i = 0;
                    row = sheet.CreateRow(++rowCount);

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.Code);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.Label);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.ProductionDescription);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.ExtraNote);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.RRPrice);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.Cost);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.DishTypeName);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.BentoBoxTypeCode);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.CuisineName);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.KicthenName);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.SapCode);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.Protein);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.Sugar);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.TotalFat);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.TotalCarb);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.Calories);
                    cell.CellStyle = contentStyle;

                    var restrictionsName = string.Join(",", dt.Restrictions);
                    cell = row.CreateCell(i++);
                    cell.SetCellValue(restrictionsName);
                    cell.CellStyle = contentStyle;

                    string enableStatus = dt.IsEnabled ? "Yes" : "No";
                    cell = row.CreateCell(i++);
                    cell.SetCellValue(enableStatus);
                    cell.CellStyle = contentStyle;
                });

                #endregion

                for (var i = 0; i < headers.Length; i++)
                {
                    sheet.AutoSizeColumn(i, true);
                }

                CreateImportDishSheet(wb);
                wb.Write(stream);

                return stream.ToArray();
            }
        }

        private void CreateImportDishSheet(XSSFWorkbook workbook)
        {
            var sheet = (XSSFSheet)workbook.CreateSheet("Import Dish Template");

            var headers = new string[]
            {
                "Code","Label", "Production Description", "Menu Description", "RPP", "Cost",
                "Dish Type Name", "Bento Box Type Name", "Cuisine Name", "Kitchen Name",
                "SAP Code", "Protein", "Sugar", "Total Fat", "Total Carb", "Calories",
                "Restriction Names", "Is Enabled"
            };

            var headerStyle = workbook.CreateCellStyle();
            var headerFont = workbook.CreateFont();
            headerFont.Boldweight = (short)FontBoldWeight.Bold;
            headerStyle.SetFont(headerFont);
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.BorderTop = BorderStyle.Thin;
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.BorderRight = BorderStyle.Thin;

            var row = sheet.CreateRow(0);
            ICell cell;

            for (int i = 0; i < headers.Length; i++)
            {
                cell = row.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
                sheet.AutoSizeColumn(i, true);
            }
        }

        public async Task<DishImportDTO> DishImport(int catererInfoId, List<DishImportInputDTO> dtos, int userId)
        {
            return await _uow.Dishes.DishImport(catererInfoId, dtos, userId);
        }
        #endregion

        #region Cuisine

        public async Task<PagedEntity<CuisineDTO>> GetCuisinesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<CuisineDTO>>(await this._uow.Cuisines.GetCuisinesAsync(filter));
            return result;
        }

        public async Task<CuisineDTO> GetCuisineByIdAsync(int id)
        {
            return _mapper.Map<CuisineDTO>(await this._uow.Cuisines.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCuisineAsync(CuisineDTO dto)
        {
            return await this._uow.Cuisines.CreateAsync(_mapper.Map<Cuisine>(dto));
        }

        public async Task<BaseOperationResponse> UpdateCuisineAsync(CuisineDTO dto)
        {
            return await this._uow.Cuisines.UpdateAsync(_mapper.Map<Cuisine>(dto));
        }

        public async Task<BaseOperationResponse> DeleteCuisineAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Cuisines.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Dish Cycle

        public async Task<PagedEntity<DishCycleSimpleDTO>> GetDishCyclesSimpleAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<DishCycleSimpleDTO>>(await this._uow.DishCycles.GetDishCyclesAsync(filter));
            return result;
        }

        public async Task<PagedEntity<DishCycleDTO>> GetDishCyclesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<DishCycleDTO>>(await this._uow.DishCycles.GetDishCyclesAsync(filter));
            return result;
        }

        public async Task<DishCycleDTO> GetDishCycleByIdAsync(int id)
        {
            return _mapper.Map<DishCycleDTO>(await this._uow.DishCycles.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDishCycleAsync(DishCycleDTO dto)
        {
            return await this._uow.DishCycles.CreateAsync(_mapper.Map<DishCycle>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDishCycleAsync(DishCycleDTO dto)
        {
            return await this._uow.DishCycles.UpdateAsync(_mapper.Map<DishCycle>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDishCycleAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.DishCycles.DeleteAsync(id);
            return result;
        }

        public async Task<List<MealCreditSetDTO>> GetDishesByMealType(int outletId, int catererId, int mealTypeId, DateTime date, int? sessionId)
        {
            var result = _mapper.Map<List<MealCreditSetDTO>>(await this._uow.DishCycles.GetDishesByMealType(outletId, catererId, mealTypeId, date, sessionId));
            return result;
        }

        public async Task<List<DishCycleScheduleSetDTO>> GetDishCycleScheduleSetMenus(int cycleId, int day, int? outletId)
        {
            var result = _mapper.Map<List<DishCycleScheduleSetDTO>>(await this._uow.DishCycles.GetDishCycleScheduleSetMenus(cycleId, day, outletId));
            return result;
        }

        public async Task<List<DishCycleDTO>> GetOutletDishCyclesAsync(int outletId, int catererId)
        {
            var result = _mapper.Map<List<DishCycleDTO>>(await this._uow.DishCycles.GetOutletDishCyclesAsync(outletId, catererId));

            foreach (var cycle in result)
            {
                List<OutletDishBlockedDateDTO> outletBlocks = [];

                foreach (var block in cycle.OutletDishBlockedDates)
                {
                    if (block.OutletId == outletId)
                    {
                        outletBlocks.Add(block);
                    }
                }

                cycle.OutletDishBlockedDates = outletBlocks;
            }

            return result;
        }

        public async Task<List<DishCycleDTO>> GetOutletStudentDishCyclesAsync(int studentId, DateTime date, int sessionId)
        {
            var cycles = await this._uow.DishCycles.GetOutletStudentDishCyclesAsync(studentId, date, sessionId);
            var result = _mapper.Map<List<DishCycleDTO>>(cycles);
            return result;
        }

        public async Task<List<DishCyclePeriodDTO>> GetOutletDishCyclePeriodsAsync(int dishCyleId)
        {
            var result = _mapper.Map<List<DishCyclePeriodDTO>>(await this._uow.DishCycles.GetOutletDishCyclePeriodsAsync(dishCyleId));
            return result;
        }


        #endregion

        #region Dish Cycle Calendar

        public async Task<PagedEntity<DishCycleCalendarDTO>> GetDishCycleCalendarsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<DishCycleCalendarDTO>>(await this._uow.DishCycleCalendars.GetDishCycleCalendarsAsync(filter));
            return result;
        }

        public async Task<BaseOperationResponse> BlockDishCycleDate(List<DishCycleBlockedDateDTO> dto)
        {
            return await this._uow.DishCycleCalendars.BlockDishCycleDate(_mapper.Map<List<DishCycleBlockedDate>>(dto));
        }

        public async Task<BaseOperationResponse> UnblockDishCycleDate(List<DishCycleBlockedDateDTO> dto)
        {
            return await this._uow.DishCycleCalendars.UnblockDishCycleDate(_mapper.Map<List<DishCycleBlockedDate>>(dto));
        }
        #endregion

        #region Outlet Calendar
        public async Task<BaseOperationResponse> DishBlockOutletDate(List<OutletDishBlockedDateDTO> dto)
        {
            return await this._uow.DishCycles.DishBlockOutletDate(_mapper.Map<List<OutletDishBlockedDate>>(dto));
        }

        public async Task<BaseOperationResponse> DishUnblockOutletDate(List<OutletDishBlockedDateDTO> dto)
        {
            return await this._uow.DishCycles.DishUnblockOutletDate(_mapper.Map<List<OutletDishBlockedDate>>(dto));
        }

        public async Task<BaseOperationResponse> CreateOutletDishCyclePeriodMenus(OutletDishViewMenuDTO model)
        {
            var result = await this._uow.DishCycles.CreateOutletDishCyclePeriodMenus(_mapper.Map<OutletDishViewMenu>(model));
            return result;
        }
        #endregion

        #region Caterer Asset Type
        public async Task<PagedEntity<CatererAssetTypeDTO>> GetCatererAssetTypesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<CatererAssetTypeDTO>>(await this._uow.CatererAssetType.GetAsync(filter));
            return result;
        }

        public async Task<CatererAssetTypeDTO> GetCatererAssetTypeByIdAsync(int id)
        {
            return _mapper.Map<CatererAssetTypeDTO>(await this._uow.CatererAssetType.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCatererAssetTypeAsync(CatererAssetTypeDTO dto)
        {
            return await this._uow.CatererAssetType.CreateAsync(_mapper.Map<CatererAssetType>(dto));
        }

        public async Task<BaseOperationResponse> UpdateCatererAssetTypeAsync(CatererAssetTypeDTO dto)
        {
            return await this._uow.CatererAssetType.UpdateAsync(_mapper.Map<CatererAssetType>(dto));
        }

        public async Task<BaseOperationResponse> DeleteCatererAssetTypeAsync(int id)
        {
            var result = await this._uow.CatererAssetType.DeleteAsync(id);
            return result;
        }

        public async Task<List<DishWithMenuDTO>> GetCurrentMenu()
        {
            var result = await this._uow.Dishes.GetCurrentMenu();
            return _mapper.Map<List<DishWithMenuDTO>>(result);
        }

        public async Task<List<DishSimpleWithFile>> GetDishForDownload(List<string> serialNumbers)
        {
            var result = await this._uow.Dishes.GetDishForDownload(serialNumbers);
            return _mapper.Map<List<DishSimpleWithFile>>(result);
        }
        #endregion
    }
}
