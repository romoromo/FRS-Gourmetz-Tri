using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces
{
    public interface IDeviceService
    {
        Task<BaseOperationResponse> CreateDeviceTypeAsync(DeviceTypeDTO dto);
        Task<BaseOperationResponse> DeleteDeviceTypeAsync(int id);
        Task<DeviceTypeDTO> GetDeviceTypeByIdAsync(int id);
        Task<PagedEntity<DeviceTypeDTO>> GetDeviceTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateDeviceTypeAsync(DeviceTypeDTO dto);
    }
}