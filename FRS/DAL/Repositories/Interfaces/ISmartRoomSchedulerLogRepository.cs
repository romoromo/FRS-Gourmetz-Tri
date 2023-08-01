using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface ISmartRoomSchedulerLogRepository
    {
        Task<BaseOperationResponse> CreateAsync(SmartRoomSchedulerLog smartRoomSchedulerLog);
        Task<BaseOperationResponse> BulkCreateAsync(List<SmartRoomSchedulerLog> smartRoomSchedulerLogs);
        Task<SmartRoomSchedulerLog> GetByIdAsync(int id);
        Task<PagedEntity<SmartRoomSchedulerLog>> GetSmartRoomSchedulerLogsAsync(BaseFilter filter);
    }
}