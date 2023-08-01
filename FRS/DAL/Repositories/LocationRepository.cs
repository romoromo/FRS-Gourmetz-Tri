using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using DAL.Core.DTO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Transactions;
using DAL.Core.Helpers;

namespace DAL.Repositories
{
    public class LocationRepository : Repository<Location>, ILocationRepository
    {
        private const string _hrTimeFormat = "hh:mm tt";
        public LocationRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<Location> GetByIdAsync(int id)
        {
            //return await GetAsync(id);
            return await _appContext.Locations
                .Include(e => e.LocationImageReferences)
                .Include(e => e.LocationAssets)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public IEnumerable<Location> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<BaseOperationResponse> GetApiLocations(int? locationId = null, int? institutionId = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<Location> query = _appContext.Locations
                    .Include(e => e.LocationFacilities)
                    .Include(e => e.LocationFacilityTypes)
                    .Include(e => e.LocationInstitutions)
                    .Where(e => e.IsActive && (!locationId.HasValue || e.Id == locationId)
                                 && (!institutionId.HasValue || e.LocationInstitutions.Any(f => f.InstitutionId == institutionId)));

                response.Data = (await query.ToListAsync())
                    .OrderBy(r => r.Name).ToList();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<List<Location>> GetSignageLocationsByUser(int userId, bool? isBooking = null)
        {
            var ugLocationIds = _appContext.UserGroupLocations
                    .Include(e => e.UserGroup)
                    .ThenInclude(e => e.Members)
                    .Where(e => e.IsActive && e.UserGroup.Members.Any(f => f.UserGroup.IsActive && f.UserId == userId))
                    .Select(e => e.LocationId);

            var locations = await FindWithIncludeAsync(e => e.IsActive && ugLocationIds.Any(f => f == e.Id), e => e.LocationType);
            return locations.OrderBy(r => r.Name).ToList();
        }
        #region Signage
        public List<SignageLocationDTO> GetSignageLocations(int? institutionId = null)
        {
            IQueryable<Location> query = _appContext.Locations
                .Include(e => e.LocationFacilities)
                .Include(e => e.LocationFacilityTypes)
                .Include(e => e.LocationInstitutions)
                .Where(e => e.IsActive && (!institutionId.HasValue || e.LocationInstitutions.Any(f => f.InstitutionId == institutionId)))
                .OrderBy(r => r.Name);

            var list = new List<SignageLocationDTO>();
            foreach (var loc in query)
            {
                var dto = new SignageLocationDTO();
                dto.ID = loc.Id;
                dto.Label = loc.Name;
                dto.Description = loc.Description;
                foreach (var inst in loc.LocationInstitutions)
                {
                    dto.InstitutionID = inst.InstitutionId;
                    list.Add(dto);
                }
            }


            return list;
        }
        #endregion

        public async Task<List<Location>> GetLocationsLoadRelatedAsync(int page, int pageSize, int? institutionId = null, bool? isBooking = null)
        {
            IQueryable<Location> query = _appContext.Locations
                .Include(e => e.LocationFacilityTypes)
                .Include(e => e.LocationFacilities)
                .Include(e => e.LocationInstitutions)
                .Where(e => e.IsActive && ((e.InstitutionId == institutionId) || (!institutionId.HasValue || e.LocationInstitutions.Any(f => f.InstitutionId == institutionId)))
                && (!isBooking.HasValue || e.IsBooking == isBooking));

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.Name);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var locations = await query.ToListAsync();

            return locations
                .OrderBy(r => r.Name).ToList();
        }

        public async Task<List<LocationTimeSlot>> GetAvailableLocations(CalendarFilter filter = null)
        {
            var result = new List<LocationTimeSlot>();

            IQueryable<Location> query = _appContext.Locations.Where(e => e.IsActive && e.IsBooking)
                .Include(e => e.Reservations)
                .Include(e => e.LocationFacilities);

            if (filter != null)
            {
                if (filter.InstitutionId.HasValue)
                {
                    query = query.Where(e => e.InstitutionId == filter.InstitutionId);
                }

                if (filter.UpdatedFrom.HasValue)
                {
                    query = query.Where(e => e.UpdatedDate >= filter.UpdatedFrom.Value);
                }

                if (filter.UpdatedTo.HasValue)
                {
                    query = query.Where(e => e.UpdatedDate <= filter.UpdatedTo.Value);
                }

                if (filter.capacity > 0)
                {
                    query = query.Where(e => e.Capacity >= filter.capacity);
                }

                if (filter.locationIds != null && filter.locationIds.Count > 0)
                {
                    query = query.Where(e => filter.locationIds.Contains(e.Id));
                }

                if (filter.facilityIds != null && filter.facilityIds.Count > 0)
                {
                    var q = query.Join(_appContext.LocationFacilities.Where(e => e.IsActive),
                        loc => loc.Id,
                        fac => fac.LocationId,
                        (loc, fac) =>
                            new { Location = loc, Facility = fac });
                    query = q.Where(e => filter.facilityIds.Contains(e.Facility.FacilityId)).Select(e => e.Location).AsQueryable();
                }

                var currentTime = DateTime.Now;
                currentTime = currentTime.AddSeconds(-300);
                if (filter.Start.HasValue && filter.End.HasValue && filter.Start >= currentTime && filter.End >= currentTime)
                {
                    var dayReservations = _appContext.Reservations.Where(f => f.IsActive && f.StartDateTime < filter.End && f.EndDateTime > filter.Start);

                    var locReservations = dayReservations.GroupBy(e => e.LocationId).Select(e => new { LocationId = e.Key, Reservation = e.Select(f => f) });


                    var q = query.GroupJoin(locReservations,
                        loc => loc.Id,
                        res => res.LocationId,
                        (loc, res) =>
                            new { Location = loc, Reservations = res.DefaultIfEmpty() });


                    //var x = q.Where(e => e.ReservationTimes.Count() == 0);
                    //var y = q.SelectMany(e => e.ReservationTimes);
                    if (filter.HasAvailableTimeSlot.HasValue && filter.HasAvailableTimeSlot.Value)
                    {
                        query = q.Select(f => f.Location).Where(e => e.LocationType.Name == LocationTypes.Room.ToString() && e.IsActive);
                    }
                    else
                    {
                        query = q.Where(e => !(e.Reservations.Any() && e.Reservations.Where(f => f.Reservation != null).Any()))
                        .Select(f => f.Location).Where(e => e.LocationType.Name == LocationTypes.Room.ToString() && e.IsActive);
                    }

                    currentTime = filter.Start.Value;
                    var locationsQuery = await query.ToListAsync();
                    foreach (var location in locationsQuery.OrderBy(r => r.Name))
                    {
                        bool isAdd = false;
                        var slot = new LocationTimeSlot
                        {
                            Location = location,
                            Reservations = dayReservations.Where(e => e.LocationId == location.Id).ToList()
                            //Id = location.Id,
                            //Capacity = location.Capacity,
                            //Name = location.Name,
                            //Description = location.Description,
                            //Status = location.Status,
                            //LocationType = location.LocationType,
                            //LocationFacilities = location.LocationFacilities
                        };

                        //var currentReservation = dayReservations.FirstOrDefault(e => e.IsActive && e.LocationId == location.Id && filter.Start >= e.StartDateTime.Date && e.StartDateTime.Date  <= filter.End);

                        //slot.CurrentReservation = currentReservation;

                        var nextReservations = _appContext.Reservations.Where(e => e.IsActive && e.LocationId == location.Id && e.StartDateTime.Date == DateTime.Today);
                        if (filter.HasAvailableTimeSlot.HasValue && filter.HasAvailableTimeSlot.Value)
                        {
                            nextReservations = nextReservations.Where(e => e.StartDateTime >= currentTime ||
                            (currentTime >= e.StartDateTime && currentTime <= e.EndDateTime));
                        }
                        else
                        {
                            nextReservations = nextReservations.Where(e => e.StartDateTime >= currentTime);
                        }

                        var nextReservation = await nextReservations.OrderBy(e => e.StartDateTime).FirstOrDefaultAsync();
                        slot.AvailableFrom = currentTime.ToString(_hrTimeFormat);

                        if (nextReservation == null)
                        {
                            var institution = location.LocationInstitutions.FirstOrDefault(e => e.IsActive);
                            TimeSpan ts = TimeSpan.FromHours(institution != null && institution.Institution.EndTime > 0 ? institution.Institution.EndTime : 21);
                            DateTime time = DateTime.Today.Add(ts);

                            string fromTimeString = time.ToString(_hrTimeFormat);
                            slot.AvailableTo = fromTimeString;
                            isAdd = true;
                        }
                        else
                        {
                            if (filter.HasAvailableTimeSlot.HasValue && filter.HasAvailableTimeSlot.Value && nextReservation != null
                                && currentTime >= nextReservation.StartDateTime && nextReservation.EndDateTime >= currentTime)
                            {
                                //IN PROGRESS
                                slot.AvailableFrom = nextReservation.EndDateTime.ToString(_hrTimeFormat);
                                //get nextReservation
                                nextReservation = nextReservations.Where(e => e.Id != nextReservation.Id).OrderBy(e => e.StartDateTime).FirstOrDefault();
                                if (nextReservation == null)
                                {
                                    var institution = location.LocationInstitutions.FirstOrDefault(e => e.IsActive);
                                    TimeSpan ts = TimeSpan.FromHours(institution != null && institution.Institution.EndTime > 0 ? institution.Institution.EndTime : 21);
                                    DateTime time = DateTime.Today.Add(ts);

                                    string fromTimeString = time.ToString(_hrTimeFormat);
                                    slot.AvailableTo = fromTimeString;
                                }
                                else
                                {
                                    slot.AvailableTo = nextReservation.StartDateTime.ToString(_hrTimeFormat);
                                }
                                isAdd = true;
                            }
                            else
                            {
                                slot.AvailableTo = nextReservation.StartDateTime.ToString(_hrTimeFormat);
                                isAdd = true;
                            }
                        }

                        if (isAdd)
                        {
                            slot.AvailableTimeDisplay = string.Format("{0} - {1}", slot.AvailableFrom, slot.AvailableTo);
                            result.Add(slot);
                        }
                    }
                }
            }

            return result.Distinct().ToList();
        }

        public async Task<List<BookingGridLocation>> GetLocationsWithParent(CalendarFilter filter = null)
        {
            IQueryable<Location> query = _appContext.Locations
                .Include(e => e.LocationFacilityTypes)
                .Include(e => e.Reservations)
                .Include(e => e.LocationFacilities)
                .Where(e => e.IsActive && e.LocationType.Name == LocationTypes.Room.ToString() && e.IsBooking);

            if (filter != null)
            {
                if (filter.InstitutionId.HasValue)
                {
                    query = query.Where(e => e.InstitutionId == filter.InstitutionId);
                }

                if (filter.UpdatedFrom.HasValue)
                {
                    query = query.Where(e => e.UpdatedDate >= filter.UpdatedFrom.Value);
                }

                if (filter.UpdatedTo.HasValue)
                {
                    query = query.Where(e => e.UpdatedDate <= filter.UpdatedTo.Value);
                }

                if (filter.capacity > 0)
                {
                    query = query.Where(e => e.Capacity >= filter.capacity);
                }

                if (filter.locationIds != null && filter.locationIds.Count > 0)
                {
                    query = query.Where(e => filter.locationIds.Contains(e.Id));
                }

                if (filter.facilityIds != null && filter.facilityIds.Count > 0)
                {
                    var q = query.Join(_appContext.LocationFacilities.Where(e => e.IsActive),
                        loc => loc.Id,
                        fac => fac.LocationId,
                        (loc, fac) =>
                            new { Location = loc, Facility = fac });
                    query = q.Where(e => filter.facilityIds.Contains(e.Facility.FacilityId)).Select(e => e.Location).AsQueryable();
                }
            }

            var locationsQuery = await query.ToListAsync();

            var locations = query.OrderBy(r => r.Name).ToList().GroupBy(e => new { e.ParentLocationId, e.ParentLocation.Name }).Select(e => new BookingGridLocation
            {
                Id = e.Key.ParentLocationId,
                Name = e.Key.Name,
                Children = e.Select(f => new BookingGridLocation
                {
                    Id = f.Id,
                    Name = f.Name,
                    ParentLocationId = f.ParentLocationId,
                    Capacity = f.Capacity,
                    Facilities = f.LocationFacilities.Where(x => x.IsActive && x.Facility.IsActive).Select(a => new BookingGridLocationFacilitiy { Id = a.FacilityId, Name = a.Facility.Name, Description = a.Facility.Description }).ToList()
                }).ToList()
            });

            var results = locations.Distinct().ToList();

            return results;
        }

        public async Task<BaseOperationResponse> CreateAsync(Location location, List<int> facilityIds, List<int> facilityTypeIds, List<int> institutionIds, string filePath = null, List<LocationImageReferenceDTO> imageReferences = null, List<LocationAssetDTO> assets = null)
        {
            var result = new BaseOperationResponse();

            if (facilityIds != null)
            {
                foreach (var facilityId in facilityIds)
                {
                    location.LocationFacilities.Add(new LocationFacility { FacilityId = facilityId });
                }
            }

            if (facilityTypeIds != null)
            {
                foreach (var facilityTypeId in facilityTypeIds)
                {
                    location.LocationFacilityTypes.Add(new LocationFacilityType { FacilityTypeId = facilityTypeId });
                }
            }

            if (institutionIds != null)
            {
                foreach (var institutionId in institutionIds)
                {
                    location.LocationInstitutions.Add(new LocationInstitution { InstitutionId = institutionId });
                }
            }

            if (location.LocationImageReferences == null)
            {
                location.LocationImageReferences = new List<LocationImageReference>();
            }

            if (imageReferences != null)
            {
                foreach (var ir in imageReferences)
                {
                    var imgRef = new LocationImageReference
                    {
                        LocationId = location.Id,
                        Remarks = ir.Remarks,
                        ImageReferenceTypeId = ir.ImageReferenceTypeId,
                        ReferenceDate = ir.ReferenceDate,
                        ImageReferenceColorId = ir.ImageReferenceColorId
                    };

                    if (!string.IsNullOrEmpty(ir.FilePath))
                    {
                        //new file
                        var img = new File();
                        img.FileName = ir.FileName ?? System.IO.Path.GetFileName(ir.FilePath);
                        img.Path = ir.FilePath;
                        img.Type = ir.FileType ?? (System.IO.Path.GetExtension(ir.FilePath) == ".pdf" ? FileType.PDF.ToString() :
                                    (System.IO.Path.GetExtension(ir.FilePath) == ".ppt" || System.IO.Path.GetExtension(ir.FilePath) == ".pptx") ? FileType.PPT.ToString() : FileType.Icon.ToString());

                        imgRef.File = img;
                    }

                    location.LocationImageReferences.Add(imgRef);
                }
            }

            if (location.LocationAssets == null)
            {
                location.LocationAssets = new List<LocationAsset>();
            }

            //if (assets != null)
            //{
            //    foreach (var ir in assets)
            //    {
            //        var ass = new LocationAsset
            //        {
            //            LocationId = location.Id,
            //            AssetId = ir.AssetId
            //            //SerialNumber = ir.SerialNumber,
            //            //AssetModelId = ir.AssetModelId,
            //            //PurchaseDate = ir.PurchaseDate,
            //            //WarrantyStart = ir.WarrantyStart,
            //            //WarrantyEnd = ir.WarrantyEnd
            //        };

            //        location.LocationAssets.Add(ass);
            //    }
            //}

            var f = await AddAsync(location);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save location!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Location location, List<int> facilityIds, List<int> facilityTypeIds, List<int> institutionIds, string filePath = null, List<LocationImageReferenceDTO> imageReferences = null, List<LocationAssetDTO> assets = null)
        {
            var result = new BaseOperationResponse();

            var f = GetSingleOrDefault(e => e.Id == location.Id);
            if (!string.IsNullOrEmpty(filePath))
            {
                if (f.Icon == null)
                {
                    //TODO: check why EF Core is not loading the Icon property; interim solution
                    var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == f.FileId);
                    if (icon == null)
                    {
                        f.Icon = new File();
                    }
                    else
                    {
                        f.Icon = icon;
                        f.FileId = icon.Id;
                    }
                }

                f.Icon.Path = filePath;
                f.Icon.FileName = System.IO.Path.GetFileName(filePath);
                f.Icon.Type = FileType.Icon.ToString();
            }

            var oldFileId = f.FileId;
            f.CopyFrom(location);
            f.FileId = oldFileId;

            var locationFacilities = _appContext.LocationFacilities.Where(e => e.LocationId == location.Id);
            _appContext.LocationFacilities.RemoveRange(locationFacilities);

            foreach (var facilityId in facilityIds)
            {
                f.LocationFacilities.Add(new LocationFacility { FacilityId = facilityId, LocationId = location.Id });
            }

            var locationFacilityTypes = _appContext.LocationFacilityTypes.Where(e => e.LocationId == location.Id);
            _appContext.LocationFacilityTypes.RemoveRange(locationFacilityTypes);

            foreach (var facilityTypeId in facilityTypeIds)
            {
                f.LocationFacilityTypes.Add(new LocationFacilityType { FacilityTypeId = facilityTypeId, LocationId = location.Id });
            }

            var locationImageReferences = _appContext.LocationImageReferences.Where(e => e.LocationId == location.Id);
            await locationImageReferences.ForEachAsync(e => { e.IsActive = false; });
            _appContext.LocationImageReferences.UpdateRange(locationImageReferences);

            if (imageReferences != null)
            {
                foreach (var ir in imageReferences)
                {
                    var exist = await locationImageReferences.FirstOrDefaultAsync(e => e.Id == ir.Id);
                    if (exist != null)
                    {
                        //exists, activate
                        exist.IsActive = true;
                        exist.Remarks = ir.Remarks;
                        exist.ImageReferenceTypeId = ir.ImageReferenceTypeId;
                        exist.ReferenceDate = ir.ReferenceDate;
                        exist.ImageReferenceColorId = ir.ImageReferenceColorId;

                        if (exist.File != null && !string.IsNullOrEmpty(ir.FilePath) && exist.File.Path != ir.FilePath)
                        {
                            var existingFile = await _appContext.Files.FindAsync(ir.Id);

                            if (existingFile == null)
                            {
                                existingFile = new File();
                            }

                            existingFile.FileName = ir.FileName ?? System.IO.Path.GetFileName(ir.FilePath);
                            existingFile.Path = ir.FilePath;
                            existingFile.Type = ir.FileType ?? (System.IO.Path.GetExtension(ir.FilePath) == ".pdf" ? FileType.PDF.ToString() :
                                        (System.IO.Path.GetExtension(ir.FilePath) == ".ppt" || System.IO.Path.GetExtension(ir.FilePath) == ".pptx") ? FileType.PPT.ToString() : FileType.Icon.ToString());

                            exist.File = existingFile;
                            _appContext.Files.Update(existingFile);
                        }

                        _appContext.LocationImageReferences.Update(exist);
                    }
                    else
                    {
                        //new file
                        var img = new File();
                        img.FileName = ir.FileName ?? System.IO.Path.GetFileName(ir.FilePath);
                        img.Path = ir.FilePath;
                        img.Type = ir.FileType ?? (System.IO.Path.GetExtension(ir.FilePath) == ".pdf" ? FileType.PDF.ToString() :
                                (System.IO.Path.GetExtension(ir.FilePath) == ".ppt" || System.IO.Path.GetExtension(ir.FilePath) == ".pptx") ? FileType.PPT.ToString() : FileType.Icon.ToString());

                        var imgRef = new LocationImageReference
                        {
                            File = img,
                            LocationId = location.Id,
                            Remarks = ir.Remarks,
                            ImageReferenceTypeId = ir.ImageReferenceTypeId,
                            ReferenceDate = ir.ReferenceDate,
                            ImageReferenceColorId = ir.ImageReferenceColorId
                        };

                        await _appContext.LocationImageReferences.AddAsync(imgRef);
                    }
                }
            }

            var locationAssets = _appContext.LocationAssets.Where(e => e.LocationId == location.Id);
            await locationAssets.ForEachAsync(e => { e.IsActive = false; });
            _appContext.LocationAssets.UpdateRange(locationAssets);

            if (assets != null)
            {
                foreach (var ir in assets)
                {
                    var exist = await locationAssets.FirstOrDefaultAsync(e => e.AssetId == ir.AssetId);
                    if (exist != null)
                    {
                        //exists, activate
                        exist.IsActive = true;
                        //exist.SerialNumber = ir.SerialNumber;
                        //exist.AssetModelId = ir.AssetModelId;
                        //exist.PurchaseDate = ir.PurchaseDate;
                        //exist.WarrantyStart = ir.WarrantyStart;
                        //exist.WarrantyEnd = ir.WarrantyEnd;
                        exist.AssetId = ir.AssetId;
                        _appContext.LocationAssets.Update(exist);
                    }
                    else
                    {
                        var ass = new LocationAsset
                        {
                            LocationId = location.Id,
                            AssetId = ir.AssetId
                            //SerialNumber = ir.SerialNumber,
                            //AssetModelId = ir.AssetModelId,
                            //PurchaseDate = ir.PurchaseDate,
                            //WarrantyStart = ir.WarrantyStart,
                            //WarrantyEnd = ir.WarrantyEnd
                        };

                        await _appContext.LocationAssets.AddAsync(ass);
                    }
                }
            }

            var locationInstitutions = _appContext.LocationInstitutions.Where(e => e.LocationId == location.Id);
            _appContext.LocationInstitutions.RemoveRange(locationInstitutions);

            foreach (var institutionId in institutionIds)
            {
                f.LocationInstitutions.Add(new LocationInstitution { InstitutionId = institutionId, LocationId = location.Id });
            }

            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save location type!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int locationId)
        {
            var isCanDelete = //!await _appContext.Facilities.SelectMany(e => e.LocationFacilities).AnyAsync(e => e.LocationId == locationId) &&
                !await _appContext.Devices.AnyAsync(e => e.LocationId == locationId) &&
                !await _appContext.Reservations.AnyAsync(e => e.LocationId == locationId);

            return isCanDelete;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int locationId)
        {
            var result = new BaseOperationResponse();
            var location = await GetSingleOrDefaultAsync(r => r.Id == locationId);

            if (location != null)
                return await Delete(location);

            result.IsSuccess = false;
            result.Message = "Location not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Location location)
        {
            var result = new BaseOperationResponse();
            SoftDelete(location);
            //delete its children
            if (location.ChildLocations.Any())
            {
                await DeleteChildren(location.ChildLocations);
            }

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete location!";
                result.IsSuccess = false;
            }

            return result;
        }

