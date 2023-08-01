using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;

namespace DAL.Repositories
{
    public class DirectoryListingCategoryRepository : Repository<DirectoryListingCategory>, IDirectoryListingCategoryRepository
    {
        public DirectoryListingCategoryRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<DirectoryListingCategory> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<DirectoryListingCategory> All()
        {
            return GetAll()
                .OrderBy(c => c.Code)
                .ToList();
        }

        public async Task<List<DirectoryListingCategory>> GetDirectoryListingCategorysLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<DirectoryListingCategory> query = _appContext.DirectoryListingCategory
                .Where(e => e.IsActive)
                .OrderBy(r => r.Code);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(DirectoryListingCategory directoryListingCategory)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Code == directoryListingCategory.Code).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await AddAsync(directoryListingCategory);
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

        public async Task<BaseOperationResponse> UpdateAsync(DirectoryListingCategory directoryListingCategory)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Code == directoryListingCategory.Code && e.Id != directoryListingCategory.Id).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await GetSingleOrDefaultAsync(e => e.Id == directoryListingCategory.Id);

            f.CopyFrom(directoryListingCategory);
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


        public async Task<BaseOperationResponse> DeleteAsync(int directoryListingCategoryId)
        {
            var result = new BaseOperationResponse();
            var directoryListingCategory = await GetSingleOrDefaultAsync(r => r.Id == directoryListingCategoryId);

            if (directoryListingCategory != null)
                return await Delete(directoryListingCategory);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(DirectoryListingCategory directoryListingCategory)
        {
            var result = new BaseOperationResponse();
            SoftDelete(directoryListingCategory);
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

        public async Task<int> GetOrCreateByCode(DirectoryListingCategory data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Code)) return 0;

            var f = await GetSingleOrDefaultAsync(c => c.Code == data.Code && c.IsActive);

            if (f == null)
            {
                await CreateAsync(data);
                f = await GetSingleOrDefaultAsync(c => c.Code == data.Code && c.IsActive);
            }

            return f.Id;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
