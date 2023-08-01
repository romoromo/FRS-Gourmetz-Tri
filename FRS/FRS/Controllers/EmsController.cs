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
    public class EmsController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public EmsController(IUnitOfWork unitOfWork, ILogger<EmsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// API calls to get emses
        /// </summary>
        /// <param name="emsId"></param>
        /// <returns>List of emses</returns>
        [HttpGet("get/{emsId:int?}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiEmses(int? emsId = null)
        {
            var result = await _unitOfWork.Emses.GetApiEmses(emsId).ConfigureAwait(false);
            var data = Mapper.Map<List<EmsViewModel>>(result.Data);

            if (emsId.HasValue)
            {
                result.Data = data.Any() ? data.First() : null;
            }
            else
            {
                result.Data = data;
            }

            return Ok(result);
        }

        [HttpGet("emses/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllEmsTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmsViewModel>))]
        public async Task<IActionResult> GetEmses()
        {
            return await GetEmses(-1, -1);
        }


        [HttpGet("emses/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllEmsTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EmsViewModel>))]
        public async Task<IActionResult> GetEmses(int pageNumber, int pageSize)
        {
            var results = await _unitOfWork.Emses.GetEmsesLoadRelatedAsync(pageNumber, pageSize).ConfigureAwait(false);
            return Ok(Mapper.Map<List<EmsViewModel>>(results));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllEmsesPolicy)]
        [ProducesResponseType(201, Type = typeof(EmsViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEms([FromBody] EmsViewModel ems)
        {
            if (ModelState.IsValid)
            {
                if (ems == null)
                    return BadRequest($"{nameof(ems)} cannot be null");


                var type = Mapper.Map<Ems>(ems);

                var result = await _unitOfWork.Emses.CreateAsync(type);
                if (result.IsSuccess)
                {
                    EmsViewModel emsVM = Mapper.Map<EmsViewModel>(result.Data);
                    return CreatedAtAction("GetEmsById", new { id = emsVM.Id }, emsVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmsesPolicy)]
        [ProducesResponseType(200, Type = typeof(EmsViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEms(int id)
        {
            //if (!await _unitOfWork.Emses.TestCanDeleteAsync(id))
            //    return BadRequest("Ems cannot be deleted."); //TODO: correct message here


            var ems = await this._unitOfWork.Emses.GetByIdAsync(id).ConfigureAwait(false);

            EmsViewModel emsVM = Mapper.Map<EmsViewModel>(ems);
            if (emsVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Emses.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting ems: " + string.Join(", ", result.Message));


            return Ok(emsVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmsesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEms(string id, [FromBody] EmsViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var ems = await this._unitOfWork.Emses.GetByIdAsync(model.Id).ConfigureAwait(false);

                EmsViewModel emsVM = Mapper.Map<EmsViewModel>(ems);
                if (emsVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<Ems>(model);
                var result = await _unitOfWork.Emses.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}