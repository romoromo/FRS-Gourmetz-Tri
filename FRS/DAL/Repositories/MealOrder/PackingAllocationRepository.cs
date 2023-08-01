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
using System.Transactions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DAL.Core.Interfaces;
using DAL.Core.DTO;
using System.ComponentModel.DataAnnotations;
using DAL.Core.Helpers;

namespace DAL.Repositories.MealOrder
{
    public class PackingAllocationRepository : Repository<PackingAllocation>, IPackingAllocationRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;

        public PackingAllocationRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<PackingAllocation>> GetPackingAllocationsAsync(BaseFilter filter)
        {
            IQueryable<PackingAllocation> query = _appContext.PackingAllocations.Include(e => e.Dishes);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<PackingAllocation> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(PackingAllocation allocation, List<DishAllocation> dishes)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {

                var check = await GetSingleOrDefaultAsync(e => e.PackingDate == allocation.PackingDate && e.RouteId == allocation.RouteId && e.IsActive == true);

                if (check != null)
                {
                    await DeleteAsync(check.Id);
                }

                var f = await AddAsync(allocation);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                    scope.Complete();
                }
                else
                {
                    result.Message = "Failed to save!";

                }
            }
            return result;

        }

        public async Task<BaseOperationResponse> UpdateAsync(PackingAllocation allocation, List<DishAllocation> dishes)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == allocation.Id);

                var dishesToDelete = this._appContext.DishAllocations.Where(x => x.PackingId == f.Id);

                this._appContext.DishAllocations.RemoveRange(dishesToDelete);

                if (allocation != null)
                {
                    dishes.ForEach(e =>
                    {
                        if (e.DishId != null && e.DishId > 0)
                        {

                            var sc = this._appContext.DishAllocations.FirstOrDefault(x => x.Id == e.Id);
                            if (sc != null)
                            {
                                sc.DishId = e.DishId;
                                sc.Qty = e.Qty;
                                this._appContext.DishAllocations.Update(sc);
                            }
                            else
                            {
                                this._appContext.DishAllocations.Add(e);
                            }
                        }
                    });
                }

                f.CopyFrom(allocation);

                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;

                    scope.Complete();
                }
                else
                {
                    result.Message = "Failed to save!";
                    result.IsSuccess = false;
                }

            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int allocationId)
        {
            var result = new BaseOperationResponse();
            var allocation = await GetSingleOrDefaultAsync(r => r.Id == allocationId);

            if (allocation != null)
                return await Delete(allocation);

            result.IsSuccess = false;
            result.Message = "Allocation not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(PackingAllocation allocation)
        {
            var result = new BaseOperationResponse();
            SoftDelete(allocation);
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
