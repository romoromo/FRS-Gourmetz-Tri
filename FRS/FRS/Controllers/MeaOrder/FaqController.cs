using System;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using DAL.Core;
using DAL.Filters;
using FRS.Attributes;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class FaqController : BaseController
    {
        private IManagementService _service;
        readonly ILogger _logger;


        public FaqController(IManagementService service, ILogger<FaqController> logger)
        {
            _service = service;
            _logger = logger;
        }

        #region Subjects

        #region Sieved
        [HttpGet("subjects/sieve/list")]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetFaqSubjects(BaseFilter filter)
        {
            var results = await this._service.GetFaqSubjectsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<FaqSubjectDTO>>(results));
        }

        #endregion

        [HttpPost("subjects")]
        [ProducesResponseType(201, Type = typeof(FaqSubjectDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateFaqSubject([FromBody] FaqSubjectDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateFaqSubjectAsync(dto);
                if (result.IsSuccess)
                {
                    FaqSubjectDTO vm = Mapper.Map<FaqSubjectDTO>(result.Data);
                    return CreatedAtAction("GetFaqSubjectById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("subjects/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(FaqSubjectDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFaqSubject(int id)
        {
            var dto = await this._service.GetFaqSubjectByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteFaqSubjectAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("subjects/update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateFaqSubject(string id, [FromBody] FaqSubjectDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetFaqSubjectByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateFaqSubjectAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("subjects/order/{id}")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> OrderFaqSubject(int id, bool isAsc)
        {
            var result = await this._service.OrderFaqSubjectAsync(id, isAsc);
            return Ok(result);
        }

        #endregion

        #region FAQ Details

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("details/sieve/list")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetFaqDetails(BaseFilter filter)
        {
            var results = await this._service.GetFaqDetailsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<FaqDetailDTO>>(results));
        }

        #endregion

        [HttpPost("details")]
        [ProducesResponseType(201, Type = typeof(FaqDetailDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateFaqDetail([FromBody] FaqDetailDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateFaqDetailAsync(dto);
                if (result.IsSuccess)
                {
                    FaqDetailDTO vm = Mapper.Map<FaqDetailDTO>(result.Data);
                    return CreatedAtAction("GetFaqDetailById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [HttpGet("details/order/{id}")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> OrderFaqDetail(int id, bool isAsc)
        {
            var result = await this._service.OrderFaqDetailAsync(id, isAsc);
            return Ok(result);
        }


        [HttpDelete("details/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(FaqDetailDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFaqDetail(int id)
        {
            var dto = await this._service.GetFaqDetailByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteFaqDetailAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("details/update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateFaqDetail(string id, [FromBody] FaqDetailDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetFaqDetailByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateFaqDetailAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
        #endregion
    }
}