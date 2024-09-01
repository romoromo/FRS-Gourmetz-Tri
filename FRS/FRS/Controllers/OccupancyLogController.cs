using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Models;
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
    public class OccupancyLogController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public OccupancyLogController(IUnitOfWork unitOfWork, ILogger<OccupancyLogController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("list")]
        //[Authorize(Authorization.Policies.ManageAllOccupancyLogsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<OccupancyLogViewModel>))]
        public async Task<IActionResult> GetOccupancyLogs(int? institutionId = null)
        {
            return await GetOccupancyLogs(-1, -1, institutionId);
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ManageAllOccupancyLogsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<OccupancyLogViewModel>))]
        public async Task<IActionResult> GetOccupancyLogs(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.OccupancyLogs.GetOccupancyLogsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(_mapper.Map<List<OccupancyLogViewModel>>(result));
        }

        [HttpGet("listfilter")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllOccupancyLogsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<OccupancyLogViewModel>))]
        public async Task<IActionResult> GetOccupancyLogsFilter(DateTime? StartTime, DateTime? EndTime, string DeviceId, string SensorId, string Status)
        {
            var result = await _unitOfWork.OccupancyLogs.GetOccupancyLogsFilter(StartTime, EndTime, DeviceId, SensorId, Status);
            return Ok(_mapper.Map<List<OccupancyLogViewModel>>(result));
        }

        [HttpGet("status")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllOccupancyLogsPolicy)]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> GetLastStatus(string DeviceId, string SensorId)
        {
            var result = await _unitOfWork.OccupancyLogs.GetLastStatus(DeviceId, SensorId);
            return Ok(result);
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllOccupancyLogsPolicy)]
        [ProducesResponseType(201, Type = typeof(OccupancyLogViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOccupancyLog([FromBody] OccupancyLogViewModel occupancyLog)
        {
            if (ModelState.IsValid)
            {
                if (occupancyLog == null)
                    return BadRequest($"{nameof(occupancyLog)} cannot be null");


                var type = _mapper.Map<OccupancyLog>(occupancyLog);

                var result = await _unitOfWork.OccupancyLogs.CreateAsync(type);
                if (result.IsSuccess)
                {
                    OccupancyLogViewModel occupancyLogVM = _mapper.Map<OccupancyLogViewModel>(result.Data);
                    return CreatedAtAction("GetOccupancyLogById", new { id = occupancyLogVM.Id }, occupancyLogVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [HttpGet("create")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllOccupancyLogsPolicy)]
        [ProducesResponseType(201, Type = typeof(OccupancyLogViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateGetOccupancyLog(OccupancyLogViewModel occupancyLog)
        {
            if (ModelState.IsValid)
            {
                if (occupancyLog == null)
                    return BadRequest($"{nameof(occupancyLog)} cannot be null");


                var type = _mapper.Map<OccupancyLog>(occupancyLog);

                var result = await _unitOfWork.OccupancyLogs.CreateAsync(type);
                if (result.IsSuccess)
                {
                    OccupancyLogViewModel occupancyLogVM = _mapper.Map<OccupancyLogViewModel>(result.Data);
                    return CreatedAtAction("GetOccupancyLogById", new { id = occupancyLogVM.Id }, occupancyLogVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOccupancyLogsPolicy)]
        [ProducesResponseType(200, Type = typeof(OccupancyLogViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteOccupancyLog(int id)
        {
            var occupancyLog = await this._unitOfWork.OccupancyLogs.GetByIdAsync(id);

            OccupancyLogViewModel occupancyLogVM = _mapper.Map<OccupancyLogViewModel>(occupancyLog);
            if (occupancyLogVM == null)
                return NotFound(id);

            var result = await _unitOfWork.OccupancyLogs.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(occupancyLogVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOccupancyLogsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateOccupancyLog(string id, [FromBody] OccupancyLogViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var occupancyLog = await this._unitOfWork.OccupancyLogs.GetByIdAsync(model.Id);

                OccupancyLogViewModel occupancyLogVM = _mapper.Map<OccupancyLogViewModel>(occupancyLog);
                if (occupancyLogVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<OccupancyLog>(model);
                var result = await _unitOfWork.OccupancyLogs.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}