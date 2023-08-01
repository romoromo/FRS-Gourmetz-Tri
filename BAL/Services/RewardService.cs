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
    public class RewardService : IRewardService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;

        public RewardService(IUnitOfWork uow, ISieveProcessor sieveProcessor)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
        }

        public async Task<PagedEntity<RewardDTO>> GetRewardsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<RewardDTO>>(await this._uow.Rewards.GetRewardsAsync(filter));
            return result;
        }

        public async Task<RewardDTO> GetByIdAsync(int id)
        {
            return Mapper.Map<RewardDTO>(await this._uow.Rewards.GetByIdAsync(id));
        }

        public async Task<List<RewardDTO>> GetByUserIdAsync(int userId)
        {
            return Mapper.Map<List<RewardDTO>>(await this._uow.Rewards.FindAsync(e => e.IsActive && e.UserId == userId));
        }

        public async Task<BaseOperationResponse> CreateAsync(RewardDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Rewards.CreateAsync(Mapper.Map<Reward>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(RewardDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Rewards.UpdateAsync(Mapper.Map<Reward>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Rewards.DeleteAsync(id);
            return result;
        }

        public async Task<List<RewardTransactionDTO>> GetRewardTransactionByIdAsync(int id)
        {
            return Mapper.Map<List<RewardTransactionDTO>>((await this._uow.RewardTransactions.FindAsync(e => e.RewardId == id)).ToList());
        }

        public async Task<BaseOperationResponse> RewardOperationAsync(RewardOperationDTO dto)
        {
            var result = new BaseOperationResponse();
            var reward = await this._uow.Rewards.GetByIdAsync(dto.RewardId);

            if (!reward.ConcurrencyStamp.SequenceEqual(dto.ConcurrencyStamp))
            {
                result.IsSuccess = false;
                result.Message = "Reward is not the latest version. Please refresh.";
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

                if (dto.TransactionType.Equals(RewardTransactionType.CREDIT.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    reward.Balance += dto.Amount;
                }
                else if (dto.TransactionType.Equals(RewardTransactionType.DEBIT.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    if (reward.Balance - dto.Amount < 0)
                    {
                        result.IsSuccess = false;
                        result.Message = "Insufficient balance.";
                        return result;
                    }
                    else
                    {
                        reward.Balance -= dto.Amount;
                    }
                }

                result = await this._uow.Rewards.UpdateAsync(reward);

                if (result.IsSuccess)
                {
                    var transaction = new RewardTransaction
                    {
                        Amount = dto.Amount,
                        TransactionType = dto.TransactionType,
                        RewardId = dto.RewardId,
                        Description = dto.Description
                    };

                    await this._uow.RewardTransactions.CreateAsync(transaction);
                }

                var d = Mapper.Map<RewardDTO>(result.Data);
                result.Data = d;
            }

            return result;
        }
    }
}
