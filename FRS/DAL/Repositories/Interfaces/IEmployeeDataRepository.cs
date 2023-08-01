using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IEmployeeDataRepository : IRepository<EmployeeData>
    {
        IEnumerable<EmployeeData> All();
        Task<List<EmployeeData>> GetEmployeeDatasLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(EmployeeData employeeData);
        Task<BaseOperationResponse> DeleteAsync(int employeeDataId);
        Task<EmployeeData> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(EmployeeData employeeData);
        Task<List<EmployeeData>> GetCurrentEmployeesByLocation(int? locationId = null);
    }
}
