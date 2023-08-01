using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IStoreInventoryRepository
    {
        Task<BaseOperationResponse> CreateAsync(StoreInventory data);
        Task<BaseOperationResponse> Delete(StoreInventory data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<StoreInventory> GetByIdAsync(int id);
        Task<PagedEntity<StoreInventory>> GetStoreInventoriesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(StoreInventory data);
        Task<PagedEntity<StoreInventoryDetail>> GetStoreInventoryDetailsAsync(BaseFilter filter);
    }
}