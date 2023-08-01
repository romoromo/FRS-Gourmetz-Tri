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
    public class CatererInfoRepository : Repository<CatererInfo>, ICatererInfoRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public CatererInfoRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<CatererInfo>> GetCatererInfosAsync(BaseFilter filter)
        {
            IQueryable<CatererInfo> query = _appContext.CatererInfos
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<CatererInfo>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<CatererInfo> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(CatererInfo data)
        {
            var result = new BaseOperationResponse();

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

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(CatererInfo data)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == data.Id);

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

        public async Task<BaseOperationResponse> Delete(CatererInfo data)
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

        public async Task<BaseOperationResponse> RequestOutlet(int catererId, int outletId, string status, bool isRsp, int? outletProfileId)
        {
            var result = new BaseOperationResponse();
            var f = await _appContext.CatererOutlets.FirstOrDefaultAsync(e => e.CatererInfoId == catererId && e.OutletId == outletId);

            if(f != null)
            {
                f.Status = status ?? "REQUESTED";
                f.OutletProfileId = outletProfileId;
                f.IsRsp = isRsp;

                _appContext.CatererOutlets.Update(f);
                
            }
            else
            {
                f = new CatererOutlet { OutletId = outletId, CatererInfoId = catererId, Status = status, IsRsp = isRsp, OutletProfileId = outletProfileId };
                _appContext.CatererOutlets.Add(f);
            }

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
