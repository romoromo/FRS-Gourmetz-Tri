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
    public class FloorRepository : Repository<Floor>, IFloorRepository
    {
        public FloorRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<Floor> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Floor> All()
        {
            return GetAll()
                .OrderBy(c => c.Code)
                .ToList();
        }

        public async Task<List<Floor>> GetFloorsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<Floor> query = _appContext.Floor
                .Where(e => e.IsActive)
                .OrderBy(r => r.Code);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(Floor floor)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(floor);
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

        public async Task<BaseOperationResponse> UpdateAsync(Floor floor)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == floor.Id);

            f.CopyFrom(floor);
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


        public async Task<BaseOperationResponse> DeleteAsync(int floorId)
        {
            var result = new BaseOperationResponse();
            var floor = await GetSingleOrDefaultAsync(r => r.Id == floorId);

            if (floor != null)
                return await Delete(floor);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Floor floor)
        {
            var result = new BaseOperationResponse();
            SoftDelete(floor);
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

        public async Task<int> GetOrCreateByCode(Floor data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Code)) return 0;

            var f = await GetSingleOrDefaultAsync(c => c.Code == data.Code && c.IsActive);

            if (f == null)
            {
                await CreateAsync(data);
                f = await GetSingleOrDefaultAsync(c => c.Code == data.Code && c.IsActive);
            }

            return f.Id;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
