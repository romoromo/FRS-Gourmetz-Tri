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
    public class NotificationEventRepository : Repository<NotificationEvent>, INotificationEventRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public NotificationEventRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<NotificationEvent>> GetNotificationEventsAsync(BaseFilter filter)
        {
            IQueryable<NotificationEvent> query = _appContext.NotificationEvents;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<NotificationEvent>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion

        public async Task<NotificationEvent> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<NotificationEvent> All()
        {
            return GetAll();
        }

        public async Task<BaseOperationResponse> CreateAsync(NotificationEvent notificationEvent)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(notificationEvent);
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

        public async Task<BaseOperationResponse> UpdateAsync(NotificationEvent notificationEvent)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == notificationEvent.Id);

            f.CopyFrom(notificationEvent);
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

        public async Task<BaseOperationResponse> DeleteAsync(int notificationEventId)
        {
            var result = new BaseOperationResponse();
            var notificationEvent = await GetSingleOrDefaultAsync(r => r.Id == notificationEventId);

            if (notificationEvent != null)
                return await Delete(notificationEvent);

            result.IsSuccess = false;
            result.Message = "Notification Event not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(NotificationEvent notificationEvent)
        {
            var result = new BaseOperationResponse();
            SoftDelete(notificationEvent);
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
