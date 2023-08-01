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
    public class BuildingRepository : Repository<Building>, IBuildingRepository
    {
        public BuildingRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<Building> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Building> All()
        {
            return GetAll()
                .OrderBy(c => c.Code)
                .ToList();
        }

        public async Task<List<Building>> GetBuildingsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<Building> query = _appContext.Building
                .Where(e => e.IsActive)
                .OrderBy(r => r.Code);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(Building building)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(building);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Building building)
        {
            var result = new BaseOperationResponse();

            building.Floors.ToList().ForEach(cSource =>
            {
                if (cSource.IsActive || cSource.Id > 0)
                {
                    var ct = _appContext.BuildingFloor.FirstOrDefault(c => cSource.BuildingId == building.Id && cSource.FloorId == c.FloorId) ?? new BuildingFloor();
                    ct.CopyFrom(cSource);
                    ct.BuildingId = building.Id;
                    _appContext.BuildingFloor.Update(ct);
                    _appContext.SaveChanges();
                }
            });

            var f = await GetSingleOrDefaultAsync(e => e.Id == building.Id);

            f.CopyFrom(building);
            Update(f);

            if (await _appContext.SaveChangesAsync() > 0)
            {
                

                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int buildingId)
        {
            var result = new BaseOperationResponse();
            var building = await GetSingleOrDefaultAsync(r => r.Id == buildingId);

            if (building != null)
                return await Delete(building);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Building building)
        {
            var result = new BaseOperationResponse();
            SoftDelete(building);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