        private async Task<bool> DeleteChildren(ICollection<Location> children)
        {
            foreach (var child in children)
            {
                if (child.ChildLocations.Any())
                {
                    await DeleteChildren(child.ChildLocations);
                }

                SoftDelete(child);
            }

            return true;
        }

        #region Location Tree
        public async Task<LocationTreeDTO> GetLocationTree(LocationTreeFilter filter = null)
        {
            IQueryable<Location> query = _appContext.Locations
                .Include(e => e.LocationFacilityTypes)
                .Include(e => e.LocationType)
                .Include(e => e.Reservations)
                .Include(e => e.LocationFacilities)
                .Where(e => e.IsActive);

            if (filter != null)
            {
                if (filter.CurrentUserInstitutionId.HasValue)
                {
                    query = query.Where(e => e.InstitutionId == filter.CurrentUserInstitutionId);
                }
            }

            var tree = new LocationTreeDTO();
            var locationQuery = await query.ToListAsync();
            //var t = locationQuery.ToTree((parent, child) => child.ParentLocationId == parent.Id);
            //var tFlatten = locationQuery.Flatten(e => e.ChildLocations).ToList();
            tree = await GetTree(locationQuery.OrderBy(r => r.Name).Distinct().ToList(), tree, null, filter);

            return tree;
        }

