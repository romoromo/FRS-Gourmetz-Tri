using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IDishCycleRepository
    {
        Task<BaseOperationResponse> CreateAsync(DishCycle dishCycleModel);
        Task<BaseOperationResponse> Delete(DishCycle dishCycleModel);
        Task<BaseOperationResponse> DeleteAsync(int dishCycleId);
        Task<DishCycle> GetByIdAsync(int id);
        Task<PagedEntity<DishCycle>> GetDishCyclesAsync(BaseFilter filter);
        Task<List<DishCycleScheduleSetMenuDTO>> GetDishCycleScheduleSetMenus(int cycleId, int day, int? outletId);
        Task<BaseOperationResponse> UpdateAsync(DishCycle dishCycleModel);
        Task<List<DishCycle>> GetOutletDishCyclesAsync(int outletId, int catererId);
        Task<BaseOperationResponse> DishUnblockOutletDate(List<OutletDishBlockedDate> blockedDates);
        Task<BaseOperationResponse> DishBlockOutletDate(List<OutletDishBlockedDate> blockedDates);
        Task<BaseOperationResponse> CreateOutletDishCyclePeriodMenus(OutletDishViewMenu models);
        Task<List<DishCyclePeriod>> GetOutletDishCyclePeriodsAsync(int dishCyleId);
        Task<List<MealCreditSetMenuDTO>> GetDishesByMealType(int outletId, int catererId, int mealTypeId, DateTime date, int? sessionId);
        Task<List<DishCycle>> GetOutletStudentDishCyclesAsync(int studentId, DateTime date, int sessionId);

    }
}