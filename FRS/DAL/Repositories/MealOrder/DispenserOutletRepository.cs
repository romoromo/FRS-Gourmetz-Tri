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
    public class DispenserOutletRepository : Repository<DispenserOutlet>, IDispenserOutletRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public DispenserOutletRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<DispenserOutlet>> GetDispenserOutletsAsync(BaseFilter filter)
        {
            IQueryable<DispenserOutlet> query = _appContext.DispenserOutlets
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<ClassLevel>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<DispenserOutlet> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(DispenserOutlet dispenserOutlet)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.OutletId == dispenserOutlet.OutletId && e.DispenserCode == dispenserOutlet.DispenserCode && e.IsActive))
            {
                result.Message = "Dispenser already exists!";
                result.IsSuccess = false;
            }
            else
            {

                var f = await AddAsync(dispenserOutlet);
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

        public async Task<BaseOperationResponse> UpdateAsync(DispenserOutlet dispenserOutlet)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.OutletId == dispenserOutlet.OutletId && e.DispenserCode == dispenserOutlet.DispenserCode && e.IsActive && e.Id != dispenserOutlet.Id))
            {
                result.Message = "Dispenser already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == dispenserOutlet.Id);

                f.CopyFrom(dispenserOutlet);

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


        public async Task<BaseOperationResponse> DeleteAsync(int dispenserOutletId)
        {
            var result = new BaseOperationResponse();
            var dispenserOutlet = await GetSingleOrDefaultAsync(r => r.Id == dispenserOutletId);

            if (dispenserOutlet != null)
                return await Delete(dispenserOutlet);

            result.IsSuccess = false;
            result.Message = "Class Level not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(DispenserOutlet dispenserOutlet)
        {
            var result = new BaseOperationResponse();
            SoftDelete(dispenserOutlet);
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
