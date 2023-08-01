using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IBentoAssetRepository
    {
        Task<BaseOperationResponse> CreateAsync(BentoAsset data);
        Task<BaseOperationResponse> Delete(BentoAsset data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<BentoAsset> GetByIdAsync(int id);
        Task<PagedEntity<BentoAsset>> GetBentoAssetsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(BentoAsset data);

        Task<BaseOperationResponse> ResetAsync();
    }
}