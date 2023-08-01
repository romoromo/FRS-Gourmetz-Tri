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
    public class ImageReferenceTypeRepository : Repository<ImageReferenceType>, IImageReferenceTypeRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public ImageReferenceTypeRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<ImageReferenceType>> GetImageReferenceTypesAsync(BaseFilter filter)
        {
            IQueryable<ImageReferenceType> query = _appContext.ImageReferenceTypes
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<ImageReferenceType>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<ImageReferenceType> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(ImageReferenceType ImageReferenceType)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(ImageReferenceType);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save Image Reference Type!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(ImageReferenceType ImageReferenceType)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == ImageReferenceType.Id);

            f.CopyFrom(ImageReferenceType);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save ImageReferenceType!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int ImageReferenceTypeId)
        {
            var result = new BaseOperationResponse();
            var ImageReferenceType = await GetSingleOrDefaultAsync(r => r.Id == ImageReferenceTypeId);

            if (ImageReferenceType != null)
                return await Delete(ImageReferenceType);

            result.IsSuccess = false;
            result.Message = "Image Reference Type not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(ImageReferenceType ImageReferenceType)
        {
            var result = new BaseOperationResponse();
            SoftDelete(ImageReferenceType);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete Image Reference Type!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
