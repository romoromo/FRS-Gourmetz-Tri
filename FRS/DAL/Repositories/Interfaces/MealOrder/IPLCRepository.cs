using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IPLCRepository
    {
        Task<BaseOperationResponse> CreateAsync(PLCModel model);
        Task<BaseOperationResponse> DeleteAsync(int id);
        Task<PLCModel> GetByIdAsync(int id);
        Task<PagedEntity<PLCModel>> GetPaged(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(PLCModel model);
    }
}
