using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Core.DTO;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class SignageController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;

        public SignageController(IUnitOfWork unitOfWork, ILogger<SignageController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet("sgnfacilities/list")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<FacilityViewModel>))]
        public async Task<IActionResult> GetFacilities(int? institutionId = null)
        {
            return await GetFacilities(-1, -1, institutionId);
        }


        [HttpGet("sgnfacilities/list/{pageNumber:int}/{pageSize:int}")]
        [ProducesResponseType(200, Type = typeof(List<FacilityViewModel>))]
        public async Task<IActionResult> GetFacilities(int pageNumber, int pageSize, int? institutionId = null)
        {
            var facilities = await _unitOfWork.Facilities.GetFacilitiesLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(Mapper.Map<List<FacilityViewModel>>(facilities));
        }


        [HttpGet("snglocations")]
        [ProducesResponseType(200, Type = typeof(List<SignageLocationDTO>))]
        public async Task<IActionResult> GetLocations(int? institutionId = null)
        {
            var locations = _unitOfWork.Locations.GetSignageLocations(institutionId);
            return Ok(locations);
        }

        [HttpPost("bookings")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<SignageBookingDTO>))]
        public async Task<IActionResult> GetAllReservations([FromBody] CalendarFilter filter = null)
        {
            if (filter != null)
            {
                filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;
            }

            var reservations = await _unitOfWork.Reservations.GetSignageReservations(filter);
            return Ok(reservations);
        }
    }
}