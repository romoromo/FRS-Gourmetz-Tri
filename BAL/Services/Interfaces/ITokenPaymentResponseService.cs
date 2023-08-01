using BAL.DTO;
using DAL.Core;
using SMV.FOMOPay.Model;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface ITokenPaymentResponseService
    {
        Task<BaseOperationResponse> CreateTokenPaymentResponseAsync(TokenPaymentResponseDTO dto);
        //Task<BaseOperationResponse> DeleteAsync(int id);
        //Task<RewardDTO> GetByIdAsync(int id);
        //Task<List<RewardDTO>> GetByUserIdAsync(int userId);
        //Task<PagedEntity<RewardDTO>> GetRewardsAsync(BaseFilter filter);
        //Task<BaseOperationResponse> UpdateAsync(RewardDTO dto);
        //Task<BaseOperationResponse> RewardOperationAsync(RewardOperationDTO dto);
        //Task<List<RewardTransactionDTO>> GetRewardTransactionByIdAsync(int id);
    }
}
