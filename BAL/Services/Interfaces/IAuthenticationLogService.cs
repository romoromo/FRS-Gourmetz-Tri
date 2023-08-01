using BAL.DTO;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IAuthenticationLogService
    {
        Task<BaseOperationResponse> CreateAsync(AuthenticationLogDTO log);
        Task<PagedEntity<AuthenticationLogDTO>> GetAuthenticationLogsAsync(BaseFilter filter);
        Task<AuthenticationLogDTO> GetByIdAsync(int id);
        Task<byte[]> GenerateAuthenticationLogXls(BaseFilter filter);
    }
}
