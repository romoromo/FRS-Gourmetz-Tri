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
using System.Diagnostics;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class PlaylistController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public PlaylistController(IUnitOfWork unitOfWork, ILogger<PlaylistController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet("id/{id}")]
        //[Authorize(Authorization.Policies.ViewAllLocationsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<PlaylistViewModel>))]
        public async Task<IActionResult> GetLocationById(int id)
        {
            var playlist = await _unitOfWork.Playlists.GetByIdAsync(id);
            return Ok(Mapper.Map<PlaylistViewModel>(playlist));
        }

        [HttpGet("playlists/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PlaylistViewModel>))]
        public async Task<IActionResult> GetPlaylists(int? institutionId = null, int? userId = null, List<int> imageIds = null)
        {
            return await GetPlaylists( - 1, -1, institutionId, userId, imageIds);
        }


        [HttpGet("playlists/list/{pageNumber:int}/{pageSize:int}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PlaylistViewModel>))]
        public async Task<IActionResult> GetPlaylists(int pageNumber, int pageSize, int? institutionId = null, int? userId = null, List<int> imageIds = null)
        {
            var results = await _unitOfWork.Playlists.GetPlaylistsLoadRelatedAsync(pageNumber, pageSize, institutionId, userId, imageIds);
            return Ok(Mapper.Map<List<PlaylistViewModel>>(results));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllContactGroupsPolicy)]
        [ProducesResponseType(201, Type = typeof(PlaylistViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePlaylist([FromBody] PlaylistViewModel playlist)
        {
            if (ModelState.IsValid)
            {
                if (playlist == null)
                    return BadRequest($"{nameof(playlist)} cannot be null");


                var type = Mapper.Map<Playlist>(playlist);

                var result = await _unitOfWork.Playlists.CreateAsync(type);
                if (result.IsSuccess && result.Data != null)
                {
                    var Id = result.Data.GetType().GetProperty("Id").GetValue(result.Data, null);
                    //ContactGroupViewModel contactGroupVM = Mapper.Map<ContactGroupViewModel>(result.Data);
                    return CreatedAtAction("GetPlaylistById", new { id = Id }, playlist);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(PlaylistViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            //var testDeleteResult = await _unitOfWork.Playlists.TestCanDeleteAsync(id);
            //if (!testDeleteResult.IsDeletable)
            //    return BadRequest(string.Format("Contact Group cannot be deleted. {0}", testDeleteResult.Message));


            var playlist = await this._unitOfWork.Playlists.GetByIdAsync(id);

            PlaylistViewModel playlistVM = Mapper.Map<PlaylistViewModel>(playlist);
            if (playlistVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Playlists.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting playlist: " + string.Join(", ", result.Message));


            return Ok(playlistVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllContactGroupsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePlaylist(string id, [FromBody] PlaylistViewModel model)
        {
            Debug.WriteLine("Model before: ", Newtonsoft.Json.JsonConvert.SerializeObject(model));
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var playlist = await this._unitOfWork.Playlists.GetByIdAsync(model.Id);

                PlaylistViewModel playlistVM = Mapper.Map<PlaylistViewModel>(playlist);
                if (playlistVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<Playlist>(model);
                Debug.WriteLine("Model to update: ", Newtonsoft.Json.JsonConvert.SerializeObject(updatedModel));
                var result = await _unitOfWork.Playlists.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
        
    }
}