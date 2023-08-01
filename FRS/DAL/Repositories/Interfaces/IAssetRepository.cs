using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAssetRepository : IRepository<Asset>
    {
        Task<BaseOperationResponse> CreateAsync(Asset asset);
        Task<BaseOperationResponse> DeleteAsync(int assetId);
        Task<Asset> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Asset asset);
        Task<PagedEntity<Asset>> GetAssetsAsync(BaseFilter filter);
        Task<Asset> GetByCodeAsync(string serialNumber);
    }
}
