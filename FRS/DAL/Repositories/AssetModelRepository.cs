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
    public class AssetModelRepository : Repository<AssetModel>, IAssetModelRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public AssetModelRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<AssetModel>> GetAssetModelsAsync(BaseFilter filter)
        {
            IQueryable<AssetModel> query = _appContext.AssetModels
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<AssetModel>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<AssetModel> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<int> GetOrCreateByCode(AssetModel data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Name)) return 0;

            var f = await GetSingleOrDefaultAsync(c => c.Name == data.Name && c.IsActive);

            if (f == null)
            {
                await CreateAsync(data);
                f = await GetSingleOrDefaultAsync(c => c.Name == data.Name && c.IsActive);
            }

            return f.Id;
        }

        public async Task<BaseOperationResponse> CreateAsync(AssetModel assetModel)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(assetModel);
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

        public async Task<BaseOperationResponse> UpdateAsync(AssetModel assetModel)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == assetModel.Id);
            if (!f.FileId.HasValue)
            {
                f.Photo = assetModel.Photo;
            }
            else
            {
                if (f.Photo == null)
                {
                    //TODO: check why EF Core is not loading the Icon property; interim solution
                    var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == f.FileId);
                    if (icon == null)
                    {
                        f.Photo = new File();
                    }
                    else
                    {
                        f.Photo = icon;
                        f.FileId = icon.Id;
                    }
                }

                f.Photo.Path = assetModel.Photo.Path;
                f.Photo.FileName = assetModel.Photo.FileName ?? System.IO.Path.GetFileName(assetModel.Photo.Path);
                f.Photo.Type = FileType.JPG.ToString();
            }

            f.CopyFrom(assetModel);
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


        public async Task<BaseOperationResponse> DeleteAsync(int assetModelId)
        {
            var result = new BaseOperationResponse();
            var assetModel = await GetSingleOrDefaultAsync(r => r.Id == assetModelId);

            if (assetModel != null)
                return await Delete(assetModel);

            result.IsSuccess = false;
            result.Message = "Asset Model not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(AssetModel assetModel)
        {
            var result = new BaseOperationResponse();
            SoftDelete(assetModel);
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
