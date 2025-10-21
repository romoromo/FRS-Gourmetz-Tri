using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Helpers;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Interfaces.MealOrder;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DAL.Repositories.MealOrder
{
    public class MealSessionRepository : Repository<MealSession>, IMealSessionRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public MealSessionRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<MealSession>> GetMealSessionsAsync(BaseFilter filter)
        {
            IQueryable<MealSession> query = _appContext.MealSessions
                .Include(e => e.Institution)
                .OrderBy(e => e.Sequence);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<MealSession>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public List<MealSessionMealPeriod> GetMealSessionsByMealPeriod(int outletId, int catererId, int outletProfileId)
        {
            var mealPeriods = _appContext.MealPeriods.Where(e => e.IsActive && e.OutletProfileId == outletProfileId).OrderBy(e => e.Sequence);
            IQueryable<MealSession> query = _appContext.MealSessions
                .Where(e => e.IsActive && e.OutletId == outletId && e.CatererId == catererId)
                .OrderBy(e => e.Sequence);

            var list = new List<MealSessionMealPeriod>();
            foreach (var mealPeriod in mealPeriods)
            {
                var mealSessions = query.Where(e => e.MealPeriodId == mealPeriod.Id).ToList();

                if (!mealSessions.Any())
                {
                    mealSessions = new List<MealSession> {
                        new MealSession() {
                            CatererId = catererId,
                            Details = new List<MealSessionDetail>(),
                            MealPeriodId = mealPeriod.Id,
                            Sequence  = mealPeriod.Sequence,
                            OutletId = outletId,
                            Name = mealPeriod.Name,
                            StartDate = mealPeriod.StartDate,
                            EndDate = mealPeriod.EndDate
                        }
                    };
                }
                else
                {
                    mealSessions.ForEach(e =>
                    {
                        e.Name = mealPeriod.Name;
                        e.StartDate = mealPeriod.StartDate;
                        e.EndDate = mealPeriod.EndDate;
                        e.Details = e.Details?.OrderBy(f => f.StartDate.TimeOfDay).ToList();
                    });
                }

                var periodSession = new MealSessionMealPeriod
                {
                    MealPeriod = mealPeriod,
                    MealSessions = mealSessions
                };

                list.Add(periodSession);
            }

            return list;
        }

        public List<MealSessionMealPeriod> GetMealSessionsByMealPeriodByOutlet(int outletId)
        {
            var mealPeriods = _appContext.MealPeriods.Where(e => e.IsActive && e.OutletProfile.Id == outletId).OrderBy(e => e.Sequence);
            IQueryable<MealSession> query = _appContext.MealSessions
                .Where(e => e.IsActive && e.OutletId == outletId)
                .OrderBy(e => e.Sequence);

            var list = new List<MealSessionMealPeriod>();
            foreach (var mealPeriod in mealPeriods)
            {
                var mealSessions = query.Where(e => e.MealPeriodId == mealPeriod.Id).ToList();

                if (!mealSessions.Any())
                {
                    mealSessions = new List<MealSession> {
                        new MealSession() {
                            CatererId = mealPeriod.OutletProfile.CatererId,
                            Details = new List<MealSessionDetail>(),
                            MealPeriodId = mealPeriod.Id,
                            Sequence  = mealPeriod.Sequence,
                            OutletId = outletId,
                            Name = mealPeriod.Name,
                            StartDate = mealPeriod.StartDate,
                            EndDate = mealPeriod.EndDate
                        }
                    };
                }
                else
                {
                    mealSessions.ForEach(e => {
                        e.Name = mealPeriod.Name;
                        e.StartDate = mealPeriod.StartDate;
                        e.EndDate = mealPeriod.EndDate;
                    });
                }

                var periodSession = new MealSessionMealPeriod
                {
                    MealPeriod = mealPeriod,
                    MealSessions = mealSessions
                };

                list.Add(periodSession);
            }

            return list;
        }

        public async Task<List<MealSessionLiteDto>> GetLiteByOutletId(int outletId)
        {
            var datas = await _appContext.MealSessions
                .Where(e => e.IsActive && e.OutletId == outletId)
                .OrderBy(e => e.Sequence)
                .Select(x => new MealSessionLiteDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Sequence = x.Sequence
                })
                .ToListAsync();
            return datas;
        }

        public async Task<MealSession> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(MealSession mealSession)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.ROSTER_MEALSESSION_CREATE.ToString(),
                Remarks = $"Class roster session was created."
            };

            var result = new BaseOperationResponse();

            var f = await AddAsync(mealSession);
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

            _appContext.ResetAuditUserAction();
            return result;
        }

        public async Task<BaseOperationResponse> BulkCreateAsync(List<MealSession> mealSessions)
        {
            var result = new BaseOperationResponse();

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                _appContext.AuditUserActivityType = new AuditUserActivityType
                {
                    GroupId = Common.GenerateUniqueStringId(),
                    ActionName = UserActivityType.ROSTER_MEALSESSION_UPDATE.ToString(),
                    Remarks = $"Class roster sessions were updated."
                };

                var mealSession = mealSessions.FirstOrDefault();

                var mealSessionIds = mealSessions.Where(e => e.Id > 0).Select(e => e.Id);
                //remove existing meal sessions of the outlet
                var oldMealSessions = _appContext.MealSessions.Where(e => e.OutletId == mealSession.OutletId &&
                                        mealSession.CatererId == mealSession.CatererId && !mealSessionIds.Any(x => x == e.Id));

                await oldMealSessions.ForEachAsync(e => { e.IsActive = false; });
                _appContext.MealSessions.UpdateRange(oldMealSessions);

                //_appContext.MealSessions.RemoveRange(oldMealSessions);

                //then add/update
                foreach(var e in mealSessions)
                {
                    var ms = this._appContext.MealSessions.FirstOrDefault(f => f.Id == e.Id);
                    if (ms != null)
                    {
                        ms.Name = e.Name;
                        ms.Sequence = e.Sequence;

                        var detailIds = e.Details.Where(x => x.Id > 0).Select(x => x.Id);
                        var oldDetails = this._appContext.MealSessionDetails.Where(a => a.MealSessionId == ms.Id
                                                                        && !detailIds.Any(y => y == a.Id));
                        await oldDetails.ForEachAsync(x => { x.IsActive = false; });
                        _appContext.MealSessionDetails.UpdateRange(oldDetails);

                        //this._appContext.MealSessionDetails.RemoveRange(oldDetails);
                        //this._appContext.MealSessionDetails.AddRange(e.Details);

                        //ms.Details = e.Details;
                        ms.Details = null;
                        Update(ms);

                        foreach (var detail in e.Details)
                        {
                            var md = this._appContext.MealSessionDetails.FirstOrDefault(a => a.Id == detail.Id);
                            if (md != null)
                            {
                                md.CopyFrom(detail);
                                md.IsActive = detail.IsActive;
                                this._appContext.MealSessionDetails.Update(md);
                            }
                            else
                            {
                                detail.MealSessionId = ms.Id;
                                this._appContext.MealSessionDetails.Add(detail);
                            }
                        }
                    }
                    else
                    {
                        Add(e);
                    }
                }

                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    scope.Complete();
                    //result.Data = f;
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

        public async Task<BaseOperationResponse> UpdateAsync(MealSession mealSession)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.ROSTER_MEALSESSION_UPDATE.ToString(),
                Remarks = $"Class roster session {mealSession.Id} was updated."
            };

            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == mealSession.Id);

            f.CopyFrom(mealSession);

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

            _appContext.ResetAuditUserAction();
            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int mealSessionId)
        {
            var result = new BaseOperationResponse();
            var mealSession = await GetSingleOrDefaultAsync(r => r.Id == mealSessionId);

            if (mealSession != null)
                return await Delete(mealSession);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(MealSession mealSession)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.ROSTER_MEALSESSION_DELETE.ToString(),
                Remarks = $"Class roster session {mealSession.Id} was deleted."
            };

            var result = new BaseOperationResponse();
            SoftDelete(mealSession);
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
