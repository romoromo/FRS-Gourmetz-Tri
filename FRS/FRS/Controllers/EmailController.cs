using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core.Interfaces;
using DAL.Models;
using FRS.Helpers;
using FRS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FRS.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailSender _emailSender;
        private IUnitOfWork _unitOfWork;
        private readonly IAccountManager _accountManager;
        private readonly IEmailQueueService _emailService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public EmailController(IEmailSender emailSender, IUnitOfWork unitOfWork, IAccountManager accountManager, ILoggerFactory loggerFactory,
            IEmailQueueService emailService, IMapper mapper)
        {
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _accountManager = accountManager;
            _emailService = emailService;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<EmailController>();
        }

        #region Email
        public async Task<IActionResult> SendEmailFromQueue()
        {
            try
            {
                var emailQueues = await this._unitOfWork.EmailQueues.GetAllUnsentEmailAsync();

                foreach(var email in emailQueues)
                {
                    var response = await _emailSender.SendEmailAsync(email.FromName, email.FromEmail, email.ToName, email.ToEmail, email.Subject, email.Body);
                    var eqDTO = _mapper.Map<EmailQueueDTO>(email);
                    eqDTO.IsSent = response.success;
                    eqDTO.IsFailed = !response.success;
                    if (!response.success)
                    {
                        eqDTO.FailMessage = response.errorMsg;
                    }

                    if (eqDTO.IsSent)
                    {
                        eqDTO.SentDate = DateTime.Now;
                    }

                    await _emailService.UpdateEmailQueueAsync(eqDTO);
                }

                _logger.LogInformation("All emails are sent!");
                return Ok("All emails are sent!");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("Error occurred while sending emails. [ERROR]: {0}", ex.Message));
                _logger.LogInformation(string.Format("Error occurred while sending emails. [EXCEPTION]: {0}", ex.InnerException != null ? ex.InnerException.ToString() : string.Empty));
                return Ok("Failed" + ex.InnerException);
            }
        }
        #endregion
    }
}