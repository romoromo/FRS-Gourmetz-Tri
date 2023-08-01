using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IOccupancyLogRepository : IRepository<OccupancyLog>
    {
        IEnumerable<OccupancyLog> All();
        Task<List<OccupancyLog>> GetOccupancyLogsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(OccupancyLog occupancyLog);
        Task<BaseOperationResponse> DeleteAsync(int occupancyLogId);
        Task<OccupancyLog> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(OccupancyLog occupancyLog);
        Task<List<OccupancyLog>> GetOccupancyLogsFilter(DateTime? startTime, DateTime? endTime, string deviceId, string sensorId, string status);
        Task<string> GetLastStatus(string deviceId, string sensorId);
    }
}
