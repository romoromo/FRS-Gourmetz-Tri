using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    public class BuildingController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public BuildingController(IUnitOfWork unitOfWork, ILogger<BuildingController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBuildingsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<BuildingViewModel>))]
        public async Task<IActionResult> GetBuildings(int? institutionId = null)
        {
            return await GetBuildings(-1, -1, institutionId);
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBuildingsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<BuildingViewModel>))]
        public async Task<IActionResult> GetBuildings(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.Buildings.GetBuildingsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            var rmap = _mapper.Map<List<BuildingViewModel>>(result);

            rmap.ForEach(b =>
            {
                b.floors?.ForEach(f =>
                {
                    f.Map = _mapper.Map<MapViewModel>(_unitOfWork.Maps.GetMapByFloorId(f.FloorId));

                    if (f.Map != null && !string.IsNullOrWhiteSpace(f.Map.map_url))
                    {
                        var file = Path.Combine(Directory.GetCurrentDirectory(), f.Map.map_url);
                        var img = Image.FromFile(file);
                        f.Map.width = img.Width;
                        f.Map.height = img.Height;
                    }
                });
            });

            return Ok(rmap);
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllBuildingsPolicy)]
        [ProducesResponseType(201, Type = typeof(BuildingViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateBuilding([FromBody] BuildingViewModel building)
        {
            if (ModelState.IsValid)
            {
                if (building == null)
                    return BadRequest($"{nameof(building)} cannot be null");


                var type = _mapper.Map<Building>(building);

                var result = await _unitOfWork.Buildings.CreateAsync(type);
                if (result.IsSuccess)
                {
                    BuildingViewModel buildingVM = _mapper.Map<BuildingViewModel>(result.Data);
                    return CreatedAtAction("GetBuildingById", new { id = buildingVM.Id }, buildingVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllBuildingsPolicy)]
        [ProducesResponseType(200, Type = typeof(BuildingViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteBuilding(int id)
        {
            var building = await this._unitOfWork.Buildings.GetByIdAsync(id);

            BuildingViewModel buildingVM = _mapper.Map<BuildingViewModel>(building);
            if (buildingVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Buildings.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(buildingVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllBuildingsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateBuilding(string id, [FromBody] BuildingViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var building = await this._unitOfWork.Buildings.GetByIdAsync(model.Id);

                BuildingViewModel buildingVM = _mapper.Map<BuildingViewModel>(building);
                if (buildingVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<Building>(model);
                var result = await _unitOfWork.Buildings.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}