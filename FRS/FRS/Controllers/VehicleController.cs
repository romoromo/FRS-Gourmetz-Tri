using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Core.DTO;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/vehicle")]
    [ApiController]
    public class VehicleController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public VehicleController(IUnitOfWork unitOfWork, ILogger<ContactGroupController> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }


        /// <summary>
        /// Vehicle API to update arrival and departure
        /// </summary>
        /// <param name="queryModel">Contains a list of objects with season ID for vehicles which arrived/departed in the parking area, a type of entry (ENTRY/EXIT), and the timestamp.</param>
        /// <returns>An operation response</returns>
        /// <remarks>
        /// Sample Request 1:
        ///
        ///     POST /api/vehicle/arrival
        ///     {
        ///         "vehicles": [{
        ///             "seasonId": "1",
        ///             "type": "ENTRY",
        ///             "timestamp": "2020-07-01 14:00:00"
        ///         },
        ///         {
        ///             "seasonId": "1",
        ///             "type": "EXIT",
        ///             "timestamp": "2020-07-01 16:30:00"
        ///         },
        ///         {
        ///             "seasonId": "2",
        ///             "type": "ENTRY",
        ///             "timestamp": "2020-07-01 09:20:00"
        ///         }]
        ///     }
        ///
        /// Response Body for a successful call:
        ///
        ///     {
        ///        "isSuccess" : true, 
        ///        "messsage": "Update successful!"
        ///     }
        ///     
        /// Response Body for unsuccessful call:
        ///
        ///     {
        ///        "isSuccess" : false, 
        ///        "messsage": "An error occurred during update. Please contact FRS Admin."
        ///     }
        ///
        /// Sample Request 2:
        ///
        ///     POST /api/vehicle/arrival
        ///     {
        ///         "vehicles": [{
        ///             "seasonId": "",
        ///             "type": "ENTRY",
        ///             "timestamp": "2020-07-01 14:00:00"
        ///         },
        ///         {
        ///             "seasonId": "2",
        ///             "type": "ENTRY",
        ///             "timestamp": "2020-07-01 16:30:00"
        ///         }]
        ///     }
        ///
        /// Response Body for unsuccessful call:
        ///
        ///     {
        ///        "isSuccess" : false, 
        ///        "messsage": "Parameter "seasonId" is null or empty in one of the vehicles"
        ///     }
        ///
        /// Sample Request 3:
        ///
        ///     POST /api/vehicle/arrival
        ///     {
        ///         "vehicles": [{
        ///             "seasonId": "1",
        ///             "type": "",
        ///             "timestamp": "2020-07-01 14:00:00"
        ///         },
        ///         {
        ///             "seasonId": "2",
        ///             "type": "EXIT",
        ///             "timestamp": "2020-07-01 16:30:00"
        ///         }]
        ///     }
        ///
        /// Response Body for unsuccessful call:
        ///
        ///     {
        ///        "isSuccess" : false, 
        ///        "messsage": "Parameter "type" is null or empty in one of the vehicles"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Server internal error</response>
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(500)]
        [HttpPost("arrival")]
        public async Task<IActionResult> UpdateArrival(VechicleEventQueryModel query)
        {
            var response = new BaseOperationResponse();

            if (query == null || query.Vehicles == null || query.Vehicles.Count == 0)
            {
                response.IsSuccess = false;
                response.Message = "Parameter cannot be null or empty";
            }
            else
            {
                //process the model
                response = await this._unitOfWork.Reservations.UpdateArrival(query.Vehicles);
            }

            return Ok(response);
        }

        /// <summary>
        /// Vehicle API to insert vehicle records to VVMS
        /// </summary>
        /// <param name="vehicles">A list of vehicle object to register in VVMS.</param>
        /// <returns>An operation response</returns>
        /// <remarks>
        /// Sample Request 1:
        ///
        ///     POST /api/vehicle/registration
        ///     {
        ///        "vehicles": [
        ///        {
	    ///             "seasonId"	:	"1"
	    ///             "personName": 	"John Doe"
	    ///             "cardType"	:	"Type 1"
	    ///             "vehicle"	:	"PLATE NO 1"
	    ///             "issueDate"	:	"2020-01-01"
	    ///             "expiryDate":	"2025-01-01"
        ///         },
        ///         {
	    ///             "seasonId"	:	"2"
	    ///             "personName": 	"Lee Min"
	    ///             "cardType"	:	"Type 1"
	    ///             "vehicle"	:	"PLATE NO 2"
	    ///             "issueDate"	:	"2019-12-31"
	    ///             "expiryDate":	"2024-12-31"
        ///         }
        ///         ]
        ///     }
        ///
        /// Response Body for a successful call:
        ///
        ///     {
        ///        "isSuccess" : true, 
        ///        "messsage": "Registration successful!"
        ///     }
        ///     
        /// Response Body for unsuccessful call:
        ///
        ///     {
        ///        "isSuccess" : false, 
        ///        "messsage": "An error occurred during registration. Error details..."
        ///     }
        ///
        /// Sample Request 2:
        ///
        ///     POST /api/vehicle/registration
        ///     {
        ///        "vehicles": []
        ///     }
        ///
        /// Response Body for unsuccessful call:
        ///
        ///     {
        ///        "isSuccess" : false, 
        ///        "messsage": "Parameter vehicles is null or empty"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Server internal error</response>
        /// 
        [HttpPost("registration")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> PostToVvms([FromBody] List<VMSVehiclePostRequestModel> vehicles)
        {
            var response = new BaseOperationResponse();

            if (vehicles == null || vehicles.Count == 0)
            {
                response.IsSuccess = false;
                response.Message = "Parameter cannot be null or empty";
            }
            else
            {
                //process the model
                //string apiUrl = _configuration["AppSettings:VMS_API_URL"];
                response = await this._unitOfWork.Reservations.RegisterVehicleToVMS(vehicles);
            }

            return Ok(response);
        }
    }
}