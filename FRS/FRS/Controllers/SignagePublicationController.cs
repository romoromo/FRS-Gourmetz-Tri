using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core.Interfaces;
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
    public class SignagePublicationController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IAccountManager _accountManager;
        private readonly IEmailSender _emailSender;


        public SignagePublicationController(IUnitOfWork unitOfWork, ILogger<SignagePublicationController> logger, IAccountManager accountManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _accountManager = accountManager;
            _emailSender = emailSender;
        }

        [HttpGet("signagepublication/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllSignagePublicationsPolicy)]
        [ProducesResponseType(200, Type = typeof(SignagePublicationViewModel))]
        public async Task<IActionResult> GetSignagePublication(int id)
        {
            return Ok(Mapper.Map<SignagePublicationViewModel>(await _unitOfWork.SignagePublications.GetByIdAsync(id)));
        }

        [HttpGet("signagepublications/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllSignagePublicationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<SignagePublicationViewModel>))]
        public async Task<IActionResult> GetSignagePublications(int? institutionId = null)
        {
            return await GetSignagePublications(-1, -1, institutionId);
        }


        [HttpGet("signagepublications/list/{pageNumber:int}/{pageSize:int}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<SignagePublicationViewModel>))]
        public async Task<IActionResult> GetSignagePublications(int pageNumber, int pageSize, int? institutionId = null)
        {
            var data = await _unitOfWork.SignagePublications.GetSignagePublicationsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(Mapper.Map<List<SignagePublicationViewModel>>(data));
        }

        [HttpGet("exportapprovedhistory")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllSignagePublicationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<SignagePublicationViewModel>))]
        public async Task<IActionResult> ExportSignagePublicationHistorys(int? publicationId = null)
        {
            if (publicationId == null) return null;

            var publication = await _unitOfWork.SignagePublications.GetByIdAsync(publicationId.Value);

            var bts = await _unitOfWork.SignagePublications.GetHistory(publicationId);

            return File(bts,
         "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
         DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_" + publication.Name + "_PublicationHistory.xls");
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllSignagePublicationsPolicy)]
        [ProducesResponseType(201, Type = typeof(SignagePublicationViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateSignagePublication([FromBody] SignagePublicationViewModel signagePublication)
        {
            if (ModelState.IsValid)
            {
                if (signagePublication == null)
                    return BadRequest($"{nameof(signagePublication)} cannot be null");


                var type = Mapper.Map<SignagePublication>(signagePublication);

                var result = await _unitOfWork.SignagePublications.CreateAsync(type);
                if (result.IsSuccess)
                {
                    SignagePublicationViewModel signagePublicationVM = Mapper.Map<SignagePublicationViewModel>(result.Data);

                    var users = await _accountManager.GetUserByClaimValueAsync("frsmgt.accesscontrol.publication.email");

                    foreach (var u in users)
                    {
                        try
                        {
                            var publicationNameKey = "{PublicationName}";

                            var title = $"New Publication With Name {publicationNameKey} Has Been Created.";
                            var content = $"New publication with name {publicationNameKey} has been created.\n" +
                                "Please open FRS to approve.";

                            try
                            {
                                var titleObj = await _unitOfWork.ApplicationSettings.GetByKeyAsync("NEW_PUBLICATION_EMAIL_TITLE").ConfigureAwait(false);
                                var contentObj = await _unitOfWork.ApplicationSettings.GetByKeyAsync("NEW_PUBLICATION_EMAIL_BODY").ConfigureAwait(false);

                                if(titleObj != null) title = titleObj.Value;
                                if (contentObj != null) content = contentObj.Value;
                            }
                            catch (Exception ex) { }

                            title = title.Replace(publicationNameKey, signagePublication.Name);
                            content = content.Replace(publicationNameKey, signagePublication.Name);

                            var emailBody = EmailTemplates.GetPlainTextTestEmail(DateTime.Now);
                            var isSuccess = await _emailSender.SendEmailAsync("Publication Approval Alert", u.Email, title, content);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    return CreatedAtAction("GetSignagePublicationById", new { id = signagePublicationVM.Id }, signagePublicationVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllSignagePublicationsPolicy)]
        [ProducesResponseType(200, Type = typeof(SignagePublicationViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSignagePublication(int id)
        {
            var signagePublication = await this._unitOfWork.SignagePublications.GetByIdAsync(id);

            SignagePublicationViewModel signagePublicationVM = Mapper.Map<SignagePublicationViewModel>(signagePublication);
            if (signagePublicationVM == null)
                return NotFound(id);

            var result = await _unitOfWork.SignagePublications.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting signage publication: " + string.Join(", ", result.Message));


            return Ok(signagePublicationVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllSignagePublicationsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSignagePublication(string id, [FromBody] SignagePublicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var signagePublication = await this._unitOfWork.SignagePublications.GetByIdAsync(model.Id);

                SignagePublicationViewModel signagePublicationVM = Mapper.Map<SignagePublicationViewModel>(signagePublication);
                if (signagePublicationVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<SignagePublication>(model);
                var result = await _unitOfWork.SignagePublications.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #region DEVICE MANAGEMENT USAGES

        [HttpGet("/device/signagepublications/list")]
        //[Authorize(Authorization.Policies.ViewAllSignagePublicationsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<SignagePublicationViewModel>))]
        public async Task<IActionResult> GetDeviceSignagePublications(int? institutionId = null)
        {
            var data = await _unitOfWork.SignagePublications.GetSignagePublicationsLoadRelatedAsync(-1, -1, institutionId);
            return Ok(Mapper.Map<List<SignagePublicationViewModel>>(data));
        }

        #endregion
    }
}