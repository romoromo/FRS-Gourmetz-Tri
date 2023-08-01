using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
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
    public class UserGroupController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly string[] ACCEPTED_FILE_TYPES = new[] { ".jpg", ".jpeg", ".png" };

        public UserGroupController(IUnitOfWork unitOfWork, ILogger<UserGroupController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("usergroups/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUserGroups(BaseFilter filter)
        {
            var userGroups = await _unitOfWork.UserGroups.GetUserGroupsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<UserGroupViewModel>>(userGroups));
        }

        #endregion

        /// <summary>
        /// API calls to get userGroups
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns>List of userGroups</returns>
        [HttpGet("get/{userGroupId:int?}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiUserGroups(int? userGroupId = null, int? institutionId = null)
        {
            var result = await _unitOfWork.UserGroups.GetApiUserGroups(userGroupId, institutionId);
            var data = Mapper.Map<List<UserGroupViewModel>>(result.Data);

            if (userGroupId.HasValue)
            {
                result.Data = data.Any() ? data.First() : null;
            }
            else
            {
                result.Data = data;
            }

            return Ok(result);
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllUserGroupsPolicy)]
        [ProducesResponseType(201, Type = typeof(UserGroupViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUserGroup([FromBody] UserGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                var userGroup = Mapper.Map<UserGroup>(model);

                var result = await _unitOfWork.UserGroups.CreateAsync(userGroup);
                if (result.IsSuccess)
                {
                    UserGroupViewModel userGroupVM = Mapper.Map<UserGroupViewModel>(result.Data);
                    return CreatedAtAction("GetUserGroupById", new { id = userGroupVM.Id }, userGroupVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllUserGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(UserGroupViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUserGroup(int id)
        {
            if (!await _unitOfWork.UserGroups.TestCanDeleteAsync(id))
                return BadRequest("UserGroup cannot be deleted. Deselect this userGroup from all locations and try again");


            var userGroupType = await this._unitOfWork.UserGroups.GetByIdAsync(id);

            UserGroupViewModel userGroupVM = Mapper.Map<UserGroupViewModel>(userGroupType);
            if (userGroupVM == null)
                return NotFound(id);

            var result = await _unitOfWork.UserGroups.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting userGroup: " + string.Join(", ", result.Message));


            return Ok(userGroupVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllUserGroupsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserGroup(string id, [FromBody] UserGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var userGroupType = await this._unitOfWork.UserGroups.GetByIdAsync(model.Id);

                UserGroupViewModel userGroupVM = Mapper.Map<UserGroupViewModel>(userGroupType);
                if (userGroupVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<UserGroup>(model);
                var result = await _unitOfWork.UserGroups.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}