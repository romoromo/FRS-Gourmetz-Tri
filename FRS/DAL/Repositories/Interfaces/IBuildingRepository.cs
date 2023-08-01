using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IBuildingRepository : IRepository<Building>
    {
        IEnumerable<Building> All();
        Task<List<Building>> GetBuildingsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(Building building);
        Task<BaseOperationResponse> DeleteAsync(int buildingId);
        Task<Building> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Building building);
    }
}
