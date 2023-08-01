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
    public class DirectoryListingCategoryController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public DirectoryListingCategoryController(IUnitOfWork unitOfWork, ILogger<DirectoryListingCategoryController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        [HttpGet("list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingCategorysPolicy)]
        [ProducesResponseType(200, Type = typeof(List<DirectoryListingCategoryViewModel>))]
        public async Task<IActionResult> GetDirectoryListingCategorys(int? institutionId = null)
        {
            return await GetDirectoryListingCategorys(-1, -1, institutionId);
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingCategorysPolicy)]
        [ProducesResponseType(200, Type = typeof(List<DirectoryListingCategoryViewModel>))]
        public async Task<IActionResult> GetDirectoryListingCategorys(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.DirectoryListingCategorys.GetDirectoryListingCategorysLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(Mapper.Map<List<DirectoryListingCategoryViewModel>>(result));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingCategorysPolicy)]
        [ProducesResponseType(201, Type = typeof(DirectoryListingCategoryViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDirectoryListingCategory([FromBody] DirectoryListingCategoryViewModel directoryListingCategory)
        {
            if (ModelState.IsValid)
            {
                if (directoryListingCategory == null)
                    return BadRequest($"{nameof(directoryListingCategory)} cannot be null");


                var type = Mapper.Map<DirectoryListingCategory>(directoryListingCategory);

                var result = await _unitOfWork.DirectoryListingCategorys.CreateAsync(type);
                if (result.IsSuccess)
                {
                    DirectoryListingCategoryViewModel directoryListingCategoryVM = Mapper.Map<DirectoryListingCategoryViewModel>(result.Data);
                    return CreatedAtAction("GetDirectoryListingCategoryById", new { id = directoryListingCategoryVM.Id }, directoryListingCategoryVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingCategorysPolicy)]
        [ProducesResponseType(200, Type = typeof(DirectoryListingCategoryViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDirectoryListingCategory(int id)
        {
            var directoryListingCategory = await this._unitOfWork.DirectoryListingCategorys.GetByIdAsync(id);

            DirectoryListingCategoryViewModel directoryListingCategoryVM = Mapper.Map<DirectoryListingCategoryViewModel>(directoryListingCategory);
            if (directoryListingCategoryVM == null)
                return NotFound(id);

            var result = await _unitOfWork.DirectoryListingCategorys.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(directoryListingCategoryVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingCategorysPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDirectoryListingCategory(string id, [FromBody] DirectoryListingCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var directoryListingCategory = await this._unitOfWork.DirectoryListingCategorys.GetByIdAsync(model.Id);

                DirectoryListingCategoryViewModel directoryListingCategoryVM = Mapper.Map<DirectoryListingCategoryViewModel>(directoryListingCategory);
                if (directoryListingCategoryVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<DirectoryListingCategory>(model);
                var result = await _unitOfWork.DirectoryListingCategorys.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}