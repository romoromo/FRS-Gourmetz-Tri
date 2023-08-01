using System.Threading.Tasks;
using BAL.DTO;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces
{
    public interface IConnectionService
    {
        Task<ConnectionDTO> GetConnectionStatus(int deviceId);
    }
}