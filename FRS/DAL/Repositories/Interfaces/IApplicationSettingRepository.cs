using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IApplicationSettingRepository : IRepository<ApplicationSetting>
    {
        IEnumerable<ApplicationSetting> All();
        Task<List<ApplicationSetting>> GetApplicationSettingsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(ApplicationSetting applicationSetting);
        Task<bool> TestCanDeleteAsync(int applicationSettingId);
        Task<BaseOperationResponse> DeleteAsync(int applicationSettingId);
        Task<ApplicationSetting> GetByIdAsync(int id);
        Task<ApplicationSetting> GetByKeyAsync(string key, int? institutionId = null);
        Task<BaseOperationResponse> UpdateAsync(ApplicationSetting applicationSetting);
    }
}
