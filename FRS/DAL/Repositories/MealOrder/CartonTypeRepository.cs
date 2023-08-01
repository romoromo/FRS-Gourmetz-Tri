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
    public class CartonTypeRepository : Repository<CartonType>, ICartonTypeRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public CartonTypeRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<CartonType>> GetCartonTypesAsync(BaseFilter filter)
        {
            IQueryable<CartonType> query = _appContext.CartonTypes
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<CartonType>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<CartonType> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(CartonType data)
        {
            var result = new BaseOperationResponse();
            if (await Exists(e => e.Code == data.Code && e.IsActive))
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
            }
            else
            {
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
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(CartonType data)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == data.Id);

            foreach (var detail in data.CartonAssets)
            {
                if (detail.Id > 0 || detail.IsActive)
                {
                    var ddetail = _appContext.CartonAssets.FirstOrDefault(c => detail.Code == c.Code) ?? new CartonAsset();
                    if (ddetail.Id > 0) detail.Id = ddetail.Id;
                    ddetail.CopyFrom(detail);
                    ddetail.Code = detail.Code;
                    ddetail.CartonTypeId = data.Id;
                    ddetail.InstitutionId = f.InstitutionId;
                    ddetail.IsActive = detail.IsActive;
                    _appContext.CartonAssets.Update(ddetail);
                    _appContext.SaveChanges();

                }
            }

            if (await Exists(e => e.Id != f.Id && e.Code == data.Code && e.IsActive))
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

        public async Task<BaseOperationResponse> Delete(CartonType data)
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
