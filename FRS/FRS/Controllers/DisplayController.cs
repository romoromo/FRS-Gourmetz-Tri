using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class DisplayController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public DisplayController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        //[AllowAnonymous]
        [HttpGet("{mac}")]
        public async Task<ContentResult> Index(string mac)
        {
            string defaultUrl = "/devicewarning";
            string url = string.Empty;
            if (!string.IsNullOrEmpty(mac))
            {
                var device = await _unitOfWork.Devices.GetByDeviceIdentifier(mac);
                if (device != null)
                {
                    if (!string.IsNullOrEmpty(device.module_path_value))
                    {
                        string baseUrl = _configuration["AppSettings:baseUrl"];
                        baseUrl = baseUrl.EndsWith("/") ? baseUrl.Substring(0, baseUrl.Length - 1) : baseUrl;

                        if (device.module_path_value.Contains("/pibdisplay") 
                             || device.module_path_value.Contains("/nursecheckincounter")
                             || device.module_path_value.Contains("/communicatorpatientdisplay"))
                        {
                            url = string.Format("{0}{1}/{2}/{3}", device.Module != null ? device.Module.Uri : string.Empty, device.module_path_value, device.MacAddress, "FRS");
                        }
                        else if (device.module_path_value.Contains("/signagedisplay"))
                        {
                            url = string.Format("{0}{1}/{2}/{3}", device.Module != null ? device.Module.Uri : string.Empty, device.module_path_value, mac, device.PublicationId);
                        }
                        else
                        {
                            url = string.Format("{0}{1}/{2}", device.Module != null ? device.Module.Uri : string.Empty, device.module_path_value, device.MacAddress);
                        }

                    }
                    else
                    {
                        url = string.Format("{0}/{1}/{2}/{3}", defaultUrl, "adminreq", device.Id, device.MacAddress);
                    }
                }
                else
                {
                    //register new device
                    var result = await _unitOfWork.Devices.UpdateDeviceStatus(mac, mac, string.Empty);
                    if (result.IsSuccess)
                    {
                        dynamic data = result.Data;
                        var objectData = data.GetType().GetProperty("data").GetValue(data, null);
                        device = objectData as Device;

                        url = string.Format("{0}/{1}/{2}/{3}", defaultUrl, "adminreq", device.Id, mac);
                    }
                    else
                    {
                        url = string.Format("{0}/{1}", defaultUrl, "new_error");
                    }
                }
            }
            else
            {
                url = string.Format("{0}/{1}", defaultUrl, "macreq");
            }

            //return Redirect(url);

            //var response = new HttpResponseMessage();
            var html = $"<html><head><meta http-equiv=\"refresh\" content=\"0; url = {url}\" /></head><body></body></html>";
            //response.Content = new StringContent(html);
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Content(html, "text/html", Encoding.UTF8); ;
        }
    }
}