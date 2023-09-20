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
using System.Transactions;
using DAL.Core.Helpers;

namespace DAL.Repositories.MealOrder
{
    public class OutletClassRosterRepository : Repository<OutletClassRoster>, IOutletClassRosterRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public OutletClassRosterRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<OutletClassRoster>> GetOutletClassRostersAsync(ClassRosterFilter filter)
        {
            var catererOutlet = await _appContext.CatererOutlets.FirstOrDefaultAsync(e => e.OutletId == filter.OutletId && e.CatererInfoId == filter.CatererId);
            IQueryable<OutletClassRoster> query = _appContext.OutletClassRosters.Where(e => e.OutletProfileId == catererOutlet.OutletProfileId);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<OutletClassRoster> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<List<OutletClassRoster>> GetByCatererOutletIdAsync(int outletId, int catererId, int mealSessionId)
        {
            var catererOutlet = await _appContext.CatererOutlets.FirstOrDefaultAsync(e => e.OutletId == outletId && e.CatererInfoId == catererId);
            var query = await FindAsync(e => e.IsActive && e.OutletProfileId == catererOutlet.OutletProfileId && e.MealSessionId == mealSessionId);

            //query.Schedules = query.Schedules.Where(e => e.Periods.Any(f => f.MealSessionDetailId == mealPeriodId)).ToList();
            return query.ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(OutletClassRoster outletClassRoster)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.ROSTER_CREATE.ToString(),
                Remarks = $"Class roster was created."
            };

            var result = new BaseOperationResponse();

            //validate class roster
            var similarRosters = await FindAsync(e => e.IsActive && e.MealSessionId == outletClassRoster.MealSessionId && e.OutletProfileId == outletClassRoster.OutletProfileId
                                        && (outletClassRoster.StartDate.Date >= e.StartDate.Date && outletClassRoster.StartDate.Date <= e.EndDate.Value.Date));

            if (similarRosters.Any())
            {
                result.Message = "Date conflicts with other class rosters.";
                result.IsSuccess = false;
            }
            else
            {
                //make sure start and end date is just date
                outletClassRoster.StartDate = outletClassRoster.StartDate.Date;
                outletClassRoster.EndDate = outletClassRoster.EndDate?.Date;
                var f = await AddAsync(outletClassRoster);
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
            }

