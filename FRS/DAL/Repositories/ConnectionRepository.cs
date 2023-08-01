using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;

namespace DAL.Repositories
{
    public class ConnectionRepository : Repository<Connection>, IConnectionRepository
    {
        public ConnectionRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<Connection> GetByIdAsync(string connectionId)
        {
            return await _appContext.Connections.FirstOrDefaultAsync(e => e.ConnectionID == connectionId);
        }

        public IEnumerable<Connection> All()
        {
            return GetAll()
                .OrderBy(c => c.ConnectionID)
                .ToList();
        }

        public async Task<List<Connection>> GetConnectionsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<Connection> query = _appContext.Connections;

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.ConnectionID);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var q = await query.ToListAsync();

            return q.OrderBy(r => r.ConnectionID).ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(Connection connection)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(connection);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save connection!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Connection connection)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == connection.Id);

            f.CopyFrom(connection);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save connection!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> DeleteAsync(string connectionId)
        {
            var result = new BaseOperationResponse();
            var connection = await GetSingleOrDefaultAsync(r => r.ConnectionID == connectionId);

            if (connection != null)
                return await Delete(connection);

            result.IsSuccess = false;
            result.Message = "Connection not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Connection connection)
        {
            var result = new BaseOperationResponse();
            SoftDelete(connection);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete connection!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
