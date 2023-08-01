using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IFacilityTypeRepository : IRepository<FacilityType>
    {
        IEnumerable<FacilityType> All();
        Task<List<FacilityType>> GetFacilityTypesLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(FacilityType facility);
        Task<bool> TestCanDeleteAsync(int facilityTypeId);
        Task<BaseOperationResponse> DeleteAsync(int facilityTypeId);
        Task<FacilityType> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(FacilityType facilityType);
    }
}
