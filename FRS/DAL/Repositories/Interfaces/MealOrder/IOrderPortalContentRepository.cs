using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IOrderPortalContentRepository
    {
        Task<BaseOperationResponse> CreateAsync(OrderPortalContent content);
        Task<BaseOperationResponse> Delete(OrderPortalContent content);
        Task<BaseOperationResponse> DeleteAsync(int contentId);
        Task<OrderPortalContent> GetByIdAsync(int id);
        Task<OrderPortalContent> GetOrderPortalContentFirst(int outletId);
        Task<PagedEntity<OrderPortalContent>> GetOrderPortalContentsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(OrderPortalContent content);
    }
}