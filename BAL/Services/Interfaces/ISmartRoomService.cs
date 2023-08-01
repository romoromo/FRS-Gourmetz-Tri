using BAL.DTO;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services
{
    public interface ISmartRoomService
    {
        Task<BaseOperationResponse> GetListOfSmartRoomResourcesAsync(SMARTRoomXML tokenResponse = null, bool isReadByFile = false);
        Task<BaseOperationResponse> GetListOfSmartRoomSchedulesAsync(SMARTRoomXML tokenResponse = null, bool isReadByFile = false);
        Task<BaseOperationResponse> GetSmartRoomToken(bool isReadByFile = false);
        Task<PagedEntity<SmartRoomSchedulerLogDTO>> GetSchedulerLogs(BaseFilter filter);
        Task<BaseOperationResponse> BulkCreateSchedulerLogs(List<SmartRoomSchedulerLogDTO> logs);
        Task<BaseOperationResponse> CreateSchedulerLog(SmartRoomSchedulerLogDTO log);
    }
}