using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IStudentPointTransactionRepository : IRepository<StudentPointTransaction>
    {
        Task<BaseOperationResponse> CreateAsync(StudentPointTransaction pointTransaction);
        Task<BaseOperationResponse> DeleteAsync(int pointTransactionId);
        Task<StudentPointTransaction> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(StudentPointTransaction pointTransaction);
        Task<PagedEntity<StudentPointTransaction>> GetPointTransactionsAsync(BaseFilter filter);
        Task<BaseOperationResponse> TopupPointBalanceByStudentGroupIdAsync(int studentGroupId, double amount);
        Task<BaseOperationResponse> TopupPointBalanceByStudentIdAsync(int studentId, double amount);
    }
}
