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
    public class MenuGroupRepository : Repository<MenuGroup>, IMenuGroupRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public MenuGroupRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<MenuGroup>> GetMenuGroupsAsync(BaseFilter filter)
        {
            IQueryable<MenuGroup> query = _appContext.MenuGroups;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion

        public async Task<List<DishCycle>> GetMenuGroupDishCyclesAsync(int studentId, DateTime orderDate)
        {
            var student = await _appContext.Students.FirstOrDefaultAsync(e => e.Id == studentId);

            var menuGroupDishCycles = _appContext.MenuGroups.Where(e => e.IsActive && e.IsPublished &&
                                            orderDate.Date >= e.StartDate.Date && orderDate.Date <= e.EndDate &&
                                            e.Classes.Any(f => f.ClassId == student.ClassId));

            var dishCycles = menuGroupDishCycles.SelectMany(e => e.MenuGroupDishCycles).Select(e => e.DishCycle).ToList();

            return dishCycles;
        }

        public async Task<List<MenuGroup>> GetActiveMenuGroupDishCyclesAsync(int studentId)
        {
            var student = await _appContext.Students.FirstOrDefaultAsync(e => e.Id == studentId);

            var menuGroupDishCycles = _appContext.MenuGroups.Where(e => e.IsActive && e.IsPublished &&
                                            DateTime.Now <= e.EndDate &&
                                            e.Classes.Any(f => f.ClassId == student.ClassId));

            //var dishCycles = menuGroupDishCycles.SelectMany(e => e.MenuGroupDishCycles.Where(m => m.IsActive && m.DishCycle.IsActive)).Select(e => e.DishCycle).ToList();

            return menuGroupDishCycles.ToList();
        }

        public async Task<List<DishByDate>> GetMenuGroupDishesByDateAsync(int studentId, DateTime startDate, DateTime endDate)
        {
            var student = await _appContext.Students.FirstOrDefaultAsync(e => e.Id == studentId);
            var list = new List<DishByDate>();

            while (startDate.Date <= endDate.Date)
            {
                var menuGroupDishCycles = _appContext.MenuGroups.Where(e => e.IsActive && e.IsPublished &&
                                            startDate.Date >= e.StartDate.Date && startDate.Date <= e.EndDate &&
                                            e.Classes.Any(f => f.ClassId == student.ClassId));

                var dishCycles = menuGroupDishCycles.SelectMany(e => e.MenuGroupDishCycles).Select(e => e.DishCycle).ToList();
                var dishSets = dishCycles.SelectMany(e => e.Sets).ToList();
                var details = dishCycles.SelectMany(e => e.Schedules).SelectMany(e => e.Details).SelectMany(e => e.Menus).ToList();
                list.Add(new DishByDate
                {
                    Date = startDate.Date,
                    DishSets = dishCycles.SelectMany(e => e.Sets).ToList(),
                    Menus = details,
                    DishCycles = dishCycles
                });

                startDate = startDate.AddDays(1);
            }

            return list;
        }

        public async Task<MenuGroup> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(MenuGroup MenuGroup)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(MenuGroup);
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

        public async Task<BaseOperationResponse> UpdateAsync(MenuGroup menuGroup)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == menuGroup.Id);
            


            //delete old MenuGroup dish cycles
            var selectedDishCycles = menuGroup.MenuGroupDishCycles.Select(a => new MenuGroupDishCycle { MenuGroupId = a.MenuGroupId, DishCycleId = a.DishCycleId });
            var dishCycles = f.MenuGroupDishCycles;//this._appContext.MenuGroupDishCycles.Where(e => e.MenuGroupId == menuGroup.Id);

            
            var toBeDeletedDC = dishCycles.Where(e => !selectedDishCycles.Any(x => x.MenuGroupId == e.MenuGroupId && x.DishCycleId == e.DishCycleId));
            //f.MenuGroupDishCycles = f.MenuGroupDishCycles.Where(e => !toBeDeletedDC.Any(x => x.DishCycleId == e.DishCycleId)).ToList();

            //this._appContext.MenuGroupDishCycles.RemoveRange(toBeDeletedDC);

            var toBeAddedDC = selectedDishCycles.Where(x => !dishCycles.Any(e => e.MenuGroupId == x.MenuGroupId && e.DishCycleId == x.DishCycleId));

            foreach (var d in toBeDeletedDC.ToList())
            {
                f.MenuGroupDishCycles.Remove(d);
            }

            foreach (var d in toBeAddedDC.ToList())
            {
                f.MenuGroupDishCycles.Add(d);
            }

            //toBeAddedDC.ToList().ForEach(e =>
            //{
            //    //this._appContext.MenuGroupDishCycles.AddAsync(new MenuGroupDishCycle { Id = menuGroup.Id, DishCycleId = e.DishCycleId });
            //    //f.MenuGroupDishCycles.Add(new MenuGroupDishCycle { Id = menuGroup.Id, DishCycleId = e.DishCycleId });
            //    dishCycles.Add(new MenuGroupDishCycle { Id = menuGroup.Id, DishCycleId = e.DishCycleId });
            //});

            var selectedClasses = menuGroup.Classes.Select(a => new MenuGroupClass { MenuGroupId = a.MenuGroupId, ClassId = a.ClassId });
            var classes = f.Classes; // this._appContext.MenuGroupClasses.Where(e => e.MenuGroupId == menuGroup.Id);

            var toBeDeletedC = classes.Where(e => !selectedClasses.Any(x => x.MenuGroupId == e.MenuGroupId && x.ClassId == e.ClassId));
            //f.Classes = f.Classes.Where(e => !toBeDeletedC.Any(x => x.ClassId == e.ClassId)).ToList();


            //this._appContext.MenuGroupClasses.RemoveRange(toBeDeletedC);
            foreach(var d in toBeDeletedC.ToList())
            {
                f.Classes.Remove(d);
            }

            var toBeAddedC = selectedClasses.Where(x => !classes.Any(e => e.MenuGroupId == x.MenuGroupId && e.ClassId == x.ClassId));

            foreach (var d in toBeAddedC.ToList())
            {
                f.Classes.Add(d);
            }

            //toBeAddedC.ToList().ForEach(e =>
            //{
            //    this._appContext.MenuGroupClasses.AddAsync(new MenuGroupClass { Id = menuGroup.Id, ClassId = e.ClassId });
            //    //f.Classes.Add(new MenuGroupClass { Id = menuGroup.Id, ClassId = e.ClassId });
            //});


            f.CopyFrom(menuGroup);
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


        public async Task<BaseOperationResponse> DeleteAsync(int menuGroupId)
        {
            var result = new BaseOperationResponse();
            var menuGroup = await GetSingleOrDefaultAsync(r => r.Id == menuGroupId);

            if (menuGroup != null)
                return await Delete(menuGroup);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(MenuGroup menuGroup)
        {
            var result = new BaseOperationResponse();
            SoftDelete(menuGroup);
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
