using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IEmailConfirmRepository
    {
        Task<BaseOperationResponse> CreateAsync(EmailConfirm ec);
        Task<BaseOperationResponse> Delete(EmailConfirm ec);
        Task<BaseOperationResponse> DeleteAsync(int ecId);
        Task<EmailConfirm> GetByIdAsync(int id);
        Task<EmailConfirm> GetByEmailAsync(string email);
        Task<PagedEntity<EmailConfirm>> GetEmailConfirmsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(EmailConfirm ec);
    }
}