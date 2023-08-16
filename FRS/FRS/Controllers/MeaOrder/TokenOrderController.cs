using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using BAL.Services.MealOrder;
using DAL;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.Helpers;
using FRS.Hubs;
using FRS.ViewModels;
using FRS.ViewModels.MealOrder;
using MealOrderPayments.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class TokenOrderController : BaseController
    {
        private ITokenOrderService _service;
        readonly ILogger _logger;
        private readonly IAccountManager _accountManager;
        private OrderController _orderController;
        private readonly IMenuService _menuService;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IEmailSender _emailSender;
        private readonly IHubContext<UserHub> _userHub;
        private readonly IStudentService _studentService;

        public TokenOrderController(ITokenOrderService service, ILogger<TokenOrderController> logger, IAccountManager accountManager, OrderController orderController,
            IMenuService menuService, IConfiguration configuration, INotificationService notificationService, IHubContext<UserHub> userHub, IEmailSender emailSender,
            IStudentService studentService)
        {
            _service = service;
            _logger = logger;
            _accountManager = accountManager;
            _orderController = orderController;
            _menuService = menuService;
            _configuration = configuration;
            _notificationService = notificationService;
            _emailSender = emailSender;
            _userHub = userHub;
            _studentService = studentService;
        }

        #region Token Orders

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("tokenorders/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTokenOrders(RequestFilter filter)
        {
            if (filter.isrequest) await _orderController.QueryAndUpdatePaymentStatus(filter.studentId);

            var results = await this._service.GetTokenOrdersAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<TokenOrderDTO>>(results));
        }

        #endregion

        [HttpGet("tokenorders/get/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(TokenOrderDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTokenOrder(int id)
        {
            var dto = await this._service.GetTokenOrderByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("tokenorders")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(201, Type = typeof(TokenOrderDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTokenOrder([FromBody] TokenOrderDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateTokenOrderAsync(dto);
                if (result.IsSuccess)
                {
                    TokenOrderDTO vm = Mapper.Map<TokenOrderDTO>(result.Data);
                    return CreatedAtAction("GetTokenOrderById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("tokenorders/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(TokenOrderDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTokenOrder(int id)
        {
            var dto = await this._service.GetTokenOrderByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteTokenOrderAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("tokenorders/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTokenOrder(string id, [FromBody] TokenOrderDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetTokenOrderByIdAsync(model.Id);

          
                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateTokenOrderAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #region Fas Meal Oder
        [HttpGet("tokenorders/fas")]
        [ProducesResponseType(201, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateFasTokenOrders(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool clear)
        {
            if (outletId == 0)
                return BadRequest("Outlet not found.");

            var result = await this._service.CreateFasTokenOrdersAsync(outletId, storeId, deliveryDate, deliveryDateTo, dishTypeId, mealSessionId, createdBy, clear);
            if (!result.IsSuccess)
                return BadRequest("The following errors occurred while processing: " + string.Join(", ", result.Message));

            return Ok(result);
        }


        [HttpGet("tokenorders/fas/ordersummary")]
        //[Authorize(Authorization.Policies.ViewAllMealSessionsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<MealSessionMealPeriodDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetFasMealOrderSummary(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo)
        {
            var mealSessions = await this._menuService.GetOutletMealSessions(outletId, deliveryDate, null, null, deliveryDateTo);
            var results = await this._service.GetFasTokenOrderSummaryAsync(outletId, storeId, deliveryDate, deliveryDateTo, mealSessions);
            return Ok(results);
        }
        #endregion

        #region Meal Plan
        [HttpGet("mealplans/student")]
        [AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStudentMealPlan(DateTime? orderDate)
        {
            var dto = await this._studentService.GetStudentMealPlanAsync(orderDate);
            return Ok(dto);
        }

        [HttpGet("tokenorders/mealplan")]
        [ProducesResponseType(201, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMealPlan(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool clear)
        {
            if (outletId == 0)
                return BadRequest("Outlet not found.");

            var result = await this._service.CreateMealPlanAsync(studentGroupId, outletId, storeId, deliveryDate, deliveryDateTo, dishTypeId, mealSessionId, createdBy, clear);
            if (!result.IsSuccess)
                return BadRequest("The following errors occurred while processing: " + string.Join(", ", result.Message));

            return Ok(result);
        }


        [HttpGet("tokenorders/mealplan/ordersummary")]
        //[Authorize(Authorization.Policies.ViewAllMealSessionsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMealPlanSummary(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int mealSessionId)
        {
            //var mealSessions = await this._menuService.GetOutletMealSessions(outletId, deliveryDate, null, null, deliveryDateTo);
            //mealSessions = mealSessions.Where(e => e.MealSessionId == mealSessionId).ToList();
            var results = await this._service.GetMealPlanOrderSummaryAsync(studentGroupId, outletId, storeId, deliveryDate, deliveryDateTo, null);
            return Ok(results);
        }

        #endregion

        #region Student Group Order
        [HttpGet("tokenorders/studentgroup/order")]
        [ProducesResponseType(201, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateStudentGroupOrder(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool clear)
        {
            if (outletId == 0)
                return BadRequest("Outlet not found.");

            var result = await this._service.CreateStudentGroupOrderAsync(studentGroupId, outletId, storeId, deliveryDate, deliveryDateTo, dishTypeId, mealSessionId, createdBy, clear);
            if (!result.IsSuccess)
                return BadRequest("The following errors occurred while processing: " + string.Join(", ", result.Message));

            return Ok(result);
        }


        [HttpGet("tokenorders/studentgroup/ordersummary")]
        //[Authorize(Authorization.Policies.ViewAllMealSessionsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStudentGroupOrderSummary(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int mealSessionId)
        {
            var mealSessions = await this._menuService.GetOutletMealSessions(outletId, deliveryDate, null, null, deliveryDateTo);
            mealSessions = mealSessions.Where(e => e.MealSessionId == mealSessionId).ToList();
            var results = await this._service.GetStudentGroupOrderSummaryAsync(studentGroupId, outletId, storeId, deliveryDate, deliveryDateTo, mealSessions);
            return Ok(results);
        }

        #endregion

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("tokenorders/cancelorder/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCancelOrderRequests(BaseFilter filter)
        {
            var results = await this._service.GetCancelOrderRequestsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<CancelOrderRequestDTO>>(results));
        }

        #endregion

        [HttpGet("tokenorders/cancelorder/get/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(CancelOrderRequestDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCancelOrderRequest(int id)
        {
            var dto = await this._service.GetCancelOrderRequestByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("tokenorders/cancelorder")]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateCancelOrderRequest([FromBody] CreateCancelOrderRequestDTO model)
        {
            try
            {
                var dto = Mapper.Map<CancelOrderRequestDTO>(model);
                _logger.LogDebug("CreateCancelOrderRequest: ", model == null ? "NULL" : JsonConvert.SerializeObject(model));
                var folderName = Path.Combine("Resources", "Orders", "Cancellation", dto.OrderId.ToString());
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                if (!string.IsNullOrEmpty(dto.ImgUrl))
                {
                    var base64Image = dto.ImgUrl;
                    var offset = base64Image.Substring(base64Image.IndexOf(',') + 1);

                    var imageInBytes = Convert.FromBase64String(offset);
                    using (var ms = new MemoryStream(imageInBytes))
                    {
                        var fullPath = Path.Combine(pathToSave, dto.FileName);
                        var dbPath = Path.Combine(folderName, dto.FileName);

                        dto.Response = "";
                        dto.Status = "Submitted";
                        dto.FilePath = dbPath;

                        using (FileStream file = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                        {
                            //byte[] bytes = new byte[file.Length];
                            //file.Read(bytes, 0, (int)file.Length);
                            //ms.Write(bytes, 0, (int)file.Length);

                            ms.WriteTo(file);
                        }

                        //var img = Image.FromStream(ms);
                        //img.Save(fullPath);
                    }
                }

                var response = await _service.CreateCancelOrderRequestAsync(dto);
                response.Data = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message} - {ex.InnerException?.ToString()} - {ex.StackTrace}");
            }
        }


        [HttpDelete("tokenorders/cancelorder/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(CancelOrderRequestDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCancelOrderRequest(int id)
        {
            var dto = await this._service.GetCancelOrderRequestByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteCancelOrderRequestAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("tokenorders/cancelorder/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCancelOrderRequest(string id, [FromBody] CancelOrderRequestDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetCancelOrderRequestByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateCancelOrderRequestAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }


        [HttpPost("tokenorders/cancelorder/approval")]
        //[AllowAnonymous]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequestApproval model)
        {
            var dto = await this._service.GetCancelOrderRequestByIdAsync(model.Id);
            if (dto == null)
                return NotFound(model.Id);

            dto.Status = model.IsApproved ? "Approved" : "Rejected";
            dto.Response = model.Response;

            var result = await this._service.ApprovalCancelOrderRequestAsync(dto);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while processing: " + string.Join(", ", result.Message));

            //approval notifications
            string title = string.Empty;
            string template = string.Empty;
            string items = string.Empty;
            NotificationSettingDTO setting;
            if (model.IsApproved)
            {
                setting = await this._notificationService.GetNotificationSettingByType(NotificationSettingType.CANCEL_REQUEST_APPROVED);
            }
            else
            {
                setting = await this._notificationService.GetNotificationSettingByType(NotificationSettingType.CANCEL_REQUEST_CANCELLED);
            }

            if (setting != null)
            {
                title = setting.Subject;
                template = setting.Template;
            }

            if (dto.Tokens != null)
            {
                items = string.Join(",", dto.Tokens.SelectMany(e => e.SelectedDishes).Select(e => e.DishLabel));
            }

            if (model.IsApproved)
            {
                title = string.IsNullOrEmpty(title) ? "Your cancellation request is approved!" : title;
                template = EmailTemplates.GetCancellationRequestApproved(template, dto.DeliveryDate.ToShortDateString(), dto.MealSessionName, items);
            }
            else
            {
                title = string.IsNullOrEmpty(title) ? "Your cancellation request is rejected!" : title;
                template = EmailTemplates.GetCancellationRequestRejected(template, dto.DeliveryDate.ToShortDateString(), dto.MealSessionName, items);
            }
            
            //send email
            //string enableNotificationEmail = _configuration["AppSettings:NOTIFICATION_EMAIL_ENABLED"];
            if (setting.IsEmailEnabled && !string.IsNullOrEmpty(dto.StudentEmail) && dto.IsNotifCancellationRequestStatus)
            {
                await _emailSender.SendEmailAsync(dto.StudentName, dto.StudentEmail, title, template);
            }

            //string enableNotificationAlert = _configuration["AppSettings:NOTIFICATION_ALERT_ENABLED"];
            if (setting.IsAlertEnabled &&  dto.IsNotifCancellationRequestStatus)
            {
                //int enableNotificationAlertEventId = Convert.ToInt32(_configuration["AppSettings:NOTIFICATION_ALERT_EVENT_ID"]);

                if (dto.UserId > 0)
                {
                    var notification = new NotificationDTO
                    {
                        Body = template,
                        Date = DateTime.Now,
                        Header = title,
                        UserId = dto.UserId
                    };

                    var response = await _notificationService.CreateAsync(notification);

                    if (response.IsSuccess && !string.IsNullOrEmpty(dto.StudentEmail))
                        await _userHub.Clients.Group(dto.StudentEmail).SendAsync("NotificationTriggered", response.Data);
                }
            }

            return Ok(dto);
        }


        [HttpPost("tokenorders/cancelorder/cancel"), DisableRequestSizeLimit]
        //[AllowAnonymous]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequestDTO dto)
        {
            try
            {
                var folderName = Path.Combine("Resources", "Orders", "Cancellation", dto.OrderId.ToString());
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                var base64Image = dto.ImgUrl;
                var offset = base64Image.Substring(base64Image.IndexOf(',') + 1);

                var imageInBytes = Convert.FromBase64String(offset);
                using (var ms = new MemoryStream(imageInBytes))
                {
                    var fullPath = Path.Combine(pathToSave, dto.FileName);
                    var dbPath = Path.Combine(folderName, dto.FileName);

                    dto.Response = "Pending";
                    dto.Status = "Submitted";

                    var img = Image.FromStream(ms);
                    img.Save(fullPath);
                }

                var response = await _service.CreateCancelOrderRequestAsync(dto);
                response.Data = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error - " + ex.StackTrace);
            }
        }

        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("tokenorders/cart/bulkcancel")]
        [ProducesResponseType(200, Type = typeof(NotificationViewModel))]
        public async Task<IActionResult> BulkCancelCart()
        {
            var ordersInCart = await this._service.BulkCancelCart();

            return Ok(ordersInCart);
        }

        [HttpPost("tokenorders/bulkcollect")]
        //[AllowAnonymous]
        public async Task<IActionResult> BulkCollect([FromBody] List<OrderCollectionDTO> orders)
        {
            try
            {
                var response = await this._service.BulkCollectAsync(orders);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error - " + ex.StackTrace);
            }
        }

        [HttpPost("tokenorders/bulkreturn")]
        //[AllowAnonymous]
        public async Task<IActionResult> BulkReturn([FromBody] List<OrderReturnDTO> orders)
        {
            try
            {
                var response = await this._service.BulkReturnAsync(orders);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error - " + ex.StackTrace);
            }
        }

        #endregion



        #region Token Orders History

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("tokensorderhistorys/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTokensOrderHistorys(RequestFilter filter)
        {
            if (filter.isrequest) await _orderController.QueryAndUpdatePaymentStatus(filter.studentId);

            var results = await this._service.GetTokensOrderHistorysAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<TokensOrderHistoryDTO>>(results));
        }

        #endregion

        [HttpGet("tokensorderhistorys/get/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(TokensOrderHistoryDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTokensOrderHistory(int id)
        {
            var dto = await this._service.GetTokensOrderHistoryByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("tokensorderhistorys")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(201, Type = typeof(TokensOrderHistoryDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTokensOrderHistory([FromBody] TokensOrderHistoryDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateTokensOrderHistoryAsync(dto);
                if (result.IsSuccess)
                {
                    TokensOrderHistoryDTO vm = Mapper.Map<TokensOrderHistoryDTO>(result.Data);
                    return CreatedAtAction("GetTokensOrderHistoryById", vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("tokensorderhistorys/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(TokensOrderHistoryDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTokensOrderHistory(int id)
        {
            var dto = await this._service.GetTokensOrderHistoryByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteTokensOrderHistoryAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        //[HttpPut("tokensorderhistorys/update/{id}")]
        ////[AllowAnonymous]
        ////[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> UpdateTokensOrderHistory(string id, [FromBody] TokensOrderHistoryDTO model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (model == null)
        //            return BadRequest($"{nameof(model)} cannot be null");

        //        if (model.Id == 0)
        //            return BadRequest("Conflicting type id in parameter and model data");


        //        var dto = await this._service.GetTokensOrderHistoryByIdAsync(model.Id);


        //        if (dto == null)
        //            return NotFound(id);

        //        var result = await this._service.UpdateTokensOrderHistoryAsync(model);
        //        if (result.IsSuccess)
        //            return NoContent();

        //        AddErrors(new string[] { result.Message });

        //    }

        //    return BadRequest(ModelState);
        //}

        #endregion

        #region Token Ordereds

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("TokensOrderHistoryeds/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentCardsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTokenOrdereds(BaseFilter filter)
        {
            var results = await this._service.GetTokenOrderedsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<TokenOrderedDTO>>(results));
        }

        #endregion

        [HttpGet("tokenordereds/get")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentCardsPolicy)]
        [ProducesResponseType(200, Type = typeof(TokenOrderedDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTokenOrdered(int? id)
        {
            var dto = await this._service.GetTokenOrderedByIdAsync(id.GetValueOrDefault());
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("tokenordereds")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentCardsPolicy)]
        [ProducesResponseType(201, Type = typeof(TokenOrderDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTokenOrdered([FromBody] TokenOrderedDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");

                var result = await this._service.CreateTokenOrderedAsync(dto);
                if (result.IsSuccess)
                {
                    TokenOrderedDTO vm = Mapper.Map<TokenOrderedDTO>(result.Data);
                    return CreatedAtAction("GetTokenOrderedById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("tokenordereds/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentCardsPolicy)]
        [ProducesResponseType(200, Type = typeof(TokenOrderedDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTokenOrdered(int id)
        {
            var dto = await this._service.GetTokenOrderedByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteTokenOrderedAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("tokenordereds/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentCardsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTokenOrdered(string id, [FromBody] TokenOrderedDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetTokenOrderedByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateTokenOrderedAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Token Labels

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("tokenlabels/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTokenLabels(BaseFilter filter)
        {
            var results = await this._service.GetTokenLabelsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<TokenLabelDTO>>(results));
        }

        #endregion

        [HttpGet("tokenlabels/get/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(TokenLabelDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTokenLabel(int id)
        {
            var dto = await this._service.GetTokenLabelByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("tokenlabels")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(201, Type = typeof(TokenLabelDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTokenLabel([FromBody] TokenLabelDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateTokenLabelAsync(dto);
                if (result.IsSuccess)
                {
                    TokenLabelDTO vm = Mapper.Map<TokenLabelDTO>(result.Data);
                    return CreatedAtAction("GetTokenLabelById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("tokenlabels/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(TokenLabelDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTokenLabel(int id)
        {
            var dto = await this._service.GetTokenLabelByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteTokenLabelAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("tokenlabels/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTokenLabel(string id, [FromBody] TokenLabelDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetTokenLabelByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateTokenLabelAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion


        #region Meal Allocations

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("mealallocations/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMealAllocations(BaseFilter filter)
        {
            var results = await this._service.GetMealAllocationsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<MealAllocationDTO>>(results));
        }

        #endregion

        [HttpGet("mealallocations/get/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(MealAllocationDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMealAllocation(int id)
        {
            var dto = await this._service.GetMealAllocationByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("mealallocations")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(201, Type = typeof(MealAllocationDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMealAllocation([FromBody] MealAllocationDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateMealAllocationAsync(dto);
                if (result.IsSuccess)
                {
                    MealAllocationDTO vm = Mapper.Map<MealAllocationDTO>(result.Data);
                    return CreatedAtAction("GetMealAllocationById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("mealallocations/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(MealAllocationDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMealAllocation(int id)
        {
            var dto = await this._service.GetMealAllocationByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteMealAllocationAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("mealallocations/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMealAllocation(string id, [FromBody] MealAllocationDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetMealAllocationByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateMealAllocationAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Packing Allocations

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("packingallocations/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetPackingAllocations(BaseFilter filter)
        {
            var results = await this._service.GetPackingAllocationsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<PackingAllocationDTO>>(results));
        }

        #endregion

        [HttpGet("packingallocations/get/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(PackingAllocationDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPackingAllocation(int id)
        {
            var dto = await this._service.GetPackingAllocationByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("packingallocations")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(201, Type = typeof(PackingAllocationDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePackingAllocation([FromBody] PackingAllocationDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreatePackingAllocationAsync(dto);
                if (result.IsSuccess)
                {
                    PackingAllocationDTO vm = Mapper.Map<PackingAllocationDTO>(result.Data);
                    return CreatedAtAction("GetPAckingAllocationById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("packingallocations/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(PackingAllocationDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePackingAllocation(int id)
        {
            var dto = await this._service.GetPackingAllocationByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeletePackingAllocationAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("packingallocations/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePackingAllocation(string id, [FromBody] PackingAllocationDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetPackingAllocationByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdatePackingAllocationAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region MealPlanOrders

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("mealplanorders/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMealPlanOrders(BaseFilter filter)
        {
            var results = await this._service.GetMealPlanOrdersAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<MealPlanOrderDTO>>(results));
        }

        #endregion

        [HttpGet("mealplanorders/get/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(MealPlanOrderDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMealPlanOrder(int id)
        {
            var dto = await this._service.GetMealPlanOrderByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("mealplanorders")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(201, Type = typeof(MealPlanOrderDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMealPlanOrder([FromBody] MealPlanOrderDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateMealPlanOrderAsync(dto);
                if (result.IsSuccess)
                {
                    MealPlanOrderDTO vm = Mapper.Map<MealPlanOrderDTO>(result.Data);
                    return CreatedAtAction("GetPAckingAllocationById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("mealplanorders/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(MealPlanOrderDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMealPlanOrder(int id)
        {
            var dto = await this._service.GetMealPlanOrderByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteMealPlanOrderAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("mealplanorders/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMealPlanOrder(string id, [FromBody] MealPlanOrderDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetMealPlanOrderByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateMealPlanOrderAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region updetOrderSession
        [HttpPost("updateOrderSession")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateOrderSession()
        {
            var result = await this._service.UpdateOrderSession();

            result.IsSuccess = true;
            result.Message = "Succesfully update Session";

            return Ok(result);
        }
        #endregion

        #region Order Report 

        [HttpGet("collection-summary")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetCollectionSummary(DateTime orderDate, int outletId)
        {
            var dtos = await this._service.GetSalesOrderCollectionSummary(orderDate, outletId);

            return Ok(dtos);
        }

        [HttpPost("salesdata")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> RetrieveSalesData(BaseFilter filter)
        {
            var dtos = await this._service.RetrieveSalesData(filter);

            return Ok(Mapper.Map<List<SalesDataDTO>>(dtos));
        }

        [HttpGet("ordersession")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetTokenOrderWithCurrentSession(BaseFilter filter)
        {
            var filterSplit = filter.Filters.Split(',');
            var mealAllocationId = filterSplit[0].Substring(23);
            filter.Filters = "";
            for (int i = 1; i < filterSplit.Length; i++)
            {
                if (i > 1)
                {
                    filter.Filters += ",";
                }
                filter.Filters += filterSplit[i];
            }
            var dtos = await this._service.GetTokenOrderWithCurrentSession(filter, Convert.ToInt32(mealAllocationId));

            return Ok(Mapper.Map<List<TokenOrderDTO>>(dtos));
        }

        [HttpPost("orderreport")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateOrderReport(BaseFilter filter)
        {
            var pdf = await this._service.GenerateOrderReport(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_OrderReport.xlsx";

            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: pdf,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: reportName
            );
        }

        [HttpPost("deliveryorderreport")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateDeliveryOrderReport(BaseFilter filter)
        {
            var pdf = await this._service.GenerateDeliveryOrderReport(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_OrderReport.xlsx";

            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: pdf,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: reportName
            );
        }

        [HttpPost("mealsummaryreport")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateMealSummaryReport(BaseFilter filter)
        {
            var pdf = await this._service.GenerateMealSummaryReport(filter);
            var reportName = DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_MealSummaryReport.xlsx";

            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: pdf,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: reportName
            );
        }

        [HttpPost("orderlabel")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateOrderLabel([FromBody] TokenLabelDTO[] token)
        {
            var pdf = await this._service.GenerateOrderLabel(token);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_OrderLabel.pdf";

            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: pdf,
                contentType: "application/vnd",
                fileDownloadName: reportName
            );
        }

        #endregion

        #region Order Cancellation
        [HttpGet("cancellations/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOrderLogs(OrderCancellationFilter filter)
        {
            var logs = await _service.GetTokenOrdersForCancellationAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<TokenOrderDTO>>(logs));
        }

        [HttpPost("cancellations/cancel")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelOrders([FromBody] CancelOrderViewModel model)
        {
            var result = await _service.CancelOrders(model.OrderIds, model.CancelledBy, model.Reason);
            return Ok(result);
        }

        #endregion
    }
}