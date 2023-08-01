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

namespace FRS.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PIBController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private IHubContext<FRSHub> _frsHub;
        private readonly IConfiguration _configuration;

        public PIBController(IUnitOfWork unitOfWork, ILogger<PIBTemplateController> logger, IConfiguration configuration, IHubContext<FRSHub> frsHub)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _frsHub = frsHub;
        }

        /// <summary>
        /// API calls to get pibTemplates
        /// </summary>
        /// <param name="pibTemplateId"></param>
        /// <returns>List of pibTemplates</returns>
        [HttpGet("get/template")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiPIBTemplates(int? templateId = null, string mac = null)
        {
            var result = await _unitOfWork.PIBTemplates.GetApiPIBTemplates(templateId, mac);
            var data = Mapper.Map<List<PIBTemplateViewModel>>(result.Data);

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
        [ProducesResponseType(200, Type = typeof(List<PIBTemplateViewModel>))]
        public async Task<IActionResult> GetPIBTemplates()
        {
            return await GetPIBTemplates(-1, -1);
        }


        [HttpGet("templates/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PIBTemplateViewModel>))]
        public async Task<IActionResult> GetPIBTemplates(int pageNumber, int pageSize)
        {
            var results = await _unitOfWork.PIBTemplates.GetPIBTemplatesLoadRelatedAsync(pageNumber, pageSize);
            return Ok(Mapper.Map<List<PIBTemplateViewModel>>(results));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllPIBTemplatesPolicy)]
        [ProducesResponseType(201, Type = typeof(PIBTemplateViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePIBTemplate([FromBody] PIBTemplateViewModel pibTemplate)
        {
            if (ModelState.IsValid)
            {
                if (pibTemplate == null)
                    return BadRequest($"{nameof(pibTemplate)} cannot be null");

                string deviceAPIUrl = _configuration["AppSettings:pibDeviceUrl"];
                if (string.IsNullOrEmpty(pibTemplate.DeviceAPIUrl))
                {
                    pibTemplate.DeviceAPIUrl = deviceAPIUrl;
                }

                var type = Mapper.Map<PIBTemplate>(pibTemplate);

                var result = await _unitOfWork.PIBTemplates.CreateAsync(type);
                if (result.IsSuccess)
                {
                    PIBTemplateViewModel pibTemplateVM = Mapper.Map<PIBTemplateViewModel>(result.Data);
                    return CreatedAtAction("GetPIBTemplateById", new { id = pibTemplateVM.Id }, pibTemplateVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllPIBTemplatesPolicy)]
        [ProducesResponseType(200, Type = typeof(PIBTemplateViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePIBTemplate(int id)
        {
            if (!await _unitOfWork.PIBTemplates.TestCanDeleteAsync(id))
                return BadRequest("PIBTemplate cannot be deleted."); //TODO: correct message here


            var pibTemplate = await this._unitOfWork.PIBTemplates.GetByIdAsync(id);

            PIBTemplateViewModel pibTemplateVM = Mapper.Map<PIBTemplateViewModel>(pibTemplate);
            if (pibTemplateVM == null)
                return NotFound(id);

            var result = await _unitOfWork.PIBTemplates.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting pibTemplate: " + string.Join(", ", result.Message));


            return Ok(pibTemplateVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllPIBTemplatesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePIBTemplate(string id, [FromBody] PIBTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var pibTemplate = await this._unitOfWork.PIBTemplates.GetByIdAsync(model.Id);

                PIBTemplateViewModel pibTemplateVM = Mapper.Map<PIBTemplateViewModel>(pibTemplate);
                if (pibTemplateVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<PIBTemplate>(model);
                var result = await _unitOfWork.PIBTemplates.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("get/restrictiontypes")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RestrictionType>))]
        public async Task<IActionResult> GetRestrictionTypes()
        {
            string apiUrl = _configuration["AppSettings:patientApiUrl"];
            var results = await GetApiRestrictionTypes(apiUrl);
            return Ok(Mapper.Map<List<RestrictionType>>(results));
        }

        [HttpGet("get/piblocations")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PIBTemplateLocationViewModel>))]
        public async Task<IActionResult> GetLocations()
        {
            var results = await _unitOfWork.PIBTemplates.GetPIBLocationsLoadRelatedAsync(-1, -1);
            return Ok(Mapper.Map<List<PIBTemplateLocationViewModel>>(results));
        }

        [HttpGet("get/piblocations/sync")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> SyncLocations()
        {
            string apiUrl = _configuration["AppSettings:pibLocationApiUrl"];
            var results = new BaseOperationResponse();
            try
            {
                var locations = GetApiLocations(apiUrl);
                results = await _unitOfWork.PIBTemplates.SyncLocationsAsync(locations);
            }
            catch (Exception ex)
            {
                results.IsSuccess = false;
                results.Data = null;
                results.Message = ex.Message;
            }

            return Ok(results);
        }

        /// <summary>
        /// API calls to map template
        /// </summary>
        /// <param name="template"></param>
        /// <returns>List of facilities</returns>
        [HttpPost("map/template")]
        [HttpGet("map/template")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> MapTemplate([FromBody] PIBTemplateViewModel template)
        {
            var result = new BaseOperationResponse();
            try
            {
                PatientInfo patientInfo = null;
                bool isMapToTemplate;
                if (template.IsStaticLink && !string.IsNullOrEmpty(template.MacAddress))
                {
                    string apiUrl = _configuration["AppSettings:pibPatientInfoApiUrl"];
                    var deviceResult = await this._unitOfWork.PIBTemplates.GetApiPIBDevices(macAddress: template.MacAddress);
                    var data = Mapper.Map<List<PIBDeviceViewModel>>(deviceResult.Data);

                    if (!string.IsNullOrEmpty(template.MacAddress) && data.Any())
                    {
                        var device = data.First();
                        apiUrl = apiUrl + string.Format("?locationCode={0}", device.location_code);
                        patientInfo = await GetPatientInfoByLocationCode(apiUrl);
                        isMapToTemplate = true;
                    }
                    else
                    {
                        isMapToTemplate = false;
                    }
                }
                else
                {
                    isMapToTemplate = template.IsMapToAPI && !string.IsNullOrEmpty(template.MapAPIUrl);
                    if (isMapToTemplate)
                    {
                        patientInfo = await GetPatientInfoByLocationCode(template.MapAPIUrl);
                    }
                }

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
                        if (isMapToTemplate && patientInfo != null)
                        {
                            if (jsonArray[i]["attrs"]["is_patient_restriction"] != null && jsonArray[i]["attrs"]["is_patient_restriction"].Value<bool>())
                            {
                                var restriction = patientInfo.restrictions_flatten.Where(e => e.restriction_type_id.ToString() == jsonArray[i]["attrs"]["variable_name"].Value<string>()).FirstOrDefault();
                                if (restriction != null)
                                {
                                    jsonArray[i]["attrs"]["text"] = restriction.restriction_labels;
                                }
                            }
                            else if (jsonArray[i]["attrs"]["is_group_patient_restriction"] != null && jsonArray[i]["attrs"]["is_group_patient_restriction"].Value<bool>()
                                && jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>() != null && jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>().Count > 0)
                            {
                                var grpVarsArray = jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>();
                                var grpIds = grpVarsArray.Select(e => (long)e).OrderBy(e => e).ToList();
                                var restriction = patientInfo.restrictions_flatten.Where(e => grpIds.Contains(e.restriction_type_id));
                                if (restriction != null)
                                {
                                    if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                    {
                                        //display as icon
                                        jsonArray[i]["attrs"]["icon"] = string.Join(", ", restriction.Select(e => e.pictures));
                                    }
                                    else
                                    {
                                        jsonArray[i]["attrs"]["text"] = string.Join(", ", restriction.Select(e => e.restriction_labels));
                                    }
                                }
                            }
                            else
                            {
                                jsonArray[i]["attrs"]["text"] = Convert.ToString(patientInfo[jsonArray[i]["attrs"]["variable_name"].Value<string>()]);
                            }
                        }
                    }
                }

                if (template.IsPostToDevice)
                {
                    var jsonString = JsonConvert.SerializeObject(jsonTemplate);
                    string deviceAPIUrl = _configuration["AppSettings:pibDeviceUrl"];
                    if (string.IsNullOrEmpty(template.DeviceAPIUrl))
                    {
                        template.DeviceAPIUrl = deviceAPIUrl;
                    }

                    result = PostToDeviceJSON(jsonString, template.DeviceAPIUrl);
                    return Ok(result);
                }
                result.Data = jsonTemplate;
                result.IsSuccess = true;
            }
            catch (Exception)
            {
                result.IsSuccess = false;
            }
            return Ok(result);
        }

        //[HttpPost("map/posttodevice")]
        //[AllowAnonymous]
        //[ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        //public async Task<IActionResult> PostImageToDevice([FromBody] PIBTemplateViewModel template)
        //{
        //    var result = new BaseOperationResponse();
        //    var apiURL = "http://183.90.63.88:3000/epaper";
        //    if (apiURL == null)
        //        throw new Exception("API URL is not set.");

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(apiURL);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        var base64Image = template.imgUri;
        //        var offset = base64Image.Substring(base64Image.IndexOf(',') + 1);

        //        var imageInBytes = Convert.FromBase64String(offset);
        //        using (var ms = new MemoryStream(imageInBytes))
        //        {
        //            var img = Image.FromStream(ms);
        //            img.Save(@"C:\test\images\sheila.png");
        //        }

        //        ByteArrayContent byteContent = new ByteArrayContent(imageInBytes);
        //        byteContent.Headers.Add("Content-Type", "application/octet-stream");

        //        var formData = new MultipartFormDataContent();
        //        formData.Add(new StringContent(template.name), "name");
        //        formData.Add(byteContent, "eImage", "eImage.png");

        //        //var response = await client.PostAsync(apiURL, new MultipartFormDataContent
        //        //{
        //        //    {new StringContent(name), "\"name\""},
        //        //    { imgUri, "\"file\"", "\"template.png\""}
        //        //});

        //        var myHttpClient = new HttpClient();
        //        var response = myHttpClient.PostAsync(apiURL, formData).Result;
        //        response.EnsureSuccessStatusCode();
        //        var content = response.Content.ReadAsStringAsync().Result;
        //        result.Data = content;

        //    }

        //    result.IsSuccess = true;
        //    return Ok(result);
        //}

        [HttpGet("executePhantomJS")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ExecutePhantomJS(string macAddress)
        {
            //ExecutePhantomJs("http://localhost:56767/pibdevicetemplate?mac_address=" + macAddress);
            return Ok();
        }

        [HttpPost("map/posttodevice")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> PostImageToDevice([FromBody] PIBTemplateViewModel template)
        {
            var result = new BaseOperationResponse();

            try
            {
                if (template.IsStaticLink && !string.IsNullOrEmpty(template.MacAddress))
                {
                    if (string.IsNullOrEmpty(template.DeviceImageAPIUrl))
                        throw new Exception("API URL is not set in PIB Device.");
                }
                else
                {
                    string deviceAPIUrl = _configuration["AppSettings:pibDeviceImageUrl"];
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
                    string fullResponse = await responseReader.ReadToEndAsync();
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
            var result = await _unitOfWork.PIBTemplates.UpdateDeviceStatus(mac_address, device_code);
            dynamic data = result.Data;
            var isNew = data.GetType().GetProperty("isNew").GetValue(data, null);

            if (isNew)
            {
                var objectData = data.GetType().GetProperty("data").GetValue(data, null);
                await _frsHub.Clients.All.SendAsync("RefreshPIBDeviceList", Mapper.Map<PIBDeviceViewModel>(objectData as PIBDevice));
            }

            return Ok(result);
        }

        [HttpGet("display/epaper")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> DisplayEPaper(string mac_address)
        {
            var result = await _unitOfWork.PIBTemplates.GetApiPIBDevices(macAddress: mac_address);
            if (!string.IsNullOrEmpty(mac_address))
            {
                var deviceData = Mapper.Map<List<PIBDeviceViewModel>>(result.Data);

                if (deviceData.Any())
                {
                    var device = deviceData.First();
                    var templateResult = await _unitOfWork.PIBTemplates.GetApiPIBTemplates(macAddress: mac_address);
                    var templateData = Mapper.Map<List<PIBTemplateViewModel>>(templateResult.Data);

                    if (templateData.Any())
                    {
                        var template = templateData.First();
                        template.DeviceImageAPIUrl = device.epaper_url;

                        try
                        {
                            PatientInfo patientInfo = null;
                            bool isMapToTemplate = true;
                            string apiUrl = _configuration["AppSettings:pibPatientInfoApiUrl"];
                            apiUrl = apiUrl + string.Format("?locationCode={0}", device.location_code);
                            patientInfo = await GetPatientInfoByLocationCode(apiUrl);

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
                                    if (isMapToTemplate && patientInfo != null)
                                    {
                                        if (jsonArray[i]["attrs"]["is_patient_restriction"] != null && jsonArray[i]["attrs"]["is_patient_restriction"].Value<bool>())
                                        {
                                            var restriction = patientInfo.restrictions_flatten.Where(e => e.restriction_type_id.ToString() == jsonArray[i]["attrs"]["variable_name"].Value<string>()).FirstOrDefault();
                                            if (restriction != null)
                                            {
                                                jsonArray[i]["attrs"]["text"] = restriction.restriction_labels;
                                            }
                                        }
                                        else if (jsonArray[i]["attrs"]["is_group_patient_restriction"] != null && jsonArray[i]["attrs"]["is_group_patient_restriction"].Value<bool>()
                                            && jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>() != null && jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>().Count > 0)
                                        {
                                            var grpVarsArray = jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>();
                                            var grpIds = grpVarsArray.Select(e => (long)e).OrderBy(e => e).ToList();
                                            var restriction = patientInfo.restrictions_flatten.Where(e => grpIds.Contains(e.restriction_type_id));
                                            if (restriction != null)
                                            {
                                                if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                                {
                                                    //display as icon
                                                    jsonArray[i]["attrs"]["icon"] = string.Join(", ", restriction.Select(e => e.pictures));
                                                }
                                                else
                                                {
                                                    jsonArray[i]["attrs"]["text"] = string.Join(", ", restriction.Select(e => e.restriction_labels));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            jsonArray[i]["attrs"]["text"] = Convert.ToString(patientInfo[jsonArray[i]["attrs"]["variable_name"].Value<string>()]);
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

                            string deviceAPIUrl = _configuration["AppSettings:pibDeviceImageUrl"];
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
        public async Task<IActionResult> PreviewAsImage([FromBody] PIBTemplateViewModel template)
        {
            var result = new BaseOperationResponse();
            try
            {
                PatientInfo patientInfo = null;
                bool isMapToTemplate = template.IsMapToAPI && !string.IsNullOrEmpty(template.MapAPIUrl);
                if (isMapToTemplate)
                {
                    patientInfo = await GetPatientInfoByLocationCode(template.MapAPIUrl);
                }

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
                        if (isMapToTemplate && patientInfo != null)
                        {
                            if (jsonArray[i]["attrs"]["is_patient_restriction"] != null && jsonArray[i]["attrs"]["is_patient_restriction"].Value<bool>())
                            {
                                var restriction = patientInfo.restrictions_flatten.Where(e => e.restriction_type_id.ToString() == jsonArray[i]["attrs"]["variable_name"].Value<string>()).FirstOrDefault();
                                if (restriction != null)
                                {
                                    jsonArray[i]["attrs"]["text"] = restriction.restriction_labels;
                                }
                            }
                            else if (jsonArray[i]["attrs"]["is_group_patient_restriction"] != null && jsonArray[i]["attrs"]["is_group_patient_restriction"].Value<bool>()
                                && jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>() != null && jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>().Count > 0)
                            {
                                var grpVarsArray = jsonArray[i]["attrs"]["group_variable_name"].Value<JArray>();
                                var grpIds = grpVarsArray.Select(e => (long)e).OrderBy(e => e).ToList();
                                var restriction = patientInfo.restrictions_flatten.Where(e => grpIds.Contains(e.restriction_type_id));
                                if (restriction != null)
                                {
                                    if (jsonArray[i]["attrs"]["is_icon"] != null && jsonArray[i]["attrs"]["is_icon"].Value<bool>())
                                    {
                                        //display as icon
                                        jsonArray[i]["attrs"]["icon"] = string.Join(", ", restriction.Select(e => e.pictures));
                                    }
                                    else
                                    {
                                        jsonArray[i]["attrs"]["text"] = string.Join(", ", restriction.Select(e => e.restriction_labels));
                                    }
                                }
                            }
                            else
                            {
                                jsonArray[i]["attrs"]["text"] = Convert.ToString(patientInfo[jsonArray[i]["attrs"]["variable_name"].Value<string>()]);
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

                    string deviceAPIUrl = _configuration["AppSettings:pibDeviceImageUrl"];
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
                                fontStyle = component["attrs"]["fontStyle"] != null ? component["attrs"]["fontStyle"].Value<string>() : "normal",
                                textWrap = component["attrs"]["text_wrap"] != null ? component["attrs"]["text_wrap"].Value<string>() : "wrap",
                                scaleY = component["attrs"]["scaleY"] != null ? component["attrs"]["scaleY"].Value<float>() : 1f,
                            };

                            if (string.IsNullOrEmpty(txtAttr.icon))
                            {
                                var points = txtAttr.fontSize * 72 / g.DpiX;
                                //var points = ((float)txtAttr.fontSize / 150f) * 72f;

                                Font drawFont = PIBDrawing.GetFont(txtAttr.fontFamily, points, txtAttr.fontStyle == "bold" ? FontStyle.Bold : txtAttr.fontStyle == "italic" ? FontStyle.Italic : FontStyle.Regular);
                                SolidBrush drawBrush = new SolidBrush(txtAttr.fill == "black" ? Color.Black : Color.White);

                                // find the width of single char using selected fonts
                                //float CharWidth = g.MeasureString("Y", drawFont).Width;

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
                                var pictures = txtAttr.icon.Split(",");
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
                                        try
                                        {
                                            string folder = _configuration["AppSettings:pibResourcesPath"];
                                            string imageUrl = string.Format("{0}/{1}", folder, picture.Trim());

                                            using (WebClient client = new WebClient())
                                            {
                                                using (Stream stream = client.OpenRead(imageUrl))
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
                                }

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
                                    var img = PIBDrawing.FixedSize(src, widthPerIcon, src.Height, makeTransparent);

                                    if ((x + img.Width - origX) > txtAttr.width)
                                    {
                                        y = y + img.Height;
                                        x = txtAttr.x;
                                    }

                                    g.DrawImage(img, x, y);
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
        #region PIB Device
        /// <summary>
        /// API calls to get pibTemplates
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="mac_address"></param>
        /// <returns>List of pibTemplates</returns>
        [HttpGet("get/device")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> GetApiPIBDevices(int? deviceId = null, string mac_address = null)
        {
            var result = await _unitOfWork.PIBTemplates.GetApiPIBDevices(deviceId, mac_address);
            var data = Mapper.Map<List<PIBDeviceViewModel>>(result.Data);

            if (deviceId.HasValue || !string.IsNullOrEmpty(mac_address))
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
        [ProducesResponseType(200, Type = typeof(List<PIBDeviceViewModel>))]
        public async Task<IActionResult> GetPIBDevices()
        {
            return await GetPIBDevices(-1, -1);
        }


        [HttpGet("devices/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllFacilityTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PIBDeviceViewModel>))]
        public async Task<IActionResult> GetPIBDevices(int pageNumber, int pageSize)
        {
            var results = await _unitOfWork.PIBTemplates.GetPIBDevicesLoadRelatedAsync(pageNumber, pageSize);
            return Ok(Mapper.Map<List<PIBDeviceViewModel>>(results));
        }

        [HttpPost("device")]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(201, Type = typeof(PIBDeviceViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePIBDevice([FromBody] PIBDeviceViewModel pibDevice)
        {
            if (ModelState.IsValid)
            {
                if (pibDevice == null)
                    return BadRequest($"{nameof(pibDevice)} cannot be null");

                var device = Mapper.Map<PIBDevice>(pibDevice);

                var result = await _unitOfWork.PIBTemplates.CreateDeviceAsync(device);
                if (result.IsSuccess)
                {
                    PIBDeviceViewModel pibDeviceVM = Mapper.Map<PIBDeviceViewModel>(result.Data);
                    return CreatedAtAction("GetPIBDeviceById", new { id = pibDeviceVM.Id }, pibDeviceVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("device/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(200, Type = typeof(PIBDeviceViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePIBDevice(int id)
        {
            if (!await _unitOfWork.PIBTemplates.TestCanDeleteAsync(id))
                return BadRequest("PIBDevice cannot be deleted."); //TODO: correct message here


            var pibDevice = await this._unitOfWork.PIBTemplates.GetByDeviceIdAsync(id);

            PIBDeviceViewModel pibDeviceVM = Mapper.Map<PIBDeviceViewModel>(pibDevice);
            if (pibDeviceVM == null)
                return NotFound(id);

            var result = await _unitOfWork.PIBTemplates.DeleteDeviceAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting pibDevice: " + string.Join(", ", result.Message));


            return Ok(pibDeviceVM);
        }

        [HttpPut("device/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllPIBDevicesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePIBDevice(string id, [FromBody] PIBDeviceViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var pibDevice = await this._unitOfWork.PIBTemplates.GetByDeviceIdAsync(model.Id);

                PIBDeviceViewModel pibDeviceVM = Mapper.Map<PIBDeviceViewModel>(pibDevice);
                if (pibDeviceVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<PIBDevice>(model);
                var result = await _unitOfWork.PIBTemplates.UpdateDeviceAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
        #endregion
        #region Private Methods
        private async Task<PatientInfo> GetPatientInfoByLocationCode(string apiURL)
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
                    patient = Mapper.Map<PatientInfo>(patient_info);
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

                    //CorporateRatesInfo dto = Mapper.Map<CorporateRatesInfo>(jsonRestrictions);
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

        private async Task<List<RestrictionType>> GetApiRestrictionTypes(string apiURL)
        {
            // var apiURL = "http://119.73.206.36:91/api/pib/GetTypes";
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

        private List<PIBTemplateLocation> GetApiLocations(string apiURL)
        {
            if (apiURL == null)
                throw new Exception("Location API URL is not set.");
            try
            {

                var locations = new List<PIBTemplateLocation>();
                var wards = new List<PIBTemplateLocationViewModel>();
                ////TODO: dummy beds for now
                //for (int i = 1; i <= 10; i++)
                //{
                //    string lbl = "BED " + i.ToString();
                //    locations.Add(new PIBTemplateLocationViewModel
                //    {
                //        alias = lbl,
                //        bed = true,
                //        code = lbl,
                //        hospital_id = 1,
                //        //Id = i,
                //        label = lbl,
                //        location_id = i,
                //        sequence = i,
                //        ward = false
                //    });
                //}


                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //TODO: hard-coded hospital id
                    string hospital_id = _configuration["AppSettings:pibHospitalId"];
                    string wardApiURL = string.Format("{0}/GetLocations", apiURL);

                    var response = client.PostAsync(wardApiURL, new StringContent(
                                    JsonConvert.SerializeObject(new
                                    {
                                        hospitalId = hospital_id,
                                        query = new
                                        {
                                            paged = false,
                                            activeOnly = true
                                        }
                                    }), Encoding.UTF8, "application/json")).Result;

                    response.EnsureSuccessStatusCode();
                    var content = response.Content.ReadAsStringAsync().Result;
                    var contentJson = JObject.Parse(content);
                    JArray jsonArray = contentJson["data"] as JArray;

                    //locations = Mapper.Map<List<PIBTemplateLocationViewModel>>(dataArray);

                    //JArray jsonArray = contentJson["data"] as JArray;

                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        if (jsonArray[i]["bed"] != null && jsonArray[i]["bed"].Value<bool>())
                        {
                            //var data = Mapper.Map<PIBTemplateLocationViewModel>(jsonArray[i]);
                            //PIBTemplateLocationViewModel data = new PIBTemplateLocationViewModel()
                            //{
                            //    location_id = jsonArray[i]["location_id"] != null ? jsonArray[i]["location_id"].Value<long>() : 0,
                            //    hospital_id = jsonArray[i]["hospital_id"] != null ? jsonArray[i]["hospital_id"].Value<long>() : 0,
                            //    code = jsonArray[i]["code"] != null ? jsonArray[i]["code"].Value<string>() : string.Empty,
                            //    label = jsonArray[i]["label"] != null ? jsonArray[i]["label"].Value<string>() : string.Empty,
                            //    alias = jsonArray[i]["alias"] != null ? jsonArray[i]["alias"].Value<string>() : string.Empty,
                            //    ward = jsonArray[i]["ward"] != null ? jsonArray[i]["ward"].Value<bool>() : false,
                            //    bed = jsonArray[i]["bed"] != null ? jsonArray[i]["bed"].Value<bool>() : false,
                            //    ancestor = jsonArray[i]["ancestor"] != null ? Mapper.Map<PIBTemplateLocationViewModel>(jsonArray[i]) : null

                            //};
                            var data = MapLocation(jsonArray[i] as JObject);
                            //data.ancestor = jsonArray[i]["ancestor"] != null ? MapLocation(jsonArray[i]["ancestor"] as JObject) : null;
                            if (jsonArray[i]["ancestor"] != null)
                            {
                                var ancestor = jsonArray[i]["ancestor"];
                                data.ancestor_id = ancestor["location_id"] != null ? ancestor["location_id"].Value<long?>() : null;
                                data.ancestor_code = ancestor["code"] != null ? ancestor["code"].Value<string>() : string.Empty;
                                data.ancestor_label = ancestor["label"] != null ? ancestor["label"].Value<string>() : string.Empty;
                                data.ancestor_alias = ancestor["alias"] != null ? ancestor["alias"].Value<string>() : string.Empty;
                            }

                            locations.Add(data);
                        }
                    }
                }

                return locations.OrderBy(e => e.label).ToList();
            }
            catch (Exception)
            {

                throw new Exception("Cannot connect to the api.");
            }
        }

        private PIBTemplateLocation MapLocation(JObject obj)
        {
            return new PIBTemplateLocation()
            {
                location_id = obj["location_id"] != null ? obj["location_id"].Value<long>() : 0,
                hospital_id = obj["hospital_id"] != null ? obj["hospital_id"].Value<long>() : 0,
                code = obj["code"] != null ? obj["code"].Value<string>() : string.Empty,
                label = obj["label"] != null ? obj["label"].Value<string>() : string.Empty,
                alias = obj["alias"] != null ? obj["alias"].Value<string>() : string.Empty,
                ward = obj["ward"] != null ? obj["ward"].Value<bool>() : false,
                bed = obj["bed"] != null ? obj["bed"].Value<bool>() : false
            };
        }

        private List<PIBTemplateLocationViewModel> GetApiBeds(string apiURL, long? ward_id = null)
        {
            if (apiURL == null)
                throw new Exception("Location API URL is not set.");

            var locations = new List<PIBTemplateLocationViewModel>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //TODO: hard-coded hospital id
                apiURL = string.Format("{0}/GetBeds?wardId={1}", apiURL, ward_id);
                var response = client.GetAsync(apiURL).Result;
                response.EnsureSuccessStatusCode();
                var content = response.Content.ReadAsStringAsync().Result;
                JArray jsonArray = JArray.Parse(content);
                locations = Mapper.Map<List<PIBTemplateLocationViewModel>>(jsonArray);
            }

            return locations.OrderBy(e => e.label).ToList();
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

        //private void ExecutePhantomJs(string apiUrl)
        //{
        //    var phantomJs = new PhantomJS();
        //    string scriptPath = _configuration["AppSettings:PHANTOMJS_SCRIPT"];
        //    var outputFilepath = @"C:\test\images\sheila1.png";
        //    phantomJs.Run(scriptPath, new[] { apiUrl, outputFilepath });
        //}
    }
}