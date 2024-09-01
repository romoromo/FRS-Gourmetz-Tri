using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core;
using DAL.Filters;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class DashboardController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private IConnectionService _connectionService;
        private readonly IMapper _mapper;
        public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger, IConnectionService connectionService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _connectionService = connectionService;
            _mapper = mapper;
        }

        [HttpPost("upcomingevents/{pageSize:int}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<ReservationViewModel>))]
        public async Task<IActionResult> GetUpcomingEvents(int pageSize, [FromBody] CalendarFilter filter)
        {
            if (filter != null)
            {
                filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                filter.StartTime = filter.StartTime != null ? Convert.ToDateTime(filter.StartTime).ToLocalTime() : filter.StartTime;
                filter.EndTime = filter.EndTime != null ? Convert.ToDateTime(filter.EndTime).ToLocalTime() : filter.EndTime;
            }

            var reservations = await _unitOfWork.Reservations.GetReservationsLoadRelatedAsync(1, pageSize, filter);
            return Ok(_mapper.Map<List<ReservationViewModel>>(reservations));
        }

        [HttpPost("inprogressevents/{pageSize:int}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<ReservationViewModel>))]
        public async Task<IActionResult> GetInProgressEvents(int pageSize, [FromBody] CalendarFilter filter)
        {
            if (filter != null)
            {
                filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                filter.StartTime = filter.StartTime != null ? Convert.ToDateTime(filter.StartTime).ToLocalTime() : filter.StartTime;
                filter.EndTime = filter.EndTime != null ? Convert.ToDateTime(filter.EndTime).ToLocalTime() : filter.EndTime;
            }

            filter.IsForAttendance = true;
            var reservations = await _unitOfWork.Reservations.GetReservationsLoadRelatedAsync(1, pageSize, filter);
            return Ok(_mapper.Map<List<ReservationViewModel>>(reservations));
        }

        [HttpPost("devices/{pageNumber:int}/{pageSize:int}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<DeviceViewModel>))]
        public async Task<IActionResult> GetDevices(int pageNumber, int pageSize, [FromBody] DeviceFilter filter)
        {
            var results = await _unitOfWork.Devices.GetDevicesLoadRelatedAsync(pageNumber, pageSize, filter);
            return Ok(_mapper.Map<List<DeviceViewModel>>(results));
        }

        [HttpPost("pibdevices/{pageNumber:int}/{pageSize:int}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<PIBDeviceViewModel>))]
        public async Task<IActionResult> GetPIBDevices(int pageNumber, int pageSize, [FromBody] DeviceFilter filter)
        {
            var results = await _unitOfWork.PIBTemplates.GetApiPIBDevices();
            return Ok(_mapper.Map<List<PIBDeviceViewModel>>(results.Data));
        }

        [HttpPost("signage/dashboard")]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetActivityReportDashboard([FromBody] DashboardFilter filter)
        {
            var results = await _unitOfWork.Devices.GetSignageDashboard(filter);
            var sgnDashboardVM = _mapper.Map<SignageDashboardViewModel>(results);
            foreach (var device in sgnDashboardVM.PagedDevices.PagedData)
            {
                string status = "OFFLINE";
                var connection = await _connectionService.GetConnectionStatus(device.Id);
                status = connection != null && connection.IsActive ? "ONLINE" : "OFFLINE";
                device.connection_status_display = status;
            }
            
            return Ok(sgnDashboardVM);
        }
    }
}