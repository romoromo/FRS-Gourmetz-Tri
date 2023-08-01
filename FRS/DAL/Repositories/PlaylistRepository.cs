using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using DAL.Core.DTO;
using Microsoft.Extensions.Configuration;
using System.DirectoryServices;
using System.ComponentModel.DataAnnotations;

namespace DAL.Repositories
{
    public class PlaylistRepository : Repository<Playlist>, IPlaylistRepository
    {
        private readonly IConfiguration _configuration;
        public PlaylistRepository(IConfiguration configuration, ApplicationDbContext context) : base(context)
        {
            _configuration = configuration;
        }


        public async Task<Playlist> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Playlist> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<List<Playlist>> GetPlaylistsLoadRelatedAsync(int page, int pageSize, int? institutionId = null, int? userId = null, List<int> imageIds = null)
        {
            IQueryable<Playlist> query = _appContext.Playlists
                .Include(e => e.PlaylistImages)
                .Where(e => e.IsActive && (!institutionId.HasValue || e.InstitutionId == institutionId));

            if (imageIds != null && imageIds.Any())
            {
                query = query.Where(e => e.PlaylistImages.Any(f => imageIds.Any(x => x == f.ImageId)));
            }

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.Name);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles.OrderBy(r => r.Name).ToList();
        }

        //public virtual Task<Playlist> GetByCode(int imageId, string code)
        //{
        //    return _entities.Include(e => e.ContactGroupDepartments)
        //        .FirstOrDefaultAsync(e => e.ContactGroupDepartments.Any(f => f.DepartmentId == departmentId) && e.Name.Equals(code, StringComparison.OrdinalIgnoreCase));
        //}

        public async Task<BaseOperationResponse> CreateAsync(Playlist playlist)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(playlist);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save playlist!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Playlist playlist)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == playlist.Id);

            var piToDelete = this._appContext.PlaylistImages.Where(e => e.PlaylistId == playlist.Id &&
                !playlist.PlaylistImages.Any(r => r.ImageId == e.ImageId));

            foreach (var toDelete in piToDelete)
            {
                toDelete.IsActive = false;
                this._appContext.PlaylistImages.Update(toDelete);
            }

            foreach (var toInsert in playlist.PlaylistImages)
            {
                var cg = await this._appContext.PlaylistImages.FirstOrDefaultAsync(r => r.PlaylistId == playlist.Id &&
                    r.ImageId == toInsert.ImageId);
                if (cg == null)
                {
                    await this._appContext.PlaylistImages.AddAsync(toInsert);
                }
                else
                {
                    cg.CopyFrom(toInsert);
                    cg.IsActive = true;
                    this._appContext.PlaylistImages.Update(cg);
                }
            }

            f.CopyFrom(playlist);
            Update(f);

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save playlist!";
                result.IsSuccess = false;
            }

            return result;
        }


        //public async Task<TestDeleteResult> TestCanDeleteAsync(int contactGroupId)
        //{
        //    var result = new TestDeleteResult();
        //    result.IsDeletable = true;
        //    if (await _appContext.ContactGroupMembers.AnyAsync(e => e.ContactGroupId == contactGroupId && e.IsActive))
        //    {
        //        result.IsDeletable = false;
        //        result.Message = "Remove all members from this contact group and try again.";
        //    }

        //    if(result.IsDeletable && await _appContext.ReservationContactGroups.AnyAsync(e => e.ContactGroupId == contactGroupId && e.IsActive))
        //    {
        //        result.IsDeletable = false;
        //        result.Message = "This contact group is being used by an event/booking.";
        //    }

        //    return result;
        //}


        public async Task<BaseOperationResponse> DeleteAsync(int playlistId)
        {
            var result = new BaseOperationResponse();
            var playlist = await GetSingleOrDefaultAsync(r => r.Id == playlistId);

            if (playlist != null)
                return await Delete(playlist);

            result.IsSuccess = false;
            result.Message = "Playlist not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Playlist playlist)
        {
            var result = new BaseOperationResponse();
            SoftDelete(playlist);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete playlist!";
                result.IsSuccess = false;
            }

            return result;
        }


        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
