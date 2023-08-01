using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;

namespace DAL.Repositories
{
    public class UserGroupRepository : Repository<UserGroup>, IUserGroupRepository
    {
        private ISieveProcessor _sieveProcessor;
        public UserGroupRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
        }

        #region Sieved
        public async Task<PagedEntity<UserGroup>> GetUserGroupsAsync(BaseFilter filter)
        {
            IQueryable<UserGroup> query = _appContext.UserGroups
                .Include(e => e.Institution);

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<UserGroup>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion

        public async Task<UserGroup> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<UserGroup> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<BaseOperationResponse> GetApiUserGroups(int? userGroupId = null, int? institutionId = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<UserGroup> query = _appContext.UserGroups
                    .Include(e => e.Institution)
                    .Where(e => e.IsActive && (!userGroupId.HasValue || e.Id == userGroupId)
                                 && (!institutionId.HasValue || e.InstitutionId == institutionId));

                response.Data = (await query.ToListAsync())
                    .OrderBy(r => r.Name).ToList();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<List<UserGroup>> GetUserGroupsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<UserGroup> query = _appContext.UserGroups
                .Where(e => e.IsActive && (!institutionId.HasValue || e.InstitutionId == institutionId));

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.Name);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles
                .OrderBy(r => r.Name).ToList();
        }

        public async Task<List<UserGroup>> GetUserGroupsByLocationId(int? locationId)
        {
            if (locationId == null) return new List<UserGroup>();

            IQueryable<UserGroup> query = _appContext.UserGroups
                .Where(e => e.IsActive && e.Locations.Any(l => l.LocationId == locationId && l.IsActive));

            var roles = await query.ToListAsync();

            return roles
                .OrderBy(r => r.Name).ToList();
        }

        public async Task<List<UserGroup>> GetActiveUserGroups()
        {
            IQueryable<UserGroup> query = _appContext.UserGroups
                .Where(e => e.IsActive);

            var roles = await query.ToListAsync();

            return roles
                .OrderBy(r => r.Name).ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(UserGroup userGroup)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(userGroup);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save user group!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(UserGroup userGroup)
        {
            var result = new BaseOperationResponse();

            var f = await _appContext.UserGroups
                .Where(e => e.Id == userGroup.Id)
                .SingleOrDefaultAsync();

            if (userGroup.Locations != null)
            {
                var entitiesToDelete = this._appContext.UserGroupLocations.Where(e => e.UserGroupId == userGroup.Id);

                foreach (var toDelete in entitiesToDelete)
                {
                    toDelete.IsActive = false;
                    this._appContext.UserGroupLocations.Update(toDelete);
                }

                foreach (var parameter in userGroup.Locations)
                {
                    var existingEntity = this._appContext.UserGroupLocations.FirstOrDefault(e => e.UserGroupId == userGroup.Id && e.LocationId == parameter.LocationId);

                    if (existingEntity == null)
                    {
                        this._appContext.UserGroupLocations.Add(parameter);
                    }
                    else
                    {
                        existingEntity.IsActive = true;
                        this._appContext.UserGroupLocations.Update(existingEntity);
                    }
                }
            }

            f.CopyFrom(userGroup);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save user group!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int id)
        {
            return !await _appContext.UserGroupMembers
                .AnyAsync(e => e.UserGroupId == id);
        }


        public async Task<BaseOperationResponse> DeleteAsync(int userGroupId)
        {
            var result = new BaseOperationResponse();
            var userGroup = await GetSingleOrDefaultAsync(r => r.Id == userGroupId);

            if (userGroup != null)
                return await Delete(userGroup);

            result.IsSuccess = false;
            result.Message = "UserGroup not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(UserGroup userGroup)
        {
            var result = new BaseOperationResponse();
            SoftDelete(userGroup);
            //delete location userGroups
            var userGroups = _appContext.UserGroupMembers.Where(e => e.UserGroupId == userGroup.Id);
            _appContext.UserGroupMembers.RemoveRange(userGroups);

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete user group type!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
