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
    public class MealPlanOrderRepository : Repository<MealPlanOrder>, IMealPlanOrderRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public MealPlanOrderRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<MealPlanOrder>> GetMealPlanOrdersAsync(BaseFilter filter)
        {
            IQueryable<MealPlanOrder> query = _appContext.MealPlanOrders;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<MealPlanOrder>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<MealPlanOrder> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(MealPlanOrder mealPlanOrder)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(mealPlanOrder);
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

        public async Task<BaseOperationResponse> UpdateAsync(MealPlanOrder mealPlanOrder)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == mealPlanOrder.Id);

            f.CopyFrom(mealPlanOrder);

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


        public async Task<BaseOperationResponse> DeleteAsync(int mealPlanOrderId)
        {
            var result = new BaseOperationResponse();
            var mealPlanOrder = await GetSingleOrDefaultAsync(r => r.Id == mealPlanOrderId);

            if (mealPlanOrder != null)
                return await Delete(mealPlanOrder);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(MealPlanOrder mealPlanOrder)
        {
            var result = new BaseOperationResponse();
            SoftDelete(mealPlanOrder);
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
