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

namespace DAL.Repositories.MealOrder
{
    public class MenuCycleRepository : Repository<MenuCycle>, IMenuCycleRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public MenuCycleRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<MenuCycle>> GetMenuCyclesAsync(BaseFilter filter)
        {
            IQueryable<MenuCycle> query = _appContext.MenuCycles;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<MenuCycle> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(MenuCycle menuCycle)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(menuCycle);
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

        public async Task<BaseOperationResponse> UpdateAsync(MenuCycle menuCycle)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == menuCycle.Id);

            //remove menus

            //remove schedules
            var selectedDays = menuCycle.Schedules.Select(a => a.Day);
            var cycleSchedules = this._appContext.MenuCycleSchedules.Where(e => e.MenuCycleId == menuCycle.Id);

            cycleSchedules.ToList().ForEach(x => {
                var schedulePeriods = this._appContext.MenuCycleSchedulePeriods.Where(e => e.MenuCycleScheduleId == x.Id);
                this._appContext.MenuCycleSchedulePeriods.RemoveRange(schedulePeriods);

            });

            //var toBeDeleted = cycleSchedules.Where(e => !selectedDays.Contains(e.Day));
            this._appContext.MenuCycleSchedules.RemoveRange(cycleSchedules);

            //add new schedule
            await this._appContext.MenuCycleSchedules.AddRangeAsync(menuCycle.Schedules);

            f.CopyFrom(menuCycle);

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


