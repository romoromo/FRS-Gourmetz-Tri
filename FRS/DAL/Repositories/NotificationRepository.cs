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

namespace DAL.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public NotificationRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Notification>> GetNotificationsAsync(BaseFilter filter)
        {
            IQueryable<Notification> query = _appContext.Notifications;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<Notification>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion

        public async Task<Notification> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Notification> All()
        {
            return GetAll()
                .OrderBy(c => c.Header)
                .ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(Notification notification)
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
                    result.Message = "Failed to save notification!";
                    result.IsSuccess = false;
                }
                
            }
            catch (Exception)
            {

            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateReadNotifications(List<int> notificationIds)
        {
            var result = new BaseOperationResponse();

            var notifications = await FindAsync(e => notificationIds.Any(a => a == e.Id));

            foreach(var notification in notifications)
            {
                notification.IsRead = true;
                Update(notification);
            }
            
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to save notification!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Notification notification)
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
                result.Message = "Failed to save notification!";
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
            result.Message = "Notification not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Notification notification)
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

        #region UserAlerts
        public async Task<BaseOperationResponse> CreateUserAlertAsync(UserOrderAlert alert)
        {
            var result = new BaseOperationResponse();
            try
            {
                var f = await _appContext.UserOrderAlerts.AddAsync(alert);
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
            catch (Exception)
            {

            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateUserAlertAsync(UserOrderAlert alert)
        {
            var result = new BaseOperationResponse();

            var f = await _appContext.UserOrderAlerts.FirstOrDefaultAsync(e => e.Id == alert.Id);

            f.CopyFrom(alert);
            _appContext.UserOrderAlerts.Update(f);
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

        public async Task<BaseOperationResponse> BulkUpdateUserAlertAsync(List<int> userIds, UserAlertType type)
        {
            var result = new BaseOperationResponse();
            var now = DateTime.Now;
            foreach(var userId in userIds)
            {
                var users = _appContext.UserOrderAlerts.Where(e => e.UserId == userId);
                UserOrderAlert alert = null;
                bool isNew = false;
                if (type == UserAlertType.ABANDONED_CART_1)
                {
                    alert = await users.FirstOrDefaultAsync(e => !e.AbandonedCart1SentDate.HasValue);
                    isNew = alert == null; 
                    alert = alert ?? new UserOrderAlert { UserId = userId };
                    alert.AbandonedCart1SentDate = now; 
                }
                else if (type == UserAlertType.ABANDONED_CART_2)
                {
                    alert = await users.FirstOrDefaultAsync(e => !e.AbandonedCart2SentDate.HasValue);
                    isNew = alert == null;
                    alert = alert ?? new UserOrderAlert { UserId = userId };
                    alert.AbandonedCart2SentDate = now;
                }
                else if(type == UserAlertType.ORDER_NOT_COLLECTED)
                {
                    alert = await users.FirstOrDefaultAsync(e => !e.MissedCollectedSentDate.HasValue);
                    isNew = alert == null;
                    alert = alert ?? new UserOrderAlert { UserId = userId };
                    alert.MissedCollectedSentDate = now;
                }
                else if (type == UserAlertType.NO_ORDER)
                {
                    alert = await users.FirstOrDefaultAsync(e => !e.NoOrderNextWeekSentDate.HasValue);
                    isNew = alert == null;
                    alert = alert ?? new UserOrderAlert { UserId = userId, };
                    alert.NoOrderNextWeekSentDate = now;
                }

                if (isNew)
                {
                    //create
                    await CreateUserAlertAsync(alert);
                }
                else
                {
                    //update
                    await UpdateUserAlertAsync(alert);
                }
            }

            result.IsSuccess = true;
            return result;
        }

        #endregion
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
