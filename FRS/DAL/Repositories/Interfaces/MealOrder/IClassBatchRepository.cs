using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IClassBatchRepository
    {
        Task<BaseOperationResponse> CreateAsync(ClassBatch classBatch);
        Task<BaseOperationResponse> Delete(ClassBatch classBatch);
        Task<BaseOperationResponse> DeleteAsync(int classBatchId);
        Task<ClassBatch> GetByIdAsync(int id);
        Task<PagedEntity<ClassBatch>> GetClassBatchesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(ClassBatch classBatch);
    }
}