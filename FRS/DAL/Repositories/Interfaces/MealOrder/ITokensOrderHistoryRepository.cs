using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ITokensOrderHistoryRepository
    {
        Task<BaseOperationResponse> CreateAsync(TokensOrderHistory order);
        Task<BaseOperationResponse> Delete(TokensOrderHistory order);
        Task<BaseOperationResponse> DeleteAsync(int orderId);
        Task<TokensOrderHistory> GetByIdAsync(int id);
        //Task<BaseOperationResponse> ImportAsync(IAccountManager accountManager, List<StudentImportDTO> dto);
        Task<PagedEntity<TokensOrderHistory>> GetTokensOrderHistorysAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(TokensOrderHistory order);
    }
}