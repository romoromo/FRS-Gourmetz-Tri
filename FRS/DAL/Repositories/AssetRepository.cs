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
    public class AssetRepository : Repository<Asset>, IAssetRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public AssetRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Asset>> GetAssetsAsync(BaseFilter filter)
        {
            IQueryable<Asset> query = _appContext.Assets;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<Asset> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<Asset> GetByCodeAsync(string serialNumber)
        {
            return await _appContext.Assets.FirstOrDefaultAsync(a => a.IsActive && a.SerialNumber == serialNumber);
        }

        public async Task<BaseOperationResponse> CreateAsync(Asset asset)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.SerialNumber == asset.SerialNumber && e.IsActive))
            {
                result.Message = "Serial Number Name already exists!";
                result.IsSuccess = false;
            }
            else
            {

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

            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Asset asset)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == asset.Id);

            if (await Exists(e => e.Id != f.Id && e.SerialNumber == asset.SerialNumber && e.IsActive))
            {
                result.Message = "Serial Number Name already exists!";
                result.IsSuccess = false;
            }
            else
            {

                f.CopyFrom(asset);

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


        public async Task<BaseOperationResponse> DeleteAsync(int assetId)
        {
            var result = new BaseOperationResponse();
            var asset = await GetSingleOrDefaultAsync(r => r.Id == assetId);

            if (asset != null)
                return await Delete(asset);

            result.IsSuccess = false;
            result.Message = "Asset not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Asset asset)
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

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
