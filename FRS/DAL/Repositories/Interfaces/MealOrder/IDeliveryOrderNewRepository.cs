using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IDeliveryOrderNewRepository
    {
        Task<BaseOperationResponse> CreateAsync(DeliveryOrderNew data);
        Task<BaseOperationResponse> Delete(DeliveryOrderNew data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<DeliveryOrderNew> GetByIdAsync(int id);
        Task<PagedEntity<DeliveryOrderNew>> GetDeliveryOrdersAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(DeliveryOrderNew data);
        Task<BaseOperationResponse> LoadAsync(DeliveryOrderNew data);
        
    }
}