using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IUserConnectionRepository : IRepository<UserConnection>
    {
        IEnumerable<UserConnection> All();
        Task<List<UserConnection>> GetUserConnectionsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(UserConnection connection);
        Task<BaseOperationResponse> DeleteAsync(string connectionId);
        Task<UserConnection> GetByUserIdAsync(int id);
        Task<UserConnection> GetByIdAsync(string connectionId);
        Task<BaseOperationResponse> UpdateAsync(UserConnection connection);
        Task<BaseOperationResponse> UpdateRangeStatusAsync(List<UserConnection> connections, string status);

    }
}
