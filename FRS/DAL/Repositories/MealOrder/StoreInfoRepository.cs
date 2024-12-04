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
    public class StoreInfoRepository : Repository<StoreInfo>, IStoreInfoRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public StoreInfoRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<StoreInfo>> GetStoreInfosAsync(BaseFilter filter)
        {
            IQueryable<StoreInfo> query = _appContext.StoreInfos
                .Include(e => e.Institution);

            if (filter.Filters != "")
            {
                filter.Filters += ",(IsActive)==true";
            } else
            {
                filter.Filters += "(IsActive)==true";
            }

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<StoreInfo>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<StoreInfo> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(StoreInfo data)
        {
            var result = new BaseOperationResponse();
            if (await Exists(e => e.Code == data.Code && e.IsActive))
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
            }
            else
            {

                var f = await AddAsync(data);
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

        public async Task<BaseOperationResponse> UpdateAsync(StoreInfo data)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == data.Id);

            if (await Exists(e => e.Id != f.Id && e.Code == data.Code && e.IsActive))
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
            }
            else
            {
                if(!string.IsNullOrEmpty(data.StoreType) && data.StoreType.Equals("Kitchen", StringComparison.OrdinalIgnoreCase))
                {
                    // check outlet cut off
                    var days = _appContext.CatererOutlets.Where(e => e.CatererInfoId == data.CatererInfoId).Select(e => e.Outlet.DaysToFreezeOrdering).ToList();
                    int.TryParse(data.CalendarDays, out int cd);
                    if (days.Any(e => e > 0 && e < cd))
                    {
                        result.Message = "Calendar days must be equal or lesser than the cut-off days of the outlets.";
                        result.IsSuccess = false;
                        return result;
                    }
                }

                f.CopyFrom(data);

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


        public async Task<BaseOperationResponse> DeleteAsync(int dataId)
        {
            var result = new BaseOperationResponse();
            var data = await GetSingleOrDefaultAsync(r => r.Id == dataId);

            if (data != null)
                return await Delete(data);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(StoreInfo data)
        {
            var result = new BaseOperationResponse();
            SoftDelete(data);
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
