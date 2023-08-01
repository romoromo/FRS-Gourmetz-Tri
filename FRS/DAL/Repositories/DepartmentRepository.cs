using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;

namespace DAL.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public DepartmentRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Department>> GetDepartmentsAsync(BaseFilter filter)
        {
            IQueryable<Department> query = _appContext.Departments
                .Include(e => e.Institution);

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<Department>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion
        public async Task<Department> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Department> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<List<Department>> GetDepartmentsLoadRelatedAsync(int page, int pageSize, int? institutionId = null, string institutionCode = null)
        {
            IQueryable<Department> query = _appContext.Departments
                .Include(e => e.Institution)
                .Where(e => e.IsActive);

            if (institutionId.HasValue)
            {
                query = query.Where(e => e.InstitutionId == institutionId);
            }
            else if (!string.IsNullOrEmpty(institutionCode))
            {
                query = query.Where(e => e.Institution.Name == institutionCode);
            }

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.Name);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles.OrderBy(r => r.Name).ToList();
        }

        public virtual Task<Department> GetByCode(int institutionId, string code)
        {
            return _entities.FirstOrDefaultAsync(e => e.InstitutionId == institutionId && e.Name.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<BaseOperationResponse> CreateAsync(Department department)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(department);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save department!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Department department)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == department.Id);

            f.CopyFrom(department);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save department!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int departmentId)
        {
            return !await _appContext.Users.AnyAsync(e => e.IsActive && e.DepartmentId == departmentId) &&
                !await _appContext.ContactGroupDepartments.AnyAsync(e => e.IsActive && e.DepartmentId == departmentId);
        }


        public async Task<BaseOperationResponse> DeleteAsync(int departmentId)
        {
            var result = new BaseOperationResponse();
            var department = await GetSingleOrDefaultAsync(r => r.Id == departmentId);

            if (department != null)
                return await Delete(department);

            result.IsSuccess = false;
            result.Message = "Department not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Department department)
        {
            var result = new BaseOperationResponse();
            SoftDelete(department);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete department!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
