using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;

namespace DAL.Repositories.MealOrder
{
    public class DishCycleCalendarRepository : Repository<DishCycleCalendar>, IDishCycleCalendarRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public DishCycleCalendarRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<DishCycleCalendar>> GetDishCycleCalendarsAsync(BaseFilter filter)
        {
            IQueryable<DishCycleCalendar> query = _appContext.DishCycleCalendars;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion



        public async Task<BaseOperationResponse> UnblockDishCycleDate(List<DishCycleBlockedDate> blockedDates)
        {
            var result = new BaseOperationResponse();

            var toRemove = _appContext.DishCycleBlockedDates.Where(e => blockedDates.Any(f => f.DishCycleId == e.DishCycleId && e.EffectiveDate == f.EffectiveDate));
            _appContext.DishCycleBlockedDates.RemoveRange(toRemove);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully unblocked the date!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to unblock the date!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> BlockDishCycleDate(List<DishCycleBlockedDate> blockedDates)
        {
            var result = new BaseOperationResponse();

            await _appContext.DishCycleBlockedDates.AddRangeAsync(blockedDates);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully blocked the date!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to block the date!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
