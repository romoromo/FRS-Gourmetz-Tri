using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.Services;
using DAL;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using FRS.Helpers;
using FRS.Hubs;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class LocationController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private IHubContext<FRSHub> _frsHub;
        private IHubContext<LocationHub> _locationHub;
        private IHubContext<FRSDeviceHub> _frsDeviceHub;
        private IHubContext<MeetingRoomHub> _meetingRoomHub;
        private readonly IConfiguration _configuration;
        private readonly ISmartRoomService _smartRoomService;
        private readonly IMapper _mapper;

        public LocationController(IUnitOfWork unitOfWork, ILogger<LocationController> logger, IHubContext<LocationHub> locationHub,
            IHubContext<FRSDeviceHub> frsDeviceHub, IHubContext<MeetingRoomHub> meetingRoomHub, IConfiguration configuration,
            ISmartRoomService smartRoomService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _locationHub = locationHub;
            _frsDeviceHub = frsDeviceHub;
            _meetingRoomHub = meetingRoomHub;
            _configuration = configuration;
            _smartRoomService = smartRoomService;
            _mapper = mapper;
        }

        /// <summary>
        /// API calls to get locations
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns>List of locations</returns>
        [HttpGet("get/{locationId:int?}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiLocations(int? locationId = null, int? institutionId = null)
        {
            var result = await _unitOfWork.Locations.GetApiLocations(locationId, institutionId);
            var data = _mapper.Map<List<LocationViewModel>>(result.Data);

            if (locationId.HasValue)
            {
                result.Data = data.Any() ? data.First() : null;
            }
            else
            {
                result.Data = data;
            }

            return Ok(result);
        }

        [HttpGet("id/{id}")]
        [Authorize(Authorization.Policies.ViewAllLocationsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<LocationViewModel>))]
        public async Task<IActionResult> GetLocationById(int id)
        {
            var location = await _unitOfWork.Locations.GetByIdAsync(id);
            return Ok(_mapper.Map<LocationViewModel>(location));
        }

        [HttpGet("locations/list")]
        [Authorize(Authorization.Policies.ViewAllLocationsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<LocationViewModel>))]
        public async Task<IActionResult> GetLocations(int? institutionId = null, bool? isBooking = null)
        {
            return await GetLocations(-1, -1, institutionId, isBooking);
        }


        [HttpGet("locations/list/{pageNumber:int}/{pageSize:int}")]
        [Authorize(Authorization.Policies.ViewAllLocationsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<LocationViewModel>))]
        public async Task<IActionResult> GetLocations(int pageNumber, int pageSize, int? institutionId = null, bool? isBooking = null)
        {
            var locations = await _unitOfWork.Locations.GetLocationsLoadRelatedAsync(pageNumber, pageSize, institutionId, isBooking);
            return Ok(_mapper.Map<List<LocationViewModel>>(locations));
        }

        [HttpPost("")]
        [Authorize(Authorization.Policies.ManageAllLocationsPolicy)]
        [ProducesResponseType(201, Type = typeof(LocationViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateLocation([FromBody] LocationViewModel location)
        {
            if (ModelState.IsValid)
            {
                if (location == null)
                    return BadRequest($"{nameof(location)} cannot be null");


                var loc = _mapper.Map<Location>(location);

                var result = await _unitOfWork.Locations.CreateAsync(loc, location.FacilityIds, location.FacilityTypeIds, location.InstitutionIds, location.FilePath, location.ImageReferences, location.LocationAssets);
                if (result.IsSuccess)
                {
                    LocationViewModel locationVM = _mapper.Map<LocationViewModel>(result.Data);
                    return CreatedAtAction("GetLocationById", new { id = locationVM.Id }, locationVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Authorization.Policies.ManageAllLocationsPolicy)]
        [ProducesResponseType(200, Type = typeof(LocationViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            if (!await _unitOfWork.Locations.TestCanDeleteAsync(id))
                return BadRequest("Location is currently being used and cannot be deleted.");


            var locationType = await this._unitOfWork.Locations.GetByIdAsync(id);

            LocationViewModel locationVM = _mapper.Map<LocationViewModel>(locationType);
            if (locationVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Locations.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting location: " + string.Join(", ", result.Message));


            return Ok(locationVM);
        }

        [HttpPut("update/{id}")]
        [Authorize(Authorization.Policies.ManageAllLocationsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateLocation(string id, [FromBody] LocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var locationType = await this._unitOfWork.Locations.GetByIdAsync(model.Id);

                LocationViewModel locationVM = _mapper.Map<LocationViewModel>(locationType);
                if (locationVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<Location>(model);
                var result = await _unitOfWork.Locations.UpdateAsync(updatedModel, model.FacilityIds, model.FacilityTypeIds, model.InstitutionIds, model.FilePath, model.ImageReferences, model.LocationAssets);
                if (result.IsSuccess)
                {
                    locationVM = _mapper.Map<LocationViewModel>(result.Data);
                    await _locationHub.Clients.Group(locationVM.Id.ToString()).SendAsync("BroadcastLocationData", locationVM);

                    var deviceResult = await _unitOfWork.Devices.GetApiPIBDevices(location_id: locationVM.Id);

                    if (deviceResult != null && deviceResult.Data != null)
                    {
                        var devices = _mapper.Map<List<DeviceViewModel>>(deviceResult.Data);

                        foreach (var device in devices)
                        {
                            await _frsDeviceHub.Clients.Group(device.Id.ToString()).SendAsync("RefreshDeviceData", device);
                        }
                    }

                    await _meetingRoomHub.Clients.Group(locationVM.Id.ToString()).SendAsync("RefreshDisplay", locationVM.Id);
                    return CreatedAtAction("GetLocationById", new { id = locationVM.Id }, locationVM);
                }

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("signage/{userId:int?}/{isBooking:bool?}")]
        //[Authorize(Authorization.Policies.ViewAllSignagePublicationsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<LocationViewModel>))]
        public async Task<IActionResult> GetLocationsByUser(int userId, bool? isBooking = null)
        {
            var locations = await _unitOfWork.Locations.GetSignageLocationsByUser(userId, isBooking);
            //var mapped = _mapper.Map<List<SimpleApiTreeResult>>(data);
            return Ok(_mapper.Map<List<LocationViewModel>>(locations));
        }

        #region Available For Bookings
        [HttpPost("GetAvailableLocations")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<LocationTimeSlotViewModel>))]
        public async Task<IActionResult> GetAvailableLocations([FromBody] CalendarFilter filter)
        {
            filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
            filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

            var locations = await _unitOfWork.Locations.GetAvailableLocations(filter);
            return Ok(_mapper.Map<List<LocationTimeSlotViewModel>>(locations));
        }
        #endregion

        #region Locations For Booking Grid
        [HttpPost("getcalendarlocations")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<BookingGridLocation>))]
        public async Task<IActionResult> GetCalendarLocations([FromBody] CalendarFilter filter)
        {
            filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
            filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

            var reservations = await _unitOfWork.Locations.GetLocationsWithParent(filter);
            return Ok(_mapper.Map<List<BookingGridLocation>>(reservations));
        }
        #endregion

        #region Location Tree
        [HttpPost("locationtree")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(LocationTreeDTO))]
        public async Task<IActionResult> GetLocationTree([FromBody] LocationTreeFilter filter)
        {
            var tree = await _unitOfWork.Locations.GetLocationTree(filter);
            return Ok(tree);
        }

        [HttpGet("exporttree")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateLocationTreeXls(int? currentUserInstitutionId, int? institutionId, int? imageReferenceColorId = null, string keyword = null, bool? isBookingOnly = null)
        {
            var xls = await _unitOfWork.Locations.GenerateLocationTreeXls(currentUserInstitutionId, institutionId, imageReferenceColorId, keyword, isBookingOnly);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_LocationTree.xlsx";
            //var filepath = Path.Combine(Utilities.GetReportPathXls(), reportName);
            //System.IO.File.WriteAllBytes(filepath, xls);
            //var returnPath = Utilities.GetRelativeReportPathPdf(reportName);

            if (xls == null || xls.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: xls,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: reportName
            );
        }


        [HttpPost("importtree"), DisableRequestSizeLimit]
        //[AllowAnonymous]
        public async Task<IActionResult> Import()
        {

            var file = Request.Form.Files[0];
            var folderName = Path.Combine("Resources", "Excel");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            Directory.CreateDirectory(pathToSave);

            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                fileName = Guid.NewGuid().ToString() + fileName;
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var resp = MapExcelToTree(fullPath);

                if (resp.Item1)
                {
                    //start importing to DB
                    return Ok(await _unitOfWork.Locations.Import(resp.Item3));
                }
                return Ok(new { IsSuccess = resp.Item1, Message = resp.Item2, Data = resp.Item3 });
            }
            else
            {
                return BadRequest();
            }

        }

        private Tuple<bool, string, List<LocationTreeImportDTO>> MapExcelToTree(string filePath)
        {
            try
            {
                IWorkbook wb = new XSSFWorkbook(new FileStream(filePath, FileMode.Open));
                var sheet = (XSSFSheet)wb.GetSheetAt(0);
                int colCount = sheet.GetRow(1).PhysicalNumberOfCells;

                if (colCount != 10) //number of columns required
                {
                    return new Tuple<bool, string, List<LocationTreeImportDTO>>(false, "There is a mismatch on the number of columns required. Please check the file.", null);
                }
                else
                {
                    var row = new List<LocationTreeImportDTO>();
                    int rowCount = sheet.PhysicalNumberOfRows;

                    for (int i = 1; i < rowCount; i++)
                    {
                        var dto = new LocationTreeImportDTO();
                        var fRow = sheet.GetRow(i);

                        if (fRow != null)
                        {
                            var institution = fRow.GetCell(0);
                            dto.Institution = institution != null ? institution.ToString().Trim() : "";
                            if (string.IsNullOrEmpty(dto.Institution))
                            {
                                throw new Exception(string.Format("Missing Institution on row {0}", i));
                            }

                            var name = fRow.GetCell(1);
                            dto.Name = name != null ? name.ToString() : "";

                            if (string.IsNullOrEmpty(dto.Name))
                            {
                                throw new Exception(string.Format("Missing Name on row {0}", i));
                            }

                            var description = fRow.GetCell(2);
                            dto.Description = description != null ? description.ToString().Trim() : "";

                            var parent = fRow.GetCell(3);
                            dto.Parent = parent != null ? parent.ToString().Trim() : "";

                            var type = fRow.GetCell(4);
                            dto.Type = type != null ? type.ToString().Trim() : "";

                            var capacity = fRow.GetCell(5);
                            dto.Capacity = capacity != null ? capacity.ToString().Trim() : "0";

                            var assignedInstitutions = fRow.GetCell(6);
                            dto.AssignedInstitutions = assignedInstitutions != null ? assignedInstitutions.ToString().Trim().Split(",").ToList() : null;

                            var facilities = fRow.GetCell(7);
                            dto.Facilities = facilities != null ? facilities.ToString().Trim().Split(",").ToList() : null;

                            var isBookingCell = fRow.GetCell(8);
                            bool isBooking = false;

                            if (isBookingCell != null)
                            {
                                bool.TryParse(isBookingCell.ToString(), out isBooking);
                            }

                            dto.IsBooking = isBooking;

                            var facePlateNumber = fRow.GetCell(9);
                            dto.FacePlateNumber = facePlateNumber != null ? facePlateNumber.ToString().Trim() : "";

                            row.Add(dto);
                        }

                    }

                    return new Tuple<bool, string, List<LocationTreeImportDTO>>(true, "File format is correct", row);
                }

            }
            catch (Exception e)
            {
                return new Tuple<bool, string, List<LocationTreeImportDTO>>(false, e.Message, null);
            }
        }
        #endregion

        #region Smart Room
        [HttpGet("syncSmartRoomResources")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> SyncSmartRoomResources(int? institutionId)
        {
            var response = await this._smartRoomService.GetListOfSmartRoomResourcesAsync();
            if (response.IsSuccess)
            {
                response = await this._unitOfWork.Locations.SyncSmartRoomResources(response.Data as SMARTRoomXML, institutionId);
            }
            
            return Ok(response);
        }

        [HttpGet("syncSmartRoomSchedules")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> SyncSmartRoomSchedules(int? institutionId)
        {
            var response = await this._smartRoomService.GetListOfSmartRoomSchedulesAsync();
            if (response.IsSuccess)
            {
                response = await this._unitOfWork.Reservations.SyncSmartRoomSchedules(response.Data as SMARTRoomXML, institutionId);
            }

            return Ok(response);
        }

        #endregion

        #region Location Types
        [HttpGet("locationtypes")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<LocationType>))]
        public async Task<IActionResult> GetLocationTypes()
        {
            var locationTypes = await _unitOfWork.Locations.GetLocationTypes();
            return Ok(locationTypes);
        }
        #endregion
    }
}