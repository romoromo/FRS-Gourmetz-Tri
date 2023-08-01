using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IWalletTransactionRepository : IRepository<WalletTransaction>
    {
        Task<BaseOperationResponse> CreateAsync(WalletTransaction walletTransaction);
        Task<BaseOperationResponse> DeleteAsync(int walletTransactionId);
        Task<WalletTransaction> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(WalletTransaction walletTransaction);
        Task<PagedEntity<WalletTransaction>> GetWalletTransactionsAsync(BaseFilter filter);
    }
}
