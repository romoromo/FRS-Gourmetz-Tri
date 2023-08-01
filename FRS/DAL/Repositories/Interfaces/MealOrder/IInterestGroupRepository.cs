using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IInterestGroupRepository
    {
        Task<BaseOperationResponse> CreateAsync(InterestGroup InterestGroup);
        Task<BaseOperationResponse> Delete(InterestGroup InterestGroup);
        Task<BaseOperationResponse> DeleteAsync(int InterestGroupId);
        Task<InterestGroup> GetByIdAsync(int id);
        Task<PagedEntity<InterestGroup>> GetInterestGroupsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(InterestGroup InterestGroup);
    }
}