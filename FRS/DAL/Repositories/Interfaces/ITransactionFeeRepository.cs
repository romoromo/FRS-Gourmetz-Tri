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
    public interface ITransactionFeeRepository : IRepository<TransactionFee>
    {
        Task<BaseOperationResponse> CreateAsync(TransactionFee transactionFee);
        Task<BaseOperationResponse> DeleteAsync(int transactionFeeId);
        Task<TransactionFee> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(TransactionFee transactionFee);
        Task<PagedEntity<TransactionFee>> GetTransactionFeesAsync(BaseFilter filter);
    }
}
