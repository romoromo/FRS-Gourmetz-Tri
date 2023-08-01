using System.Threading.Tasks;
using BAL.DTO;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces
{
    public interface IApplicationSettingService
    {
        Task<ApplicationSettingDTO> GetApplicationSettingByKey(string key);
    }
}