        private async Task<LocationTreeDTO> GetTree(List<Location> locations, LocationTreeDTO tree, LocationTreeDTO parentNode, LocationTreeFilter filter = null)
        {
            var nodes = locations.Where(x => parentNode == null ? !x.ParentLocationId.HasValue : x.ParentLocationId == parentNode.Id);
            foreach (var node in nodes)
            {
                var descendants = node.GetLocationAndDescendants().Where(e => e.IsActive);
                var newNode = new LocationTreeDTO(node.Id, node.Name, node.LocationType.Name);
                if (parentNode == null)
                {
                    if (tree.Children == null) tree.Children = new List<LocationTreeDTO>();
                    if (filter != null)
                    {
                        bool isFound = false;
                        isFound =
                                (descendants.Any(e => (!filter.InstitutionId.HasValue) || (e.LocationInstitutions.Any(f => f.IsActive && f.InstitutionId == filter.InstitutionId)))) &&
                                (descendants.Any(e => (!filter.ImageReferenceColorId.HasValue) || (e.LocationImageReferences.Any(f => f.IsActive && f.ImageReferenceColorId == filter.ImageReferenceColorId)))) &&
                                (descendants.Any(e => (string.IsNullOrEmpty(filter.Keyword)) ||
                                                        (e.Name.ToLower().Contains(filter.Keyword.ToLower()) ||
                                                        (!string.IsNullOrEmpty(e.Description) && e.Description.ToLower().Contains(filter.Keyword.ToLower())) ||
                                                        (!string.IsNullOrEmpty(e.FacePlateNumber) && e.FacePlateNumber.ToLower().Contains(filter.Keyword.ToLower()))))) &&
                                (descendants.Any(e => (!filter.IsBookingOnly) || (e.IsBooking == filter.IsBookingOnly)));

                        if (isFound)
                        {
                            tree.Children.Add(newNode);
                        }
                    }
                    else
                    {
                        tree.Children.Add(newNode);
                    }

                    tree.Children = tree.Children.OrderBy(e => e.Value).ToList();
                }
                else
                {
                    if (parentNode.Children == null) parentNode.Children = new List<LocationTreeDTO>();
                    //if (newNode.Children == null) newNode.Children = new List<LocationTreeDTO>();
                    if (newNode.Type == LocationTypes.Building.ToString())
                    {
                        newNode.Children = new List<LocationTreeDTO>();
                    }

                    newNode.ParentId = parentNode.Id;

                    if (filter != null)
                    {
                        bool isFound = false;
                        isFound =
                                (descendants.Any(e => (!filter.InstitutionId.HasValue) || (e.InstitutionId == filter.InstitutionId || e.LocationInstitutions.Any(f => f.IsActive && f.InstitutionId == filter.InstitutionId)))) &&
                                (descendants.Any(e => (!filter.ImageReferenceColorId.HasValue) || (e.LocationImageReferences.Any(f => f.IsActive && f.ImageReferenceColorId == filter.ImageReferenceColorId)))) &&
                                (descendants.Any(e => (string.IsNullOrEmpty(filter.Keyword)) ||
                                                        (e.Name.ToLower().Contains(filter.Keyword.ToLower()) ||
                                                        (!string.IsNullOrEmpty(e.Description) && e.Description.ToLower().Contains(filter.Keyword.ToLower())) ||
                                                        (!string.IsNullOrEmpty(e.FacePlateNumber) && e.FacePlateNumber.ToLower().Contains(filter.Keyword.ToLower()))))) &&
                                (descendants.Any(e => (!filter.IsBookingOnly) || (e.IsBooking == filter.IsBookingOnly)));

                        if (isFound)
                        {
                            parentNode.Children.Add(newNode);
                        }
                    }
                    else
                    {
                        parentNode.Children.Add(newNode);
                    }

                    parentNode.Children = parentNode.Children.OrderBy(e => e.Value).ToList();
                }

                await GetTree(locations, tree, newNode, filter);
            }

            return tree;
        }

