using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface ITokenPaymentResponseRepository : IRepository<TokenPaymentResponse>
    //public interface IRewardRepository : IRepository<Reward>
    {
        Task<BaseOperationResponse> CreateAsync(TokenPaymentResponse tokenPaymentResponse);

        Task<TokenPaymentResponse> GetByIdAsync(string id);

        Task<BaseOperationResponse> UpdateAsync(TokenPaymentUpdate tokenPaymentUpdate);

        Task<BaseOperationResponse> UpdateAsync(string strTransacionOrderId, string strUpdateStatuse);

        //Task<BaseOperationResponse> DeleteAsync(int rewardId);
        //Task<Reward> GetByIdAsync(int id);
        //Task<BaseOperationResponse> UpdateAsync(Reward reward);
        //Task<PagedEntity<Reward>> GetRewardsAsync(BaseFilter filter);
    }
}
