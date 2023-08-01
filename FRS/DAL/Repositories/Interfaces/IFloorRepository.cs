using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IFloorRepository : IRepository<Floor>
    {
        IEnumerable<Floor> All();
        Task<List<Floor>> GetFloorsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(Floor floor);
        Task<BaseOperationResponse> DeleteAsync(int floorId);
        Task<Floor> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Floor floor);
    }
}
