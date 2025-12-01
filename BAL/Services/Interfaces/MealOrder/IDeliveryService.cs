using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IDeliveryService
    {
        Task<BaseOperationResponse> CreateCatererInfoAsync(CatererInfoSimpleDTO dto);
        Task<BaseOperationResponse> DeleteCatererInfoAsync(int id);
        Task<CatererInfoDTO> GetCatererInfoByIdAsync(int id);
        Task<PagedEntity<CatererInfoDTO>> GetCatererInfosAsync(BaseFilter filter);
        Task<CatererInfoSimpleDTO> GetCatererInfoSimpleByIdAsync(int id);
        Task<PagedEntity<CatererInfoSimpleDTO>> GetCatererInfosSimpleAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateCatererInfoAsync(CatererInfoSimpleDTO dto);
        Task<BaseOperationResponse> RequestOutlet(int catererId, int outletId, string status, bool isRsp, int? outletProfileId);

        Task<BaseOperationResponse> CreateOutletProfileAsync(OutletProfileDTO dto);
        Task<bool> TestDeleteOutletProfileAsync(int id);
        Task<BaseOperationResponse> DeleteOutletProfileAsync(int id);
        Task<OutletProfileDTO> GetOutletProfileByIdAsync(int id);
        Task<PagedEntity<OutletProfileDTO>> GetOutletProfilesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateOutletProfileAsync(OutletProfileDTO dto);

        Task<BaseOperationResponse> CreateOutletAsync(OutletSimpleDTO dto);
        Task<BaseOperationResponse> DeleteOutletAsync(int id);
        Task<OutletDTO> GetOutletByIdAsync(int id);
        Task<OutletSimpleDTO> GetOutletByIdSimpleAsync(int id);
        Task<PagedEntity<OutletSimpleDTO>> GetOutletsSimpleAsync(BaseFilter filter);
        Task<PagedEntity<OutletDTO>> GetOutletsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateOutletAsync(OutletSimpleDTO dto);
        Task<BaseOperationResponse> UpdateStoresAsync(OutletDTO dto);

        Task<BaseOperationResponse> CreateBentoBoxTypeAsync(BentoBoxTypeDTO dto);
        Task<BaseOperationResponse> DeleteBentoBoxTypeAsync(int id);
        Task<BentoBoxTypeDTO> GetBentoBoxTypeByIdAsync(int id);
        Task<PagedEntity<BentoBoxTypeDTO>> GetBentoBoxTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateBentoBoxTypeAsync(BentoBoxTypeDetailsDTO dto);


        Task<BaseOperationResponse> CreateCartonTypeAsync(CartonTypeDTO dto);
        Task<BaseOperationResponse> DeleteCartonTypeAsync(int id);
        Task<CartonTypeDTO> GetCartonTypeByIdAsync(int id);
        Task<PagedEntity<CartonTypeDTO>> GetCartonTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateCartonTypeAsync(CartonTypeDetailsDTO dto);

        Task<BaseOperationResponse> CreateDeliveryOrderAsync(DeliveryOrderDTO dto);
        Task<BaseOperationResponse> DeleteDeliveryOrderAsync(int id);
        Task<DeliveryOrderDTO> GetDeliveryOrderByIdAsync(int id);
        Task<PagedEntity<DeliveryOrderDTO>> GetDeliveryOrdersAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateDeliveryOrderAsync(DeliveryOrderDTO dto);

        Task<BaseOperationResponse> CreateDeliveryOrderNewAsync(DeliveryOrderNewDTO dto);
        Task<BaseOperationResponse> DeleteDeliveryOrderNewAsync(int id);
        Task<DeliveryOrderNewDTO> GetDeliveryOrderNewByIdAsync(int id);
        Task<PagedEntity<DeliveryOrderNewDTO>> GetDeliveryOrderNewsAsync(BaseFilter filter);
        Task<PagedEntity<BentoUsageCountDTO>> GetBentoUsage(BaseFilter filter);
        Task<BaseOperationResponse> UpdateDeliveryOrderNewAsync(DeliveryOrderNewDTO dto);
        Task<BaseOperationResponse> LoadDeliveryOrderNewAsync(DeliveryOrderNewDTO dto);


        Task<BaseOperationResponse> CreateStoreInventoryAsync(StoreInventoryDTO dto);
        Task<BaseOperationResponse> DeleteStoreInventoryAsync(int id);
        Task<StoreInventoryDTO> GetStoreInventoryByIdAsync(int id);
        Task<PagedEntity<StoreInventoryDTO>> GetStoreInventoriesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateStoreInventoryAsync(StoreInventoryDTO dto);
        Task<PagedEntity<StoreInventoryDetailDTO>> GetStoreInventoryDetailsAsync(BaseFilter filter);

        Task<BaseOperationResponse> CreateTrackingStatusAsync(TrackingStatusDTO dto);
        Task<BaseOperationResponse> DeleteTrackingStatusAsync(int id);
        Task<TrackingStatusDTO> GetTrackingStatusByIdAsync(int id);
        Task<PagedEntity<TrackingStatusDTO>> GetTrackingStatussAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateTrackingStatusAsync(TrackingStatusDTO dto);

        Task<BaseOperationResponse> CreateStoreInfoAsync(StoreInfoDTO dto);
        Task<BaseOperationResponse> DeleteStoreInfoAsync(int id);
        Task<StoreInfoDTO> GetStoreInfoByIdAsync(int id);
        Task<PagedEntity<StoreInfoDTO>> GetStoreInfosAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateStoreInfoAsync(StoreInfoDTO dto);

        Task<BaseOperationResponse> CreateBentoAssetAsync(BentoAssetDTO dto);
        Task<BaseOperationResponse> DeleteBentoAssetAsync(int id);
        Task<BentoAssetDTO> GetBentoAssetByIdAsync(int id);
        Task<PagedEntity<BentoAssetDTO>> GetBentoAssetsAsync(BentoAssetsFilter filter);
        Task<PagedEntity<BentoAssetDTO>> GetBentoAssetsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateBentoAssetAsync(BentoAssetDTO dto);
        Task<BaseOperationResponse> ResetBentoAssetAsync();

        Task<BaseOperationResponse> CreateCartonAssetAsync(CartonAssetDTO dto);
        Task<BaseOperationResponse> DeleteCartonAssetAsync(int id);
        Task<CartonAssetDTO> GetCartonAssetByIdAsync(int id);
        Task<PagedEntity<CartonAssetDTO>> GetCartonAssetsAsync(CartonAssetsFilter filter);
        Task<BaseOperationResponse> UpdateCartonAssetAsync(CartonAssetDTO dto);
        Task<BaseOperationResponse> ResetCartonAssetAsync();
        Task<byte[]> GenerateCartonAssetLabel(int id);
        Task<BaseOperationResponse> CreateDisposableBoxAsync(CartonDisposableBoxDTO dto);
        Task<BaseOperationResponse> DeleteDisposableBoxAsync(int id);
        Task<CartonDisposableBoxDTO> GetDisposableBoxByIdAsync(int id);
        Task<PagedEntity<CartonDisposableBoxDTO>> GetDisposableBoxesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateDisposableAsync(CartonDisposableBoxDTO dto);
        Task<BaseOperationResponse> ResetDisposableBoxAsync();
        Task<BaseOperationResponse> ResetDishDisposableBoxAsync(CartonDisposableBoxDTO dto);


        Task<BaseOperationResponse> CreateDriverAsync(DriverDTO dto);
        Task<BaseOperationResponse> DeleteDriverAsync(int id);
        Task<DriverDTO> GetDriverByIdAsync(int id);
        Task<PagedEntity<DriverDTO>> GetDriversAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateDriverAsync(DriverDTO dto);

        Task<BaseOperationResponse> CreateRouteAsync(RouteDTO dto);
        Task<BaseOperationResponse> DeleteRouteAsync(int id);
        Task<RouteDTO> GetRouteByIdAsync(int id);
        Task<PagedEntity<RouteDTO>> GetRoutesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateRouteAsync(RouteDTO dto);

        Task<BaseOperationResponse> CreateOutletTermAsync(OutletTermDTO dto);
        Task<BaseOperationResponse> DeleteOutletTermAsync(int id);
        Task<OutletTermDTO> GetOutletTermByIdAsync(int id);
        Task<PagedEntity<OutletTermDTO>> GetOutletTermsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateOutletTermAsync(OutletTermDTO dto);

        Task<byte[]> GeneratePrintDo(int doId);

        Task<byte[]> GeneratePrintCartonLabel(int doId);


        Task<PagedEntity<CatererAssetDTO>> GetCatererAssetsAsync(CatererAsserFilter filter);
        Task<CatererAssetDTO> GetCatererAssetByIdAsync(int id);
        Task<BaseOperationResponse> CreateCatererAssetAsync(CatererAssetDTO dto);
        Task<BaseOperationResponse> UpdateCatererAssetAsync(CatererAssetDTO dto);
        Task<BaseOperationResponse> DeleteCatererAssetAsync(int id);
        Task<string> GetAssetQRCode(int catererId);
        Task<byte[]> GenerateAssetQRCode(int id, int catererId);
    }
}