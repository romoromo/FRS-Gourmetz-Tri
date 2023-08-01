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
    public class ImageReferenceColorRepository : Repository<ImageReferenceColor>, IImageReferenceColorRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public ImageReferenceColorRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<ImageReferenceColor>> GetImageReferenceColorsAsync(BaseFilter filter)
        {
            IQueryable<ImageReferenceColor> query = _appContext.ImageReferenceColors
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<ImageReferenceColor>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<ImageReferenceColor> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(ImageReferenceColor ImageReferenceColor)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(ImageReferenceColor);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save Image Reference Color!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(ImageReferenceColor ImageReferenceColor)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == ImageReferenceColor.Id);

            f.CopyFrom(ImageReferenceColor);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save Image Reference Color!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int ImageReferenceColorId)
        {
            var result = new BaseOperationResponse();
            var ImageReferenceColor = await GetSingleOrDefaultAsync(r => r.Id == ImageReferenceColorId);

            if (ImageReferenceColor != null)
                return await Delete(ImageReferenceColor);

            result.IsSuccess = false;
            result.Message = "Image Reference Color not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(ImageReferenceColor ImageReferenceColor)
        {
            var result = new BaseOperationResponse();
            SoftDelete(ImageReferenceColor);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete Image Reference Color!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
