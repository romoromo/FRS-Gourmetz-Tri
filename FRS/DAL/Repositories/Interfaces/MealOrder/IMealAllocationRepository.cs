using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IMealAllocationRepository
    {
        Task<BaseOperationResponse> CreateAsync(MealAllocation allocation, List<TokenLabel> tokens);
        Task<BaseOperationResponse> Delete(MealAllocation allocation);
        Task<BaseOperationResponse> DeleteAsync(int allocationId);
        Task<MealAllocation> GetByIdAsync(int id);
        //Task<BaseOperationResponse> ImportAsync(IAccountManager accountManager, List<StudentImportDTO> dto);
        Task<PagedEntity<MealAllocation>> GetMealAllocationsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(MealAllocation allocation, List<TokenLabel> tokens);
        IQueryable<MealAllocation> GetKioskOrderDish(MealAllocationAdditionalDishFilter filter);
        Task<List<MealAllocation>> GetMealAllocations(int outletId, DateTime orderDate); 
    }
}