using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IRouteRepository
    {
        Task<BaseOperationResponse> CreateAsync(Route route);
        Task<BaseOperationResponse> Delete(Route route);
        Task<BaseOperationResponse> DeleteAsync(int routeId);
        Task<Route> GetByIdAsync(int id);
        Task<PagedEntity<Route>> GetRoutesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(Route route, List<RouteNode> nodes);
    }
}