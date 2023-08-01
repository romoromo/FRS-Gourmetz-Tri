using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface ISignageCompilationRepository : IRepository<SignageCompilation>
    {
        IEnumerable<SignageCompilation> All();
        Task<List<SignageCompilation>> GetSignageCompilationsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(SignageCompilation signageCompilation);
        Task<BaseOperationResponse> DeleteAsync(int signageCompilationId);
        Task<SignageCompilation> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(SignageCompilation signageCompilation);
        Task<SignageCompilation> GetByCodeAsync(string code);
        Task<List<SignageCompilation>> GetSignageCompilations(string ids);
        Task<BaseOperationResponse> AddOrUpdateCompilationComponentAsync(SignageCompilationComponent signageCompilationComponent);
    }
}
