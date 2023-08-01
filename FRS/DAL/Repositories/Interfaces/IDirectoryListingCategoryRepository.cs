using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDirectoryListingCategoryRepository : IRepository<DirectoryListingCategory>
    {
        IEnumerable<DirectoryListingCategory> All();
        Task<List<DirectoryListingCategory>> GetDirectoryListingCategorysLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(DirectoryListingCategory directoryListingCategory);
        Task<BaseOperationResponse> DeleteAsync(int directoryListingCategoryId);
        Task<DirectoryListingCategory> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(DirectoryListingCategory directoryListingCategory);
    }
}
