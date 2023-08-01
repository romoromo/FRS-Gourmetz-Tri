using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IStoreInfoRepository
    {
        Task<BaseOperationResponse> CreateAsync(StoreInfo data);
        Task<BaseOperationResponse> Delete(StoreInfo data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<StoreInfo> GetByIdAsync(int id);
        Task<PagedEntity<StoreInfo>> GetStoreInfosAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(StoreInfo data);
    }
}