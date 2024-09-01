using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Core.Interfaces;
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
    public class ContactGroupController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IAccountManager _accountManager;
        private readonly IMapper _mapper;

        public ContactGroupController(IUnitOfWork unitOfWork, ILogger<ContactGroupController> logger, IAccountManager accountManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _accountManager = accountManager;
            _mapper = mapper;
        }

        [HttpGet("get/id/{id}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(ContactGroupViewModel))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> ApiGetUserById(int id)
        {
            var results = await _unitOfWork.ContactGroups.GetByIdAsync(id, false);
            return Ok(_mapper.Map<ContactGroupViewModel>(results));
        }

        [HttpGet("contactgroups/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ContactGroupViewModel>))]
        public async Task<IActionResult> GetContactGroups(int? institutionId = null, int? userId = null, List<int> departmentIds = null, bool isSimple = false)
        {
            return await GetContactGroups(-1, -1, institutionId, userId, departmentIds, isSimple);
        }


        [HttpGet("contactgroups/list/{pageNumber:int}/{pageSize:int}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ContactGroupViewModel>))]
        public async Task<IActionResult> GetContactGroups(int pageNumber, int pageSize, int? institutionId = null, int? userId = null, List<int> departmentIds = null, bool isSimple = false)
        {
            var results = await _unitOfWork.ContactGroups.GetContactGroupsLoadRelatedAsync(pageNumber, pageSize, institutionId, userId, departmentIds, isSimple);
            if (isSimple)
            {
                return Ok(_mapper.Map<List<ContactGroupSimpleViewModel>>(results));
            }
            return Ok(_mapper.Map<List<ContactGroupViewModel>>(results));
        }

        [HttpGet("contactgroupmembers/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ContactGroupViewModel>))]
        public async Task<IActionResult> GetContactGroupMembers(int? userId = null)
        {
            return await GetContactGroupMembers(-1, -1, userId);
        }

        [HttpGet("contactgroupmembers/list/{pageNumber:int}/{pageSize:int}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ContactGroupViewModel>))]
        public async Task<IActionResult> GetContactGroupMembers(int pageNumber, int pageSize, int? userId = null)
        {
            var results = await _unitOfWork.ContactGroups.GetContactGroupMembersAsync(pageNumber, pageSize, userId);
            return Ok(_mapper.Map<List<ContactGroupMemberViewModel>>(results));
        }

        [HttpPost("")]
        [Authorize(Authorization.Policies.ManageAllContactGroupsPolicy)]
        [ProducesResponseType(201, Type = typeof(ContactGroupViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateContactGroup([FromBody] ContactGroupViewModel contactGroup)
        {
            if (ModelState.IsValid)
            {
                if (contactGroup == null)
                    return BadRequest($"{nameof(contactGroup)} cannot be null");


                var cg = _mapper.Map<ContactGroup>(contactGroup);


                var result = await _unitOfWork.ContactGroups.CreateAsync(cg);
                if (result.IsSuccess && result.Data != null)
                {
                    var Id = result.Data.GetType().GetProperty("Id").GetValue(result.Data, null);
                    //ContactGroupViewModel contactGroupVM = _mapper.Map<ContactGroupViewModel>(result.Data);
                    return CreatedAtAction("GetContactGroupById", new { id = Id }, contactGroup);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Authorization.Policies.ManageAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(ContactGroupViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteContactGroup(int id)
        {
            var testDeleteResult = await _unitOfWork.ContactGroups.TestCanDeleteAsync(id);
            if (!testDeleteResult.IsDeletable)
                return BadRequest(string.Format("Contact Group cannot be deleted. {0}", testDeleteResult.Message));


            var contactGroup = await this._unitOfWork.ContactGroups.GetByIdAsync(id);

            ContactGroupViewModel contactGroupVM = _mapper.Map<ContactGroupViewModel>(contactGroup);
            if (contactGroupVM == null)
                return NotFound(id);

            var result = await _unitOfWork.ContactGroups.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting contact group: " + string.Join(", ", result.Message));


            return Ok(contactGroupVM);
        }

        [HttpPut("update/{id}")]
        [Authorize(Authorization.Policies.ManageAllContactGroupsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateContactGroup(string id, [FromBody] ContactGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var contactGroup = await this._unitOfWork.ContactGroups.GetByIdAsync(model.Id);

                ContactGroupViewModel contactGroupVM = _mapper.Map<ContactGroupViewModel>(contactGroup);
                if (contactGroupVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<ContactGroup>(model);
                var result = await _unitOfWork.ContactGroups.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("ldap/sync")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> SyncLdap()
        {
            var response = await _accountManager.SyncLdap();
            return Ok(response);
        }
    }
}