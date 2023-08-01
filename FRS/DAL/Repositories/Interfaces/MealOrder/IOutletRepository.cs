using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IOutletRepository
    {
        Task<BaseOperationResponse> CreateAsync(Outlet data);
        Task<BaseOperationResponse> Delete(Outlet data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<Outlet> GetByIdAsync(int id);
        Task<PagedEntity<Outlet>> GetOutletsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(Outlet data);
        Task<BaseOperationResponse> UpdateStoresAsync(Outlet data, List<StoreInfo> stores);
    }
}