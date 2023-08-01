using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ICartonDisposableBoxRepository
    {
        Task<BaseOperationResponse> CreateAsync(CartonDisposableBox data);
        Task<BaseOperationResponse> Delete(CartonDisposableBox data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<CartonDisposableBox> GetByIdAsync(int id);
        Task<PagedEntity<CartonDisposableBox>> GetDisposableBoxesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(CartonDisposableBox data);

        Task<BaseOperationResponse> ResetAsync();
        Task<BaseOperationResponse> ResetDishAsync(CartonDisposableBox data);
    }
}