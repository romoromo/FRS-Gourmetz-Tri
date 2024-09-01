using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
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
    public class FacilityTypeController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public FacilityTypeController(IUnitOfWork unitOfWork, ILogger<FacilityTypeController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("facilitytypes/list")]
        [Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<FacilityViewModel>))]
        public async Task<IActionResult> GetFacilityTypes(int? institutionId = null)
        {
            return await GetFacilityTypes( - 1, -1, institutionId);
        }


        [HttpGet("facilitytypes/list/{pageNumber:int}/{pageSize:int}")]
        [Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<FacilityTypeViewModel>))]
        public async Task<IActionResult> GetFacilityTypes(int pageNumber, int pageSize, int? institutionId = null)
        {
            var facilities = await _unitOfWork.FacilityTypes.GetFacilityTypesLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(_mapper.Map<List<FacilityTypeViewModel>>(facilities));
        }

        [HttpPost("")]
        [Authorize(Authorization.Policies.ManageAllFacilityTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(FacilityTypeViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateFacility([FromBody] FacilityTypeViewModel facilityType)
        {
            if (ModelState.IsValid)
            {
                if (facilityType == null)
                    return BadRequest($"{nameof(facilityType)} cannot be null");


                var type = _mapper.Map<FacilityType>(facilityType);

                var result = await _unitOfWork.FacilityTypes.CreateAsync(type);
                if (result.IsSuccess)
                {
                    FacilityTypeViewModel facilityTypeVM = _mapper.Map<FacilityTypeViewModel>(result.Data);
                    return CreatedAtAction("GetFacilityTypeById", new { id = facilityTypeVM.Id }, facilityTypeVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Authorization.Policies.ManageAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(FacilityViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFacilityType(int id)
        {
            if (!await _unitOfWork.FacilityTypes.TestCanDeleteAsync(id))
                return BadRequest("Facility type cannot be deleted. Remove all facilities from this facility type and try again");


            var facilityType = await this._unitOfWork.FacilityTypes.GetByIdAsync(id);

            FacilityTypeViewModel facilityVM = _mapper.Map<FacilityTypeViewModel>(facilityType);
            if (facilityVM == null)
                return NotFound(id);

            var result = await _unitOfWork.FacilityTypes.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting facility: " + string.Join(", ", result.Message));


            return Ok(facilityVM);
        }

        [HttpPut("update/{id}")]
        [Authorize(Authorization.Policies.ManageAllFacilityTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateFacilityType(string id, [FromBody] FacilityTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var facilityType = await this._unitOfWork.FacilityTypes.GetByIdAsync(model.Id);

                FacilityTypeViewModel facilityVM = _mapper.Map<FacilityTypeViewModel>(facilityType);
                if (facilityVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<FacilityType>(model);
                var result = await _unitOfWork.FacilityTypes.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}