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
    public class StoreInventoryRepository : Repository<StoreInventory>, IStoreInventoryRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public StoreInventoryRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<StoreInventory>> GetStoreInventoriesAsync(BaseFilter filter)
        {
            IQueryable<StoreInventory> query = _appContext.StoreInventories;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<StoreInventory> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(StoreInventory data)
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

        public async Task<BaseOperationResponse> UpdateAsync(StoreInventory data)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == data.Id);


            f.CopyFrom(data);

            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                foreach (var detail in data.StoreInventoryDetails)
                {
                    if (detail.Id > 0 || detail.IsActive)
                    {
                        var ddetail = _appContext.StoreInventoryDetails.FirstOrDefault(c => detail.Id == c.Id) ?? new StoreInventoryDetail();
                        ddetail.CopyFrom(detail);
                        if (ddetail.StoreInventoryId == null) ddetail.StoreInventoryId = f.Id;
                        ddetail.IsActive = detail.IsActive;
                        _appContext.StoreInventoryDetails.Update(ddetail);
                        _appContext.SaveChanges();
                    }


                }


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

        public async Task<BaseOperationResponse> Delete(StoreInventory data)
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

        public async Task<PagedEntity<StoreInventoryDetail>> GetStoreInventoryDetailsAsync(BaseFilter filter)
        {
            IQueryable<StoreInventoryDetail> query = _appContext.StoreInventoryDetails;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
