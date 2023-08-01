using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;

namespace DAL.Repositories.MealOrder
{
    public class RouteRepository : Repository<Route>, IRouteRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public RouteRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Route>> GetRoutesAsync(BaseFilter filter)
        {
            IQueryable<Route> query = _appContext.Routes
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<Route> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(Route route)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(route);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Route route, List<RouteNode> nodes)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == route.Id);

            var nodesToDelete = this._appContext.RouteNodes.Where(x => x.RouteId == f.Id &&
                                   (nodes == null || !nodes.Any(a => a.StoreId == x.StoreId)));

            this._appContext.RouteNodes.RemoveRange(nodesToDelete);

            if (nodes != null)
            {
                nodes.ForEach(e =>
                {
                    var sr = this._appContext.RouteNodes.FirstOrDefault(x => x.RouteId == e.RouteId && x.StoreId == e.StoreId);
                    if (sr != null)
                    {
                        sr.IsActive = true;
                        sr.order = e.order;
                        sr.interval = e.interval;
                        this._appContext.RouteNodes.Update(sr);
                    }
                    else
                    {
                        this._appContext.RouteNodes.Add(e);
                    }
                });
            }

            f.CopyFrom(route);

            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int routeId)
        {
            var result = new BaseOperationResponse();
            var route = await GetSingleOrDefaultAsync(r => r.Id == routeId);

            if (route != null)
                return await Delete(route);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Route route)
        {
            var result = new BaseOperationResponse();
            SoftDelete(route);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
