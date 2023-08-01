using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IDeliveryOrderRepository
    {
        Task<BaseOperationResponse> CreateAsync(DeliveryOrder data);
        Task<BaseOperationResponse> Delete(DeliveryOrder data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<DeliveryOrder> GetByIdAsync(int id);
        Task<PagedEntity<DeliveryOrder>> GetDeliveryOrdersAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(DeliveryOrder data);
    }
}