            _appContext.ResetAuditUserAction();
            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(OutletClassRoster outletClassRoster)
        {
            var result = new BaseOperationResponse();

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                _appContext.AuditUserActivityType = new AuditUserActivityType
                {
                    GroupId = Common.GenerateUniqueStringId(),
                    ActionName = UserActivityType.ROSTER_UPDATE.ToString(),
                    Remarks = $"Class roster was updated."
                };

                //validate class roster
                var similarRosters = await FindAsync(e => e.Id != outletClassRoster.Id && e.IsActive && e.MealSessionId == outletClassRoster.MealSessionId && e.OutletProfileId == outletClassRoster.OutletProfileId
                                        && (outletClassRoster.StartDate.Date >= e.StartDate.Date && outletClassRoster.StartDate.Date <= e.EndDate.Value.Date));

                if (similarRosters.Any())
                {
                    result.Message = "Date conflicts with other class rosters.";
                    result.IsSuccess = false;
                }
                else
                {
                    var f = await GetSingleOrDefaultAsync(e => e.Id == outletClassRoster.Id);

                    //remove menus

                    //remove schedules
                    var selectedDays = outletClassRoster.Schedules.Select(a => a.Day);
                    var cycleSchedules = this._appContext.OutletClassRosterSchedules.Where(e => e.OutletClassRosterId == outletClassRoster.Id);

                    //remove deleted schedules
                    var schedToDelete = cycleSchedules.Where(e => !outletClassRoster.Schedules.Any(x => x.Id == e.Id));
                    this._appContext.OutletClassRosterSchedules.RemoveRange(schedToDelete);

                    //add new schedules
                    var schedToAdd = outletClassRoster.Schedules.Where(e => !cycleSchedules.Any(x => x.Id == e.Id));
                    this._appContext.OutletClassRosterSchedules.AddRange(schedToAdd);

                    //if cycle still exist, check the periods
                    cycleSchedules.ToList().ForEach(x =>
                    {
                        var sched = outletClassRoster.Schedules.FirstOrDefault(e => e.Id == x.Id);

                        if (sched != null)
                        {
                            var newPeriods = sched?.Periods.Where(e => e.Id == 0);
                            foreach (var newPeriod in newPeriods)
                            {
                                var np = _appContext.OutletClassRosterSchedulePeriods.FirstOrDefault(e => e.Id == newPeriod.Id) ?? new OutletClassRosterSchedulePeriod();
                                np.CopyFrom(newPeriod);
                                np.Classes = newPeriod.Classes;
                                this._appContext.OutletClassRosterSchedulePeriods.Add(np);

                                _appContext.SaveChanges();
                                foreach (var c in newPeriod.Classes)
                                {
                                    c.OutletClassRosterSchedulePeriodId = np.Id;
                                    this._appContext.OutletClassRosterSchedulePeriodClasses.Add(c);
                                }
                            }

                            var classesToSave = sched?.Periods.Where(e => e.Id > 0).SelectMany(e => e.Classes);
                            if (classesToSave != null)
                            {
                                var existingClasses = x.Periods.SelectMany(e => e.Classes);
                                var classesToDelete = existingClasses.Where(e => !classesToSave.Any(a => a.OutletClassRosterSchedulePeriodId == e.OutletClassRosterSchedulePeriodId && a.ClassId == e.ClassId));
                                this._appContext.OutletClassRosterSchedulePeriodClasses.RemoveRange(classesToDelete);

                            //add new classes
                            var classesToAdd = classesToSave.Where(e => !x.Periods.SelectMany(a => a.Classes).Any(a => a.OutletClassRosterSchedulePeriodId == e.OutletClassRosterSchedulePeriodId && a.ClassId == e.ClassId));
                                this._appContext.OutletClassRosterSchedulePeriodClasses.AddRange(classesToAdd);
                            }

                        //update day
                        x.Day = sched.Day;
                            this._appContext.OutletClassRosterSchedules.Update(x);
                        }

                    });

                    //var toBeDeleted = cycleSchedules.Where(e => !selectedDays.Contains(e.Day));
                    //this._appContext.OutletClassRosterSchedules.RemoveRange(cycleSchedules);

                    //add new schedule
                    //await this._appContext.OutletClassRosterSchedules.AddRangeAsync(outletClassRoster.Schedules);

                    f.StartDate = outletClassRoster.StartDate;
                    f.EndDate = outletClassRoster.EndDate;
                    f.Label = outletClassRoster.Label;

                    Update(f);
                    if (await _appContext.SaveChangesAsync() > 0)
                    {
                        result.Message = "Successfully saved!";
                        result.IsSuccess = true;
                        result.Data = f;
                        scope.Complete();
                    }
                    else
                    {
                        result.Message = "Failed to save!";
                        result.IsSuccess = false;
                    }
                }

            }
            _appContext.ResetAuditUserAction();
            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int outletClassRosterId)
        {
            var result = new BaseOperationResponse();
            var outletClassRoster = await GetSingleOrDefaultAsync(r => r.Id == outletClassRosterId);

            if (outletClassRoster != null)
                return await Delete(outletClassRoster);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(OutletClassRoster outletClassRoster)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.ROSTER_DELETE.ToString(),
                Remarks = $"Class roster was deleted."
            };

            var result = new BaseOperationResponse();
            SoftDelete(outletClassRoster);
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
            _appContext.ResetAuditUserAction();

