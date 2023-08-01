using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;

namespace DAL.Repositories
{
    public class RewardTransactionRepository : Repository<RewardTransaction>, IRewardTransactionRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public RewardTransactionRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<RewardTransaction>> GetRewardTransactionsAsync(BaseFilter filter)
        {
            IQueryable<RewardTransaction> query = _appContext.RewardTransactions;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<RewardTransaction> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(RewardTransaction rewardTransaction)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(rewardTransaction);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(RewardTransaction rewardTransaction)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == rewardTransaction.Id);

            f.CopyFrom(rewardTransaction);

            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int rewardTransactionId)
        {
            var result = new BaseOperationResponse();
            var rewardTransaction = await GetSingleOrDefaultAsync(r => r.Id == rewardTransactionId);

            if (rewardTransaction != null)
                return await Delete(rewardTransaction);

            result.IsSuccess = false;
            result.Message = "RewardTransaction not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(RewardTransaction rewardTransaction)
        {
            var result = new BaseOperationResponse();
            SoftDelete(rewardTransaction);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
