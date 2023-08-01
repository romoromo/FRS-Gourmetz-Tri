using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ITokenOrderedRepository
    {
        Task<BaseOperationResponse> CreateAsync(TokenOrdered token);
        Task<BaseOperationResponse> Delete(TokenOrdered token);
        Task<BaseOperationResponse> DeleteAsync(int tokenId);
        Task<TokenOrdered> GetByIdAsync(int id);
        //Task<Student> GetStudentByCardIdAsync(string cardId);
        //Task<BaseOperationResponse> ActivateStudentCardByIdAsync(string cardId);
        Task<PagedEntity<TokenOrdered>> GetTokenOrderedsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(TokenOrdered token);
    }
}