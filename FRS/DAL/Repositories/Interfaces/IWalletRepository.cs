using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        Task<BaseOperationResponse> CreateAsync(Wallet wallet);
        Task<BaseOperationResponse> DeleteAsync(int walletId);
        Task<Wallet> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Wallet wallet);
        Task<PagedEntity<Wallet>> GetWalletsAsync(BaseFilter filter);
    }
}
