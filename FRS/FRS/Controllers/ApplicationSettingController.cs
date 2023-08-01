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
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class ApplicationSettingController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public ApplicationSettingController(IUnitOfWork unitOfWork, ILogger<ApplicationSettingController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        [HttpGet("applicationsettings/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllApplicationSettingsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ApplicationSettingViewModel>))]
        public async Task<IActionResult> GetApplicationSettings(int? institutionId = null)
        {
            return await GetApplicationSettings(-1, -1, institutionId);
        }


        [HttpGet("applicationsettings/list/{pageNumber:int}/{pageSize:int}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllApplicationSettingsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ApplicationSettingViewModel>))]
        public async Task<IActionResult> GetApplicationSettings(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.ApplicationSettings.GetApplicationSettingsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(Mapper.Map<List<ApplicationSettingViewModel>>(result));
        }

        [HttpGet("get")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(ApplicationSettingViewModel))]
        public async Task<IActionResult> GetApplicationSettings(int? institutionId = null, string key = null)
        {
            var result = await _unitOfWork.ApplicationSettings.GetByKeyAsync(key, institutionId);
            return Ok(Mapper.Map<ApplicationSettingViewModel>(result));
        }
        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllApplicationSettingsPolicy)]
        [ProducesResponseType(201, Type = typeof(ApplicationSettingViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateApplicationSetting([FromBody] ApplicationSettingViewModel applicationSetting)
        {
            if (ModelState.IsValid)
            {
                if (applicationSetting == null)
                    return BadRequest($"{nameof(applicationSetting)} cannot be null");


                var type = Mapper.Map<ApplicationSetting>(applicationSetting);

                var result = await _unitOfWork.ApplicationSettings.CreateAsync(type);
                if (result.IsSuccess)
                {
                    ApplicationSettingViewModel applicationSettingVM = Mapper.Map<ApplicationSettingViewModel>(result.Data);
                    return CreatedAtAction("GetApplicationSettingById", new { id = applicationSettingVM.Id }, applicationSettingVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllApplicationSettingsPolicy)]
        [ProducesResponseType(200, Type = typeof(ApplicationSettingViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteApplicationSetting(int id)
        {
            var applicationSetting = await this._unitOfWork.ApplicationSettings.GetByIdAsync(id);

            ApplicationSettingViewModel appSettingVM = Mapper.Map<ApplicationSettingViewModel>(applicationSetting);
            if (appSettingVM == null)
                return NotFound(id);

            var result = await _unitOfWork.ApplicationSettings.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(appSettingVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllApplicationSettingsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateApplicationSetting(string id, [FromBody] ApplicationSettingViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var applicationSetting = await this._unitOfWork.ApplicationSettings.GetByIdAsync(model.Id);

                ApplicationSettingViewModel appSettingVM = Mapper.Map<ApplicationSettingViewModel>(applicationSetting);
                if (appSettingVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<ApplicationSetting>(model);
                var result = await _unitOfWork.ApplicationSettings.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}