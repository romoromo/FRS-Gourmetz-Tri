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
    public class NotificationSettingRepository : Repository<NotificationSetting>, INotificationSettingRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public NotificationSettingRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<NotificationSetting>> GetNotificationSettingsAsync(BaseFilter filter)
        {
            IQueryable<NotificationSetting> query = _appContext.NotificationSettings;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<NotificationSetting>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion

        public async Task<NotificationSetting> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<NotificationSetting> GetByTypeAsync(NotificationSettingType type)
        {
            return await GetFirstOrDefaultAsync(e => e.IsActive && e.Type == type);
        }

        public async Task<BaseOperationResponse> CreateAsync(NotificationSetting notification)
        {
            var result = new BaseOperationResponse();
            try
            {
                var f = await AddAsync(notification);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save notification setting!";
                    result.IsSuccess = false;
                }
                
            }
            catch (Exception)
            {

            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(NotificationSetting notification)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == notification.Id);

            f.CopyFrom(notification);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save notification setting!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> DeleteAsync(int notificationId)
        {
            var result = new BaseOperationResponse();
            var notification = await GetSingleOrDefaultAsync(r => r.Id == notificationId);

            if (notification != null)
                return await Delete(notification);

            result.IsSuccess = false;
            result.Message = "Notification setting not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(NotificationSetting notification)
        {
            var result = new BaseOperationResponse();
            SoftDelete(notification);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete notification!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
