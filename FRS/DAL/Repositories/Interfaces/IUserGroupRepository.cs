using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IUserGroupRepository : IRepository<UserGroup>
    {
        Task<PagedEntity<UserGroup>> GetUserGroupsAsync(BaseFilter filter);
        IEnumerable<UserGroup> All();
        Task<BaseOperationResponse> GetApiUserGroups(int? userGroupId = null, int? institutionId = null);
        Task<List<UserGroup>> GetUserGroupsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(UserGroup userGroup);
        Task<BaseOperationResponse> DeleteAsync(int userGroupId);
        Task<UserGroup> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(UserGroup userGroup);
        Task<bool> TestCanDeleteAsync(int locationId);
        Task<List<UserGroup>> GetUserGroupsByLocationId(int? locationId);
        Task<List<UserGroup>> GetActiveUserGroups();
    }
}
