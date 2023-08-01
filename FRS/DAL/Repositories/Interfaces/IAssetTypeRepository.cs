using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAssetTypeRepository : IRepository<AssetType>
    {
        Task<BaseOperationResponse> CreateAsync(AssetType assetType);
        Task<BaseOperationResponse> DeleteAsync(int assetTypeId);
        Task<AssetType> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(AssetType assetType);
        Task<PagedEntity<AssetType>> GetAssetTypesAsync(BaseFilter filter);
    }
}
