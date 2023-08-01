using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IConnectionRepository : IRepository<Connection>
    {
        IEnumerable<Connection> All();
        Task<List<Connection>> GetConnectionsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(Connection connection);
        Task<BaseOperationResponse> DeleteAsync(string connectionId);
        Task<Connection> GetByIdAsync(string connectionId);
        Task<BaseOperationResponse> UpdateAsync(Connection connection);
    }
}
