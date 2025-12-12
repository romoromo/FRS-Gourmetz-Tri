using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface ISortingAreaRepository
    {
        Task<BaseOperationResponse> CreateAsync(SortingArea model);
        Task<BaseOperationResponse> Delete(SortingArea model);
        Task<BaseOperationResponse> DeleteAsync(int id);
        Task<SortingArea> GetByIdAsync(int id);
        Task<PagedEntity<SortingArea>> GetAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(SortingArea model);
    }
}
