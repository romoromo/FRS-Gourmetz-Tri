using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IRestrictionTypeRepository
    {
        Task<BaseOperationResponse> CreateAsync(RestrictionType RestrictionType);
        Task<BaseOperationResponse> Delete(RestrictionType RestrictionType);
        Task<BaseOperationResponse> DeleteAsync(int RestrictionTypeId);
        Task<RestrictionType> GetByIdAsync(int id);
        Task<PagedEntity<RestrictionType>> GetRestrictionTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(RestrictionType RestrictionType);
    }
}