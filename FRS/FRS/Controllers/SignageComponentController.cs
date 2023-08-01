using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Models;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class SignageComponentController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public SignageComponentController(IUnitOfWork unitOfWork, ILogger<SignageComponentController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        [HttpGet("signagecomponents/list")]
        //[Authorize(Authorization.Policies.ViewAllSignageComponentsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<SignageComponentViewModel>))]
        public async Task<IActionResult> GetSignageComponents(int? institutionId = null)
        {
            return await GetSignageComponents( - 1, -1, institutionId);
        }


        [HttpGet("signagecomponents/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllSignageComponentsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<SignageComponentViewModel>))]
        public async Task<IActionResult> GetSignageComponents(int pageNumber, int pageSize, int? institutionId = null)
        {
            var data = await _unitOfWork.SignageComponents.GetSignageComponentsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(Mapper.Map<List<SignageComponentViewModel>>(data));
        }

        [HttpGet("signagecomponents/export")]
        //[Authorize(Authorization.Policies.ViewAllSignageComponentsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<SignageComponentViewModel>))]
        public async Task<IActionResult> GetSignageComponents(string ids)
        {
            var data = await _unitOfWork.SignageComponents.GetSignageComponents(ids);
            return Ok(Mapper.Map<List<SignageComponentViewModel>>(data));
        }

        [HttpGet("signagecomponents/import")]
        //[Authorize(Authorization.Policies.ViewAllSignageComponentsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<SignageComponentViewModel>))]
        public async Task<IActionResult> ImportSignageComponents(string filename)
        {
            try
            {
                var fullpath = Path.Combine(Directory.GetCurrentDirectory(), filename);

                var JSON = System.IO.File.ReadAllText(fullpath);

                var datas = JsonConvert.DeserializeObject<List<SignageComponentViewModel>>(JSON);

                foreach(var d in datas)
                {
                    await AddOrUpdate(d);
                }

                return Ok(new { });
            } catch(Exception ex)
            {
                return StatusCode(500, "Internal server error - " + ex.StackTrace);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task AddOrUpdate(SignageComponentViewModel d)
        {
            var dbdata = await _unitOfWork.SignageComponents.GetByCodeAsync(d.Name);

            if (dbdata == null)
            {
                d.Id = 0;

                await CreateSignageComponent(d);
            }
            else
            {
                d.Id = dbdata.Id;

                await UpdateSignageComponent(dbdata.Id.ToString(), d);
            }
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllSignageComponentsPolicy)]
        [ProducesResponseType(201, Type = typeof(SignageComponentViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateSignageComponent([FromBody] SignageComponentViewModel signageComponent)
        {
            if (ModelState.IsValid)
            {
                if (signageComponent == null)
                    return BadRequest($"{nameof(signageComponent)} cannot be null");


                var type = Mapper.Map<SignageComponent>(signageComponent);

                var result = await _unitOfWork.SignageComponents.CreateAsync(type);
                if (result.IsSuccess)
                {
                    SignageComponentViewModel signageComponentVM = Mapper.Map<SignageComponentViewModel>(result.Data);
                    return CreatedAtAction("GetSignageComponentById", new { id = signageComponentVM.Id }, signageComponentVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllSignageComponentsPolicy)]
        [ProducesResponseType(200, Type = typeof(SignageComponentViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSignageComponent(int id)
        {
            var signageComponent = await this._unitOfWork.SignageComponents.GetByIdAsync(id);

            SignageComponentViewModel signageComponentVM = Mapper.Map<SignageComponentViewModel>(signageComponent);
            if (signageComponentVM == null)
                return NotFound(id);

            var result = await _unitOfWork.SignageComponents.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting signage component: " + string.Join(", ", result.Message));


            return Ok(signageComponentVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllSignageComponentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSignageComponent(string id, [FromBody] SignageComponentViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var signageComponent = await this._unitOfWork.SignageComponents.GetByIdAsync(model.Id);

                SignageComponentViewModel signageComponentVM = Mapper.Map<SignageComponentViewModel>(signageComponent);
                if (signageComponentVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<SignageComponent>(model);
                var result = await _unitOfWork.SignageComponents.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}