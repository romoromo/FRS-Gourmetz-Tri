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
    public class EmsRepository : Repository<Ems>, IEmsRepository
    {
        public EmsRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<Ems> GetByCodeAsync(string code)
        {
            return await _appContext.Emses
                .SingleOrDefaultAsync(e => e.IsActive && (e.Name.Equals(code, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<Ems> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Ems> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<BaseOperationResponse> GetApiEmses(int? emsId = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<Ems> query = _appContext.Emses.Include(e => e.Institution)
                .Where(e => e.IsActive && (!emsId.HasValue || e.Id == emsId))
                .OrderBy(r => r.Name);

                response.Data = await query.ToListAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<List<Ems>> GetEmsesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<Ems> query = _appContext.Emses.Include(e => e.Institution).Where(e => e.IsActive)
                .OrderBy(r => r.Name);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(Ems ems)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(ems);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save ems!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Ems ems)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == ems.Id);

            f.CopyFrom(ems);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save ems!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int emsId)
        {
            //TODO: add correct logic here, for now prohibit deletion of ems
            return false;//!await _appContext..AnyAsync(e => e == emsId);
        }


        public async Task<BaseOperationResponse> DeleteAsync(int emsId)
        {
            var result = new BaseOperationResponse();
            var ems = await GetSingleOrDefaultAsync(r => r.Id == emsId);

            if (ems != null)
                return await Delete(ems);

            result.IsSuccess = false;
            result.Message = "Ems not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Ems ems)
        {
            var result = new BaseOperationResponse();
            SoftDelete(ems);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete ems!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
