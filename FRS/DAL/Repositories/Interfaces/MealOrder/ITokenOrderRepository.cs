using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Models.StoredProcedures;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ITokenOrderRepository
    {
        Task<List<spSalesOrderReport>> GetSalesOrders(SalesOrderReportFilter filter);
        Task<BaseOperationResponse> CreateAsync(TokenOrder order, List<TokenOrdered> tokenOrders);
        Task<BaseOperationResponse> Delete(TokenOrder order);
        Task<BaseOperationResponse> DeleteAsync(int orderId);
        Task<TokenOrder> GetByIdAsync(int id);
        //Task<BaseOperationResponse> ImportAsync(IAccountManager accountManager, List<StudentImportDTO> dto);
        Task<PagedEntity<TokenOrder>> GetTokenOrdersAsync(BaseFilter filter);
        Task<PagedEntity<TokenOrder>> GetCancellationOrdersAsync(OrderCancellationFilter filter);
        Task<BaseOperationResponse> BulkCancelCart();
        Task<List<TokenOrder>> GetUnupdatedTokenOrdersAsync();
        Task<BaseOperationResponse> UpdateAsync(TokenOrder order, List<TokenOrdered> tokenOrders);
        Task<BaseOperationResponse> BulkCollectAsync(List<OrderCollectionDTO> orders);
        Task<BaseOperationResponse> BulkReturnAsync(List<OrderReturnDTO> orders);
        Task<BaseOperationResponse> UpdateAsync(TokenOrder order);
        Task<BaseOperationResponse> CreateFasTokenOrdersAsync(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool clear);
        Task<FasTokenOrderSummaryDTO> GetFasTokenOrderSummaryAsync(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, List<MealSessionDetail> mealSessionDetails);
        Task<BaseOperationResponse> CancelOrders(List<int> orderIds, int cancelledById, string reason);
        Task<BaseOperationResponse> UpdateCancellationStatus(int orderId, bool isApproved, string response);
        Task<BaseOperationResponse> UpdateOrderCancelRequestStatus(int orderId, string status);
        Task<BaseOperationResponse> CreateMealPlanAsync(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool clear);
        Task<MealPlanTokenOrderSummaryDTO> GetMealPlanOrderSummaryAsync(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, List<MealSessionDetail> mealSessionDetails);
        Task<List<StudentGroupMealPlan>> GetStudentMealPlansAsync(int studentId, DateTime? orderDate);
    }
}