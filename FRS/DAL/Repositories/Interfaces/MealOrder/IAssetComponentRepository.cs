using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IAssetComponentRepository
    {
        Task<BaseOperationResponse> CreateAsync(AssetComponent asset);
        Task<BaseOperationResponse> Delete(AssetComponent asset);
        Task<BaseOperationResponse> DeleteAsync(int assetId);
        Task<AssetComponent> GetByIdAsync(int id);
        Task<PagedEntity<AssetComponent>> GetAsync(AssetComponentFilter filter);
        Task<BaseOperationResponse> UpdateAsync(AssetComponent asset);
    }
}
