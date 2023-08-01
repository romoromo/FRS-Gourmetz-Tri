using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IOutletProfileRepository
    {
        Task<BaseOperationResponse> CreateAsync(OutletProfile data);
        Task<bool> TestDeleteAsync(int dataId);
        Task<BaseOperationResponse> Delete(OutletProfile data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<OutletProfile> GetByIdAsync(int id);
        Task<PagedEntity<OutletProfile>> GetOutletProfilesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(OutletProfile data);
    }
}