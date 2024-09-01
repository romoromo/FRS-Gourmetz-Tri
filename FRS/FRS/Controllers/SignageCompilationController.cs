using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using System.IO;
using Newtonsoft.Json;
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
    public class SignageCompilationController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private SignageComponentController _componentController;
        private readonly IMapper _mapper;


        public SignageCompilationController(IUnitOfWork unitOfWork, ILogger<SignageCompilationController> logger, SignageComponentController componentController, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _componentController = componentController;
            _mapper = mapper;
        }


        [HttpGet("signagecompilations/list")]
        //[Authorize(Authorization.Policies.ViewAllSignageCompilationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<SignageCompilationViewModel>))]
        public async Task<IActionResult> GetSignageCompilations(int? institutionId = null)
        {
            return await GetSignageCompilations(-1, -1, institutionId);
        }


        [HttpGet("signagecompilations/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllSignageCompilationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<SignageCompilationViewModel>))]
        public async Task<IActionResult> GetSignageCompilations(int pageNumber, int pageSize, int? institutionId = null)
        {
            var data = await _unitOfWork.SignageCompilations.GetSignageCompilationsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(_mapper.Map<List<SignageCompilationViewModel>>(data));
        }

        [HttpGet("signagecompilations/export")]
        //[Authorize(Authorization.Policies.ViewAllSignageComponentsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<SignageComponentViewModel>))]
        public async Task<IActionResult> GetSignageComponents(string ids)
        {
            var data = await _unitOfWork.SignageCompilations.GetSignageCompilations(ids);
            return Ok(_mapper.Map<List<SignageCompilationViewModel>>(data));
        }

        [HttpGet("signagecompilations/import")]
        //[Authorize(Authorization.Policies.ViewAllSignageComponentsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<SignageComponentViewModel>))]
        public async Task<IActionResult> ImportSignageComponents(string filename)
        {
            try
            {
                var fullpath = Path.Combine(Directory.GetCurrentDirectory(), filename);

                var JSON = System.IO.File.ReadAllText(fullpath);

                var datas = JsonConvert.DeserializeObject<List<SignageCompilationViewModel>>(JSON);

                foreach (var di in datas)
                {
                    var components = di.components;
                    di.components = null;

                    var dbdata = await _unitOfWork.SignageCompilations.GetByCodeAsync(di.Name);

                    if (dbdata == null)
                    {
                        di.Id = 0;

                        await CreateSignageCompilation(di);
                    }
                    else
                    {
                        di.Id = dbdata.Id;

                        await UpdateSignageCompilation(dbdata.Id.ToString(), di);
                    }

                    if (components != null)
                    {
                        dbdata = await _unitOfWork.SignageCompilations.GetByCodeAsync(di.Name);

                        foreach (var dco in components)
                        {
                            await _componentController.AddOrUpdate(dco);

                            var dcodata = await _unitOfWork.SignageComponents.GetByCodeAsync(dco.Name);

                            dco.CompilationId = dbdata.Id;
                            dco.ComponentId = dcodata.Id;

                            await _unitOfWork.SignageCompilations.AddOrUpdateCompilationComponentAsync(_mapper.Map<SignageCompilationComponent>(dco));
                        }
                    }
                }

                return Ok(new { });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error - " + ex.StackTrace);
            }
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllSignageCompilationsPolicy)]
        [ProducesResponseType(201, Type = typeof(SignageCompilationViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateSignageCompilation([FromBody] SignageCompilationViewModel signageCompilation)
        {
            if (ModelState.IsValid)
            {
                if (signageCompilation == null)
                    return BadRequest($"{nameof(signageCompilation)} cannot be null");


                var type = _mapper.Map<SignageCompilation>(signageCompilation);

                var result = await _unitOfWork.SignageCompilations.CreateAsync(type);
                if (result.IsSuccess)
                {
                    SignageCompilationViewModel signageCompilationVM = _mapper.Map<SignageCompilationViewModel>(result.Data);
                    return CreatedAtAction("GetSignageCompilationById", new { id = signageCompilationVM.Id }, signageCompilationVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllSignageCompilationsPolicy)]
        [ProducesResponseType(200, Type = typeof(SignageCompilationViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSignageCompilation(int id)
        {
            var signageCompilation = await this._unitOfWork.SignageCompilations.GetByIdAsync(id);

            SignageCompilationViewModel signageCompilationVM = _mapper.Map<SignageCompilationViewModel>(signageCompilation);
            if (signageCompilationVM == null)
                return NotFound(id);

            var result = await _unitOfWork.SignageCompilations.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting signage compilation: " + string.Join(", ", result.Message));


            return Ok(signageCompilationVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllSignageCompilationsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSignageCompilation(string id, [FromBody] SignageCompilationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var signageCompilation = await this._unitOfWork.SignageCompilations.GetByIdAsync(model.Id);

                SignageCompilationViewModel signageCompilationVM = _mapper.Map<SignageCompilationViewModel>(signageCompilation);
                if (signageCompilationVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<SignageCompilation>(model);
                var result = await _unitOfWork.SignageCompilations.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}