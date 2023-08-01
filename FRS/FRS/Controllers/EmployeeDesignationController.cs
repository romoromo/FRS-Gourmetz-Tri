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
    public class EmployeeDesignationController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public EmployeeDesignationController(IUnitOfWork unitOfWork, ILogger<EmployeeDesignationController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        [HttpGet("list")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDesignationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmployeeDesignationViewModel>))]
        public async Task<IActionResult> GetEmployeeDesignations(int? institutionId = null)
        {
            return await GetEmployeeDesignations(-1, -1, institutionId);
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDesignationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmployeeDesignationViewModel>))]
        public async Task<IActionResult> GetEmployeeDesignations(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.EmployeeDesignations.GetEmployeeDesignationsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(Mapper.Map<List<EmployeeDesignationViewModel>>(result));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDesignationsPolicy)]
        [ProducesResponseType(201, Type = typeof(EmployeeDesignationViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEmployeeDesignation([FromBody] EmployeeDesignationViewModel employeeDesignation)
        {
            if (ModelState.IsValid)
            {
                if (employeeDesignation == null)
                    return BadRequest($"{nameof(employeeDesignation)} cannot be null");


                var type = Mapper.Map<EmployeeDesignation>(employeeDesignation);

                var result = await _unitOfWork.EmployeeDesignations.CreateAsync(type);
                if (result.IsSuccess)
                {
                    EmployeeDesignationViewModel employeeDesignationVM = Mapper.Map<EmployeeDesignationViewModel>(result.Data);
                    return CreatedAtAction("GetEmployeeDesignationById", new { id = employeeDesignationVM.Id }, employeeDesignationVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDesignationsPolicy)]
        [ProducesResponseType(200, Type = typeof(EmployeeDesignationViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEmployeeDesignation(int id)
        {
            var employeeDesignation = await this._unitOfWork.EmployeeDesignations.GetByIdAsync(id);

            EmployeeDesignationViewModel employeeDesignationVM = Mapper.Map<EmployeeDesignationViewModel>(employeeDesignation);
            if (employeeDesignationVM == null)
                return NotFound(id);

            var result = await _unitOfWork.EmployeeDesignations.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(employeeDesignationVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmployeeDesignationsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEmployeeDesignation(string id, [FromBody] EmployeeDesignationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var employeeDesignation = await this._unitOfWork.EmployeeDesignations.GetByIdAsync(model.Id);

                EmployeeDesignationViewModel employeeDesignationVM = Mapper.Map<EmployeeDesignationViewModel>(employeeDesignation);
                if (employeeDesignationVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<EmployeeDesignation>(model);
                var result = await _unitOfWork.EmployeeDesignations.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}