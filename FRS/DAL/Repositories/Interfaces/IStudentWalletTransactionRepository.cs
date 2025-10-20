using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IStudentWalletTransactionRepository : IRepository<StudentWalletTransaction>
    {
        Task<BaseOperationResponse> CreateAsync(StudentWalletTransaction walletTransaction);
        Task<BaseOperationResponse> DeleteAsync(int walletTransactionId);
        Task<StudentWalletTransaction> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(StudentWalletTransaction walletTransaction);
        Task<PagedEntity<StudentWalletTransaction>> GetWalletTransactionsAsync(BaseFilter filter);
        Task<BaseOperationResponse> TopupWalletBalanceByStudentGroupIdAsync(int studentGroupId, double amount, int userId);
        Task<BaseOperationResponse> TopupWalletBalanceByStudentIdAsync(int studentId, double amount, int userId);
        Task<BaseOperationResponse> OffBoardingStudent(int studentId, int userId);
    }
}
