using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces
{
    public interface IChannelInfoService
    {
        Task<BaseOperationResponse> CreateChannelInfoAsync(ChannelInfoDTO dto);
        Task<BaseOperationResponse> DeleteChannelInfoAsync(int id);
        Task<ChannelInfoDTO> GetChannelInfoByIdAsync(int id);
        Task<PagedEntity<ChannelInfoDTO>> GetChannelInfosAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateChannelInfoAsync(ChannelInfoDTO dto);
    }
}