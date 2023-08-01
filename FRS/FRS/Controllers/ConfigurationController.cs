using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {
        private readonly IConfiguration _configuration;
        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult ConfigurationData()
        {
            return Ok(new Dictionary<string, string>
            {
                { "baseUrl", _configuration["AppSettings:baseUrl"] },
                { "downloadPath", _configuration["AppSettings:downloadPath"] },
                { "androidQRUrl", _configuration["AppSettings:androidQRUrl"] }
            });
        }
    }
}

