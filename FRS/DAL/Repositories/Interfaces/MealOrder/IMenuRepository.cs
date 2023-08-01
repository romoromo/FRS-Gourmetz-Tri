using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IMenuRepository
    {
        Task<BaseOperationResponse> CreateAsync(Menu Menu);
        Task<BaseOperationResponse> Delete(Menu Menu);
        Task<BaseOperationResponse> DeleteAsync(int MenuId);
        Task<Menu> GetByIdAsync(int id);
        Task<MenuDishMealType> GetMenuDishMealTypesByIdAsync(int id, int page, int pageSize);
        Task<PagedEntity<Menu>> GetMenusAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(Menu Menu);
    }
}