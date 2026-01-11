using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        IEnumerable<Notification> All();
        Task<BaseOperationResponse> CreateAsync(Notification notification);
        Task<BaseOperationResponse> DeleteAsync(int notificationId);
        Task<Notification> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Notification notification);
        Task<PagedEntity<Notification>> GetNotificationsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateReadNotifications(List<int> notificationIds);
        Task<BaseOperationResponse> CreateUserAlertAsync(UserOrderAlert alert);
        Task<BaseOperationResponse> UpdateUserAlertAsync(UserOrderAlert alert);
        Task<BaseOperationResponse> BulkUpdateUserAlertAsync(List<int> userIds, UserAlertType type);

        Task InitNotificationByUserId(int? userId);
    }
}
