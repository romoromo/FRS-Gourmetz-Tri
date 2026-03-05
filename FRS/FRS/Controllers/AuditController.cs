using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using DAL;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiKeyAuthorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuditController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IAuthenticationLogService _authenticationLogService;
        private readonly IUpDownTimeLogService _upDownTimeLogService;
        private readonly IAuditLogService _auditLogService;
        private ITokenOrderService _tokenOrderService;
        private readonly IMapper _mapper;

        public AuditController(IUnitOfWork unitOfWork, ILogger<AuditController> logger, IAuthenticationLogService authenticationLogService,
            IUpDownTimeLogService upDownTimeLogService,
            IAuditLogService auditLogService, ITokenOrderService tokenOrderService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authenticationLogService = authenticationLogService;
            _upDownTimeLogService = upDownTimeLogService;
            _auditLogService = auditLogService;
            _tokenOrderService = tokenOrderService;
            _mapper = mapper;
        }

        #region Sieved
        [HttpGet("authlogs/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAuthenticationLogs(BaseFilter filter)
        {
            var logs = await _authenticationLogService.GetAuthenticationLogsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<AuthenticationLogDTO>>(logs));
        }

        [HttpGet("updowntimelogs/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUpDownTimeLogs(BaseFilter filter)
        {
            var logs = await _upDownTimeLogService.GetUpDownTimeLogsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<UpDownTimeLogDTO>>(logs));
        }

        [HttpGet("datalogs/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAuditLogs(BaseFilter filter)
        {
            var logs = await _auditLogService.GetDataLogsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<AuditLogDTO>>(logs));
        }


        [HttpGet("datalogs/details/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAuditDetailLogs(BaseFilter filter)
        {
            var logs = await _auditLogService.GetDataLogDetailsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<AuditLogDetailDTO>>(logs));
        }

        [HttpPost("exportauthlogs")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateAuthLogXls(BaseFilter filter)
        {
            var xls = await _authenticationLogService.GenerateAuthenticationLogXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_AuthLogs.xlsx";

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

        [HttpPost("exportupdowntimelogs")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateUpDownTimeLogXls(BaseFilter filter)
        {
            var xls = await _upDownTimeLogService.GenerateUpDownTimeLogXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_UpDownTimeLogs.xlsx";

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

        [HttpPost("exportdatalogs")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateDataLogXls(BaseFilter filter)
        {
            var xls = await _auditLogService.GenerateDataLogXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_DataLogs.xlsx";
            //var filepath = Path.Combine(Utilities.GetReportPathXls(), reportName);
            //System.IO.File.WriteAllBytes(filepath, xls);
            //var returnPath = Utilities.GetRelativeReportPathPdf(reportName);

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

        #region external logs
        [ApiKeyAuthorize]
        [HttpGet("externalloginlogs/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetExternalLoginLogs(BaseFilter filter)
        {
            var logs = await _auditLogService.GetExternalLoginLogsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<ExternalAppLoginLogDTO>>(logs));
        }

        [HttpPost("externalloginlogs")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateExternalLoginLogXls(BaseFilter filter)
        {
            var xls = await _auditLogService.GenerateExternalLoginLogXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_LoginLogs.xlsx";

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

        [ApiKeyAuthorize]
        [HttpGet("orderlogs/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOrderLogs(SalesOrderReportFilter filter)
        {
            var logs = await _tokenOrderService.GetSalesOrdersAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<TokenOrderDTO>>(logs));
        }

        [HttpPost("exportorders")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateOrderLogsXls(SalesOrderReportFilter filter)
        {
            var xls = await _tokenOrderService.GenerateOrderLogXls2(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_OrderLogs.xlsx";

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

        [HttpPost("exportorders/flatten")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateFlattenOrderLogsXls(SalesOrderReportFilter filter)
        {
            var xls = await _tokenOrderService.GenerateFlattenOrderLogXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_OrdersDetails.xlsx";

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

        [HttpPost("export/collection/flatten")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateOrderCollectionXls(SalesOrderReportFilter filter)
        {
            var xls = await _tokenOrderService.GenerateOrderCollectionXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_OrderCollection.xlsx";

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

        #region user activity
        [HttpGet("useractivity/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUserActivityLogs(UserActivityLogReportFilter filter)
        {
            var logs = await _auditLogService.GetUserActivityLogs(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<UserActivityLogDTO>>(logs));
        }

        [HttpPost("exportuseractivity/flatten")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateUserActivityLogsXls(UserActivityLogReportFilter filter)
        {
            var xls = await _auditLogService.GenerateUserActivityLogsXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_UserActivityLogs.xlsx";

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

        [HttpGet("report/fas-billing-report")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<FASMonthlyBillingReportDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetWalletTransactions(FASMonthlyBillingFilter filter)
        {
            var datas = await _tokenOrderService.GetFASMonthlyBillingReport(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<FASMonthlyBillingReportDTO>>(datas));
        }

        [HttpPost("report/generate-fas-billing-report")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateWalletTransaction(FASMonthlyBillingFilter filter)
        {
            var xls = await _tokenOrderService.GenerateFASMonthlyBillingReport(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "FAS-Monthly-Billing-Report.xlsx";

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

        [HttpGet("report/detailed-basic-report")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<FASMonthlyBillingReportDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDetailedBasicWalletTopUpReport(DetailedBasicWalletTopUpFilter filter)
        {
            var datas = await _tokenOrderService.GetDetailedBasicWalletTopUpReport(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<DetailedBasicWalletTopUpReport>>(datas));
        }

        [HttpPost("report/generate-detailed-basic-report")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateGetDetailedBasicWalletTopUpReport(DetailedBasicWalletTopUpFilter filter)
        {
            var xls = await _tokenOrderService.GenerateGetDetailedBasicWalletTopUpReport(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_Detailed-Basic-Wallet-Top-Up-Report.xlsx";

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