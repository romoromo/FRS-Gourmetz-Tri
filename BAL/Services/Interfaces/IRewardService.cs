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
    public interface IRewardService
    {
        Task<BaseOperationResponse> CreateAsync(RewardDTO dto);
        Task<BaseOperationResponse> DeleteAsync(int id);
        Task<RewardDTO> GetByIdAsync(int id);
        Task<List<RewardDTO>> GetByUserIdAsync(int userId);
        Task<PagedEntity<RewardDTO>> GetRewardsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(RewardDTO dto);
        Task<BaseOperationResponse> RewardOperationAsync(RewardOperationDTO dto);
        Task<List<RewardTransactionDTO>> GetRewardTransactionByIdAsync(int id);
    }
}
