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
    public class ModuleController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public ModuleController(IUnitOfWork unitOfWork, ILogger<ModuleController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// API calls to get modules
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns>List of modules</returns>
        [HttpGet("get/{moduleId:int?}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiModules(int? moduleId = null)
        {
            var result = await _unitOfWork.Modules.GetApiModules(moduleId).ConfigureAwait(false);
            var data = Mapper.Map<List<ModuleViewModel>>(result.Data);

            if (moduleId.HasValue)
            {
                result.Data = data.Any() ? data.First() : null;
            }
            else
            {
                result.Data = data;
            }

            return Ok(result);
        }

        [HttpGet("modules/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllModuleTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ModuleViewModel>))]
        public async Task<IActionResult> GetModules()
        {
            return await GetModules(-1, -1);
        }


        [HttpGet("modules/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllModuleTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ModuleViewModel>))]
        public async Task<IActionResult> GetModules(int pageNumber, int pageSize)
        {
            var results = await _unitOfWork.Modules.GetModulesLoadRelatedAsync(pageNumber, pageSize).ConfigureAwait(false);
            return Ok(Mapper.Map<List<ModuleViewModel>>(results));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllModulesPolicy)]
        [ProducesResponseType(201, Type = typeof(ModuleViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateModule([FromBody] ModuleViewModel module)
        {
            if (ModelState.IsValid)
            {
                if (module == null)
                    return BadRequest($"{nameof(module)} cannot be null");


                var type = Mapper.Map<Module>(module);

                var result = await _unitOfWork.Modules.CreateAsync(type);
                if (result.IsSuccess)
                {
                    ModuleViewModel moduleVM = Mapper.Map<ModuleViewModel>(result.Data);
                    return CreatedAtAction("GetModuleById", new { id = moduleVM.Id }, moduleVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllModulesPolicy)]
        [ProducesResponseType(200, Type = typeof(ModuleViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteModule(int id)
        {
            //if (!await _unitOfWork.Modules.TestCanDeleteAsync(id))
            //    return BadRequest("Module cannot be deleted."); //TODO: correct message here


            var module = await this._unitOfWork.Modules.GetByIdAsync(id).ConfigureAwait(false);

            ModuleViewModel moduleVM = Mapper.Map<ModuleViewModel>(module);
            if (moduleVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Modules.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting module: " + string.Join(", ", result.Message));


            return Ok(moduleVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllModulesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateModule(string id, [FromBody] ModuleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var module = await this._unitOfWork.Modules.GetByIdAsync(model.Id).ConfigureAwait(false);

                ModuleViewModel moduleVM = Mapper.Map<ModuleViewModel>(module);
                if (moduleVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<Module>(model);
                var result = await _unitOfWork.Modules.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}