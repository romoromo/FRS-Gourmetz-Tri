using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.Services;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
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
    public class SmartRoomSchedulerController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly ISmartRoomService _smartRoomService;

        public SmartRoomSchedulerController(IUnitOfWork unitOfWork, ILogger<SmartRoomSchedulerController> logger, ISmartRoomService smartRoomService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _smartRoomService = smartRoomService;
        }

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("logs/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetSchedulerLogs(BaseFilter filter)
        {
            var logs = await _smartRoomService.GetSchedulerLogs(filter);
            return Ok(logs);
        }

        #endregion

        //[ApiKeyAuthorize]
        [HttpGet("scheduler/execute")]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> ExecuteScheduler(int? institutionId, bool isReadByFile = false)
        {
            bool isSyncResourcesSuccess = false, isSyncSchedulesSuccess = false;
            int totalResources = 0, totalSchedules = 0;
            var log = new SmartRoomSchedulerLogDTO
            {
                EventDateTime = DateTime.Now
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Authenticating Login Credentials for SMARTRoom API...");
            if (!institutionId.HasValue)
            {
                var institution = await _unitOfWork.Institutions.GetDefaultInstitutionAsync();
                institutionId = institution?.Id;
            }

            var tokenResponse = await this._smartRoomService.GetSmartRoomToken(isReadByFile);
            SMARTRoomXML token = null;
            if (tokenResponse.IsSuccess)
            {
                token = tokenResponse.Data as SMARTRoomXML;
            }

            sb.AppendLine(tokenResponse.Message);

            //proceed only if authenticated
            if (tokenResponse.IsSuccess)
            {
                sb.AppendLine("\n\nFetch all resources from Resources API...");
                var response = await this._smartRoomService.GetListOfSmartRoomResourcesAsync(token, isReadByFile);
                if (response.IsSuccess)
                {
                    var smartRoom = response.Data as SMARTRoomXML;
                    if(smartRoom != null && smartRoom.ApplicationData != null && smartRoom.ApplicationData.Rooms != null && smartRoom.ApplicationData.Rooms.Room != null)
                    {
                        totalResources = smartRoom.ApplicationData.Rooms.Room.Count();
                    }
                    
                    sb.AppendLine(response.Message);
                    sb.AppendLine(string.Format("No. of records: <b>{0}</b>", totalResources));
                    sb.AppendLine("\nStart syncing the records to FRS Locations...");
                    response = await this._unitOfWork.Locations.SyncSmartRoomResources(response.Data as SMARTRoomXML, institutionId);
                    sb.AppendLine(response.Message);
                    isSyncResourcesSuccess = response.IsSuccess;
                }

                sb.AppendLine("\n\nFetch all schedules from Schedules API...");
                response = await this._smartRoomService.GetListOfSmartRoomSchedulesAsync(token, isReadByFile);
                if (response.IsSuccess)
                {
                    var smartRoom = response.Data as SMARTRoomXML;
                    if (smartRoom != null && smartRoom.ApplicationData != null && smartRoom.ApplicationData.Schedules != null && smartRoom.ApplicationData.Schedules.Schedule != null)
                    {
                        totalSchedules = smartRoom.ApplicationData.Schedules.Schedule.Count();
                    }

                    sb.AppendLine(response.Message);
                    sb.AppendLine(string.Format("No. of records: <b>{0}</b>", totalSchedules));
                    sb.AppendLine("\nStart syncing the records to FRS Reservations...");
                    response = await this._unitOfWork.Reservations.SyncSmartRoomSchedules(response.Data as SMARTRoomXML, institutionId);
                    sb.AppendLine(response.Message);
                    isSyncSchedulesSuccess = response.IsSuccess;
                }
            }

            log.Details = sb.ToString();
            log.Status = isSyncResourcesSuccess && isSyncSchedulesSuccess ? "Success" : "Failed";
            log.NoRecordsAffected = totalResources + totalSchedules;

            var result = await this._smartRoomService.CreateSchedulerLog(log);
            return Ok(result);
        }
    }
}