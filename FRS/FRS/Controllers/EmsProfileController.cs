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
    public class EmsProfileController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;
        private IHubContext<FRSHub> _frsHub;

        public EmsProfileController(IUnitOfWork unitOfWork, ILogger<EmsProfileController> logger, IHubContext<FRSHub> frsHub, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _frsHub = frsHub;
            _mapper = mapper;
        }

        /// <summary>
        /// API calls to create emsProfile
        /// </summary>
        /// <param name="emsProfile"></param>
        /// <returns>BaseOperationResponse</returns>
        [HttpPost("create")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ApiCreateEmsProfile([FromBody] EmsProfileViewModel emsProfile)
        {
            var result = new BaseOperationResponse();
            if (ModelState.IsValid)
            {
                var emsProfileInfo = _mapper.Map<EmsProfile>(emsProfile);

                result = await _unitOfWork.EmsProfiles.CreateAsync(emsProfileInfo);
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Missing required fields. Please check your inputs.";
            }

            return Ok(result);
        }

        [HttpGet("get/emsprofiles")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmsProfileViewModel>))]
        public async Task<IActionResult> GetEmsProfiles()
        {
            var results = await _unitOfWork.EmsProfiles.GetEmsProfilesLoadRelatedAsync(-1, -1);
            return Ok(_mapper.Map<List<EmsProfileViewModel>>(results));
        }

        [HttpPost("GetAllEmsProfiles")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllEmsProfilesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmsProfileViewModel>))]
        public async Task<IActionResult> GetAllEmsProfiles()
        {
                return await GetEmsProfiles(-1, -1);
        }

        [HttpGet("GetEmsProfiles/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllEmsProfilesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<EmsProfileViewModel>))]
        public async Task<IActionResult> GetEmsProfiles(int pageNumber, int pageSize)
        {
            var facilities = await _unitOfWork.EmsProfiles.GetEmsProfilesLoadRelatedAsync(pageNumber, pageSize);
            return Ok(_mapper.Map<List<EmsProfileViewModel>>(facilities));
        }

        [HttpPost("")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllEmsProfilesPolicy)]
        [ProducesResponseType(201, Type = typeof(EmsProfileViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEmsProfile([FromBody] EmsProfileViewModel emsProfile)
        {
            if (ModelState.IsValid)
            {
                if (emsProfile == null)
                    return BadRequest($"{nameof(emsProfile)} cannot be null");


                var emsProfileInfo = _mapper.Map<EmsProfile>(emsProfile);

                var result = await _unitOfWork.EmsProfiles.CreateAsync(emsProfileInfo);
                if (result.IsSuccess)
                {
                    EmsProfileViewModel vm = _mapper.Map<EmsProfileViewModel>(result.Data);
                    return CreatedAtAction("GetEmsProfileById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmsProfilesPolicy)]
        [ProducesResponseType(200, Type = typeof(EmsProfileViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEmsProfile(int id)
        {
   
            var emsProfileType = await this._unitOfWork.EmsProfiles.GetByIdAsync(id);

            EmsProfileViewModel emsProfileVM = _mapper.Map<EmsProfileViewModel>(emsProfileType);
            if (emsProfileVM == null)
                return NotFound(id);

            var result = await _unitOfWork.EmsProfiles.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting ems profile: " + string.Join(", ", result.Message));


            return Ok(emsProfileVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmsProfilesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEmsProfile(string id, [FromBody] EmsProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var emsProfileType = await this._unitOfWork.EmsProfiles.GetByIdAsync(model.Id);

                EmsProfileViewModel emsProfileVM = _mapper.Map<EmsProfileViewModel>(emsProfileType);
                if (emsProfileVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<EmsProfile>(model);
                var result = await _unitOfWork.EmsProfiles.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                {
                    return NoContent();
                }

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}