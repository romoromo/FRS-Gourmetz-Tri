using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPIBTemplateRepository : IRepository<PIBTemplate>
    {
        Task<PIBTemplate> GetByCodeAsync(string code);
        IEnumerable<PIBTemplate> All();
        Task<List<PIBTemplate>> GetPIBTemplatesLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> GetApiPIBTemplates(int? pibTemplateId = null, string macAddress = null);
        Task<BaseOperationResponse> CreateAsync(PIBTemplate pibTemplate);
        Task<bool> TestCanDeleteAsync(int pibTemplateId);
        Task<BaseOperationResponse> DeleteAsync(int pibTemplateId);
        Task<PIBTemplate> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(PIBTemplate pibTemplate);

        Task<List<PIBDevice>> GetPIBDevicesLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> GetApiPIBDevices(int? pibDeviceId = null, string macAddress = null);
        Task<BaseOperationResponse> CreateDeviceAsync(PIBDevice pibDevice);
        Task<bool> TestCanDeleteDeviceAsync(int pibDeviceId);
        Task<BaseOperationResponse> DeleteDeviceAsync(int pibDeviceId);
        Task<PIBDevice> GetByDeviceIdAsync(int id);
        Task<BaseOperationResponse> UpdateDeviceAsync(PIBDevice pibDevice);
        Task<BaseOperationResponse> UpdateDeviceStatus(string mac_address, string device_code);
        Task<List<PIBTemplateLocation>> GetPIBLocationsLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> CreateLocationsAsync(List<PIBTemplateLocation> locations);
        Task<BaseOperationResponse> SyncLocationsAsync(List<PIBTemplateLocation> pibLocations);
    }
}
