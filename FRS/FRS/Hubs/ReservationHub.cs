using AutoMapper;
using BAL.DTO;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core;
using DAL.Models;
using DAL.Repositories;
using FRS.Helpers;
using FRS.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.Hubs
{
    public class FRSHub : Hub
    {
        public async Task BroadcastReservationData(ReservationViewModel data) => await Clients.All.SendAsync("BroadcastReservationData", data);
        public async Task BroadcastDeviceData(DeviceViewModel data) => await Clients.All.SendAsync("BroadcastDeviceData", data);
        public async Task RefreshPIBDeviceList(PIBDeviceViewModel data) => await Clients.All.SendAsync("RefreshPIBDeviceList", data);
    }

    public class ReservationHub : Hub
    {
        public async Task BroadcastReservationData(ReservationViewModel data) => await Clients.All.SendAsync("BroadcastReservationData", data);
    }

    public class EMSHub : Hub
    {
    }

    public class DeviceHub : Hub
    {
        public async Task BroadcastDeviceData(DeviceViewModel data) => await Clients.All.SendAsync("BroadcastDeviceData", data);
    }

    //public class PIBDeviceHub : Hub
    //{
    //    private IHubContext<PIBDeviceHub> _hub;
    //    public PIBDeviceHub(IHubContext<PIBDeviceHub> hub)
    //    {
    //        _hub = hub;
    //    }

    //    public override Task OnConnectedAsync()
    //    {
    //        var httpContext = Context.GetHttpContext();

    //        var query = httpContext.Request.Query;
    //        var macAddress = query.GetQueryParameterValue<string>("mac_address");
    //        var connectionId = Context.ConnectionId;

    //        Groups.AddToGroupAsync(Context.ConnectionId, macAddress);

    //        return base.OnConnectedAsync();
    //    }

    //    public override Task OnDisconnectedAsync(Exception exception)
    //    {
    //        var httpContext = Context.GetHttpContext();

    //        var query = httpContext.Request.Query;
    //        var macAddress = query.GetQueryParameterValue<string>("mac_address");
    //        var connectionId = Context.ConnectionId;

    //        _hub.Clients.All.SendAsync("RefreshPIBDeviceList", null);

    //        return base.OnDisconnectedAsync(exception);
    //    }

    //    public async Task BroadcastDeviceData(PIBDeviceViewModel data) => await Clients.Group(data.Id.ToString()).SendAsync("BroadcastDeviceData", data);
    //}

    public class LocationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var query = httpContext.Request.Query;
            var locationId = query.GetQueryParameterValue<string>("location_id");
            var connectionId = Context.ConnectionId;

            Groups.AddToGroupAsync(Context.ConnectionId, locationId);

            return base.OnConnectedAsync();
        }

        public async Task BroadcastLocationData(LocationViewModel data) => await Clients.Group(data.Id.ToString()).SendAsync("BroadcastDeviceData", data);
    }

    public class QueueHub : Hub
    {
        public const string UpdateQueueKey = "UpdateQueue";
        public const string RefreshTableKey = "RefreshTable";

        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QueueHub(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var query = httpContext.Request.Query;
            var deviceId = query.GetQueryParameterValue<string>("device_id");
            var connectionId = Context.ConnectionId;

            Groups.AddToGroupAsync(Context.ConnectionId, deviceId);

            return base.OnConnectedAsync();
        }

        public async Task QueueReturn(Dictionary<string, string> data)
        {
            await _unitOfWork.QueueTableMaps.QueueReturn(data);

            await Clients.Group("MonitoringTable").SendAsync(RefreshTableKey);
        }

        public async Task UpdateQueue(Dictionary<string, string> data) => await Clients.Group(data[QueueTableMapRepository.DevideId].ToString()).SendAsync(UpdateQueueKey, data);
    }

    public class EmployeeScheduleHub : Hub
    {
        public const string UpdateKey = "UpdateSchedule";

        private IUnitOfWork _unitOfWork;

        public EmployeeScheduleHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var query = httpContext.Request.Query;
            var locationId = query.GetQueryParameterValue<string>("location_id");
            var connectionId = Context.ConnectionId;

            Groups.AddToGroupAsync(Context.ConnectionId, locationId);

            return base.OnConnectedAsync();
        }

        public async Task UpdateSchedule(int? locationId) => await Clients.Group(locationId.ToString()).SendAsync(UpdateKey, locationId);
    }

    public class MeetingRoomHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var query = httpContext.Request.Query;
            var location_id = query.GetQueryParameterValue<string>("location_id");
            var connectionId = Context.ConnectionId;

            Groups.AddToGroupAsync(Context.ConnectionId, location_id);

            return base.OnConnectedAsync();
        }
    }

    #region FRS Device Hub
    public class FRSDeviceHub : Hub
    {
        private IUnitOfWork _unitOfWork;
        private IHubContext<FRSHub> _frsHub;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUpDownTimeLogService _upDownTimeLogService;
        private readonly IMapper _mapper;

        public FRSDeviceHub(IUnitOfWork unitOfWork, IHubContext<FRSHub> frsHub, ILoggerFactory loggerFactory, IEmailSender emailSender, IUpDownTimeLogService upDownTimeLogService, IMapper mapper)
        {
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _frsHub = frsHub;
            _upDownTimeLogService = upDownTimeLogService;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<FRSDeviceHub>();
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("FRSDeviceHub OnConnectedAsync START");
            var httpContext = Context.GetHttpContext();

            var query = httpContext.Request.Query;
            string deviceId = query.GetQueryParameterValue<string>("device_id");
            string skipStatus = query.GetQueryParameterValue<string>("skip_status");
            string source = query.GetQueryParameterValue<string>("source");

            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);

            if (string.IsNullOrEmpty(skipStatus))
            {
                var connectionId = Context.ConnectionId;
                _logger.LogInformation(string.Format("DEVICE ID: {0}", deviceId));
                _logger.LogInformation(string.Format("CONNECTION ID: {0}", connectionId));
                _logger.LogInformation(string.Format("SOURCE: {0}", source));

                if (string.IsNullOrEmpty(source))
                {
                    //addToGroupTask.Wait();
                    BaseOperationResponse connectionTask;
                    Device deviceTask;
                    var connections = await _unitOfWork.Connections.FindAsync(e => e.Type == ConnectionType.DEVICE.ToString() && e.Identifier == deviceId).ConfigureAwait(false);
                    var connection = connections.FirstOrDefault();

                    if (connection == null)
                    {
                        _logger.LogInformation(string.Format("DEVICE CONNECTION IS NEW"));
                        connection = new Connection()
                        {
                            ConnectionID = Context.ConnectionId,
                            Identifier = deviceId,
                            Type = ConnectionType.DEVICE.ToString()
                        };
                        connectionTask = await _unitOfWork.Connections.CreateAsync(connection);
                        _logger.LogInformation(string.Format("CONNECTION ADDED TO DB RESULT: IsSuccess ({0})- Message({1})", connectionTask.IsSuccess, connectionTask.Message));
                    }
                    else
                    {
                        _logger.LogInformation(string.Format("DEVICE CONNECTION EXISTS"));
                        connection.IsActive = true;
                        connection.ConnectionID = Context.ConnectionId;
                        connectionTask = await _unitOfWork.Connections.UpdateAsync(connection);
                        _logger.LogInformation(string.Format("CONNECTION UPDATED TO DB RESULT: IsSuccess ({0})- Message({1})", connectionTask.IsSuccess, connectionTask.Message));
                    }

                    //connectionTask.Wait();

                    //refresh pib device list
                    _logger.LogInformation(string.Format("GetByDeviceIdAsync"));

                    //deviceTask = await _unitOfWork.PIBTemplates.GetByDeviceIdAsync(Convert.ToInt32(connection.Identifier));
                    var device = await _unitOfWork.Devices.GetByIdAsync(Convert.ToInt32(connection.Identifier)).ConfigureAwait(false);
                    //deviceTask.Wait();
                    //var device = deviceTask.Result;
                    if (device != null)
                    {
                        await _upDownTimeLogService.CreateAsync(new UpDownTimeLogDTO()
                        {
                            CreatedDate = DateTime.Now,
                            IsUp = true,
                            DeviceIdentifier = device.Code
                        });

                        _logger.LogInformation(string.Format("DEVICE IS ALIVE"));
                        device.device_status = 1;
                        _logger.LogInformation(string.Format("FRSDeviceHub device_status=1 updated from FRSDeviceHub OnConnectedAsync for DEVICE ID {0}.", device.Id));

                        device.last_heartbeat = DateTime.Now;
                        _logger.LogInformation(string.Format("TRYING TO UPDATE DEVICE TO DB"));
                        var updateDeviceTask = await _unitOfWork.Devices.UpdateAsync(device);
                        //updateDeviceTask.Wait();
                        _logger.LogInformation(string.Format("FRSDeviceHub Device UPDATED MESSAGE: {0} - {1}.", updateDeviceTask.IsSuccess, updateDeviceTask.Message));

                        _logger.LogInformation(string.Format("TRYING TO REFRESH DEVICE LIST"));
                        await _frsHub.Clients.All.SendAsync("RefreshPIBDeviceList", _mapper.Map<DeviceViewModel>(device));
                        //task.Wait();
                        _logger.LogInformation(string.Format("DEVICE LIST REFRESH IS INVOKED"));
                    }
                    else
                    {
                        _logger.LogInformation(string.Format("CANNOT FIND DEVICE"));
                    }
                }
                else
                {
                    _logger.LogInformation(string.Format("Connecting the signalr but won't track its status. It's probably from Preview"));
                }
                //Task.Run(async () => {
                //    try
                //    {

                //    }
                //    finally
                //    {

                //    }
                //}).ContinueWith(t => Console.WriteLine(string.Format("Connect task DONE.")));
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("FRSDeviceHub OnDisconnectedAsync START");
            _logger.LogInformation(string.Format("EXCEPTION: {0}", exception != null ? exception.InnerException != null ? exception.InnerException.ToString() : exception.Message : string.Empty));
            BaseOperationResponse connectionTask;
            BaseOperationResponse updateDeviceTask;
            Device deviceTask;

            _logger.LogInformation(string.Format("CONNECTION ID: {0}", Context.ConnectionId));

            var connections = await _unitOfWork.Connections.FindAsync(e => e.Type == ConnectionType.DEVICE.ToString() && e.ConnectionID == Context.ConnectionId);
            var connection = connections.FirstOrDefault();
            if (connection != null)
            {
                _logger.LogInformation(string.Format("CONNECTION FOUND: {0}", connection.Identifier));
                var device = await _unitOfWork.Devices.GetByIdAsync(Convert.ToInt32(connection.Identifier)).ConfigureAwait(false);
                //deviceTask.Wait();

                _logger.LogInformation(string.Format("TRYING TO DELETE CONNECTION FROM THE DB"));
                //var device = deviceTask.Result;
                connectionTask = await _unitOfWork.Connections.DeleteAsync(Context.ConnectionId);
                //connectionTask.Wait();
                _logger.LogInformation(string.Format("CONNECTION DELETED TO DB RESULT: IsSuccess ({0})- Message({1})", connectionTask.IsSuccess, connectionTask.Message));

                if (device != null)
                {
                    await _upDownTimeLogService.CreateAsync(new UpDownTimeLogDTO()
                    {
                        CreatedDate = DateTime.Now,
                        IsUp = false,
                        DeviceIdentifier = device.Code
                    });

                    //refresh pib device list
                    device.device_status = 0;
                    //device.last_heartbeat = null;
                    _logger.LogInformation(string.Format("FRSDeviceHub device_status=0 updated from FRSDeviceHub OnDisconnectedAsync for DEVICE ID {0}.", device.Id));

                    _logger.LogInformation(string.Format("TRYING TO UPDATE DEVICE TO DB"));
                    updateDeviceTask = await _unitOfWork.Devices.UpdateAsync(device);
                    //updateDeviceTask.Wait();
                    _logger.LogInformation(string.Format("FRSDeviceHub Device UPDATED MESSAGE: {0} - {1}.", updateDeviceTask.IsSuccess, updateDeviceTask.Message));

                    _logger.LogInformation(string.Format("TRYING TO REFRESH DEVICE LIST"));
                    var task = _frsHub.Clients.All.SendAsync("RefreshPIBDeviceList", _mapper.Map<DeviceViewModel>(device));
                    //task.Wait();

                    _logger.LogInformation(string.Format("DEVICE LIST REFRESH IS INVOKED"));

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, device.Id.ToString());
                    //removeToGroupTask.Wait();
                    _logger.LogInformation(string.Format("CONNECTION TOTALLY REMOVED"));

                    //InitiateNotification(device);

                    //int delay = 5;

                    //try
                    //{
                    //    var delaysetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("DEVICE_DOWN_NOTIFICATION_DELAY_MINUTES").ConfigureAwait(false);
                    //    if (delaysetting != null) delay = Int32.Parse(delaysetting.Value);
                    //}
                    //catch (Exception ex) { }

                    //_logger.LogInformation(string.Format("WAITTT"));

                    //var usergroups = await _unitOfWork.UserGroups.GetUserGroupsByLocationId(device.LocationId);

                    //_logger.LogInformation(string.Format("device " + device.Id));
                    //_logger.LogInformation(string.Format("device " + device.Code));
                    //_logger.LogInformation(string.Format("location " + device.LocationId + " " + device.LocationName));
                    //_logger.LogInformation(string.Format("UGE " + usergroups.Count()));

                    //await Task.Delay(delay * 60 * 1000);

                    //device = await _unitOfWork.Devices.GetByIdAsync(device.Id).ConfigureAwait(false);

                    //if (device.device_status == 1) return;

                    //_logger.LogInformation(string.Format("WAIT DONEEE"));

                    //usergroups.ForEach(async ug =>
                    //{
                    //    _logger.LogInformation(string.Format("UG " + ug.Name));
                    //    if (!string.IsNullOrWhiteSpace(ug.emails))
                    //    {
                    //        var emails = ug.emails.Split(";");

                    //        foreach (var e in emails)
                    //        {
                    //            try
                    //            {
                    //                _logger.LogInformation(string.Format("eb " + e));

                    //                var deviceIdentifierKey = "{deviceIdentifier}";
                    //                var delayKey = "{delay}";
                    //                var locationNameKey = "{locationName}";
                    //                var ipAddressKey = "{ipAddress}";
                    //                var macAddressKey = "{macAddress}";
                    //                var serialNumberKey = "{serialNumber}";

                    //                var title = $"Device with identifier {deviceIdentifierKey} is down.";
                    //                var content = $"Device with identifier {deviceIdentifierKey} is down more than {delayKey} minutes.\n\n" +
                    //                    $"Device Details:\n" +
                    //                    $"Identifier: {deviceIdentifierKey}\n" +
                    //                    $"Location: {locationNameKey}\n" +
                    //                    $"IP Address: {ipAddressKey}\n" +
                    //                    $"Mac Address: {macAddressKey}\n" +
                    //                    $"Serial: {serialNumberKey}";

                    //                try
                    //                {
                    //                    var titleObj = await _unitOfWork.ApplicationSettings.GetByKeyAsync("DEVICE_DOWN_EMAIL_TITLE").ConfigureAwait(false);
                    //                    var contentObj = await _unitOfWork.ApplicationSettings.GetByKeyAsync("DEVICE_DOWN_EMAIL_BODY").ConfigureAwait(false);

                    //                    if (titleObj != null) title = titleObj.Value;
                    //                    if (contentObj != null) content = contentObj.Value;
                    //                }
                    //                catch (Exception ex) { }

                    //                title = title.Replace(deviceIdentifierKey, device.Code);
                    //                content = content.Replace(deviceIdentifierKey, device.Code);
                    //                content = content.Replace(delayKey, delay + "");
                    //                content = content.Replace(locationNameKey, device.Location?.Name ?? "");
                    //                content = content.Replace(ipAddressKey, device.IpAddress);
                    //                content = content.Replace(macAddressKey, device.MacAddress);
                    //                content = content.Replace(serialNumberKey, device.SerialNumber);


                    //                var emailBody = EmailTemplates.GetPlainTextTestEmail(DateTime.Now);
                    //                var isSuccess = await _emailSender.SendEmailAsync("Device Administrator", e, title, content
                    //                    );

                    //                _logger.LogInformation(string.Format("ee " + e));
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //            }
                    //        }
                    //    }
                    //});
                }

                //_frsHub.Clients.Group(connection.Identifier).SendAsync("RefreshDeviceData", device);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async void InitiateNotification(Device device)
        {
            
        }

        /// <summary>
        /// Initiate the heartbeat callback.
        /// </summary>
        //private void Heartbeat()
        //{
        //    var heartbeat = Context.Features.Get<IConnectionHeartbeatFeature>();

        //    heartbeat.OnHeartbeat(async state =>
        //    {
        //        (HttpContext context, string connectionId) = ((HttpContext, string))state;

        //        var connection = _unitOfWork.Connections.Find(e => e.ConnectionID == connectionId).FirstOrDefault();

        //        if (connection != null)
        //        {
        //            //refresh pib device list
        //            var device = await _unitOfWork.PIBTemplates.GetByDeviceIdAsync(Convert.ToInt32(connection.Identifier));
        //            if (device != null)
        //            {
        //                device.device_status = 1;
        //                device.last_heartbeat = DateTime.Now;
        //            }
        //        }
        //    }, (Context.GetHttpContext(), Context.ConnectionId));
        //}

        public async Task RefreshDeviceData(PIBDeviceViewModel data) => await Clients.Group(data.Id.ToString()).SendAsync("RefreshDeviceData", data);
    }
    #endregion

    #region
    public class PIBDeviceHub : Hub
    {
        private IUnitOfWork _unitOfWork;
        private IHubContext<FRSHub> _frsHub;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUpDownTimeLogService _upDownTimeLogService;
        private readonly IMapper _mapper;
        public PIBDeviceHub(IUnitOfWork unitOfWork, IHubContext<FRSHub> frsHub, ILoggerFactory loggerFactory, IEmailSender emailSender, IUpDownTimeLogService upDownTimeLogService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _frsHub = frsHub;
            _logger = loggerFactory.CreateLogger<PIBDeviceHub>();
            _emailSender = emailSender;
            _upDownTimeLogService = upDownTimeLogService;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("PIBDeviceHub OnConnectedAsync START");
            var httpContext = Context.GetHttpContext();

            var query = httpContext.Request.Query;
            string deviceId = query.GetQueryParameterValue<string>("device_id");
            string skipStatus = query.GetQueryParameterValue<string>("skip_status");

            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);

            if (string.IsNullOrEmpty(skipStatus))
            {
                var connectionId = Context.ConnectionId;
                _logger.LogInformation(string.Format("DEVICE ID: {0}", deviceId));
                _logger.LogInformation(string.Format("CONNECTION ID: {0}", connectionId));


                //addToGroupTask.Wait();
                BaseOperationResponse connectionTask;
                Device deviceTask;
                var connections = await _unitOfWork.Connections.FindAsync(e => e.Type == ConnectionType.PIBDEVICE.ToString() && e.Identifier == deviceId).ConfigureAwait(false);
                var connection = connections.FirstOrDefault();

                if (connection == null)
                {
                    _logger.LogInformation(string.Format("DEVICE CONNECTION IS NEW"));
                    connection = new Connection()
                    {
                        ConnectionID = Context.ConnectionId,
                        Identifier = deviceId,
                        Type = ConnectionType.PIBDEVICE.ToString()
                    };
                    connectionTask = await _unitOfWork.Connections.CreateAsync(connection);
                    _logger.LogInformation(string.Format("CONNECTION ADDED TO DB"));
                }
                else
                {
                    _logger.LogInformation(string.Format("DEVICE CONNECTION EXISTS"));
                    connection.IsActive = true;
                    connection.ConnectionID = Context.ConnectionId;
                    connectionTask = await _unitOfWork.Connections.UpdateAsync(connection);
                    _logger.LogInformation(string.Format("CONNECTION UPDATED TO DB RESULT: IsSuccess ({0})- Message({1})", connectionTask.IsSuccess, connectionTask.Message));
                }

                //connectionTask.Wait();

                //refresh pib device list
                _logger.LogInformation(string.Format("GetByDeviceIdAsync"));

                //deviceTask = await _unitOfWork.PIBTemplates.GetByDeviceIdAsync(Convert.ToInt32(connection.Identifier));
                var device = await _unitOfWork.Devices.GetByIdAsync(Convert.ToInt32(connection.Identifier)).ConfigureAwait(false);
                //deviceTask.Wait();
                //var device = deviceTask.Result;
                if (device != null)
                {
                    await _upDownTimeLogService.CreateAsync(new UpDownTimeLogDTO()
                    {
                        CreatedDate = DateTime.Now,
                        IsUp = true,
                        DeviceIdentifier = device.Code
                    });

                    _logger.LogInformation(string.Format("DEVICE IS ALIVE"));
                    device.device_status = 1;
                    _logger.LogInformation(string.Format("PIBDeviceHub device_status=1 updated from pibDeviceHub OnConnectedAsync for DEVICE ID {0}.", device.Id));

                    device.last_heartbeat = DateTime.Now;
                    _logger.LogInformation(string.Format("TRYING TO UPDATE DEVICE TO DB"));
                    var updateDeviceTask = await _unitOfWork.Devices.UpdateAsync(device);
                    //updateDeviceTask.Wait();
                    _logger.LogInformation(string.Format("PIBDeviceHub Device UPDATED MESSAGE: {0} - {1}.", updateDeviceTask.IsSuccess, updateDeviceTask.Message));

                    _logger.LogInformation(string.Format("TRYING TO REFRESH DEVICE LIST"));
                    await _frsHub.Clients.All.SendAsync("RefreshPIBDeviceList", _mapper.Map<DeviceViewModel>(device));
                    //task.Wait();
                    _logger.LogInformation(string.Format("DEVICE LIST REFRESH IS INVOKED"));
                }
                else
                {
                    _logger.LogInformation(string.Format("CANNOT FIND DEVICE"));
                }

                //Task.Run(async () => {
                //    try
                //    {

                //    }
                //    finally
                //    {

                //    }
                //}).ContinueWith(t => Console.WriteLine(string.Format("Connect task DONE.")));
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("PIBDeviceHub OnDisconnectedAsync START");
            _logger.LogInformation(string.Format("EXCEPTION: {0}", exception != null ? exception.InnerException != null ? exception.InnerException.ToString() : exception.Message : string.Empty));
            BaseOperationResponse connectionTask;
            BaseOperationResponse updateDeviceTask;
            Device deviceTask;

            _logger.LogInformation(string.Format("CONNECTION ID:", Context.ConnectionId));

            var connections = await _unitOfWork.Connections.FindAsync(e => e.Type == ConnectionType.PIBDEVICE.ToString() && e.ConnectionID == Context.ConnectionId);
            var connection = connections.FirstOrDefault();
            if (connection != null)
            {
                _logger.LogInformation(string.Format("CONNECTION FOUND:", connection.Identifier));
                var device = await _unitOfWork.Devices.GetByIdAsync(Convert.ToInt32(connection.Identifier)).ConfigureAwait(false);
                //deviceTask.Wait();

                _logger.LogInformation(string.Format("TRYING TO DELETE CONNECTION FROM THE DB"));
                //var device = deviceTask.Result;
                connectionTask = await _unitOfWork.Connections.DeleteAsync(Context.ConnectionId);
                //connectionTask.Wait();
                _logger.LogInformation(string.Format("CONNECTION DELETED"));

                if (device != null)
                {
                    await _upDownTimeLogService.CreateAsync(new UpDownTimeLogDTO()
                    {
                        CreatedDate = DateTime.Now,
                        IsUp = false,
                        DeviceIdentifier = device.Code
                    });

                    //refresh pib device list
                    device.device_status = 0;
                    _logger.LogInformation(string.Format("PIBDeviceHub device_status=0 updated from pibDeviceHub OnDisconnectedAsync for DEVICE ID {0}.", device.Id));

                    //device.last_heartbeat = null;
                    _logger.LogInformation(string.Format("TRYING TO UPDATE DEVICE TO DB"));
                    updateDeviceTask = await _unitOfWork.Devices.UpdateAsync(device);
                    //updateDeviceTask.Wait();
                    _logger.LogInformation(string.Format("PIBDeviceHub Device UPDATED MESSAGE: {0} - {1}.", updateDeviceTask.IsSuccess, updateDeviceTask.Message));

                    _logger.LogInformation(string.Format("TRYING TO REFRESH DEVICE LIST"));
                    var task = _frsHub.Clients.All.SendAsync("RefreshPIBDeviceList", _mapper.Map<DeviceViewModel>(device));
                    //task.Wait();

                    _logger.LogInformation(string.Format("DEVICE LIST REFRESH IS INVOKED"));

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, device.Id.ToString());
                    //removeToGroupTask.Wait();
                    _logger.LogInformation(string.Format("CONNECTION TOTALLY REMOVED"));

                    //InitiateNotification(device);

                    //int delay = 5;

                    //try
                    //{
                    //    var delaysetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("DEVICE_DOWN_NOTIFICATION_DELAY_MINUTES").ConfigureAwait(false);
                    //    if (delaysetting != null) delay = Int32.Parse(delaysetting.Value);
                    //}
                    //catch (Exception ex) { }

                    //_logger.LogInformation(string.Format("WAITTT"));

                    //var usergroups = await _unitOfWork.UserGroups.GetUserGroupsByLocationId(device.LocationId);

                    //_logger.LogInformation(string.Format("device " + device.Id));
                    //_logger.LogInformation(string.Format("device " + device.Code));
                    //_logger.LogInformation(string.Format("location " + device.LocationId + " " + device.LocationName));
                    //_logger.LogInformation(string.Format("UGE " + usergroups.Count()));

                    //await Task.Delay(delay * 60 * 1000);

                    //device = await _unitOfWork.Devices.GetByIdAsync(device.Id).ConfigureAwait(false);

                    //if (device.device_status == 1) return;

                    //_logger.LogInformation(string.Format("WAIT DONEEE"));

                    //usergroups.ForEach(async ug =>
                    //{
                    //    _logger.LogInformation(string.Format("UG " + ug.Name));
                    //    if (!string.IsNullOrWhiteSpace(ug.emails))
                    //    {
                    //        var emails = ug.emails.Split(";");

                    //        foreach (var e in emails)
                    //        {
                    //            try
                    //            {
                    //                _logger.LogInformation(string.Format("eb " + e));

                    //                var deviceIdentifierKey = "{deviceIdentifier}";
                    //                var delayKey = "{delay}";
                    //                var locationNameKey = "{locationName}";
                    //                var ipAddressKey = "{ipAddress}";
                    //                var macAddressKey = "{macAddress}";
                    //                var serialNumberKey = "{serialNumber}";

                    //                var title = $"Device with identifier {deviceIdentifierKey} is down.";
                    //                var content = $"Device with identifier {deviceIdentifierKey} is down more than {delayKey} minutes.\n\n" +
                    //                    $"Device Details:\n" +
                    //                    $"Identifier: {deviceIdentifierKey}\n" +
                    //                    $"Location: {locationNameKey}\n" +
                    //                    $"IP Address: {ipAddressKey}\n" +
                    //                    $"Mac Address: {macAddressKey}\n" +
                    //                    $"Serial: {serialNumberKey}";

                    //                try
                    //                {
                    //                    var titleObj = await _unitOfWork.ApplicationSettings.GetByKeyAsync("DEVICE_DOWN_EMAIL_TITLE").ConfigureAwait(false);
                    //                    var contentObj = await _unitOfWork.ApplicationSettings.GetByKeyAsync("DEVICE_DOWN_EMAIL_BODY").ConfigureAwait(false);

                    //                    if (titleObj != null) title = titleObj.Value;
                    //                    if (contentObj != null) content = contentObj.Value;
                    //                }
                    //                catch (Exception ex) { }

                    //                title = title.Replace(deviceIdentifierKey, device.Code);
                    //                content = content.Replace(deviceIdentifierKey, device.Code);
                    //                content = content.Replace(delayKey, delay + "");
                    //                content = content.Replace(locationNameKey, device.Location?.Name ?? "");
                    //                content = content.Replace(ipAddressKey, device.IpAddress);
                    //                content = content.Replace(macAddressKey, device.MacAddress);
                    //                content = content.Replace(serialNumberKey, device.SerialNumber);


                    //                var emailBody = EmailTemplates.GetPlainTextTestEmail(DateTime.Now);
                    //                var isSuccess = await _emailSender.SendEmailAsync("Device Administrator", e, title, content
                    //                    );

                    //                _logger.LogInformation(string.Format("ee " + e));
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //            }
                    //        }
                    //    }
                    //});
                }

                //_frsHub.Clients.Group(connection.Identifier).SendAsync("RefreshDeviceData", device);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async void InitiateNotification(Device device)
        {
            //int delay = 5;

            //try
            //{
            //    var delaysetting = await _unitOfWork.ApplicationSettings.GetByKeyAsync("DEVICE_DOWN_NOTIFICATION_DELAY_MINUTES").ConfigureAwait(false);
            //    if (delaysetting != null) delay = Int32.Parse(delaysetting.Value);
            //}
            //catch (Exception ex) { }


            //await Task.Delay(delay * 60 * 1000);

            //var usergroups = await _unitOfWork.UserGroups.GetUserGroupsByLocationId(device.LocationId);

            //usergroups.ForEach(async ug =>
            //{
            //    if (!string.IsNullOrWhiteSpace(ug.emails))
            //    {
            //        var emails = ug.emails.Split(";");

            //        foreach (var e in emails)
            //        {
            //            try
            //            {
            //                var emailBody = EmailTemplates.GetPlainTextTestEmail(DateTime.Now);
            //                var isSuccess = await _emailSender.SendEmailAsync("Device Administrator", e, $"Device with identifier {device.Code} is down.",
            //                    $"Device with identifier {device.Code} is down more than {delay} minutes.\n\n" +
            //                    $"Device Details:\n" +
            //                    $"Identifier: {device.Code}\n" +
            //                    $"Location: {device.Location.Name}\n" +
            //                    $"IP Address: {device.IpAddress}\n" +
            //                    $"Mac Address: {device.MacAddress}\n" +
            //                    $"Serial: {device.SerialNumber}"

            //                    );
            //            }
            //            catch (Exception ex)
            //            {
            //            }
            //        }
            //    }
            //});
        }
    }

    #endregion
    static public class SignalrExtensions
    {
        static public HttpContext GetHttpContext(this HubCallerContext context) =>
           context
             ?.Features
             .Select(x => x.Value as IHttpContextFeature)
             .FirstOrDefault(x => x != null)
             ?.HttpContext;

        static public T GetQueryParameterValue<T>(this IQueryCollection httpQuery, string queryParameterName) =>
           httpQuery.TryGetValue(queryParameterName, out var value) && value.Any()
             ? (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T))
             : default(T);
    }
}
