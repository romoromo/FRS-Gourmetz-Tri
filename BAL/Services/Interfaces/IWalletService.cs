using BAL.DTO;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IWalletService
    {
        Task<BaseOperationResponse> CreateAsync(WalletDTO dto);
        Task<BaseOperationResponse> DeleteAsync(int id);
        Task<WalletDTO> GetByIdAsync(int id);
        Task<List<WalletDTO>> GetByUserIdAsync(int userId);
        Task<PagedEntity<WalletDTO>> GetWalletsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(WalletDTO dto);
        Task<BaseOperationResponse> TopUpAsync(WalletTopUpDTO dto);
        Task<BaseOperationResponse> WalletOperationAsync(WalletOperationDTO dto);
        Task<List<WalletTransactionDTO>> GetWalletTransactionByIdAsync(int id);
    }
}
