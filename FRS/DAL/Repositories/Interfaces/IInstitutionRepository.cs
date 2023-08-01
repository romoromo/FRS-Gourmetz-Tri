using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IInstitutionRepository : IRepository<Institution>
    {
        Task<Institution> GetDefaultInstitutionAsync();
        Task<Institution> GetByCodeAsync(string code);
        IEnumerable<Institution> All();
        Task<List<Institution>> GetInstitutionsLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> GetApiInstitutions(int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(Institution institution);
        Task<bool> TestCanDeleteAsync(int institutionId);
        Task<BaseOperationResponse> DeleteAsync(int institutionId);
        Task<Institution> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Institution institution);
    }
}
