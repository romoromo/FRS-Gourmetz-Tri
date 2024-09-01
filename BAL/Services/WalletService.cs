using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL;
using BAL.Services.Interfaces;
using BAL.DTO;
using AutoMapper;
using DAL.Repositories.Interfaces;

namespace BAL.Services
{
    public class WalletService : IWalletService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public WalletService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedEntity<WalletDTO>> GetWalletsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<WalletDTO>>(await this._uow.Wallets.GetWalletsAsync(filter));
            return result;
        }

        public async Task<WalletDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<WalletDTO>(await this._uow.Wallets.GetByIdAsync(id));
        }

        public async Task<List<WalletDTO>> GetByUserIdAsync(int userId)
        {
            return _mapper.Map<List<WalletDTO>>(await this._uow.Wallets.FindAsync(e => e.IsActive && e.UserId == userId));
        }

        public async Task<BaseOperationResponse> CreateAsync(WalletDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Wallets.CreateAsync(_mapper.Map<Wallet>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(WalletDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Wallets.UpdateAsync(_mapper.Map<Wallet>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Wallets.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> TopUpAsync(WalletTopUpDTO dto)
        {
            var result = new BaseOperationResponse();
            var wallet = await this._uow.Wallets.GetByIdAsync(dto.WalletId);

            if (!wallet.ConcurrencyStamp.SequenceEqual(dto.ConcurrencyStamp))
            {
                result.IsSuccess = false;
                result.Message = "Wallet is not the latest version. Please refresh.";
            }
            else
            {
                wallet.Balance += dto.Amount;
                result = await this._uow.Wallets.UpdateAsync(wallet);
            }

            return result;
        }

        public async Task<List<WalletTransactionDTO>> GetWalletTransactionByIdAsync(int id)
        {
            return _mapper.Map<List<WalletTransactionDTO>>((await this._uow.WalletTransactions.FindAsync(e => e.WalletId == id)).ToList());
        }

        public async Task<BaseOperationResponse> WalletOperationAsync(WalletOperationDTO dto)
        {
            var result = new BaseOperationResponse();
            var wallet = await this._uow.Wallets.GetByIdAsync(dto.WalletId);

            if (!wallet.ConcurrencyStamp.SequenceEqual(dto.ConcurrencyStamp))
            {
                result.IsSuccess = false;
                result.Message = "Wallet is not the latest version. Please refresh.";
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(dto.TransactionType) ||
                    (!dto.TransactionType.Equals(RewardTransactionType.CREDIT.ToString(), StringComparison.OrdinalIgnoreCase) &&
                    !dto.TransactionType.Equals(RewardTransactionType.DEBIT.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    result.IsSuccess = false;
                    result.Message = "Transaction type is missing or invalid.";
                    return result;
                }

                if (dto.TransactionType.Equals(WalletTransactionType.CREDIT.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    wallet.Balance += dto.Amount;
                }
                else if (dto.TransactionType.Equals(WalletTransactionType.DEBIT.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    if (wallet.Balance - dto.Amount < 0)
                    {
                        result.IsSuccess = false;
                        result.Message = "Insufficient balance.";
                        return result;
                    }
                    else
                    {
                        wallet.Balance -= dto.Amount;
                    }
                }

                result = await this._uow.Wallets.UpdateAsync(wallet);

                if (result.IsSuccess)
                {
                    var transaction = new WalletTransaction
                    {
                        Amount = dto.Amount,
                        TransactionType = dto.TransactionType,
                        WalletId = dto.WalletId,
                        Description = dto.Description
                    };

                    await this._uow.WalletTransactions.CreateAsync(transaction);
                }

                var d = _mapper.Map<WalletDTO>(result.Data);
                result.Data = d;
            }

            return result;
        }
    }
}
