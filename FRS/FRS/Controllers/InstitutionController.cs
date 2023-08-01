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
    public class InstitutionController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public InstitutionController(IUnitOfWork unitOfWork, ILogger<InstitutionController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// API call to get default institution
        /// </summary>
        /// <returns>First Default Institution</returns>
        [HttpGet("get/default")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetDefaultInstitution()
        {
            var result = await _unitOfWork.Institutions.GetDefaultInstitutionAsync();
            return Ok(Mapper.Map<InstitutionViewModel>(result));
        }

        /// <summary>
        /// API calls to get institutions
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns>List of institutions</returns>
        [HttpGet("get/{institutionId:int?}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiInstitutions(int? institutionId = null)
        {
            var result = await _unitOfWork.Institutions.GetApiInstitutions(institutionId);
            var data = Mapper.Map<List<InstitutionViewModel>>(result.Data);

            if (institutionId.HasValue)
            {
                result.Data = data.Any() ? data.First() : null;
            }
            else
            {
                result.Data = data;
            }

            return Ok(result);
        }

        [HttpGet("institutions/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<InstitutionViewModel>))]
        public async Task<IActionResult> GetInstitutions()
        {
            return await GetInstitutions(-1, -1);
        }


        [HttpGet("institutions/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<InstitutionViewModel>))]
        public async Task<IActionResult> GetInstitutions(int pageNumber, int pageSize)
        {
            var results = await _unitOfWork.Institutions.GetInstitutionsLoadRelatedAsync(pageNumber, pageSize);
            return Ok(Mapper.Map<List<InstitutionViewModel>>(results));
        }

        [HttpPost("")]
        [Authorize(Authorization.Policies.ManageAllInstitutionsPolicy)]
        [ProducesResponseType(201, Type = typeof(InstitutionViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateFacility([FromBody] InstitutionViewModel institution)
        {
            if (ModelState.IsValid)
            {
                if (institution == null)
                    return BadRequest($"{nameof(institution)} cannot be null");


                var type = Mapper.Map<Institution>(institution);

                var result = await _unitOfWork.Institutions.CreateAsync(type);
                if (result.IsSuccess)
                {
                    InstitutionViewModel institutionVM = Mapper.Map<InstitutionViewModel>(result.Data);
                    return CreatedAtAction("GetInstitutionById", new { id = institutionVM.Id }, institutionVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Authorization.Policies.ManageAllInstitutionsPolicy)]
        [ProducesResponseType(200, Type = typeof(InstitutionViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteInstitution(int id)
        {
            if (!await _unitOfWork.Institutions.TestCanDeleteAsync(id))
                return BadRequest("Institution cannot be deleted."); //TODO: correct message here


            var institution = await this._unitOfWork.Institutions.GetByIdAsync(id);

            InstitutionViewModel institutionVM = Mapper.Map<InstitutionViewModel>(institution);
            if (institutionVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Institutions.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting institution: " + string.Join(", ", result.Message));


            return Ok(institutionVM);
        }

        [HttpPut("update/{id}")]
        [Authorize(Authorization.Policies.ManageAllInstitutionsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateInstitution(string id, [FromBody] InstitutionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var institution = await this._unitOfWork.Institutions.GetByIdAsync(model.Id);

                InstitutionViewModel institutionVM = Mapper.Map<InstitutionViewModel>(institution);
                if (institutionVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<Institution>(model);
                var result = await _unitOfWork.Institutions.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}