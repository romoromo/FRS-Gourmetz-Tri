using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IServiceContractRepository : IRepository<ServiceContract>
    {
        Task<BaseOperationResponse> CreateAsync(ServiceContract serviceContract);
        Task<BaseOperationResponse> DeleteAsync(int serviceContractId);
        Task<ServiceContract> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(ServiceContract serviceContract);
        Task<PagedEntity<ServiceContract>> GetServiceContractsAsync(BaseFilter filter);
    }
}
