using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.Helpers;
using FRS.Hubs;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class DeviceManagerController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private IHubContext<FRSHub> _frsHub;
        private IHubContext<FRSDeviceHub> _frsDeviceHub;
        private IHubContext<PIBDeviceHub> _pibDeviceHub;
        private IHttpContextAccessor _httpAccessor;
        private readonly IEmailSender _emailSender;
        private IConnectionService _connectionService;

        public DeviceManagerController(IUnitOfWork unitOfWork, ILogger<DeviceManagerController> logger, IHubContext<FRSHub> frsHub,
            IHubContext<FRSDeviceHub> frsDeviceHub,
            IHubContext<PIBDeviceHub> pibDeviceHub,
            IHttpContextAccessor httpAccessor, IEmailSender emailSender, IConnectionService connectionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _frsHub = frsHub;
            _httpAccessor = httpAccessor;
            _frsDeviceHub = frsDeviceHub;
            _pibDeviceHub = pibDeviceHub;
            _emailSender = emailSender;
            _connectionService = connectionService;
        }

        [HttpGet("getImage/{id}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetDeviceImage(int id)
        {
            var result = await this._unitOfWork.Devices.GetByIdAsync(id);

            if (result == null)
                return BadRequest("Device with id " + id + " is not found.");

            if (string.IsNullOrWhiteSpace(result.IpAddress))
                return BadRequest("Device IP Address is empty or have not been set up.");

            var filename = "S" + result.Code + ".jpg";
            filename = string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Screenshots");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullpath = Path.Combine(folderPath, filename);

            var scheme = !string.IsNullOrWhiteSpace(result.Scheme) ? result.Scheme : "http";

            try
            {
                var client = new WebClient();
                if (scheme == "https")
#pragma warning disable SCS0004 // Certificate Validation has been disabled.
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
#pragma warning restore SCS0004 // Certificate Validation has been disabled.

                client.DownloadFile(scheme + "://" + result.IpAddress + "/admin/download/SSimg", fullpath);
            }
            catch (Exception ex)
            {
                return BadRequest("The Device with IP Address " + result.IpAddress + " can't be reach. " + ex.Message);
            }

            if (!System.IO.File.Exists(fullpath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(fullpath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fullpath, out contentType))
            {
                contentType = "image/jpeg";
            }

            return File(memory, contentType, filename);
            //return base.File(fullpath, "image/jpeg");
        }

        //private string GetContentType(string path)
        //{
        //    var provider = new FileExtensionContentTypeProvider();
        //    string contentType;
        //    if (!provider.TryGetContentType(path, out contentType))
        //    {
        //        contentType = "image/jpeg";
        //    }
        //    return contentType;
        //}

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("devices/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUsers(BaseFilter filter)
        {
            var devices = await _unitOfWork.Devices.GetDevicesAsync(filter).ConfigureAwait(false);
            var deviceVMs = Mapper.Map<PagedEntityViewModel<DeviceViewModel>>(devices);
            foreach (var device in deviceVMs.PagedData)
            {
                string status = "OFFLINE";
                var connection = await _connectionService.GetConnectionStatus(device.Id);
                status = connection != null && connection.IsActive ? "ONLINE" : "OFFLINE";
                device.connection_status_display = status;
            }

            return Ok(deviceVMs);
        }

        #endregion

        [HttpGet("get/{identifier}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetDevice(string identifier)
        {
            var result = await _unitOfWork.Devices.GetRegisteredDevice(identifier);
            result.Data = Mapper.Map<DeviceViewModel>(result.Data);
            return Ok(result);
        }

        //[HttpGet("get/device")]
        //[AllowAnonymous]
        //[ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        //public async Task<IActionResult> GetApiDevices(string mac_address = null)
        //{
        //    if(mac_address == null) mac_address = _httpAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        //    var result = await _unitOfWork.Devices.GetByDeviceIdentifier(mac_address);
        //    result.Data = Mapper.Map<DeviceViewModel>(result.Data);

        //    return Ok(result);
        //}

        [HttpGet("exists/{identifier}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> DeviceExists(string identifier)
        {
            BaseOperationResponse response = new BaseOperationResponse();
            try
            {

                var result = await _unitOfWork.Devices.GetByDeviceIdentifier(identifier);
                response.IsSuccess = true;
                response.Data = Mapper.Map<DeviceViewModel>(result);
            }
            catch (Exception)
            {
                response.IsSuccess = false;
            }
            return Ok(response);

        }
        [HttpGet("devices/list")]
        //[Authorize(Authorization.Policies.ViewAllDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<DeviceViewModel>))]
        public async Task<IActionResult> GetUnknownDevices(int? institutionId = null)
        {
            return await GetUnknownDevices(-1, -1, institutionId);
        }


        [HttpGet("devices/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<DeviceViewModel>))]
        public async Task<IActionResult> GetUnknownDevices(int pageNumber, int pageSize, int? institutionId = null)
        {
            var results = await _unitOfWork.Devices.GetDevicesLoadRelatedAsync(pageNumber, pageSize, new DeviceFilter { InstitutionId = institutionId, IsActive = true });
            return Ok(Mapper.Map<List<DeviceViewModel>>(results));
        }

        /// <summary>
        /// API calls to get devices
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="mac_address"></param>
        /// <param name="isAutoRegister"></param>
        /// <returns>List of pibTemplates</returns>
        [HttpGet("get/device")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiPIBDevices(int? deviceId = null, string mac_address = null, bool isAutoRegister = true, bool useIP = false)
        {
            if (string.IsNullOrEmpty(mac_address) && useIP)
            {
                var response = new BaseOperationResponse();
                string ip = _httpAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                if (string.IsNullOrEmpty(ip))
                {
                    ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
                }

                if (string.IsNullOrEmpty(ip))
                {
                    response.Message = "Cannot get your IP Address.";
                    response.IsSuccess = false;
                }
                else
                {

                    var deviceDTO = await _unitOfWork.Devices.GetApiPIBDeviceByIPAddress(ip);
                    if (deviceDTO == null)
                    {
                        var newDevice = await _unitOfWork.Devices.UpdateDeviceStatus(ip, string.Empty, ip);
                        dynamic newDeviceData = newDevice.Data;
                        var objectData = newDeviceData.GetType().GetProperty("data").GetValue(newDeviceData, null);
                        var device = (objectData as Device);
                        device.module_path = string.Format("{0}/{1}/{2}/{3}", "/devicewarning", "adminreq", device.Id, device.MacAddress);

                        deviceDTO = device;
                        await _frsHub.Clients.All.SendAsync("RefreshPIBDeviceList", device).ConfigureAwait(false);
                    }

                    if (!deviceDTO.LocationId.HasValue)
                    {
                        response.Message = "Your device is not authorised to access the system. Please contact the administrator.";
                        response.IsSuccess = false;
                    }
                    else
                    {
                        response.IsSuccess = true;
                    }

                    response.Data = deviceDTO;
                }

                return Ok(response);
            }

            var result = await _unitOfWork.Devices.GetApiPIBDevices(deviceId, mac_address);
            var data = Mapper.Map<List<DeviceViewModel>>(result.Data);

            if (deviceId.HasValue || !string.IsNullOrEmpty(mac_address))
            {
                if (data.Any())
                {
                    result.Data = data.First();
                }
                else
                {
                    //register if isAutoRegister
                    if (isAutoRegister)
                    {
                        var newDevice = await _unitOfWork.Devices.UpdateDeviceStatus(mac_address, string.Empty, string.Empty);
                        dynamic newDeviceData = newDevice.Data;
                        var objectData = newDeviceData.GetType().GetProperty("data").GetValue(newDeviceData, null);
                        var device = (objectData as Device);
                        device.module_path = string.Format("{0}/{1}/{2}/{3}", "/devicewarning", "adminreq", device.Id, device.MacAddress);

                        result.Data = device;
                        await _frsHub.Clients.All.SendAsync("RefreshPIBDeviceList", device);
                    }
                }
            }
            else
            {
                result.Data = data;
            }

            return Ok(result);
        }

        /// <summary>
        /// API calls to create device
        /// </summary>
        /// <param name="device"></param>
        /// <returns>BaseOperationResponse</returns>
        [HttpPost("create")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ApiCreateDevice([FromBody] DeviceViewModel device)
        {
            var result = new BaseOperationResponse();
            if (ModelState.IsValid)
            {
                var deviceInfo = Mapper.Map<Device>(device);

                result = await _unitOfWork.Devices.CreateAsync(deviceInfo);
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Missing required fields. Please check your inputs.";
            }

            return Ok(result);
        }

        [HttpPost("")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDevicesPolicy)]
        [ProducesResponseType(201, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDevice([FromBody] DeviceViewModel device)
        {
            if (ModelState.IsValid)
            {
                if (device == null)
                    return BadRequest($"{nameof(device)} cannot be null");


                var deviceInfo = Mapper.Map<Device>(device);

                var result = await _unitOfWork.Devices.CreateAsync(deviceInfo);
                if (result.IsSuccess)
                {
                    DeviceViewModel vm = Mapper.Map<DeviceViewModel>(result.Data);
                    return CreatedAtAction("GetDeviceById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            if (!await _unitOfWork.Devices.TestCanDeleteAsync(id))
                return BadRequest("Device cannot be deleted. Meeting is currently on-going and try again");


            var deviceType = await this._unitOfWork.Devices.GetByIdAsync(id);

            DeviceViewModel deviceVM = Mapper.Map<DeviceViewModel>(deviceType);
            if (deviceVM == null)
                return NotFound(id);

            var result = await _unitOfWork.Devices.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting device: " + string.Join(", ", result.Message));


            return Ok(deviceVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDevicesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDevice(string id, [FromBody] DeviceViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var deviceType = await this._unitOfWork.Devices.GetByIdAsync(model.Id);

                DeviceViewModel deviceVM = Mapper.Map<DeviceViewModel>(deviceType);
                if (deviceVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<Device>(model);
                var result = await _unitOfWork.Devices.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                {
                    if (!deviceVM.IsApproved && updatedModel.IsApproved)
                    {
                        //alert the device that it is already approved
                        await _frsHub.Clients.All.SendAsync("BroadcastDeviceData", Mapper.Map<DeviceViewModel>(result.Data));
                    }

                    await _frsDeviceHub.Clients.Group(deviceVM.Id.ToString()).SendAsync("RefreshDeviceData", updatedModel);
                    await _pibDeviceHub.Clients.Group(deviceVM.Id.ToString()).SendAsync("RefreshDeviceData", updatedModel);
                    await _frsHub.Clients.All.SendAsync("RefreshPIBDeviceList", updatedModel).ConfigureAwait(false);

                    if (model.rebootWhenUpdate)
                    {
                        await _frsDeviceHub.Clients.Group(deviceVM.Id.ToString()).SendAsync("Reboot", deviceVM).ConfigureAwait(false);
                        await _pibDeviceHub.Clients.Group(deviceVM.Id.ToString()).SendAsync("Reboot", updatedModel);
                    }

                    return NoContent();
                }

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("get/servertime")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(DateTime))]
        public async Task<IActionResult> GetServerTime()
        {
            return Ok(DateTime.Now);
        }

        [HttpGet("device/reboot/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RebootPIBDevice(int id)
        {
            Utilities.CreateLogger<DeviceManagerController>().LogInformation(LoggingEvents.DEVICE_REBOOT, null, string.Format("Reboot called. ID:{0}", id));
            var device = await this._unitOfWork.Devices.GetByIdAsync(id).ConfigureAwait(false);

            DeviceViewModel deviceVM = Mapper.Map<DeviceViewModel>(device);
            await RestartDeviceAsync(deviceVM);
            await _frsDeviceHub.Clients.Group(deviceVM.Id.ToString()).SendAsync("Reboot", deviceVM).ConfigureAwait(false);

            return Ok(deviceVM);
        }

        [HttpGet("device/changechannel/{id}/{channel}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChannelUpDevice(int id, string channel)
        {
            var device = await this._unitOfWork.Devices.GetByIdAsync(id).ConfigureAwait(false);

            HttpClient client = new HttpClient();

            var uri = $"http://{device.IpAddress}:8080/jsonrpc";
            var body = "{\"jsonrpc\": \"2.0\", \"method\": \"Player.Open\", \"params\": { \"item\": { \"channelid\": " + channel + "} }, \"id\": 1 }";


            var authenticationString = $"kodi:kodi";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));


            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
                Content = new StringContent(body, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json),
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

            var response = new HttpResponseMessage();

            try
            {
                response = await client.SendAsync(request).ConfigureAwait(false);

                //response = await client.PostAsJsonAsync(uri, body);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return Ok("Success " + responseBody);
            }
            catch (Exception ex)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Error = (serializer, err) =>
                {
                    err.ErrorContext.Handled = true;
                };

                return Ok("Failed" + ex.GetBaseException());// + "\n\n" + body);
            }
        }

        [HttpGet("device/channelup/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChannelUpDevice(int id)
        {
            var device = await this._unitOfWork.Devices.GetByIdAsync(id).ConfigureAwait(false);

            HttpClient client = new HttpClient();

            var uri = $"http://{device.IpAddress}:8080/jsonrpc";
            var body = "{\"jsonrpc\":\"2.0\",\"method\":\"Input.ExecuteAction\",\"params\":{\"action\":\"channelup\"},\"id\": 1}";


            var authenticationString = $"kodi:kodi";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));


            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
                Content = new StringContent(body, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json),
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

            var response = new HttpResponseMessage();

            try
            {
                response = await client.SendAsync(request).ConfigureAwait(false);

                //response = await client.PostAsJsonAsync(uri, body);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return Ok("Success " + responseBody);
            } catch(Exception ex)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Error = (serializer, err) =>
                {
                    err.ErrorContext.Handled = true;
                };

                return Ok("Failed" + ex.GetBaseException());
            }
        }

        [HttpGet("device/channeldown/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChannelDownDevice(int id)
        {
            var device = await this._unitOfWork.Devices.GetByIdAsync(id).ConfigureAwait(false);

            HttpClient client = new HttpClient();

            var uri = $"http://{device.IpAddress}:8080/jsonrpc";
            var body = "{\"jsonrpc\":\"2.0\",\"method\":\"Input.ExecuteAction\",\"params\":{\"action\":\"channeldown\"},\"id\": 1}";


            var authenticationString = $"kodi:kodi";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));


            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
                Content = new StringContent(body, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json),
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

            var response = new HttpResponseMessage();

            try
            {
                response = await client.SendAsync(request).ConfigureAwait(false);

                //response = await client.PostAsJsonAsync(uri, body);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return Ok("Success " + responseBody);
            }
            catch (Exception ex)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Error = (serializer, err) =>
                {
                    err.ErrorContext.Handled = true;
                };

                return Ok("Failed" + ex.GetBaseException());
            }
        }

        [HttpGet("device/screenon/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ScreenOnDevice(int id)
        {
            var device = await this._unitOfWork.Devices.GetByIdAsync(id).ConfigureAwait(false);

            HttpClient client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"http://{device.IpAddress}/admin/screen_on.php")
            };

            try
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok("Failed" + ex.GetBaseException());
            }
        }

        [HttpGet("device/screenoff/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ScreenOffDevice(int id)
        {
            var device = await this._unitOfWork.Devices.GetByIdAsync(id).ConfigureAwait(false);

            HttpClient client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"http://{device.IpAddress}/admin/screen_off.php")
            };

            try
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Failed" + ex.GetBaseException());
            }
        }

        [HttpGet("device/off/{id}")]
        [AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Off(int id)
        {
            var device = await this._unitOfWork.Devices.GetByIdAsync(id).ConfigureAwait(false);

            try
            {
                DeviceViewModel deviceVM = Mapper.Map<DeviceViewModel>(device);
                deviceVM.isScreenOn = false;
                await UpdateDevice(deviceVM.Id + "", deviceVM);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Failed" + ex.GetBaseException());
            }
        }

        [HttpGet("device/on/{id}")]
        [AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> On(int id)
        {
            var device = await this._unitOfWork.Devices.GetByIdAsync(id).ConfigureAwait(false);

            try
            {
                DeviceViewModel deviceVM = Mapper.Map<DeviceViewModel>(device);
                deviceVM.isScreenOn = true;
                await UpdateDevice(deviceVM.Id + "", deviceVM);

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok("Failed" + ex.GetBaseException());
            }
        }

        [HttpGet("device/refresh/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(DeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RefreshPIBDevice(int id)
        {
            Utilities.CreateLogger<DeviceManagerController>().LogInformation(LoggingEvents.DEVICE_REFRESH, null, string.Format("Reboot called. ID:{0}", id));

            var device = await this._unitOfWork.Devices.GetByIdAsync(id).ConfigureAwait(false);

            DeviceViewModel deviceVM = Mapper.Map<DeviceViewModel>(device);
            await _frsDeviceHub.Clients.Group(deviceVM.Id.ToString()).SendAsync("RefreshPanel", deviceVM);
            await _pibDeviceHub.Clients.Group(deviceVM.Id.ToString()).SendAsync("RefreshPanel", deviceVM);
            //await _frsHub.Clients.All.SendAsync("RefreshPIBDeviceList", deviceVM).ConfigureAwait(false);

            return Ok(deviceVM);
        }

        [HttpPost("device/pushmessages")]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetActivityReportDashboard([FromBody] DevicePushMessageDTO data)
        {
            var results = await _unitOfWork.Devices.PushMessages(data);

            if (results.IsSuccess)
            {
                var listOfDevices = Mapper.Map<List<DeviceViewModel>>(results.Data as List<Device>);

                foreach (var device in listOfDevices)
                {
                    await _frsDeviceHub.Clients.Group(device.Id.ToString()).SendAsync("RefreshDeviceData", device);
                    await _pibDeviceHub.Clients.Group(device.Id.ToString()).SendAsync("RefreshDeviceData", device);
                }

                results.Data = listOfDevices;
            }


            return Ok(results);
        }

        [HttpPost("device/bulkreboot")]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> BulkReboot([FromBody] DashboardFilter filter)
        {
            Utilities.CreateLogger<DeviceManagerController>().LogInformation(LoggingEvents.DEVICE_REBOOT, null, "Bulk Reboot called.");

            var devices = await _unitOfWork.Devices.GetSignageDashboardDevices(filter);
            foreach (var device in devices)
            {
                DeviceViewModel deviceVM = Mapper.Map<DeviceViewModel>(device);
                await RestartDeviceAsync(deviceVM);
                await _frsDeviceHub.Clients.Group(deviceVM.Id.ToString()).SendAsync("Reboot", deviceVM).ConfigureAwait(false);
            }

            return Ok(new BaseOperationResponse() { IsSuccess = true, Message = "Successfully rebooted all devices" });
        }

        [HttpPost("device/bulkrefresh")]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> BulkRefreshh([FromBody] DashboardFilter filter)
        {
            Utilities.CreateLogger<DeviceManagerController>().LogInformation(LoggingEvents.DEVICE_REFRESH, null, "Bulk Refresh called.");
            var devices = await _unitOfWork.Devices.GetSignageDashboardDevices(filter);
            foreach (var device in devices)
            {
                DeviceViewModel deviceVM = Mapper.Map<DeviceViewModel>(device);
                await _frsDeviceHub.Clients.Group(deviceVM.Id.ToString()).SendAsync("RefreshPanel", deviceVM);
                await _pibDeviceHub.Clients.Group(deviceVM.Id.ToString()).SendAsync("RefreshPanel", deviceVM);
                await _frsHub.Clients.All.SendAsync("RefreshPIBDeviceList", deviceVM).ConfigureAwait(false);
            }

            return Ok(new BaseOperationResponse() { IsSuccess = true, Message = "Successfully refreshed all devices" });
        }

        private async Task RestartDeviceAsync(DeviceViewModel device)
        {
            //HttpClientHandler clientHandler = new HttpClientHandler();
            //clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var clientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var client = new HttpClient(clientHandler))
            {
                try
                {

                    var appsetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("URL_DEVICE_REBOOT_API").ConfigureAwait(false);
                    if (appsetting != null)
                    {
                        string apiURL = string.Format("{0}://{1}{2}{3}", device.Scheme, device.IpAddress,
                                                    !string.IsNullOrEmpty(device.Port) ? ":" + device.Port : string.Empty, appsetting.Value);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(apiURL);
                        var content = await response.Content.ReadAsStringAsync();
                        Utilities.CreateLogger<DeviceManagerController>().LogInformation(LoggingEvents.DEVICE_REBOOT, null, "Response:" + content);
                    }
                }
                catch (Exception ex)
                {
                    Utilities.CreateLogger<DeviceManagerController>().LogError(LoggingEvents.DEVICE_REBOOT_ERROR, ex, "An error occurred while rebooting");
                }
            }
        }

        [HttpGet("devicedownemail")]
        //[AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeviceDownEmail(string email)
        {
            try
            {
                int delay = 15;

                try
                {
                    var delaysetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("DEVICE_DOWN_NOTIFICATION_DELAY_MINUTES").ConfigureAwait(false);
                    if (delaysetting != null) delay = Int32.Parse(delaysetting.Value);
                }
                catch (Exception ex) { }

                var usergroups = await _unitOfWork.UserGroups.GetActiveUserGroups();

                foreach (var ug in usergroups)
                {
                    if (!string.IsNullOrWhiteSpace(ug.emails))
                    {
                        var devices = await _unitOfWork.Devices.GetDownDevicesByUserGroupId(ug.Id);

                        var now = DateTime.Now;

                        var downDevices = devices.Where(d =>
                        {
                            var connections = _unitOfWork.Connections.Find(e => !e.IsActive && e.Type == ConnectionType.DEVICE.ToString() && e.Identifier == d.Id + "");
                            var connection = connections.FirstOrDefault();

                            if (connection == null ||
                                (connection != null && connection.UpdatedDate > now.AddMinutes(delay * -1))) return false;
                            else return true;
                        });

                        if (downDevices.Count() > 0)
                        {

                            var deviceIdentifierKey = "{deviceIdentifier}";
                            var delayKey = "{delay}";
                            var locationNameKey = "{locationName}";
                            var ipAddressKey = "{ipAddress}";
                            var macAddressKey = "{macAddress}";
                            var serialNumberKey = "{serialNumber}";
                            var updateDateKey = "{updateDate}";

                            var title = $"Devices is down.";
                            var content = $"Devices list down below is down more than {delayKey} minutes.\n\n" +
                                $"Device List:\n\n";

                            try
                            {
                                var titleObj = await _unitOfWork.ApplicationSettings.GetByKeyAsync("DEVICE_DOWN_EMAIL_TITLE").ConfigureAwait(false);
                                var contentObj = await _unitOfWork.ApplicationSettings.GetByKeyAsync("DEVICE_DOWN_EMAIL_BODY").ConfigureAwait(false);

                                if (titleObj != null) title = titleObj.Value;
                                if (contentObj != null) content = contentObj.Value;
                            }
                            catch (Exception ex) { }

                            content = content.Replace(delayKey, delay + "");

                            content += $"<table border=\"1\" cellspacing=\"0\">" +
                                $"<tr>" +
                                $"<th>Identifier</th>" +
                                $"<th>Location</th>" +
                                $"<th>IP Address</th>" +
                                $"<th>Mac Address</th>" +
                                $"<th>Last Up</th>" +
                                $"</tr>";

                            foreach (var device in downDevices)
                            {
                                var list = $"<tr>" +
                                $"<td>{deviceIdentifierKey}</td>" +
                                $"<td>{locationNameKey}</td>" +
                                $"<td>{ipAddressKey}</td>" +
                                $"<td>{macAddressKey}</td>" +
                                $"<td>{updateDateKey}</td>" +
                                $"</tr>";

                                //$"Identifier: {deviceIdentifierKey}\n" +
                                //    $"Location: {locationNameKey}\n" +
                                //    $"IP Address: {ipAddressKey}\n" +
                                //    $"Mac Address: {macAddressKey}\n" +
                                //    $"Serial: {serialNumberKey}\n" +
                                //    $"Last On: {updateDateKey}" +
                                //    $"\n\n";

                                var connections = _unitOfWork.Connections.Find(c => c.Type == ConnectionType.DEVICE.ToString() && c.Identifier == device.Id + "");
                                var connection = connections.FirstOrDefault();

                                list = list.Replace(deviceIdentifierKey, device.Code);
                                list = list.Replace(locationNameKey, device.Location?.Name ?? "");
                                list = list.Replace(ipAddressKey, device.IpAddress);
                                list = list.Replace(macAddressKey, device.MacAddress);
                                list = list.Replace(serialNumberKey, device.SerialNumber);
                                list = list.Replace(updateDateKey, connection?.UpdatedDate.ToString("yyyy/MM/dd hh:mm tt") ?? "");


                                content += list;
                            }

                            content += $"</table>";

                            var emails = ug.emails.Split(";");

                            foreach (var e in emails)
                            {
                                try
                                {
                                    var emailBody = EmailTemplates.GetPlainTextTestEmail(DateTime.Now);
                                    var isSuccess = await _emailSender.SendEmailAsync("Device Administrator", e, title, content
                                        );
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                    }
                }

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok("Failed" + ex.InnerException);
            }
        }

    }
}