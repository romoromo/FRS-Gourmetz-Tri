using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAssetModelRepository : IRepository<AssetModel>
    {
        Task<BaseOperationResponse> CreateAsync(AssetModel facility);
        Task<BaseOperationResponse> DeleteAsync(int AssetModelId);
        Task<AssetModel> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(AssetModel AssetModel);
        Task<PagedEntity<AssetModel>> GetAssetModelsAsync(BaseFilter filter);
        Task<int> GetOrCreateByCode(AssetModel model);
    }
}
