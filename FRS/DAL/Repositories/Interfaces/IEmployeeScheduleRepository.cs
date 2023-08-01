using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IEmployeeScheduleRepository : IRepository<EmployeeSchedule>
    {
        IEnumerable<EmployeeSchedule> All();
        Task<List<EmployeeSchedule>> GetEmployeeSchedulesLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(EmployeeSchedule employeeSchedule);
        Task<BaseOperationResponse> DeleteAsync(int employeeScheduleId);
        Task<EmployeeSchedule> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(EmployeeSchedule employeeSchedule);
    }
}
