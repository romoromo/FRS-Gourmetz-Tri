using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IOutletClassRosterRepository
    {
        Task<BaseOperationResponse> CreateAsync(OutletClassRoster outletClassRoster);
        Task<BaseOperationResponse> CreateSchedulePeriodMenus(List<OutletClassRosterSchedulePeriod> periods);
        Task<BaseOperationResponse> Delete(OutletClassRoster outletClassRoster);
        Task<BaseOperationResponse> DeleteAsync(int outletClassRosterId);
        Task<OutletClassRoster> GetByIdAsync(int id);
        Task<List<OutletClassRoster>> GetByCatererOutletIdAsync(int outletId, int catererId, int mealSessionId);
        Task<PagedEntity<OutletClassRoster>> GetOutletClassRostersAsync(ClassRosterFilter filter);
        Task<List<OutletClassRosterSchedulePeriod>> GetOutletClassRosterSchedulePeriods(int outletClassRosterId, int day);
        Task<List<OutletClassRoster>> GetOutletOutletClassRostersAsync(int outletId, int catererId);
        Task<BaseOperationResponse> UpdateAsync(OutletClassRoster outletClassRoster);
        Task<MealSessionDetail> GetCurrentOrderMealSessionAsync(int? outletId, DateTime orderDate, int mealSessionId, int classId);
    }
}