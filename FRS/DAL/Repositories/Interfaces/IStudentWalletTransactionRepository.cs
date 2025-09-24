using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
