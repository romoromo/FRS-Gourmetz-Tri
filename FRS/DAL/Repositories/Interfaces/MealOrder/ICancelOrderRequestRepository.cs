using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface ICancelOrderRequestRepository : IRepository<CancelOrderRequest>
    {
        Task<BaseOperationResponse> CreateAsync(CancelOrderRequest cancelOrderRequest);
        Task<BaseOperationResponse> DeleteAsync(int cancelOrderRequestId);
        Task<CancelOrderRequest> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(CancelOrderRequest cancelOrderRequest);
        Task<PagedEntity<CancelOrderRequest>> GetCancelOrderRequestsAsync(CancelOrderRequestFilter filter);
    }
}
