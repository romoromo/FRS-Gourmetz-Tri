using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface ITokenPaymentRepository : IRepository<TokenPaymentRequest>
    //public interface IRewardRepository : IRepository<Reward>
    {
        Task<BaseOperationResponse> CreateAsync(TokenPaymentRequest tokenPaymentRequest);

       // Task<BaseOperationResponse> UpdateAsync(TokenPaymentUpdate reward);

        //Task<BaseOperationResponse> DeleteAsync(int rewardId);
        //Task<Reward> GetByIdAsync(int id);
        //Task<BaseOperationResponse> UpdateAsync(Reward reward);
        //Task<PagedEntity<Reward>> GetRewardsAsync(BaseFilter filter);
    }
}
