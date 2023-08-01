using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IKioskSettingsRepository : IRepository<KioskSettings>
    {
        IEnumerable<KioskSettings> All();
        Task<List<KioskSettings>> GetKioskSettingsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(KioskSettings kioskSettings);
        //Task<TestDeleteResult> TestCanDeleteAsync(int playlistId);
        Task<BaseOperationResponse> DeleteAsync(int kioskSettingsId);
        Task<KioskSettings> GetByIdAsync(int id);
        //Task<Playlist> GetByCode(int playlistId, string code);
        Task<BaseOperationResponse> UpdateAsync(KioskSettings kioskSettings);
    }
}
