using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL;
using BAL.Services.Interfaces;
using BAL.DTO;
using AutoMapper;
using DAL.Repositories.Interfaces;
using System.IO;
using NPOI.HSSF.UserModel;
using BAL.Services.Interfaces.MealOrder;
using BAL.DTO.MealOrder;
using DAL.Models.MealOrder;
using DAL.Core.Interfaces;
using DAL.Core.Logging;
using Microsoft.Extensions.Logging;

namespace BAL.Services.MealOrder
{
    public class DishService : IDishService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private IAccountManager _accountManager;
        private ILogger _logger;

        public DishService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
            _logger = Logger.CreateLogger<DishService>();
        }

        #region Dish Type

        public async Task<PagedEntity<DishTypeDTO>> GetDishTypesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<DishTypeDTO>>(await this._uow.DishTypes.GetDishTypesAsync(filter));
            return result;
        }

        public async Task<DishTypeDTO> GetDishTypeByIdAsync(int id)
        {
            return Mapper.Map<DishTypeDTO>(await this._uow.DishTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDishTypeAsync(DishTypeDTO dto)
        {
            return await this._uow.DishTypes.CreateAsync(Mapper.Map<DishType>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDishTypeAsync(DishTypeDTO dto)
        {
            return await this._uow.DishTypes.UpdateAsync(Mapper.Map<DishType>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDishTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.DishTypes.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Dish 

        public async Task<PagedEntity<DishDTO>> GetDishesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<DishDTO>>(await this._uow.Dishes.GetDishesAsync(filter));
            return result;
        }

        public async Task<List<DishSimple>> GetDishChangesAsync(DateTime updatedAfter, DateTime? updatedBefore)
        {
            var result = Mapper.Map<List<DishSimple>>(await this._uow.Dishes.GetDishChangesAsync(updatedAfter, updatedBefore));
            return result;
        }

        public async Task<string> GenerateCode(int id)
        {
            return await this._uow.Dishes.GenerateCode(id);
        }

        public async Task<DishDTO> GetDishByIdAsync(int id)
        {
            return Mapper.Map<DishDTO>(await this._uow.Dishes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDishAsync(DishDTO dto)
        {
            dto = GetDishWithFile(dto);
            return await this._uow.Dishes.CreateAsync(Mapper.Map<Dish>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDishAsync(DishDTO dto)
        {
            dto = GetDishWithFile(dto);
            return await this._uow.Dishes.UpdateAsync(Mapper.Map<Dish>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDishAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Dishes.DeleteAsync(id);
            return result;
        }

        private DishDTO GetDishWithFile(DishDTO dto)
        {
            if(!string.IsNullOrEmpty(dto.FilePath))
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
        #endregion

        #region Cuisine

        public async Task<PagedEntity<CuisineDTO>> GetCuisinesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<CuisineDTO>>(await this._uow.Cuisines.GetCuisinesAsync(filter));
            return result;
        }

        public async Task<CuisineDTO> GetCuisineByIdAsync(int id)
        {
            return Mapper.Map<CuisineDTO>(await this._uow.Cuisines.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCuisineAsync(CuisineDTO dto)
        {
            return await this._uow.Cuisines.CreateAsync(Mapper.Map<Cuisine>(dto));
        }

        public async Task<BaseOperationResponse> UpdateCuisineAsync(CuisineDTO dto)
        {
            return await this._uow.Cuisines.UpdateAsync(Mapper.Map<Cuisine>(dto));
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
            var result = Mapper.Map<PagedEntity<DishCycleSimpleDTO>>(await this._uow.DishCycles.GetDishCyclesAsync(filter));
            return result;
        }

        public async Task<PagedEntity<DishCycleDTO>> GetDishCyclesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<DishCycleDTO>>(await this._uow.DishCycles.GetDishCyclesAsync(filter));
            return result;
        }

        public async Task<DishCycleDTO> GetDishCycleByIdAsync(int id)
        {
            return Mapper.Map<DishCycleDTO>(await this._uow.DishCycles.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDishCycleAsync(DishCycleDTO dto)
        {
            return await this._uow.DishCycles.CreateAsync(Mapper.Map<DishCycle>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDishCycleAsync(DishCycleDTO dto)
        {
            return await this._uow.DishCycles.UpdateAsync(Mapper.Map<DishCycle>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDishCycleAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.DishCycles.DeleteAsync(id);
            return result;
        }

        public async Task<List<MealCreditSetDTO>> GetDishesByMealType(int outletId, int catererId, int mealTypeId, DateTime date, int? sessionId)
        {
            var result = Mapper.Map<List<MealCreditSetDTO>>(await this._uow.DishCycles.GetDishesByMealType(outletId, catererId, mealTypeId, date, sessionId));
            return result;
        }

        public async Task<List<DishCycleScheduleSetDTO>> GetDishCycleScheduleSetMenus(int cycleId, int day, int? outletId)
        {
            var result = Mapper.Map<List<DishCycleScheduleSetDTO>>(await this._uow.DishCycles.GetDishCycleScheduleSetMenus(cycleId, day, outletId));
            return result;
        }

        public async Task<List<DishCycleDTO>> GetOutletDishCyclesAsync(int outletId, int catererId)
        {
            var result = Mapper.Map<List<DishCycleDTO>>(await this._uow.DishCycles.GetOutletDishCyclesAsync(outletId, catererId));
            return result;
        }

        public async Task<List<DishCycleDTO>> GetOutletDishCyclesAsync(int studentId, DateTime date, int sessionId)
        {
            var cycles = await this._uow.DishCycles.GetOutletDishCyclesAsync(studentId, date, sessionId);
            var result = Mapper.Map<List<DishCycleDTO>>(cycles);
            return result;
        }

        public async Task<List<DishCyclePeriodDTO>> GetOutletDishCyclePeriodsAsync(int dishCyleId)
        {
            var result = Mapper.Map<List<DishCyclePeriodDTO>>(await this._uow.DishCycles.GetOutletDishCyclePeriodsAsync(dishCyleId));
            return result;
        }

        
        #endregion

        #region Dish Cycle Calendar

        public async Task<PagedEntity<DishCycleCalendarDTO>> GetDishCycleCalendarsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<DishCycleCalendarDTO>>(await this._uow.DishCycleCalendars.GetDishCycleCalendarsAsync(filter));
            return result;
        }

        public async Task<BaseOperationResponse> BlockDishCycleDate(List<DishCycleBlockedDateDTO> dto)
        {
            return await this._uow.DishCycleCalendars.BlockDishCycleDate(Mapper.Map<List<DishCycleBlockedDate>>(dto));
        }

        public async Task<BaseOperationResponse> UnblockDishCycleDate(List<DishCycleBlockedDateDTO> dto)
        {
            return await this._uow.DishCycleCalendars.UnblockDishCycleDate(Mapper.Map<List<DishCycleBlockedDate>>(dto));
        }
        #endregion

        #region Outlet Calendar
        public async Task<BaseOperationResponse> DishBlockOutletDate(List<OutletDishBlockedDateDTO> dto)
        {
            return await this._uow.DishCycles.DishBlockOutletDate(Mapper.Map<List<OutletDishBlockedDate>>(dto));
        }

        public async Task<BaseOperationResponse> DishUnblockOutletDate(List<OutletDishBlockedDateDTO> dto)
        {
            return await this._uow.DishCycles.DishUnblockOutletDate(Mapper.Map<List<OutletDishBlockedDate>>(dto));
        }

        public async Task<BaseOperationResponse> CreateOutletDishCyclePeriodMenus(OutletDishViewMenuDTO model)
        {
            var result = await this._uow.DishCycles.CreateOutletDishCyclePeriodMenus(Mapper.Map<OutletDishViewMenu>(model));
            return result;
        }
        #endregion
    }
}
