using System.Threading.Tasks;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IOrderPortalService
    {
        Task<BaseOperationResponse> CreateOrderPortalContentAsync(OrderPortalContentDTO dto);
        Task<BaseOperationResponse> DeleteOrderPortalContentAsync(int id);
        Task<OrderPortalContentDTO> GetOrderPortalContentByIdAsync(int id);
        Task<OrderPortalContentDTO> GetOrderPortalContentFirst(int outletId);
        Task<PagedEntity<OrderPortalContentDTO>> GetOrderPortalContentsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateOrderPortalContentAsync(OrderPortalContentDTO dto);
    }
}