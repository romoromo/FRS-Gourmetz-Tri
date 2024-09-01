using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Models;
using FRS.Hubs;
using FRS.ViewModels;   
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class EmployeeScheduleController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        private IHubContext<EmployeeScheduleHub> _hub;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public EmployeeScheduleController(IUnitOfWork unitOfWork, ILogger<EmployeeScheduleController> logger, IHubContext<EmployeeScheduleHub> hub, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hub = hub;
            _mapper = mapper;
        }


        [HttpGet("list")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeSchedulesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmployeeScheduleViewModel>))]
        public async Task<IActionResult> GetEmployeeSchedules(int? institutionId = null)
        {
            return await GetEmployeeSchedules(-1, -1, institutionId);
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeSchedulesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmployeeScheduleViewModel>))]
        public async Task<IActionResult> GetEmployeeSchedules(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.EmployeeSchedules.GetEmployeeSchedulesLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(_mapper.Map<List<EmployeeScheduleViewModel>>(result));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeSchedulesPolicy)]
        [ProducesResponseType(201, Type = typeof(EmployeeScheduleViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEmployeeSchedule([FromBody] EmployeeScheduleViewModel employeeSchedule)
        {
            if (ModelState.IsValid)
            {
                if (employeeSchedule == null)
                    return BadRequest($"{nameof(employeeSchedule)} cannot be null");


                var type = _mapper.Map<EmployeeSchedule>(employeeSchedule);

                var result = await _unitOfWork.EmployeeSchedules.CreateAsync(type);
                if (result.IsSuccess)
                {
                    EmployeeScheduleViewModel employeeScheduleVM = _mapper.Map<EmployeeScheduleViewModel>(result.Data);
                    return CreatedAtAction("GetEmployeeScheduleById", new { id = employeeScheduleVM.Id }, employeeScheduleVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeSchedulesPolicy)]
        [ProducesResponseType(200, Type = typeof(EmployeeScheduleViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEmployeeSchedule(int id)
        {
            var employeeSchedule = await this._unitOfWork.EmployeeSchedules.GetByIdAsync(id);

            EmployeeScheduleViewModel employeeScheduleVM = _mapper.Map<EmployeeScheduleViewModel>(employeeSchedule);
            if (employeeScheduleVM == null)
                return NotFound(id);

            var result = await _unitOfWork.EmployeeSchedules.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(employeeScheduleVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeSchedulesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEmployeeSchedule(string id, [FromBody] EmployeeScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var employeeSchedule = await this._unitOfWork.EmployeeSchedules.GetByIdAsync(model.Id);

                EmployeeScheduleViewModel employeeScheduleVM = _mapper.Map<EmployeeScheduleViewModel>(employeeSchedule);
                if (employeeScheduleVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<EmployeeSchedule>(model);
                var result = await _unitOfWork.EmployeeSchedules.UpdateAsync(updatedModel);

                foreach (var l in updatedModel.Locations)
                {
                    await _hub.Clients.Group(l.LocationId.ToString()).SendAsync(EmployeeScheduleHub.UpdateKey, l.LocationId);
                }

                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}