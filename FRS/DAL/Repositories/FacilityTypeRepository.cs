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
    public class FacilityTypeRepository : Repository<FacilityType>, IFacilityTypeRepository
    {
        public FacilityTypeRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<FacilityType> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<FacilityType> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<List<FacilityType>> GetFacilityTypesLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<FacilityType> query = _appContext.FacilityTypes
                .Where(e => !institutionId.HasValue || e.InstitutionId == institutionId && e.IsActive);

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

        public async Task<BaseOperationResponse> CreateAsync(FacilityType facilityType)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(facilityType);
            if(await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save facility type!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(FacilityType facilityType)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == facilityType.Id);

            f.CopyFrom(facilityType);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save facility type!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int facilityTypeId)
        {
            return !await _appContext.LocationFacilityTypes.AnyAsync(e => e.IsActive && e.FacilityTypeId == facilityTypeId);
        }


        public async Task<BaseOperationResponse> DeleteAsync(int facilityTypeId)
        {
            var result = new BaseOperationResponse();
            var facilityType = await GetSingleOrDefaultAsync(r => r.Id == facilityTypeId);

            if (facilityType != null)
                return await Delete(facilityType);

            result.IsSuccess = false;
            result.Message = "Facility Type not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(FacilityType facilityType)
        {
            var result = new BaseOperationResponse();
            SoftDelete(facilityType);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete facility type!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
