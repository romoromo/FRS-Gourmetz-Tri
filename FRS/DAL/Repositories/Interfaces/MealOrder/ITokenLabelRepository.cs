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
    public interface ITokenLabelRepository
    {
        Task<BaseOperationResponse> CreateAsync(TokenLabel order, List<TokenDishLabel> tokenOrders);
        Task<BaseOperationResponse> Delete(TokenLabel order);
        Task<BaseOperationResponse> DeleteAsync(int orderId);
        Task<TokenLabel> GetByIdAsync(int id);
        //Task<BaseOperationResponse> ImportAsync(IAccountManager accountManager, List<StudentImportDTO> dto);
        Task<PagedEntity<TokenLabel>> GetTokenLabelsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(TokenLabel order, List<TokenDishLabel> tokenOrders);
    }
}