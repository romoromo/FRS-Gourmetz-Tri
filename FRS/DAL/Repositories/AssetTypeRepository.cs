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

namespace DAL.Repositories
{
    public class AssetTypeRepository : Repository<AssetType>, IAssetTypeRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public AssetTypeRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<AssetType>> GetAssetTypesAsync(BaseFilter filter)
        {
            IQueryable<AssetType> query = _appContext.AssetTypes
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<AssetType>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<AssetType> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(AssetType assetType)
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

        public async Task<BaseOperationResponse> UpdateAsync(AssetType assetType)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == assetType.Id);

            f.CopyFrom(assetType);

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
                        f.Icon = new File();
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


        public async Task<BaseOperationResponse> DeleteAsync(int assetTypeId)
        {
            var result = new BaseOperationResponse();
            var assetType = await GetSingleOrDefaultAsync(r => r.Id == assetTypeId);

            if (assetType != null)
                return await Delete(assetType);

            result.IsSuccess = false;
            result.Message = "Asset Type not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(AssetType assetType)
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

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
