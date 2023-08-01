using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;

namespace DAL.Repositories
{
    public class PIBTemplateRepository : Repository<PIBTemplate>, IPIBTemplateRepository
    {
        public PIBTemplateRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<PIBTemplate> GetByCodeAsync(string code)
        {
            return await _appContext.PIBTemplates
                .SingleOrDefaultAsync(e => e.IsActive && (e.Name.Equals(code, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<PIBTemplate> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<PIBTemplate> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<BaseOperationResponse> GetApiPIBTemplates(int? pibTemplateId = null, string macAddress = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<PIBTemplate> query = _appContext.PIBTemplates.Where(e => e.IsActive);

                if (pibTemplateId.HasValue)
                {
                    query = query.Where(e => e.Id == pibTemplateId);
                }
                else if (!string.IsNullOrEmpty(macAddress))
                {
                    var device = await _appContext.PIBDevices.FirstOrDefaultAsync(e => e.IsActive && e.mac_address.Trim().ToLower() == macAddress.Trim().ToLower());
                    query = query.Where(e => e.Locations.Any(f => f.location_id == device.location_id)).Select(e => new PIBTemplate
                    {
                        Description = e.Description,
                        DeviceImageAPIUrl = device.epaper_url,
                        DeviceAPIURl = e.DeviceAPIURl,
                        Id = e.Id,
                        ImgUrl = e.ImgUrl,
                        IsActive = e.IsActive,
                        IsMapToAPI = e.IsMapToAPI,
                        IsPostToDevice = e.IsPostToDevice,
                        Locations = e.Locations,
                        MapAPIUrl = e.MapAPIUrl,
                        Name = e.Name,
                        TemplateBody = e.TemplateBody
                    });
                }


                response.Data = (await query.ToListAsync()).OrderBy(e => e.Name).ToList();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<List<PIBTemplate>> GetPIBTemplatesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<PIBTemplate> query = _appContext.PIBTemplates
                //.Include(e => e.Locations)
                .Where(e => e.IsActive);

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.Name);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles.OrderBy(r => r.Name).ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(PIBTemplate pibTemplate)
        {
            var result = new BaseOperationResponse();
            var locations = new List<PIBTemplateLocation>();
            locations.AddRange(pibTemplate.Locations);
            pibTemplate.Locations = new List<PIBTemplateLocation>();

            var f = await AddAsync(pibTemplate);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                //assign locations
                //add/delete locations
                //delete deselected locations
                if (locations != null && locations.Any())
                {
                    var locationsToDelete = _appContext.PIBTemplateLocations.Where(e => e.IsActive && e.PIBTemplateId == f.Id &&
                        !locations.Any(x => x.location_id == e.location_id && x.PIBTemplateId == e.PIBTemplateId));
                    await locationsToDelete.ForEachAsync(e => { e.PIBTemplateId = null; });
                    _appContext.PIBTemplateLocations.UpdateRange(locationsToDelete);
                }

                foreach (var location in locations)
                {
                    var loc = await _appContext.PIBTemplateLocations.FirstOrDefaultAsync(e => e.location_id == location.location_id);
                    if (loc == null)
                    {
                        //create one
                        location.PIBTemplateId = f.Id;
                        _appContext.PIBTemplateLocations.Add(location);
                    }
                    else
                    {
                        //update isactive
                        loc.IsActive = true;
                        loc.PIBTemplateId = f.Id;
                        _appContext.PIBTemplateLocations.Update(loc);
                    }
                }
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save template locations!";
                    result.IsSuccess = false;
                }
            }
            else
            {
                result.Message = "Failed to save template!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(PIBTemplate pibTemplate)
        {
            if (pibTemplate.Locations == null)
            {
                pibTemplate.Locations = new List<PIBTemplateLocation>();
            }

            var result = new BaseOperationResponse();

            var f = await _appContext.PIBTemplates.Include(e => e.Locations).FirstOrDefaultAsync(e => e.Id == pibTemplate.Id);

            //add/delete locations
            //delete deselected locations
            if (f.Locations != null && f.Locations.Any())
            {
                var locationsToDelete = _appContext.PIBTemplateLocations.Where(e => e.IsActive && e.PIBTemplateId == pibTemplate.Id &&
                    !pibTemplate.Locations.Any(x => x.location_id == e.location_id && x.PIBTemplateId == e.PIBTemplateId));
                await locationsToDelete.ForEachAsync(e => { e.PIBTemplateId = null; });
                _appContext.PIBTemplateLocations.UpdateRange(locationsToDelete);
            }

            foreach (var location in pibTemplate.Locations)
            {
                var loc = await _appContext.PIBTemplateLocations.FirstOrDefaultAsync(e => e.location_id == location.location_id);
                if (loc == null)
                {
                    //create one
                    location.PIBTemplateId = pibTemplate.Id;
                    await _appContext.PIBTemplateLocations.AddAsync(location);
                }
                else
                {
                    //update isactive
                    loc.IsActive = true;
                    loc.PIBTemplateId = pibTemplate.Id;
                    _appContext.PIBTemplateLocations.Update(loc);
                }
            }

            f.CopyFrom(pibTemplate);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save template!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int pibTemplateId)
        {
            //TODO: add correct logic here, for now prohibit deletion of pibTemplate
            return true;//!await _appContext..AnyAsync(e => e == pibTemplateId);
        }


        public async Task<BaseOperationResponse> DeleteAsync(int pibTemplateId)
        {
            var result = new BaseOperationResponse();
            var pibTemplate = await GetSingleOrDefaultAsync(r => r.Id == pibTemplateId);

            if (pibTemplate != null)
                return await Delete(pibTemplate);

            result.IsSuccess = false;
            result.Message = "PIB Template not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(PIBTemplate pibTemplate)
        {
            var result = new BaseOperationResponse();
            SoftDelete(pibTemplate);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete template!";
                result.IsSuccess = false;
            }

            return result;
        }

        #region PIB Device
        public async Task<BaseOperationResponse> GetApiPIBDevices(int? pibDeviceId = null, string macAddress = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<PIBDevice> query = _appContext.PIBDevices
                .Where(e => e.IsActive && ((!pibDeviceId.HasValue || e.Id == pibDeviceId) ||
                        (!string.IsNullOrEmpty(macAddress) || e.mac_address.Trim().ToLower() == macAddress.Trim().ToLower())));

                response.Data = (await query.ToListAsync()).OrderBy(r => r.device_label).ToList();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<List<PIBDevice>> GetPIBDevicesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<PIBDevice> query = _appContext.PIBDevices
                .Where(e => e.IsActive);

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.device_label);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var d = await query.ToListAsync();

            return d.OrderBy(r => r.device_label).ToList();
        }

        public async Task<BaseOperationResponse> UpdateDeviceStatus(string mac_address, string device_code)
        {
            var result = new BaseOperationResponse();
            bool isNew = false;
            if (string.IsNullOrEmpty(mac_address) && string.IsNullOrEmpty(device_code))
            {
                result.Message = "Device not registered. Missing MAC Address/Device Code!";
                result.IsSuccess = false;
                return result;
            }

            var f = await _appContext.PIBDevices.FirstOrDefaultAsync(e =>
                        (!string.IsNullOrEmpty(mac_address) && e.mac_address.ToLower() == mac_address.Trim().ToLower()) ||
                        (!string.IsNullOrEmpty(device_code) && e.device_code.ToLower() == device_code.Trim().ToLower()));
            if (f != null)
            {
                //if (f.last_heartbeat == null || DateTime.Now.Subtract(f.last_heartbeat.Value).TotalMinutes > 1.5)
                //{
                //    f.device_status = 0;
                //}
                if (!f.IsActive)
                {
                    isNew = true;
                }

                f.IsActive = true;
                f.last_heartbeat = DateTime.Now;
                f.device_status = 1;

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
                f = new PIBDevice
                {
                    mac_address = mac_address,
                    device_code = device_code,
                    last_heartbeat = DateTime.Now,
                    device_status = 1
                };
                await _appContext.PIBDevices.AddAsync(f);
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
        public async Task<BaseOperationResponse> CreateDeviceAsync(PIBDevice pibDevice)
        {
            var result = new BaseOperationResponse();
            var f = await _appContext.PIBDevices.AddAsync(pibDevice);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f.Entity;
            }
            else
            {
                result.Message = "Failed to save device!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateDeviceAsync(PIBDevice pibDevice)
        {
            var result = new BaseOperationResponse();

            var f = await _appContext.PIBDevices.FirstOrDefaultAsync(e => e.Id == pibDevice.Id);

            f.CopyFrom(pibDevice);
            _appContext.PIBDevices.Update(f);
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

            return result;
        }


        public async Task<bool> TestCanDeleteDeviceAsync(int pibDeviceId)
        {
            //TODO: add correct logic here, for now prohibit deletion of pibDevice
            return true;//!await _appContext..AnyAsync(e => e == pibDeviceId);
        }


        public async Task<BaseOperationResponse> DeleteDeviceAsync(int pibDeviceId)
        {
            var result = new BaseOperationResponse();
            var pibDevice = await _appContext.PIBDevices.SingleOrDefaultAsync(r => r.Id == pibDeviceId);

            if (pibDevice != null)
                return await DeleteDevice(pibDevice);

            result.IsSuccess = false;
            result.Message = "PIB Device not found.";
            return result;
        }

        public async Task<BaseOperationResponse> DeleteDevice(PIBDevice pibDevice)
        {
            var result = new BaseOperationResponse();
            pibDevice.IsActive = false;
            _appContext.PIBDevices.Update(pibDevice);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully deleted!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete device!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<PIBDevice> GetByDeviceIdAsync(int id)
        {
            return await _appContext.PIBDevices.FindAsync(id);
        }
        #endregion

        #region PIB Locations
        public async Task<List<PIBTemplateLocation>> GetPIBLocationsLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<PIBTemplateLocation> query = _appContext.PIBTemplateLocations
                //.Include(e => e.ancestor)
                .Where(e => e.IsActive && e.bed)
                .OrderBy(r => r.label);

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.label);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var d = await query.OrderBy(e => e.ancestor_alias).ThenBy(e => e.alias).ToListAsync();

            return d;
        }

        public async Task<BaseOperationResponse> CreateLocationsAsync(List<PIBTemplateLocation> locations)
        {
            var result = new BaseOperationResponse();
            await _appContext.PIBTemplateLocations.AddRangeAsync(locations);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to save locations!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> SyncLocationsAsync(List<PIBTemplateLocation> pibLocations)
        {
            var result = new BaseOperationResponse();
            foreach (var location in pibLocations)
            {
                var loc = await _appContext.PIBTemplateLocations.FirstOrDefaultAsync(e => e.location_id == location.location_id);
                if (loc == null)
                {
                    //create one
                    await _appContext.PIBTemplateLocations.AddAsync(location);
                }
                else
                {
                    //update isactive
                    loc.CopyFrom(location);
                    loc.IsActive = true;
                    _appContext.PIBTemplateLocations.Update(loc);
                }
            }

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to save locations!";
                result.IsSuccess = false;
            }

            return result;
        }

        #endregion

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
