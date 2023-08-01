using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IExternalAppLoginLogRepository : IRepository<ExternalAppLoginLog>
    {
        Task<BaseOperationResponse> CreateAsync(ExternalAppLoginLog externalAppLoginLog);
        Task<BaseOperationResponse> DeleteAsync(int externalAppLoginLogId);
        Task<ExternalAppLoginLog> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(ExternalAppLoginLog externalAppLoginLog);
        Task<PagedEntity<ExternalAppLoginLog>> GetExternalAppLoginLogsAsync(BaseFilter filter);
    }
}
