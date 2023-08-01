using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using DAL.Filters;
using Sieve.Services;
using DAL.Core.DTO;
using Microsoft.Extensions.Logging;
using DAL.Core.Logging;

namespace DAL.Repositories
{
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        private ISieveProcessor _sieveProcessor;
        private ILogger _logger;

        public DeviceRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor) : base(context)
        {
            _sieveProcessor = sieveProcessor;
            _logger = Logger.CreateLogger<DeviceRepository>();
        }

        #region Sieved
        public async Task<PagedEntity<Device>> GetDevicesAsync(BaseFilter filter)
        {
            IQueryable<Device> query = _appContext.Devices;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();

            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<Device>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion

        public async Task<Device> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Device> All()
        {
            return GetAll()
                .OrderBy(c => c.Code)
                .ToList();
        }

        public async Task<List<Device>> GetDevicesByEmsGroupId(int? emsGroupId)
        {
            IQueryable<Device> query = _appContext.Devices
                .Where(e => e.IsActive && e.ems_id == emsGroupId);

            var d = (await query.ToListAsync()).OrderByDescending(r => r.CreatedDate).ThenBy(e => e.MacAddress).ToList();

            return d;
        }

        public async Task<List<Device>> GetDownDevicesByUserGroupId(int userGroupId)
        {
            IQueryable<Device> query = _appContext.Devices
                .Where(e => e.IsActive && e.Location != null && e.Location.IsActive && e.Location.UserGroupLocations != null && e.Location.UserGroupLocations.Any(u => u.IsActive && u.UserGroupId == userGroupId));

            var d = await query.OrderByDescending(de => de.UpdatedDate).ToListAsync();
            return d;
        }

        public async Task<Device> GetByDeviceIdentifier(string identifier)
        {
            Device device = await GetFirstOrDefaultAsync(e => e.IsActive && identifier.Trim().Equals(e.Code.Trim(), StringComparison.CurrentCultureIgnoreCase));

            if(device == null)
            {
                //fail over as other apps are still using mac address
                device = await GetFirstOrDefaultAsync(e => e.IsActive && identifier.Trim().Equals(e.MacAddress.Trim(), StringComparison.CurrentCultureIgnoreCase));
            }

            return device;
        }

        public async Task<Device> GetFirstDeviceWithIdentifier(string identifier)
        {
            IQueryable<Device> query = _appContext.Devices;

            query = query.Where(e => e.IsActive && identifier.Trim().Equals(e.Code.Trim()));

            if (query.ToList().Count() <= 0)
            {
                //fail over as other apps are still using mac address
                query = _appContext.Devices.Where(e => e.IsActive && identifier.Trim().Equals(e.MacAddress.Trim()));
            }

            if (query.ToList().Count > 1) query = query.Where(e => !string.IsNullOrEmpty(e.IpAddress));

            return query.FirstOrDefault();
        }

        public async Task<BaseOperationResponse> GetRegisteredDevice(string identifier)
        {
            var response = new BaseOperationResponse();
            try
            {
                Device device = await GetFirstOrDefaultAsync(e => e.IsApproved && e.IsActive && 
                                identifier.Trim().Equals(e.Code, StringComparison.CurrentCultureIgnoreCase));

                if (device == null)
                {
                    //fail over as other apps are still using mac address
                    device = await GetFirstOrDefaultAsync(e => e.IsApproved && e.IsActive && 
                                    identifier.Trim().Equals(e.MacAddress, StringComparison.CurrentCultureIgnoreCase));
                }

                response.IsSuccess = true;
                response.Data = device;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<List<Device>> GetDevicesLoadRelatedAsync(int page, int pageSize, DeviceFilter filter = null)
        {
            IQueryable<Device> query = _appContext.Devices;
            //.Include(e => e.Institution)
            //.Include(e => e.Location);

            if (filter != null)
            {
                if (filter.InstitutionId.HasValue)
                {
                    query = query.Where(e => e.InstitutionId == filter.InstitutionId);
                }

                if (filter.IsActive.HasValue)
                {
                    query = query.Where(e => e.IsActive == filter.IsActive);
                }

                if (filter.IsApproved.HasValue)
                {
                    query = query.Where(e => e.IsApproved == filter.IsApproved);
                }
            }
            else
            {
                query = query.Where(e => e.IsActive);
            }

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.Code);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles.OrderBy(r => r.Code).ToList(); ;
        }

        public async Task<BaseOperationResponse> CreateAsync(Device device)
        {
            var result = new BaseOperationResponse();
            if (await Exists(e => e.IsActive && e.InstitutionId == device.InstitutionId &&
                (e.Code == device.Code || (
                e.MacAddress == device.MacAddress &&
                e.SerialNumber == device.SerialNumber))))
            {
                result.Message = "Device already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await AddAsync(device);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save device!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Device device)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == device.Id);
            if (await Exists(e => e.IsActive && e.Id != f.Id && e.InstitutionId == device.InstitutionId &&
                (e.Code == device.Code || (
                e.MacAddress == device.MacAddress &&
                e.SerialNumber == device.SerialNumber))))
            {
                result.Message = "Code or SN and Mac address already exists!";
                result.IsSuccess = false;
            }
            else
            {
                f.CopyFrom(device);
                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save device type!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int deviceId)
        {
            return true; //TODO: check if meeting is on-going or something
        }


        public async Task<BaseOperationResponse> DeleteAsync(int deviceId)
        {
            var result = new BaseOperationResponse();
            var device = await GetSingleOrDefaultAsync(r => r.Id == deviceId);

            if (device != null)
                return await Delete(device);

            result.IsSuccess = false;
            result.Message = "Device not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Device device)
        {
            var result = new BaseOperationResponse();
            SoftDelete(device);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete device!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateDeviceStatus(string mac_address, string device_code, string ipAddress)
        {
            var result = new BaseOperationResponse();
            bool isNew = false;
            if (string.IsNullOrEmpty(mac_address) && string.IsNullOrEmpty(device_code) && string.IsNullOrEmpty(ipAddress))
            {
                result.Message = "Device not registered. Missing MAC Address/Device Code/IP Address!";
                result.IsSuccess = false;
                return result;
            }

            Device f = null;
            if (!string.IsNullOrEmpty(device_code))
            {
                f = await _appContext.Devices.FirstOrDefaultAsync(e => e.IsActive &&
                            device_code.Trim().Equals(e.Code, StringComparison.CurrentCultureIgnoreCase));
            }

            if (f == null && !string.IsNullOrEmpty(mac_address))
            {
                f = await _appContext.Devices.FirstOrDefaultAsync(e => e.IsActive &&
                            mac_address.Trim().Equals(e.MacAddress, StringComparison.CurrentCultureIgnoreCase));
            }

            if (f == null && !string.IsNullOrEmpty(ipAddress))
            {
                f = await _appContext.Devices.FirstOrDefaultAsync(e => e.IsActive &&
                            ipAddress.Trim().Equals(e.IpAddress, StringComparison.CurrentCultureIgnoreCase));
            }

            if (f != null)
            {
                //if (f.last_heartbeat == null || DateTime.Now.Subtract(f.last_heartbeat.Value).TotalMinutes > 1.5)
                //{
                //    f.device_status = 0;
                //}
                f.device_status = 1;
                _logger.LogInformation(string.Format("device_status=1 updated from UpdateDeviceStatus for DEVICE ID {0}", f.Id));
                if (!f.IsActive)
                {
                    isNew = true;
                    f.device_status = 0;
                    _logger.LogInformation(string.Format("device_status=0 updated from UpdateDeviceStatus for DEVICE ID {0}", f.Id));
                }

                f.IsActive = true;
                f.last_heartbeat = DateTime.Now;

                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Device status saved.";
                    result.IsSuccess = true;
                    result.Data = new { isNew = isNew, data = f };
                }
                else
                {
                    result.Message = "Failed to save device status!";
                    result.IsSuccess = false;
                }
            }
            else
            {
                f = new Device
                {
                    MacAddress = mac_address.Trim(),
                    Code = !string.IsNullOrEmpty(device_code) ? device_code : !string.IsNullOrEmpty(mac_address) ? mac_address : ipAddress,
                    IpAddress = ipAddress,
                    last_heartbeat = DateTime.Now,
                    //Rotation = "Normal",
                    //Brightness = 5,
                    //Volume = 50,
                    //isScreenOn = true,
                    device_status = 0
                };

                _logger.LogInformation(string.Format("device_status=0 updated from UpdateDeviceStatus for DEVICE ID {0}. Device registered.", f.Id));
                await _appContext.Devices.AddAsync(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Device registered.";
                    result.IsSuccess = true;
                    result.Data = new { isNew = true, data = f };
                }
                else
                {
                    result.Message = "Device not registered. Failed to save device info!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        public async Task<Device> GetApiPIBDeviceByIPAddress(string ip)
        {
            Device device = await GetFirstOrDefaultAsync(e => e.IsActive &&
                            ip.Trim().Equals(e.IpAddress, StringComparison.CurrentCultureIgnoreCase));

            return device;
        }

        public async Task<BaseOperationResponse> GetApiPIBDevices(int? pibDeviceId = null, string macAddress = null, long? location_id = null, string module_path = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<Device> query = _appContext.Devices
                .Where(e => e.IsActive);

                if (pibDeviceId.HasValue && pibDeviceId.Value > 0)
                {
                    query = query.Where(e => e.Id == pibDeviceId);
                }
                else if (!string.IsNullOrEmpty(macAddress))
                {
                    query = query.Where(e => e.Code.Trim().ToLower() == macAddress.Trim().ToLower() || e.MacAddress.Trim().ToLower() == macAddress.Trim().ToLower());
                }
                else if (location_id.HasValue && location_id.Value > 0)
                {
                    query = query.Where(e => e.LocationId == location_id);
                }
                else if (!string.IsNullOrEmpty(module_path))
                {
                    query = query.Where(e => e.module_path_value == module_path);
                }

                response.Data = (await query.ToListAsync()).OrderBy(r => r.MacAddress).ToList();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        #region Dashboard
        public async Task<SignageDashboardDTO> GetSignageDashboard(DashboardFilter filter)
        {
            var dto = new SignageDashboardDTO();
            //IQueryable<Device> devices = _appContext.Devices.Where(e => e.IsActive && e.Module.Route.Equals(Constants.SIGNAGE_DISPLAY)
            //    && (string.IsNullOrEmpty(filter.Keyword) || e.Location.Name.Contains(filter.Keyword));
            var connections = _appContext.Connections.Where(e => e.IsActive).Select(e => e.Identifier);
            IQueryable<Device> devices = _appContext.Devices.Where(e => e.LocationId.HasValue && e.Location.IsActive);

            devices = this._sieveProcessor.Apply(filter.Filter, devices, applyPagination: false);

            var connectedDevices = devices.Where(e => connections.Any(f => f == e.Id.ToString()));

            //extra filters not handles by sieve
            int totalCount = 0, asleep = 0, alive = 0, unreachable = 0, totalAlive = 0;
            switch (filter.Status)
            {
                case "alive":
                    devices = devices.Where(e => connectedDevices.Any(f => f.Id == e.Id) && e.isScreenOn);
                    alive = devices.Count();
                    totalCount = alive;
                    break;
                case "asleep":
                    devices = devices.Where(e => connectedDevices.Any(f => f.Id == e.Id) && !e.isScreenOn);
                    asleep = devices.Count();
                    totalCount = asleep;
                    break;
                case "unreachable":
                    devices = devices.Where(e => connections.All(f => f != e.Id.ToString()));
                    unreachable = devices.Count();
                    totalCount = unreachable;
                    break;
                default:
                    totalCount = devices.Count();
                    totalAlive = connectedDevices.Count();
                    asleep = devices.Count(e => connectedDevices.Any(f => f.Id == e.Id) && !e.isScreenOn);
                    alive = totalAlive - asleep;
                    unreachable = devices.Count(e => connections.All(f => f != e.Id.ToString()));
                    break;
            }


            dto.Asleep = asleep;
            dto.Alive = alive;
            dto.Unreachable = unreachable;

            devices = _sieveProcessor.Apply(filter.Filter, devices, applyFiltering: false, applySorting: false);
            var pagedResult = new PagedEntity<Device>
            {
                Filter = filter.Filter,
                TotalCount = totalCount,
                PagedData = await devices.ToListAsync()
            };

            dto.PagedDevices = pagedResult;
            //dto.Devices = await devices.ToListAsync();

            //TODO: Update this when EMS Schedule is available
            return dto;
        }


        public async Task<BaseOperationResponse> PushMessages(DevicePushMessageDTO data)
        {
            var result = new BaseOperationResponse();
            var updatedDevices = new List<Device>();

            if(data.DeviceIds == null || data.DeviceIds.Count == 0)
            {
                data.DeviceIds = new List<int>();
                var connections = _appContext.Connections.Where(e => e.IsActive).Select(e => e.Identifier);
                IQueryable<Device> devices = _appContext.Devices.Where(e => e.LocationId.HasValue && e.Location.IsActive);

                devices = this._sieveProcessor.Apply(data.Filter.Filter, devices, applyPagination: false);

                var connectedDevices = devices.Where(e => e.device_status == 1 && connections.Any(f => f == e.Id.ToString()));

                //extra filters not handles by sieve
                switch (data.Filter.Status)
                {
                    case "alive":
                        devices = devices.Where(e => connectedDevices.Any(f => f.Id == e.Id) && e.isScreenOn);
                        break;
                    case "asleep":
                        devices = devices.Where(e => connectedDevices.Any(f => f.Id == e.Id) && !e.isScreenOn);
                        break;
                    case "unreachable":
                        devices = devices.Where(e => e.device_status == 0 && connections.All(f => f != e.Id.ToString()));
                        break;
                    default:
                        break;
                }

                data.DeviceIds = await devices.Select(e => e.Id).ToListAsync();
            }

            foreach(var d in data.DeviceIds)
            {
                var device = await GetSingleOrDefaultAsync(e => e.Id == d);

                if(device != null)
                {
                    device.EmergencyMessage1 = data.EmergencyMessage1;
                    device.EmergencyMessage2 = data.EmergencyMessage2;
                    device.EmergencyMessage3 = data.EmergencyMessage3;
                    device.EmergencyMessage4 = data.EmergencyMessage4;
                    updatedDevices.Add(device);
                    Update(device);
                }
            }

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully pushed!";
                result.IsSuccess = true;
                result.Data = updatedDevices;
            }
            else
            {
                result.Message = "Failed to push messages!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<List<Device>> GetSignageDashboardDevices(DashboardFilter filter)
        {
            var connections = _appContext.Connections.Where(e => e.IsActive).Select(e => e.Identifier);
            IQueryable<Device> devices = _appContext.Devices.Where(e => e.LocationId.HasValue && e.Location.IsActive);

            devices = this._sieveProcessor.Apply(filter.Filter, devices, applyPagination: false);

            var connectedDevices = devices.Where(e => connections.Any(f => f == e.Id.ToString()));

            //extra filters not handles by sieve
            switch (filter.Status)
            {
                case "alive":
                    devices = devices.Where(e => connectedDevices.Any(f => f.Id == e.Id) && e.isScreenOn);
                    break;
                case "asleep":
                    devices = devices.Where(e => connectedDevices.Any(f => f.Id == e.Id) && !e.isScreenOn);
                    break;
                case "unreachable":
                    devices = devices.Where(e => connections.All(f => f != e.Id.ToString()));
                    break;
                default:
                    break;
            }

            return await devices.ToListAsync();
        }
        #endregion

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
