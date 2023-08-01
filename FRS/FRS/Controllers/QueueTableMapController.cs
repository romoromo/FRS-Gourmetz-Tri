using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Models;
using DAL.Repositories;
using FRS.Hubs;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class QueueTableMapController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        private IHubContext<QueueHub> _queueHub;
        readonly ILogger _logger;
        private ApplicationSettingController _applicationSettingController;

        private const string tokenKey = "QUEUE_ACCESS_TOKEN_VALUE";
        private const string tokenExpireKey = "QUEUE_ACCESS_TOKEN_EXPIRE";


        public QueueTableMapController(IUnitOfWork unitOfWork, ILogger<QueueTableMapController> logger, IHubContext<QueueHub> queueHub, ApplicationSettingController applicationSettingController)
        {
            _unitOfWork = unitOfWork;
            _queueHub = queueHub;
            _logger = logger;
            _applicationSettingController = applicationSettingController;
        }


        [HttpGet("list")]
        //[Authorize(Authorization.Policies.ManageAllQueueTableMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<QueueTableMapViewModel>))]
        public async Task<IActionResult> GetQueueTableMaps(int? institutionId = null)
        {
            return await GetQueueTableMaps(-1, -1, institutionId);
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ManageAllQueueTableMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<QueueTableMapViewModel>))]
        public async Task<IActionResult> GetQueueTableMaps(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.QueueTableMaps.GetQueueTableMapsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(Mapper.Map<List<QueueTableMapViewModel>>(result));
        }

        [HttpPost("gettoken")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllQueueTableMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(QueueTableMapViewModel))]
        public async Task<IActionResult> GetToken([FromBody] Dictionary<string, string> param)
        {
            try
            {
                var username = param.ContainsKey("username") ? param["username"] : "";
                var password = param.ContainsKey("password") ? param["password"] : "";

                var dbusername = await _unitOfWork.ApplicationSettings.GetByKeyAsync("QUEUE_ACCESS_TOKEN_USERNAME").ConfigureAwait(false);
                var dbpassword = await _unitOfWork.ApplicationSettings.GetByKeyAsync("QUEUE_ACCESS_TOKEN_PASSWORD").ConfigureAwait(false);

                if (dbusername == null || dbpassword == null || username != dbusername.Value || password != dbpassword.Value) return BadRequest(new { ErrorCode = "ERR01", ErrorMessage = "Username or Password are not matched." });

                var expireminutes = await _unitOfWork.ApplicationSettings.GetByKeyAsync("QUEUE_ACCESS_TOKEN_VALID_MINUTES").ConfigureAwait(false);

                var expiretime = await _unitOfWork.ApplicationSettings.GetByKeyAsync(tokenExpireKey).ConfigureAwait(false);

                var token = await _unitOfWork.ApplicationSettings.GetByKeyAsync(tokenKey).ConfigureAwait(false);


                if (expiretime != null && DateTime.Now < DateTime.Parse(expiretime.Value))
                {
                    return Ok(new { token = token.Value, expire = expiretime.Value });
                }
                else
                {
                    var tokenstr = RandomString(64);
                    var timedate = DateTime.Now;

                    var minutes = expireminutes?.Value ?? "5";
                    var intminutes = Int32.Parse(minutes);

                    timedate = timedate.AddMinutes(intminutes);

                    var timedatestr = timedate.ToString("yyyy-MM-dd HH:mm:ss");

                    if (token == null) await _applicationSettingController.CreateApplicationSetting(new ApplicationSettingViewModel
                    {
                        Key = tokenKey,
                        Value = tokenstr,
                    });
                    else
                    {
                        token.Value = tokenstr;
                        await _applicationSettingController.UpdateApplicationSetting(token.Id.ToString(), Mapper.Map<ApplicationSettingViewModel>(token));

                    }

                    if (expiretime == null) await _applicationSettingController.CreateApplicationSetting(new ApplicationSettingViewModel
                    {
                        Key = tokenExpireKey,
                        Value = timedatestr,
                    });
                    else
                    {
                        expiretime.Value = timedatestr;
                        await _applicationSettingController.UpdateApplicationSetting(expiretime.Id.ToString(), Mapper.Map<ApplicationSettingViewModel>(expiretime));

                    }

                    return Ok(new { token = tokenstr, expire = timedatestr });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        public static string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllQueueTableMapsPolicy)]
        [ProducesResponseType(201, Type = typeof(QueueTableMapViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateQueueTableMap([FromBody] QueueTableMapViewModel queueTableMap)
        {
            if (ModelState.IsValid)
            {
                if (queueTableMap == null)
                    return BadRequest($"{nameof(queueTableMap)} cannot be null");


                var type = Mapper.Map<QueueTableMap>(queueTableMap);

                var result = await _unitOfWork.QueueTableMaps.CreateAsync(type);
                if (result.IsSuccess)
                {
                    QueueTableMapViewModel queueTableMapVM = Mapper.Map<QueueTableMapViewModel>(result.Data);
                    return CreatedAtAction("GetQueueTableMapById", new { id = queueTableMapVM.Id }, queueTableMapVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }

        [HttpPost("callqueue")]
        //[AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(QueueTableMapViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CallQueue([FromBody] QueueLogViewModel param)
        {
            //if (param.isT)
            //{
            //    var expiretime = await _unitOfWork.ApplicationSettings.GetByKeyAsync(tokenExpireKey).ConfigureAwait(false);
            //    var token = await _unitOfWork.ApplicationSettings.GetByKeyAsync(tokenKey).ConfigureAwait(false);

            //    if (token == null || token.Value != param.Token)
            //        return BadRequest(new { ErrorCode = "ERR02", ErrorMessage = "Access Token is not valid." });

            //    if (expiretime == null || DateTime.Now > DateTime.Parse(expiretime.Value))
            //        return BadRequest(new { ErrorCode = "ERR02", ErrorMessage = "Access Token is not valid." });
            //}


            var result = await _unitOfWork.QueueTableMaps.CallQueue(Mapper.Map<QueueLog>(param));

            if (!result.IsSuccess)
            {
                if (result.Message == "Queue Id not exist!")
                    return BadRequest(new { ErrorCode = "ERR03", ErrorMessage = "Queue Id is not exist." });

                if (result.Message == "Failed to save!")
                    return BadRequest(new { ErrorCode = "ERR05", ErrorMessage = "Update data is failed, please try again." });
            }

            var list = await _unitOfWork.QueueTableMaps.DeviceLists(Mapper.Map<QueueLog>(param));

            if (list == null || list.Count() == 0)
                return BadRequest(new { ErrorCode = "ERR06", ErrorMessage = "No device was found with Queue Id." });

            foreach (var l in list)
            {
                param.StationId = l.StationId;
                param.RoomName = l.RoomName;

                if (l.Device != null && param.CallAction != QueueTableMapRepository.silentcall) await _queueHub.Clients.Group(l.Device.Code).SendAsync(QueueHub.UpdateQueueKey, param);
            }

            return Ok(result);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllQueueTableMapsPolicy)]
        [ProducesResponseType(200, Type = typeof(QueueTableMapViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteQueueTableMap(int id)
        {
            var queueTableMap = await this._unitOfWork.QueueTableMaps.GetByIdAsync(id);

            QueueTableMapViewModel queueTableMapVM = Mapper.Map<QueueTableMapViewModel>(queueTableMap);
            if (queueTableMapVM == null)
                return NotFound(id);

            var result = await _unitOfWork.QueueTableMaps.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(queueTableMapVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllQueueTableMapsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateQueueTableMap(string id, [FromBody] QueueTableMapViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var queueTableMap = await this._unitOfWork.QueueTableMaps.GetByIdAsync(model.Id);

                QueueTableMapViewModel queueTableMapVM = Mapper.Map<QueueTableMapViewModel>(queueTableMap);
                if (queueTableMapVM == null)
                    return NotFound(id);

                var updatedModel = Mapper.Map<QueueTableMap>(model);
                var result = await _unitOfWork.QueueTableMaps.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }
    }
}