using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IDishTypeRepository
    {
        Task<BaseOperationResponse> CreateAsync(DishType dishType);
        Task<BaseOperationResponse> Delete(DishType dishType);
        Task<BaseOperationResponse> DeleteAsync(int dishTypeId);
        Task<DishType> GetByIdAsync(int id);
        Task<PagedEntity<DishType>> GetDishTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(DishType dishType);
    }
}