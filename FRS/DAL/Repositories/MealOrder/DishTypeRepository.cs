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
    public class DishTypeRepository : Repository<DishType>, IDishTypeRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public DishTypeRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<DishType>> GetDishTypesAsync(BaseFilter filter)
        {
            IQueryable<DishType> query = _appContext.DishTypes
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<DishType>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<DishType> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(DishType dishType)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(dishType);
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

        public async Task<BaseOperationResponse> UpdateAsync(DishType dishType)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == dishType.Id);

            //delete old dish type periods
            var selectedPeriodIds = dishType.DishTypePeriods.Select(a => a.PeriodId);
            var periods = this._appContext.DishTypePeriods.Where(e => e.DishTypeId == dishType.Id);

            var toBeDeleted = periods.Where(e => !selectedPeriodIds.Contains(e.PeriodId));
            this._appContext.DishTypePeriods.RemoveRange(toBeDeleted);

            var toBeAdded = selectedPeriodIds.Except(periods.Select(e => e.PeriodId));

            toBeAdded.ToList().ForEach(e => {
                this._appContext.DishTypePeriods.AddAsync(new DishTypePeriod { DishTypeId = dishType.Id, PeriodId = e });
            });

            f.CopyFrom(dishType);

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


        public async Task<BaseOperationResponse> DeleteAsync(int dishTypeId)
        {
            var result = new BaseOperationResponse();
            var dishType = await GetSingleOrDefaultAsync(r => r.Id == dishTypeId);

            if (dishType != null)
                return await Delete(dishType);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(DishType dishType)
        {
            var result = new BaseOperationResponse();
            SoftDelete(dishType);
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

        public async Task<IEnumerable<DishType>> GetDishTypesByActiveDishCyclesAsync(int outletId, DateTime deliveryDate, DateTime deliveryDateTo)
        {
            var activeDishCycles = _appContext.DishCycles.Where(e => e.IsActive &&
                    e.OutletProfile.Caterer.CatererOutlets.Any(o => o.IsActive && o.OutletId == outletId) &&
                    (deliveryDate.Date >= e.StartDate.Date &&
                    (!e.EndDate.HasValue || e.EndDate.Value.Date >= deliveryDate.Date)));

            return activeDishCycles.Select(e => e.DishType).Where(e => e != null).Distinct();
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
