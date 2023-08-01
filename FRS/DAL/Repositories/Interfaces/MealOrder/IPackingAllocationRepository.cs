using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IPackingAllocationRepository
    {
        Task<BaseOperationResponse> CreateAsync(PackingAllocation allocation, List<DishAllocation> dishes);
        Task<BaseOperationResponse> Delete(PackingAllocation allocation);
        Task<BaseOperationResponse> DeleteAsync(int allocationId);
        Task<PackingAllocation> GetByIdAsync(int id);
        //Task<BaseOperationResponse> ImportAsync(IAccountManager accountManager, List<StudentImportDTO> dto);
        Task<PagedEntity<PackingAllocation>> GetPackingAllocationsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(PackingAllocation allocation, List<DishAllocation> dishes);
    }
}