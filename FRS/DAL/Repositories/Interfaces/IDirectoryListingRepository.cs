using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDirectoryListingRepository : IRepository<DirectoryListing>
    {
        IEnumerable<DirectoryListing> All();
        Task<List<DirectoryListing>> GetDirectoryListingsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(DirectoryListing directoryListing);
        Task<BaseOperationResponse> DeleteAsync(int directoryListingId);
        Task<DirectoryListing> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(DirectoryListing directoryListing);
        Task<BaseOperationResponse> ImportFile(List<List<string>> data);
        Task<List<DirectoryListing>> GetDirectoryListingsExcludeInternal(int? institutionId = null);
        Task<byte[]> GetTemplate();
    }
}
