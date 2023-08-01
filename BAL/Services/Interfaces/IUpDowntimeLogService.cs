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
    public interface IUpDownTimeLogService
    {
        Task<BaseOperationResponse> CreateAsync(UpDownTimeLogDTO log);
        Task<PagedEntity<UpDownTimeLogDTO>> GetUpDownTimeLogsAsync(BaseFilter filter);
        Task<UpDownTimeLogDTO> GetByIdAsync(int id);
        Task<byte[]> GenerateUpDownTimeLogXls(BaseFilter filter);
    }
}
