using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class NotificationService : INotificationService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedEntity<NotificationDTO>> GetNotificationsAsync(BaseFilter filter)
        {
            //Get UserId from filter
            var match = Regex.Match(filter.Filters, @"\(UserId\)==(\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var parsedId))
                await this._uow.Notifications.InitNotificationByUserId(parsedId);

            var result = _mapper.Map<PagedEntity<NotificationDTO>>(await this._uow.Notifications.GetNotificationsAsync(filter));
            return result;
        }

        public async Task<List<NotificationDTO>> GetNotificationsByUser(int userId)
        {
            var result = _mapper.Map<List<NotificationDTO>>(await this._uow.Notifications.FindAsync(e => e.UserId == userId));
            return result;
        }

        public async Task<NotificationDTO> GetNotificationById(int id)
        {
            var result = _mapper.Map<NotificationDTO>(await this._uow.Notifications.GetByIdAsync(id));
            return result;
        }

        public async Task<BaseOperationResponse> CreateAsync(NotificationDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Notifications.CreateAsync(_mapper.Map<Notification>(dto));
            if (result != null && result.Data is Notification)
            {
                result.Data = _mapper.Map<NotificationDTO>(result.Data as Notification);
            }
            return result;
        }

        public async Task<BaseOperationResponse> UpdateReadNotifications(List<int> notificationIds)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Notifications.UpdateReadNotifications(notificationIds);
            return result;
        }

        #region Notification Events

        public async Task<PagedEntity<NotificationEventDTO>> GetNotificationEventsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<NotificationEventDTO>>(await this._uow.NotificationEvents.GetNotificationEventsAsync(filter));
            return result;
        }

        public async Task<NotificationEventDTO> GetNotificationEventByIdAsync(int id)
        {
            return _mapper.Map<NotificationEventDTO>(await this._uow.NotificationEvents.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateNotificationEventAsync(NotificationEventDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.NotificationEvents.CreateAsync(_mapper.Map<NotificationEvent>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateNotificationEventAsync(NotificationEventDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.NotificationEvents.UpdateAsync(_mapper.Map<NotificationEvent>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteNotificationEventAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.NotificationEvents.DeleteAsync(id);
            return result;
        }

        #endregion

        #region
        public async Task<PagedEntity<NotificationSettingDTO>> GetNotificationSettingsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<NotificationSettingDTO>>(await this._uow.NotificationSettings.GetNotificationSettingsAsync(filter));
            return result;
        }

        public async Task<NotificationSettingDTO> GetNotificationSettingByType(NotificationSettingType type)
        {
            var result = _mapper.Map<NotificationSettingDTO>(await this._uow.NotificationSettings.GetByTypeAsync(type));
            return result;
        }

        public async Task<NotificationSettingDTO> GetNotificationSettingById(int id)
        {
            var result = _mapper.Map<NotificationSettingDTO>(await this._uow.NotificationSettings.GetByIdAsync(id));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateNotificationSettingAsync(NotificationSettingDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.NotificationSettings.UpdateAsync(_mapper.Map<NotificationSetting>(dto));
            if (result != null && result.Data is NotificationSetting)
            {
                result.Data = _mapper.Map<NotificationSettingDTO>(result.Data as NotificationSetting);
            }
            return result;
        }

        public async Task<BaseOperationResponse> BulkUpdateNotificationSettingAsync(List<NotificationSettingDTO> dto)
        {
            var result = new BaseOperationResponse();
            foreach (var setting in dto)
            {
                result = await this._uow.NotificationSettings.UpdateAsync(_mapper.Map<NotificationSetting>(setting));
                result.Data = null;
            }

            return result;
        }

        public async Task<BaseOperationResponse> CreateUserAlertAsync(UserOrderAlertDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Notifications.CreateUserAlertAsync(_mapper.Map<UserOrderAlert>(dto));

            return result;
        }

        public async Task<BaseOperationResponse> UpdateUserAlertAsync(UserOrderAlertDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Notifications.UpdateUserAlertAsync(_mapper.Map<UserOrderAlert>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> BulkUpdateUserAlertAsync(List<int> userIds, UserAlertType type)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Notifications.BulkUpdateUserAlertAsync(userIds, type);
            return result;
        }
        #endregion
    }
}
