using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMapRepository : IRepository<Map>
    {
        IEnumerable<Map> All();
        Task<List<Map>> GetMapsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(Map map);
        Task<BaseOperationResponse> DeleteAsync(int mapId);
        Task<Map> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Map map);
        Task<List<Line>> GetConnectors(int pointId);
        Task<List<Point>> GetFloorConnectors(int? mapId);
        Task<BaseOperationResponse> GetRoute(int startDirectoryId, int destDirectoryId, bool wheelchair, bool sheltered);
        Task<BaseOperationResponse> GetRouteTest(int startDirectoryId, int destDirectoryId, bool wheelchair, bool sheltered);
        Map GetMapByFloorId(int id);
        Point GetPointById(int id);
    }
}
