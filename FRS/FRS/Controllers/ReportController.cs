using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using DAL;
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
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiKeyAuthorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ReportController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private ITokenOrderService _tokenOrderService;

        public ReportController(IUnitOfWork unitOfWork, ILogger<AuditController> logger, ITokenOrderService tokenOrderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tokenOrderService = tokenOrderService;
        }

        [HttpGet("orders/sieve/list")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOrderLogs(SalesOrderReportFilter filter)
        {
            var logs = await _tokenOrderService.GetSalesOrdersAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<TokenOrderDTO>>(logs));
        }

        [HttpPost("orders/payment")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GeneratePaymentReport(SalesOrderReportFilter filter)
        {
            var xls = await _tokenOrderService.GeneratePaymentReport(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_PaymentReport.xlsx";

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

        [HttpGet("orders/cancellation")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOrderCancellations(SalesOrderReportFilter filter)
        {
            var orders = await _tokenOrderService.GetSalesOrdersAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<TokenOrderDTO>>(orders));
        }

        [HttpPost("orders/cancellation/export")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateCancelledOrdersXls(SalesOrderReportFilter filter)
        {
            var xls = await _tokenOrderService.GenerateCancelledOrdersXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_CancelledOrders.xlsx";

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
    }
}