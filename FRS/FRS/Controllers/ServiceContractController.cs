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
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ServiceContractController : BaseController
    {
        private IServiceContractService _service;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public ServiceContractController(IServiceContractService service, ILogger<ServiceContractController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        #region Service Contracts

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("servicecontracts/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllServiceContractsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetServiceContracts(BaseFilter filter)
        {
            var results = await this._service.GetServiceContractsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<ServiceContractDTO>>(results));
        }

        #endregion

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllServiceContractsPolicy)]
        [ProducesResponseType(201, Type = typeof(ServiceContractDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateServiceContract([FromBody] ServiceContractDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateServiceContractAsync(dto);
                if (result.IsSuccess)
                {
                    ServiceContractDTO vm = _mapper.Map<ServiceContractDTO>(result.Data);
                    return CreatedAtAction("GetServiceContractById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllServiceContractsPolicy)]
        [ProducesResponseType(200, Type = typeof(ServiceContractDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteServiceContract(int id)
        {
            var dto = await this._service.GetServiceContractByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteServiceContractAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllServiceContractsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateServiceContract(string id, [FromBody] ServiceContractDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetServiceContractByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateServiceContractsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpPost("exportreport")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateReportXls([FromBody] ServiceContractFilter serviceContractFilter)
        {
            var xls = await _service.GenerateReportXls(serviceContractFilter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_ServiceContracts.xlsx";

            if (xls == null || xls.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: xls,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: reportName
            );
        }

        #endregion
    }
}