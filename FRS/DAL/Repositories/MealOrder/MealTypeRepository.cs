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
    public class MealTypeRepository : Repository<MealType>, IMealTypeRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public MealTypeRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<MealType>> GetMealTypesAsync(BaseFilter filter)
        {
            IQueryable<MealType> query = _appContext.MealTypes
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<MealType>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<MealType> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(MealType mealType)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(mealType);
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

        public async Task<BaseOperationResponse> UpdateAsync(MealType mealType)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == mealType.Id);

            //delete old dishes
            var selectedDishIds = mealType.Dishes.Select(a => a.DishId);
            var dishes = this._appContext.MealTypeDishes.Where(e => e.MealTypeId == mealType.Id);

            var toBeDeleted = dishes.Where(e => !selectedDishIds.Contains(e.DishId));
            this._appContext.MealTypeDishes.RemoveRange(toBeDeleted);

            var toBeAdded = selectedDishIds.Except(dishes.Select(e => e.DishId));

            toBeAdded.ToList().ForEach(e => {
                this._appContext.MealTypeDishes.AddAsync(new MealTypeDish { MealTypeId = mealType.Id, DishId = e });
            });

            f.CopyFrom(mealType);

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


        public async Task<BaseOperationResponse> DeleteAsync(int mealTypeId)
        {
            var result = new BaseOperationResponse();
            var mealType = await GetSingleOrDefaultAsync(r => r.Id == mealTypeId);

            if (mealType != null)
                return await Delete(mealType);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(MealType mealType)
        {
            var result = new BaseOperationResponse();
            SoftDelete(mealType);
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
