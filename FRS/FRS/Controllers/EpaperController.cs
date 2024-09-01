using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
using Newtonsoft.Json.Linq;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EpaperController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private IHubContext<FRSHub> _frsHub;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public EpaperController(IUnitOfWork unitOfWork, ILogger<EpaperController> logger, IConfiguration configuration, IHubContext<FRSHub> frsHub, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _frsHub = frsHub;
            _mapper = mapper;
        }

        /// <summary>
        /// API calls to get epaperTemplates
        /// </summary>
        /// <param name="epaperTemplateId"></param>
        /// <returns>List of epaperTemplates</returns>
        [HttpGet("get/template")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiEpaperTemplates(int? templateId = null, string mac = null)
        {
            var result = await _unitOfWork.EpaperTemplates.GetApiEpaperTemplates(templateId, mac);
            var data = _mapper.Map<List<EpaperTemplateViewModel>>(result.Data);

            if (templateId.HasValue || !string.IsNullOrEmpty(mac))
            {
                result.Data = data.Any() ? data.First() : null;
            }
            else
            {
                result.Data = data;
            }

            return Ok(result);
        }

        [HttpGet("templates/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EpaperTemplateViewModel>))]
        public async Task<IActionResult> GetEpaperTemplates()
        {
            return await GetEpaperTemplates(-1, -1);
        }


        [HttpGet("templates/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EpaperTemplateViewModel>))]
        public async Task<IActionResult> GetEpaperTemplates(int pageNumber, int pageSize)
        {
            var results = await _unitOfWork.EpaperTemplates.GetEpaperTemplatesLoadRelatedAsync(pageNumber, pageSize);
            return Ok(_mapper.Map<List<EpaperTemplateViewModel>>(results));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllEpaperTemplatesPolicy)]
        [ProducesResponseType(201, Type = typeof(EpaperTemplateViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEpaperTemplate([FromBody] EpaperTemplateViewModel epaperTemplate)
        {
            if (ModelState.IsValid)
            {
                if (epaperTemplate == null)
                    return BadRequest($"{nameof(epaperTemplate)} cannot be null");

                string deviceAPIUrl = _configuration["AppSettings:epaperDeviceUrl"];
                if (string.IsNullOrEmpty(epaperTemplate.DeviceAPIUrl))
                {
                    epaperTemplate.DeviceAPIUrl = deviceAPIUrl;
                }

                var type = _mapper.Map<EpaperTemplate>(epaperTemplate);

                var result = await _unitOfWork.EpaperTemplates.CreateAsync(type);
                if (result.IsSuccess)
                {
                    EpaperTemplateViewModel epaperTemplateVM = _mapper.Map<EpaperTemplateViewModel>(result.Data);
                    return CreatedAtAction("GetEpaperTemplateById", new { id = epaperTemplateVM.Id }, epaperTemplateVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEpaperTemplatesPolicy)]
        [ProducesResponseType(200, Type = typeof(EpaperTemplateViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEpaperTemplate(int id)
        {
            if (!await _unitOfWork.EpaperTemplates.TestCanDeleteAsync(id))
                return BadRequest("EpaperTemplate cannot be deleted."); //TODO: correct message here


            var epaperTemplate = await this._unitOfWork.EpaperTemplates.GetByIdAsync(id);

            EpaperTemplateViewModel epaperTemplateVM = _mapper.Map<EpaperTemplateViewModel>(epaperTemplate);
            if (epaperTemplateVM == null)
                return NotFound(id);

            var result = await _unitOfWork.EpaperTemplates.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting epaperTemplate: " + string.Join(", ", result.Message));


            return Ok(epaperTemplateVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEpaperTemplatesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEpaperTemplate(string id, [FromBody] EpaperTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var epaperTemplate = await this._unitOfWork.EpaperTemplates.GetByIdAsync(model.Id);

                EpaperTemplateViewModel epaperTemplateVM = _mapper.Map<EpaperTemplateViewModel>(epaperTemplate);
                if (epaperTemplateVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<EpaperTemplate>(model);
                var result = await _unitOfWork.EpaperTemplates.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        //[HttpGet("get/epaperlocations")]
        //[AllowAnonymous]
        ////[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        //[ProducesResponseType(200, Type = typeof(List<EpaperTemplateLocationViewModel>))]
        //public async Task<IActionResult> GetLocations()
        //{
        //    var results = await _unitOfWork.EpaperTemplates.GetEpaperLocationsLoadRelatedAsync(-1, -1);
        //    return Ok(_mapper.Map<List<EpaperTemplateLocationViewModel>>(results));
        //}

        //[HttpGet("get/epaperlocations/sync")]
        //[AllowAnonymous]
        ////[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        //[ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        //public async Task<IActionResult> SyncLocations()
        //{
        //    string apiUrl = _configuration["AppSettings:epaperLocationApiUrl"];
        //    var results = new BaseOperationResponse();
        //    try
        //    {
        //        var locations = GetApiLocations(apiUrl);
        //        results = await _unitOfWork.EpaperTemplates.SyncLocationsAsync(locations);
        //    }
        //    catch (Exception ex)
        //    {
        //        results.IsSuccess = false;
        //        results.Data = null;
        //        results.Message = ex.Message;
        //    }

        //    return Ok(results);
        //}
        [HttpPost("map/posttodevice")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> PostImageToDevice([FromBody] EpaperTemplateViewModel template)
        {
            var result = new BaseOperationResponse();

            try
            {
                if (template.IsStaticLink && !string.IsNullOrEmpty(template.MacAddress))
                {
                    if (string.IsNullOrEmpty(template.DeviceImageAPIUrl))
                        throw new Exception("API URL is not set in Epaper Device.");
                }
                else
                {
                    string deviceAPIUrl = _configuration["AppSettings:epaperDeviceImageUrl"];
                    if (string.IsNullOrEmpty(template.DeviceImageAPIUrl))
                    {
                        template.DeviceImageAPIUrl = deviceAPIUrl;
                    }
                }

                var base64Image = template.ImgUrl;
                var offset = base64Image.Substring(base64Image.IndexOf(',') + 1);

                var imageInBytes = Convert.FromBase64String(offset);
                var ms = new MemoryStream(imageInBytes);
                //var img = Image.FromStream(ms);
                //img.Save(@"C:\test\images\sheila.png");

                // Generate post objects
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                postParameters.Add("name", "eImage.png");
                //postParameters.Add("file", new FormUpload.FileParameter(data, "template.png", "image/png"));
                //postParameters.Add("eImage", new FormUpload.FileParameter(data, "template.png", "image/png"));
                postParameters.Add("eImage", new FormUpload.FileParameter(imageInBytes, "template.png", "image/png"));

                // Create request and receive response
                string postURL = template.DeviceImageAPIUrl;//"http://183.90.63.88:3000/epaper"; ;
                string userAgent = "Someone";
                try
                {
                    HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, userAgent, postParameters);

                    // Process response
                    StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                    string fullResponse = responseReader.ReadToEnd();
                    webResponse.Close();
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("There is a problem posting the image to the device, {0}", ex.Message);
                }


            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                throw;
            }
            return Ok(result);
        }

        [HttpGet("heartbeat")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> Heartbeat(string mac_address, string device_code)
        {
            var result = await _unitOfWork.EpaperTemplates.UpdateDeviceStatus(mac_address, device_code);
            dynamic data = result.Data;
            var isNew = data.GetType().GetProperty("isNew").GetValue(data, null);

            if (isNew)
            {
                var objectData = data.GetType().GetProperty("data").GetValue(data, null);
                await _frsHub.Clients.All.SendAsync("RefreshEpaperDeviceList", _mapper.Map<EpaperDeviceViewModel>(objectData as EpaperDevice));
            }

            return Ok(result);
        }

        [HttpGet("display/epaper")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> DisplayEPaper(string mac_address)
        {
            var result = await _unitOfWork.EpaperTemplates.GetApiEpaperDevices(macAddress: mac_address);
            if (!string.IsNullOrEmpty(mac_address))
            {
                var deviceData = _mapper.Map<List<EpaperDeviceViewModel>>(result.Data);

                if (deviceData.Any())
                {
                    var device = deviceData.First();
                    var templateResult = await _unitOfWork.EpaperTemplates.GetApiEpaperTemplates(macAddress: mac_address);
                    var templateData = _mapper.Map<List<EpaperTemplateViewModel>>(templateResult.Data);

                    if (templateData.Any())
                    {
                        var template = templateData.First();
                        template.DeviceImageAPIUrl = device.epaper_url;

                        try
                        {
                            var filter = new CalendarFilter
                            {
                                //Start = start,
                                //End = end,
                                locationIds = device.location_id.HasValue ? new List<int> { (int)device.location_id } : template.LocationIds
                            };

                            filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                            filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                            LocationApiInformation locationDetail = await this._unitOfWork.Reservations.GetLocationDetail(filter);

                            #region template mapping
                            var jsonTemplate = JObject.Parse(template.TemplateBody);
                            var child = jsonTemplate["children"] as JArray;

                            JArray jsonArray = child[0]["children"] as JArray;
                            for (int i = 0; i < jsonArray.Count; i++)
                            {
                                if (jsonArray[i]["className"] != null && jsonArray[i]["className"].Value<string>() == "Text" &&
                                    jsonArray[i]["attrs"] != null &&
                                    jsonArray[i]["attrs"]["is_variable"] != null && jsonArray[i]["attrs"]["is_variable"].Value<bool>())
                                {
                                    //jsonArray[i]["attrs"]["fontStyle"] = "normal";
                                    //variable, map patient info and replace the value
                                    if (locationDetail != null)
                                    {
                                        //if (jsonArray[i]["attrs"]["is_patient_restriction"] != null && jsonArray[i]["attrs"]["is_patient_restriction"].Value<bool>())
                                        //{
                                        //    var restriction = locationDetail.restrictions_flatten.Where(e => e.restriction_type_id.ToString() == jsonArray[i]["attrs"]["variable_name"].Value<string>()).FirstOrDefault();
                                        //    if (restriction != null)
                                        //    {
                                        //        jsonArray[i]["attrs"]["text"] = restriction.restriction_labels;
                                        //    }
                                        //}
                                        //else 
                                        if (jsonArray[i]["attrs"]["is_room_info"] != null && jsonArray[i]["attrs"]["is_room_info"].Value<bool>()
                                            && jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>() != null && jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>().Count > 0)
                                        {
                                            var grpVarsArray = jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>();
                                            var grpVariables = grpVarsArray.Select(e => (string)e).OrderBy(e => e).ToList();
                                            string combinedTexts = string.Empty;
                                            string combinedIcons = string.Empty;
                                            foreach (var variable in grpVariables)
                                            {
                                                if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                                {
                                                    //only facility has icons
                                                    if (variable == "location_facility")
                                                    {
                                                        if (locationDetail.Facilities != null)
                                                        {
                                                            if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                                            {
                                                                //display as icon
                                                                combinedIcons = string.Join(", ", locationDetail.Facilities.Select(e => e.Filename));
                                                                combinedTexts = string.Join(", ", locationDetail.Facilities.Select(e => e.Name));
                                                            }
                                                        }
                                                    }
                                                    else if (variable == "event_id")
                                                    {
                                                        if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                                        {
                                                            string baseUrl = _configuration["AppSettings:baseUrl"];
                                                            string attendarUrl = !string.IsNullOrEmpty(baseUrl) && baseUrl.EndsWith('/') ? "attendance?reservationid=" : "/attendance?reservationid=";
                                                            string frsUrl = string.Format("{0}{1}{2}", baseUrl, attendarUrl, locationDetail[variable]);
                                                            var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chf=bg,s,FFFFFF00&&chs={1}x{2}&chl={0}", frsUrl, 180, 130);
                                                            //display as icon
                                                            jsonArray[i]["attrs"]["event_id"] = Convert.ToString(locationDetail[variable]);
                                                            combinedIcons += Convert.ToString(url);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (variable == "location_facility")
                                                    {
                                                        combinedTexts += string.Join(", ", locationDetail.Facilities.Select(e => e.Name));
                                                    }
                                                    else
                                                    {
                                                        if (variable == "next_reservation_description" && locationDetail[variable] == null)
                                                        {
                                                            combinedTexts += "-";
                                                        }
                                                        else
                                                        {
                                                            if (!string.IsNullOrEmpty(combinedTexts) && !string.IsNullOrEmpty(Convert.ToString(locationDetail[variable])))
                                                            {
                                                                combinedTexts += ", ";
                                                            }

                                                            combinedTexts += Convert.ToString(locationDetail[variable]);
                                                        }
                                                    }
                                                }
                                            }

                                            if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                            {
                                                jsonArray[i]["attrs"]["icon"] = combinedIcons;
                                            }

                                            jsonArray[i]["attrs"]["text"] = combinedTexts;
                                        }
                                        else
                                        {
                                            jsonArray[i]["attrs"]["text"] = Convert.ToString(locationDetail[jsonArray[i]["attrs"]["variable_name"].Value<string>()]);
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region draw template
                            Bitmap output = DrawTemplate(JsonConvert.SerializeObject(jsonTemplate));

                            #endregion
                            //map done, now posting
                            byte[] outputImgByte;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                output.Save(ms, ImageFormat.Png);
                                outputImgByte = ms.ToArray();
                            }

                            //output.Save(@"C:\test\images\output.png");

                            #region Posting to epaper
                            // Generate post objects
                            Dictionary<string, object> postParameters = new Dictionary<string, object>();
                            postParameters.Add("name", "eImage.png");
                            postParameters.Add("eImage", new FormUpload.FileParameter(outputImgByte, "template.png", "image/png"));

                            string deviceAPIUrl = _configuration["AppSettings:epaperDeviceImageUrl"];
                            if (string.IsNullOrEmpty(template.DeviceImageAPIUrl))
                            {
                                template.DeviceImageAPIUrl = deviceAPIUrl;
                            }
                            // Create request and receive response
                            string postURL = template.DeviceImageAPIUrl;//"http://183.90.63.88:3000/epaper"; ;
                            string userAgent = "Someone";
                            try
                            {
                                HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, userAgent, postParameters);

                                // Process response
                                StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                                string fullResponse = responseReader.ReadToEnd();
                                webResponse.Close();
                                result.IsSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                result.IsSuccess = false;
                                result.Message = string.Format("There is a problem posting the image to the device, {0}", ex.Message);
                            }

                            result.Data = jsonTemplate;
                            #endregion
                        }
                        catch (Exception)
                        {
                            result.IsSuccess = false;
                        }
                    }
                }
            }


            return Ok(result);
        }

        [HttpPost("preview/image")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> PreviewAsImage([FromBody] EpaperTemplateViewModel template)
        {
            var result = new BaseOperationResponse();
            try
            {
                var filter = new CalendarFilter
                {
                    //Start = start,
                    //End = end,
                    locationIds = template.LocationIds
                };

                filter.Start = filter.Start != null ? Convert.ToDateTime(filter.Start).ToLocalTime() : filter.Start;
                filter.End = filter.End != null ? Convert.ToDateTime(filter.End).ToLocalTime() : filter.End;

                LocationApiInformation locationDetail = await this._unitOfWork.Reservations.GetLocationDetail(filter);

                #region template mapping
                var jsonTemplate = JObject.Parse(template.TemplateBody);
                var child = jsonTemplate["children"] as JArray;

                JArray jsonArray = child[0]["children"] as JArray;
                for (int i = 0; i < jsonArray.Count; i++)
                {
                    if (jsonArray[i]["className"] != null && jsonArray[i]["className"].Value<string>() == "Text" &&
                        jsonArray[i]["attrs"] != null &&
                        jsonArray[i]["attrs"]["is_variable"] != null && jsonArray[i]["attrs"]["is_variable"].Value<bool>())
                    {
                        //jsonArray[i]["attrs"]["fontStyle"] = "normal";
                        //variable, map patient info and replace the value
                        if (locationDetail != null)
                        {
                            //if (jsonArray[i]["attrs"]["is_patient_restriction"] != null && jsonArray[i]["attrs"]["is_patient_restriction"].Value<bool>())
                            //{
                            //    var restriction = locationDetail.restrictions_flatten.Where(e => e.restriction_type_id.ToString() == jsonArray[i]["attrs"]["variable_name"].Value<string>()).FirstOrDefault();
                            //    if (restriction != null)
                            //    {
                            //        jsonArray[i]["attrs"]["text"] = restriction.restriction_labels;
                            //    }
                            //}
                            //else 
                            if (jsonArray[i]["attrs"]["is_room_info"] != null && jsonArray[i]["attrs"]["is_room_info"].Value<bool>()
                                && jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>() != null && jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>().Count > 0)
                            {
                                var grpVarsArray = jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>();
                                var grpVariables = grpVarsArray.Select(e => (string)e).OrderBy(e => e).ToList();
                                string combinedTexts = string.Empty;
                                string combinedIcons = string.Empty;
                                foreach (var variable in grpVariables)
                                {
                                    if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                    {
                                        //only facility has icons
                                        if(variable == "location_facility")
                                        {
                                            if (locationDetail.Facilities != null)
                                            {
                                                if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                                {
                                                    //display as icon
                                                    combinedIcons = string.Join(", ", locationDetail.Facilities.Select(e => e.Filename));
                                                    combinedTexts = string.Join(", ", locationDetail.Facilities.Select(e =>  e.Name));
                                                }
                                            }
                                        }
                                        else if (variable == "event_id")
                                        {
                                            if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                            {
                                                string baseUrl = _configuration["AppSettings:baseUrl"];
                                                string attendarUrl = !string.IsNullOrEmpty(baseUrl) && baseUrl.EndsWith('/') ? "attendance?reservationid=" : "/attendance?reservationid=";
                                                string frsUrl = string.Format("{0}{1}{2}", baseUrl, attendarUrl, locationDetail[variable]);
                                                var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chf=bg,s,FFFFFF00&&chs={1}x{2}&chl={0}", frsUrl, 180, 130);
                                                //display as icon
                                                jsonArray[i]["attrs"]["event_id"] = Convert.ToString(locationDetail[variable]);
                                                combinedIcons += Convert.ToString(url);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (variable == "location_facility")
                                        {
                                            combinedTexts += string.Join(", ", locationDetail.Facilities.Select(e => e.Name));
                                        }
                                        else
                                        {
                                            if(variable == "next_reservation_description" && locationDetail[variable] == null)
                                            {
                                                combinedTexts += "-";
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(combinedTexts) && !string.IsNullOrEmpty(Convert.ToString(locationDetail[variable])))
                                                {
                                                    combinedTexts += ", ";
                                                }

                                                combinedTexts += Convert.ToString(locationDetail[variable]);
                                            }
                                        }
                                    }
                                }

                                if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                {
                                    jsonArray[i]["attrs"]["icon"] = combinedIcons;
                                }

                                jsonArray[i]["attrs"]["text"] = combinedTexts;
                            }
                            else
                            {
                                jsonArray[i]["attrs"]["text"] = Convert.ToString(locationDetail[jsonArray[i]["attrs"]["variable_name"].Value<string>()]);
                            }
                        }
                    }
                }

                #endregion

                #region draw template
                Bitmap output = DrawTemplate(JsonConvert.SerializeObject(jsonTemplate));

                #endregion

                //map done, now posting
                byte[] outputImgByte;
                using (MemoryStream ms = new MemoryStream())
                {
                    output.Save(ms, ImageFormat.Png);
                    outputImgByte = ms.ToArray();
                }

                try
                {
                    string tempImgSrc = string.Format("{0}\\{1}", _configuration["AppSettings:resourcesFolder"], "output.png");

                    output.Save(tempImgSrc);

                    string tempJpgImgSrc = string.Format("{0}\\{1}", _configuration["AppSettings:resourcesFolder"], "output.jpg");

                    output.Save(tempJpgImgSrc);
                }
                catch (Exception) { }

                if (template.IsPostToDevice)
                {
                    #region Posting to epaper
                    // Generate post objects
                    Dictionary<string, object> postParameters = new Dictionary<string, object>();
                    postParameters.Add("name", "eImage.png");
                    postParameters.Add("eImage", new FormUpload.FileParameter(outputImgByte, "template.png", "image/png"));

                    string deviceAPIUrl = _configuration["AppSettings:epaperDeviceImageUrl"];
                    if (string.IsNullOrEmpty(template.DeviceImageAPIUrl))
                    {
                        template.DeviceImageAPIUrl = deviceAPIUrl;
                    }

                    // Create request and receive response
                    string postURL = template.DeviceImageAPIUrl;
                    string userAgent = "Someone";
                    try
                    {
                        HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, userAgent, postParameters);

                        // Process response
                        StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                        string fullResponse = responseReader.ReadToEnd();
                        webResponse.Close();
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("There is a problem posting the image to the device, {0}", ex.Message));
                    }
                }

                result.Data = outputImgByte;
                result.IsSuccess = true;
                #endregion
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
            }


            return Ok(result);
        }

        
        #region Epaper Device
        /// <summary>
        /// API calls to get epaperTemplates
        /// </summary>
        /// <param name="epaperTemplateId"></param>
        /// <returns>List of epaperTemplates</returns>
        [HttpGet("get/device/{deviceId:int?}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiEpaperDevices(int? deviceId = null)
        {
            var result = await _unitOfWork.EpaperTemplates.GetApiEpaperTemplates(deviceId);
            var data = _mapper.Map<List<EpaperDeviceViewModel>>(result.Data);

            if (deviceId.HasValue)
            {
                result.Data = data.Any() ? data.First() : null;
            }
            else
            {
                result.Data = data;
            }

            return Ok(result);
        }

        [HttpGet("devices/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EpaperDeviceViewModel>))]
        public async Task<IActionResult> GetEpaperDevices()
        {
            return await GetEpaperDevices(-1, -1);
        }


        [HttpGet("devices/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<EpaperDeviceViewModel>))]
        public async Task<IActionResult> GetEpaperDevices(int pageNumber, int pageSize)
        {
            var results = await _unitOfWork.EpaperTemplates.GetEpaperDevicesLoadRelatedAsync(pageNumber, pageSize);
            return Ok(_mapper.Map<List<EpaperDeviceViewModel>>(results));
        }

        [HttpPost("device")]
        //[Authorize(Authorization.Policies.ManageAllEpaperDevicesPolicy)]
        [ProducesResponseType(201, Type = typeof(EpaperDeviceViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateEpaperDevice([FromBody] EpaperDeviceViewModel epaperDevice)
        {
            if (ModelState.IsValid)
            {
                if (epaperDevice == null)
                    return BadRequest($"{nameof(epaperDevice)} cannot be null");

                var device = _mapper.Map<EpaperDevice>(epaperDevice);

                var result = await _unitOfWork.EpaperTemplates.CreateDeviceAsync(device);
                if (result.IsSuccess)
                {
                    EpaperDeviceViewModel epaperDeviceVM = _mapper.Map<EpaperDeviceViewModel>(result.Data);
                    return CreatedAtAction("GetEpaperDeviceById", new { id = epaperDeviceVM.Id }, epaperDeviceVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("device/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEpaperDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(EpaperDeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEpaperDevice(int id)
        {
            if (!await _unitOfWork.EpaperTemplates.TestCanDeleteAsync(id))
                return BadRequest("EpaperDevice cannot be deleted."); //TODO: correct message here


            var epaperDevice = await this._unitOfWork.EpaperTemplates.GetByDeviceIdAsync(id);

            EpaperDeviceViewModel epaperDeviceVM = _mapper.Map<EpaperDeviceViewModel>(epaperDevice);
            if (epaperDeviceVM == null)
                return NotFound(id);

            var result = await _unitOfWork.EpaperTemplates.DeleteDeviceAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting epaperDevice: " + string.Join(", ", result.Message));


            return Ok(epaperDeviceVM);
        }

        [HttpPut("device/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllEpaperDevicesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEpaperDevice(string id, [FromBody] EpaperDeviceViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var epaperDevice = await this._unitOfWork.EpaperTemplates.GetByDeviceIdAsync(model.Id);

                EpaperDeviceViewModel epaperDeviceVM = _mapper.Map<EpaperDeviceViewModel>(epaperDevice);
                if (epaperDeviceVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<EpaperDevice>(model);
                var result = await _unitOfWork.EpaperTemplates.UpdateDeviceAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
        #endregion
        #region Private Methods
        private Bitmap DrawTemplate(string json)
        {
            //TODO: hard-coded image output size
            int height = 984;
            int width = 1304;
            var bmp = new Bitmap(width, height); //PixelFormat.Format32bppArgb

            bmp.MakeTransparent();
            var g = Graphics.FromImage(bmp);
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.Clear(Color.White);

            var jsonTemplate = JObject.Parse(json);
            var child = jsonTemplate["children"] as JArray;


            JArray jsonArrayChildren = child[0]["children"] as JArray;
            JArray jsonArray = new JArray(jsonArrayChildren.OrderBy(obj => PIBDrawing.SafeGet(obj["zLayer"])));

            for (int i = 0; i < jsonArray.Count; i++)
            {
                if (jsonArray[i]["className"] != null)
                {
                    var component = jsonArray[i];
                    switch (jsonArray[i]["className"].Value<string>())
                    {
                        case "Text":
                            var txtAttr = new
                            {
                                x = component["attrs"]["x"] != null ? component["attrs"]["x"].Value<int>() : 0,
                                y = component["attrs"]["y"] != null ? component["attrs"]["y"].Value<int>() : 0,
                                txt = component["attrs"]["text"] != null ? component["attrs"]["text"].Value<string>() : string.Empty,
                                fontSize = component["attrs"]["fontSize"] != null ? component["attrs"]["fontSize"].Value<int>() : 0,
                                fontFamily = component["attrs"]["fontFamily"] != null ? component["attrs"]["fontFamily"].Value<string>() : string.Empty,
                                width = component["attrs"]["width"] != null ? component["attrs"]["width"].Value<int>() : 0,
                                fill = component["attrs"]["fill"] != null ? component["attrs"]["fill"].Value<string>() : "black",
                                icon = component["attrs"]["icon"] != null ? component["attrs"]["icon"].Value<string>() : "",
                                event_id = component["attrs"]["event_id"] != null ? component["attrs"]["event_id"].Value<string>() : string.Empty,
                                fontStyle = component["attrs"]["fontStyle"] != null ? component["attrs"]["fontStyle"].Value<string>() : "normal",
                                textWrap = component["attrs"]["text_wrap"] != null ? component["attrs"]["text_wrap"].Value<string>() : "wrap",
                                scaleY = component["attrs"]["scaleY"] != null ? component["attrs"]["scaleY"].Value<float>() : 1f
                            };

                            if (string.IsNullOrEmpty(txtAttr.icon))
                            {
                                var points = txtAttr.fontSize * 72 / g.DpiX;
                                //var points = ((float)txtAttr.fontSize / 150f) * 72f;

                                Font drawFont = PIBDrawing.GetFont(txtAttr.fontFamily, points, txtAttr.fontStyle == "bold" ? FontStyle.Bold : txtAttr.fontStyle == "italic" ? FontStyle.Italic : FontStyle.Regular);
                                SolidBrush drawBrush = new SolidBrush(txtAttr.fill == "black" ? Color.Black : Color.White);

                                // find the width of single char using selected fonts
                                float CharWidth = g.MeasureString("Y", drawFont).Width;

                                // draw first part of string
                                //g.DrawString(txtAttr.txt, drawFont, drawBrush, txtAttr.x, txtAttr.y);

                                //Font trFont = new Font("Times New Roman", 12);
                                if (txtAttr.textWrap == "wrap")
                                {
                                    int scale = (int)Math.Ceiling(txtAttr.scaleY);
                                    scale = scale == 0 ? 1 : scale;
                                    Size container = new Size(txtAttr.width, drawFont.Height * scale);
                                    Rectangle rect = new Rectangle(txtAttr.x, txtAttr.y, container.Width, container.Height);
                                    g.DrawRectangle(Pens.Transparent, rect);
                                    g.DrawString(txtAttr.txt, drawFont, drawBrush, rect);
                                }
                                else if (txtAttr.textWrap == "ellipsis")
                                {
                                    StringFormat format = new StringFormat();
                                    format.Trimming = StringTrimming.EllipsisWord;
                                    Rectangle rect = new Rectangle(txtAttr.x, txtAttr.y, txtAttr.width, drawFont.Height);
                                    g.DrawString(txtAttr.txt, drawFont, drawBrush, rect, format);
                                }
                                else if (txtAttr.textWrap == "resize")
                                {
                                    Font font = PIBDrawing.GetAdjustedFont(g, txtAttr.txt, drawFont, txtAttr.width, 100, 1, true);
                                    SizeF size = g.MeasureString(txtAttr.txt, font);
                                    Rectangle rect = new Rectangle(txtAttr.x, txtAttr.y, txtAttr.width, drawFont.Height);
                                    g.DrawString(txtAttr.txt, font, drawBrush, rect);
                                }
                                else if (txtAttr.textWrap == "resize and wrap")
                                {
                                    int scale = (int)Math.Ceiling(txtAttr.scaleY);
                                    scale = scale == 0 ? 1 : scale;
                                    Size container = new Size(txtAttr.width, drawFont.Height * scale);
                                    var f = PIBDrawing.GetAdjustedFontWrap(g, txtAttr.txt, drawFont, container);
                                    g.DrawString(txtAttr.txt, f, drawBrush, new Rectangle(txtAttr.x, txtAttr.y, container.Width, container.Height));
                                }
                            }
                            else
                            {
                                bool isQrCode = !string.IsNullOrEmpty(txtAttr.event_id);
                                var picNames = isQrCode ? new string[] { } : txtAttr.txt.Split(",");
                                var pictures = isQrCode ? new string[] { txtAttr.icon } : txtAttr.icon.Split(",");
                                int x = txtAttr.x;
                                int y = txtAttr.y;
                                int origX = txtAttr.x;
                                int origY = txtAttr.y;
                                List<Bitmap> bitmaps = new List<Bitmap>();
                                //download pictures
                                foreach (var picture in pictures)
                                {
                                    if (!string.IsNullOrEmpty(picture.Trim()))
                                    {
                                        if (isQrCode)
                                        {
                                            try
                                            {
                                                using (WebClient client = new WebClient())
                                                {
                                                    using (Stream stream = client.OpenRead(picture.Trim()))
                                                    {
                                                        if (stream != null)
                                                        {
                                                            var src = new Bitmap(stream);

                                                            if (src != null)
                                                            {
                                                                bitmaps.Add(src);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                        else
                                        {
                                            try
                                            {


                                                //string folder = _configuration["AppSettings:baseUrl"];
                                                //string imageUrl = string.Format("{0}/{1}", folder, picture.Trim());

                                                var folderName = Path.Combine("Resources", "Images");
                                                var imageUrl = Path.Combine(Directory.GetCurrentDirectory(), folderName, picture.Trim());

                                                using (Stream stream = System.IO.File.OpenRead(imageUrl))
                                                {
                                                    if (stream != null)
                                                    {
                                                        var src = new Bitmap(stream);

                                                        if (src != null)
                                                        {
                                                            bitmaps.Add(src);
                                                        }
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                    }
                                }

                                int pic = 0;
                                foreach (var src in bitmaps)
                                {
                                    bool makeTransparent = true;
                                    Color pixel = src.GetPixel(0, 0);
                                    //if (pixel.A == 255)
                                    //{
                                    //    makeTransparent = false;
                                    //}

                                    if (makeTransparent)
                                    {
                                        //src.MakeTransparent();
                                    }

                                    int widthPerIcon = txtAttr.width / bitmaps.Count;
                                    var scale = (float)src.Width / widthPerIcon;
                                    var imgWidth = (int)((float)src.Width * scale);
                                    var imgHeight = (int)((float)src.Height * scale);


                                    var points = txtAttr.fontSize * 72 / g.DpiX;
                                    Font drawFont = PIBDrawing.GetFont(txtAttr.fontFamily, points, txtAttr.fontStyle == "bold" ? FontStyle.Bold : txtAttr.fontStyle == "italic" ? FontStyle.Italic : FontStyle.Regular);
                                    SolidBrush drawBrush = new SolidBrush(txtAttr.fill == "black" ? Color.Black : Color.White);

                                    // find the width of single char using selected fonts
                                    float charWidth = g.MeasureString("Y", drawFont).Width;

                                    var img = PIBDrawing.FixedSize(src, widthPerIcon, src.Height, makeTransparent, !isQrCode, (int)points, isQrCode ? string.Empty : picNames[pic++], drawFont, drawBrush);

                                    if ((x + img.Width - origX) > txtAttr.width)
                                    {
                                        y = y + img.Height;
                                        x = txtAttr.x;
                                    }

                                    

                                    ////Create new image that will be bigger then original image to make place for footer
                                    //Bitmap newImage = new Bitmap(img.Height + (int)points, img.Width);

                                    ////Get graphics and copy image and below the footer
                                    //Graphics gr = Graphics.FromImage(newImage);
                                    //gr.DrawImage(img, new Point(x, y));
                                    ////gr.FillRectangle(new SolidBrush(Color.Black), 0, img.Height, img.Width, 200);
                                    //gr.DrawString(picNames[pic++], drawFont, drawBrush, 0, img.Height + (int)points);
                                    ////Anything else you like, circles, rectangles, texts etc..
                                    //gr.Dispose();

                                    //var newImage2 = PIBDrawing.FixedSize(newImage, widthPerIcon, newImage.Height, makeTransparent);

                                    g.DrawImage(img, x, y);
                                    

                                    //Rectangle rect = new Rectangle(x, y + (int)points, txtAttr.width, drawFont.Height * 2);
                                    //g.DrawRectangle(Pens.Transparent, rect);
                                    //g.DrawString(picNames[pic++], drawFont, drawBrush, rect);

                                    x = x + img.Width;
                                }
                            }

                            break;
                        case "Line":
                            var lineAttr = new
                            {
                                points = component["attrs"]["points"] != null ? (component["attrs"]["points"] as JArray).Select(e => (int)e).ToArray() : new int[] { 0, 0 },
                                stroke = component["attrs"]["stroke"] != null ? component["attrs"]["stroke"].Value<string>() : "black",
                            };

                            var x1 = lineAttr.points[0];
                            var y1 = lineAttr.points[1];
                            var x2 = lineAttr.points[lineAttr.points.Length - 2];
                            var y2 = lineAttr.points[lineAttr.points.Length - 1];
                            g.DrawLine(new Pen(Color.Black, 2), x1, y1, x2, y2);
                            break;
                        case "Rect":
                            var rectAttr = new
                            {
                                x = component["attrs"]["x"] != null ? component["attrs"]["x"].Value<int>() : 0,
                                y = component["attrs"]["y"] != null ? component["attrs"]["y"].Value<int>() : 0,
                                width = component["attrs"]["width"] != null ? component["attrs"]["width"].Value<int>() : 0,
                                height = component["attrs"]["height"] != null ? component["attrs"]["height"].Value<int>() : 0,
                                scaleX = component["attrs"]["scaleX"] != null ? component["attrs"]["scaleX"].Value<int>() : 1,
                                scaleY = component["attrs"]["scaleY"] != null ? component["attrs"]["scaleY"].Value<int>() : 1,
                                fill = component["attrs"]["fill"] != null ? component["attrs"]["fill"].Value<string>() : "black"
                            };
                            if (rectAttr.fill == "black")
                            {
                                SolidBrush brush = new SolidBrush(Color.Black);
                                g.FillRectangle(brush, rectAttr.x, rectAttr.y, rectAttr.scaleX * (rectAttr.width), rectAttr.scaleY * (rectAttr.height));

                            }
                            else
                            {
                                var strokeWidth = 30 * 72 / g.DpiX;
                                g.DrawRectangle(new Pen(Color.Black, 5), rectAttr.x, rectAttr.y, rectAttr.scaleX * (rectAttr.width), rectAttr.scaleY * (rectAttr.height));
                            }
                            break;
                        case "Image":
                            var imgAttr = new
                            {
                                src = component["attrs"]["src"] != null ? component["attrs"]["src"].Value<string>() : string.Empty,
                                x = component["attrs"]["x"] != null ? component["attrs"]["x"].Value<int>() : 0,
                                y = component["attrs"]["y"] != null ? component["attrs"]["y"].Value<int>() : 0,
                                scaleX = component["attrs"]["scaleX"] != null ? component["attrs"]["scaleX"].Value<int>() : 1,
                                scaleY = component["attrs"]["scaleY"] != null ? component["attrs"]["scaleY"].Value<int>() : 1
                            };

                            if (!string.IsNullOrEmpty(imgAttr.src))
                            {
                                try
                                {
                                    using (WebClient client = new WebClient())
                                    {
                                        using (Stream stream = client.OpenRead(imgAttr.src))
                                        {
                                            if (stream != null)
                                            {
                                                var src = new Bitmap(stream);

                                                if (src != null)
                                                {
                                                    bool makeTransparent = true;
                                                    Color pixel = src.GetPixel(0, 0);
                                                    //if (pixel.A == 255)
                                                    //{
                                                    //    makeTransparent = false;
                                                    //}

                                                    if (makeTransparent)
                                                    {
                                                        //src.MakeTransparent();
                                                    }
                                                    var img = PIBDrawing.FixedSize(src, src.Width * imgAttr.scaleX, src.Height * imgAttr.scaleY, makeTransparent);
                                                    g.DrawImage(img, imgAttr.x, imgAttr.y);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }


                            }

                            break;
                    }
                }
            }

            return bmp;


        }
        private PatientInfo GetPatientInfoByLocationCode(string apiURL)
        {
            // var apiURL = "http://119.73.206.36:91/api/registration/GetRegistrationsByLocationCode";
            if (apiURL == null)
                throw new Exception("Patient Info API URL is not set.");

            PatientInfo patient = null;
            using (var client = new HttpClient())
            {
                try
                {
                    //client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.GetAsync(apiURL).Result;
                    //var response = client.GetAsync(string.Format("?locationCode={0}", locationCode)).Result;
                    response.EnsureSuccessStatusCode();
                    var content = response.Content.ReadAsStringAsync().Result;

                    JArray jsonArray = JArray.Parse(content);
                    var patient_info = JObject.Parse(jsonArray[0].ToString());
                    patient = _mapper.Map<PatientInfo>(patient_info);
                    //var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    //patient = new PatientInfo
                    //{
                    //    identifier = patient_info["profile"]["identifier"].Value<string>(),
                    //    profile_id = patient_info["profile"]["profile_id"].Value<string>(),
                    //    name = patient_info["profile"]["name"].Value<string>(),
                    //    preferred_name = patient_info["profile"]["preferred_name"].Value<string>(),
                    //    gender = patient_info["profile"]["gender"].Value<string>(),
                    //    vip = patient_info["profile"]["vip"].Value<bool>(),
                    //    case_number = patient_info["case_number"].Value<string>(),
                    //    status = patient_info["status"].Value<string>(),
                    //    location_code = patient_info["location"]["code"].Value<string>(),
                    //    location_label = patient_info["location"]["label"].Value<string>(),
                    //    attending_doctor = patient_info["attending_doctor"].Value<string>(),
                    //    attending_nurse = patient_info["attending_nurse"].Value<string>(),
                    //    admission_date = patient_info["admission_date"].Value<DateTime?>()
                    //};

                    //string restrictionContent = patient_info["restrictions"].Value<string>();
                    //JArray jsonRestrictions = JArray.Parse(restrictionContent);

                    //CorporateRatesInfo dto = _mapper.Map<CorporateRatesInfo>(jsonRestrictions);
                    //dummy restrictions
                    //patient.location_label = "SKCH Ward 710";
                    //patient.mode_of_feeding = "Oral";
                    //patient.food_restrictions = "No Nuts, No Dairy";
                    //patient.diet_type = "DM 1500";
                    //patient.diet_texture = "Normal, Nectar";
                    //patient.fluid_restrictions = "1L per day";
                    //patient.function_status = "A1, Sit out of bed, BD, PU Care Bundle";
                    //patient.special_instructions = "TLC";
                }
                catch (Exception ex)
                {
                }
            }
            return patient;
        }

        private List<RestrictionType> GetApiRestrictionTypes(string apiURL)
        {
            // var apiURL = "http://119.73.206.36:91/api/epaper/GetTypes";
            if (apiURL == null)
                throw new Exception("Patient Info API URL is not set.");

            var restrictionTypes = new List<RestrictionType>();

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(apiURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.PostAsync(apiURL, new StringContent(
                                JsonConvert.SerializeObject(new
                                {
                                    tenantId = 1,
                                    paged = false,
                                    activeOnly = true,
                                    orderBy = "sequence"
                                }), Encoding.UTF8, "application/json")).Result;
                //var response = client.GetAsync(string.Format("?locationCode={0}", locationCode)).Result;
                response.EnsureSuccessStatusCode();
                var content = response.Content.ReadAsStringAsync().Result;
                var contentJson = JObject.Parse(content);
                JArray jsonArray = contentJson["data"] as JArray;

                for (int i = 0; i < jsonArray.Count; i++)
                {
                    RestrictionType data = new RestrictionType()
                    {
                        restriction_type_id = jsonArray[i]["restriction_type_id"] != null ? jsonArray[i]["restriction_type_id"].Value<long>() : 0,
                        tenant_id = jsonArray[i]["tenant_id"] != null ? jsonArray[i]["tenant_id"].Value<long>() : 0,
                        code = jsonArray[i]["code"] != null ? jsonArray[i]["code"].Value<string>() : string.Empty,
                        label = jsonArray[i]["label"] != null ? jsonArray[i]["label"].Value<string>() : string.Empty,
                    };


                    restrictionTypes.Add(data);
                }
            }

            return restrictionTypes.OrderBy(e => e.label).ToList();
        }

        private BaseOperationResponse PostToDeviceJSON(string templateBody, string url)
        {
            var resp = new BaseOperationResponse();
            var apiURL = url;
            if (apiURL == null)
                throw new Exception("Device API URL is not set.");

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.PostAsync(apiURL, new StringContent(
                                JsonConvert.SerializeObject(new
                                {
                                    data = templateBody
                                }), Encoding.UTF8, "application/json")).Result;

                    //var cleanQParameters = System.Web.HttpUtility.HtmlEncode(string.Format("?data={0}", templateBody));
                    //var response = client.GetAsync(cleanQParameters).Result;
                    response.EnsureSuccessStatusCode();
                    var content = response.Content.ReadAsStringAsync().Result;

                    resp.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    resp.IsSuccess = false;
                    resp.Message = ex.Message;
                }
            }
            return resp;
        }
        #endregion
    }
}