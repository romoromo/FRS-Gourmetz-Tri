using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IEmailQueueRepository : IRepository<EmailQueue>
    {
        Task<BaseOperationResponse> CreateAsync(EmailQueue assetType);
        Task<BaseOperationResponse> DeleteAsync(int assetTypeId);
        Task<EmailQueue> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(EmailQueue assetType);
        Task<PagedEntity<EmailQueue>> GetEmailQueuesAsync(BaseFilter filter);
        Task<List<EmailQueue>> GetAllUnsentEmailAsync();
    }
}
