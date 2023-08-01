using System.Threading.Tasks;
using BAL.DTO;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces
{
    public interface IServiceContractService
    {
        Task<BaseOperationResponse> CreateServiceContractAsync(ServiceContractDTO dto);
        Task<BaseOperationResponse> DeleteServiceContractAsync(int id);
        Task<ServiceContractDTO> GetServiceContractByIdAsync(int id);
        Task<PagedEntity<ServiceContractDTO>> GetServiceContractsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateServiceContractsync(ServiceContractDTO dto);
        Task<byte[]> GenerateReportXls(ServiceContractFilter filter);
    }
}