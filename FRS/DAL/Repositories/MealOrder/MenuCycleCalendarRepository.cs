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
    public class MenuCycleCalendarRepository : Repository<MenuCycleCalendar>, IMenuCycleCalendarRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public MenuCycleCalendarRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<MenuCycleCalendar>> GetMenuCycleCalendarsAsync(BaseFilter filter)
        {
            IQueryable<MenuCycleCalendar> query = _appContext.MenuCycleCalendars;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<MenuCycleCalendar> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(MenuCycleCalendar menuCycleCalendar)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(menuCycleCalendar);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(MenuCycleCalendar menuCycleCalendar)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == menuCycleCalendar.Id);

            //remove menus

            //remove blocked dates
            var blockedDates = this._appContext.MenuCycleCalendarBlockedDates.Where(e => e.MenuCycleCalendarId == menuCycleCalendar.Id);

            this._appContext.MenuCycleCalendarBlockedDates.RemoveRange(blockedDates);

            //add new blocked dates
            await this._appContext.MenuCycleCalendarBlockedDates.AddRangeAsync(menuCycleCalendar.BlockedDates);

            f.CopyFrom(menuCycleCalendar);

            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int menuCycleCalendarId)
        {
            var result = new BaseOperationResponse();
            var menuCycleCalendar = await GetSingleOrDefaultAsync(r => r.Id == menuCycleCalendarId);

            if (menuCycleCalendar != null)
                return await Delete(menuCycleCalendar);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(MenuCycleCalendar menuCycleCalendar)
        {
            var result = new BaseOperationResponse();
            SoftDelete(menuCycleCalendar);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
