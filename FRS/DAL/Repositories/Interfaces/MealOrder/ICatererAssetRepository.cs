using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ICatererAssetRepository
    {
        Task<BaseOperationResponse> CreateAsync(CatererAsset asset);
        Task<BaseOperationResponse> Delete(CatererAsset asset);
        Task<BaseOperationResponse> DeleteAsync(int assetId);
        Task<CatererAsset> GetByIdAsync(int id);
        Task<PagedEntity<CatererAsset>> GetAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(CatererAsset asset);
    }
}
