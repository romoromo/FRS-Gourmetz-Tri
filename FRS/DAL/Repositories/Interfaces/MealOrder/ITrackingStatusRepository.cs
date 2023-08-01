using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ITrackingStatusRepository
    {
        Task<BaseOperationResponse> CreateAsync(TrackingStatus data);
        Task<BaseOperationResponse> Delete(TrackingStatus data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<TrackingStatus> GetByIdAsync(int id);
        Task<PagedEntity<TrackingStatus>> GetTrackingStatussAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(TrackingStatus data);
    }
}