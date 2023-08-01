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
    public class KioskSettingsRepository : Repository<KioskSettings>, IKioskSettingsRepository
    {
        private readonly IConfiguration _configuration;
        public KioskSettingsRepository(IConfiguration configuration, ApplicationDbContext context) : base(context)
        {
            _configuration = configuration;
        }


        public async Task<KioskSettings> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<KioskSettings> All()
        {
            return GetAll()
                .OrderBy(c => c.label)
                .ToList();
        }

        public async Task<List<KioskSettings>> GetKioskSettingsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<KioskSettings> query = _appContext.KioskSettings
                .Where(e => e.IsActive);


            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.label);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles.OrderBy(r => r.label).ToList();
        }

        //public virtual Task<Playlist> GetByCode(int imageId, string code)
        //{
        //    return _entities.Include(e => e.ContactGroupDepartments)
        //        .FirstOrDefaultAsync(e => e.ContactGroupDepartments.Any(f => f.DepartmentId == departmentId) && e.Name.Equals(code, StringComparison.OrdinalIgnoreCase));
        //}

        public async Task<BaseOperationResponse> CreateAsync(KioskSettings kioskSettings)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(kioskSettings);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save kiosk settings!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(KioskSettings kioskSettings)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == kioskSettings.Id);

            f.CopyFrom(kioskSettings);
            Update(f);

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save kiosk settings!";
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


        public async Task<BaseOperationResponse> DeleteAsync(int kioskSettingsId)
        {
            var result = new BaseOperationResponse();
            var kioskSettings = await GetSingleOrDefaultAsync(r => r.Id == kioskSettingsId);

            if (kioskSettings != null)
                return await Delete(kioskSettings);

            result.IsSuccess = false;
            result.Message = "Kiosk Settings not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(KioskSettings kioskSettings)
        {
            var result = new BaseOperationResponse();
            SoftDelete(kioskSettings);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete kiosk settings!";
                result.IsSuccess = false;
            }

            return result;
        }


        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
