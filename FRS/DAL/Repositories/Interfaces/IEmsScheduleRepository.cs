using DAL.Core;
using DAL.Models;
using Ical.Net.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IEmsScheduleRepository : IRepository<EmsSchedule>
    {
        IEnumerable<EmsSchedule> All();
        Task<List<EmsSchedule>> GetEmsSchedulesLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> CreateAsync(EmsSchedule obj);
        Task<BaseOperationResponse> DeleteAsync(int id);
        Task<EmsSchedule> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(EmsSchedule obj);

        Task<List<RSchedule>> GetRecSchedulesAsync(DateTime? start, DateTime? end);
    }
}
