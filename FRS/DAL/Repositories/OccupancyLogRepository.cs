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
    public class OccupancyLogRepository : Repository<OccupancyLog>, IOccupancyLogRepository
    {
        public OccupancyLogRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<OccupancyLog> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<OccupancyLog> All()
        {
            return GetAll()
                .OrderByDescending(c => c.Datetime)
                .ToList();
        }

        public async Task<List<OccupancyLog>> GetOccupancyLogsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<OccupancyLog> query = _appContext.OccupancyLog
                .Where(e => e.IsActive)
                .OrderByDescending(c => c.Datetime);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<List<OccupancyLog>> GetOccupancyLogsFilter(DateTime? startTime, DateTime? endTime, string deviceId, string sensorId, string status)
        {
            if (startTime == null) startTime = DateTime.Today;

            IQueryable<OccupancyLog> query = _appContext.OccupancyLog
                .Where(e => e.IsActive)
                .OrderByDescending(c => c.Datetime);

            if (startTime != null) query = query.Where(e => e.Datetime >= startTime);
            if (endTime != null) query = query.Where(e => e.Datetime <= endTime);

            if (!string.IsNullOrWhiteSpace(deviceId)) query = query.Where(e => EF.Functions.Like(e.DeviceId, $"%{deviceId}%"));
            if (!string.IsNullOrWhiteSpace(sensorId)) query = query.Where(e => EF.Functions.Like(e.SensorId, $"%{sensorId}%"));
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(e => EF.Functions.Like(e.Status, $"%{status}%"));

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<string> GetLastStatus(string deviceId, string sensorId)
        {
            IQueryable<OccupancyLog> query = _appContext.OccupancyLog
                .Where(e => e.IsActive)
                .OrderByDescending(c => c.Datetime);

            if (!string.IsNullOrWhiteSpace(deviceId)) query = query.Where(e => e.DeviceId == deviceId);
            if (!string.IsNullOrWhiteSpace(sensorId)) query = query.Where(e => e.SensorId == sensorId);

            var o = query.Take(1).ToList();

            if (o.Count() > 0) return o[0].Status; 

            return "";
        }

        public async Task<BaseOperationResponse> CreateAsync(OccupancyLog occupancyLog)
        {
            var result = new BaseOperationResponse();

            if (occupancyLog.Datetime == null) occupancyLog.Datetime = DateTime.Now;

            var f = await AddAsync(occupancyLog);
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

        public async Task<BaseOperationResponse> UpdateAsync(OccupancyLog occupancyLog)
        {
            var result = new BaseOperationResponse();

            

            var f = await GetSingleOrDefaultAsync(e => e.Id == occupancyLog.Id);

            f.CopyFrom(occupancyLog);
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


        public async Task<BaseOperationResponse> DeleteAsync(int occupancyLogId)
        {
            var result = new BaseOperationResponse();
            var occupancyLog = await GetSingleOrDefaultAsync(r => r.Id == occupancyLogId);

            if (occupancyLog != null)
                return await Delete(occupancyLog);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(OccupancyLog occupancyLog)
        {
            var result = new BaseOperationResponse();
            SoftDelete(occupancyLog);
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