        #endregion

        #region Location Type
        public async Task<List<LocationType>> GetLocationTypes()
        {
            return (await _appContext.LocationTypes.Where(e => e.IsActive).ToListAsync())
                .OrderBy(r => r.Name).ToList();
        }
        #endregion


        #region Excel
        public async Task<byte[]> GenerateLocationTreeXls(int? currentUserInstitutionId, int? institutionId, int? imageReferenceColorId = null, string keyword = null, bool? isBookingOnly = null)
        {
            var locations = _appContext.Locations
                       .Where(e => e.IsActive && (!currentUserInstitutionId.HasValue || e.InstitutionId == currentUserInstitutionId));

            if (locations != null)
            {
                var filter = new LocationTreeFilter
                {
                    ImageReferenceColorId = imageReferenceColorId,
                    InstitutionId = institutionId,
                    CurrentUserInstitutionId = currentUserInstitutionId,
                    IsBookingOnly = isBookingOnly.GetValueOrDefault(),
                    Keyword = keyword
                };

                var tree = new LocationTreeDTO();
                tree = await GetTree(locations.OrderBy(r => r.Name).Distinct().ToList(), tree, null, filter);
                var locIds = tree.Children.Flatten(e => e.Children).ToList().Select(e => e.Id);

                locations = locations.Where(e => locIds.Any(f => f == e.Id));

                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Location Tree");
                    var headers = new string[] { "Institution", "Name", "Description", "Parent", "Type", "Capacity", "Assigned Institutions", "Facilities", "Is Available For Booking", "Face Plate Number" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    ICell cell;
                    for (var i = 0; i < headers.Length; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();
                    locations.ToList().ForEach(dt =>
                    {
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(0);
                        cell.SetCellValue(dt.Institution != null ? dt.Institution.Name : string.Empty);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(1);
                        cell.SetCellValue(dt.Name);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(2);
                        cell.SetCellValue(dt.Description);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(3);
                        cell.SetCellValue(dt.ParentLocation != null ? dt.ParentLocation.Name : string.Empty);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(4);
                        cell.SetCellValue(dt.LocationType != null ? dt.LocationType.Name : string.Empty);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(5);
                        cell.SetCellValue(dt.Capacity);
                        cell.CellStyle = contentStyle;

                        string assignedInstitutions = string.Join(",", dt.LocationInstitutions.Where(e => e.IsActive)
                                                    .Select(e => e.Institution != null ? e.Institution.Name : string.Empty));


                        cell = row.CreateCell(6);
                        cell.SetCellValue(assignedInstitutions);
                        cell.CellStyle = contentStyle;

                        string assignedFacilities = string.Join(",", dt.LocationFacilities.Where(e => e.IsActive)
                                                    .Select(e => e.Facility != null ? e.Facility.Name : string.Empty));

                        cell = row.CreateCell(7);
                        cell.SetCellValue(assignedFacilities);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(8);
                        cell.SetCellValue(dt.IsBooking);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(9);
                        cell.SetCellValue(dt.FacePlateNumber);
                        cell.CellStyle = contentStyle;
                    });

                    #endregion

                    for (var i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Import
        public async Task<BaseOperationResponse> Import(List<LocationTreeImportDTO> rows)
        {
            var result = new BaseOperationResponse();
            //get institution
            try
            {
                if (rows.Count > 0)
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = new System.TimeSpan(24, 0, 0) },
                            TransactionScopeAsyncFlowOption.Enabled))
                    {
                        string institutionName = rows[0].Institution;
                        int institutionId = 0;
                        //disable all locations first
                        //var locationsToDisable = _appContext.Locations.Where(e => e.Institution.Name.Equals(institutionName, StringComparison.InvariantCultureIgnoreCase));

                        //locationsToDisable.ToList().ForEach(e => {
                        //    e.IsActive = false;
                        //    _appContext.Locations.Update(e);
                        //});

                        //await _appContext.SaveChangesAsync();
                        List<int> locationIds = new List<int>();

                        foreach (var row in rows)
                        {
                            var institution = await _appContext.Institutions.FirstOrDefaultAsync(e => e.IsActive && e.Name.Equals(institutionName, StringComparison.InvariantCultureIgnoreCase));

                            if (institution == null)
                            {
                                throw new Exception(string.Format("Institution not found."));
                            }
                            else
                            {
                                institutionId = institution.Id;

                                var locationType = await _appContext.LocationTypes.FirstOrDefaultAsync(e => e.IsActive && e.Name.Equals(row.Type, StringComparison.InvariantCultureIgnoreCase));

                                Location parentLocation = null;
                                if (!string.IsNullOrEmpty(row.Parent))
                                {
                                    parentLocation = await _appContext.Locations.FirstOrDefaultAsync(e => e.IsActive && e.InstitutionId == institutionId &&
                                                                e.Name.Equals(row.Parent, StringComparison.InvariantCultureIgnoreCase));
                                }

                                var location = await _appContext.Locations.FirstOrDefaultAsync(e => e.IsActive && e.InstitutionId == institutionId &&
                                                (parentLocation == null || e.ParentLocationId == parentLocation.Id) &&
                                                e.Name.Equals(row.Name, StringComparison.InvariantCultureIgnoreCase));

                                if (location == null)
                                {
                                    location = new Location();
                                }

                                location.Name = row.Name;
                                location.Description = row.Description;
                                location.InstitutionId = institution.Id;
                                location.IsBooking = row.IsBooking;
                                location.FacePlateNumber = row.FacePlateNumber;
                                if (int.TryParse(row.Capacity, out int capacity))
                                {
                                    location.Capacity = capacity;
                                }

                                if (parentLocation != null)
                                {
                                    location.ParentLocationId = parentLocation.Id;
                                }

                                if (locationType != null)
                                {
                                    location.LocationTypeId = locationType.Id;
                                }

                                if (location != null && location.Id > 0)
                                {

                                    var locationFacilities = _appContext.LocationFacilities.Where(e => e.LocationId == location.Id);
                                    _appContext.LocationFacilities.RemoveRange(locationFacilities);

                                    var locationInstitutions = _appContext.LocationInstitutions.Where(e => e.LocationId == location.Id);
                                    _appContext.LocationInstitutions.RemoveRange(locationInstitutions);
                                }

                                var institutions = _appContext.Institutions.Where(e => e.IsActive && row.AssignedInstitutions.Any(f => f.Trim().ToLower() == e.Name.ToLower()));

                                if (institutions != null)
                                {
                                    foreach (var inst in institutions)
                                    {
                                        location.LocationInstitutions.Add(new LocationInstitution { InstitutionId = inst.Id });
                                    }
                                }


                                var facilities = _appContext.Facilities.Where(e => e.IsActive && row.Facilities.Any(f => f.Trim().ToLower() == e.Name.ToLower()));

                                if (facilities != null)
                                {
                                    foreach (var fac in facilities)
                                    {
                                        location.LocationFacilities.Add(new LocationFacility { FacilityId = fac.Id });
                                    }
                                }

                                if (location == null || location.Id == 0)
                                {
                                    //create location
                                    var loc = await AddAsync(location);
                                    await _appContext.SaveChangesAsync();
                                    locationIds.Add(loc.Id);
                                }
                                else
                                {
                                    //update

                                    location.IsActive = true;
                                    Update(location);
                                    locationIds.Add(location.Id);
                                    await _appContext.SaveChangesAsync();
                                }
                            }
                        }

                        //disable removed locations
                        var locationsToDisable = _appContext.Locations.Where(e => e.InstitutionId == institutionId && locationIds.All(f => f != e.Id));

                        //var locIds = locationsToDisable.Select(e => e.Id).ToList();
                        //string ids = string.Join(",", locIds);

                        foreach (var loc in locationsToDisable)
                        {
                            loc.IsActive = false;
                            Update(loc);
                            await _appContext.SaveChangesAsync();
                        }



                        scope.Complete();
                        result.IsSuccess = true;
                        result.Message = "File Imported!";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }



            return result;
        }
        #endregion

        #region Smart Room
        public async Task<BaseOperationResponse> SyncSmartRoomResources(SMARTRoomXML smartRoom, int? institutionId)
        {
            var result = new BaseOperationResponse();

            if (smartRoom != null && smartRoom.ApplicationData != null && smartRoom.ApplicationData.Rooms != null)
            {
                //remove rooms aren't in the list
                var locsToDelete = await FindAsync(e => e.IsActive && e.SmartRoomResourceId.HasValue && !smartRoom.ApplicationData.Rooms.Room.Any(f => f.ID == e.SmartRoomResourceId));
                SoftDeleteRange(locsToDelete);

                var locType = await _appContext.LocationTypes.FirstOrDefaultAsync(e => e.Name == LocationTypes.Room.ToString());
                var rootLocation = await GetFirstOrDefaultAsync(e => e.IsActive && e.InstitutionId == institutionId && !e.ParentLocationId.HasValue);

                if (rootLocation != null)
                {
                    var parentLocation = await GetFirstOrDefaultAsync(e => e.ParentLocation != null && e.ParentLocation.ParentLocationId == rootLocation.Id && e.SmartRoomResourceId.HasValue);
                    if (parentLocation == null)
                    {
                        //create if it doesn't exist
                        var buildingType = await _appContext.LocationTypes.FirstOrDefaultAsync(e => e.Name == LocationTypes.Building.ToString());
                        parentLocation = new Location
                        {
                            InstitutionId = institutionId,
                            Description = "SMARTRoom Resources",
                            Name = "SMARTRoom Resources",
                            IsBooking = true,
                            LocationTypeId = buildingType?.Id,
                            ParentLocationId = rootLocation.Id
                        };

                        await AddAsync(parentLocation);
                    }

                    foreach (var room in smartRoom.ApplicationData.Rooms.Room)
                    {
                        var existingRoom = await this._appContext.SmartRoomResources.FirstOrDefaultAsync(e => e.SmartRoomResourceID == room.ID);
                        if (existingRoom != null)
                        {
                            existingRoom.CopyFrom(room);
                            existingRoom.SmartRoomResourceID = room.ID;
                            this._appContext.SmartRoomResources.Update(existingRoom);

                            var existingLocation = await GetFirstOrDefaultAsync(e => e.InstitutionId == institutionId && e.SmartRoomResource != null && e.SmartRoomResource.SmartRoomResourceID == existingRoom.SmartRoomResourceID);
                            if (existingLocation != null)
                            {
                                existingLocation.Capacity = room.SeatingCapacity;
                                existingLocation.Description = room.Name;
                                existingLocation.Name = room.Name;
                                existingLocation.IsBooking = true;
                                existingLocation.IsActive = true;
                                if (room.FloorID != null)
                                {
                                    existingLocation.FloorId = room.FloorID.Value;
                                }
                                existingLocation.ExchangeId = room.ExchangeID;
                                existingLocation.Extension = room.Extension;
                                existingLocation.IP = room.IP;
                                existingLocation.deviceName = room.DeviceName;
                                existingLocation.category = room.Category;
                                existingLocation.remarks = room.Remarks;
                                if (room.FloorLocationX != null) existingLocation.floorX = room.FloorLocationX.Value;
                                if (room.FloorLocationY != null) existingLocation.floorY = room.FloorLocationY.Value;

                                Update(existingLocation);
                            }
                            else
                            {
                                var location = new Location
                                {
                                    Capacity = room.SeatingCapacity,
                                    InstitutionId = institutionId,
                                    Description = room.Name,
                                    Name = room.Name,
                                    FloorId = room.FloorID.Value,
                                    ExchangeId = room.ExchangeID,
                                    Extension = room.Extension,
                                    IP = room.IP,
                                    deviceName = room.DeviceName,
                                    category = room.Category,
                                    remarks = room.Remarks,
                                    floorX = room.FloorLocationX.Value,
                                    floorY = room.FloorLocationY.Value,
                                    IsBooking = true,
                                    LocationTypeId = locType?.Id,
                                    ParentLocationId = parentLocation.Id,
                                    LocationInstitutions = new List<LocationInstitution>()
                                };

                                if (institutionId.HasValue)
                                {
                                    location.LocationInstitutions.Add(new LocationInstitution { InstitutionId = institutionId.Value });
                                }

                                await AddAsync(location);
                            }
                        }
                        else
                        {
                            existingRoom = new SmartRoomResource();
                            existingRoom.CopyFrom(room);
                            existingRoom.SmartRoomResourceID = room.ID;
                            await _appContext.SmartRoomResources.AddAsync(existingRoom);

                            var location = new Location
                            {
                                Capacity = room.SeatingCapacity,
                                InstitutionId = institutionId,
                                Description = room.Name,
                                Name = room.Name,
                                IsBooking = true,
                                FloorId = room.FloorID.Value,
                                ExchangeId = room.ExchangeID,
                                Extension = room.Extension,
                                IP = room.IP,
                                deviceName = room.DeviceName,
                                category = room.Category,
                                remarks = room.Remarks,
                                floorX = room.FloorLocationX.Value,
                                floorY = room.FloorLocationY.Value,
                                LocationTypeId = locType?.Id,
                                ParentLocationId = parentLocation.Id,
                                SmartRoomResourceId = existingRoom.Id,
                                LocationInstitutions = new List<LocationInstitution>()
                            };

                            if (institutionId.HasValue)
                            {
                                location.LocationInstitutions.Add(new LocationInstitution { InstitutionId = institutionId.Value });
                            }

                            await AddAsync(location);
                        }

                    }

                    if (await _appContext.SaveChangesAsync() > 0)
                    {
                        result.Message = "Successfully synced Smart Room Resources!";
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.Message = "Failed to sync Smart Room Resources!";
                        result.IsSuccess = false;
                    }
                }
            }
            else
            {
                result.Message = "No rooms to sync!";
                result.IsSuccess = false;
            }

            return result;
        }
        #endregion

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
