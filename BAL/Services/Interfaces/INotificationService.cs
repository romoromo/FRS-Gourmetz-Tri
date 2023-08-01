using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDTO>> GetNotificationsByUser(int userId);
        Task<NotificationDTO> GetNotificationById(int id);
        Task<BaseOperationResponse> CreateAsync(NotificationDTO dto);
        Task<PagedEntity<NotificationDTO>> GetNotificationsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateReadNotifications(List<int> notificationIds);

        Task<BaseOperationResponse> CreateNotificationEventAsync(NotificationEventDTO dto);
        Task<BaseOperationResponse> DeleteNotificationEventAsync(int id);
        Task<NotificationEventDTO> GetNotificationEventByIdAsync(int id);
        Task<PagedEntity<NotificationEventDTO>> GetNotificationEventsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateNotificationEventAsync(NotificationEventDTO dto);

        Task<NotificationSettingDTO> GetNotificationSettingById(int id);
        Task<BaseOperationResponse> UpdateNotificationSettingAsync(NotificationSettingDTO dto);
        Task<PagedEntity<NotificationSettingDTO>> GetNotificationSettingsAsync(BaseFilter filter);
        Task<BaseOperationResponse> BulkUpdateNotificationSettingAsync(List<NotificationSettingDTO> dto);
        Task<NotificationSettingDTO> GetNotificationSettingByType(NotificationSettingType type);

        Task<BaseOperationResponse> CreateUserAlertAsync(UserOrderAlertDTO dto);
        Task<BaseOperationResponse> UpdateUserAlertAsync(UserOrderAlertDTO dto);
        Task<BaseOperationResponse> BulkUpdateUserAlertAsync(List<int> userIds, UserAlertType type);
    }
}