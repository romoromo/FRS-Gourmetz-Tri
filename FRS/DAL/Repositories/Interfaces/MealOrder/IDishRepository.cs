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
    public interface IDishRepository
    {
        Task<BaseOperationResponse> CreateAsync(Dish Dish);
        Task<BaseOperationResponse> Delete(Dish Dish);
        Task<BaseOperationResponse> DeleteAsync(int DishId);
        Task<Dish> GetByIdAsync(int id);
        Task<string> GenerateCode(int id);
        Task<PagedEntity<Dish>> GetDishesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(Dish Dish);
        Task<List<Dish>> GetDishChangesAsync(DateTime updatedAfter, DateTime? updatedBefore);
        Task<PagedEntity<DishLiteDTO>> GetDishesLiteAsync(BaseFilter filter);
    }
}