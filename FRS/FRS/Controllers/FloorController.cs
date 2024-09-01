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

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class FloorController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public FloorController(IUnitOfWork unitOfWork, ILogger<FloorController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("list")]
        //[Authorize(Authorization.Policies.ManageAllFloorsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<FloorViewModel>))]
        public async Task<IActionResult> GetFloors(int? institutionId = null)
        {
            return await GetFloors(-1, -1, institutionId);
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ManageAllFloorsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<FloorViewModel>))]
        public async Task<IActionResult> GetFloors(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.Floors.GetFloorsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(_mapper.Map<List<FloorViewModel>>(result));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllFloorsPolicy)]
        [ProducesResponseType(201, Type = typeof(FloorViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateFloor([FromBody] FloorViewModel floor)
        {
            if (ModelState.IsValid)
            {
                if (floor == null)
                    return BadRequest($"{nameof(floor)} cannot be null");


                var type = _mapper.Map<Floor>(floor);

                var result = await _unitOfWork.Floors.CreateAsync(type);
                if (result.IsSuccess)
                {
                    FloorViewModel floorVM = _mapper.Map<FloorViewModel>(result.Data);
                    return CreatedAtAction("GetFloorById", new { id = floorVM.Id }, floorVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllFloorsPolicy)]
        [ProducesResponseType(200, Type = typeof(FloorViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFloor(int id)
        {
            var floor = await this._unitOfWork.Floors.GetByIdAsync(id);

            FloorViewModel floorVM = _mapper.Map<FloorViewModel>(floor);
            if (floorVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Floors.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(floorVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllFloorsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateFloor(string id, [FromBody] FloorViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var floor = await this._unitOfWork.Floors.GetByIdAsync(model.Id);

                FloorViewModel floorVM = _mapper.Map<FloorViewModel>(floor);
                if (floorVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<Floor>(model);
                var result = await _unitOfWork.Floors.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}