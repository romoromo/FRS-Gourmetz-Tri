using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IDriverRepository
    {
        Task<BaseOperationResponse> CreateAsync(Driver driver);
        Task<BaseOperationResponse> Delete(Driver driver);
        Task<BaseOperationResponse> DeleteAsync(int driverId);
        Task<Driver> GetByIdAsync(int id);
        Task<PagedEntity<Driver>> GetDriversAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(Driver driver);
    }
}