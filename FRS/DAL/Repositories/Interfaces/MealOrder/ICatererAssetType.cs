using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ICatererAssetTypeRepository
    {
        Task<BaseOperationResponse> CreateAsync(CatererAssetType assetType);
        Task<BaseOperationResponse> Delete(CatererAssetType assetType);
        Task<BaseOperationResponse> DeleteAsync(int assetTypeId);
        Task<CatererAssetType> GetByIdAsync(int id);
        Task<PagedEntity<CatererAssetType>> GetAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(CatererAssetType assetType);
    }
}
