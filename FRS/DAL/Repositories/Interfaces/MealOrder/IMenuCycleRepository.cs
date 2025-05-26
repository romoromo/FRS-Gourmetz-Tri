using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IMenuCycleRepository
    {
        Task<BaseOperationResponse> CreateAsync(MenuCycle MenuCycle);
        Task<BaseOperationResponse> Delete(MenuCycle MenuCycle);
        Task<BaseOperationResponse> DeleteAsync(int MenuCycleId);
        Task<MenuCycle> GetByIdAsync(int id);
        Task<PagedEntity<MenuCycle>> GetMenuCyclesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(MenuCycle MenuCycle);
        Task<List<MenuCycle>> GetOutletMenuCyclesAsync(int outletId, int catererId);

        Task<BaseOperationResponse> BlockMenuCycleDate(List<MenuCycleBlockedDate> blockedDates);
        Task<BaseOperationResponse> UnblockMenuCycleDate(List<MenuCycleBlockedDate> blockedDates);
        Task<List<MenuCycleSchedulePeriod>> GetMenuCycleSchedulePeriods(int menuCycleId, int day);
        Task<BaseOperationResponse> CreateSchedulePeriodMenus(List<MenuCycleSchedulePeriod> periods);

        Task<BaseOperationResponse> UnblockOutletDate(List<OutletBlockedDate> blockedDates);
        Task<BaseOperationResponse> BlockOutletDate(List<OutletBlockedDate> blockedDates);
        Task<BaseOperationResponse> CreateOutletSchedulePeriodMenus(List<MenuCycleSchedulePeriod> periods);
        Task<List<MenuCycleSchedulePeriod>> GetOutletMenuCycleSchedulePeriods(int outletId, int catererId, int menuCycleId, int day);

        Task<List<OutletClassRosterSchedule>> GetStudentMenuCyclesAsync(int studentId);

        Task<List<OutletClassRosterSchedule>> GetStudentMenuCycles2Async(int studentId);
        Task<Student> GetStudentAsync(int studentId);
        Task<List<Student>> GetStudentsByOutletAsync(int outletId);
        Task<List<OutletClassRosterSchedule>> GetStudentsMenuCyclesAsync(List<int> studentIds);
    }
}