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
    public class InstitutionRepository : Repository<Institution>, IInstitutionRepository
    {
        public InstitutionRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<Institution> GetDefaultInstitutionAsync()
        {
            return await _appContext.Institutions
                .FirstOrDefaultAsync(e => e.IsActive && e.IsDefault);
        }

        public async Task<Institution> GetByCodeAsync(string code)
        {
            return await _appContext.Institutions
                .SingleOrDefaultAsync(e => e.IsActive && (e.Name.Equals(code, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<Institution> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Institution> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<BaseOperationResponse> GetApiInstitutions(int? institutionId = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<Institution> query = _appContext.Institutions
                .Where(e => e.IsActive && (!institutionId.HasValue || e.Id == institutionId));

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

        public async Task<List<Institution>> GetInstitutionsLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<Institution> query = _appContext.Institutions.Where(e => e.IsActive);

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

        public async Task<BaseOperationResponse> CreateAsync(Institution institution)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(institution);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save institution!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Institution institution)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == institution.Id);

            f.CopyFrom(institution);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save institution!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int institutionId)
        {
            //TODO: add correct logic here, for now prohibit deletion of institution
            return false;//!await _appContext..AnyAsync(e => e == institutionId);
        }


        public async Task<BaseOperationResponse> DeleteAsync(int institutionId)
        {
            var result = new BaseOperationResponse();
            var institution = await GetSingleOrDefaultAsync(r => r.Id == institutionId);

            if (institution != null)
                return await Delete(institution);

            result.IsSuccess = false;
            result.Message = "Institution not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Institution institution)
        {
            var result = new BaseOperationResponse();
            SoftDelete(institution);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete institution!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
