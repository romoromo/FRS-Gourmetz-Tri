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
    public class DeliveryOrderRepository : Repository<DeliveryOrder>, IDeliveryOrderRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public DeliveryOrderRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<DeliveryOrder>> GetDeliveryOrdersAsync(BaseFilter filter)
        {
            IQueryable<DeliveryOrder> query = _appContext.DeliveryOrders
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<DeliveryOrder>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<DeliveryOrder> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(DeliveryOrder data)
        {
            var result = new BaseOperationResponse();

            var dateTimeNow = DateTime.Now.ToString("yMMddHHmm");
            var toStore = _appContext.StoreInfos.FirstOrDefault(c => data.ToStoreId == c.Id);
            var DONumber = toStore.Code + dateTimeNow;
            data.DONumber = DONumber;
            //if (await Exists(e => e.DONumber == data.DONumber && e.IsActive))
            //{
            //    result.Message = "Code already exists!";
            //    result.IsSuccess = false;
            //}
            //else
            //{
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
            //}

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(DeliveryOrder data)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == data.Id);

            if (await Exists(e => e.Id != f.Id && e.DONumber == data.DONumber && e.IsActive))
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
            }
            else
            {

                f.CopyFrom(data);

                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    foreach (var detail in data.DeliveryDetails)
                    {
                        if (detail.Id > 0 || detail.IsActive)
                        {
                            var ddetail = _appContext.DeliveryDetails.FirstOrDefault(c => detail.Id == c.Id) ?? new DeliveryDetail();
                            ddetail.CopyFrom(detail);
                            if (ddetail.DeliveryOrderId == null) ddetail.DeliveryOrderId = f.Id;
                            ddetail.IsActive = detail.IsActive;
                            _appContext.DeliveryDetails.Update(ddetail);
                            _appContext.SaveChanges();

                            foreach (var bento in detail.DeliveryBentos)
                            {
                                if (bento.Id > 0 || bento.IsActive)
                                {
                                    var dbento = _appContext.DeliveryBentos.FirstOrDefault(c => bento.Id == c.Id) ?? new DeliveryBento();
                                    dbento.CopyFrom(bento);
                                    if (dbento.DeliveryDetailId == null) dbento.DeliveryDetailId = ddetail.Id;
                                    dbento.IsActive = bento.IsActive;
                                    _appContext.DeliveryBentos.Update(dbento);
                                    _appContext.SaveChanges();
                                }
                            }
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

        public async Task<BaseOperationResponse> Delete(DeliveryOrder data)
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

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
