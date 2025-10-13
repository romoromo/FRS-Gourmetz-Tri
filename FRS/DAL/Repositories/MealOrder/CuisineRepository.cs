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
    public class CuisineRepository : Repository<Cuisine>, ICuisineRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public CuisineRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Cuisine>> GetCuisinesAsync(BaseFilter filter)
        {
            IQueryable<Cuisine> query = _appContext.Cuisines
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            return result;
        }

        #endregion
        public async Task<Cuisine> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(Cuisine cuisine)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(cuisine);
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

        public async Task<BaseOperationResponse> UpdateAsync(Cuisine cuisine)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == cuisine.Id);

            f.CopyFrom(cuisine);

            if (cuisine.Icon != null && !string.IsNullOrEmpty(cuisine.Icon.Path))
            {
                if (!f.FileId.HasValue)
                {
                    f.Icon = cuisine.Icon;
                }
                else
                {
                    if (f.Icon == null)
                    {
                        //TODO: check why EF Core is not loading the Icon property; interim solution
                        var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == f.FileId);
                        if (icon == null)
                        {
                            f.Icon = new Models.File();
                        }
                        else
                        {
                            f.Icon = icon;
                            f.FileId = icon.Id;
                        }
                    }

                    f.Icon.Path = cuisine.Icon.Path;
                    f.Icon.FileName = cuisine.Icon.FileName ?? System.IO.Path.GetFileName(cuisine.Icon.Path);
                    f.Icon.Type = FileType.Icon.ToString();
                }
            }

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


        public async Task<BaseOperationResponse> DeleteAsync(int cuisineId)
        {
            var result = new BaseOperationResponse();
            var cuisine = await GetSingleOrDefaultAsync(r => r.Id == cuisineId);

            if (cuisine != null)
                return await Delete(cuisine);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Cuisine cuisine)
        {
            var result = new BaseOperationResponse();
            SoftDelete(cuisine);
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
