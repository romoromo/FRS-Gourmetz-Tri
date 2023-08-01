using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ICuisineRepository
    {
        Task<BaseOperationResponse> CreateAsync(Cuisine cuisine);
        Task<BaseOperationResponse> Delete(Cuisine cuisine);
        Task<BaseOperationResponse> DeleteAsync(int cuisineId);
        Task<Cuisine> GetByIdAsync(int id);
        Task<PagedEntity<Cuisine>> GetCuisinesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(Cuisine cuisine);
    }
}