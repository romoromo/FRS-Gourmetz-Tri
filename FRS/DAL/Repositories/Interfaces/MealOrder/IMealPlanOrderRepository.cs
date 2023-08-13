using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IMealPlanOrderRepository
    {
        Task<BaseOperationResponse> CreateAsync(MealPlanOrder mealPlanOrder);
        Task<BaseOperationResponse> Delete(MealPlanOrder mealPlanOrder);
        Task<BaseOperationResponse> DeleteAsync(int mealPlanOrderId);
        Task<MealPlanOrder> GetByIdAsync(int id);
        Task<PagedEntity<MealPlanOrder>> GetMealPlanOrdersAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(MealPlanOrder mealPlanOrder);
    }
}