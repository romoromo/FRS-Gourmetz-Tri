using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using DAL;
using DAL.Core;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.Helpers;
using FRS.Hubs;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation;

namespace FRS.Controllers
{
    //[ApiKeyAuthorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class NotificationController : BaseController
    {
        readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly IAccountManager _accountManager;
        private IHubContext<UserHub> _userHub;
        private readonly IEmailSender _emailSender;
        private readonly IStudentService _studentService;
        private readonly IConfiguration _configuration;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService, IAccountManager accountManager, IHubContext<UserHub> userHub,
            IEmailSender emailSender, IConfiguration configuration, IStudentService studentService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _accountManager = accountManager;
            _userHub = userHub;
            _emailSender = emailSender;
            _studentService = studentService;
            _configuration = configuration;
        }

        #region Notifications
        #region Sieved
        
        [HttpGet("notifications/sieve/list")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetNotifications(BaseFilter filter)
        {
            var notifications = await _notificationService.GetNotificationsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<NotificationViewModel>>(notifications));
        }

        #endregion

        [HttpGet("notifications/{id:int}")]
        [ProducesResponseType(200, Type = typeof(NotificationViewModel))]
        public async Task<IActionResult> GetNotification(int id)
        {
            var notification = await _notificationService.GetNotificationById(id);
            return Ok(Mapper.Map<NotificationViewModel>(notification));
        }

