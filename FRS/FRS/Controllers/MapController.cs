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
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class MapController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public MapController(IUnitOfWork unitOfWork, ILogger<MapController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("floorconnector")]
        //[Authorize(Authorization.Policies.ManageAllMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PointViewModel>))]
        public async Task<IActionResult> GetFloorConnectors(int? mapId = null)
        {
            var result = await _unitOfWork.Maps.GetFloorConnectors(mapId);
            var rmap = _mapper.Map<List<PointViewModel>>(result);
            return Ok(rmap);
        }

        [HttpGet("getpointbyid")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(PointViewModel))]
        public async Task<IActionResult> GetPointById(int id)
        {
            var result = _mapper.Map<PointViewModel>(_unitOfWork.Maps.GetPointById(id));

            return Ok(result);
        }

        [HttpGet("getmapbyid")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(MapViewModel))]
        public async Task<IActionResult> GetMapById(int id)
        {
            var result = _mapper.Map<MapViewModel>(await _unitOfWork.Maps.GetByIdAsync(id));

            if (result != null && !string.IsNullOrWhiteSpace(result.map_url))
            {
                var file = Path.Combine(Directory.GetCurrentDirectory(), result.map_url);
                var img = Image.FromFile(file);
                result.width = img.Width;
                result.height = img.Height;
            }

            return Ok(result);
        }

        [HttpGet("getroute")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PointViewModel>))]
        public async Task<IActionResult> GetRoute(int startDirectoryId, int destDirectoryId, bool wheelchair, bool sheltered)
        {
            var result = await _unitOfWork.Maps.GetRoute(startDirectoryId, destDirectoryId, wheelchair, sheltered);

            var pointMap = new Dictionary<int, DAL.Models.Point>();

            if (result.Data != null)
            {
                foreach (var p in (List<DAL.Models.Point>)result.Data)
                {
                    pointMap.Add(p.Id, p);
                }
            }

            result.Data = _mapper.Map<List<PointViewModel>>(result.Data);

            if (result.Data != null)
            {
                foreach (var p in (List<PointViewModel>)result.Data)
                {
                    if (pointMap[p.Id]?.Map != null)
                    {
                        pointMap[p.Id].Map.Points = null;
                        pointMap[p.Id].Map.Lines = null;
                        p.MapInfo = _mapper.Map<MapViewModel>(pointMap[p.Id].Map);

                        p.MapInfo.FloorOrder = pointMap[p.Id]?.Map?.Floor?.Buildings?.FirstOrDefault(b => b.Building != null && b.IsActive)?.order;

                        if (!string.IsNullOrWhiteSpace(p.MapInfo.map_url))
                        {
                            var file = Path.Combine(Directory.GetCurrentDirectory(), p.MapInfo.map_url);
                            var img = Image.FromFile(file);
                            p.MapInfo.width = img.Width;
                            p.MapInfo.height = img.Height;
                        }
                    }
                }
            }

            return Ok(result);
        }

        [HttpGet("getroutetest")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PointViewModel>))]
        public async Task<IActionResult> GetRouteTest(int startDirectoryId, int destDirectoryId, bool wheelchair, bool sheltered)
        {
            var result = await _unitOfWork.Maps.GetRouteTest(startDirectoryId, destDirectoryId, wheelchair, sheltered);


            result.Data = _mapper.Map<List<PointViewModel>>(result.Data);

            

            return Ok(result);
        }


        [HttpGet("list")]
        //[Authorize(Authorization.Policies.ManageAllMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<MapViewModel>))]
        public async Task<IActionResult> GetMaps(int? institutionId = null)
        {
            return await GetMaps(-1, -1, institutionId);
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ManageAllMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<MapViewModel>))]
        public async Task<IActionResult> GetMaps(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.Maps.GetMapsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            var rmap = _mapper.Map<List<MapViewModel>>(result);

            foreach (var map in rmap)
            {
                foreach (var point in map.Points)
                {
                    if (point.IsFloorConnector)
                    {
                        var connectors = await _unitOfWork.Maps.GetConnectors(point.Id);
                        point.connectors = _mapper.Map<List<LineViewModel>>(connectors);

                        foreach (var con in point.connectors)
                        {
                            con.DisplayLabel = (point.Id == con.Point0Id) ? con.Label1 : con.Label0;
                        }
                    }
                }
            }

            return Ok(rmap);
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllMapsPolicy)]
        [ProducesResponseType(201, Type = typeof(MapViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMap([FromBody] MapViewModel map)
        {
            if (ModelState.IsValid)
            {
                if (map == null)
                    return BadRequest($"{nameof(map)} cannot be null");

                AddConnectorToLine(map);

                var type = _mapper.Map<Map>(map);
                var result = await _unitOfWork.Maps.CreateAsync(type);
                if (result.IsSuccess)
                {
                    MapViewModel mapVM = _mapper.Map<MapViewModel>(result.Data);
                    return CreatedAtAction("GetMapById", new { id = mapVM.Id }, mapVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(MapViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMap(int id)
        {
            var map = await this._unitOfWork.Maps.GetByIdAsync(id);

            MapViewModel mapVM = _mapper.Map<MapViewModel>(map);
            if (mapVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Maps.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(mapVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMapsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMap(string id, [FromBody] MapViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var map = await this._unitOfWork.Maps.GetByIdAsync(model.Id);

                MapViewModel mapVM = _mapper.Map<MapViewModel>(map);
                if (mapVM == null)
                    return NotFound(id);

                AddConnectorToLine(model);

                var updatedModel = _mapper.Map<Map>(model);

                var result = await _unitOfWork.Maps.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        void AddConnectorToLine(MapViewModel map)
        {
            map.Points?.ForEach(p =>
            {
                if (p.connectors != null && p.connectors.Count() > 0)
                {
                    map.Lines?.AddRange(p.connectors);
                }
            });
        }
    }
}