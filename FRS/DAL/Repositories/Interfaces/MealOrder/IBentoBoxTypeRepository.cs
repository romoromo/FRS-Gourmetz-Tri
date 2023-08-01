using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IBentoBoxTypeRepository
    {
        Task<BaseOperationResponse> CreateAsync(BentoBoxType data);
        Task<BaseOperationResponse> Delete(BentoBoxType data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<BentoBoxType> GetByIdAsync(int id);
        Task<PagedEntity<BentoBoxType>> GetBentoBoxTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(BentoBoxType data);
    }
}