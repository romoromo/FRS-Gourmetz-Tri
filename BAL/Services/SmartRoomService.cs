using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL;
using BAL.Services.Interfaces;
using BAL.DTO;
using AutoMapper;
using DAL.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using DAL.Core.DTO;
using System.Net;
using FRS.BAL;
using System.IO;

namespace BAL.Services
{
    public class SmartRoomService : ISmartRoomService
    {
        private IUnitOfWork _uow;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public SmartRoomService(IUnitOfWork uow, IConfiguration configuration, IMapper mapper)
        {
            this._uow = uow;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<BaseOperationResponse> GetListOfSmartRoomResourcesAsync(SMARTRoomXML tokenResponse = null, bool isReadByFile = false)
        {
            var response = new BaseOperationResponse();
            string roomURL = "";
            try
            {
                var appSetting = await this._uow.ApplicationSettings.GetByKeyAsync("SMARTROOM_API_ROOM");
                if (appSetting != null)
                {
                    roomURL = appSetting.Value;
                }

                if (tokenResponse == null)
                {
                    var token = await GetSmartRoomToken();
                    if (token.IsSuccess)
                    {
                        tokenResponse = token.Data as SMARTRoomXML;
                    }
                }

                if (tokenResponse != null && tokenResponse.ApplicationData != null)
                {

                    if (isReadByFile)
                    {
                        string filePath = string.Empty;
                        appSetting = await this._uow.ApplicationSettings.GetByKeyAsync("SMARTROOM_API_RESOURCES_FILE_PATH");
                        if (appSetting != null)
                        {
                            filePath = appSetting.Value;
                        }

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            string fullResponse = System.IO.File.ReadAllText(filePath);
                            response.Data = fullResponse.DeserializeXML<SMARTRoomXML>();
                            response.Message = string.Format("Successful call to SMARTRoom API {0}", roomURL);
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = string.Format("Failed to find file path {0}.", filePath);
                        }
                        
                    }
                    else
                    {
                        Dictionary<string, object> postParameters = new Dictionary<string, object>();
                        string userAgent = "FRS";
                        WebHeaderCollection headers = new WebHeaderCollection();
                        headers.Add("Authorization", tokenResponse.ApplicationData.AppUserToken.AccessToken);

                        HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(roomURL, userAgent, postParameters, headers);

                        // Process response
                        using (var responseReader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            string fullResponse = await responseReader.ReadToEndAsync();
                            tokenResponse = fullResponse.DeserializeXML<SMARTRoomXML>();

                            response.Data = tokenResponse;
                            response.Message = string.Format("Successful call to SMARTRoom API {0}", roomURL);
                        }

                        webResponse.Close();
                        response.IsSuccess = true;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to Authenticate";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = string.Format("There is a problem calling SMARTRoom API {0}, {1}", roomURL, ex.Message);
            }
            return response;
        }

        public async Task<BaseOperationResponse> GetListOfSmartRoomSchedulesAsync(SMARTRoomXML tokenResponse = null, bool isReadByFile = false)
        {
            var response = new BaseOperationResponse();
            string roomURL = "";
            try
            {
                var appSetting = await this._uow.ApplicationSettings.GetByKeyAsync("SMARTROOM_API_ROOM_SCHEDULE");
                if (appSetting != null)
                {
                    roomURL = appSetting.Value;
                }

                if (tokenResponse == null)
                {
                    var token = await GetSmartRoomToken();
                    if (token.IsSuccess)
                    {
                        tokenResponse = token.Data as SMARTRoomXML;
                    }
                }

                if (tokenResponse != null && tokenResponse.ApplicationData != null)
                {
                    if (isReadByFile)
                    {
                        string filePath = string.Empty;
                        appSetting = await this._uow.ApplicationSettings.GetByKeyAsync("SMARTROOM_API_SCHEDULES_FILE_PATH");
                        if (appSetting != null)
                        {
                            filePath = appSetting.Value;
                        }

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            string fullResponse = System.IO.File.ReadAllText(filePath);
                            response.Data = fullResponse.DeserializeXML<SMARTRoomXML>();
                            response.Message = string.Format("Successful call to SMARTRoom API {0}", roomURL);
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = string.Format("Failed to find file path {0}.", filePath);
                        }

                    }
                    else
                    {
                        Dictionary<string, object> postParameters = new Dictionary<string, object>();
                        string userAgent = "FRS";
                        WebHeaderCollection headers = new WebHeaderCollection();
                        headers.Add("Authorization", tokenResponse.ApplicationData.AppUserToken.AccessToken);

                        HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(roomURL, userAgent, postParameters, headers);

                        // Process response
                        using (var responseReader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            string fullResponse = await responseReader.ReadToEndAsync();
                            tokenResponse = fullResponse.DeserializeXML<SMARTRoomXML>();

                            response.Data = tokenResponse;
                            response.Message = string.Format("Successful call to SMARTRoom API {0}", roomURL);
                        }

                        webResponse.Close();
                        response.IsSuccess = true;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to Authenticate";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = string.Format("There is a problem calling SMARTRoom API {0}, {1}", roomURL, ex.Message);
            }

            return response;
        }

        public async Task<BaseOperationResponse> GetSmartRoomToken(bool isReadByFile = false)
        {
            BaseOperationResponse authResponse = new BaseOperationResponse();
            try
            {
                string tokenURL = "";
                var appSetting = await this._uow.ApplicationSettings.GetByKeyAsync("SMARTROOM_API_TOKEN");
                if (appSetting != null)
                {
                    tokenURL = appSetting.Value;
                }

                if (isReadByFile)
                {
                    string filePath = string.Empty;
                    appSetting = await this._uow.ApplicationSettings.GetByKeyAsync("SMARTROOM_API_TOKEN_FILE_PATH");
                    if (appSetting != null)
                    {
                        filePath = appSetting.Value;
                    }

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        string fullResponse = System.IO.File.ReadAllText(filePath);
                        authResponse.Data = fullResponse.DeserializeXML<SMARTRoomXML>();
                        authResponse.Message = string.Format("Successful call to SMARTRoom API {0}", tokenURL);
                        authResponse.IsSuccess = true;
                    }
                    else
                    {
                        authResponse.IsSuccess = false;
                        authResponse.Message = string.Format("Failed to find file path {0}.", filePath);
                    }

                }
                else
                {
                    //string userId = _configuration["AppSettings:SMARTROOM_API_USERID"];
                    //string password = _configuration["AppSettings:SMARTROOM_API_PASS"];
                    var smartId= await this._uow.ApplicationSettings.GetByKeyAsync("SMARTROOM_API_USERID");
                    var smartPass = await this._uow.ApplicationSettings.GetByKeyAsync("SMARTROOM_API_PASS");

                    string userId = smartId.Value;
                    string password = smartPass.Value;

                    Dictionary<string, object> postParameters = new Dictionary<string, object>();
                    postParameters.Add("UserID", userId);
                    postParameters.Add("Password", password);
                    string userAgent = "FRS";
                    HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(tokenURL, userAgent, postParameters);

                    // Process response
                    using (var responseReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string fullResponse = await responseReader.ReadToEndAsync();
                        authResponse.Data = fullResponse.DeserializeXML<SMARTRoomXML>();
                    }

                    webResponse.Close();
                    authResponse.Message = "Authenticated. Token Received.";
                    authResponse.IsSuccess = true;
                }
                
            }
            catch (Exception ex)
            {
                authResponse.IsSuccess = false;
                authResponse.Message = string.Format("There is a problem authenticating FRS to SMARTRoom API, {0}", ex.Message);
            }

            return authResponse;
        }

        public async Task<PagedEntity<SmartRoomSchedulerLogDTO>> GetSchedulerLogs(BaseFilter filter)
        {
            return _mapper.Map<PagedEntity<SmartRoomSchedulerLogDTO>>(await _uow.SmartRoomSchedulerLogs.GetSmartRoomSchedulerLogsAsync(filter));
        }

        public async Task<BaseOperationResponse> CreateSchedulerLog(SmartRoomSchedulerLogDTO log)
        {
            return await this._uow.SmartRoomSchedulerLogs.CreateAsync(_mapper.Map<SmartRoomSchedulerLog>(log));
        }

        public async Task<BaseOperationResponse> BulkCreateSchedulerLogs(List<SmartRoomSchedulerLogDTO> logs)
        {
            return await this._uow.SmartRoomSchedulerLogs.BulkCreateAsync(_mapper.Map<List<SmartRoomSchedulerLog>>(logs));
        }
    }
}
