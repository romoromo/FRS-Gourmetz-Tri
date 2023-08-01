using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IMealTypeRepository
    {
        Task<BaseOperationResponse> CreateAsync(MealType mealType);
        Task<BaseOperationResponse> Delete(MealType mealType);
        Task<BaseOperationResponse> DeleteAsync(int mealTypeId);
        Task<MealType> GetByIdAsync(int id);
        Task<PagedEntity<MealType>> GetMealTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(MealType mealType);
    }
}