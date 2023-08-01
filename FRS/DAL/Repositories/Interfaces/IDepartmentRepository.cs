using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        IEnumerable<Department> All();
        Task<List<Department>> GetDepartmentsLoadRelatedAsync(int page, int pageSize, int? institutionId = null, string institutionCode = null);
        Task<BaseOperationResponse> CreateAsync(Department facility);
        Task<bool> TestCanDeleteAsync(int departmentId);
        Task<BaseOperationResponse> DeleteAsync(int departmentId);
        Task<Department> GetByIdAsync(int id);
        Task<Department> GetByCode(int institutionId, string code);
        Task<BaseOperationResponse> UpdateAsync(Department department);
        Task<PagedEntity<Department>> GetDepartmentsAsync(BaseFilter filter);
    }
}
