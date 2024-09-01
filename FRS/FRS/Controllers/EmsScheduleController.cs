using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Models;
using DAL.Repositories;
using FRS.Hubs;
using FRS.ViewModels;
using Ical.Net.DataTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class EmsScheduleController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private IHubContext<FRSHub> _frsHub;
        private IHubContext<EMSHub> _emsHub;
        private DeviceManagerController _deviceManagerController;
        private readonly IMapper _mapper;

        public EmsScheduleController(IUnitOfWork unitOfWork, ILogger<EmsScheduleController> logger, IHubContext<FRSHub> frsHub, IHubContext<EMSHub> emsHub, DeviceManagerController deviceManagerController, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _frsHub = frsHub;
            _emsHub = emsHub;
            _deviceManagerController = deviceManagerController;
            _mapper = mapper;
        }

        /// <summary>
        /// API calls to create emsSchedule
        /// </summary>
        /// <param name="emsSchedule"></param>
        /// <returns>BaseOperationResponse</returns>
        [Microsoft.AspNetCore.Mvc.HttpPost("create")]
        //[Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ApiCreateEmsSchedule([FromBody] EmsScheduleViewModel emsSchedule)
        {
            var result = new BaseOperationResponse();
            if (ModelState.IsValid)
            {
                var emsScheduleInfo = _mapper.Map<EmsSchedule>(emsSchedule);

                result = await _unitOfWork.EmsSchedules.CreateAsync(emsScheduleInfo);
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Missing required fields. Please check your inputs.";
            }

            return Ok(result);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("get/recschedules")]
        //[Microsoft.AspNetCore.Authorization.AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RSchedule>))]
        public async Task<IActionResult> GetRecSchedules(DateTime? start, DateTime? end)
        {
            var results = await _unitOfWork.EmsSchedules.GetRecSchedulesAsync(start, end);
            return Ok(_mapper.Map<List<RSchedule>>(results));
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("get/emsschedules")]
        //[Microsoft.AspNetCore.Authorization.AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmsScheduleViewModel>))]
        public async Task<IActionResult> GetEmsSchedules()
        {
            var results = await _unitOfWork.EmsSchedules.GetEmsSchedulesLoadRelatedAsync(-1, -1);
            return Ok(_mapper.Map<List<EmsScheduleViewModel>>(results));
        }



        [Microsoft.AspNetCore.Mvc.HttpPost("GetAllEmsSchedules")]
        //[Microsoft.AspNetCore.Authorization.AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllEmsSchedulesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmsScheduleViewModel>))]
        public async Task<IActionResult> GetAllEmsSchedules()
        {
            return await GetEmsSchedules(-1, -1);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("GetEmsSchedules/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllEmsSchedulesPolicy)]
        //[Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<EmsScheduleViewModel>))]
        public async Task<IActionResult> GetEmsSchedules(int pageNumber, int pageSize)
        {
            var facilities = await _unitOfWork.EmsSchedules.GetEmsSchedulesLoadRelatedAsync(pageNumber, pageSize);
            return Ok(_mapper.Map<List<EmsScheduleViewModel>>(facilities));
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("")]
        //[Microsoft.AspNetCore.Authorization.AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllEmsSchedulesPolicy)]
        [ProducesResponseType(201, Type = typeof(EmsScheduleViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEmsSchedule([FromBody] EmsScheduleViewModel emsSchedule)
        {
            if (ModelState.IsValid)
            {
                if (emsSchedule == null)
                    return BadRequest($"{nameof(emsSchedule)} cannot be null");


                var emsScheduleInfo = _mapper.Map<EmsSchedule>(emsSchedule);

                var result = await _unitOfWork.EmsSchedules.CreateAsync(emsScheduleInfo);
                if (result.IsSuccess)
                {
                    await _emsHub.Clients.All.SendAsync("ReloadSchedule").ConfigureAwait(false);
                    EmsScheduleViewModel vm = _mapper.Map<EmsScheduleViewModel>(result.Data);
                    return CreatedAtAction("GetEmsScheduleById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [Microsoft.AspNetCore.Mvc.HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmsSchedulesPolicy)]
        [ProducesResponseType(200, Type = typeof(EmsScheduleViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEmsSchedule(int id)
        {

            var emsScheduleType = await this._unitOfWork.EmsSchedules.GetByIdAsync(id);

            EmsScheduleViewModel emsScheduleVM = _mapper.Map<EmsScheduleViewModel>(emsScheduleType);
            if (emsScheduleVM == null)
                return NotFound(id);

            var result = await _unitOfWork.EmsSchedules.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting ems schedule: " + string.Join(", ", result.Message));


            return Ok(emsScheduleVM);
        }

        [Microsoft.AspNetCore.Mvc.HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmsSchedulesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEmsSchedule(string id, [FromBody] EmsScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var emsScheduleType = await this._unitOfWork.EmsSchedules.GetByIdAsync(model.Id);

                EmsScheduleViewModel emsScheduleVM = _mapper.Map<EmsScheduleViewModel>(emsScheduleType);
                if (emsScheduleVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<EmsSchedule>(model);
                var result = await _unitOfWork.EmsSchedules.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                {
                    await _emsHub.Clients.All.SendAsync("ReloadSchedule").ConfigureAwait(false);
                    return NoContent();
                }

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("trigger")]
        //[Microsoft.AspNetCore.Authorization.AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RSchedule>))]
        public async Task<IActionResult> TriggerSchedules(int scheduleId)
        {
            var schedule = await _unitOfWork.EmsSchedules.GetByIdAsync(scheduleId);

            var devices = await _unitOfWork.Devices.GetDevicesByEmsGroupId(schedule.EmsGroupId);

            if (schedule.EmsProfile == null) return BadRequest("Profile had not been set.");


            foreach (var dq in devices)
            {
                var d = _mapper.Map<DeviceViewModel>(dq);

                if (schedule.EmsProfile.ScreenStatus == 1) d.isScreenOn = true;
                if (schedule.EmsProfile.ScreenStatus == 2) d.isScreenOn = false;
                if (!string.IsNullOrWhiteSpace(schedule.EmsProfile.Resolution)) d.Resolution = schedule.EmsProfile.Resolution;
                if (!string.IsNullOrWhiteSpace(schedule.EmsProfile.Rotation)) d.Rotation = schedule.EmsProfile.Rotation;
                if (schedule.EmsProfile.Brightness != null) d.Brightness = schedule.EmsProfile.Brightness;
                if (schedule.EmsProfile.Volume != null) d.Volume = schedule.EmsProfile.Volume;
                if (!string.IsNullOrWhiteSpace(schedule.EmsProfile.module_path_value)) d.module_path_value = schedule.EmsProfile.module_path_value;
                //if (schedule.EmsProfile.CommunicatorMenuId == null) d.CommunicatorMenuId = schedule.EmsProfile.CommunicatorMenuId;

                

                if (schedule.EmsProfile.reboot) d.rebootWhenUpdate = schedule.EmsProfile.reboot;

                await _deviceManagerController.UpdateDevice(d.Id + "", d);
            };

            return Ok("Success");
        }
    }


}