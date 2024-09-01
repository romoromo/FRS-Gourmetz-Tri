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
using System.Diagnostics;
using NPOI.SS.Formula.Functions;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class KioskSettingsController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public KioskSettingsController(IUnitOfWork unitOfWork, ILogger<KioskSettingsController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("id/{id}")]
        //[Authorize(Authorization.Policies.ViewAllLocationsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<KioskSettingsViewModel>))]
        public async Task<IActionResult> GetLocationById(int id)
        {
            var kioskSettings = await _unitOfWork.KioskSettings.GetByIdAsync(id);
            return Ok(_mapper.Map<KioskSettingsViewModel>(kioskSettings));
        }

        [HttpGet("KioskSettings/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<KioskSettingsViewModel>))]
        public async Task<IActionResult> GetKioskSettings(int? institutionId = null)
        {
            return await GetKioskSettings(-1, -1, institutionId);
        }


        [HttpGet("kioskSettings/list/{pageNumber:int}/{pageSize:int}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<KioskSettingsViewModel>))]
        public async Task<IActionResult> GetKioskSettings(int pageNumber, int pageSize, int? institutionId = null)
        {
            var results = await _unitOfWork.KioskSettings.GetKioskSettingsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(_mapper.Map<List<KioskSettingsViewModel>>(results));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllContactGroupsPolicy)]
        [ProducesResponseType(201, Type = typeof(KioskSettingsViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateKioskSettings([FromBody] KioskSettingsViewModel kioskSettings)
        {
            if (ModelState.IsValid)
            {
                if (kioskSettings == null)
                    return BadRequest($"{nameof(kioskSettings)} cannot be null");


                var type = _mapper.Map<KioskSettings>(kioskSettings);

                var result = await _unitOfWork.KioskSettings.CreateAsync(type);
                if (result.IsSuccess && result.Data != null)
                {
                    var Id = result.Data.GetType().GetProperty("Id").GetValue(result.Data, null);
                    //ContactGroupViewModel contactGroupVM = _mapper.Map<ContactGroupViewModel>(result.Data);
                    return CreatedAtAction("GetKioskSettingsById", new { id = Id }, kioskSettings);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(KioskSettingsViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteKioskSettings(int id)
        {
            //var testDeleteResult = await _unitOfWork.Playlists.TestCanDeleteAsync(id);
            //if (!testDeleteResult.IsDeletable)
            //    return BadRequest(string.Format("Contact Group cannot be deleted. {0}", testDeleteResult.Message));


            var kioskSettings = await this._unitOfWork.KioskSettings.GetByIdAsync(id);

            KioskSettingsViewModel kioskSettingsVM = _mapper.Map<KioskSettingsViewModel>(kioskSettings);
            if (kioskSettingsVM == null)
                return NotFound(id);

            var result = await _unitOfWork.KioskSettings.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting kiosk settings: " + string.Join(", ", result.Message));


            return Ok(kioskSettingsVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllContactGroupsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateKioskSettings(string id, [FromBody] KioskSettingsViewModel model)
        {
            Debug.WriteLine("Model before: ", Newtonsoft.Json.JsonConvert.SerializeObject(model));
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var kioskSettings = await this._unitOfWork.KioskSettings.GetByIdAsync(model.Id);

                KioskSettingsViewModel kioskSettingsVM = _mapper.Map<KioskSettingsViewModel>(kioskSettings);
                if (kioskSettingsVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<KioskSettings>(model);
                Debug.WriteLine("Model to update: ", Newtonsoft.Json.JsonConvert.SerializeObject(updatedModel));
                var result = await _unitOfWork.KioskSettings.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
        
    }
}