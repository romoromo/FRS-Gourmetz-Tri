using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ICartonAssetRepository
    {
        Task<BaseOperationResponse> CreateAsync(CartonAsset data);
        Task<BaseOperationResponse> Delete(CartonAsset data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<CartonAsset> GetByIdAsync(int id);
        Task<PagedEntity<CartonAsset>> GetCartonAssetsAsync(CartonAssetsFilter filter);
        Task<BaseOperationResponse> UpdateAsync(CartonAsset data);
        Task<BaseOperationResponse> ResetAsync();
    }
}