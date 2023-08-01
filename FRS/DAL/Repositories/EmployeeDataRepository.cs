using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;

namespace DAL.Repositories
{
    public class EmployeeDataRepository : Repository<EmployeeData>, IEmployeeDataRepository
    {
        public EmployeeDataRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<EmployeeData> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<EmployeeData> All()
        {
            return GetAll()
                .OrderBy(c => c.Code)
                .ToList();
        }

        public async Task<List<EmployeeData>> GetEmployeeDatasLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<EmployeeData> query = _appContext.EmployeeData
                .Where(e => e.IsActive)
                .OrderBy(r => r.Code);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<List<EmployeeData>> GetCurrentEmployeesByLocation(int? locationId = null)
        {
            var now = DateTime.Now;
            var day = (int) now.DayOfWeek;

            IQueryable<EmployeeScheduleSlot> query = _appContext.EmployeeScheduleSlot
                .Where(e => e.IsActive 
                && e.LocationId == locationId 
                && e.Schedule != null && e.Schedule.IsActive && e.Schedule.EffectiveDate <= now && e.Schedule.IneffectiveDate >= now 
                && e.EmployeeData != null && e.EmployeeData.IsActive
                && e.Shift != null && e.Shift.IsActive && e.Shift.StartTime != null && e.Shift.EndTime != null 
                && e.day == day);

            var roles = query.ToList().Select(s =>
            {
                var startTime = DateTime.Today.Add(s.Shift.StartTime.Value.TimeOfDay);
                var endTime = DateTime.Today.Add(s.Shift.EndTime.Value.TimeOfDay);

                var prevExtend = false;
                //var prevNoDisplay = false;
                //int? prevShiftId = null;

                var currentNoDisplay = false;
                var currentExtend = false;

                var shifts = _appContext.EmployeeScheduleInfo.Where(e => e.IsActive
                && e.ScheduleId == s.ScheduleId
                && e.LocationId == locationId
                && e.day == day
                && e.Shift != null && e.Shift.IsActive && e.Shift.StartTime != null && e.Shift.EndTime != null
                && e.extend);

                foreach(var shift in shifts)
                {
                    var st = DateTime.Today.Add(shift.Shift.StartTime.Value.TimeOfDay);
                    var et = DateTime.Today.Add(shift.Shift.EndTime.Value.TimeOfDay);

                    if(st < startTime && shift.ShiftId != s.ShiftId)
                    {
                        prevExtend = shift.extend;
                        //prevShiftId = shift.Shift.Id;
                        //prevNoDisplay = shift.noDisplay;
                    }
                }

                var currentInfo = _appContext.EmployeeScheduleInfo.FirstOrDefault(e => e.IsActive
                && e.ScheduleId == s.ScheduleId
                && e.LocationId == s.LocationId
                && e.day == day
                && e.ShiftId == s.ShiftId);

                if (currentInfo != null)
                {
                    currentNoDisplay = currentInfo.noDisplay;
                    currentExtend = currentInfo.extend;
                }

                if(prevExtend)
                {
                    return null;
                } else if (!currentNoDisplay && ((currentExtend && endTime < now) || (startTime <= now && endTime >= now))) {
                    return s.EmployeeData;
                }

                return null;
            });

            return roles.Where(c => c != null).ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(EmployeeData employeeData)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Code == employeeData.Code).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await AddAsync(employeeData);
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

        public async Task<BaseOperationResponse> UpdateAsync(EmployeeData employeeData)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Code == employeeData.Code && e.Id != employeeData.Id).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await GetSingleOrDefaultAsync(e => e.Id == employeeData.Id);

            f.CopyFrom(employeeData);
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


        public async Task<BaseOperationResponse> DeleteAsync(int employeeDataId)
        {
            var result = new BaseOperationResponse();
            var employeeData = await GetSingleOrDefaultAsync(r => r.Id == employeeDataId);

            if (employeeData != null)
                return await Delete(employeeData);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(EmployeeData employeeData)
        {
            var result = new BaseOperationResponse();
            SoftDelete(employeeData);
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

        public async Task<int> GetOrCreateByCode(EmployeeData data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Code)) return 0;

            var f = await GetSingleOrDefaultAsync(c => c.Code == data.Code && c.IsActive);

            if (f == null)
            {
                await CreateAsync(data);
                f = await GetSingleOrDefaultAsync(c => c.Code == data.Code && c.IsActive);
            }

            return f.Id;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
