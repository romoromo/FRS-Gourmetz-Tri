using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.MealOrder
{
    public class CatererAssetRepository : Repository<CatererAsset>, ICatererAssetRepository
    {
        private readonly ISieveProcessor _sieveProcessor;
        public CatererAssetRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
        }
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;

        public async Task<PagedEntity<CatererAsset>> GetAsync(CatererAsserFilter filter)
        {
            IQueryable<CatererAsset> query = _appContext.CatererAssets.Where(m => m.CatererAssetType.CatererInfoId == filter.catererInfoId)
                .Include(m => m.CatererAssetType);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            return result;
        }

        public async Task<CatererAsset> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(CatererAsset asset)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(asset);
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

        public async Task<BaseOperationResponse> UpdateAsync(CatererAsset asset)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == asset.Id);

            f.CopyFrom(asset);

            if (asset.Icon != null && !string.IsNullOrEmpty(asset.Icon.Path))
            {
                if (!f.FileId.HasValue)
                {
                    f.Icon = asset.Icon;
                }
                else
                {
                    if (f.Icon == null)
                    {
                        var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == f.FileId);
                        if (icon == null)
                        {
                            f.Icon = new Models.File();
                        }
                        else
                        {
                            f.Icon = icon;
                            f.FileId = icon.Id;
                        }
                    }

                    f.Icon.Path = asset.Icon.Path;
                    f.Icon.FileName = asset.Icon.FileName ?? System.IO.Path.GetFileName(asset.Icon.Path);
                    f.Icon.Type = FileType.Icon.ToString();
                }
            }

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

        public async Task<BaseOperationResponse> Delete(CatererAsset asset)
        {
            var result = new BaseOperationResponse();
            SoftDelete(asset);
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

        public async Task<BaseOperationResponse> DeleteAsync(int assetId)
        {
            var result = new BaseOperationResponse();
            var cuisine = await GetSingleOrDefaultAsync(r => r.Id == assetId);

            if (cuisine != null)
                return await Delete(cuisine);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }
    }
}
