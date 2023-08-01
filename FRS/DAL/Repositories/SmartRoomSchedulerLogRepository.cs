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

namespace DAL.Repositories
{
    public class SmartRoomSchedulerLogRepository : ISmartRoomSchedulerLogRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private readonly ApplicationDbContext _appContext;
        private int? _currentInstitutionId;
        public SmartRoomSchedulerLogRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
            this._appContext = context;
        }

        #region Sieved
        public async Task<PagedEntity<SmartRoomSchedulerLog>> GetSmartRoomSchedulerLogsAsync(BaseFilter filter)
        {
            IQueryable<SmartRoomSchedulerLog> query = _appContext.SmartRoomSchedulerLogs;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<SmartRoomSchedulerLog>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion
        public async Task<SmartRoomSchedulerLog> GetByIdAsync(int id)
        {
            return await this._appContext.SmartRoomSchedulerLogs.FindAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(SmartRoomSchedulerLog smartRoomSchedulerLog)
        {
            var result = new BaseOperationResponse();
            var f = await this._appContext.SmartRoomSchedulerLogs.AddAsync(smartRoomSchedulerLog);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f.Entity;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> BulkCreateAsync(List<SmartRoomSchedulerLog> smartRoomSchedulerLogs)
        {
            var result = new BaseOperationResponse();

            await this._appContext.SmartRoomSchedulerLogs.AddRangeAsync(smartRoomSchedulerLogs);
            if (await _appContext.SaveChangesAsync() > 0)
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
    }
}
