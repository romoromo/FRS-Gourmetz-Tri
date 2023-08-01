using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IEmployeeDesignationRepository : IRepository<EmployeeDesignation>
    {
        IEnumerable<EmployeeDesignation> All();
        Task<List<EmployeeDesignation>> GetEmployeeDesignationsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(EmployeeDesignation employeeDesignation);
        Task<BaseOperationResponse> DeleteAsync(int employeeDesignationId);
        Task<EmployeeDesignation> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(EmployeeDesignation employeeDesignation);
    }
}
