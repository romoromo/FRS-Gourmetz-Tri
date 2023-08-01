using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using FRS.Hubs;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BookingController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private IHubContext<ReservationHub> _reservationHub;

        public BookingController(IUnitOfWork unitOfWork, ILogger<ReservationController> logger, IHubContext<ReservationHub> reservationHub)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _reservationHub = reservationHub;
        }


        [HttpPost("GetAllReservations")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllReservationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ReservationViewModel>))]
        public async Task<IActionResult> GetAllReservations([FromBody] CalendarFilter filter)
        {
            if (filter != null)
            {
                return await GetAllReservations(-1, -1, filter);
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
            }

            var reservations = await _unitOfWork.Bookings.GetReservationsLoadRelatedAsync(pageNumber, pageSize, filter);
            return Ok(Mapper.Map<List<ReservationViewModel>>(reservations));
        }

        [HttpPost("GetAllTimeIntervals")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<TimeIntervalViewModel>))]
        public async Task<IActionResult> GetAllTimeIntervals([FromBody] TimeIntervalFilter filter = null)
        {
            filter.StartDate = filter.StartDate != null ? Convert.ToDateTime(filter.StartDate).ToLocalTime() : filter.StartDate;
            filter.EndDate = filter.EndDate != null ? Convert.ToDateTime(filter.EndDate).ToLocalTime() : filter.EndDate;
            var times = _unitOfWork.Bookings.GetAllTimeIntervals(filter);
            return Ok(Mapper.Map<List<TimeIntervalViewModel>>(times));
        }

        [HttpPost("GetBookingGridRows")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<BookingGridViewModel>))]
        public async Task<IActionResult> GetBookingGridRows([FromBody] CalendarFilter filter = null)
        {
            filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
            filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;
            var rows = _unitOfWork.Bookings.GetBookingGrid(filter);
            return Ok(Mapper.Map<List<BookingGridViewModel>>(rows));
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

                //reservation.StartDateTime = new DateTime(reservation.StartDateTime.Year, reservation.StartDateTime.Month, reservation.StartDateTime.Day,
                //                                        reservation.StartTime.Hour, reservation.StartTime.Minutes, 0);
                //reservation.EndDateTime = new DateTime(reservation.StartDateTime.Year, reservation.StartDateTime.Month, reservation.StartDateTime.Day,
                //                                        reservation.EndTime.Hour, reservation.EndTime.Minutes, 0);

                var res = Mapper.Map<Reservation>(reservation);

                var result = await _unitOfWork.Bookings.CreateAsync(res);
                if (result.IsSuccess)
                {
                    CalendarFilter filter = new CalendarFilter();
                    filter.Start = filter.End = DateTime.Now.Date;
                    var rows = _unitOfWork.Bookings.GetBookingGrid(filter);
                    await _reservationHub.Clients.All.SendAsync("refreshBookingGrid", rows);

                    ReservationViewModel reservationVM = Mapper.Map<ReservationViewModel>(result.Data);
                    return CreatedAtAction("GetReservationById", new { id = reservationVM.Id }, reservationVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Authorization.Policies.ManageAllReservationsPolicy)]
        [ProducesResponseType(200, Type = typeof(ReservationViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            if (!await _unitOfWork.Bookings.TestCanDeleteAsync(id))
                return BadRequest("Reservation cannot be deleted. Remove all users from this reservation and try again");


            var reservationType = await this._unitOfWork.Bookings.GetByIdAsync(id);

            ReservationViewModel reservationVM = Mapper.Map<ReservationViewModel>(reservationType);
            if (reservationVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Bookings.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting reservation: " + string.Join(", ", result.Message));


            return Ok(reservationVM);
        }

        [HttpPut("update/{id}")]
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

                ReservationViewModel reservationVM = Mapper.Map<ReservationViewModel>(reservationType);
                if (reservationVM == null)
                    return NotFound(id);

                if (model.IsAllDay)
                {
                    model.StartDateTime = new DateTime(model.StartDateTime.Year, model.StartDateTime.Month, model.StartDateTime.Day);
                    model.EndDateTime = new DateTime(model.EndDateTime.Year, model.EndDateTime.Month, model.EndDateTime.Day, 23, 59, 0);
                }
                model.StartDateTime = model.StartDateTime != null ? Convert.ToDateTime(model.StartDateTime).ToLocalTime() : model.StartDateTime;
                model.EndDateTime = model.EndDateTime != null ? Convert.ToDateTime(model.EndDateTime).ToLocalTime() : model.EndDateTime;

                //model.StartDateTime = new DateTime(model.StartDateTime.Year, model.StartDateTime.Month, model.StartDateTime.Day,
                //                                       model.StartTime.Hour, model.StartTime.Minutes, 0);
                //model.EndDateTime = new DateTime(model.StartDateTime.Year, model.StartDateTime.Month, model.StartDateTime.Day,
                //                                        model.EndTime.Hour, model.EndTime.Minutes, 0);

                var updatedModel = Mapper.Map<Reservation>(model);

                var result = await _unitOfWork.Reservations.UpdateAsync(updatedModel);
                result.Data = Mapper.Map<ReservationViewModel>(result.Data);
                if (result.IsSuccess)
                    return Ok(result);

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
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
        public async Task<IActionResult> ApiGetBookings(int? locationId = null, DateTime? start = null, DateTime? end = null)
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

                var data = _unitOfWork.Bookings.GetBookingGrid(filter);
                if (locationId.HasValue)
                {
                    //result.Data = data.Any() ? Mapper.Map<BookingGridRowViewModel>(data.First()) : null;
                }
                else
                {
                    result.Data = Mapper.Map<List<BookingGridRowViewModel>>(data);
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

                result = await _unitOfWork.Bookings.GetNextBooking(filter);
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

                result = await _unitOfWork.Bookings.GetCurrentDetail(filter);
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
        public async Task<IActionResult> ApiValidateCheckIn(int institutionId, string userName, string pin, int reservationId)
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Bookings.ValidateCheckIn(userName, pin, reservationId);
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
                result = await _unitOfWork.Bookings.GetExtensionTimes(reservationId);
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
        public async Task<IActionResult> ApiValidateExtend(int reservationId, int timeIntervalId)
        {
            var result = new BaseOperationResponse();
            try
            {
                result = await _unitOfWork.Bookings.ValidateExtend(reservationId, timeIntervalId);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
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
                result = await _unitOfWork.Bookings.ValidateEnd(reservationId);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                throw;
            }

            return Ok(result);
        }
        #endregion
    }
}