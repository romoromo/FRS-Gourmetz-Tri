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
    public class SimpleResultController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;


        public SimpleResultController(IUnitOfWork unitOfWork, ILogger<SimpleResultController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region DEVICE MANAGEMENT USAGES

        [HttpGet("institutions/list")]
        //[Authorize(Authorization.Policies.ViewAllSignagePublicationsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<SimpleApiResult>))]
        public async Task<IActionResult> GetInstitutions(int? institutionId = null)
        {
            var results = await _unitOfWork.Institutions.GetInstitutionsLoadRelatedAsync(-1, -1);
            return Ok(Mapper.Map<List<SimpleApiResult>>(results));
        }

        [HttpGet("locations/list")]
        //[Authorize(Authorization.Policies.ViewAllSignagePublicationsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<SimpleApiResult>))]
        public async Task<IActionResult> GetLocations(int? institutionId = null)
        {
            var results = await _unitOfWork.Locations.GetLocationsLoadRelatedAsync(-1, -1, institutionId);
            return Ok(Mapper.Map<List<SimpleApiResult>>(results));
        }

        [HttpGet("signagepublications/list")]
        //[Authorize(Authorization.Policies.ViewAllSignagePublicationsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<SimpleApiResult>))]
        public async Task<IActionResult> GetSignagePublications(int? institutionId = null)
        {
            var data = await _unitOfWork.SignagePublications.GetSignagePublicationsLoadRelatedAsync(-1, -1, institutionId);
            return Ok(Mapper.Map<List<SimpleApiResult>>(data));
        }
        #endregion
    }
}