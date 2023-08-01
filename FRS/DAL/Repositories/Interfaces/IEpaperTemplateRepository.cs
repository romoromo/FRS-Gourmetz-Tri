using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IEpaperTemplateRepository : IRepository<EpaperTemplate>
    {
        Task<EpaperTemplate> GetByCodeAsync(string code);
        IEnumerable<EpaperTemplate> All();
        Task<List<EpaperTemplate>> GetEpaperTemplatesLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> GetApiEpaperTemplates(int? epaperTemplateId = null, string macAddress = null);
        Task<BaseOperationResponse> CreateAsync(EpaperTemplate epaperTemplate);
        Task<bool> TestCanDeleteAsync(int epaperTemplateId);
        Task<BaseOperationResponse> DeleteAsync(int epaperTemplateId);
        Task<EpaperTemplate> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(EpaperTemplate epaperTemplate);

        Task<List<EpaperDevice>> GetEpaperDevicesLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> GetApiEpaperDevices(int? epaperDeviceId = null, string macAddress = null);
        Task<BaseOperationResponse> CreateDeviceAsync(EpaperDevice epaperDevice);
        Task<bool> TestCanDeleteDeviceAsync(int epaperDeviceId);
        Task<BaseOperationResponse> DeleteDeviceAsync(int epaperDeviceId);
        Task<EpaperDevice> GetByDeviceIdAsync(int id);
        Task<BaseOperationResponse> UpdateDeviceAsync(EpaperDevice epaperDevice);
        Task<BaseOperationResponse> UpdateDeviceStatus(string mac_address, string device_code);

        //Task<List<Location>> GetEpaperLocationsLoadRelatedAsync(int page, int pageSize);
        //Task<BaseOperationResponse> CreateLocationsAsync(List<EpaperTemplateLocation> locations);
        //Task<BaseOperationResponse> SyncLocationsAsync(List<EpaperTemplateLocation> epaperLocations);
    }
}
