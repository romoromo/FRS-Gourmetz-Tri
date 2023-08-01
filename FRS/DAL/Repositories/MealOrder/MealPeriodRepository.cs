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
    public class MealPeriodRepository : Repository<MealPeriod>, IMealPeriodRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public MealPeriodRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<MealPeriod>> GetMealPeriodsAsync(BaseFilter filter)
        {
            IQueryable<MealPeriod> query = _appContext.MealPeriods
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<MealPeriod>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<MealPeriod> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<List<MealPeriod>> GetByOutletIdAsync(int outletProfileId)
        {
            return (await FindAsync(e => e.IsActive && e.OutletProfile.Id == outletProfileId)).OrderBy(e => e.Sequence).ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(MealPeriod mealPeriod)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(mealPeriod);
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

        public async Task<BaseOperationResponse> UpdateAsync(MealPeriod mealPeriod)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == mealPeriod.Id);

            f.CopyFrom(mealPeriod);

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


        public async Task<BaseOperationResponse> DeleteAsync(int mealPeriodId)
        {
            var result = new BaseOperationResponse();
            var mealPeriod = await GetSingleOrDefaultAsync(r => r.Id == mealPeriodId);

            if (mealPeriod != null)
                return await Delete(mealPeriod);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(MealPeriod mealPeriod)
        {
            var result = new BaseOperationResponse();
            SoftDelete(mealPeriod);
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
