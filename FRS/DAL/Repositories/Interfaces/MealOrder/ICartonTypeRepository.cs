using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ICartonTypeRepository
    {
        Task<BaseOperationResponse> CreateAsync(CartonType data);
        Task<BaseOperationResponse> Delete(CartonType data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<CartonType> GetByIdAsync(int id);
        Task<PagedEntity<CartonType>> GetCartonTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(CartonType data);
    }
}