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
    public class MediaRepository : Repository<Media>, IMediaRepository
    {
        public MediaRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<Media> GetByCodeAsync(string code)
        {
            return await _appContext.Medias
                .SingleOrDefaultAsync(e => e.IsActive && (e.Name.Equals(code, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<Media> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Media> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<BaseOperationResponse> GetApiMedias(int? mediaId = null, string name = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<Media> query = _appContext.Medias.Include(e => e.Institution)
                .Where(e => e.IsActive);

                if (mediaId.HasValue)
                {
                    query = query.Where(e => e.Id == mediaId);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(e => e.Name.ToLower() == name.ToLower());
                }

                response.Data = await query.OrderBy(e => e.Name).ToListAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<List<Media>> GetMediasLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<Media> query = _appContext.Medias.Include(e => e.Institution).Where(e => e.IsActive)
                .OrderBy(r => r.Name);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(Media media, string[] roles, string[] userGroups)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(media);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save media!";
                result.IsSuccess = false;
            }

            if (roles != null)
            {
                var newIds = _appContext.Roles.Where(u => roles.Contains(u.Name)).Select(u => u.Id);

                var userToAdd = newIds.ToList();

                if (userToAdd.Any()) userToAdd.ForEach(uid =>
                {
                    var ur = new MediaRole()
                    {
                        RoleId = uid,
                        MediaId = f.Id
                    };

                    ur.IsActive = true;

                    _appContext.MediaRole.Add(ur);
                });

                _appContext.SaveChanges();
            }

            if (userGroups != null)
            {
                var newIds = _appContext.UserGroups.Where(u => userGroups.Contains(u.Name)).Select(u => u.Id);

                var userToAdd = newIds.ToList();

                if (userToAdd.Any()) userToAdd.ForEach(uid =>
                {
                    var ur = new MediaUserGroup()
                    {
                        UserGroupId = uid,
                        MediaId = f.Id
                    };

                    ur.IsActive = true;

                    _appContext.MediaUserGroup.Add(ur);
                });

                _appContext.SaveChanges();
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Media media, string[] roles, string[] userGroups)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == media.Id);

            f.CopyFrom(media);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save media!";
                result.IsSuccess = false;
            }

            if (roles != null)
            {
                var newIds = _appContext.Roles.Where(u => roles.Contains(u.Name)).Select(u => u.Id);
                var existingUsers = f.Roles.Where(u => u.IsActive).Select(u => u.RoleId.Value);

                var userToRemove = existingUsers.Except(newIds).Distinct().ToList();
                var userToAdd = newIds.Except(existingUsers).Distinct().ToList();

                if (userToRemove.Any()) userToRemove.ForEach(uid =>
                {
                    var ur = _appContext.MediaRole.FirstOrDefault(u => u.MediaId == f.Id && u.RoleId == uid);
                    if (ur != null)
                    {
                        ur.IsActive = false;
                        _appContext.MediaRole.Update(ur);
                    }
                });

                if (userToAdd.Any()) userToAdd.ForEach(uid =>
                {
                    var ur = f.Roles.FirstOrDefault(r => r.RoleId == uid) ?? new MediaRole()
                    {
                        RoleId = uid,
                        MediaId = f.Id
                    };

                    ur.IsActive = true;

                    if (ur.Id > 0) _appContext.MediaRole.Update(ur);
                    else _appContext.MediaRole.Add(ur);
                });

                _appContext.SaveChanges();
            }

            if (userGroups != null)
            {
                var newIds = _appContext.UserGroups.Where(u => userGroups.Contains(u.Name)).Select(u => u.Id);
                var existingUsers = f.UserGroups.Where(u => u.IsActive).Select(u => u.UserGroupId.Value);

                var userToRemove = existingUsers.Except(newIds).Distinct().ToList();
                var userToAdd = newIds.Except(existingUsers).Distinct().ToList();

                if (userToRemove.Any()) userToRemove.ForEach(uid =>
                {
                    var ur = _appContext.MediaUserGroup.FirstOrDefault(u => u.MediaId == f.Id && u.UserGroupId == uid);
                    if (ur != null)
                    {
                        ur.IsActive = false;
                        _appContext.MediaUserGroup.Update(ur);
                    }
                });

                if (userToAdd.Any()) userToAdd.ForEach(uid =>
                {
                    var ur = f.UserGroups.FirstOrDefault(r => r.UserGroupId == uid) ?? new MediaUserGroup()
                    {
                        UserGroupId = uid,
                        MediaId = f.Id
                    };

                    ur.IsActive = true;

                    if (ur.Id > 0) _appContext.MediaUserGroup.Update(ur);
                    else _appContext.MediaUserGroup.Add(ur);
                });

                _appContext.SaveChanges();
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int mediaId)
        {
            //TODO: add correct logic here, for now prohibit deletion of media
            return false;//!await _appContext..AnyAsync(e => e == mediaId);
        }

        public async Task<bool> TestCanCreateAsync(string name)
        {
            var media = await GetSingleOrDefaultAsync(e => e.IsActive && e.Name.ToLower() == name.ToLower());// Find(e => e.IsActive && e.Name.ToLower() == name.ToLower()).FirstOrDefault();
            return media == null;
        }

        public async Task<BaseOperationResponse> DeleteAsync(int mediaId)
        {
            var result = new BaseOperationResponse();
            var media = await GetSingleOrDefaultAsync(r => r.Id == mediaId);

            if (media != null)
                return await Delete(media);

            result.IsSuccess = false;
            result.Message = "Media not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Media media)
        {
            var result = new BaseOperationResponse();
            SoftDelete(media);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete media!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
