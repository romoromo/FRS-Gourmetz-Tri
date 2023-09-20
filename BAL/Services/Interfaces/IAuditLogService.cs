using BAL.DTO;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.StoredProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<PagedEntity<AuditLogDTO>> GetDataLogsAsync(BaseFilter filter);
        Task<PagedEntity<AuditLogDetailDTO>> GetDataLogDetailsAsync(BaseFilter filter);
        Task<AuditLogDTO> GetByIdAsync(int id);
        Task<byte[]> GenerateDataLogXls(BaseFilter filter);
        Task<PagedEntity<ExternalAppLoginLogDTO>> GetExternalLoginLogsAsync(BaseFilter filter);
        Task<BaseOperationResponse> CreateExternalLoginLogAsync(ExternalAppLoginLogDTO externalAppLoginLog);
        Task<byte[]> GenerateExternalLoginLogXls(BaseFilter filter);
        Task<PagedEntity<UserActivityLogDTO>> GetUserActivityLogs(UserActivityLogReportFilter filter);
        Task<byte[]> GenerateUserActivityLogsXls(UserActivityLogReportFilter filter);
    }
}
