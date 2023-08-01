using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IMealPeriodRepository
    {
        Task<BaseOperationResponse> CreateAsync(MealPeriod mealPeriod);
        Task<BaseOperationResponse> Delete(MealPeriod mealPeriod);
        Task<BaseOperationResponse> DeleteAsync(int mealPeriodId);
        Task<MealPeriod> GetByIdAsync(int id);
        Task<List<MealPeriod>> GetByOutletIdAsync(int outletProfileId);
        Task<PagedEntity<MealPeriod>> GetMealPeriodsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(MealPeriod mealPeriod);
    }
}