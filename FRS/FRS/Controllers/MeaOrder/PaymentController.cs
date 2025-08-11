using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;  
using BAL.Services.Interfaces.MealOrder;
using BAL.Services.MealOrder;
using DAL;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OpenIddict.Validation.AspNetCore;
using FRS.Controllers;
using MealOrderPayments.Controllers;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class PaymentController : BaseController
    {
        private IPaymentService _service;
        private IStudentService _studentService;
        private ITokenOrderService _tokenService;
        readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private OrderController _orderController;

        public PaymentController(IPaymentService service, ILogger<PaymentController> logger, ITokenOrderService tokenService, IEmailSender emailSender, IStudentService studentservice, IMapper mapper, OrderController orderController)
        {
            _service = service;
            _logger = logger;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _studentService = studentservice;
            _mapper = mapper;
            _orderController = orderController;
        }

        #region Payment Types

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("paymenttypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllPaymentTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetPaymentTypes(BaseFilter filter)
        {
            var results = await this._service.GetPaymentTypesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<PaymentTypeDTO>>(results));
        }

        #endregion

        [HttpPost("paymenttypes")]
        //[Authorize(Authorization.Policies.ManageAllPaymentTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(PaymentTypeDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePaymentType([FromBody] PaymentTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreatePaymentTypeAsync(dto);
                if (result.IsSuccess)
                {
                    PaymentTypeDTO vm = _mapper.Map<PaymentTypeDTO>(result.Data);
                    return CreatedAtAction("GetPaymentTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("paymenttypes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllPaymentTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(PaymentTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePaymentType(int id)
        {
            var dto = await this._service.GetPaymentTypeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeletePaymentTypeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("paymenttypes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllPaymentTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePaymentType(string id, [FromBody] PaymentTypeDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetPaymentTypeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdatePaymentTypeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Payment

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("payments/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllPaymentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetPayments(BaseFilter filter)
        {
            var results = await this._service.GetPaymentsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<PaymentDTO>>(results));
        }

        #endregion

        [HttpPost("payment")]
        //[Authorize(Authorization.Policies.ManageAllPaymentsPolicy)]
        [ProducesResponseType(201, Type = typeof(PaymentDTO))]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDTO dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (dto == null)
                        return BadRequest($"{nameof(dto)} cannot be null");

                    dto.InvoiceNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");

                    var tos = dto.TokenOrders;
                    dto.TokenOrders = null;

                    var mpos = dto.MealPlanOrders;
                    dto.MealPlanOrders = null;


                    var result = await this._service.CreatePaymentAsync(dto);
                    if (result.IsSuccess)
                    {
                        PaymentDTO vm = _mapper.Map<PaymentDTO>(result.Data);

                        if (tos != null)
                        {
                            foreach (var to in tos)
                            {

                                var dt = await this._tokenService.GetTokenOrderByIdAsync(to.Id);



                                if (dt != null)
                                {
                                    dt.PaymentId = vm.Id;
                                    if (vm.Status == "SUCCESS") dt.Status = dt.Status == "cancelled" ? dt.Status : "paid";
                                    var r = await this._tokenService.UpdateTokenOrderAsync(dt);
                                }
                            }
                        }

                        if(mpos != null)
                        {
                            foreach (var to in mpos)
                            {

                                var dt = await this._tokenService.GetMealPlanOrderByIdAsync(to.Id);



                                if (dt != null)
                                {
                                    dt.PaymentId = vm.Id;
                                    if (vm.Status == "SUCCESS") dt.Status = dt.Status == "cancelled" ? dt.Status : "paid";
                                    var r = await this._tokenService.UpdateMealPlanOrderAsync(dt);

                                    if (to.StudentGroupId.HasValue && to.ProfileId.HasValue) await _studentService.CreateOrUpdateStudentGroupDetailAsync(to.StudentGroupId.Value, to.ProfileId.Value, true);
                                }
                            }
                        }


                        vm.TokenOrders = tos;
                        vm.MealPlanOrders = mpos;


                        if (vm.Status == "SUCCESS" && vm.total == 0)
                        {
                            await _orderController.SendInvoice(vm);
                            vm.invoiceSent = true;

                            //vm.TokenOrders = new List<TokenOrderDTO>();
                            //vm.MealPlanOrders = new List<MealPlanOrderDTO>();

                            //await _service.UpdatePaymentAsync(vm);
                        }


                        return CreatedAtAction("GetPaymentById", new { id = vm.Id }, vm);
                    }

                    AddErrors(new string[] { result.Message + "- errorPayment" });

                    if(!result.IsSuccess) return BadRequest(result);

                    return Ok(result);
                }

                return BadRequest(ModelState);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error -- " + ex.ToString());
            }
        }


        [HttpDelete("payment/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllPaymentsPolicy)]
        [ProducesResponseType(200, Type = typeof(PaymentDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var dto = await this._service.GetPaymentByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeletePaymentAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("payment/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllPaymentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePayment(string id, [FromBody] PaymentDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetPaymentByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdatePaymentAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        
        #endregion

        #region Transaction Fees

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("transactionfees/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllTransactionFeesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTransactionFees(BaseFilter filter)
        {
            var results = await this._service.GetTransactionFeesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<TransactionFeeDTO>>(results));
        }

        #endregion

        [HttpPost("transactionfees")]
        //[Authorize(Authorization.Policies.ManageAllTransactionFeesPolicy)]
        [ProducesResponseType(201, Type = typeof(TransactionFeeDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTransactionFee([FromBody] TransactionFeeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateTransactionFeeAsync(dto);
                if (result.IsSuccess)
                {
                    TransactionFeeDTO vm = _mapper.Map<TransactionFeeDTO>(result.Data);
                    return CreatedAtAction("GetTransactionFeeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("transactionfees/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllTransactionFeesPolicy)]
        [ProducesResponseType(200, Type = typeof(TransactionFeeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTransactionFee(int id)
        {
            var dto = await this._service.GetTransactionFeeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteTransactionFeeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("transactionfees/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllTransactionFeesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTransactionFee(string id, [FromBody] TransactionFeeDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetTransactionFeeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateTransactionFeeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Voucher Types

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("vouchertypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllVoucherTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetVoucherTypes(BaseFilter filter)
        {
            var results = await this._service.GetVoucherTypesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<VoucherTypeDTO>>(results));
        }

        #endregion

        [HttpPost("vouchertypes")]
        //[Authorize(Authorization.Policies.ManageAllVoucherTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(VoucherTypeDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateVoucherType([FromBody] VoucherTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateVoucherTypeAsync(dto);
                if (result.IsSuccess)
                {
                    VoucherTypeDTO vm = _mapper.Map<VoucherTypeDTO>(result.Data);
                    return CreatedAtAction("GetVoucherTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("vouchertypes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllVoucherTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(VoucherTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteVoucherType(int id)
        {
            var dto = await this._service.GetVoucherTypeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteVoucherTypeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("vouchertypes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllVoucherTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateVoucherType(string id, [FromBody] VoucherTypeDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetVoucherTypeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateVoucherTypeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Vouchers

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("vouchers/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllVoucherPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetVoucher(BaseFilter filter)
        {
            var results = await this._service.GetVouchersAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<VoucherDTO>>(results));
        }

        #endregion

        [HttpPost("vouchers")]
        //[Authorize(Authorization.Policies.ManageAllVoucherPolicy)]
        [ProducesResponseType(201, Type = typeof(VoucherDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateVoucherAsync(dto);
                if (result.IsSuccess)
                {
                    VoucherDTO vm = _mapper.Map<VoucherDTO>(result.Data);
                    return CreatedAtAction("GetVoucherById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("vouchers/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllVoucherPolicy)]
        [ProducesResponseType(200, Type = typeof(VoucherDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var dto = await this._service.GetVoucherByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteVoucherAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("vouchers/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllVoucherPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateVoucher(string id, [FromBody] VoucherDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetVoucherByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateVoucherAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("vouchers/validate")]
        //[Authorize(Authorization.Policies.ViewAllVoucherPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> ValidateVoucher(int studentId, string code)
        {
            if(string.IsNullOrEmpty(code))
                return BadRequest("Invalid code.");

            return Ok(await this._service.ValidateVoucher(studentId, code));
        }

        [HttpGet("vouchers/valids")]
        //[Authorize(Authorization.Policies.ViewAllVoucherPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<VoucherDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllValidVoucher(int studentId)
        {
            return Ok(_mapper.Map<List<VoucherDTO>>(await this._service.GetAllValidVoucher(studentId)));
        }

        [HttpGet("vouchers/sieve/student-list")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetVoucherStudent(BaseFilter filter)
        {
            var results = await this._service.GetVouchersStudentAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<VoucherStudentLiteDTO>>(results));
        }

        [HttpDelete("vouchers/student/delete/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteVoucherStudent([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation($"DeleteVoucherStudent Id : {id}");

                var dto = await this._service.DeleteStudentVoucherAndUpdateCountVoucher(id);
                if (!dto.IsSuccess)
                    throw new Exception("The following errors occurred while deleting: " + string.Join(", ", dto.Message));
                return Ok(dto);
            }
            catch (Exception ex) {
                _logger.LogError($"Error DeleteVoucherStudent : {ex.Message}", ex);
                _logger.LogError($"Error DeleteVoucherStudent : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        #endregion

        #region Waivers

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("waivers/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllWaiversPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetWaivers(BaseFilter filter)
        {
            var results = await this._service.GetWaiversAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<WaiverDTO>>(results));
        }

        #endregion

        [HttpPost("waivers")]
        //[Authorize(Authorization.Policies.ManageAllWaiversPolicy)]
        [ProducesResponseType(201, Type = typeof(WaiverDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateWaiver([FromBody] WaiverDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateWaiverAsync(dto);
                if (result.IsSuccess)
                {
                    WaiverDTO vm = _mapper.Map<WaiverDTO>(result.Data);
                    return CreatedAtAction("GetWaiverById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("waivers/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllWaiversPolicy)]
        [ProducesResponseType(200, Type = typeof(WaiverDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteWaiver(int id)
        {
            var dto = await this._service.GetWaiverByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteWaiverAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("waivers/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllWaiversPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateWaiver(string id, [FromBody] WaiverDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetWaiverByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateWaiverAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion
    }
}