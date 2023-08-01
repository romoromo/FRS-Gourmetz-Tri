using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IFacilityRepository : IRepository<Facility>
    {
        IEnumerable<Facility> All();
        Task<BaseOperationResponse> GetApiFacilities(int? facilityId = null, int? institutionId = null);
        Task<List<Facility>> GetFacilitiesLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(Facility facility, string filePath = null);
        Task<BaseOperationResponse> DeleteAsync(int facilityId);
        Task<Facility> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Facility facility, string filePath = null);
        Task<bool> TestCanDeleteAsync(int locationId);
    }
}
