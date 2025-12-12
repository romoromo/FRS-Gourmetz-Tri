using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using BAL.Services.MealOrder;
using DAL;
using DAL.Core;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OpenIddict.Validation.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class DeliveryController : BaseController
    {
        private IDeliveryService _service;
        readonly ILogger _logger;
        private readonly IAccountManager _accountManager;
        private readonly IMapper _mapper;

        public DeliveryController(IDeliveryService service, ILogger<StaffController> logger, IAccountManager accountManager, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _accountManager = accountManager;
            _mapper = mapper;
        }

        #region CatererInfos

        [ApiKeyAuthorize]
        [HttpGet("catererinfos/simple/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllCatererInfosPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCatererInfosSimple(BaseFilter filter)
        {
            var results = await this._service.GetCatererInfosSimpleAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<CatererInfoSimpleDTO>>(results));
        }

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("catererinfos/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllCatererInfosPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCatererInfos(BaseFilter filter)
        {
            var results = await this._service.GetCatererInfosAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<CatererInfoDTO>>(results));
        }

        #endregion

        [HttpGet("catererinfos/get/id/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(CatererInfoDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCatererInfoByIdAsync(int id)
        {
            var dto = await this._service.GetCatererInfoByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("catererinfos")]
        //[Authorize(Authorization.Policies.ManageAllCatererInfosPolicy)]
        [ProducesResponseType(201, Type = typeof(CatererInfoDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCatererInfo([FromBody] CatererInfoSimpleDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateCatererInfoAsync(dto);
                if (result.IsSuccess)
                {
                    //CatererInfoDTO vm = _mapper.Map<CatererInfoDTO>(result.Data);
                    return CreatedAtAction("GetCatererInfoById", new { });
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("catererinfos/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllCatererInfosPolicy)]
        [ProducesResponseType(200, Type = typeof(CatererInfoDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCatererInfo(int id)
        {
            var dto = await this._service.GetCatererInfoSimpleByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteCatererInfoAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("catererinfos/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllCatererInfosPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCatererInfo(string id, [FromBody] CatererInfoSimpleDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetCatererInfoSimpleByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateCatererInfoAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("catererinfos/request/{catererId}/{outletId}/{status}/{isRsp}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RequestOutlet(int catererId, int outletId, string status, bool isRsp, int? outletProfileId)
        {
            return Ok(await this._service.RequestOutlet(catererId, outletId, status, isRsp, outletProfileId));
        }

        #endregion

        #region Outlets

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("outlets/simple/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllOutletsPolicy)]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutletsSimple(BaseFilter filter)
        {
            var results = await this._service.GetOutletsSimpleAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<OutletSimpleDTO>>(results));
        }

        #endregion

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("outlets/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllOutletsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutlets(BaseFilter filter)
        {
            var results = await this._service.GetOutletsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<OutletDTO>>(results));
        }

        #endregion

        [HttpGet("outlets/get/id/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(OutletDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOutletBy(int id)
        {
            var dto = await this._service.GetOutletByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpGet("outlets/simple/get/id/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(OutletSimpleDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOutletByIdSimple(int id)
        {
            var dto = await this._service.GetOutletByIdSimpleAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("outlets")]
        //[Authorize(Authorization.Policies.ManageAllOutletsPolicy)]
        [ProducesResponseType(201, Type = typeof(OutletDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOutlet([FromBody] OutletSimpleDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateOutletAsync(dto);
                if (result.IsSuccess)
                {
                    //OutletDTO vm = _mapper.Map<OutletDTO>(result.Data);
                    //return CreatedAtAction("GetOutletById", new { id = vm.Id }, vm);
                    return CreatedAtAction("GetOutletById", new { });
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("outlets/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOutletsPolicy)]
        [ProducesResponseType(200, Type = typeof(OutletDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteOutlet(int id)
        {
            var dto = await this._service.GetOutletByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteOutletAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("outlets/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOutletsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateOutlet(string id, [FromBody] OutletSimpleDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetOutletByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateOutletAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpPut("outlets/updatestores/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOutletsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStores(string id, [FromBody] OutletDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetOutletByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateStoresAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region OutletProfiles

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("outletprofiles/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllOutletProfilesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutletProfiles(BaseFilter filter)
        {
            var results = await this._service.GetOutletProfilesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<OutletProfileDTO>>(results));
        }

        #endregion

        [HttpGet("outletprofiles/get/id/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(OutletProfileDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOutletProfileBy(int id)
        {
            var dto = await this._service.GetOutletProfileByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("outletprofiles")]
        //[Authorize(Authorization.Policies.ManageAllOutletProfilesPolicy)]
        [ProducesResponseType(201, Type = typeof(OutletProfileDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOutletProfile([FromBody] OutletProfileDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateOutletProfileAsync(dto);
                if (result.IsSuccess)
                {
                    OutletProfileDTO vm = _mapper.Map<OutletProfileDTO>(result.Data);
                    return CreatedAtAction("GetOutletProfileById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("outletprofiles/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOutletProfilesPolicy)]
        [ProducesResponseType(200, Type = typeof(OutletProfileDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteOutletProfile(int id)
        {
            var dto = await this._service.GetOutletProfileByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            if (!await _service.TestDeleteOutletProfileAsync(id))
                return BadRequest("Outlet Profile cannot be deleted.");

            var result = await this._service.DeleteOutletProfileAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("outletprofiles/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOutletProfilesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateOutletProfile(string id, [FromBody] OutletProfileDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetOutletProfileByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateOutletProfileAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region BentoBoxTypes

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("bentoboxtypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllBentoBoxTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetBentoBoxTypes(BaseFilter filter)
        {
            var results = await this._service.GetBentoBoxTypesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<BentoBoxTypeDTO>>(results));
        }

        #endregion

        [HttpPost("bentoboxtypes")]
        //[Authorize(Authorization.Policies.ManageAllBentoBoxTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(BentoBoxTypeDTO))]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateBentoBoxType([FromBody] BentoBoxTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateBentoBoxTypeAsync(dto);
                if (result.IsSuccess)
                {
                    BentoBoxTypeDTO vm = _mapper.Map<BentoBoxTypeDTO>(result.Data);
                    return CreatedAtAction("GetBentoBoxTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("bentoboxtypes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllBentoBoxTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(BentoBoxTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> DeleteBentoBoxType(int id)
        {
            var dto = await this._service.GetBentoBoxTypeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteBentoBoxTypeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("bentoboxtypes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllBentoBoxTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> UpdateBentoBoxType(string id, [FromBody] BentoBoxTypeDetailsDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetBentoBoxTypeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateBentoBoxTypeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion


        #region CartonTypes

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("cartontypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllCartonTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCartonTypes(BaseFilter filter)
        {
            var results = await this._service.GetCartonTypesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<CartonTypeDTO>>(results));
        }

        #endregion

        [HttpPost("cartontypes")]
        //[Authorize(Authorization.Policies.ManageAllCartonTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(CartonTypeDTO))]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateCartonType([FromBody] CartonTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateCartonTypeAsync(dto);
                if (result.IsSuccess)
                {
                    CartonTypeDTO vm = _mapper.Map<CartonTypeDTO>(result.Data);
                    return CreatedAtAction("GetCartonTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("cartontypes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllCartonTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(CartonTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> DeleteCartonType(int id)
        {
            var dto = await this._service.GetCartonTypeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteCartonTypeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("cartontypes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllCartonTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> UpdateCartonType(string id, [FromBody] CartonTypeDetailsDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetCartonTypeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateCartonTypeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion


        #region DeliveryOrders

        #region Sieved
        [HttpGet("deliveryorders/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDeliveryOrdersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDeliveryOrders(BaseFilter filter)
        {
            var results = await this._service.GetDeliveryOrdersAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<DeliveryOrderDTO>>(results));
        }

        #endregion

        [HttpPost("deliveryorders")]
        //[Authorize(Authorization.Policies.ManageAllDeliveryOrdersPolicy)]
        [ProducesResponseType(201, Type = typeof(DeliveryOrderDTO))]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateDeliveryOrder([FromBody] DeliveryOrderDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateDeliveryOrderAsync(dto);
                if (result.IsSuccess)
                {
                    DeliveryOrderDTO vm = _mapper.Map<DeliveryOrderDTO>(result.Data);
                    return CreatedAtAction("GetDeliveryOrderById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("deliveryorders/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDeliveryOrdersPolicy)]
        [ProducesResponseType(200, Type = typeof(DeliveryOrderDTO))]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDeliveryOrder(int id)
        {
            var dto = await this._service.GetDeliveryOrderByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteDeliveryOrderAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("deliveryorders/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDeliveryOrdersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDeliveryOrder(string id, [FromBody] DeliveryOrderDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetDeliveryOrderByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateDeliveryOrderAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region DeliveryOrderNews

        #region Sieved
        [HttpGet("deliveryordernews/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDeliveryOrdersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDeliveryOrderNews(BaseFilter filter)
        {
            var results = await this._service.GetDeliveryOrderNewsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<DeliveryOrderNewDTO>>(results));
        }

        #endregion

        #region Sieved BentoUsage
        [HttpGet("bentousages/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDeliveryOrdersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetBentoUsages(BaseFilter filter)
        {
            var results = await this._service.GetBentoUsage(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<BentoUsageCountDTO>>(results));
        }

        #endregion

        [HttpPost("deliveryordernews")]
        //[Authorize(Authorization.Policies.ManageAllDeliveryOrdersPolicy)]
        [ProducesResponseType(201, Type = typeof(DeliveryOrderNewDTO))]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateDeliveryOrderNew([FromBody] DeliveryOrderNewDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateDeliveryOrderNewAsync(dto);
                if (result.IsSuccess)
                {
                    DeliveryOrderNewDTO vm = _mapper.Map<DeliveryOrderNewDTO>(result.Data);
                    return CreatedAtAction("GetDeliveryOrderByIdNew", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("deliveryordernews/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDeliveryOrdersPolicy)]
        [ProducesResponseType(200, Type = typeof(DeliveryOrderNewDTO))]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDeliveryOrderNew(int id)
        {
            var dto = await this._service.GetDeliveryOrderNewByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteDeliveryOrderNewAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("deliveryordernews/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDeliveryOrdersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDeliveryOrderNew(string id, [FromBody] DeliveryOrderNewDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetDeliveryOrderNewByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateDeliveryOrderNewAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpPut("deliveryordernews/loading")]
        //[Authorize(Authorization.Policies.ManageAllDeliveryOrdersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        [ProducesResponseType(404)]
        public async Task<IActionResult> LoadDeliveryOrderNew([FromBody] DeliveryOrderNewDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetDeliveryOrderNewByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(model.Id);

                var result = await this._service.LoadDeliveryOrderNewAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region StoreInventory

        #region Sieved
        [HttpGet("storeinventories/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDeliveryOrdersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStoreInventories(BaseFilter filter)
        {
            var results = await this._service.GetStoreInventoriesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<StoreInventoryDTO>>(results));
        }

        #endregion

        #region Sieved detail
        [HttpGet("storeinventorydetails/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDeliveryOrdersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStoreInventoryDetails(BaseFilter filter)
        {
            var results = await this._service.GetStoreInventoryDetailsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<StoreInventoryDetailDTO>>(results));
        }

        #endregion

        [HttpPost("storeinventories")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDeliveryOrdersPolicy)]
        [ProducesResponseType(201, Type = typeof(StoreInventoryDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateStoreInventory([FromBody] StoreInventoryDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateStoreInventoryAsync(dto);
                if (result.IsSuccess)
                {
                    StoreInventoryDTO vm = _mapper.Map<StoreInventoryDTO>(result.Data);
                    return CreatedAtAction("GetStoreInventoryById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("storeinventories/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDeliveryOrdersPolicy)]
        [ProducesResponseType(200, Type = typeof(StoreInventoryDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStoreInventories(int id)
        {
            var dto = await this._service.GetStoreInventoryByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteStoreInventoryAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("storeinventories/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDeliveryOrdersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStoreInventory(string id, [FromBody] StoreInventoryDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetStoreInventoryByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateStoreInventoryAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion


        #region TrackingStatuss

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("trackingstatuss/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllTrackingStatussPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTrackingStatuss(BaseFilter filter)
        {
            var results = await this._service.GetTrackingStatussAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<TrackingStatusDTO>>(results));
        }

        #endregion

        [HttpPost("trackingstatuss")]
        //[Authorize(Authorization.Policies.ManageAllTrackingStatussPolicy)]
        [ProducesResponseType(201, Type = typeof(TrackingStatusDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTrackingStatus([FromBody] TrackingStatusDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateTrackingStatusAsync(dto);
                if (result.IsSuccess)
                {
                    TrackingStatusDTO vm = _mapper.Map<TrackingStatusDTO>(result.Data);
                    return CreatedAtAction("GetTrackingStatusById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("trackingstatuss/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllTrackingStatussPolicy)]
        [ProducesResponseType(200, Type = typeof(TrackingStatusDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTrackingStatus(int id)
        {
            var dto = await this._service.GetTrackingStatusByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteTrackingStatusAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("trackingstatuss/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllTrackingStatussPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTrackingStatus(string id, [FromBody] TrackingStatusDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetTrackingStatusByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateTrackingStatusAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region StoreInfos

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("storeinfos/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStoreInfosPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStoreInfos(BaseFilter filter)
        {
            var results = await this._service.GetStoreInfosAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<StoreInfoDTO>>(results));
        }

        #endregion

        [HttpPost("storeinfos")]
        //[Authorize(Authorization.Policies.ManageAllStoreInfosPolicy)]
        [ProducesResponseType(201, Type = typeof(StoreInfoDTO))]
        //[AllowAnonymous]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateStoreInfo([FromBody] StoreInfoDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateStoreInfoAsync(dto);
                if (result.IsSuccess)
                {
                    StoreInfoDTO vm = _mapper.Map<StoreInfoDTO>(result.Data);
                    return CreatedAtAction("GetStoreInfoById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("storeinfos/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllStoreInfosPolicy)]
        [ProducesResponseType(200, Type = typeof(StoreInfoDTO))]
        //[AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStoreInfo(int id)
        {
            var dto = await this._service.GetStoreInfoByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteStoreInfoAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("storeinfos/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllStoreInfosPolicy)]
        [ProducesResponseType(204)]
        //[AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStoreInfo(string id, [FromBody] StoreInfoDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetStoreInfoByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateStoreInfoAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion


        #region BentoAssets

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("bentoassets/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllBentoAssetsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetBentoAssets(BentoAssetsFilter filter)
        {
            var results = await this._service.GetBentoAssetsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<BentoAssetDTO>>(results));
        }

        #endregion

        [HttpPost("bentoassets")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBentoAssetsPolicy)]
        [ProducesResponseType(201, Type = typeof(BentoAssetDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateBentoAsset([FromBody] BentoAssetDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateBentoAssetAsync(dto);
                if (result.IsSuccess)
                {
                    BentoAssetDTO vm = _mapper.Map<BentoAssetDTO>(result.Data);
                    return CreatedAtAction("GetBentoAssetById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("bentoassets/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBentoAssetsPolicy)]
        [ProducesResponseType(200, Type = typeof(BentoAssetDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteBentoAsset(int id)
        {
            var dto = await this._service.GetBentoAssetByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteBentoAssetAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("bentoassets/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBentoAssetsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateBentoAsset(string id, [FromBody] BentoAssetDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetBentoAssetByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateBentoAssetAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("bentoassets/resetV2")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBentoAssetsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResetBentoAsset()
        {
            var result = await this._service.ResetBentoAssetAsync();
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while resetting: " + string.Join(", ", result.Message));

            return Ok(result);
        }

        #endregion

        #region CartonAssets

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("cartonassets/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllCartonAssetsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCartonAssets(CartonAssetsFilter filter)
        {
            var results = await this._service.GetCartonAssetsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<CartonAssetDTO>>(results));
        }

        #endregion

        [HttpPost("cartonassets")]
        //[Authorize(Authorization.Policies.ManageAllCartonAssetsPolicy)]
        [ProducesResponseType(201, Type = typeof(CartonAssetDTO))]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateCartonAsset([FromBody] CartonAssetDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateCartonAssetAsync(dto);
                if (result.IsSuccess)
                {
                    CartonAssetDTO vm = _mapper.Map<CartonAssetDTO>(result.Data);
                    return CreatedAtAction("GetCartonAssetById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("cartonassets/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllCartonAssetsPolicy)]
        [ProducesResponseType(200, Type = typeof(CartonAssetDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> DeleteCartonAsset(int id)
        {
            var dto = await this._service.GetCartonAssetByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteCartonAssetAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("cartonassets/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllCartonAssetsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> UpdateCartonAsset(string id, [FromBody] CartonAssetDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetCartonAssetByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateCartonAssetAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("cartonassets/reset")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBentoAssetsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResetCartonAsset()
        {
            var result = await this._service.ResetCartonAssetAsync();
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while resetting: " + string.Join(", ", result.Message));

            return Ok(result);
        }

        [HttpPost("cartonassets/label")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateCartonAssetLabel([FromQuery] int id)
        {
            var pdf = await this._service.GenerateCartonAssetLabel(id);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_CartonAssetLabel.pdf";

            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: pdf,
                contentType: "application/vnd",
                fileDownloadName: reportName
            );
        }

        #endregion

        #region CartonDisposableBox

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("disposableboxes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllBentoAssetsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDisposableBoxes(BaseFilter filter)
        {
            var results = await this._service.GetDisposableBoxesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<CartonDisposableBoxDTO>>(results));
        }

        #endregion

        [HttpPost("disposableboxes")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBentoAssetsPolicy)]
        [ProducesResponseType(201, Type = typeof(CartonDisposableBoxDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDisposableBox([FromBody] CartonDisposableBoxDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");

                if (dto.reset)
                {
                    var result2 = await this._service.ResetDishDisposableBoxAsync(dto);

                    if (result2.IsSuccess || !result2.IsSuccess)
                    {
                        var result = await this._service.CreateDisposableBoxAsync(dto);
                        if (result.IsSuccess)
                        {
                            CartonDisposableBoxDTO vm = _mapper.Map<CartonDisposableBoxDTO>(result.Data);
                            return CreatedAtAction("GetDisposableBoxById", new { id = vm.Id }, vm);
                        }

                        AddErrors(new string[] { result.Message });

                    }
                }
                else
                {
                    var result = await this._service.CreateDisposableBoxAsync(dto);
                    if (result.IsSuccess)
                    {
                        CartonDisposableBoxDTO vm = _mapper.Map<CartonDisposableBoxDTO>(result.Data);
                        return CreatedAtAction("GetDisposableBoxById", new { id = vm.Id }, vm);
                    }

                    AddErrors(new string[] { result.Message });
                }

            }

            return BadRequest(ModelState);
        }


        [HttpDelete("disposableboxes/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBentoAssetsPolicy)]
        [ProducesResponseType(200, Type = typeof(CartonDisposableBoxDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDisposableBox(int id)
        {
            var dto = await this._service.GetDisposableBoxByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteDisposableBoxAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("disposableboxes/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBentoAssetsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDisposableBox(string id, [FromBody] CartonDisposableBoxDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetDisposableBoxByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateDisposableAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("disposableboxes/reset")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllBentoAssetsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResetDisposableBox()
        {
            var result = await this._service.ResetDisposableBoxAsync();
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while resetting: " + string.Join(", ", result.Message));

            return Ok(result);
        }

        #endregion

        #region Drivers

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("drivers/sieve/list")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDrivers(BaseFilter filter)
        {
            var results = await this._service.GetDriversAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<DriverDTO>>(results));
        }

        #endregion

        [HttpPost("drivers")]
        [ProducesResponseType(201, Type = typeof(DriverDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDriver([FromBody] DriverDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateDriverAsync(dto);
                if (result.IsSuccess)
                {
                    DriverDTO vm = _mapper.Map<DriverDTO>(result.Data);
                    return CreatedAtAction("GetDriverById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("drivers/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(DriverDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var dto = await this._service.GetDriverByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteDriverAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("drivers/update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDriver(string id, [FromBody] DriverDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetDriverByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateDriverAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Routes

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("routes/sieve/list")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRoutes(BaseFilter filter)
        {
            var results = await this._service.GetRoutesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<RouteDTO>>(results));
        }

        #endregion

        [HttpPost("routes")]
        [ProducesResponseType(201, Type = typeof(RouteDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRoute([FromBody] RouteDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateRouteAsync(dto);
                if (result.IsSuccess)
                {
                    RouteDTO vm = _mapper.Map<RouteDTO>(result.Data);
                    return CreatedAtAction("GetRouteById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("routes/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(RouteDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var dto = await this._service.GetRouteByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteRouteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("routes/update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRoute(string id, [FromBody] RouteDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetRouteByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateRouteAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region PrintDo
        [HttpGet("printDo/{id}")]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> PrintDO(int id)
        {
            var dto = await this._service.GetDeliveryOrderNewByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var pdf = await this._service.GeneratePrintDo(id);
            var reportName = dto.DONumber + "_Print.pdf";

            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: pdf,
                contentType: "application/vnd",
                fileDownloadName: reportName
            );
        }

        [HttpGet("printCCLabel/{id}")]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> PrintCCLabel(int id)
        {
            var dto = await this._service.GetCartonAssetByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var pdf = await this._service.GeneratePrintCartonLabel(id);
            var reportName = dto.Code + "_Label.pdf";

            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: pdf,
                contentType: "application/vnd",
                fileDownloadName: reportName
            );
        }

        [HttpGet("printCCLabelV2/{cartonId}/{routeId}")]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> PrintCCLabelV2(int cartonId, int routeId)
        {
            var dto = await this._service.GetCartonAssetByIdAsync(cartonId);
            if (dto == null)
                return NotFound(cartonId);

            var dto2 = await this._service.GetRouteByIdAsync(routeId);
            if (dto2 == null)
                return NotFound(routeId);

            var pdf = await this._service.GeneratePrintCartonLabelV2(cartonId, routeId);
            var reportName = dto.Code + "_Label.pdf";

            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: pdf,
                contentType: "application/vnd",
                fileDownloadName: reportName
            );
        }
        #endregion

        #region Outlet Terns

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("outletterms/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllOutletsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutletTerms(BaseFilter filter)
        {
            var results = await this._service.GetOutletTermsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<OutletTermDTO>>(results));
        }

        #endregion

        [HttpPost("outletterms")]
        //[Authorize(Authorization.Policies.ManageAllOutletsPolicy)]
        [ProducesResponseType(201, Type = typeof(OutletTermDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOutletTerm([FromBody] OutletTermDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateOutletTermAsync(dto);
                if (result.IsSuccess)
                {
                    OutletTermDTO vm = _mapper.Map<OutletTermDTO>(result.Data);
                    return CreatedAtAction("GetOutletTermById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("outletterms/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOutletsPolicy)]
        [ProducesResponseType(200, Type = typeof(OutletDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteOutletTerm(int id)
        {
            var dto = await this._service.GetOutletTermByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteOutletTermAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("outletterms/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllOutletsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateOutletTerm(string id, [FromBody] OutletTermDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetOutletTermByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateOutletTermAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion


        #region Caterer Asset

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("catererasset/sieve/list")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCatererAssets(CatererAsserFilter filter)
        {
            var results = await this._service.GetCatererAssetsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<CatererAssetDTO>>(results));
        }

        #endregion

        [HttpPost("catererasset")]
        [ProducesResponseType(201, Type = typeof(CatererAssetDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCatererAsset([FromBody] CatererAssetDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateCatererAssetAsync(dto);
                if (result.IsSuccess)
                {
                    CatererAssetDTO vm = _mapper.Map<CatererAssetDTO>(result.Data);
                    return CreatedAtAction("GetCatererAssetByIdAsync", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("catererasset/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(CatererAssetTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCatererAsset(int id)
        {
            var dto = await this._service.GetCatererAssetByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteCatererAssetAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("catererasset/update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCatererAsset(string id, [FromBody] CatererAssetDTO model)
        {
            if (ModelState.IsValid)
            {
                var dto = await this._service.GetCatererAssetByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateCatererAssetAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("catererasset/getQRCode/{catererId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAssetQRCode(int catererId)
        {
            var result = await _service.GetAssetQRCode(catererId);
            return Ok(new { result });
        }

        [HttpPost("catererasset/generateQRCode")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateQRCode(int id, int catererId)
        {
            var pdf = await this._service.GenerateAssetQRCode(id, catererId);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_CatererAsset.pdf";

            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("");
            }

            return File(fileContents: pdf, contentType: "application/vnd", fileDownloadName: reportName
            );
        }
        #endregion

        #region Asset Component

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("assetcmp/sieve/list")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Getassetcomponents(AssetComponentFilter filter)
        {
            var results = await this._service.GetAssetComponentAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<AssetComponentDTO>>(results));
        }

        #endregion

        [HttpPost("assetcmp")]
        [ProducesResponseType(201, Type = typeof(AssetComponentDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAssetComponent([FromBody] AssetComponentDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateAssetComponentAsync(dto);
                if (result.IsSuccess)
                {
                    AssetComponentDTO vm = _mapper.Map<AssetComponentDTO>(result.Data);
                    return CreatedAtAction("GetAssetComponentByIdAsync", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("assetcmp/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(AssetComponentDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAssetComponent(int id)
        {
            var dto = await this._service.GetAssetComponentByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteAssetComponentAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("assetcmp/update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAssetComponent(string id, [FromBody] AssetComponentDTO model)
        {
            if (ModelState.IsValid)
            {
                var dto = await this._service.GetAssetComponentByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateAssetComponentAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
        #endregion

        [ApiKeyAuthorize]
        [HttpGet("sortingarea/sieve/list")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetSortingAssets(BaseFilter filter)
        {
            var results = await this._service.GetSortingAreaAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<SortingAreaDTO>>(results));
        }

        [HttpPost("sortingarea")]
        [ProducesResponseType(201, Type = typeof(SortingAreaDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateSortingArea([FromBody] SortingAreaDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateSortingAreaAsync(dto);
                if (result.IsSuccess)
                {
                    SortingAreaDTO vm = _mapper.Map<SortingAreaDTO>(result.Data);
                    return CreatedAtAction("GetSortingAreaByIdAsync", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("sortingarea/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(SortingAreaDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSortingArea(int id)
        {
            var dto = await this._service.GetSortingAreaByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteSortingAreaAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("sortingarea/update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSortingArea(string id, [FromBody] SortingAreaDTO model)
        {
            if (ModelState.IsValid)
            {
                var dto = await this._service.GetSortingAreaByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateSortingAreaAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpPost("sortingarea/generateQRCode")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateSortingAreaQRCode(int id, int catererId)
        {
            var pdf = await this._service.GenerateSortingAreaQRCode(id, catererId);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_SortingArea.pdf";
            if (pdf == null || pdf.Length == 0)
            {
                return BadRequest("");
            }
            return File(fileContents: pdf, contentType: "application/vnd", fileDownloadName: reportName
            );
        }
    }
}