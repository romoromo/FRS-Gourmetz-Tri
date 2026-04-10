using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IMenuService
    {
        Task<BaseOperationResponse> CreateMenuAsync(MenuDTO dto);
        Task<BaseOperationResponse> DeleteMenuAsync(int id);
        Task<MenuDTO> GetMenuByIdAsync(int id);
        Task<PagedEntity<MenuDTO>> GetMenusAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateMenuAsync(MenuDTO dto);

        Task<BaseOperationResponse> CreateMenuCycleAsync(MenuCycleDTO dto);
        Task<BaseOperationResponse> DeleteMenuCycleAsync(int id);
        Task<MenuCycleDTO> GetMenuCycleByIdAsync(int id);
        Task<PagedEntity<MenuCycleDTO>> GetMenuCyclesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateMenuCycleAsync(MenuCycleDTO dto);
        Task<BaseOperationResponse> BlockMenuCycleDate(List<MenuCycleBlockedDateDTO> dto);
        Task<BaseOperationResponse> UnblockMenuCycleDate(List<MenuCycleBlockedDateDTO> dto);
        Task<List<MenuCycleDTO>> GetOutletMenuCyclesAsync(int outletId, int catererId);
        Task<BaseOperationResponse> CreateOutletSchedulePeriodMenus(List<MenuCycleSchedulePeriodDTO> model);

        Task<List<MenuCycleSchedulePeriodDTO>> GetMenuCycleSchedulePeriods(int menuCycleId, int day);
        Task<List<MenuCycleSchedulePeriodDTO>> GetOutletMenuCycleSchedulePeriods(int outletId, int catererId, int menuCycleId, int day);
        Task<BaseOperationResponse> CreateSchedulePeriodMenus(List<MenuCycleSchedulePeriodDTO> model);

        Task<BaseOperationResponse> CreateMenuCycleCalendarAsync(MenuCycleCalendarDTO dto);
        Task<BaseOperationResponse> DeleteMenuCycleCalendarAsync(int id);
        Task<MenuCycleCalendarDTO> GetMenuCycleCalendarByIdAsync(int id);
        Task<PagedEntity<MenuCycleCalendarDTO>> GetMenuCycleCalendarsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateMenuCycleCalendarAsync(MenuCycleCalendarDTO dto);

        Task<BaseOperationResponse> BlockOutletDate(List<OutletBlockedDateDTO> dto);
        Task<BaseOperationResponse> UnblockOutletDate(List<OutletBlockedDateDTO> dto);

        Task<MenuDishMealTypeDTO> GetMenuDishMealTypesByIdAsync(int id, int page, int pageSize);

        Task<List<OutletClassRosterDTO>> GetStudentMenuCyclesAsync(int studentId, int outletId, int catererId);
        Task<List<OutletClassRosterDTO>> GetStudentMenuCycles2Async(int classId, int studentId, int outletId, int catererId);
        Task<List<MealSessionDetailDTO>> GetStudentSessions(int studentId, DateTime orderDate);
        Task<List<MealSessionDetailDTO>> GetOutleOnlyMealSessions(int outletId, DateTime orderDate);
        Task<List<MealSessionDetailDTO>> GetOutletMealSessions(int outletId, DateTime orderDate, DateTime? startDate, DateTime? endDate, DateTime? orderDateTo = null);
        Task<List<MealSessionDetailDTO>> GetMealAllocationSessionDetail(int outletId, DateTime orderDate);
        Task<List<RouteDTO>> GetMealAllocationRoute(int outletId, DateTime orderDate);
        Task<BaseOperationResponse> CreateMenuGroupAsync(MenuGroupDTO dto);
        Task<BaseOperationResponse> DeleteMenuGroupAsync(int id);
        Task<MenuGroupDTO> GetMenuGroupByIdAsync(int id);
        Task<PagedEntity<MenuGroupSimpleDTO>> GetMenuGroupsSimpleAsync(BaseFilter filter);
        Task<PagedEntity<MenuGroupDTO>> GetMenuGroupsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateMenuGroupAsync(MenuGroupDTO dto);

        Task<List<DishCycleDTO>> GetMenuGroupDishCyclesAsync(int studentId, DateTime orderDate);
        Task<List<MenuGroupDTO>> GetActiveMenuGroupDishCyclesAsync(int studentId);

        Task<List<OutletDishBlockedDateDTO>> GetStudentBlockedDates(int studentId);
        Task<List<DishByDateDTO>> GetMenuGroupDishesByDateAsync(int studentId, DateTime startDate, DateTime endDate);
    }
}