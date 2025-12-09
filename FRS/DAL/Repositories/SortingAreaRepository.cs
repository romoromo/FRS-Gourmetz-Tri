using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class SortingAreaRepository : Repository<SortingArea>, ISortingAreaRepository
    {
        private readonly ISieveProcessor _sieveProcessor;
        public SortingAreaRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
        }
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;

        public async Task<BaseOperationResponse> CreateAsync(SortingArea model)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(model);
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

        public async Task<BaseOperationResponse> Delete(SortingArea model)
        {
            var result = new BaseOperationResponse();
            SoftDelete(model);
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

        public async Task<BaseOperationResponse> DeleteAsync(int id)
        {
            var result = new BaseOperationResponse();
            var data = await GetSingleOrDefaultAsync(r => r.Id == id);

            if (data != null)
                return await Delete(data);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<PagedEntity<SortingArea>> GetAsync(BaseFilter filter)
        {
            IQueryable<SortingArea> query = _appContext.SortingAreas
               .Include(e => e.CatererInfo)
               .Include(e => e.Route);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        public async Task<SortingArea> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> UpdateAsync(SortingArea model)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == model.Id);
            if (await Exists(e => e.Id != f.Id && e.Code == model.Code && e.IsActive))
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
            }
            else
            {
                f.CopyFrom(model);

                f.UpdatedDate = DateTime.Now;
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
            }

            return result;
        }
    }
}