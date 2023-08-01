using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IDishService
    {
        Task<BaseOperationResponse> CreateDishTypeAsync(DishTypeDTO dto);
        Task<BaseOperationResponse> DeleteDishTypeAsync(int id);
        Task<DishTypeDTO> GetDishTypeByIdAsync(int id);
        Task<PagedEntity<DishTypeDTO>> GetDishTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateDishTypeAsync(DishTypeDTO dto);

        Task<BaseOperationResponse> CreateDishAsync(DishDTO dto);
        Task<BaseOperationResponse> DeleteDishAsync(int id);
        Task<DishDTO> GetDishByIdAsync(int id);
        Task<PagedEntity<DishDTO>> GetDishesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateDishAsync(DishDTO dto);
        Task<List<DishSimple>> GetDishChangesAsync(DateTime updatedAfter, DateTime? updatedBefore);
        Task<string> GenerateCode(int id);
        Task<List<MealCreditSetDTO>> GetDishesByMealType(int outletId, int catererId, int mealTypeId, DateTime date, int? sessionId);

        Task<BaseOperationResponse> CreateCuisineAsync(CuisineDTO dto);
        Task<BaseOperationResponse> DeleteCuisineAsync(int id);
        Task<CuisineDTO> GetCuisineByIdAsync(int id);
        Task<PagedEntity<CuisineDTO>> GetCuisinesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateCuisineAsync(CuisineDTO dto);

        Task<BaseOperationResponse> CreateDishCycleAsync(DishCycleDTO dto);
        Task<BaseOperationResponse> DeleteDishCycleAsync(int id);
        Task<DishCycleDTO> GetDishCycleByIdAsync(int id);
        Task<PagedEntity<DishCycleDTO>> GetDishCyclesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateDishCycleAsync(DishCycleDTO dto);

        Task<PagedEntity<DishCycleCalendarDTO>> GetDishCycleCalendarsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UnblockDishCycleDate(List<DishCycleBlockedDateDTO> dto);
        Task<BaseOperationResponse> BlockDishCycleDate(List<DishCycleBlockedDateDTO> dto);

        Task<List<DishCycleScheduleSetDTO>> GetDishCycleScheduleSetMenus(int cycleId, int day, int? outletId);

        Task<List<DishCycleDTO>> GetOutletDishCyclesAsync(int outletId, int catererId);
        Task<BaseOperationResponse> DishBlockOutletDate(List<OutletDishBlockedDateDTO> dto);
        Task<BaseOperationResponse> DishUnblockOutletDate(List<OutletDishBlockedDateDTO> dto);

        Task<BaseOperationResponse> CreateOutletDishCyclePeriodMenus(OutletDishViewMenuDTO model);
        Task<List<DishCyclePeriodDTO>> GetOutletDishCyclePeriodsAsync(int dishCyleId);
    }
}