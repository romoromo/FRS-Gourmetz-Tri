using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IStaffTypeRepository
    {
        Task<BaseOperationResponse> CreateAsync(StaffType StaffType);
        Task<BaseOperationResponse> Delete(StaffType StaffType);
        Task<BaseOperationResponse> DeleteAsync(int StaffTypeId);
        Task<StaffType> GetByIdAsync(int id);
        Task<PagedEntity<StaffType>> GetStaffTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(StaffType StaffType);
    }
}