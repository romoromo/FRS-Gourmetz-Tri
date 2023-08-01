using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface INotificationSettingRepository : IRepository<NotificationSetting>
    {
        Task<PagedEntity<NotificationSetting>> GetNotificationSettingsAsync(BaseFilter filter);
        Task<BaseOperationResponse> CreateAsync(NotificationSetting notification);
        Task<BaseOperationResponse> DeleteAsync(int notificationId);
        Task<NotificationSetting> GetByIdAsync(int id);
        Task<NotificationSetting> GetByTypeAsync(NotificationSettingType type);
        Task<BaseOperationResponse> UpdateAsync(NotificationSetting notification);
    }
}
