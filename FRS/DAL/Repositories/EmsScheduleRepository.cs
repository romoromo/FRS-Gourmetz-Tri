using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Ical.Net.DataTypes;
using Ical.Net;
using Ical.Net.CalendarComponents;

namespace DAL.Repositories
{
    public class EmsScheduleRepository : Repository<EmsSchedule>, IEmsScheduleRepository
    {
        public EmsScheduleRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<EmsSchedule> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<List<EmsSchedule>> GetEmsSchedulesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<EmsSchedule> query = _appContext.EmsSchedules
                .Where(e => e.IsActive);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<List<RSchedule>> GetRecSchedulesAsync(DateTime? start, DateTime? end)
        {
            if (start == null) start = DateTime.Now;
            if (end == null) end = start.Value.AddDays(2);

            IQueryable<EmsSchedule> query = _appContext.EmsSchedules
                .Where(e => e.IsActive && (e.EffectiveDate >= start || e.IneffectiveDate > start));

            var result = new List<RSchedule>();

            query.ToList().ForEach(s => {
                var rrule = new RecurrencePattern(FrequencyType.Weekly, 1);
                rrule.Until = s.IneffectiveDate.Value;

                if (s.Monday) rrule.ByDay.Add(new WeekDay(DayOfWeek.Monday));
                if (s.Tuesday) rrule.ByDay.Add(new WeekDay(DayOfWeek.Tuesday));
                if (s.Wednesday) rrule.ByDay.Add(new WeekDay(DayOfWeek.Wednesday));
                if (s.Thursday) rrule.ByDay.Add(new WeekDay(DayOfWeek.Thursday));
                if (s.Friday) rrule.ByDay.Add(new WeekDay(DayOfWeek.Friday));
                if (s.Saturday) rrule.ByDay.Add(new WeekDay(DayOfWeek.Saturday));
                if (s.Sunday) rrule.ByDay.Add(new WeekDay(DayOfWeek.Sunday));

                var vEvent = new CalendarEvent
                {
                    Start = new CalDateTime(s.EffectiveDate.Value + s.StartTime.Value.TimeOfDay),
                };
                vEvent.RecurrenceRules = new List<RecurrencePattern> { rrule };
                var calendar = new Calendar();
                calendar.Events.Add(vEvent);
                var occurrences = calendar.GetOccurrences(start.Value, end.Value);

                occurrences.ToList().ForEach(o =>
                {
                    result.Add(new RSchedule() {
                        date = o.Period.StartTime.Value,
                        scheduleId = s.Id
                    });
                });
                
            });

            result = result.OrderBy(r => r.date).ToList();

            return result;
        }

        public IEnumerable<EmsSchedule> All()
        {
            return GetAll()
                .ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(EmsSchedule emsSchedule)
        {
            if (emsSchedule.StartTime.HasValue) emsSchedule.StartTime = DateTime.MinValue + emsSchedule.StartTime.Value.TimeOfDay;
            if (emsSchedule.EndTime.HasValue) emsSchedule.EndTime = DateTime.MinValue + emsSchedule.EndTime.Value.TimeOfDay;

            var result = new BaseOperationResponse();
            if (Exists(e => e.EmsGroupId == emsSchedule.EmsGroupId && e.EffectiveDate < emsSchedule.IneffectiveDate && emsSchedule.EffectiveDate < e.IneffectiveDate && e.StartTime < emsSchedule.EndTime && emsSchedule.StartTime < e.EndTime && ((e.Monday && emsSchedule.Monday) || (e.Tuesday && emsSchedule.Tuesday) || (e.Wednesday && emsSchedule.Wednesday) || (e.Thursday && emsSchedule.Thursday) || (e.Friday && emsSchedule.Friday) || (e.Saturday && emsSchedule.Saturday) || (e.Sunday && emsSchedule.Sunday))).Result)
            {
                result.Message = "EmsSchedule is overlapping with another schedule!";
                result.IsSuccess = false;
            }
            else
            {
                

                var f = await AddAsync(emsSchedule);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save ems schedule!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(EmsSchedule emsSchedule)
        {
            if (emsSchedule.StartTime.HasValue) emsSchedule.StartTime = DateTime.MinValue + emsSchedule.StartTime.Value.TimeOfDay;
            if (emsSchedule.EndTime.HasValue) emsSchedule.EndTime = DateTime.MinValue + emsSchedule.EndTime.Value.TimeOfDay;

            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == emsSchedule.Id);
            if (Exists(e => e.Id != f.Id && e.EmsGroupId == emsSchedule.EmsGroupId && e.EffectiveDate < emsSchedule.IneffectiveDate && emsSchedule.EffectiveDate < e.IneffectiveDate && e.StartTime < emsSchedule.EndTime && emsSchedule.StartTime < e.EndTime && ((e.Monday && emsSchedule.Monday) || (e.Tuesday && emsSchedule.Tuesday) || (e.Wednesday && emsSchedule.Wednesday) || (e.Thursday && emsSchedule.Thursday) || (e.Friday && emsSchedule.Friday) || (e.Saturday && emsSchedule.Saturday) || (e.Sunday && emsSchedule.Sunday))).Result)
            {
                result.Message = "EmsSchedule is overlapping with another schedule!";
                result.IsSuccess = false;
            }
            else
            {
                f.CopyFrom(emsSchedule);
                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save ems schedule type!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int emsScheduleId)
        {
            var result = new BaseOperationResponse();
            var emsSchedule = await GetSingleOrDefaultAsync(r => r.Id == emsScheduleId);

            if (emsSchedule != null)
                return await Delete(emsSchedule);

            result.IsSuccess = false;
            result.Message = "EmsSchedule not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(EmsSchedule emsSchedule)
        {
            var result = new BaseOperationResponse();
            SoftDelete(emsSchedule);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete ems schedule!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }

    public class RSchedule
    {
        public DateTime date { get; set; }
        public int scheduleId { get; set; }
    }
}
