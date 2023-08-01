using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;
using DAL.Core.DTO;

namespace DAL.Repositories.MealOrder
{
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public MenuRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Menu>> GetMenusAsync(BaseFilter filter)
        {
            IQueryable<Menu> query = _appContext.Menus;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<Menu> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<MenuDishMealType> GetMenuDishMealTypesByIdAsync(int id, int page, int pageSize)
        {
            var menu = await _appContext.Menus.SingleOrDefaultAsync(e => e.Id == id);
            var mealTypes = _appContext.MealTypes.Where(e => e.CatererId == menu.CatererId && e.IsActive).OrderBy(e => e.Name);
            var dishes = _appContext.Dishes.Where(e => e.IsActive && e.IsEnabled && e.CatererId == menu.CatererId).OrderBy(e => e.Label);
            var menuDishes = _appContext.MenuDishes.Where(e => e.MenuId == id);

            var dto = new MenuDishMealType
            {
                Columns = mealTypes.Select(e => new MenuListCol { ColName = e.Name, MealTypeId = e.Id }).ToList(),
                TotalCount = dishes.Count(),
                CurrentPage = page,
                PageSize = pageSize,
                Rows = new List<MenuListRow>()
            };

            IQueryable<Dish> pagedQuery = dishes;

            if(page > -1 && pageSize > -1)
            {
                var skip = (page - 1) * pageSize;
                pagedQuery = pagedQuery.Skip(skip).Take(pageSize);
            }
            

            foreach (var d in pagedQuery)
            {
                var row = new MenuListRow { DishId = d.Id, DishLabel = d.Label, Cells = new List<MenuListCell>() };
                foreach (var mealType in mealTypes)
                {
                    var cell = new MenuListCell
                    {
                        Checked = menuDishes.Any(x => x.MealTypeId == mealType.Id && x.DishId == d.Id),
                        DishId = d.Id,
                        MealTypeId = mealType.Id
                    };

                    row.Cells.Add(cell);
                }

                dto.Rows.Add(row);
            }

            return dto;
        }

        public async Task<BaseOperationResponse> CreateAsync(Menu menu)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(menu);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Menu menu)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == menu.Id);

            //delete old menu dishes
            var selectedDishes = menu.MenuDishes.Select(a => new { MenuId = a.MenuId, DishId = a.DishId, MealTypeId = a.MealTypeId });
            var dishes = this._appContext.MenuDishes.Where(e => e.MenuId == menu.Id);

            var toBeDeleted = dishes.Where(e => !selectedDishes.Any(x => x.MenuId == e.MenuId && x.DishId == e.DishId && x.MealTypeId == e.MealTypeId));
            this._appContext.MenuDishes.RemoveRange(toBeDeleted);

            var toBeAdded = selectedDishes.Where(x => !dishes.Any(e => e.MenuId == x.MenuId && e.DishId == x.DishId && e.MealTypeId == x.MealTypeId));

            toBeAdded.ToList().ForEach(e =>
            {
                this._appContext.MenuDishes.AddAsync(new MenuDish { MenuId = menu.Id, DishId = e.DishId, MealTypeId = e.MealTypeId });
            });

            f.CopyFrom(menu);

            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int menuId)
        {
            var result = new BaseOperationResponse();
            var menu = await GetSingleOrDefaultAsync(r => r.Id == menuId);

            if (menu != null)
                return await Delete(menu);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Menu menu)
        {
            var result = new BaseOperationResponse();
            SoftDelete(menu);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
