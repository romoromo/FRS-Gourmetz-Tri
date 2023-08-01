using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IMealSessionDetailRepository
    {
        Task<BaseOperationResponse> CreateAsync(MealSessionDetail mealSessionDetail);
        Task<BaseOperationResponse> Delete(MealSessionDetail mealSessionDetail);
        Task<BaseOperationResponse> DeleteAsync(int mealSessionDetailId);
        Task<MealSessionDetail> GetByIdAsync(int id);
        Task<PagedEntity<MealSessionDetail>> GetMealSessionDetailsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(MealSessionDetail mealSessionDetail);
    }
}