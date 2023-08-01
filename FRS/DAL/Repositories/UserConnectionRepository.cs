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
    public class UserConnectionRepository : Repository<UserConnection>, IUserConnectionRepository
    {
        public UserConnectionRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<UserConnection> GetByUserIdAsync(int id)
        {
            return await _appContext.UserConnections.FirstOrDefaultAsync(e => e.UserId == id);
        }

        public async Task<UserConnection> GetByIdAsync(string connectionId)
        {
            return await _appContext.UserConnections.FirstOrDefaultAsync(e => e.ConnectionID == connectionId);
        }

        public IEnumerable<UserConnection> All()
        {
            return GetAll()
                .OrderBy(c => c.ConnectionID)
                .ToList();
        }

        public async Task<List<UserConnection>> GetUserConnectionsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<UserConnection> query = _appContext.UserConnections;

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.UpdatedDate);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var q = await query.ToListAsync();

            return q.OrderBy(r => r.UpdatedDate).ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(UserConnection connection)
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

        public async Task<BaseOperationResponse> UpdateAsync(UserConnection connection)
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

        public async Task<BaseOperationResponse> UpdateRangeStatusAsync(List<UserConnection> connections, string status)
        {
            var result = new BaseOperationResponse();

            //just update all records
            foreach (var connection in connections)
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == connection.Id);

                f.CopyFrom(connection);
                Update(f);

                connection.Status = status;
                _appContext.UserConnections.Update(connection);
            }

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to save!";
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
            result.Message = "UserConnection not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(UserConnection connection)
        {
            var result = new BaseOperationResponse();
            //just retain the last status used
            //connection.Status = UserConnectionStatus.OFFLINE.ToString();
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
