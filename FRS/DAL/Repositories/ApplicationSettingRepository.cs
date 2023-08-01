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
    public class ApplicationSettingRepository : Repository<ApplicationSetting>, IApplicationSettingRepository
    {
        public ApplicationSettingRepository(ApplicationDbContext context) : base(context)
        { }

        public Task<ApplicationSetting> GetByKeyAsync(string key, int? institutionId = null)
        {
            return GetSingleOrDefaultAsync(e => e.IsActive && e.Key == key && (!institutionId.HasValue || e.InstitutionId == institutionId));
        }

        public async Task<ApplicationSetting> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<ApplicationSetting> All()
        {
            return GetAll()
                .OrderBy(c => c.Key)
                .ToList();
        }

        public async Task<List<ApplicationSetting>> GetApplicationSettingsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<ApplicationSetting> query = _appContext.ApplicationSettings
                .Include(e => e.Institution)
                .Where(e => !institutionId.HasValue || e.InstitutionId == institutionId && e.IsActive)
                .OrderBy(r => r.Key);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(ApplicationSetting applicationSetting)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(applicationSetting);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save application setting!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(ApplicationSetting applicationSetting)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == applicationSetting.Id);

            f.CopyFrom(applicationSetting);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save application setting!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int applicationSettingId)
        {
            return true;//TODO: 
        }


        public async Task<BaseOperationResponse> DeleteAsync(int applicationSettingId)
        {
            var result = new BaseOperationResponse();
            var applicationSetting = await GetSingleOrDefaultAsync(r => r.Id == applicationSettingId);

            if (applicationSetting != null)
                return await Delete(applicationSetting);

            result.IsSuccess = false;
            result.Message = "Application Setting not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(ApplicationSetting applicationSetting)
        {
            var result = new BaseOperationResponse();
            SoftDelete(applicationSetting);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete application setting!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
