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
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class EmployeeDataController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public EmployeeDataController(IUnitOfWork unitOfWork, ILogger<EmployeeDataController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        [HttpGet("list")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDatasPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmployeeDataViewModel>))]
        public async Task<IActionResult> GetEmployeeDatas(int? institutionId = null)
        {
            return await GetEmployeeDatas(-1, -1, institutionId);
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDatasPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmployeeDataViewModel>))]
        public async Task<IActionResult> GetEmployeeDatas(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.EmployeeDatas.GetEmployeeDatasLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(Mapper.Map<List<EmployeeDataViewModel>>(result));
        }

        [HttpGet("getcurrentemployeebylocation")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDatasPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmployeeDataViewModel>))]
        public async Task<IActionResult> GetCurrentEmployeesByLocation(int? locationId = null)
        {
            var result = await _unitOfWork.EmployeeDatas.GetCurrentEmployeesByLocation(locationId);
            return Ok(Mapper.Map<List<EmployeeDataViewModel>>(result));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDatasPolicy)]
        [ProducesResponseType(201, Type = typeof(EmployeeDataViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEmployeeData([FromBody] EmployeeDataViewModel employeeData)
        {
            if (ModelState.IsValid)
            {
                if (employeeData == null)
                    return BadRequest($"{nameof(employeeData)} cannot be null");


                var type = Mapper.Map<EmployeeData>(employeeData);

                var result = await _unitOfWork.EmployeeDatas.CreateAsync(type);
                if (result.IsSuccess)
                {
                    EmployeeDataViewModel employeeDataVM = Mapper.Map<EmployeeDataViewModel>(result.Data);
                    return CreatedAtAction("GetEmployeeDataById", new { id = employeeDataVM.Id }, employeeDataVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDatasPolicy)]
        [ProducesResponseType(200, Type = typeof(EmployeeDataViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEmployeeData(int id)
        {
            var employeeData = await this._unitOfWork.EmployeeDatas.GetByIdAsync(id);

            EmployeeDataViewModel employeeDataVM = Mapper.Map<EmployeeDataViewModel>(employeeData);
            if (employeeDataVM == null)
                return NotFound(id);

            var result = await _unitOfWork.EmployeeDatas.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(employeeDataVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDatasPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEmployeeData(string id, [FromBody] EmployeeDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var employeeData = await this._unitOfWork.EmployeeDatas.GetByIdAsync(model.Id);

                EmployeeDataViewModel employeeDataVM = Mapper.Map<EmployeeDataViewModel>(employeeData);
                if (employeeDataVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<EmployeeData>(model);
                var result = await _unitOfWork.EmployeeDatas.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}