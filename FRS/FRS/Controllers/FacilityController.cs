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
    public class FacilityController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly string[] ACCEPTED_FILE_TYPES = new[] { ".jpg", ".jpeg", ".png" };

        public FacilityController(IUnitOfWork unitOfWork, ILogger<FacilityController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        /// <summary>
        /// API calls to get facilities
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns>List of facilities</returns>
        [HttpGet("get/{facilityId:int?}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiFacilities(int? facilityId = null, int? institutionId = null)
        {
            var result = await _unitOfWork.Facilities.GetApiFacilities(facilityId, institutionId);
            var data = Mapper.Map<List<FacilityViewModel>>(result.Data);

            if (facilityId.HasValue)
            {
                result.Data = data.Any() ? data.First() : null;
            }
            else
            {
                result.Data = data;
            }

            return Ok(result);
        }

        [HttpGet("facilities/list")]
        [Authorize(Authorization.Policies.ViewAllFacilitiesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<FacilityViewModel>))]
        public async Task<IActionResult> GetFacilities(int? institutionId = null)
        {
            return await GetFacilities(-1, -1, institutionId);
        }


        [HttpGet("facilities/list/{pageNumber:int}/{pageSize:int}")]
        [Authorize(Authorization.Policies.ViewAllFacilitiesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<FacilityViewModel>))]
        public async Task<IActionResult> GetFacilities(int pageNumber, int pageSize, int? institutionId = null)
        {
            var facilities = await _unitOfWork.Facilities.GetFacilitiesLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(Mapper.Map<List<FacilityViewModel>>(facilities));
        }

        [HttpPost("")]
        [Authorize(Authorization.Policies.ManageAllFacilitiesPolicy)]
        [ProducesResponseType(201, Type = typeof(FacilityViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateFacility([FromBody] FacilityViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                var facility = Mapper.Map<Facility>(model);

                var result = await _unitOfWork.Facilities.CreateAsync(facility, model.FilePath);
                if (result.IsSuccess)
                {
                    FacilityViewModel facilityVM = Mapper.Map<FacilityViewModel>(result.Data);
                    return CreatedAtAction("GetFacilityById", new { id = facilityVM.Id }, facilityVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Authorization.Policies.ManageAllFacilitiesPolicy)]
        [ProducesResponseType(200, Type = typeof(FacilityViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFacility(int id)
        {
            if (!await _unitOfWork.Facilities.TestCanDeleteAsync(id))
                return BadRequest("Facility cannot be deleted. Deselect this facility from all locations and try again");


            var facilityType = await this._unitOfWork.Facilities.GetByIdAsync(id);

            FacilityViewModel facilityVM = Mapper.Map<FacilityViewModel>(facilityType);
            if (facilityVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Facilities.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting facility: " + string.Join(", ", result.Message));


            return Ok(facilityVM);
        }

        [HttpPut("update/{id}")]
        [Authorize(Authorization.Policies.ManageAllFacilitiesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateFacility(string id, [FromBody] FacilityViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var facilityType = await this._unitOfWork.Facilities.GetByIdAsync(model.Id);

                FacilityViewModel facilityVM = Mapper.Map<FacilityViewModel>(facilityType);
                if (facilityVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<Facility>(model);
                var result = await _unitOfWork.Facilities.UpdateAsync(updatedModel, model.FilePath);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}