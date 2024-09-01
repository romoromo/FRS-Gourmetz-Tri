using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using FRS.Helpers;
using FRS.Hubs;
using FRS.ViewModels;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using OpenIddict.Validation.AspNetCore;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ReservationController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private IHubContext<FRSHub> _frsHub;
        private IHubContext<LocationHub> _locationHub;
        private IHubContext<FRSDeviceHub> _frsDeviceHub;
        private IHubContext<MeetingRoomHub> _meetingRoomHub;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;

        public ReservationController(IUnitOfWork unitOfWork, ILogger<ReservationController> logger, IHubContext<FRSHub> frsHub, IHubContext<LocationHub> locationHub,
            IHubContext<FRSDeviceHub> frsDeviceHub, IHubContext<MeetingRoomHub> meetingRoomHub,
        IEmailSender emailSender,
            IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _frsHub = frsHub;
            _emailSender = emailSender;
            _configuration = configuration;
            _locationHub = locationHub;
            _frsDeviceHub = frsDeviceHub;
            _meetingRoomHub = meetingRoomHub;
            _mapper = mapper;
        }


        [HttpPost("GetAllReservations")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllReservationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ReservationViewModel>))]
        public async Task<IActionResult> GetAllReservations([FromBody] CalendarFilter filter)
        {
            if (filter != null)
            {
                return await GetAllReservations(filter.PageNumber, filter.PageSize, filter);
            }
            else
            {
                return await GetAllReservations(-1, -1);
            }
        }


        [HttpPost("GetAllReservations/{pageNumber:int}/{pageSize:int}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllReservationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ReservationViewModel>))]
        public async Task<IActionResult> GetAllReservations(int pageNumber, int pageSize, CalendarFilter filter = null)
        {
            if (filter != null)
            {
                filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                filter.StartTime = filter.StartTime != null ? Convert.ToDateTime(filter.StartTime).ToLocalTime() : filter.StartTime;
                filter.EndTime = filter.EndTime != null ? Convert.ToDateTime(filter.EndTime).ToLocalTime() : filter.EndTime;
            }

            var reservations = await _unitOfWork.Reservations.GetReservationsLoadRelatedAsync(pageNumber, pageSize, filter);
            return Ok(_mapper.Map<List<ReservationViewModel>>(reservations));
        }

        [HttpPost("calendarevents")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllReservationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<CalendarEvent>))]
        public async Task<IActionResult> GetCalendarEvents([FromBody] CalendarFilter filter = null)
        {
            if (filter != null)
            {
                filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                filter.StartTime = filter.StartTime != null ? Convert.ToDateTime(filter.StartTime).ToLocalTime() : filter.StartTime;
                filter.EndTime = filter.EndTime != null ? Convert.ToDateTime(filter.EndTime).ToLocalTime() : filter.EndTime;
            }

            var events = await _unitOfWork.Reservations.GetCalendarEvents(filter);
            return Ok(events);
        }

        [HttpPost("GetAllTimeIntervals")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<TimeIntervalViewModel>))]
        public async Task<IActionResult> GetAllTimeIntervals([FromBody] TimeIntervalFilter filter = null)
        {
            filter.StartDate = filter.StartDate != null ? Convert.ToDateTime(filter.StartDate).ToLocalTime() : filter.StartDate;
            filter.EndDate = filter.EndDate != null ? Convert.ToDateTime(filter.EndDate).ToLocalTime() : filter.EndDate;

            var times = _unitOfWork.Reservations.GetAllTimeIntervals(filter);
            return Ok(_mapper.Map<List<TimeIntervalViewModel>>(times));
        }

        [HttpPost("GetBookingGridRows")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<BookingGridRowViewModel>))]
        public async Task<IActionResult> GetBookingGridRows([FromBody] CalendarFilter filter = null)
        {
            filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
            filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

            filter.StartTime = filter.StartTime != null ? Convert.ToDateTime(filter.StartTime).ToLocalTime() : filter.StartTime;
            filter.EndTime = filter.EndTime != null ? Convert.ToDateTime(filter.EndTime).ToLocalTime() : filter.EndTime;

            var rows = _unitOfWork.Reservations.GetBookingGrid(filter);
            return Ok(_mapper.Map<List<BookingGridRowViewModel>>(rows));
        }

        [HttpPost("")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.AddReservationPolicy)]
        [ProducesResponseType(201, Type = typeof(ReservationViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationViewModel reservation)
        {
            if (ModelState.IsValid)
            {
                if (reservation == null)
                    return BadRequest($"{nameof(reservation)} cannot be null");

                if (reservation.IsAllDay)
                {
                    reservation.StartDateTime = new DateTime(reservation.StartDateTime.Year, reservation.StartDateTime.Month, reservation.StartDateTime.Day);
                    reservation.EndDateTime = new DateTime(reservation.EndDateTime.Year, reservation.EndDateTime.Month, reservation.EndDateTime.Day, 23, 59, 0);
                }

                reservation.StartDateTime = reservation.StartDateTime != null ? Convert.ToDateTime(reservation.StartDateTime).ToLocalTime() : reservation.StartDateTime;
                reservation.EndDateTime = reservation.EndDateTime != null ? Convert.ToDateTime(reservation.EndDateTime).ToLocalTime() : reservation.EndDateTime;
                reservation.RepeatEndDateTime = reservation.RepeatEndDateTime != null ? Convert.ToDateTime(reservation.RepeatEndDateTime).ToLocalTime() : reservation.RepeatEndDateTime;
                //reservation.StartDateTime = new DateTime(reservation.StartDateTime.Year, reservation.StartDateTime.Month, reservation.StartDateTime.Day,
                //                                        reservation.StartTime.Hour, reservation.StartTime.Minutes, 0);
                //reservation.EndDateTime = new DateTime(reservation.StartDateTime.Year, reservation.StartDateTime.Month, reservation.StartDateTime.Day,
                //                                        reservation.EndTime.Hour, reservation.EndTime.Minutes, 0);

                var res = _mapper.Map<Reservation>(reservation);

                //if there are contact groups, build invitees
                if (reservation.ContactGroups != null)
                {
                    foreach (var cg in reservation.ContactGroups)
                    {
                        if (res.ReservationInvitees == null)
                        {
                            res.ReservationInvitees = new List<ReservationInvitee>();
                        }

                        if (cg.Members != null && cg.Members.Any())
                        {
                            foreach (var cgm in cg.Members)
                            {
                                res.ReservationInvitees.Add(new ReservationInvitee
                                {
                                    ContactGroupId = cg.Id,
                                    Email = cgm.Email,
                                    Name = cgm.Name,
                                    Company = cgm.Company,
                                    Department = cgm.Department,
                                    Designation = cgm.Designation,
                                    PhoneNumber = cgm.PhoneNumber,
                                    UserId = cgm.UserId
                                });
                            }
                        }
                        else
                        {
                            //get members from DB
                            var members = await _unitOfWork.ContactGroups.GetMembers(cg.Id);
                            foreach(var cgm in members)
                            {
                                res.ReservationInvitees.Add(new ReservationInvitee
                                {
                                    ContactGroupId = cg.Id,
                                    Email = cgm.Email,
                                    Name = cgm.Name,
                                    Company = cgm.Company,
                                    Department = cgm.Department,
                                    Designation = cgm.Designation,
                                    PhoneNumber = cgm.PhoneNumber,
                                    UserId = cgm.UserId
                                });
                            }
                        }
                    }
                }

                var result = await _unitOfWork.Reservations.CreateAsync(res,reservation.FilePath);
                if (result.IsSuccess)
                {
                    ReservationViewModel reservationVM = _mapper.Map<ReservationViewModel>(result.Data);

                    CalendarFilter filter = new CalendarFilter();
                    filter.Start = filter.End = DateTime.Now.Date;
                    //var rows = _unitOfWork.Reservations.GetBookingGrid(filter);
                    await _frsHub.Clients.All.SendAsync("BroadcastReservationData", reservationVM);

                    if (reservation.LocationId.HasValue)
                    {
                        await _locationHub.Clients.Group(reservation.LocationId.ToString()).SendAsync("BroadcastLocationData", reservation.LocationId);
                    }

                    if (reservation.LocationId.HasValue)
                    {
                        var deviceResult = await _unitOfWork.Devices.GetApiPIBDevices(location_id: reservation.LocationId);

                        if (deviceResult != null && deviceResult.Data != null)
                        {
                            var devices = _mapper.Map<List<DeviceViewModel>>(deviceResult.Data);

                            foreach (var device in devices)
                            {
                                await _frsDeviceHub.Clients.Group(device.Id.ToString()).SendAsync("RefreshDeviceData", device);
                            }
                        }

                        await _meetingRoomHub.Clients.Group(reservation.LocationId.ToString()).SendAsync("RefreshDisplay", reservation.LocationId);
                    }

                    try
                    {
                        //send emails to participants
                        if (res.ReservationInvitees != null)
                        {
                            foreach (var resInvitee in res.ReservationInvitees)
                            {
                                string recipientName = resInvitee.User != null ? resInvitee.User.FullName : "Participant";
                                string recipientEmail = resInvitee.User != null ? resInvitee.User.Email : resInvitee.Email;
                                string baseUrl = _configuration["AppSettings:baseUrl"];
                                if (!string.IsNullOrEmpty(baseUrl) && baseUrl.Substring(baseUrl.Length - 1) == "/")
                                {
                                    baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
                                }

                                var responseUrl = string.Format("{0}/api/reservation/response?reservationId={1}&userName={2}",
                                    baseUrl, reservationVM.Id, recipientEmail);

                                var emailBody = EmailTemplates.GetParticipantEmail(reservation.ShortDescription, reservation.LongDescription, reservation.Notes,
                                    reservation.LocationName, recipientName,
                                res.CreatedByUser != null ? res.CreatedByUser.FullName : "FRS Admin", string.Format("{0} - {1}", reservation.StartDateTime, reservation.EndDateTime), responseUrl);

                                string subject = string.Format("Invitation: ({0}) @ {1} - {2}", reservation.ShortDescription, reservation.StartDateTime, reservation.EndDateTime);
                                //ics
                                var ics = _emailSender.CreateCalendarEntry(reservation.StartDateTime, reservation.EndDateTime, subject, reservation.LongDescription, reservation.LocationName,
                                    res.CreatedByUser != null ? res.CreatedByUser.FullName : "FRS Admin", res.CreatedByUser != null ? res.CreatedByUser.Email : "smv.notification@gmail.com");

                                var serializer = new CalendarSerializer(new SerializationContext());
                                var serializedCalendar = serializer.SerializeToString(ics);
                                var bytesCalendar = Encoding.UTF8.GetBytes(serializedCalendar);
                                //MemoryStream ms = new MemoryStream(bytesCalendar);
                                //System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(ms, "event.ics", "text/calendar");

                                var attachments = new List<EmailAttachment>();
                                attachments.Add(new EmailAttachment { FileName = "event.ics", Stream = bytesCalendar });
                                await _emailSender.SendEmailAsync(recipientName, recipientEmail, subject, emailBody, attachments: attachments);
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }

                    //ReservationViewModel reservationVM = _mapper.Map<ReservationViewModel>(result.Data);
                    return CreatedAtAction("GetReservationById", new { id = reservationVM.Id }, reservationVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllReservationsPolicy)]
        [ProducesResponseType(200, Type = typeof(ReservationViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteReservation(int id, int? cancelledBy, string reason, string recurApplyChangesType, bool isCancel = false)
        {
            if (!await _unitOfWork.Reservations.TestCanDeleteAsync(id))
                return BadRequest("Reservation cannot be deleted. Remove all users from this reservation and try again");


            var reservation = await this._unitOfWork.Reservations.GetByIdAsync(id);

            ReservationViewModel reservationVM = _mapper.Map<ReservationViewModel>(reservation);
            if (reservationVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Reservations.DeleteAsync(id, cancelledBy, reason, recurApplyChangesType, isCancel);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting reservation: " + string.Join(", ", result.Message));

            if (reservation.LocationId.HasValue)
            {
                var deviceResult = await _unitOfWork.Devices.GetApiPIBDevices(location_id: reservation.LocationId);

                if (deviceResult != null && deviceResult.Data != null)
                {
                    var devices = _mapper.Map<List<DeviceViewModel>>(deviceResult.Data);

                    foreach (var device in devices)
                    {
                        await _frsDeviceHub.Clients.Group(device.Id.ToString()).SendAsync("RefreshDeviceData", device);
                    }
                }

                await _meetingRoomHub.Clients.Group(reservation.LocationId.ToString()).SendAsync("RefreshDisplay", reservation.LocationId);
            }

            return Ok(reservationVM);
        }

        [HttpPut("update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.AddReservationPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateReservation(string id, [FromBody] ReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var reservationType = await this._unitOfWork.Reservations.GetByIdAsync(model.Id);

                ReservationViewModel reservationVM = _mapper.Map<ReservationViewModel>(reservationType);
                if (reservationVM == null)
                    return NotFound(id);

                if (model.IsAllDay)
                {
                    model.StartDateTime = new DateTime(model.StartDateTime.Year, model.StartDateTime.Month, model.StartDateTime.Day);
                    model.EndDateTime = new DateTime(model.EndDateTime.Year, model.EndDateTime.Month, model.EndDateTime.Day, 23, 59, 0);
                }
                model.StartDateTime = model.StartDateTime != null ? Convert.ToDateTime(model.StartDateTime).ToLocalTime() : model.StartDateTime;
                model.EndDateTime = model.EndDateTime != null ? Convert.ToDateTime(model.EndDateTime).ToLocalTime() : model.EndDateTime;
                model.RepeatEndDateTime = model.RepeatEndDateTime != null ? Convert.ToDateTime(model.RepeatEndDateTime).ToLocalTime() : model.RepeatEndDateTime;

                //model.StartDateTime = new DateTime(model.StartDateTime.Year, model.StartDateTime.Month, model.StartDateTime.Day,
                //                                       model.StartTime.Hour, model.StartTime.Minutes, 0);
                //model.EndDateTime = new DateTime(model.StartDateTime.Year, model.StartDateTime.Month, model.StartDateTime.Day,
                //                                        model.EndTime.Hour, model.EndTime.Minutes, 0);

                var updatedModel = _mapper.Map<Reservation>(model);

                //if there are contact groups, build invitees
                if (model.ContactGroups != null)
                {
                    foreach (var cg in model.ContactGroups)
                    {
                        if (updatedModel.ReservationInvitees == null)
                        {
                            updatedModel.ReservationInvitees = new List<ReservationInvitee>();
                        }

                        if (cg.Members != null && cg.Members.Any())
                        {
                            foreach (var cgm in cg.Members)
                            {
                                updatedModel.ReservationInvitees.Add(new ReservationInvitee
                                {
                                    ContactGroupId = cg.Id,
                                    Email = cgm.Email,
                                    Name = cgm.Name,
                                    Company = cgm.Company,
                                    Department = cgm.Department,
                                    Designation = cgm.Designation,
                                    PhoneNumber = cgm.PhoneNumber,
                                    UserId = cgm.UserId
                                });
                            }
                        }
                        else
                        {
                            //get members from DB
                            var members = await _unitOfWork.ContactGroups.GetMembers(cg.Id);
                            foreach (var cgm in members)
                            {
                                updatedModel.ReservationInvitees.Add(new ReservationInvitee
                                {
                                    ContactGroupId = cg.Id,
                                    Email = cgm.Email,
                                    Name = cgm.Name,
                                    Company = cgm.Company,
                                    Department = cgm.Department,
                                    Designation = cgm.Designation,
                                    PhoneNumber = cgm.PhoneNumber,
                                    UserId = cgm.UserId
                                });
                            }
                        }
                    }
                }

                var result = await _unitOfWork.Reservations.UpdateAsync(updatedModel, model.RecurApplyChangesType);
                if (result.IsSuccess)
                {
                    try
                    {
                        if (updatedModel.LocationId.HasValue)
                        {
                            await _locationHub.Clients.Group(updatedModel.LocationId.ToString()).SendAsync("BroadcastLocationData", updatedModel.LocationId);
                        }

                        if (updatedModel.LocationId.HasValue)
                        {
                            var deviceResult = await _unitOfWork.Devices.GetApiPIBDevices(location_id: updatedModel.LocationId);

                            if (deviceResult != null && deviceResult.Data != null)
                            {
                                var devices = _mapper.Map<List<DeviceViewModel>>(deviceResult.Data);

                                foreach (var device in devices)
                                {
                                    await _frsDeviceHub.Clients.Group(device.Id.ToString()).SendAsync("RefreshDeviceData", device);
                                }
                            }

                            await _meetingRoomHub.Clients.Group(updatedModel.LocationId.ToString()).SendAsync("RefreshDisplay", updatedModel.LocationId);
                        }

                        ReservationUpdateResponseData data = (ReservationUpdateResponseData)(result.Data);
                        var updatedReservation = data.Reservation;
                        var addedInvitees = data.Invitees;
                        result.Data = _mapper.Map<ReservationViewModel>(updatedReservation);

                        //send emails to participants
                        if (updatedModel.ReservationInvitees != null)
                        {
                            if(updatedReservation.Status != BookingStatus.Completed.ToString() &&
                                    updatedReservation.Status != BookingStatus.Released.ToString() &&
                                    updatedReservation.Status != BookingStatus.Cancelled.ToString())
                            {
                                if (model.LocationId == updatedReservation.LocationId)
                                {
                                    if (addedInvitees.Count > 0)
                                    {
                                        //will only notify newly added invites if no change in location and status
                                        updatedModel.ReservationInvitees = addedInvitees;
                                    }
                                    else
                                    {
                                        updatedModel.ReservationInvitees = new List<ReservationInvitee>();//don't resend
                                    }
                                }
                                else
                                {
                                    //resend to all
                                }
                            }
                            else
                            {
                                updatedModel.ReservationInvitees = new List<ReservationInvitee>();
                            }

                            foreach (var resInvitee in updatedModel.ReservationInvitees)
                            {
                                string recipientName = resInvitee.User != null ? resInvitee.User.FullName : "Participant";
                                string recipientEmail = resInvitee.User != null ? resInvitee.User.Email : resInvitee.Email;

                                string baseUrl = _configuration["AppSettings:baseUrl"];
                                if (!string.IsNullOrEmpty(baseUrl) && baseUrl.Substring(baseUrl.Length - 1) == "/")
                                {
                                    baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
                                }

                                var responseUrl = string.Format("{0}/api/reservation/response?reservationId={1}&userName={2}",
                                    baseUrl, updatedReservation.Id, recipientEmail);

                                var emailBody = EmailTemplates.GetParticipantEmail(model.ShortDescription, model.LongDescription, model.Notes,
                                    model.LocationName, recipientName,
                                updatedModel.CreatedByUser != null ? updatedModel.CreatedByUser.FullName : "FRS Admin", string.Format("{0} - {1}", model.StartDateTime, model.EndDateTime), responseUrl);
                                string subject = string.Format("Invitation: ({0}) @ {1} - {2}", model.ShortDescription, model.StartDateTime, model.EndDateTime);
                                await _emailSender.SendEmailAsync(recipientName, recipientEmail, subject, emailBody);
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    return Ok(result);
                }

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #region Excel
        //public IActionResult GenerateAttendanceXls(int reservationId)
        //{
        //    var xls = _unitOfWork.Reservations.GenerateAttendanceXlsLayout(reservationId);
        //    var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_AttendanceList.xlsx";
        //    //var filepath = Path.Combine(Utilities.GetReportPathXls(), reportName);
        //    //System.IO.File.WriteAllBytes(filepath, xls);
        //    //var returnPath = Utilities.GetRelativeReportPathPdf(reportName);

        //    if (xls == null || xls.Length == 0)
        //    {
        //        return BadRequest(""); 
        //    }

        //    return File(
        //        fileContents: xls,
        //        contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //        fileDownloadName: reportName
        //    );
        //}
        #endregion

        [HttpGet("response")]
        //[AllowAnonymous]
        [Produces("text/html")]
        //[Authorize(Authorization.Policies.ManageAllReservationsPolicy)]
        public async Task<IActionResult> Response(int reservationId, string userName, string status)
        {
            var events = await _unitOfWork.Reservations.Response(reservationId, userName, status);

            var content = string.Format("<html><body><h1>Event Response</h1><p>{0}</p></body></html>", events.Message);
            return new ContentResult()
            {
                Content = content,
                ContentType = "text/html",
            };

            //return Ok(events);
        }

        #region Api Calls
        /// <summary>
        /// API calls to get bookings
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>booking list</returns>
        [HttpGet("get")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiGetBookings(int? id = null, int? locationId = null, DateTime? start = null, DateTime? end = null)
        {
            var result = new BaseOperationResponse();
            try
            {
                if (id.HasValue)
                {
                    var reservation = await _unitOfWork.Reservations.GetByIdAsync(id.Value);
                    result.Data = _mapper.Map<ReservationViewModel>(reservation);
                }
                else
                {
                    var filter = new CalendarFilter
                    {
                        Start = start,
                        End = end,
                        locationIds = locationId.HasValue ? new List<int> { locationId.Value } : null
                    };

                    filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                    filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                    var data = await _unitOfWork.Reservations.GetReservationsLoadRelatedAsync(-1, -1, filter);
                    //if (locationId.HasValue)
                    //{
                    //    result.Data = data.Any() ? _mapper.Map<ReservationViewModel>(data.First()) : null;
                    //}
                    //else
                    //{
                    result.Data = _mapper.Map<List<ReservationViewModel>>(data);
                    //}

                    //var data = _unitOfWork.Reservations.GetBookingGrid(filter);
                    //if (locationId.HasValue)
                    //{
                    //    result.Data = data.Any() ? _mapper.Map<BookingGridRowViewModel>(data.First()) : null;
                    //}
                    //else
                    //{
                    //    result.Data = _mapper.Map<List<BookingGridRowViewModel>>(data);
                    //}
                }

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }

        [HttpGet("getKiosk")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetKiosk( DateTime? start = null, DateTime? end = null)
        {
            var result = new BaseOperationResponse();
            try
            {
                if (start == null)
                {
                    start = DateTime.Now;
                    end = (DateTime.Now).AddMonths(3);
                }
                
                    var filter = new CalendarFilter
                    {
                        Start = start,
                        End = end,
                        IsForKiosk = true
                    };

                    filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                    filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                var data = await _unitOfWork.Reservations.GetReservationsLoadRelatedAsync(-1, -1, filter);
                    result.Data = _mapper.Map<List<ReservationViewModel>>(data);

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }

        [HttpGet("getNextReservation")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiGetNextBooking(int? locationId = null, DateTime? start = null, DateTime? end = null)
        {
            var result = new BaseOperationResponse();
            try
            {
                var filter = new CalendarFilter
                {
                    Start = start,
                    End = end,
                    locationIds = locationId.HasValue ? new List<int> { locationId.Value } : null
                };

                filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                result = await _unitOfWork.Reservations.GetNextBooking(filter);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }

        [HttpGet("getCurrentDetail")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiGetReservationDetail(int? locationId = null, DateTime? start = null, DateTime? end = null)
        {
            var result = new BaseOperationResponse();
            try
            {
                var filter = new CalendarFilter
                {
                    Start = start,
                    End = end,
                    locationIds = locationId.HasValue ? new List<int> { locationId.Value } : null
                };

                filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                result = await _unitOfWork.Reservations.GetCurrentDetail(filter);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }

        [HttpPost("checkin"), HttpGet("checkin")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiValidateCheckIn(string userName, string pin, int reservationId, string name, string company, string designation)
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Reservations.ValidateCheckIn(userName, pin, reservationId, name, company, designation);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }

        [HttpGet("getExtensionTimes")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiGetBookingExtensionTimes(int reservationId)
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Reservations.GetExtensionTimes(reservationId);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }

        [HttpGet("extend")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiValidateExtend(int reservationId, double totalMinutes)
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Reservations.ValidateExtend(reservationId, totalMinutes);

                if (result.IsSuccess)
                {
                    var reservation = await this._unitOfWork.Reservations.GetByIdAsync(reservationId);
                    if (reservation != null && reservation.LocationId.HasValue)
                    {
                        var deviceResult = await _unitOfWork.Devices.GetApiPIBDevices(location_id: reservation.LocationId);

                        if (deviceResult != null && deviceResult.Data != null)
                        {
                            var devices = _mapper.Map<List<DeviceViewModel>>(deviceResult.Data);

                            foreach (var device in devices)
                            {
                                await _frsDeviceHub.Clients.Group(device.Id.ToString()).SendAsync("RefreshDeviceData", device);
                            }
                        }

                        await _meetingRoomHub.Clients.Group(reservation.LocationId.ToString()).SendAsync("RefreshDisplay", reservation.LocationId);
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
            }

            return Ok(result);
        }

        [HttpGet("end")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiValidateEnd(int reservationId)
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Reservations.ValidateEnd(reservationId);
                if (result.IsSuccess)
                {
                    //send sms if event is done
                    // Find your Account Sid and Token at twilio.com/console
                    // DANGER! This is insecure. See http://twil.io/secure
                    //const string accountSid = "ACe5fdcfcc7a394946897a74fe174f7f21";
                    //const string authToken = "a7423021c06e3a470a5f76dc71191584";

                    //TwilioClient.Init(accountSid, authToken);
                    dynamic data = result.Data;
                    List<ReservationInvitee> attendees = data.GetType().GetProperty("Attendees").GetValue(data, null);
                    string baseUrl = _configuration["AppSettings:baseUrl"];

                    foreach (var attendee in attendees)
                    {
                        var phoneNumber = !string.IsNullOrEmpty(attendee.PhoneNumber) ? attendee.PhoneNumber :
                            (attendee.User != null && !string.IsNullOrEmpty(attendee.User.PhoneNumber)) ? attendee.User.PhoneNumber : string.Empty;
                        if (!string.IsNullOrEmpty(phoneNumber))
                        {
                            if (!string.IsNullOrEmpty(baseUrl) && baseUrl.Substring(baseUrl.Length - 1) == "/")
                            {
                                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
                            }

                            //string url = string.Format("{0}/reservation/feedback?reservationId={1}&attendeeId={2}",
                            //    baseUrl, reservationId, attendee.Id);

                            //string body = string.Format("Your event has ended. Click {0} to leave a feedback.", url);
                            //var message = MessageResource.Create(
                            //    body: body,
                            //    from: new Twilio.Types.PhoneNumber("+12029464813"),
                            //    to: new Twilio.Types.PhoneNumber(phoneNumber)
                            //);

                            //Console.WriteLine(body);
                        }
                    }

                    var reservation = await this._unitOfWork.Reservations.GetByIdAsync(reservationId);
                    if (reservation != null && reservation.LocationId.HasValue)
                    {
                        var deviceResult = await _unitOfWork.Devices.GetApiPIBDevices(location_id: reservation.LocationId);

                        if (deviceResult != null && deviceResult.Data != null)
                        {
                            var devices = _mapper.Map<List<DeviceViewModel>>(deviceResult.Data);

                            foreach (var device in devices)
                            {
                                await _frsDeviceHub.Clients.Group(device.Id.ToString()).SendAsync("RefreshDeviceData", device);
                            }
                        }

                        await _meetingRoomHub.Clients.Group(reservation.LocationId.ToString()).SendAsync("RefreshDisplay", reservation.LocationId);
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }

        [HttpPost("attendance/signin"), HttpGet("attendance/signin")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiValidateAttendanceSignIn(string userName, int reservationId, string name, string company, string designation, string status)
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Reservations.ValidateAttendanceSignIn(userName, reservationId, name, company, designation, status);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }

        [HttpGet("processexpiredbookings")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiProcessExpiredBookings()
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Reservations.ProcessExpiredBookings();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }

        [HttpGet("monitorinprogressevents")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiMonitorInprogressEvents()
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Reservations.MonitorInprogressBookings();
                if (result.IsSuccess)
                {
                    dynamic data = result.Data;
                    List<Reservation> updatedReservations = data.GetType().GetProperty("data").GetValue(data, null);
                    if(updatedReservations != null)
                    {
                        foreach (var reservation in updatedReservations)
                        {
                            //broadcast to device
                            if (reservation.LocationId.HasValue)
                            {
                                var deviceResult = await _unitOfWork.Devices.GetApiPIBDevices(location_id: reservation.LocationId);

                                if (deviceResult != null && deviceResult.Data != null)
                                {
                                    var devices = _mapper.Map<List<DeviceViewModel>>(deviceResult.Data);

                                    foreach (var device in devices)
                                    {
                                        await _frsDeviceHub.Clients.Group(device.Id.ToString()).SendAsync("RefreshDeviceData", device);
                                    }
                                }

                                await _meetingRoomHub.Clients.Group(reservation.LocationId.ToString()).SendAsync("RefreshDisplay", reservation.LocationId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            result.Data = null;
            return Ok(result);
        }

        [HttpGet("completeevents")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiCompleteEvents()
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Reservations.CompleteBookings();
                if (result.IsSuccess)
                {
                    dynamic data = result.Data;
                    List<Reservation> updatedReservations = data.GetType().GetProperty("data").GetValue(data, null);
                    if (updatedReservations != null)
                    {
                        foreach (var reservation in updatedReservations)
                        {
                            //broadcast to device
                            if (reservation.LocationId.HasValue)
                            {
                                var deviceResult = await _unitOfWork.Devices.GetApiPIBDevices(location_id: reservation.LocationId);

                                if (deviceResult != null && deviceResult.Data != null)
                                {
                                    var devices = _mapper.Map<List<DeviceViewModel>>(deviceResult.Data);

                                    foreach (var device in devices)
                                    {
                                        await _frsDeviceHub.Clients.Group(device.Id.ToString()).SendAsync("RefreshDeviceData", device);
                                    }
                                }

                                await _meetingRoomHub.Clients.Group(reservation.LocationId.ToString()).SendAsync("RefreshDisplay", reservation.LocationId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            result.Data = null;
            return Ok(result);
        }

        [HttpPost("feedback"), HttpGet("feedback")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiValidateFeedback(int reservationId, int userId, string feedback, string comment)
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Reservations.ValidateFeedback(reservationId, userId, feedback, comment);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }

        [HttpGet("locationcurrentdetail")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiGetLocationDetail(int? locationId = null, DateTime? start = null, DateTime? end = null)
        {
            var result = new LocationApiInformation();
            try
            {
                var filter = new CalendarFilter
                {
                    Start = start,
                    End = end,
                    locationIds = locationId.HasValue ? new List<int> { locationId.Value } : null
                };

                filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                result = await this._unitOfWork.Reservations.GetLocationDetail(filter);
            }
            catch (Exception ex)
            {
                result = null;
                throw;
            }

            return Ok(result);
        }
        #endregion

        #region Vehicle Logs
        [HttpPost("GetVehicleLogs")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllReservationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<VMSVehicleLog>))]
        public async Task<IActionResult> GetVehicleLogs([FromBody] VehicleLogFilter filter)
        {
            if (filter != null)
            {
                filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;
            }

            var logs = await _unitOfWork.Reservations.GetVehicleLogs(filter);
            return Ok(_mapper.Map<List<VMSVehicleLog>>(logs));
        }
        #endregion

    }
}