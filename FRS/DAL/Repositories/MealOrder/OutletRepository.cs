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
    public class OutletRepository : Repository<Outlet>, IOutletRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public OutletRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Outlet>> GetOutletsAsync(BaseFilter filter)
        {
            IQueryable<Outlet> query = _appContext.Outlets.Include(e => e.CatererOutlets);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<Outlet>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<Outlet> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(Outlet data)
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

        public async Task<BaseOperationResponse> UpdateAsync(Outlet data)
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

        public async Task<BaseOperationResponse> Delete(Outlet data)
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


        public async Task<BaseOperationResponse> UpdateStoresAsync(Outlet data, List<StoreInfo> stores)
        {
            var result = new BaseOperationResponse();

            var nodesToDelete = this._appContext.StoreInfos.Where(x => x.OutletId == data.Id).ToList();

            if (nodesToDelete != null)
            {
                nodesToDelete.ForEach(e =>
                {
                    var sr = this._appContext.StoreInfos.FirstOrDefault(x => x.Id == e.Id && x.OutletId == e.OutletId);
                    if (sr != null)
                    {
                        sr.OutletId = null;
                        this._appContext.StoreInfos.Update(sr);
                    }
                });
            }

            if (stores != null)
            {
                stores.ForEach(e =>
                {
                    var sr = this._appContext.StoreInfos.FirstOrDefault(x => x.Id == e.Id);
                    if (sr != null)
                    {
                        sr.OutletId = data.Id;
                        this._appContext.StoreInfos.Update(sr);
                    }
                    else
                    {
                        this._appContext.StoreInfos.Add(e);
                    }
                });
            }

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = stores;
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