            return result;
        }

        #region Sieved
        public async Task<List<OutletClassRosterSchedulePeriod>> GetOutletClassRosterSchedulePeriods(int outletClassRosterId, int day)
        {
            IQueryable<OutletClassRosterSchedulePeriod> query = _appContext.OutletClassRosterSchedulePeriods.Where(e => e.IsActive && 
                                                                        e.OutletClassRosterSchedule.OutletClassRosterId == outletClassRosterId &&
                                                                        e.OutletClassRosterSchedule.Day == day);

            return await query.OrderBy(e => e.MealSessionDetail.Sequence).ToListAsync();
        }

        public async Task<BaseOperationResponse> CreateSchedulePeriodMenus(List<OutletClassRosterSchedulePeriod> periods)
        {
            var result = new BaseOperationResponse();

            bool isModified = false;
            foreach (var period in periods)
            {
                var classes = _appContext.OutletClassRosterSchedulePeriodClasses.Where(e => 
                                    e.OutletClassRosterSchedulePeriod.OutletClassRosterScheduleId == period.OutletClassRosterScheduleId &&
                                    e.OutletClassRosterSchedulePeriod.MealSessionDetailId == period.MealSessionDetailId);

                var mealPeriodSchedule = await _appContext.OutletClassRosterSchedulePeriods.FirstOrDefaultAsync(e => e.OutletClassRosterScheduleId == period.OutletClassRosterScheduleId &&
                                    e.MealSessionDetailId == period.MealSessionDetailId);
                var toAddRange = period.Classes.Where(e => !classes.Any(f => f.ClassId == e.ClassId));
                toAddRange.ToList().ForEach(e => {
                    e.OutletClassRosterSchedulePeriodId = mealPeriodSchedule.Id;
                    _appContext.OutletClassRosterSchedulePeriodClasses.AddAsync(e);
                });
                
                var toRemove = classes.Where(e => !period.Classes.Any(f => e.ClassId == f.ClassId));
                if (!isModified)
                {
                    isModified = toAddRange.Any() || toRemove.Any();
                }

                _appContext.OutletClassRosterSchedulePeriodClasses.RemoveRange(toRemove);
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


        public async Task<List<OutletClassRoster>> GetOutletOutletClassRostersAsync(int outletId, int catererId)
        {
            var outlet = await _appContext.CatererOutlets.FirstOrDefaultAsync(e => e.OutletId == outletId && e.CatererInfoId == catererId);

            IQueryable<OutletClassRoster> query = _appContext.OutletClassRosters.Where(e => e.OutletProfileId == outlet.OutletProfileId);

            return await query.ToListAsync();
        }

        public async Task<MealSessionDetail> GetCurrentOrderMealSessionAsync(int? outletId, DateTime orderDate, int mealSessionId, int classId)
        {
            try
            {
                var classRoster = await _appContext.OutletClassRosters.FirstOrDefaultAsync(e => e.IsActive &&
                                                e.OutletProfile.Caterer.CatererOutlets.Any(o => o.IsActive && o.OutletId == outletId) && e.MealSessionId == mealSessionId &&
                                                (orderDate.Date >= e.StartDate.Date &&
                                                (!e.EndDate.HasValue || e.EndDate.Value.Date >= orderDate.Date)));


                if (classRoster != null && classRoster.Schedules != null && classRoster.Schedules.Any())
                {
                    var schedules = classRoster.Schedules.Where(e => e.IsActive);
                    int numOfDays = schedules.Count();

                    var span = orderDate.Date.Subtract(classRoster.StartDate.Date);
                    int day = span.Days + 1;
                    int d = day == 0 ? 1 : (day % numOfDays);
                    if (d == 0) { d = numOfDays; }
                    var sched = schedules.FirstOrDefault(e => e.Day == d);

                    //assuming order cannot be transferred from one period to another
                    var mealSessionDetails = _appContext.MealSessionDetails.Where(e => e.IsActive && e.MealSessionId == mealSessionId).Select(e => e.Id).ToList();
                    var periodIds = _appContext.OutletClassRosterSchedulePeriods.Where(e => e.IsActive &&
                                mealSessionDetails.Any(x=> x == e.MealSessionDetailId) &&
                                e.OutletClassRosterScheduleId == sched.Id).Select(e => e.Id).ToList();

                    var periodClass = await _appContext.OutletClassRosterSchedulePeriodClasses.FirstOrDefaultAsync(e => e.IsActive &&
                                periodIds.Any(x=> x == e.OutletClassRosterSchedulePeriodId) &&
                                e.ClassId == classId);

                    return periodClass?.OutletClassRosterSchedulePeriod?.MealSessionDetail;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
