using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using DAL;
using DAL.Core;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.ViewModels;
using FRS.ViewModels.MealOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/email")]
    public class EmailApiController : BaseController
    {
        private IEmailQueueService _service;
        readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly IAccountManager _accountManager;
        private IApplicationSettingService _appSetting;
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public EmailApiController(IEmailQueueService service, ILogger<EmailApiController> logger, IEmailSender emailSender, IApplicationSettingService appSetting, IAccountManager accountManager,
            IStudentService studentService, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _emailSender = emailSender;
            _accountManager = accountManager;
            _appSetting = appSetting;
            _studentService = studentService;
            _mapper = mapper;
        }

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("contactussubjects/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllContactUsSubjectsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetContactUsSubjects(BaseFilter filter)
        {
            var results = await this._service.GetContactUsSubjectsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<ContactUsSubjectDTO>>(results));
        }

        #endregion

        [HttpPost("contactussubjects")]
        //[Authorize(Authorization.Policies.ManageAllContactUsSubjectsPolicy)]
        [ProducesResponseType(201, Type = typeof(ContactUsSubjectDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateContactUsSubject([FromBody] ContactUsSubjectDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateContactUsSubjectAsync(dto);
                if (result.IsSuccess)
                {
                    ContactUsSubjectDTO vm = _mapper.Map<ContactUsSubjectDTO>(result.Data);
                    return CreatedAtAction("GetContactUsSubjectById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("contactussubjects/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllContactUsSubjectsPolicy)]
        [ProducesResponseType(200, Type = typeof(ContactUsSubjectDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteContactUsSubject(int id)
        {
            var dto = await this._service.GetContactUsSubjectByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteContactUsSubjectAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("contactussubjects/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllContactUsSubjectsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateContactUsSubject(string id, [FromBody] ContactUsSubjectDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetContactUsSubjectByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateContactUsSubjectAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpPost("contactus/sendemail")]
        //[Authorize(Authorization.Policies.ManageAllInterestGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> SendTestEmail([FromBody] ContactUsEmailViewModel model)
        {
            var user = await _accountManager.GetUserByIdAsync(model.UserId);
            if(user != null)
            {
                var contactUsEmails = await _appSetting.GetApplicationSettingByKey("MOS_CONTACTUS_EMAIL");
                if (contactUsEmails != null && !string.IsNullOrEmpty(contactUsEmails.Value))
                {
                    var student = await _studentService.GetStudentByIdAsync(model.StudentId);
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Format("<b>FROM:</b> {0} ({1}) <br />", user.FriendlyName, user.Email));
                    sb.AppendLine(string.Format("<b>Full Name:</b> {0} <br />", student?.Name ?? student?.FullName));
                    sb.AppendLine(string.Format("<b>School Name:</b> {0} <br /><br />", student?.OutletName));
                    sb.AppendLine(string.Format("<b>Subject:</b> <br /><span>{0}</span><br />", model.Subject));
                    sb.AppendLine(string.Format("<b>DETAILS:</b> <br /><p>{0}</p>", model.Detail));
                    var emails = contactUsEmails.Value.Split(';').Select(e => e.Trim()).ToList();
                    foreach (var email in emails)
                    {
                        await _emailSender.SendEmailAsync(user.FriendlyName, email, "GOe Contact Us - " + model.Subject, sb.ToString());
                        //await _emailSender.SendEmailAsync(user.FriendlyName, user.Email, "GOe Contact Us", email, model.Subject, model.Detail);
                    }
                }

                return Ok();
            }

            return BadRequest();
        }

        #region Contact Us Details

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("contactusdetails/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllContactUsDetailsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetContactUsDetails(BaseFilter filter)
        {
            var results = await this._service.GetContactUsDetailsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<ContactUsDetailDTO>>(results));
        }

        #endregion

        [HttpPost("contactusdetails")]
        //[Authorize(Authorization.Policies.ManageAllContactUsDetailsPolicy)]
        [ProducesResponseType(201, Type = typeof(ContactUsDetailDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateContactUsDetail([FromBody] ContactUsDetailDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateContactUsDetailAsync(dto);
                if (result.IsSuccess)
                {
                    ContactUsDetailDTO vm = _mapper.Map<ContactUsDetailDTO>(result.Data);
                    return CreatedAtAction("GetContactUsDetailById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("contactusdetails/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllContactUsDetailsPolicy)]
        [ProducesResponseType(200, Type = typeof(ContactUsDetailDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteContactUsDetail(int id)
        {
            var dto = await this._service.GetContactUsDetailByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteContactUsDetailAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("contactusdetails/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllContactUsDetailsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateContactUsDetail(string id, [FromBody] ContactUsDetailDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetContactUsDetailByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateContactUsDetailAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
        #endregion

        #region Email Templates
        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("emailtemplates/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllEmailTemplatesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetEmailTemplates(BaseFilter filter)
        {
            var results = await this._service.GetEmailTemplatesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<EmailTemplateDTO>>(results));
        }

        #endregion

        [HttpPost("emailtemplates")]
        //[Authorize(Authorization.Policies.ManageAllEmailTemplatesPolicy)]
        [ProducesResponseType(201, Type = typeof(EmailTemplateDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEmailTemplate([FromBody] EmailTemplateDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateEmailTemplateAsync(dto);
                if (result.IsSuccess)
                {
                    EmailTemplateDTO vm = _mapper.Map<EmailTemplateDTO>(result.Data);
                    return CreatedAtAction("GetEmailTemplateById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("emailtemplates/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmailTemplatesPolicy)]
        [ProducesResponseType(200, Type = typeof(EmailTemplateDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEmailTemplate(int id)
        {
            var dto = await this._service.GetEmailTemplateByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteEmailTemplateAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("emailtemplates/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmailTemplatesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEmailTemplate(string id, [FromBody] EmailTemplateDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetEmailTemplateByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateEmailTemplateAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("emailtemplates/sendtestemail")]
        //[Authorize(Authorization.Policies.ManageAllInterestGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SendTestEmail(int templateId, string email)
        {
            var template = await this._service.GetEmailTemplateByIdAsync(templateId);
            if (template == null)
                return NotFound(templateId);

            var result = await _emailSender.SendEmailAsync(template.FromName, template.FromEmail, email, email, template.Subject, template.Body);
            if (!result.success)
                throw new Exception("The following errors occurred while sending the email: " + string.Join(", ", result.errorMsg));

            return Ok(result);
        }
        #endregion

    }
}