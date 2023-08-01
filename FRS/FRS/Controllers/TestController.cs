using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using FRS.Helpers;
using FRS.Hubs;
using FRS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace FRS.Controllers
{
    public class TestController : Controller
    {
        private readonly IEmailSender _emailSender;
        private IUnitOfWork _unitOfWork;
        private readonly IAccountManager _accountManager;
        private readonly IWalletService _walletService;
        private readonly IRewardService _rewardService;
        private readonly INotificationService _notificationService;
        private IHubContext<UserHub> _userHub;

        public TestController(IEmailSender emailSender, IUnitOfWork unitOfWork, IAccountManager accountManager, IWalletService walletService, IRewardService rewardService, IHubContext<UserHub> userHub,
                INotificationService notificationService)
        {
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _accountManager = accountManager;
            _walletService = walletService;
            _rewardService = rewardService;
            _notificationService = notificationService;
            _userHub = userHub;
        }

        #region Test Email
        public async Task<IActionResult> TestEmail(string email)
        {
            try
            {
                var emailBody = EmailTemplates.GetPlainTextTestEmail(DateTime.Now);
                //var isSuccess = await _emailSender.SendEmailAsync("Test Recipient", string.IsNullOrEmpty(email) ? "smv.notification@gmail.com" : email, "Test Email", emailBody);
                var isSuccess = await _emailSender.SendEmailAsync("Test Recipient", "smv.notification@gmail.com", "Test", email, "Test Email", emailBody);
                return Ok(isSuccess.success ? "Success" : "Failed: " + isSuccess.errorMsg);
            }
            catch (Exception ex)
            {
                return Ok("Failed" + ex.InnerException);
            }
        }
        #endregion

        #region Test Notification
        public async Task<IActionResult> TestNotification(string email)
        {
            try
            {
                var user = await _accountManager.GetUserByEmailAsync(email);
                var notification = new NotificationDTO
                {
                    Body = "This is a test notification.",
                    Date = DateTime.Now,
                    EventId = 1,
                    Header = "Test Notification",
                    UserId = user.Id
                };

                var response = await _notificationService.CreateAsync(notification);

                if(response.IsSuccess && !string.IsNullOrEmpty(email))
                    await _userHub.Clients.Group(email).SendAsync("NotificationTriggered", response.Data);

                return Ok(response.IsSuccess ? "Success" : "Failed: " + response.Message);
            }
            catch (Exception ex)
            {
                return Ok("Failed" + ex.InnerException);
            }
        }
        #endregion

        #region Create Institution
        public async Task<IActionResult> CreateNewInstitution(string institutionCode)
        {
            try
            {
                //1. create institution, check if it has unique code
                var institution = await _unitOfWork.Institutions.GetByCodeAsync(institutionCode) ?? new Institution
                {
                    Name = institutionCode,
                    Description = institutionCode,
                    StartTime = 8,
                    EndTime = 20,
                    IsActive = true
                };

                if (institution.Id > 0)
                {
                    throw new Exception("Code already exists");
                }

                var newInstitution = Mapper.Map<InstitutionViewModel>((await _unitOfWork.Institutions.CreateAsync(institution)).Data);

                //2. Create default admin role
                string role = institutionCode + " Default Admin";
                var newRole = await _accountManager.CreateRoleAsync(new ApplicationRole
                {
                    InstitutionId = newInstitution.Id,
                    IsActive = true,
                    Name = role,
                    NormalizedName = role,
                    Description = role
                }, DAL.Core.ApplicationPermissionsTrees.GetAllPermissionValues());

                //3.Create default department
                var newDepartment = Mapper.Map<DepartmentViewModel>((await _unitOfWork.Departments.CreateAsync(new Department
                {
                    InstitutionId = newInstitution.Id,
                    Name = "Default Department",
                    Description = "Default Department",
                    IsActive = true
                })).Data);

                var adminRole = await _accountManager.GetRoleLoadRelatedAsync(role, newInstitution.Id);
                //4. Create default admin user
                string email = "admin_" + institutionCode + "@" + institutionCode + ".com";
                var newUser = await _accountManager.CreateUserAsync(new ApplicationUser
                {
                    DepartmentId = newDepartment.Id,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "default admin",
                    InstitutionId = newInstitution.Id,
                    IsActive = true,
                    IsEnabled = true,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper(),
                    PhoneNumber = "111111",
                    Pin = "888888",
                    UserName = email
                }, new string[] { role }, "smv888");

                var locationType = await _unitOfWork.Locations.GetLocationTypes();
                //5. Create default Location
                var newLocationRoot = await _unitOfWork.Locations.CreateAsync(new Location
                {
                    Capacity = 1,
                    Description = institutionCode,
                    InstitutionId = newInstitution.Id,
                    IsActive = true,
                    Name = institutionCode,
                    LocationTypeId = locationType.FirstOrDefault(e => e.Name == "Hospital").Id
                }, new List<int>(), new List<int>(), new List<int> { newInstitution.Id }, null);

                var rootLocation = AutoMapper.Mapper.Map<LocationViewModel>(newLocationRoot.Data);
                var newLocationBuilding = await _unitOfWork.Locations.CreateAsync(new Location
                {
                    Capacity = 1,
                    Description = institutionCode + " Building 1",
                    InstitutionId = newInstitution.Id,
                    IsActive = true,
                    Name = institutionCode + " Building 1",
                    LocationTypeId = locationType.FirstOrDefault(e => e.Name == "Building").Id,
                    ParentLocationId = rootLocation.Id
                }, new List<int>(), new List<int>(), new List<int> { newInstitution.Id }, null);

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok("Failed" + ex.InnerException);
            }
        }
        #endregion

        #region Create Default User Wallet
        public async Task<IActionResult> GenerateUserWallet()
        {
            try
            {
                BaseFilter filter = new BaseFilter { Filters = null, Page = 1 };
                var pagedUsers = await _accountManager.GetUsersAsync(filter);
                var users = pagedUsers.PagedData;
                foreach (var user in users)
                {
                    if (!user.UserWallets.Any())
                    {
                        var dto = new WalletDTO
                        {
                            Balance = 0,
                            UserId = user.Id
                        };

                        await this._walletService.CreateAsync(dto);
                    }

                    if (!user.UserRewards.Any())
                    {
                        var dto = new RewardDTO
                        {
                            Balance = 0,
                            UserId = user.Id
                        };

                        await this._rewardService.CreateAsync(dto);
                    }

                }
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok("Failed" + ex.InnerException);
            }
        }
        #endregion

        #region Generate Device
        public async Task<IActionResult> GenerateDevices(int count = 10000)
        {
            try
            {
                List<Device> randomDevices = new List<Device>();

                for (int i = 0; i < count; i++)
                {
                    string name = string.Format("device_{0}{1}", i, DateTime.Now.ToString(@"ddhhmm"));
                    var device = new Device
                    {
                        Brightness = 1,
                        Code = name,
                        device_label = name,
                        IpAddress = name,
                        MacAddress = name
                    };
                    await _unitOfWork.Devices.CreateAsync(device);
                }

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok("Failed" + ex.InnerException);
            }
        }
        #endregion

        #region Generate Default Claims
        public async Task<IActionResult> GenerateDefaultRoleClaims(string role, int? institutionId)
        {
            try
            {
                var existingRole = await _accountManager.GetRoleLoadRelatedAsync(role, institutionId);
                await _accountManager.UpdateRoleAsync(existingRole,
                    DAL.Core.ApplicationPermissionsTrees.AllPermissions.Select(e => e.Value).ToArray());
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok("Failed" + ex.InnerException);
            }
        }
        #endregion

        #region Generate ACL
        public async Task<IActionResult> GenerateACL()
        {
            var tree = ApplicationPermissionsTrees.Tree();
            var json = JsonConvert.SerializeObject(tree, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
            return Ok(Mapper.Map<ApplicationPermissionsTreeDTO>(tree));
        }
        #endregion
    }
}