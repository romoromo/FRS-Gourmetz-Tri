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
    public class BentoAssetRepository : Repository<BentoAsset>, IBentoAssetRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public BentoAssetRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<BentoAsset>> GetBentoAssetsAsync(BaseFilter filter)
        {
            IQueryable<BentoAsset> query = _appContext.BentoAssets
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        public async Task<PagedEntity<BentoAsset>> GetBentoAssetsAsync(BentoAssetsFilter filter)
        {
            IQueryable<BentoAsset> query = _appContext.BentoAssets.Where(m => m.BentoBoxType.CatererInfoId == filter.CatererInfoId)
                    .Include(e => e.Institution);

            if (filter.LastUpdatetime is not null)
            {
                query = _appContext.BentoAssets.Where(m => m.BentoBoxType.CatererInfoId == filter.CatererInfoId && m.LastUpdateTime >= filter.LastUpdatetime)
                    .Include(e => e.Institution);
            }

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<BentoAsset> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(BentoAsset data)
        {
            var result = new BaseOperationResponse();
            if (await Exists(e => e.Code == data.Code && e.IsActive))
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
            }
            else
            {
                data.LastUpdateTime = DateTime.Now;
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

        public async Task<BaseOperationResponse> UpdateAsync(BentoAsset data)
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
                f.CopyFrom(data);

                f.LastUpdateTime = DateTime.Now;
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

        public async Task<BaseOperationResponse> Delete(BentoAsset data)
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

        public async Task<BaseOperationResponse> ResetAsync()
        {
            var result = new BaseOperationResponse();
            if (await _appContext.Database.ExecuteSqlRawAsync("UPDATE [dbo].[BentoAssets] SET [CartonAssetId] = null,[DishId] = null,[StoreInfoId] = 1,[ToStoreInfoId] = null") > 0)
            {
                result.Message = "Successfully Update!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to update!";
                result.IsSuccess = false;
            }

            return result;

        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
