using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IRewardTransactionRepository : IRepository<RewardTransaction>
    {
        Task<BaseOperationResponse> CreateAsync(RewardTransaction rewardTransaction);
        Task<BaseOperationResponse> DeleteAsync(int rewardTransactionId);
        Task<RewardTransaction> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(RewardTransaction rewardTransaction);
        Task<PagedEntity<RewardTransaction>> GetRewardTransactionsAsync(BaseFilter filter);
    }
}