        public async Task<BaseOperationResponse> DeleteAsync(int menuCycleId)
        {
            var result = new BaseOperationResponse();
            var menuCycle = await GetSingleOrDefaultAsync(r => r.Id == menuCycleId);

            if (menuCycle != null)
                return await Delete(menuCycle);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(MenuCycle menuCycle)
        {
            var result = new BaseOperationResponse();
            SoftDelete(menuCycle);
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

        public async Task<BaseOperationResponse> BlockMenuCycleDate(List<MenuCycleBlockedDate> blockedDates)
        {
            var result = new BaseOperationResponse();

            await _appContext.MenuCycleBlockedDates.AddRangeAsync(blockedDates);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully blocked the date!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to block the date!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UnblockMenuCycleDate(List<MenuCycleBlockedDate> blockedDates)
        {
            var result = new BaseOperationResponse();

            var toRemove = _appContext.MenuCycleBlockedDates.Where(e => blockedDates.Any(f => f.MenuCycleId == e.MenuCycleId && e.EffectiveDate == f.EffectiveDate)); 
            _appContext.MenuCycleBlockedDates.RemoveRange(toRemove);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully unblocked the date!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to unblock the date!";
                result.IsSuccess = false;
            }

            return result;
        }

        #region Sieved
        public async Task<List<MenuCycleSchedulePeriod>> GetMenuCycleSchedulePeriods(int menuCycleId, int day)
        {
            IQueryable<MenuCycleSchedulePeriod> query = _appContext.MenuCycleSchedulePeriods.Where(e => e.IsActive && 
                                                                        e.MenuCycleSchedule.MenuCycleId == menuCycleId &&
                                                                        e.MenuCycleSchedule.Day == day);

            return await query.OrderBy(e => e.MealPeriod.Sequence).ToListAsync();
        }

        public async Task<BaseOperationResponse> CreateSchedulePeriodMenus(List<MenuCycleSchedulePeriod> periods)
        {
            var result = new BaseOperationResponse();

            bool isModified = false;
            foreach (var period in periods)
            {
                var menus = _appContext.MenuCycleSchedulePeriodMenus.Where(e => 
                                    e.MenuCycleSchedulePeriod.MenuCycleScheduleId == period.MenuCycleScheduleId &&
                                    e.MenuCycleSchedulePeriod.MealPeriodId == period.MealPeriodId);

                var mealPeriodSchedule = await _appContext.MenuCycleSchedulePeriods.FirstOrDefaultAsync(e => e.MenuCycleScheduleId == period.MenuCycleScheduleId &&
                                    e.MealPeriodId == period.MealPeriodId);
                var toAddRange = period.Menus.Where(e => !menus.Any(f => f.MenuId == e.MenuId));
                toAddRange.ToList().ForEach(e => {
                    e.MenuCycleSchedulePeriodId = mealPeriodSchedule.Id;
                    _appContext.MenuCycleSchedulePeriodMenus.AddAsync(e);
                });
                
                var toRemove = menus.Where(e => !period.Menus.Any(f => e.MenuId == f.MenuId));
                if (!isModified)
                {
                    isModified = toAddRange.Any() || toRemove.Any();
                }

                _appContext.MenuCycleSchedulePeriodMenus.RemoveRange(toRemove);
            }
            
            
            if (!isModified || await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        #endregion


        public async Task<List<MenuCycle>> GetOutletMenuCyclesAsync(int outletId, int catererId)
        {
            var outlet = await _appContext.CatererOutlets.FirstOrDefaultAsync(e => e.OutletId == outletId && e.CatererInfoId == catererId);

            IQueryable<MenuCycle> query = _appContext.MenuCycles.Where(e => e.OutletProfileId == outlet.OutletProfileId);

            return await query.ToListAsync();
        }

        public async Task<BaseOperationResponse> BlockOutletDate(List<OutletBlockedDate> blockedDates)
        {
            var result = new BaseOperationResponse();

            await _appContext.OutletBlockedDates.AddRangeAsync(blockedDates);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully blocked the date!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to block the date!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UnblockOutletDate(List<OutletBlockedDate> blockedDates)
        {
            var result = new BaseOperationResponse();

            var toRemove = _appContext.OutletBlockedDates.Where(e => blockedDates.Any(f => f.OutletId == e.OutletId && f.MenuCycleId == e.MenuCycleId && e.EffectiveDate == f.EffectiveDate));
            _appContext.OutletBlockedDates.RemoveRange(toRemove);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully unblocked the date!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to unblock the date!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> CreateOutletSchedulePeriodMenus(List<MenuCycleSchedulePeriod> periods)
        {
            var result = new BaseOperationResponse();

            foreach (var period in periods)
            {
                var menus = _appContext.OutletMenuCycleSchedulePeriodMenus.Where(e =>
                                    e.MenuCycleSchedulePeriod.MenuCycleScheduleId == period.MenuCycleScheduleId &&
                                    e.MenuCycleSchedulePeriod.MealPeriodId == period.MealPeriodId);

                var mealPeriodSchedule = await _appContext.MenuCycleSchedulePeriods.FirstOrDefaultAsync(e => e.MenuCycleScheduleId == period.MenuCycleScheduleId &&
                                    e.MealPeriodId == period.MealPeriodId);
                var toAddRange = period.OutletMenus.Where(e => !menus.Any(f => f.MenuId == e.MenuId));
                toAddRange.ToList().ForEach(e => {
                    e.MenuCycleSchedulePeriodId = mealPeriodSchedule.Id;
                    _appContext.OutletMenuCycleSchedulePeriodMenus.AddAsync(e);
                });

                var toRemove = menus.Where(e => !period.OutletMenus.Any(f => e.MenuId == f.MenuId));
                _appContext.OutletMenuCycleSchedulePeriodMenus.RemoveRange(toRemove);

                var menuDishes = _appContext.OutletMenuDishes.Where(e =>
                                    e.MenuCycleSchedulePeriod.MenuCycleScheduleId == period.MenuCycleScheduleId &&
                                    e.MenuCycleSchedulePeriod.MealPeriodId == period.MealPeriodId);

                var menuDishToAddRange = period.OutletMenuDishes.Where(e => !menuDishes.Any(f => f.MenuId == e.MenuId && f.DishId == e.DishId
                                        && f.MealTypeId == e.MealTypeId && f.MenuId == e.MenuId));
                menuDishToAddRange.ToList().ForEach(e => {
                    e.MenuCycleSchedulePeriodId = mealPeriodSchedule.Id;
                    _appContext.OutletMenuDishes.AddAsync(e);
                });

                var menuDishToRemove = menuDishes.Where(e => !period.OutletMenuDishes.Any(f => f.MenuId == e.MenuId && f.DishId == e.DishId
                                        && f.MealTypeId == e.MealTypeId && f.MenuId == e.MenuId));
                _appContext.OutletMenuDishes.RemoveRange(menuDishToRemove);

                //update menu dishes
                //var excludedDishes = period.OutletMenus.SelectMany(f => f.Menu.OutletMenuDishes);
                //var dishesToRemove = _appContext.OutletMenuDishes.Where(e => !excludedDishes.Any(f => f.OutletId == e.OutletId && f.MealTypeId == e.MealTypeId && e.MenuId == f.MenuId && f.DishId == e.DishId));
                //_appContext.OutletMenuDishes.RemoveRange(dishesToRemove);

                //var oldExcludedDishes = menus.SelectMany(f => f.Menu.OutletMenuDishes);
                //var dishesToAdd = _appContext.OutletMenuDishes.Where(e => !oldExcludedDishes.Any(f => f.OutletId == e.OutletId && f.MealTypeId == e.MealTypeId && e.MenuId == f.MenuId && f.DishId == e.DishId));
                //_appContext.OutletMenuDishes.AddRange(dishesToAdd);
            }


            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }

            return result;
        }

        public async Task<List<MenuCycleSchedulePeriod>> GetOutletMenuCycleSchedulePeriods(int outletId, int catererId, int menuCycleId, int day)
        {
            var outlet = await _appContext.CatererOutlets.FirstOrDefaultAsync(e => e.OutletId == outletId && e.CatererInfoId == catererId);
            var menuCycle = outlet.OutletProfile.MenuCycles.FirstOrDefault(e => e.Id == menuCycleId);
            var menuCycleSchedule = menuCycle.Schedules.FirstOrDefault(e => e.Day == day);
            //IQueryable<MenuCycleSchedulePeriod> query = _appContext.MenuCycleSchedulePeriods.Where(e => e.IsActive &&
            //                                                            e.MenuCycleSchedule.MenuCycleId == menuCycleId &&
            //                                                            e.MenuCycleSchedule.Day == day &&
            //                                                            e.MenuCycleSchedule.MenuCycle.OutletProfile.Outlets.Any(x => x.Id == outletId));
            
            var results = new List<MenuCycleSchedulePeriod>();
            var periods = menuCycle.OutletProfile.MealPeriods.OrderBy(e => e.Sequence).ToList();
            periods.ForEach(e =>
            {
                var schedulePeriod = new MenuCycleSchedulePeriod();
                var period = menuCycleSchedule.Periods.FirstOrDefault(f => f.MealPeriodId == e.Id);
                schedulePeriod.MealPeriod = e;
                schedulePeriod.MealPeriodId = e.Id;
                schedulePeriod.MenuCycleScheduleId = menuCycleSchedule.Id;
                schedulePeriod.Menus = period?.Menus;
                schedulePeriod.OutletMenus = period?.OutletMenus;
                schedulePeriod.OutletMenuDishes = period?.OutletMenuDishes;
                //var outletScheduleMenus = new List<OutletMenuCycleSchedulePeriodMenu>();
                //foreach(var menu in e.Menus)
                //{
                //    outletScheduleMenus.AddRange(_appContext.OutletMenuCycleSchedulePeriodMenus
                //                .Where(f => f.OutletId == outletId && f.MenuId == menu.MenuId &&
                //                        f.MenuCycleSchedulePeriod.MealPeriodId == e.MealPeriodId &&
                //                        f.MenuCycleSchedulePeriod.MenuCycleSchedule.Day == day).ToList());
                //}

                //e.OutletMenus = outletScheduleMenus;
                results.Add(schedulePeriod);
            });

            return results;
        }

        #region Student Calendar
        public async Task<List<Student>> GetStudentsByOutletAsync(int outletId)
        {
            return await _appContext.Students.Where(e => e.OutletId == outletId).ToListAsync();
        }

        public async Task<Student> GetStudentAsync(int studentId)
        {
            return await _appContext.Students.FirstOrDefaultAsync(e => e.Id == studentId);
        }

        public async Task<List<OutletClassRosterSchedule>> GetStudentMenuCyclesAsync(int studentId)
        {
            //var outlet = await _appContext.CatererOutlets.FirstOrDefaultAsync(e => e.OutletId == outletId && e.CatererInfoId == catererId);
            var student = await _appContext.Students.FirstOrDefaultAsync(e => e.Id == studentId);
            IQueryable<OutletClassRosterSchedule> query = _appContext.OutletClassRosters.Where(e => e.IsActive).SelectMany(e => e.Schedules);
            //query = query.Where(e => e.IsActive && e.Periods.SelectMany(f => f.Classes).Any(x => x.ClassId == student.ClassId));
            return await query.ToListAsync();
        }

        public async Task<List<OutletClassRosterSchedule>> GetStudentsMenuCyclesAsync(List<int> studentIds)
        {
            var student = await _appContext.Students.FirstOrDefaultAsync(e => studentIds.Contains(e.Id));
            IQueryable<OutletClassRosterSchedule> query = _appContext.OutletClassRosters.Where(e => e.IsActive).SelectMany(e => e.Schedules);
            return await query.ToListAsync();
        }

        #endregion

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
