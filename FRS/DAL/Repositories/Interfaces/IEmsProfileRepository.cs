using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IEmsProfileRepository : IRepository<EmsProfile>
    {
        IEnumerable<EmsProfile> All();
        Task<List<EmsProfile>> GetEmsProfilesLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> CreateAsync(EmsProfile obj);
        Task<BaseOperationResponse> DeleteAsync(int id);
        Task<EmsProfile> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(EmsProfile obj);
        Task<EmsProfile> GetByCodeAsync(string code);
    }
}
