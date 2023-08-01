using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IQueueTableMapRepository : IRepository<QueueTableMap>
    {
        IEnumerable<QueueTableMap> All();
        Task<List<QueueTableMap>> GetQueueTableMapsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(QueueTableMap queueTableMap);
        Task<BaseOperationResponse> DeleteAsync(int queueTableMapId);
        Task<QueueTableMap> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(QueueTableMap queueTableMap);
        Task<BaseOperationResponse> CallQueue(QueueLog param);
        Task<List<QueueTableMap>> DeviceLists(QueueLog param);
        Task QueueReturn(Dictionary<string, string> data);
    }
}
