using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IRestrictionRepository
    {
        Task<BaseOperationResponse> CreateAsync(Restriction Restriction);
        Task<BaseOperationResponse> Delete(Restriction Restriction);
        Task<BaseOperationResponse> DeleteAsync(int RestrictionId);
        Task<Restriction> GetByIdAsync(int id);
        Task<PagedEntity<Restriction>> GetRestrictionsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(Restriction Restriction);
    }
}