using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IDishCycleCalendarRepository
    {
        Task<PagedEntity<DishCycleCalendar>> GetDishCycleCalendarsAsync(BaseFilter filter);

        Task<BaseOperationResponse> UnblockDishCycleDate(List<DishCycleBlockedDate> blockedDates);

        Task<BaseOperationResponse> BlockDishCycleDate(List<DishCycleBlockedDate> blockedDates);
    }
}