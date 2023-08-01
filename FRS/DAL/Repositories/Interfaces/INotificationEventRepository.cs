using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface INotificationEventRepository : IRepository<NotificationEvent>
    {
        IEnumerable<NotificationEvent> All();
        Task<BaseOperationResponse> CreateAsync(NotificationEvent NotificationEvent);
        Task<BaseOperationResponse> DeleteAsync(int NotificationEventId);
        Task<NotificationEvent> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(NotificationEvent NotificationEvent);
        Task<PagedEntity<NotificationEvent>> GetNotificationEventsAsync(BaseFilter filter);
    }
}
