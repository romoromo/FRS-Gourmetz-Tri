using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface ILocationRepository : IRepository<Location>
    {
        IEnumerable<Location> All();
        Task<BaseOperationResponse> GetApiLocations(int? locationId = null, int? institutionId = null);
        Task<List<Location>> GetLocationsLoadRelatedAsync(int page, int pageSize, int? institutionId = null, bool? isBooking = null);
        Task<BaseOperationResponse> CreateAsync(Location location, List<int> facilityIds, List<int> facilityTypeIds, List<int> institutionIds, string filePath = null, List<LocationImageReferenceDTO> imageReferences = null, List<LocationAssetDTO> assets = null);
        Task<BaseOperationResponse> DeleteAsync(int id);
        Task<Location> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Location location, List<int> facilityIds, List<int> facilityTypeIds, List<int> institutionIds, string filePath = null, List<LocationImageReferenceDTO> imageReferences = null, List<LocationAssetDTO> assets = null);
        Task<bool> TestCanDeleteAsync(int id);
        Task<List<LocationTimeSlot>> GetAvailableLocations(CalendarFilter filter = null);
        List<SignageLocationDTO> GetSignageLocations(int? institutionId = null);
        Task<LocationTreeDTO> GetLocationTree(LocationTreeFilter filter = null);
        Task<List<LocationType>> GetLocationTypes();
        Task<List<BookingGridLocation>> GetLocationsWithParent(CalendarFilter filter = null);
        Task<byte[]> GenerateLocationTreeXls(int? currentUserInstitutionId, int? institutionId, int? imageReferenceColorId = null, string keyword = null, bool? isBookingOnly = null);
        Task<BaseOperationResponse> Import(List<LocationTreeImportDTO> rows);
        Task<List<Location>> GetSignageLocationsByUser(int userId, bool? isBooking = null);
        Task<BaseOperationResponse> SyncSmartRoomResources(SMARTRoomXML smartRoom, int? institutionId);
    }
}
