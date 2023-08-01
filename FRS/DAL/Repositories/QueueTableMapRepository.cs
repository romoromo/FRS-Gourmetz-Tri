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
    public class QueueTableMapRepository : Repository<QueueTableMap>, IQueueTableMapRepository
    {
        public const string DevideId = "DeviceId";
        //public const string QueueNo = "QueueNo";
        //public const string Queueid = "Queueid";
        //public const string CallAction = "CallAction";

        public const string call = "call";
        public const string clear = "clear";
        public const string missedqueue = "missed queue";
        public const string silentcall = "silent call";
        public const string queueid = "queueid";
        public const string devideId = "deviceId";
        public const string stationId = "stationId";
        public const string currentDisplay = "currentDisplay";

        public QueueTableMapRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<QueueTableMap> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<QueueTableMap> All()
        {
            return GetAll()
                .OrderBy(c => c.QueueId)
                .ToList();
        }

        public async Task<List<QueueTableMap>> GetQueueTableMapsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<QueueTableMap> query = _appContext.QueueTableMap
                .Where(e => e.IsActive)
                .OrderBy(r => r.QueueId);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<List<QueueTableMap>> DeviceLists(QueueLog param)
        {
            IQueryable<QueueTableMap> query = _appContext.QueueTableMap
                .Where(e => e.IsActive && e.QueueId == param.Queueid);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateLogAsync(QueueLog queueLog)
        {
            var result = new BaseOperationResponse();

            var f = await _appContext.QueueLog.AddAsync(queueLog);
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

        public async Task<BaseOperationResponse> CreateAsync(QueueTableMap queueTableMap)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.QueueId == queueTableMap.QueueId && e.DeviceIdentifier == queueTableMap.DeviceIdentifier && e.StationId == queueTableMap.StationId && e.IsActive).Result)
            {
                result.Message = "Device Id and Pair Id already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await AddAsync(queueTableMap);
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

        public async Task<BaseOperationResponse> UpdateAsync(QueueTableMap queueTableMap)
        {
            var result = new BaseOperationResponse();

            if (queueTableMap.IsActive && Exists(e => (e.QueueId == queueTableMap.QueueId && e.DeviceIdentifier == queueTableMap.DeviceIdentifier && e.StationId == queueTableMap.StationId && e.IsActive) && e.Id != queueTableMap.Id).Result)
            {
                result.Message = "Device Id and Pair Id already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await GetSingleOrDefaultAsync(e => e.Id == queueTableMap.Id);

            f.CopyFrom(queueTableMap);
            f.IsActive = queueTableMap.IsActive;
            Update(f);
            f.IsActive = queueTableMap.IsActive;
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


        public async Task<BaseOperationResponse> DeleteAsync(int queueTableMapId)
        {
            var result = new BaseOperationResponse();
            var queueTableMap = await GetSingleOrDefaultAsync(r => r.Id == queueTableMapId);

            if (queueTableMap != null)
                return await Delete(queueTableMap);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(QueueTableMap queueTableMap)
        {
            var result = new BaseOperationResponse();
            SoftDelete(queueTableMap);
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

        public async Task<int> GetOrCreateByCode(QueueTableMap data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.QueueId)) return 0;

            var f = await GetSingleOrDefaultAsync(c => c.QueueId == data.QueueId && c.IsActive);

            if (f == null)
            {
                await CreateAsync(data);
                f = await GetSingleOrDefaultAsync(c => c.QueueId == data.QueueId && c.IsActive);
            }

            return f.Id;
        }

        public async Task<BaseOperationResponse> CallQueue(QueueLog param)
        {
            var result = new BaseOperationResponse();

            var queueTableMaps = _appContext.QueueTableMap.Where(d => d.QueueId == param.Queueid).ToList();

            if (queueTableMaps.Count() == 0)
            {
                result.Message = "Queue Id not exist!";
                result.IsSuccess = false;
                return result;
            }

            foreach (var q in queueTableMaps)
            {
                if (param.CallAction == call || param.CallAction == silentcall) q.LastCall = param.QueueNo;
                else if (param.CallAction == clear) q.LastCall = "";

                q.LastReturn = "NACK";
            }

            try
            {
                await _appContext.SaveChangesAsync();


                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = "Failed to save! " + ex.Message;
                result.IsSuccess = false;
            }

            await CreateLogAsync(param);

            return result;
        }

        public async Task QueueReturn(Dictionary<string, string> param)
        {
            var result = new BaseOperationResponse();

            var queueTableMap = _appContext.QueueTableMap.FirstOrDefault(d => d.QueueId == param[queueid] && d.Device != null && d.Device.Code == param[devideId] && d.StationId == param[stationId] && d.IsActive);

            if (queueTableMap != null)
            {
                queueTableMap.CurrentDisplay = param[currentDisplay];
                queueTableMap.LastReturn = "OK";

                await _appContext.SaveChangesAsync();
            }
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
