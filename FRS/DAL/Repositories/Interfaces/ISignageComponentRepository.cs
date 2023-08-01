using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface ISignageComponentRepository : IRepository<SignageComponent>
    {
        IEnumerable<SignageComponent> All();
        Task<List<SignageComponent>> GetSignageComponentsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(SignageComponent signageComponent);
        Task<BaseOperationResponse> DeleteAsync(int signageComponentId);
        Task<SignageComponent> GetByIdAsync(int id);
        Task<SignageComponent> GetByCodeAsync(string code);
        Task<BaseOperationResponse> UpdateAsync(SignageComponent signageComponent);
        Task<List<SignageComponent>> GetSignageComponents(string ids);
    }
}
