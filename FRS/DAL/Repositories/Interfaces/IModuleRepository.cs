using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IModuleRepository : IRepository<Module>
    {
        Task<Module> GetByCodeAsync(string code);
        IEnumerable<Module> All();
        Task<List<Module>> GetModulesLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> GetApiModules(int? moduleId = null);
        Task<BaseOperationResponse> CreateAsync(Module module);
        Task<bool> TestCanDeleteAsync(int moduleId);
        Task<BaseOperationResponse> DeleteAsync(int moduleId);
        Task<Module> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Module module);
    }
}
