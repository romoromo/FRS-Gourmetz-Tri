using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.Services.Interfaces;
using DAL;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
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
    public class EmailQueueController : BaseController
    {
        private IEmailQueueService _service;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public EmailQueueController(IEmailQueueService service, ILogger<EmailQueueController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("emailqueue/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllEmailQueuesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetEmailQueues(BaseFilter filter)
        {
            var results = await this._service.GetEmailQueuesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<EmailQueueDTO>>(results));
        }

        #endregion

        [HttpPost("emailqueue")]
        //[Authorize(Authorization.Policies.ManageAllEmailQueuesPolicy)]
        [ProducesResponseType(201, Type = typeof(EmailQueueDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEmailQueue([FromBody] EmailQueueDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateEmailQueueAsync(dto);
                if (result.IsSuccess)
                {
                    EmailQueueDTO vm = _mapper.Map<EmailQueueDTO>(result.Data);
                    return CreatedAtAction("GetEmailQueueById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("emailqueue/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmailQueuesPolicy)]
        [ProducesResponseType(200, Type = typeof(EmailQueueDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEmailQueue(int id)
        {
            var dto = await this._service.GetEmailQueueByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteEmailQueueAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("emailqueue/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEmailQueuesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEmailQueue(string id, [FromBody] EmailQueueDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetEmailQueueByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateEmailQueueAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
        
    }
}