using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.MealOrder
{
    public class CatererAssetTypeRepository : Repository<CatererAssetType>, ICatererAssetTypeRepository
    {
        private readonly ISieveProcessor _sieveProcessor;
        public CatererAssetTypeRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
        }
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
        
        public async Task<PagedEntity<CatererAssetType>> GetAsync(BaseFilter filter)
        {
            IQueryable<CatererAssetType> query = _appContext.CatererAssetTypes;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            return result;
        }

        public async Task<CatererAssetType> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(CatererAssetType assetType)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(assetType);
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

        public async Task<BaseOperationResponse> UpdateAsync(CatererAssetType assetType)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == assetType.Id);

            f.CopyFrom(assetType);

            if (assetType.Icon != null && !string.IsNullOrEmpty(assetType.Icon.Path))
            {
                if (!f.FileId.HasValue)
                {
                    f.Icon = assetType.Icon;
                }
                else
                {
                    if (f.Icon == null)
                    {
                        //TODO: check why EF Core is not loading the Icon property; interim solution
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

                    f.Icon.Path = assetType.Icon.Path;
                    f.Icon.FileName = assetType.Icon.FileName ?? System.IO.Path.GetFileName(assetType.Icon.Path);
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

        public async Task<BaseOperationResponse> Delete(CatererAssetType assetType)
        {
            var result = new BaseOperationResponse();
            SoftDelete(assetType);
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

        public async Task<BaseOperationResponse> DeleteAsync(int assetTypeId)
        {
            var result = new BaseOperationResponse();
            var cuisine = await GetSingleOrDefaultAsync(r => r.Id == assetTypeId);

            if (cuisine != null)
                return await Delete(cuisine);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }        
    }
}
