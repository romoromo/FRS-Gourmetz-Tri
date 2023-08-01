using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces
{
    public interface IAssetService
    {
        Task<BaseOperationResponse> CreateAssetModelAsync(AssetModelDTO dto);
        Task<BaseOperationResponse> CreateAssetTypeAsync(AssetTypeDTO dto);
        Task<BaseOperationResponse> CreateAssetAsync(AssetDTO dto);
        Task<BaseOperationResponse> DeleteAssetModelAsync(int id);
        Task<BaseOperationResponse> DeleteAssetTypeAsync(int id);
        Task<BaseOperationResponse> DeleteAssetAsync(int id);
        Task<AssetModelDTO> GetAssetModelByIdAsync(int id);
        Task<PagedEntity<AssetModelDTO>> GetAssetModelsAsync(BaseFilter filter);
        Task<AssetTypeDTO> GetAssetTypeByIdAsync(int id);
        Task<PagedEntity<AssetTypeDTO>> GetAssetTypesAsync(BaseFilter filter);
        Task<AssetDTO> GetAssetByIdAsync(int id);
        Task<PagedEntity<AssetDTO>> GetAssetsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAssetModelAsync(AssetModelDTO dto);
        Task<BaseOperationResponse> UpdateAssetTypeAsync(AssetTypeDTO dto);
        Task<BaseOperationResponse> UpdateAssetAsync(AssetDTO dto);
        Task<byte[]> GetTemplate();
        Task<BaseOperationResponse> ImportFile(List<List<string>> data);
    }
}