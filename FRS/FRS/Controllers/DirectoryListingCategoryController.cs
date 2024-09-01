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
using NPOI.SS.Formula.Functions;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class DirectoryListingCategoryController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public DirectoryListingCategoryController(IUnitOfWork unitOfWork, ILogger<DirectoryListingCategoryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
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
            return Ok(_mapper.Map<List<DirectoryListingCategoryViewModel>>(result));
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


                var type = _mapper.Map<DirectoryListingCategory>(directoryListingCategory);

                var result = await _unitOfWork.DirectoryListingCategorys.CreateAsync(type);
                if (result.IsSuccess)
                {
                    DirectoryListingCategoryViewModel directoryListingCategoryVM = _mapper.Map<DirectoryListingCategoryViewModel>(result.Data);
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

            DirectoryListingCategoryViewModel directoryListingCategoryVM = _mapper.Map<DirectoryListingCategoryViewModel>(directoryListingCategory);
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

                DirectoryListingCategoryViewModel directoryListingCategoryVM = _mapper.Map<DirectoryListingCategoryViewModel>(directoryListingCategory);
                if (directoryListingCategoryVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<DirectoryListingCategory>(model);
                var result = await _unitOfWork.DirectoryListingCategorys.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}