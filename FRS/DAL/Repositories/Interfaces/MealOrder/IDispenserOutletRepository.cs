using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IDispenserOutletRepository
    {
        Task<BaseOperationResponse> CreateAsync(DispenserOutlet dispenserOutlet);
        Task<BaseOperationResponse> Delete(DispenserOutlet dispenserOutlet);
        Task<BaseOperationResponse> DeleteAsync(int dispenserOutletId);
        Task<DispenserOutlet> GetByIdAsync(int id);
        Task<PagedEntity<DispenserOutlet>> GetDispenserOutletsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(DispenserOutlet dispenserOutlet);
    }
}