        [HttpPost("")]
        [ProducesResponseType(201, Type = typeof(NotificationViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationViewModel notificationVM)
        {
            if (ModelState.IsValid)
            {
                if (notificationVM == null)
                    return BadRequest($"{nameof(notificationVM)} cannot be null");


                var notification = Mapper.Map<NotificationDTO>(notificationVM);

                var result = await _notificationService.CreateAsync(notification);
                if (result.IsSuccess)
                {
                    notificationVM = Mapper.Map<NotificationViewModel>(result.Data);
                    var user = await _accountManager.GetUserByIdAsync(notificationVM.UserId);

                    if(user != null && !string.IsNullOrEmpty(user.Email))
                        await _userHub.Clients.Group(user.Email).SendAsync("NotificationTriggered", notificationVM);

                    
                    return CreatedAtAction("GetNotificationById", new { id = notificationVM.Id }, notificationVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [HttpPost("notifications/read")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateReadNotifications([FromBody] List<int> notificationIds)
        {
            var result = await _notificationService.UpdateReadNotifications(notificationIds);
            return Ok(result);
        }

        [HttpGet("notifications/student/trigger")]
        //[Authorize(Authorization.Policies.ManageAllInterestGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TriggerStudentNotification(int notificationEventId, string email, int? userId)
        {
            var template = await this._notificationService.GetNotificationEventByIdAsync(notificationEventId);
            if (template == null)
                return NotFound(notificationEventId);

            ApplicationUser user = null;
            if (!string.IsNullOrEmpty(email))
            {
                user = await _accountManager.GetUserByEmailAsync(email);
            }
            else
            {
                if (userId.HasValue)
                {
                    user = await _accountManager.GetUserByIdAsync(userId.Value);
                }
            }

            if(user == null)
            {
                return NotFound(userId);
            }
            
            var notification = new NotificationDTO
            {
                Body = template.Template,
                Date = DateTime.Now,
                EventId = notificationEventId,
                Header = template.Title,
                UserId = user.Id
            };

            var response = await _notificationService.CreateAsync(notification);

            if (response.IsSuccess && !string.IsNullOrEmpty(email))
                await _userHub.Clients.Group(email).SendAsync("NotificationTriggered", response.Data);

            return Ok(response);
        }

        #endregion

        #region Notification Events

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("notificationevents/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllNotificationEventsPolicy)]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetNotificationEvents(BaseFilter filter)
        {
            var results = await this._notificationService.GetNotificationEventsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<NotificationEventDTO>>(results));
        }

        #endregion

        [HttpPost("notificationevents")]
        //[Authorize(Authorization.Policies.ManageAllNotificationEventsPolicy)]
        [ProducesResponseType(201, Type = typeof(NotificationEventDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateNotificationEvent([FromBody] NotificationEventDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._notificationService.CreateNotificationEventAsync(dto);
                if (result.IsSuccess)
                {
                    NotificationEventDTO vm = Mapper.Map<NotificationEventDTO>(result.Data);
                    return CreatedAtAction("GetNotificationEventById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("notificationevents/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllNotificationEventsPolicy)]
        [ProducesResponseType(200, Type = typeof(NotificationEventDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteNotificationEvent(int id)
        {
            var dto = await this._notificationService.GetNotificationEventByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._notificationService.DeleteNotificationEventAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("notificationevents/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllNotificationEventsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateNotificationEvent(string id, [FromBody] NotificationEventDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._notificationService.GetNotificationEventByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._notificationService.UpdateNotificationEventAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("notifications/trigger/noorder")]
        [ProducesResponseType(200, Type = typeof(NotificationViewModel))]
        public async Task<IActionResult> TriggerNoOrderNotifications(DayOfWeek dayOfWeek = DayOfWeek.Friday)
        {
            try
            {
                var setting = await this._notificationService.GetNotificationSettingByType(NotificationSettingType.NO_ORDER);
                if(setting != null)
                {
                    dayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), setting.DayEnabled, true);
                }

                // process no order for next week notification
                var today = DateTime.Today;
                if (today.DayOfWeek == dayOfWeek)
                {
                    int daysToAdd = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
                    var from = today.AddDays(daysToAdd);
                    var to = from.AddDays(4);
                    var studentsWithNoOrderNextWeek = await _studentService.GetStudentsWithNoOrder(today, to);
                    string template = setting?.Template;

                    string title = "Forgot to order meals for next week?";
                    foreach (var student in studentsWithNoOrderNextWeek)
                    {
                        template = EmailTemplates.GetNoOrderNextWeek(template, student.Name, from.ToShortDateString(), to.ToShortDateString(), today.Date.ToShortDateString());

                        //send email
                        //string enableNotificationEmail = _configuration["AppSettings:NOTIFICATION_EMAIL_ENABLED"];
                        if (setting != null && setting.IsEmailEnabled && !string.IsNullOrEmpty(student.Email) && student.isNotifNoOrderMadeForNextWeek)
                        {
                            bool allowedEmail = true;
                            //check for whitelisted emails
                            if (!string.IsNullOrEmpty(setting.AllowedEmails))
                            {
                                allowedEmail = setting.AllowedEmails.Split(";")
                                                .Select(e => e.Trim()).Any(e => e.Equals(student.Email, StringComparison.OrdinalIgnoreCase));
                            }

                            if(allowedEmail)
                                await _emailSender.SendEmailAsync(student.Name, student.Email, title, template);
                        }

                        //string enableNotificationAlert = _configuration["AppSettings:NOTIFICATION_ALERT_ENABLED"];
                        if (setting != null && setting.IsAlertEnabled && student.isNotifNoOrderMadeForNextWeek)
                        {
                            //int enableNotificationAlertEventId = Convert.ToInt32(_configuration["AppSettings:NOTIFICATION_ALERT_EVENT_ID"]);
                            //var template = await this._notificationService.GetNotificationEventByIdAsync(enableNotificationAlertEventId);

                            if (student.UserId.HasValue)
                            {
                                var notification = new NotificationDTO
                                {
                                    Body = template,
                                    Date = DateTime.Now,
                                    //EventId = enableNotificationAlertEventId,
                                    Header = title,
                                    UserId = student.UserId.Value
                                };

                                var response = await _notificationService.CreateAsync(notification);

                                if (response.IsSuccess && !string.IsNullOrEmpty(student.Email))
                                    await _userHub.Clients.Group(student.Email).SendAsync("NotificationTriggered", response.Data);
                            }
                        }
                    }

                    await _notificationService.BulkUpdateUserAlertAsync(studentsWithNoOrderNextWeek.Where(e => e.UserId.HasValue)
                                .Select(e => e.UserId.Value).ToList(), UserAlertType.NO_ORDER);
                }

                

                return Ok(new BaseOperationResponse { IsSuccess = true });
            }
            catch (Exception ex)
            {
                _logger.LogError("No Order TriggerNotifications ERROR:", ex.InnerException?.StackTrace, ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("notifications/trigger/abandonedcart1")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> TriggerAbandonedCart1Notifications()
        {
            try
            {
                string title1 = "Cart Reminder - Did you leave something behind?";
                string template1 = string.Empty;
                string baseUrl = _configuration["AppSettings:baseUrl"];
                int hoursLeft = 4;
                var setting1 = await this._notificationService.GetNotificationSettingByType(NotificationSettingType.ABANDONED_CART_1);
                if (setting1 != null)
                {
                    if (!string.IsNullOrEmpty(setting1.Subject))
                    {
                        title1 = setting1.Subject;
                    }

                    if (!string.IsNullOrEmpty(setting1.Template))
                    {
                        template1 = setting1.Template;
                    }

                    hoursLeft = setting1.NumHoursLeftCutoff;
                }

                template1 = EmailTemplates.GetStudentsWithAbandonedCart1(template1, baseUrl);
                // process abandoned cart 4 hours abandoned
                var studentsWithAbandonedCart1 = await _studentService.GetStudentsWithAbandonedCart1(hoursLeft);
                var students = studentsWithAbandonedCart1.Where(e => !string.IsNullOrEmpty(e.Email));
                foreach (var student in students)
                {
                    //send email
                    // enableNotificationEmail = _configuration["AppSettings:NOTIFICATION_EMAIL_ENABLED"];
                    if (setting1 != null && setting1.IsEmailEnabled && !string.IsNullOrEmpty(student.Email) && student.isNotifAbandonCart)
                    {
                        bool allowedEmail = true;
                        //check for whitelisted emails
                        if (!string.IsNullOrEmpty(setting1.AllowedEmails))
                        {
                            allowedEmail = setting1.AllowedEmails.Split(";")
                                            .Select(e => e.Trim()).Any(e => e.Equals(student.Email, StringComparison.OrdinalIgnoreCase));
                        }

                        if (allowedEmail)
                            await _emailSender.SendEmailAsync(student.Name, student.Email, title1, template1);
                    }

                    //string enableNotificationAlert = _configuration["AppSettings:NOTIFICATION_ALERT_ENABLED"];
                    if (setting1 != null && setting1.IsAlertEnabled && student.isNotifAbandonCart)
                    {
                        //int enableNotificationAlertEventId = Convert.ToInt32(_configuration["AppSettings:NOTIFICATION_ALERT_EVENT_ID"]);
                        //var template = await this._notificationService.GetNotificationEventByIdAsync(enableNotificationAlertEventId);

                        if (student.UserId.HasValue)
                        {
                            var notification = new NotificationDTO
                            {
                                Body = template1,
                                Date = DateTime.Now,
                                //EventId = enableNotificationAlertEventId,
                                Header = title1,
                                UserId = student.UserId.Value
                            };

                            var response = await _notificationService.CreateAsync(notification);

                            if (response.IsSuccess && !string.IsNullOrEmpty(student.Email))
                                await _userHub.Clients.Group(student.Email).SendAsync("NotificationTriggered", response.Data);
                        }
                    }
                }

                await _notificationService.BulkUpdateUserAlertAsync(students.Where(e => e.UserId.HasValue)
                                .Select(e => e.UserId.Value).ToList(), UserAlertType.ABANDONED_CART_1);

                return Ok(new BaseOperationResponse { IsSuccess = true });
            }
            catch (Exception ex)
            {
                _logger.LogError("TriggerNotifications ERROR:", ex.InnerException?.StackTrace, ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("notifications/trigger/abandonedcart2")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> TriggerAbandonedCart2Notifications()
        {
            try
            {
                string template2 = string.Empty;
                string title2 = "Cart Reminder - Hurry! Check out your items before the cut off time.";
                string baseUrl = _configuration["AppSettings:baseUrl"];
                int daysLeft = 4;
                var setting2 = await this._notificationService.GetNotificationSettingByType(NotificationSettingType.ABANDONED_CART_2);
                if (setting2 != null)
                {
                    if (!string.IsNullOrEmpty(setting2.Subject))
                    {
                        title2 = setting2.Subject;
                    }

                    if (!string.IsNullOrEmpty(setting2.Template))
                    {
                        template2 = setting2.Template;
                    }

                    daysLeft = setting2.NumDaysBeforeCutoff;
                }

                
                template2 = EmailTemplates.GetStudentsWithAbandonedCart2(template2, baseUrl);
                // process abandoned cart 4-days before cutoff
                var students = (await _studentService.GetStudentsWithAbandonedCart2(daysLeft))
                                                        .Where(e => !string.IsNullOrEmpty(e.Email));
                foreach (var student2 in students)
                {
                    //send email
                    //string enableNotificationEmail = _configuration["AppSettings:NOTIFICATION_EMAIL_ENABLED"];
                    if (setting2 != null && setting2.IsEmailEnabled && !string.IsNullOrEmpty(student2.Email) && student2.isNotifAbandonCart)
                    {
                        bool allowedEmail = true;
                        //check for whitelisted emails
                        if (!string.IsNullOrEmpty(setting2.AllowedEmails))
                        {
                            allowedEmail = setting2.AllowedEmails.Split(";")
                                            .Select(e => e.Trim()).Any(e => e.Equals(student2.Email, StringComparison.OrdinalIgnoreCase));
                        }

                        if (allowedEmail)
                            await _emailSender.SendEmailAsync(student2.Name, student2.Email, title2, template2);
                    }

                    //string enableNotificationAlert = _configuration["AppSettings:NOTIFICATION_ALERT_ENABLED"];
                    if (setting2 != null && setting2.IsAlertEnabled && student2.isNotifAbandonCart)
                    {
                        //int enableNotificationAlertEventId = Convert.ToInt32(_configuration["AppSettings:NOTIFICATION_ALERT_EVENT_ID"]);
                        //var template = await this._notificationService.GetNotificationEventByIdAsync(enableNotificationAlertEventId);

                        if (student2.UserId.HasValue)
                        {
                            var notification = new NotificationDTO
                            {
                                Body = template2,
                                Date = DateTime.Now,
                                //EventId = enableNotificationAlertEventId,
                                Header = title2,
                                UserId = student2.UserId.Value
                            };

                            var response = await _notificationService.CreateAsync(notification);

                            if (response.IsSuccess && !string.IsNullOrEmpty(student2.Email))
                                await _userHub.Clients.Group(student2.Email).SendAsync("NotificationTriggered", response.Data);
                        }
                    }
                }

                await _notificationService.BulkUpdateUserAlertAsync(students.Where(e => e.UserId.HasValue)
                                .Select(e => e.UserId.Value).ToList(), UserAlertType.ABANDONED_CART_2);

                return Ok(new BaseOperationResponse { IsSuccess = true });
            }
            catch (Exception ex)
            {
                _logger.LogError("TriggerNotifications ERROR:", ex.InnerException?.StackTrace, ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("notifications/trigger/notcollected")]
        [ProducesResponseType(200, Type = typeof(NotificationViewModel))]
        public async Task<IActionResult> TriggerNoCollectedNotifications(int daysPassed = 1)
        {
            try
            {
                var setting = await this._notificationService.GetNotificationSettingByType(NotificationSettingType.ORDER_NOT_COLLECTED);
                
                // process orders not collected
                var students = (await _studentService.GetStudentsWithOrdersNotCollected(daysPassed))
                                .Where(e => e.Student != null && !string.IsNullOrEmpty(e.Student.Email));

                foreach (var student in students)
                {
                    if (setting != null && student.TokenOrders.Any())
                    {
                        string title = "Forgot to collect order meals";
                        if (!string.IsNullOrEmpty(setting.Subject))
                        {
                            title = setting.Subject;
                        }

                        string template = EmailTemplates.GetStudentsWithOrdersNotCollected(setting.Template, student.Student.Name, student.TokenOrders.First().DeliveryDate.ToShortDateString());

                        //send email
                        // enableNotificationEmail = _configuration["AppSettings:NOTIFICATION_EMAIL_ENABLED"];
                        if (setting.IsEmailEnabled && !string.IsNullOrEmpty(student.Student.Email) && student.Student.isNotifMissedCollection)
                        {
                            bool allowedEmail = true;
                            //check for whitelisted emails
                            if (!string.IsNullOrEmpty(setting.AllowedEmails))
                            {
                                allowedEmail = setting.AllowedEmails.Split(";")
                                                .Select(e => e.Trim()).Any(e => e.Equals(student.Student.Email, StringComparison.OrdinalIgnoreCase));
                            }

                            if (allowedEmail)
                                await _emailSender.SendEmailAsync(student.Student.Name, student.Student.Email, title, template);
                        }

                        //string enableNotificationAlert = _configuration["AppSettings:NOTIFICATION_ALERT_ENABLED"];
                        if (setting.IsAlertEnabled && student.Student.isNotifMissedCollection)
                        {
                            //int enableNotificationAlertEventId = Convert.ToInt32(_configuration["AppSettings:NOTIFICATION_ALERT_EVENT_ID"]);
                            //var template = await this._notificationService.GetNotificationEventByIdAsync(enableNotificationAlertEventId);

                            if (student.Student.UserId.HasValue)
                            {
                                var notification = new NotificationDTO
                                {
                                    Body = template,
                                    Date = DateTime.Now,
                                    //EventId = enableNotificationAlertEventId,
                                    Header = title,
                                    UserId = student.Student.UserId.Value
                                };

                                var response = await _notificationService.CreateAsync(notification);

                                if (response.IsSuccess && !string.IsNullOrEmpty(student.Student.Email))
                                    await _userHub.Clients.Group(student.Student.Email).SendAsync("NotificationTriggered", response.Data);
                            }
                        }
                    }
                }

                await _notificationService.BulkUpdateUserAlertAsync(students.Where(e => e.Student.UserId.HasValue)
                                .Select(e => e.Student.UserId.Value).ToList(), UserAlertType.ORDER_NOT_COLLECTED);

                return Ok(new BaseOperationResponse { IsSuccess = true });
            }
            catch (Exception ex)
            {
                _logger.LogError("No Order TriggerNotifications ERROR:", ex.InnerException?.StackTrace, ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }

        #region Notification Setting

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("notificationsettings/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllNotificationEventsPolicy)]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetNotificationSettings(BaseFilter filter)
        {
            var results = await this._notificationService.GetNotificationSettingsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<NotificationSettingDTO>>(results));
        }

        #endregion

        [HttpPost("notificationsettings/update/bulk")]
        //[Authorize(Authorization.Policies.ManageAllNotificationEventsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> BulkUpdateNotificationSetting([FromBody] List<NotificationSettingViewModel> model)
        {
            var dto = Mapper.Map<List<NotificationSettingDTO>>(model);
            var result = await this._notificationService.BulkUpdateNotificationSettingAsync(dto);
            return Ok(result);
        }

        #endregion
    }
}