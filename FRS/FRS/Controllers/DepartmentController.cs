using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
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
    public class DepartmentController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public DepartmentController(IUnitOfWork unitOfWork, ILogger<DepartmentController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("departments/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDepartments(BaseFilter filter)
        {
            var departments = await _unitOfWork.Departments.GetDepartmentsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<DepartmentViewModel>>(departments));
        }

        #endregion

        [HttpGet("departments/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<DepartmentViewModel>))]
        public async Task<IActionResult> GetDepartments(int? institutionId = null, string institutionCode = null)
        {
            return await GetDepartments( - 1, -1, institutionId, institutionCode);
        }


        [HttpGet("departments/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<DepartmentViewModel>))]
        public async Task<IActionResult> GetDepartments(int pageNumber, int pageSize, int? institutionId = null, string institutionCode = null)
        {
            var facilities = await _unitOfWork.Departments.GetDepartmentsLoadRelatedAsync(pageNumber, pageSize, institutionId, institutionCode);
            return Ok(_mapper.Map<List<DepartmentViewModel>>(facilities));
        }

        [HttpPost("")]
        [Authorize(Authorization.Policies.ManageAllDepartmentsPolicy)]
        [ProducesResponseType(201, Type = typeof(DepartmentViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentViewModel department)
        {
            if (ModelState.IsValid)
            {
                if (department == null)
                    return BadRequest($"{nameof(department)} cannot be null");


                var type = _mapper.Map<Department>(department);

                var result = await _unitOfWork.Departments.CreateAsync(type);
                if (result.IsSuccess)
                {
                    DepartmentViewModel departmentVM = _mapper.Map<DepartmentViewModel>(result.Data);
                    return CreatedAtAction("GetDepartmentById", new { id = departmentVM.Id }, departmentVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Authorization.Policies.ManageAllDepartmentsPolicy)]
        [ProducesResponseType(200, Type = typeof(DepartmentViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            if (!await _unitOfWork.Departments.TestCanDeleteAsync(id))
                return BadRequest("Department cannot be deleted. Remove all users and contact groups from this department and try again");


            var department = await this._unitOfWork.Departments.GetByIdAsync(id);

            DepartmentViewModel departmentVM = _mapper.Map<DepartmentViewModel>(department);
            if (departmentVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Departments.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting department: " + string.Join(", ", result.Message));


            return Ok(departmentVM);
        }

        [HttpPut("update/{id}")]
        [Authorize(Authorization.Policies.ManageAllDepartmentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDepartment(string id, [FromBody] DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var department = await this._unitOfWork.Departments.GetByIdAsync(model.Id);

                DepartmentViewModel departmentVM = _mapper.Map<DepartmentViewModel>(department);
                if (departmentVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<Department>(model);
                var result = await _unitOfWork.Departments.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}