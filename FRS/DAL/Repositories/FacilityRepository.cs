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
    public class FacilityRepository : Repository<Facility>, IFacilityRepository
    {
        public FacilityRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<Facility> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Facility> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<BaseOperationResponse> GetApiFacilities(int? facilityId = null, int? institutionId = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<Facility> query = _appContext.Facilities
                    .Include(e => e.Icon)
                    .Include(e => e.Institution)
                    .Where(e => e.IsActive && (!facilityId.HasValue || e.Id == facilityId)
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

        public async Task<List<Facility>> GetFacilitiesLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<Facility> query = _appContext.Facilities
                .Include(e => e.Icon)
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

        public async Task<BaseOperationResponse> CreateAsync(Facility facility, string filePath = null)
        {
            var result = new BaseOperationResponse();

            if (!string.IsNullOrEmpty(filePath))
            {
                facility.Icon = new File
                {
                    Path = filePath,
                    FileName = System.IO.Path.GetFileName(filePath),
                    Type = FileType.Icon.ToString()
                };
            }

            var f = await AddAsync(facility);
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

        public async Task<BaseOperationResponse> UpdateAsync(Facility facility, string filePath = null)
        {
            var result = new BaseOperationResponse();

            var f = await _appContext.Facilities
                .Where(e => e.Id == facility.Id)
                .SingleOrDefaultAsync();

            if (!string.IsNullOrEmpty(filePath))
            {
                if (f.Icon == null)
                {
                    //TODO: check why EF Core is not loading the Icon property; interim solution
                    var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == f.FileId);
                    if (icon == null)
                    {
                        f.Icon = new File();
                    }
                    else
                    {
                        f.Icon = icon;
                        f.FileId = icon.Id;
                    }
                }

                f.Icon.Path = filePath;
                f.Icon.FileName = System.IO.Path.GetFileName(filePath);
                f.Icon.Type = FileType.Icon.ToString();
            }

            var oldFileId = f.FileId;
            f.CopyFrom(facility);
            f.FileId = oldFileId;
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


        public async Task<bool> TestCanDeleteAsync(int id)
        {
            return !await _appContext.LocationFacilities
                .AnyAsync(e => e.IsActive && e.FacilityId == id);
        }


        public async Task<BaseOperationResponse> DeleteAsync(int facilityId)
        {
            var result = new BaseOperationResponse();
            var facility = await GetSingleOrDefaultAsync(r => r.Id == facilityId);

            if (facility != null)
                return await Delete(facility);

            result.IsSuccess = false;
            result.Message = "Facility not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Facility facility)
        {
            var result = new BaseOperationResponse();
            SoftDelete(facility);
            //delete location facilities
            var locationFacilities = _appContext.LocationFacilities.Where(e => e.FacilityId == facility.Id);
            await locationFacilities.ForEachAsync(e => { e.IsActive = false; });
            _appContext.LocationFacilities.UpdateRange(locationFacilities);

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
