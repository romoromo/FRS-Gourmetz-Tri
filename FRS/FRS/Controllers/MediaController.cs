using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Models;
using FRS.Helpers;
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
    public class MediaController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public MediaController(IUnitOfWork unitOfWork, ILogger<MediaController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// API calls to get medias
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns>List of medias</returns>
        [HttpGet("get/{mediaId:int?}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiMedias(int? mediaId = null)
        {
            var result = await _unitOfWork.Medias.GetApiMedias(mediaId).ConfigureAwait(false);
            var data = Mapper.Map<List<MediaViewModel>>(result.Data);

            data = await Map(data);
            if (mediaId.HasValue)
            {
                result.Data = data.Any() ? data.First() : null;
            }
            else
            {
                result.Data = data;
            }

            return Ok(result);
        }

        [HttpGet("medias/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllMediaTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<MediaViewModel>))]
        public async Task<IActionResult> GetMedias()
        {
            return await GetMedias(-1, -1);
        }


        [HttpGet("medias/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllMediaTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<MediaViewModel>))]
        public async Task<IActionResult> GetMedias(int pageNumber, int pageSize)
        {
            var results = await _unitOfWork.Medias.GetMediasLoadRelatedAsync(pageNumber, pageSize).ConfigureAwait(false);
            var data = Mapper.Map<List<MediaViewModel>>(results);
            data = await Map(data);
            return Ok(data);
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllMediasPolicy)]
        [ProducesResponseType(201, Type = typeof(MediaViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMedia([FromBody] MediaViewModel media)
        {
            if (ModelState.IsValid)
            {
                if (media == null)
                    return BadRequest($"{nameof(media)} cannot be null");

                if (!await _unitOfWork.Medias.TestCanCreateAsync(media.Name))
                    return BadRequest("Name already exists.");

                var type = Mapper.Map<Media>(media);

                var result = await _unitOfWork.Medias.CreateAsync(type, media.RolesArr, media.UserGroupsArr);
                if (result.IsSuccess)
                {
                    //create sub folder
                    var appsetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("PATH_BASE_MEDIA_FOLDER_DIRECTORY").ConfigureAwait(false);
                    if (appsetting != null)
                    {
                        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), appsetting.Value, type.Name.ToString());
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        else
                        {
                            //TODO: rename folder or delete contents?
                            try
                            {
                                Directory.Delete(folderPath, true);
                                Directory.CreateDirectory(folderPath);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    MediaViewModel mediaVM = Mapper.Map<MediaViewModel>(result.Data);
                    return CreatedAtAction("GetMediaById", new { id = mediaVM.Id }, mediaVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMediasPolicy)]
        [ProducesResponseType(200, Type = typeof(MediaViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            //if (!await _unitOfWork.Medias.TestCanDeleteAsync(id))
            //    return BadRequest("Media cannot be deleted."); //TODO: correct message here


            var media = await this._unitOfWork.Medias.GetByIdAsync(id);

            MediaViewModel mediaVM = Mapper.Map<MediaViewModel>(media);
            if (mediaVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Medias.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting media: " + string.Join(", ", result.Message));

            try
            {
                //create sub folder
                var appsetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("PATH_BASE_MEDIA_FOLDER_DIRECTORY").ConfigureAwait(false);
                if (appsetting != null)
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), appsetting.Value, mediaVM.Name.ToString());
                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);
                    }
                }
            }
            catch (Exception)
            {
            }

            return Ok(mediaVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllMediasPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMedia(string id, [FromBody] MediaViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var media = await this._unitOfWork.Medias.GetByIdAsync(model.Id).ConfigureAwait(false);

                MediaViewModel mediaVM = Mapper.Map<MediaViewModel>(media);
                if (mediaVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<Media>(model);
                var result = await _unitOfWork.Medias.UpdateAsync(updatedModel, model.RolesArr, model.UserGroupsArr);
                if (result.IsSuccess)
                {
                    //create sub folder
                    var appsetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("PATH_BASE_MEDIA_FOLDER_DIRECTORY").ConfigureAwait(false);
                    if (appsetting != null)
                    {
                        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), appsetting.Value, updatedModel.Name.ToString());
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                    }

                    return NoContent();
                }

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        private async Task<List<MediaViewModel>> Map(IEnumerable<MediaViewModel> medias)
        {
            var appsetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("PATH_BASE_MEDIA_FOLDER_DIRECTORY").ConfigureAwait(false);
            string folderPath = string.Empty;
            var list = new List<MediaViewModel>();
            foreach (var media in medias)
            {
                if (appsetting != null)
                {
                    folderPath = Path.Combine(Directory.GetCurrentDirectory(), appsetting.Value, media.Name.ToString());
                }

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var dirInfo = new DirectoryInfo(folderPath);
                media.DirectorySize = Utilities.GetDirectorySize(dirInfo, true);
                media.NumberOfFiles = dirInfo.GetFiles().Length;
                list.Add(media);
            }

            return list;
        }
    }
}