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
    public class EpaperTemplateRepository : Repository<EpaperTemplate>, IEpaperTemplateRepository
    {
        public EpaperTemplateRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<EpaperTemplate> GetByCodeAsync(string code)
        {
            return await _appContext.EpaperTemplates
                .SingleOrDefaultAsync(e => e.IsActive && (e.Name.Equals(code, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<EpaperTemplate> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<EpaperTemplate> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<BaseOperationResponse> GetApiEpaperTemplates(int? pibTemplateId = null, string macAddress = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<EpaperTemplate> query = query = _appContext.EpaperTemplates.Where(e => e.IsActive);

                if (pibTemplateId.HasValue)
                {
                    query = query.Where(e => e.Id == pibTemplateId);
                }
                else if (!string.IsNullOrEmpty(macAddress))
                {
                    var device = await _appContext.EpaperDevices.FirstOrDefaultAsync(e => e.IsActive && e.mac_address.Trim().ToLower() == macAddress.Trim().ToLower());
                    query = query.Where(e => e.Locations.Any(f => f.LocationId == device.location_id)).Select(e => new EpaperTemplate
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


                response.Data = query.ToList().OrderBy(e => e.Name).ToList();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<List<EpaperTemplate>> GetEpaperTemplatesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<EpaperTemplate> query = _appContext.EpaperTemplates
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

        public async Task<BaseOperationResponse> CreateAsync(EpaperTemplate pibTemplate)
        {
            var result = new BaseOperationResponse();
            var locations = new List<EpaperTemplateLocation>();
            locations.AddRange(pibTemplate.Locations);
            pibTemplate.Locations = new List<EpaperTemplateLocation>();

            var f = await AddAsync(pibTemplate);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                //assign locations
                //add/delete locations
                //delete deselected locations
                if (locations != null && locations.Any())
                {
                    var locationsToDelete = _appContext.EpaperTemplateLocations.Where(e => e.IsActive && e.EpaperTemplateId == f.Id &&
                        !locations.Any(x => x.LocationId == e.LocationId && x.EpaperTemplateId == e.EpaperTemplateId));
                    await locationsToDelete.ForEachAsync(e => { e.IsActive = false; });
                    _appContext.EpaperTemplateLocations.UpdateRange(locationsToDelete);
                }

                foreach (var location in locations)
                {
                    var loc = await _appContext.EpaperTemplateLocations.FirstOrDefaultAsync(e => e.LocationId == location.LocationId);
                    if (loc == null)
                    {
                        //create one
                        location.EpaperTemplateId = f.Id;
                        await _appContext.EpaperTemplateLocations.AddAsync(location);
                    }
                    else
                    {
                        //update isactive
                        loc.IsActive = true;
                        loc.EpaperTemplateId = f.Id;
                        _appContext.EpaperTemplateLocations.Update(loc);
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

        public async Task<BaseOperationResponse> UpdateAsync(EpaperTemplate pibTemplate)
        {
            if (pibTemplate.Locations == null)
            {
                pibTemplate.Locations = new List<EpaperTemplateLocation>();
            }

            var result = new BaseOperationResponse();

            var f = await _appContext.EpaperTemplates.Include(e => e.Locations).FirstOrDefaultAsync(e => e.Id == pibTemplate.Id);

            //add/delete locations
            //delete deselected locations
            if (f.Locations != null && f.Locations.Any())
            {
                var locationsToDelete = _appContext.EpaperTemplateLocations.Where(e => e.IsActive && e.EpaperTemplateId == pibTemplate.Id &&
                    !pibTemplate.Locations.Any(x => x.LocationId == e.LocationId && x.EpaperTemplateId == e.EpaperTemplateId));
                await locationsToDelete.ForEachAsync(e => { e.IsActive = false; });
                _appContext.EpaperTemplateLocations.UpdateRange(locationsToDelete);
            }

            foreach (var location in pibTemplate.Locations)
            {
                var loc = await _appContext.EpaperTemplateLocations.FirstOrDefaultAsync(e => e.LocationId == location.LocationId);
                if (loc == null)
                {
                    //create one
                    location.EpaperTemplateId = pibTemplate.Id;
                    await _appContext.EpaperTemplateLocations.AddAsync(location);
                }
                else
                {
                    //update isactive
                    loc.IsActive = true;
                    loc.EpaperTemplateId = pibTemplate.Id;
                    _appContext.EpaperTemplateLocations.Update(loc);
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
            result.Message = "Epaper Template not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(EpaperTemplate pibTemplate)
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

        #region Epaper Device
        public async Task<BaseOperationResponse> GetApiEpaperDevices(int? pibDeviceId = null, string macAddress = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<EpaperDevice> query = _appContext.EpaperDevices
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

        public async Task<List<EpaperDevice>> GetEpaperDevicesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<EpaperDevice> query = _appContext.EpaperDevices
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

            var f = await _appContext.EpaperDevices.FirstOrDefaultAsync(e =>
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
                f = new EpaperDevice
                {
                    mac_address = mac_address,
                    device_code = device_code,
                    last_heartbeat = DateTime.Now,
                    device_status = 1
                };
                await _appContext.EpaperDevices.AddAsync(f);
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
        public async Task<BaseOperationResponse> CreateDeviceAsync(EpaperDevice pibDevice)
        {
            var result = new BaseOperationResponse();
            var f = await _appContext.EpaperDevices.AddAsync(pibDevice);
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

        public async Task<BaseOperationResponse> UpdateDeviceAsync(EpaperDevice pibDevice)
        {
            var result = new BaseOperationResponse();

            var f = await _appContext.EpaperDevices.FirstOrDefaultAsync(e => e.Id == pibDevice.Id);

            f.CopyFrom(pibDevice);
            _appContext.EpaperDevices.Update(f);
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
            var pibDevice = await _appContext.EpaperDevices.SingleOrDefaultAsync(r => r.Id == pibDeviceId);

            if (pibDevice != null)
                return await DeleteDevice(pibDevice);

            result.IsSuccess = false;
            result.Message = "Epaper Device not found.";
            return result;
        }

        public async Task<BaseOperationResponse> DeleteDevice(EpaperDevice pibDevice)
        {
            var result = new BaseOperationResponse();
            pibDevice.IsActive = false;
            _appContext.EpaperDevices.Update(pibDevice);
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

        public async Task<EpaperDevice> GetByDeviceIdAsync(int id)
        {
            return await _appContext.EpaperDevices.FindAsync(id);
        }
        #endregion

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
