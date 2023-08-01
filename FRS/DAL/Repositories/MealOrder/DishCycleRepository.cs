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
    public class DishCycleRepository : Repository<DishCycle>, IDishCycleRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public DishCycleRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<DishCycle>> GetDishCyclesAsync(BaseFilter filter)
        {
            IQueryable<DishCycle> query = _appContext.DishCycles
                                        .Include(e => e.OutletProfile)
                                        .Include(e => e.OutletProfile.Caterer)
                                        .Include(e => e.OutletProfile.Outlets)
                                        .Include(e => e.OutletProfile.Caterer.CatererOutlets);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion

        public async Task<List<DishCycle>> GetOutletDishCyclesAsync(int outletId, int catererId)
        {
            var outlet = await _appContext.CatererOutlets.FirstOrDefaultAsync(e => e.OutletId == outletId && e.CatererInfoId == catererId);

            IQueryable<DishCycle> query = _appContext.DishCycles.Where(e => e.OutletProfileId == outlet.OutletProfileId);

            return await query.ToListAsync();
        }

        public async Task<List<DishCyclePeriod>> GetOutletDishCyclePeriodsAsync(int dishCyleId)
        {
            return await _appContext.DishCyclePeriods.Where(e => e.DishCycleId == dishCyleId).ToListAsync();
        }

        public async Task<DishCycle> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<List<DishCycleScheduleSetMenuDTO>> GetDishCycleScheduleSetMenus(int cycleId, int day, int? outletId)
        {
            var cycle = _appContext.DishCycles.FirstOrDefault(e => e.IsActive && e.Id == cycleId);
            List<DishCycleScheduleSetMenuDTO> list = new List<DishCycleScheduleSetMenuDTO>();
            if (cycle != null)
            {
                IQueryable<DishCycleScheduleSet> sets = _appContext.DishCycleScheduleSets.Where(e => e.IsActive &&
                                                                        e.DishCycleId == cycleId).OrderBy(e => e.Sequence);

                foreach (var set in sets)
                {
                    DishCycleScheduleSetMenuDTO s = new DishCycleScheduleSetMenuDTO();
                    s.CopyFrom(set);
                    s.DishCycleType = set.DishCycleType;
                    
                    s.ExcludedMenus = _appContext.OutletDishCyclePeriodMenus.Where(e => !outletId.HasValue || (e.OutletId == outletId && e.DishCycleScheduleSetId == set.Id)).ToList();

                    if (cycle.CycleType == "Main Menu")
                    {
                        var c = _appContext.DishCycles.FirstOrDefault(e => e.IsActive && e.Id == set.CycleTypeId);
                        s.Label = c != null ? c.Label : s.Label;
                    }

                    var menus = _appContext.DishCycleScheduleDetailMenus.Where(e => e.IsActive);

                    int? id = cycle.CycleType != "Main Menu" ? cycleId : set.CycleTypeId;
                    int d = cycle.CycleType != "Main Menu" ? day : ((day % set.DishCycleType.NumOfDays) == 0 ? set.DishCycleType.NumOfDays : (day % set.DishCycleType.NumOfDays));
                    var schedule = await _appContext.DishCycleSchedules.FirstOrDefaultAsync(e => e.IsActive &&
                                                                             e.DishCycleId == id &&
                                                                             e.Day == d);

                    if (schedule != null)
                    {
                        if (cycle.CycleType == "Main Menu")
                        {
                            menus = _appContext.DishCycleScheduleDetailMenus.Where(e => e.IsActive && schedule.Details.Any(f => f.Id == e.DishCycleScheduleDetailId && f.Sequence == set.CycleTypeSequence));
                        }
                        else
                        {
                            menus = _appContext.DishCycleScheduleDetailMenus.Where(e => e.IsActive && schedule.Details.Any(f => f.Id == e.DishCycleScheduleDetailId && f.Sequence == set.Sequence));
                        }

                        s.Menus = menus.ToList();
                    }
                    

                    list.Add(s);
                }
            }
            
            return list;
        }

        public async Task<List<MealCreditSetMenuDTO>> GetDishesByMealType(int outletId, int catererId, int mealTypeId, DateTime date, int? sessionId)
        {
            var catererOutlet = _appContext.CatererOutlets.FirstOrDefault(e => e.IsActive && e.CatererInfoId == catererId && e.OutletId == outletId);
            var sessionDetail = _appContext.MealSessionDetails.FirstOrDefault(e => e.Id == sessionId);
            if (catererOutlet == null || sessionDetail == null)
            {
                return null;
            }

            var results = new List<MealCreditSetMenuDTO>();
            var cycles = _appContext.DishCycles.Where(e => e.IsActive && e.OutletProfileId == catererOutlet.OutletProfileId && e.MealTypeId == mealTypeId &&
                                e.DishCyclePeriods.Any(f => f.MealPeriodId == sessionDetail.MealSession.MealPeriodId));

            foreach(var cycle in cycles)
            {
                var setMenu = new MealCreditSetMenuDTO()
                {
                    MealTypeId = mealTypeId,
                    ExcludedMenus = new List<OutletDishCyclePeriodMenu>(),
                    Menus = new List<DishCycleScheduleDetailMenu>()
                };

                if (cycle.StartDate.Date > date.Date)
                    continue;

                IQueryable<DishCycleScheduleSet> sets = _appContext.DishCycleScheduleSets.Where(e => e.IsActive &&
                                                                            e.DishCycleId == cycle.Id).OrderBy(e => e.Sequence);
                List<DishCycleScheduleDetailMenu> menus = new List<DishCycleScheduleDetailMenu>();
                List<OutletDishCyclePeriodMenu> excludedMenus = new List<OutletDishCyclePeriodMenu>();
                foreach (var set in sets)
                {
                    var exMenus = _appContext.OutletDishCyclePeriodMenus.Where(e => e.OutletId == outletId && e.DishCycleScheduleSetId == set.Id).ToList();
                    if (exMenus.Any())
                    {
                        excludedMenus.AddRange(exMenus);
                    }

                    var detailMenus = _appContext.DishCycleScheduleDetailMenus.Where(e => e.IsActive);

                    //identify what day from the date passed
                    var span = date.Date.Subtract(cycle.StartDate.Date);
                    int day = span.Days;
                    int d = day == 0 ? 1 : ((day % cycle.NumOfDays) == 0 ? cycle.NumOfDays : (day % cycle.NumOfDays));
                    var schedule = await _appContext.DishCycleSchedules.FirstOrDefaultAsync(e => e.IsActive &&
                                                                             e.DishCycleId == cycle.Id &&
                                                                             e.Day == d);

                    if (schedule != null)
                    {
                        if (cycle.CycleType == "Main Menu" || cycle.CycleType == "Ala Carte")
                        {
                            if(set.DishCycleType != null)
                            {
                                //get menus from sub dish cycle
                                var subspan = date.Date.Subtract(set.DishCycleType.StartDate.Date);
                                int subday = subspan.Days;
                                int subd = subday == 0 ? 1 : ((subday % set.DishCycleType.NumOfDays) == 0 ? set.DishCycleType.NumOfDays : (day % set.DishCycleType.NumOfDays));
                                var subSchedule = await _appContext.DishCycleSchedules.FirstOrDefaultAsync(e => e.IsActive &&
                                                                                 e.DishCycleId == set.CycleTypeId &&
                                                                                 e.Day == d);

                                var detail = subSchedule.Details.FirstOrDefault(f => f.Sequence == set.CycleTypeSequence);
                                if(detail != null)
                                {
                                    detailMenus = detailMenus.Where(e => detail.Id == e.DishCycleScheduleDetailId);
                                }
                            }
                        }
                        else
                        {
                            var detail = schedule.Details.FirstOrDefault(f => f.Sequence == set.Sequence);
                            if (detail != null)
                            {
                                detailMenus = detailMenus.Where(e => detail.Id == e.DishCycleScheduleDetailId);
                            }
                        }

                        if (detailMenus.Any())
                        {
                            menus.AddRange(detailMenus);
                        }

                    }
                }

                setMenu.DishCycleId = cycle.Id;
                setMenu.ExcludedMenus.AddRange(excludedMenus);
                setMenu.Menus.AddRange(menus);

                results.Add(setMenu);
            }

            return results;
        }

        public async Task<BaseOperationResponse> CreateAsync(DishCycle dishCycle)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(dishCycle);
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

        public async Task<BaseOperationResponse> UpdateAsync(DishCycle dishCycle)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == dishCycle.Id);

            //update periods
            var periodsToDelete = this._appContext.DishCyclePeriods.Where(x => x.DishCycleId == f.Id &&
                                    (dishCycle.DishCyclePeriods == null || !dishCycle.DishCyclePeriods.Any(a => a.MealPeriodId == x.MealPeriodId)));

            this._appContext.DishCyclePeriods.RemoveRange(periodsToDelete);

            if (dishCycle.DishCyclePeriods != null)
            {
                dishCycle.DishCyclePeriods.ToList().ForEach(e =>
                {
                    var sr = this._appContext.DishCyclePeriods.FirstOrDefault(x => x.DishCycleId == e.DishCycleId && x.MealPeriodId == e.MealPeriodId);
                    if (sr != null)
                    {
                        sr.IsActive = true;
                        this._appContext.DishCyclePeriods.Update(sr);
                    }
                    else
                    {
                        this._appContext.DishCyclePeriods.Add(e);
                    }
                });
            }

            //remove schedules
            var cycleSchedules = this._appContext.DishCycleSchedules.Where(e => e.DishCycleId == dishCycle.Id);

            //remove deleted schedules
            var schedToDelete = cycleSchedules.Where(e => !dishCycle.Schedules.Any(x => x.Id == e.Id));
            this._appContext.DishCycleSchedules.RemoveRange(schedToDelete);

            //add new schedules
            var schedToAdd = dishCycle.Schedules.Where(e => !cycleSchedules.Any(x => x.Id == e.Id));
            this._appContext.DishCycleSchedules.AddRange(schedToAdd);

            var cycleSets = this._appContext.DishCycleScheduleSets.Where(e => e.DishCycleId == dishCycle.Id);

            //remove deleted sets
            var setToDelete = cycleSets.Where(e => !dishCycle.Sets.Any(x => x.Id == e.Id && x.CycleTypeId == e.CycleTypeId));
            this._appContext.DishCycleScheduleSets.RemoveRange(setToDelete);

            //add new sets
            var setToAdd = dishCycle.Sets.Where(e => !cycleSets.Any(x => x.Id == e.Id && x.CycleTypeId == e.CycleTypeId));
            this._appContext.DishCycleScheduleSets.AddRange(setToAdd);

            //modify sets
            var setsToUpdate = dishCycle.Sets.Where(e => cycleSets.Any(x => x.Id == e.Id && x.CycleTypeId == e.CycleTypeId)).ToList();
            setsToUpdate.ForEach((set) => {
                var s = this._appContext.DishCycleScheduleSets.FirstOrDefault(e => set.Id == e.Id);
                s.CycleTypeSequence = set.CycleTypeSequence;
                s.MealTypeId = set.MealTypeId;
                s.Label = set.Label;
                s.Price = set.Price;
                this._appContext.DishCycleScheduleSets.Update(s);
            });

            //if cycle still exist, check the details
            cycleSchedules.ToList().ForEach(x =>
            {
                var sched = dishCycle.Schedules.FirstOrDefault(e => e.Id == x.Id);

                if (sched != null)
                {
                    //remove deleted details
                    var detToDelete = x.Details.Where(e => !sched.Details.Any(a => a.Id == e.Id));
                    this._appContext.DishCycleScheduleDetails.RemoveRange(detToDelete);

                    //add new details
                    var detToAdd = sched.Details.Where(e => !x.Details.Any(a => a.Id == e.Id));
                    this._appContext.DishCycleScheduleDetails.AddRange(detToAdd);

                    var menusToSave = sched?.Details.SelectMany(e => e.Menus);
                    if (menusToSave != null)
                    {
                        var existingMenus = x.Details.SelectMany(e => e.Menus);
                        var menusToDelete = existingMenus.Where(e => !menusToSave.Any(a => a.DishCycleScheduleDetailId == e.DishCycleScheduleDetailId && a.DishId == e.DishId));
                        this._appContext.DishCycleScheduleDetailMenus.RemoveRange(menusToDelete);

                        //add new classes
                         var menusToAdd = menusToSave.Where(e => !x.Details.SelectMany(a => a.Menus).Any(a => a.DishCycleScheduleDetailId == e.DishCycleScheduleDetailId && a.DishId == e.DishId));
                        this._appContext.DishCycleScheduleDetailMenus.AddRange(menusToAdd);
                    }

                    //update day
                    x.Day = sched.Day;
                    this._appContext.DishCycleSchedules.Update(x);
                }

            });

            f.CopyFrom(dishCycle);

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


        public async Task<BaseOperationResponse> DeleteAsync(int dishCycleId)
        {
            var result = new BaseOperationResponse();
            var dishCycle = await GetSingleOrDefaultAsync(r => r.Id == dishCycleId);

            if (dishCycle != null)
                return await Delete(dishCycle);

            result.IsSuccess = false;
            result.Message = "DishCycle  not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(DishCycle dishCycle)
        {
            var result = new BaseOperationResponse();
            SoftDelete(dishCycle);
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

        public async Task<BaseOperationResponse> DishBlockOutletDate(List<OutletDishBlockedDate> blockedDates)
        {
            var result = new BaseOperationResponse();

            await _appContext.OutletDishBlockedDates.AddRangeAsync(blockedDates);
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

        public async Task<BaseOperationResponse> DishUnblockOutletDate(List<OutletDishBlockedDate> blockedDates)
        {
            var result = new BaseOperationResponse();

            var toRemove = _appContext.OutletDishBlockedDates.Where(e => blockedDates.Any(f => f.OutletId == e.OutletId && f.DishCycleId == e.DishCycleId && e.EffectiveDate == f.EffectiveDate));
            _appContext.OutletDishBlockedDates.RemoveRange(toRemove);
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

        public async Task<BaseOperationResponse> CreateOutletDishCyclePeriodMenus(OutletDishViewMenu model)
        {
            var result = new BaseOperationResponse();
            if(model.ExcludedMenus == null || !model.ExcludedMenus.Any())
            {
                var toRemove = _appContext.OutletDishCyclePeriodMenus.Where(e => e.OutletId == model.OutletId).ToList();
                _appContext.OutletDishCyclePeriodMenus.RemoveRange(toRemove);
            }

            var grpByOutlet = model.ExcludedMenus.GroupBy(e => e.OutletId);
            foreach (var menus in grpByOutlet)
            {
                var existingMenus = _appContext.OutletDishCyclePeriodMenus.Where(e => e.OutletId == menus.Key);
                var toRemove = existingMenus.Where(e => !menus.Any(f => f.DishCyclePeriodId == e.DishCyclePeriodId && f.DishCycleScheduleSetId == e.DishCycleScheduleSetId
                                 && f.DishId == e.DishId));

                _appContext.OutletDishCyclePeriodMenus.RemoveRange(toRemove);

                var toAdd = menus.Where(e => !existingMenus.Any(f => f.DishCyclePeriodId == e.DishCyclePeriodId && f.DishCycleScheduleSetId == e.DishCycleScheduleSetId
                                 && f.DishId == e.DishId));

                await _appContext.OutletDishCyclePeriodMenus.AddRangeAsync(toAdd);

            }

            await _appContext.SaveChangesAsync();
            result.Message = "Successfully saved!";
            result.IsSuccess = true;

            return result;
        }
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
