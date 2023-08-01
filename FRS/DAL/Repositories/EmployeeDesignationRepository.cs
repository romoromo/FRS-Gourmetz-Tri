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
    public class EmployeeDesignationRepository : Repository<EmployeeDesignation>, IEmployeeDesignationRepository
    {
        public EmployeeDesignationRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<EmployeeDesignation> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<EmployeeDesignation> All()
        {
            return GetAll()
                .OrderBy(c => c.Code)
                .ToList();
        }

        public async Task<List<EmployeeDesignation>> GetEmployeeDesignationsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<EmployeeDesignation> query = _appContext.EmployeeDesignation
                .Where(e => e.IsActive)
                .OrderBy(r => r.Code);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(EmployeeDesignation employeeDesignation)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Code == employeeDesignation.Code).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await AddAsync(employeeDesignation);
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

        public async Task<BaseOperationResponse> UpdateAsync(EmployeeDesignation employeeDesignation)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Code == employeeDesignation.Code && e.Id != employeeDesignation.Id).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await GetSingleOrDefaultAsync(e => e.Id == employeeDesignation.Id);

            f.CopyFrom(employeeDesignation);
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


        public async Task<BaseOperationResponse> DeleteAsync(int employeeDesignationId)
        {
            var result = new BaseOperationResponse();
            var employeeDesignation = await GetSingleOrDefaultAsync(r => r.Id == employeeDesignationId);

            if (employeeDesignation != null)
                return await Delete(employeeDesignation);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(EmployeeDesignation employeeDesignation)
        {
            var result = new BaseOperationResponse();
            SoftDelete(employeeDesignation);
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

        public async Task<int> GetOrCreateByCode(EmployeeDesignation data)
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
