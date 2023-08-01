using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IMenuCycleCalendarRepository
    {
        Task<BaseOperationResponse> CreateAsync(MenuCycleCalendar MenuCycleCalendar);
        Task<BaseOperationResponse> Delete(MenuCycleCalendar MenuCycleCalendar);
        Task<BaseOperationResponse> DeleteAsync(int MenuCycleCalendarId);
        Task<MenuCycleCalendar> GetByIdAsync(int id);
        Task<PagedEntity<MenuCycleCalendar>> GetMenuCycleCalendarsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(MenuCycleCalendar MenuCycleCalendar);
    }
}