using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDeviceRepository : IRepository<Device>
    {
        IEnumerable<Device> All();
        Task<List<Device>> GetDevicesLoadRelatedAsync(int page, int pageSize, DeviceFilter filter = null);
        Task<Device> GetByDeviceIdentifier(string identifier);
        Task<BaseOperationResponse> GetRegisteredDevice(string identifier);
        Task<BaseOperationResponse> CreateAsync(Device device);
        Task<bool> TestCanDeleteAsync(int deviceId);
        Task<BaseOperationResponse> DeleteAsync(int deviceId);
        Task<Device> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Device device);
        Task<BaseOperationResponse> UpdateDeviceStatus(string mac_address, string device_code, string ipAddress);
        Task<Device> GetApiPIBDeviceByIPAddress(string ip);
        Task<BaseOperationResponse> GetApiPIBDevices(int? pibDeviceId = null, string macAddress = null, long? location_id = null, string module_path = null);
        Task<PagedEntity<Device>> GetDevicesAsync(BaseFilter filter);
        Task<SignageDashboardDTO> GetSignageDashboard(DashboardFilter filter);
        Task<BaseOperationResponse> PushMessages(DevicePushMessageDTO data);
        Task<List<Device>> GetSignageDashboardDevices(DashboardFilter filter);
        Task<List<Device>> GetDevicesByEmsGroupId(int? emsGroupId);
        Task<List<Device>> GetDownDevicesByUserGroupId(int userGroupid);
        Task<Device> GetFirstDeviceWithIdentifier(string identifier);
    }
}
