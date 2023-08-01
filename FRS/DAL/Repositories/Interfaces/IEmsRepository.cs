using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IEmsRepository : IRepository<Ems>
    {
        Task<Ems> GetByCodeAsync(string code);
        IEnumerable<Ems> All();
        Task<List<Ems>> GetEmsesLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> GetApiEmses(int? emsId = null);
        Task<BaseOperationResponse> CreateAsync(Ems ems);
        Task<bool> TestCanDeleteAsync(int emsId);
        Task<BaseOperationResponse> DeleteAsync(int emsId);
        Task<Ems> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Ems ems);
    }
}
