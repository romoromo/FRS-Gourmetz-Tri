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
    public class EmployeeScheduleRepository : Repository<EmployeeSchedule>, IEmployeeScheduleRepository
    {
        public EmployeeScheduleRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<EmployeeSchedule> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<EmployeeSchedule> All()
        {
            return GetAll()
                .OrderBy(c => c.Code)
                .ToList();
        }

        public async Task<List<EmployeeSchedule>> GetEmployeeSchedulesLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<EmployeeSchedule> query = _appContext.EmployeeSchedule
                .Where(e => e.IsActive)
                .OrderBy(r => r.Code);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(EmployeeSchedule employeeSchedule)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Code == employeeSchedule.Code).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await AddAsync(employeeSchedule);
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

        public async Task<BaseOperationResponse> UpdateAsync(EmployeeSchedule employeeSchedule)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Code == employeeSchedule.Code && e.Id != employeeSchedule.Id).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await GetSingleOrDefaultAsync(e => e.Id == employeeSchedule.Id);

            f.CopyFrom(employeeSchedule);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                employeeSchedule.Shifts.ToList().ForEach(cSource =>
                {
                    if (cSource.IsActive || cSource.Id > 0)
                    {
                        var ct = _appContext.EmployeeScheduleShift.FirstOrDefault(c => cSource.Id == c.Id) ?? new EmployeeScheduleShift();
                        ct.CopyFrom(cSource);
                        if(f.Id != ct.ScheduleId) ct.ScheduleId = f.Id;
                        ct.IsActive = cSource.IsActive;
                        _appContext.EmployeeScheduleShift.Update(ct);
                        _appContext.SaveChanges();
                    }
                });

                employeeSchedule.Locations.ToList().ForEach(cSource =>
                {
                    if (cSource.IsActive || cSource.Id > 0)
                    {
                        var ct = _appContext.EmployeeScheduleLocation.FirstOrDefault(c => cSource.Id == c.Id) ?? new EmployeeScheduleLocation();
                        ct.CopyFrom(cSource);
                        if (f.Id != ct.ScheduleId) ct.ScheduleId = f.Id;
                        ct.IsActive = cSource.IsActive;
                        _appContext.EmployeeScheduleLocation.Update(ct);
                        _appContext.SaveChanges();
                    }
                });

                employeeSchedule.Slots.ToList().ForEach(cSource =>
                {
                    if (cSource.IsActive || cSource.Id > 0)
                    {
                        var ct = _appContext.EmployeeScheduleSlot.FirstOrDefault(c => cSource.Id == c.Id) ?? new EmployeeScheduleSlot();
                        ct.CopyFrom(cSource);
                        if (f.Id != ct.ScheduleId) ct.ScheduleId = f.Id;
                        ct.IsActive = cSource.IsActive;
                        _appContext.EmployeeScheduleSlot.Update(ct);
                        _appContext.SaveChanges();
                    }
                });

                employeeSchedule.Infos.ToList().ForEach(cSource =>
                {
                    if ((cSource.IsActive || cSource.extend || cSource.noDisplay) || cSource.Id > 0)
                    {
                        var ct = _appContext.EmployeeScheduleInfo.FirstOrDefault(c => cSource.Id == c.Id) ?? new EmployeeScheduleInfo();
                        ct.CopyFrom(cSource);
                        if (f.Id != ct.ScheduleId) ct.ScheduleId = f.Id;
                        ct.IsActive = cSource.IsActive;
                        _appContext.EmployeeScheduleInfo.Update(ct);
                        _appContext.SaveChanges();
                    }
                });

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


        public async Task<BaseOperationResponse> DeleteAsync(int employeeScheduleId)
        {
            var result = new BaseOperationResponse();
            var employeeSchedule = await GetSingleOrDefaultAsync(r => r.Id == employeeScheduleId);

            if (employeeSchedule != null)
                return await Delete(employeeSchedule);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(EmployeeSchedule employeeSchedule)
        {
            var result = new BaseOperationResponse();
            SoftDelete(employeeSchedule);
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

        public async Task<int> GetOrCreateByCode(EmployeeSchedule data)
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
