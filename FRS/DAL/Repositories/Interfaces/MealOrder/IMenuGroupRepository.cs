using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IMenuGroupRepository
    {
        Task<BaseOperationResponse> CreateAsync(MenuGroup MenuGroup);
        Task<BaseOperationResponse> Delete(MenuGroup MenuGroup);
        Task<BaseOperationResponse> DeleteAsync(int MenuGroupId);
        Task<MenuGroup> GetByIdAsync(int id);
        Task<PagedEntity<MenuGroup>> GetMenuGroupsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(MenuGroup MenuGroup);
        Task<List<DishCycle>> GetMenuGroupDishCyclesAsync(int studentId, DateTime orderDate);
        Task<List<MenuGroup>> GetActiveMenuGroupDishCyclesAsync(int studentId);
        Task<List<DishByDate>> GetMenuGroupDishesByDateAsync(int studentId, DateTime startDate, DateTime endDate);
    }
}