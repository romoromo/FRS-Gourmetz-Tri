using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface ITokenOrderService
    {
        Task<PagedEntity<TokenOrderDTO>> GetSalesOrdersAsync(SalesOrderReportFilter filter);

        Task<BaseOperationResponse> CreateTokenOrderAsync(TokenOrderDTO dto);
        Task<BaseOperationResponse> DeleteTokenOrderAsync(int id);
        Task<TokenOrderDTO> GetTokenOrderByIdAsync(int id);
        Task<PagedEntity<TokenOrderDTO>> GetTokenOrdersAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateTokenOrderAsync(TokenOrderDTO dto);
        Task<List<TokenOrderDTO>> GetUnupdatedTokenOrdersAsync();

        Task<BaseOperationResponse> CreateTokensOrderHistoryAsync(TokensOrderHistoryDTO dto, TokenOrder tokenOrder = null);
        Task<BaseOperationResponse> DeleteTokensOrderHistoryAsync(int id);
        Task<TokensOrderHistoryDTO> GetTokensOrderHistoryByIdAsync(int id);
        Task<PagedEntity<TokensOrderHistoryDTO>> GetTokensOrderHistorysAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateTokensOrderHistoryAsync(TokensOrderHistoryDTO dto);

        Task<BaseOperationResponse> CreateTokenOrderedAsync(TokenOrderedDTO dto);
        Task<BaseOperationResponse> DeleteTokenOrderedAsync(int id);
        Task<TokenOrderedDTO> GetTokenOrderedByIdAsync(int id);
        Task<PagedEntity<TokenOrderedDTO>> GetTokenOrderedsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateTokenOrderedAsync(TokenOrderedDTO dto);

        Task<BaseOperationResponse> CreateTokenLabelAsync(TokenLabelDTO dto);
        Task<BaseOperationResponse> DeleteTokenLabelAsync(int id);
        Task<TokenLabelDTO> GetTokenLabelByIdAsync(int id);
        Task<PagedEntity<TokenLabelDTO>> GetTokenLabelsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateTokenLabelAsync(TokenLabelDTO dto);

        Task<BaseOperationResponse> CreateMealAllocationAsync(MealAllocationDTO dto);
        Task<BaseOperationResponse> DeleteMealAllocationAsync(int id);
        Task<MealAllocationDTO> GetMealAllocationByIdAsync(int id);
        Task<PagedEntity<MealAllocationDTO>> GetMealAllocationsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateMealAllocationAsync(MealAllocationDTO dto);

        Task<BaseOperationResponse> CreatePackingAllocationAsync(PackingAllocationDTO dto);
        Task<BaseOperationResponse> DeletePackingAllocationAsync(int id);
        Task<PackingAllocationDTO> GetPackingAllocationByIdAsync(int id);
        Task<PagedEntity<PackingAllocationDTO>> GetPackingAllocationsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdatePackingAllocationAsync(PackingAllocationDTO dto);

        Task<List<SalesDataDTO>> RetrieveSalesData(BaseFilter filter);

        Task<List<TokenOrder>> GetTokenOrderWithCurrentSession(BaseFilter filter, int sessionDetailId);

        Task<byte[]> GenerateOrderReport(BaseFilter filter);
        Task<byte[]> GenerateMealSummaryReport(BaseFilter filter);

        Task<byte[]> GenerateDeliveryOrderReport(BaseFilter filter);

        Task<byte[]> GenerateOrderLabel(TokenLabelDTO[] dto);

        Task<BaseOperationResponse> CreateCancelOrderRequestAsync(CancelOrderRequestDTO dto);
        Task<BaseOperationResponse> DeleteCancelOrderRequestAsync(int id);
        Task<CancelOrderRequestDTO> GetCancelOrderRequestByIdAsync(int id);
        Task<PagedEntity<CancelOrderRequestDTO>> GetCancelOrderRequestsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateCancelOrderRequestAsync(CancelOrderRequestDTO dto);
        Task<BaseOperationResponse> ApprovalCancelOrderRequestAsync(CancelOrderRequestDTO dto);

        Task<byte[]> GenerateOrderLogXls(BaseFilter filter);
        Task<byte[]> GenerateOrderLogXls2(SalesOrderReportFilter filter);
        Task<byte[]> GenerateFlattenOrderLogXls(SalesOrderReportFilter filter);
        Task<BaseOperationResponse> CreateFasTokenOrdersAsync(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool clear);
        Task<FasTokenOrderSummaryDTO> GetFasTokenOrderSummaryAsync(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, List<MealSessionDetailDTO> mealSessionDetails);

        Task<BaseOperationResponse> UpdateOrderSession();
        Task<BaseOperationResponse> BulkCancelCart();
        Task<BaseOperationResponse> BulkCollectAsync(List<OrderCollectionDTO> orders);
        Task<BaseOperationResponse> BulkReturnAsync(List<OrderReturnDTO> orders);

        Task<PagedEntity<TokenOrderDTO>> GetTokenOrdersForCancellationAsync(OrderCancellationFilter filter);
        Task<BaseOperationResponse> CancelOrders(List<int> orderIds, int cancelledById, string reason);

        Task<byte[]> GenerateCancelledOrdersXls(SalesOrderReportFilter filter);
        Task<List<SalesOrderCollectionSummary>> GetSalesOrderCollectionSummary(DateTime orderDate, int outletId);
    }
}