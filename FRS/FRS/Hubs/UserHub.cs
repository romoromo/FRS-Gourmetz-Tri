using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Core.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FRS.Hubs
{
    public class UserHub : Hub
    {
        private IUnitOfWork _unitOfWork;
        private IHubContext<UserHub> _userHub;
        private readonly IAccountManager _accountManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public UserHub(IUnitOfWork unitOfWork, IHubContext<UserHub> userHub, ILoggerFactory loggerFactory, IAccountManager accountManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userHub = userHub;
            _accountManager = accountManager;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<UserHub>();
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("UserHub OnConnectedAsync START");
            var httpContext = Context.GetHttpContext();

            var query = httpContext.Request.Query;
            string email = query.GetQueryParameterValue<string>("email");
            string status = query.GetQueryParameterValue<string>("status");

            if (string.IsNullOrEmpty(email))
            {
                throw new HubException("{email} needs to have a value.");
            }

            //if (string.IsNullOrEmpty(status))
            //{
            //    throw new HubException("{status} needs to have a value.");
            //}

            bool enableUserSignalrConnection = _configuration["AppSettings:ENABLE_SIGNALR_USER_CONNECTION"] == "Y";

            if (enableUserSignalrConnection)
            {
                var user = await _accountManager.GetUserByEmailAsync(email);

                if (user == null)
                {
                    throw new HubException("User is not found in the database.");
                }

                if (string.IsNullOrEmpty(status))
                {
                    status = !string.IsNullOrEmpty(user.Status) ? user.Status : UserConnectionStatus.OFFDUTY.ToString();
                }
            
                var connectionId = Context.ConnectionId;
                _logger.LogInformation(string.Format("USER ID: {0}", user.Id));
                _logger.LogInformation(string.Format("CONNECTION ID: {0}", connectionId));

                BaseOperationResponse connectionTask;
                var connections = await _unitOfWork.UserConnections.FindAsync(e => e.Type == ConnectionType.USER.ToString() && e.ConnectionID == connectionId && e.UserId == user.Id);
                var connection = connections.FirstOrDefault();

                if (connection == null)
                {
                    _logger.LogInformation(string.Format("USER CONNECTION IS NEW"));
                    connection = new UserConnection()
                    {
                        ConnectionID = Context.ConnectionId,
                        Identifier = email,
                        UserId = user.Id,
                        Status = status,
                        Type = ConnectionType.USER.ToString()
                    };
                    connectionTask = await _unitOfWork.UserConnections.CreateAsync(connection);
                    _logger.LogInformation(string.Format("CONNECTION ADDED TO DB"));
                }
                else
                {
                    _logger.LogInformation(string.Format("USER CONNECTION EXISTS"));
                    connection.IsActive = true;
                    connection.ConnectionID = Context.ConnectionId;
                    connectionTask = await _unitOfWork.UserConnections.UpdateAsync(connection);
                    _logger.LogInformation(string.Format("CONNECTION UPDATED TO DB"));
                }

                _logger.LogInformation(string.Format("Create/Update IsSuccess: {0}", connectionTask.IsSuccess));

                //Update User status
                _logger.LogInformation(string.Format("UPDATE USER STATUS"));
                user.Status = status;
                user.IsConnected = true;
                var userTask = await _accountManager.UpdateUserAsync(user);
                _logger.LogInformation(string.Format("User status update IsSuccess: {0}", userTask.Item1));
                if (userTask.Item1)
                {
                    _logger.LogInformation(string.Format("User status update IsSuccess: {0}", userTask.Item1));
                }
                else
                {
                    _logger.LogInformation(string.Format("User status update Error: {0}", userTask.Item2));
                }

                if (connectionTask.IsSuccess)
                {
                    _logger.LogInformation(string.Format("BroadcastUserStatusChange"));
                    var activeConnections = await _unitOfWork.UserConnections.FindAsync(e => e.Type == ConnectionType.USER.ToString() && e.Identifier == email && e.IsActive);
                    await Clients.All.SendAsync("BroadcastUserStatusChange", new { id = connection.UserId, email = connection.Identifier, status = status, isConnected = activeConnections.Any() });
                }
                else
                {
                    _logger.LogInformation(string.Format("User Connection Create/Update Error: {0}", connectionTask.Message));
                }
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, email);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("UserHub OnDisconnectedAsync START");
            _logger.LogInformation(string.Format("(DISCONNECTED) EXCEPTION: {0}", exception != null ? exception.InnerException != null ? exception.InnerException.ToString() : exception.Message : string.Empty));
            BaseOperationResponse connectionTask;
            _logger.LogInformation(string.Format("(DISCONNECTED) CONNECTION ID:", Context.ConnectionId));

            bool enableUserSignalrConnection = _configuration["AppSettings:ENABLE_SIGNALR_USER_CONNECTION"] == "Y";
            if (enableUserSignalrConnection)
            {
                var connections = await _unitOfWork.UserConnections.FindAsync(e => e.Type == ConnectionType.USER.ToString() && e.ConnectionID == Context.ConnectionId);
                var connection = connections.FirstOrDefault();
                if (connection != null)
                {
                    _logger.LogInformation(string.Format("(DISCONNECTED) CONNECTION FOUND - UserID {0}:", connection.UserId.ToString()));
                    _logger.LogInformation(string.Format("(DISCONNECTED) TRYING TO DELETE CONNECTION FROM THE DB"));
                    connectionTask = await _unitOfWork.UserConnections.DeleteAsync(Context.ConnectionId);

                    if (connectionTask.IsSuccess)
                    {
                        _logger.LogInformation(string.Format("(DISCONNECTED) CONNECTION DELETED"));
                    }
                    else
                    {
                        _logger.LogInformation(string.Format("(DISCONNECTED) User Connection Delete Error: {0}", connectionTask.Message));
                    }

                    var user = await _accountManager.GetUserByEmailAsync(connection.Identifier);
                    string status = UserConnectionStatus.OFFDUTY.ToString(); //just to have a default value

                    if (user != null)
                    {
                        //if (user.Status == UserConnectionStatus.OFFLINE.ToString() || user.Status == UserConnectionStatus.ONLINE.ToString())
                        //{
                        //    status = UserConnectionStatus.OFFDUTY.ToString();
                        //}
                        //else
                        //{
                        //    status = user.Status;
                        //}
                        status = user.Status;
                        //get if there are active sessions
                        connections = await _unitOfWork.UserConnections.FindAsync(e => e.Type == ConnectionType.USER.ToString() && e.Identifier == user.Email && e.IsActive);
                        user.IsConnected = false;
                        //update status to offline only if there are no active sessions //uncomment if user status default status is OFFLINE
                        //if (connections.Count() == 0)
                        //{
                        //    _logger.LogInformation(string.Format("(DISCONNECTED) UPDATE USER STATUS"));
                        //    //user.Status = UserConnectionStatus.OFFLINE.ToString(); //uncomment to retain the status
                        var userTask = await _accountManager.UpdateUserAsync(user);
                        _logger.LogInformation(string.Format("(DISCONNECTED) User status update IsSuccess: {0}", userTask.Item1));
                        if (userTask.Item1)
                        {
                            _logger.LogInformation(string.Format("(DISCONNECTED) User status update IsSuccess: {0}", userTask.Item1));
                        }
                        else
                        {
                            _logger.LogInformation(string.Format("(DISCONNECTED) User status update Error: {0}", userTask.Item2));
                        }
                        //}

                    }

                    var activeConnections = await _unitOfWork.UserConnections.FindAsync(e => e.Type == ConnectionType.USER.ToString() && e.Identifier == connection.Identifier && e.IsActive);

                    _logger.LogInformation(string.Format("(DISCONNECTED) TRYING TO Invoke BroadcastUserStatusChange"));
                    //await _userHub.Clients.All.SendAsync("BroadcastUserStatusChange", new { Id= connection.UserId, status = UserConnectionStatus.OFFLINE.ToString() });
                    await Clients.Others.SendAsync("BroadcastUserStatusChange", new { id = connection.UserId, email = connection.Identifier, status = status, isConnected = activeConnections.Any() });
                    _logger.LogInformation(string.Format("(DISCONNECTED) BroadcastUserStatusChange IS INVOKED"));
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateStatus(string email, string status)
        {
            try
            {
                
                if (string.IsNullOrEmpty(email))
                {
                    throw new HubException("{email} needs to have a value.");
                }

                bool enableUserStatusUpdateSignalrConnection = _configuration["AppSettings:ENABLE_SIGNALR_USER_STATUS_UPDATE"] == "Y";
                if (enableUserStatusUpdateSignalrConnection)
                {
                    var user = await _accountManager.GetUserByEmailAsync(email);

                    if (user == null)
                    {
                        throw new HubException("User is not found in the database.");
                    }

                    user.Status = status;
                    await _accountManager.UpdateUserAsync(user);

                    var connections = await _unitOfWork.UserConnections.FindAsync(e => e.Type == ConnectionType.USER.ToString() && e.Identifier == email && e.IsActive);
                    var activeConnections = connections.ToList();
                    await _unitOfWork.UserConnections.UpdateRangeStatusAsync(activeConnections, status);

                    _logger.LogInformation(string.Format("(UpdateStatus) TRYING TO Invoke BroadcastUserStatusChange"));
                    await Clients.All.SendAsync("BroadcastUserStatusChange", new { id = user.Id, email = email, status = status, isConnected = activeConnections.Any() });
                    _logger.LogInformation(string.Format("(UpdateStatus) BroadcastUserStatusChange IS INVOKED"));
                }
            }
            catch (Exception ex)
            {
                throw new HubException("There is a problem updating the user status." + ex.Message);
            }
        }
    }
}
