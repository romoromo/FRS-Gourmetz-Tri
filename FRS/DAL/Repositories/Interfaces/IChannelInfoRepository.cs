using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IChannelInfoRepository : IRepository<ChannelInfo>
    {
        Task<BaseOperationResponse> CreateAsync(ChannelInfo data);
        Task<BaseOperationResponse> DeleteAsync(int id);
        Task<ChannelInfo> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(ChannelInfo data);
        Task<PagedEntity<ChannelInfo>> GetChannelInfosAsync(BaseFilter filter);
    }
}
