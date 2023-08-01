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
    public class EmsProfileRepository : Repository<EmsProfile>, IEmsProfileRepository
    {
        public EmsProfileRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<EmsProfile> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<EmsProfile> GetByCodeAsync(string code)
        {
            return await GetSingleOrDefaultAsync(p => p.Code == code);
        }

        public async Task<List<EmsProfile>> GetEmsProfilesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<EmsProfile> query = _appContext.EmsProfiles
                .Where(e => e.IsActive)
                .OrderBy(r => r.Code);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public IEnumerable<EmsProfile> All()
        {
            return GetAll()
                .OrderBy(c => c.Code)
                .ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(EmsProfile emsProfile)
        {
            var result = new BaseOperationResponse();
            if (Exists(e => e.Code == emsProfile.Code).Result)
            {
                result.Message = "EmsProfile already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await AddAsync(emsProfile);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save ems profile!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(EmsProfile emsProfile)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == emsProfile.Id);
            if (Exists(e => e.Id != f.Id && e.Code == emsProfile.Code).Result)
            {
                result.Message = "EmsProfile already exists!";
                result.IsSuccess = false;
            }
            else
            {
                f.CopyFrom(emsProfile);
                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save ems profile type!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int emsProfileId)
        {
            var result = new BaseOperationResponse();
            var emsProfile = await GetSingleOrDefaultAsync(r => r.Id == emsProfileId);

            if (emsProfile != null)
                return await Delete(emsProfile);

            result.IsSuccess = false;
            result.Message = "EmsProfile not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(EmsProfile emsProfile)
        {
            var result = new BaseOperationResponse();
            SoftDelete(emsProfile);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete ems profile!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
