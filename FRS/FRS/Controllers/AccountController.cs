using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FRS.ViewModels;
using AutoMapper;
using DAL.Models;
using DAL.Core.Interfaces;
using FRS.Authorization;
using FRS.Helpers;
using Microsoft.AspNetCore.JsonPatch;
using DAL.Core;
using OpenIddict.Validation.AspNetCore;
using DAL;
using Microsoft.Extensions.Configuration;
using System.Web;
using DAL.Core.DTO;
using Microsoft.AspNetCore.SignalR;
using FRS.Hubs;
using FRS.Attributes;
using DAL.Filters;
using BAL.Services.Interfaces;
using BAL.DTO;
using DAL.Core.Helpers;
using BAL.Services.Interfaces.MealOrder;
using Microsoft.Extensions.Logging;
using DAL.Core.Logging;
using Newtonsoft.Json;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IAccountManager _accountManager;
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;
        private IUnitOfWork _unitOfWork;
        private readonly ApplicationUserManager _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private const string GetUserByIdActionName = "GetUserById";
        private const string GetRoleByIdActionName = "GetRoleById";
        private IHubContext<UserHub> _userHub;
        private IWalletService _walletService;
        private IRewardService _rewardService;
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AccountController(IAccountManager accountManager, IAuthorizationService authorizationService, IUnitOfWork unitOfWork,
            ApplicationUserManager userManager, IEmailSender emailSender, IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager, IWalletService walletService, IRewardService rewardService,
            IHubContext<UserHub> userHub, IStudentService studentService, IMapper mapper)
        {
            _accountManager = accountManager;
            _authorizationService = authorizationService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _signInManager = signInManager;
            _userHub = userHub;
            _walletService = walletService;
            _rewardService = rewardService;
            _studentService = studentService;
            _mapper = mapper;
            _logger = Logger.CreateLogger<AccountController>();
        }

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("users/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUsers(BaseFilter filter)
        {
            var users = await _accountManager.GetUsersAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<UserViewModel>>(users));
        }

        [ApiKeyAuthorize]
        [HttpGet("roles/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRoles(BaseFilter filter)
        {
            var roles = await _accountManager.GetRolesAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<RoleViewModel>>(roles));
        }
        #endregion

        [HttpGet("users/me")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<IActionResult> GetCurrentUser()
        {
            return await GetUserByUserName(this.User.Identity.Name);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("users/{id}", Name = GetUserByIdActionName)]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (!(await _authorizationService.AuthorizeAsync(this.User, id, AccountManagementOperations.Read)).Succeeded)
                return new ChallengeResult();


            UserViewModel userVM = await GetUserViewModelHelper(id);

            if (userVM != null)
                return Ok(userVM);
            else
                return NotFound(id);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("users/email/{email}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByEmail2(string email)
        {
            ApplicationUser appUser = await _accountManager.GetUserByEmailAsync(email);

            //if (!(await _authorizationService.AuthorizeAsync(this.User, appUser?.Id ?? null, AccountManagementOperations.Read)).Succeeded)
            //    return new ChallengeResult();

            if (appUser == null)
            {
                //email is used as identifier. Need to recheck using email


                appUser = await _accountManager.GetUserByUserNameAsync(email);
                if (appUser == null)
                    return NotFound(null);
            }

            //UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);

            var userVM = _mapper.Map<UserSimpleViewModel>(appUser);

            if (userVM != null)
                return Ok(userVM);
            else
                return NotFound(null);
        }

        //[HttpGet("users/id/{id}")]
        ////[AllowAnonymous]
        //[ProducesResponseType(200, Type = typeof(UserViewModel))]
        //[ProducesResponseType(403)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetUserById2(int id)
        //{
        //    ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

        //    if (appUser == null)
        //        return NotFound(null);



        //    var userVM = _mapper.Map<UserSimpleViewModel>(appUser);

        //    if (userVM != null)
        //        return Ok(userVM);
        //    else
        //        return NotFound(null);
        //}

        [HttpGet("users/username/{userName}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            ApplicationUser appUser = await _accountManager.GetUserByUserNameAsync(userName);

            //if (!(await _authorizationService.AuthorizeAsync(this.User, appUser?.Id ?? null, AccountManagementOperations.Read)).Succeeded)
            //    return new ChallengeResult();

            if (appUser == null)
            {
                //email is used as identifier. Need to recheck using email
                appUser = await _accountManager.GetUserByEmailAsync(userName);
                if (appUser == null)
                    return NotFound(null);
            }

            //UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);

            var userVM = _mapper.Map<UserViewModel>(appUser);

            if (userVM != null)
                return Ok(userVM);
            else
                return NotFound(null);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("users/exist/{email}")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            ApplicationUser appUser = await _accountManager.GetUserByUserNameAsync(email);

            if (appUser == null)
            {
                appUser = await _accountManager.GetUserByEmailAsync(email);
            }

            //if (!(await _authorizationService.AuthorizeAsync(this.User, appUser?.Id ?? null, AccountManagementOperations.Read)).Succeeded)
            //    return new ChallengeResult();

            if (appUser == null)
                return NotFound(null);

            //UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);

            var userVM = _mapper.Map<UserViewModel>(appUser);

            if (userVM != null)
                return Ok(userVM);
            else
                return NotFound(null);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("users")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetUsers(int? institutionId = null)
        {
            return await GetUsers(-1, -1, institutionId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("users/{pageNumber:int}/{pageSize:int}")]
        [Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetUsers(int pageNumber, int pageSize, int? institutionId = null)
        {
            var usersAndRoles = await _accountManager.GetUsersAndRolesAsync(pageNumber, pageSize, institutionId);

            List<UserViewModel> usersVM = new List<UserViewModel>();

            foreach (var item in usersAndRoles)
            {
                var userVM = _mapper.Map<UserViewModel>(item.Item1);
                userVM.Roles = item.Item2;

                usersVM.Add(userVM);
            }

            return Ok(usersVM);
        }


        [HttpPut("users/me")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserEditViewModel user)
        {
            return await UpdateUser(Utilities.GetUserId(this.User), user);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("users/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserEditViewModel user)
        {
            string dataUserJson = JsonConvert.SerializeObject(user);
            _logger.LogInformation($"UpdateUser UserEditViewModel : {dataUserJson}");
            _logger.LogInformation($"UpdateUser Id : {id}");

            try
            {
                ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);
                bool sendEmail = appUser != null && !appUser.IsEnabled;//(!appUser.DepartmentId.HasValue || !appUser.Roles.Any() ||);
                string[] currentRoles = appUser != null ? (await _accountManager.GetUserRolesAsync(appUser)).ToArray() : null;

                var manageUsersPolicy = _authorizationService.AuthorizeAsync(this.User, id, AccountManagementOperations.Update);
                var assignRolePolicy = _authorizationService.AuthorizeAsync(this.User, Tuple.Create(user.Roles, currentRoles), Authorization.Policies.AssignAllowedRolesPolicy);


                if ((await Task.WhenAll(manageUsersPolicy, assignRolePolicy)).Any(r => !r.Succeeded))
                    return new ChallengeResult();


                if (ModelState.IsValid)
                {
                    if (user == null)
                        return BadRequest($"{nameof(user)} cannot be null");

                    if (id != user.Id)
                        return BadRequest("Conflicting user id in parameter and model data");

                    if (appUser == null)
                        return NotFound(id);


                    if (Utilities.GetUserId(this.User) == id && string.IsNullOrWhiteSpace(user.CurrentPassword))
                    {
                        if (!string.IsNullOrWhiteSpace(user.NewPassword))
                            return BadRequest("Current password is required when changing your own password");

                        if (appUser.UserName != user.UserName)
                            return BadRequest("Current password is required when changing your own username");
                    }


                    bool isValid = true;

                    if (Utilities.GetUserId(this.User) == id && (appUser.UserName != user.UserName || !string.IsNullOrWhiteSpace(user.NewPassword)))
                    {
                        if (!await _accountManager.CheckPasswordAsync(appUser, user.CurrentPassword))
                        {
                            isValid = false;
                            AddErrors(new string[] { "The username/password couple is invalid." });
                        }
                    }

                    if (isValid)
                    {
                        _mapper.Map<UserViewModel, ApplicationUser>(user, appUser);
                        if (sendEmail) appUser.IsEnabled = true;

                        var result = await _accountManager.UpdateUserAsync(appUser, user.Roles);
                        if (result.Item1)
                        {
                            //broadcast newly updated user
                            UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);
                            await _userHub.Clients.All.SendAsync("BroadcastUpdatedUser", userVM);

                            if (!string.IsNullOrWhiteSpace(user.NewPassword))
                            {
                                if (!string.IsNullOrWhiteSpace(user.CurrentPassword))
                                    result = await _accountManager.UpdatePasswordAsync(appUser, user.CurrentPassword, user.NewPassword);
                                else
                                    result = await _accountManager.ResetPasswordAsync(appUser, user.NewPassword);
                            }

                            if (result.Item1)
                            {
                                //send email
                                //if (sendEmail)
                                //{
                                //    var url = _configuration["AppSettings:baseUrl"];
                                //    await _emailSender.SendEmailAsync(user.FullName, user.Email, "FRS Account Ready",
                                //        EmailTemplates.GetAccountReadyEmail(user.Email, url));
                                //}
                                return NoContent();
                            }
                        }

                        AddErrors(result.Item2);
                    }
                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error UpdateUser : {ex.Message}", ex);
                _logger.LogError($"Error UpdateUser : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [HttpGet("users/report/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUserReport(BaseFilter filter)
        {
            var logs = await _accountManager.GetUserReportAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<UserReportDTO>>(logs));
        }

        [HttpPost("users/report/export")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateUserReportXls(BaseFilter filter)
        {
            var xls = await _accountManager.GenerateUserReportXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_UserReport.xlsx";

            if (xls == null || xls.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: xls,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: reportName
            );
        }

        [HttpGet("roles/report/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllDepartmentsPolicy)]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRoleReport(RoleReportFilter filter)
        {
            var logs = await _accountManager.GetRoleReportAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<RoleReportDTO>>(logs));
        }

        [HttpPost("roles/report/export")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateRoleReportXls(RoleReportFilter filter)
        {
            var xls = await _accountManager.GenerateRoleReportXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_RoleReport.xlsx";

            if (xls == null || xls.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: xls,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: reportName
            );
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("changepassword/{id}")]
        //[AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] UserEditViewModel user)
        {
            string dataUserJson = JsonConvert.SerializeObject(user);
            _logger.LogInformation($"ChangePassword UserEditViewModel : {dataUserJson}");
            _logger.LogInformation($"ChangePassword Id : {id}");

            try
            {
                ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

                bool isValid = true;


                if (string.IsNullOrWhiteSpace(user.CurrentPassword) || !await _accountManager.CheckPasswordAsync(appUser, user.CurrentPassword))
                {
                    isValid = false;
                    return BadRequest(new { Error = "Invalid", ErrorDescription = "The username/password couple is invalid." });
                }

                if (!string.IsNullOrWhiteSpace(user.NewPassword) && !string.IsNullOrWhiteSpace(user.CurrentPassword))
                {
                    await _accountManager.UpdatePasswordAsync(appUser, user.CurrentPassword, user.NewPassword);
                }
                else
                {
                    return BadRequest(new { Error = "Invalid", ErrorDescription = "Need to provide Current Password and New Password." } );
                }

                return Ok(new { Error = "", ErrorDescription = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ChangePassword : {ex.Message}", ex);
                _logger.LogError($"Error ChangePassword : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("changeemail/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChangeEmail(int id, [FromBody] UserEditEmailViewModel user)
        {
            string dataUserJson = JsonConvert.SerializeObject(user);
            _logger.LogInformation($"ChangeEmail UserEditEmailViewModel : {dataUserJson}");
            _logger.LogInformation($"ChangeEmail Id : {id}");
            try
            {
                ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

                bool isValid = true;


                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    await _accountManager.UpdateEmailAsync(appUser, user.Email);
                }
                else
                {
                    return BadRequest(new { Error = "Invalid", ErrorDescription = "Need to provide email." });
                }

                return Ok(new { Error = "", ErrorDescription = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ChangeEmail : {ex.Message}", ex);
                _logger.LogError($"Error ChangeEmail : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("students/changeemail/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChangeStudentEmail(int id, [FromBody] StudentEditEmailViewModel student)
        {
            string dataJSON = JsonConvert.SerializeObject(student);
            _logger.LogInformation($"ChangeStudentEmail StudentEditEmailViewModel : {dataJSON}");
            _logger.LogInformation($"ChangeStudentEmail Id : {id}");
            try
            {
                if (string.IsNullOrWhiteSpace(student.NewEmail))
                {
                    throw new Exception("Invalid email.");
                }

                var dto = await this._studentService.GetStudentByEmailAsync(student.CurrentEmail);
                if (dto == null)
                {
                    var studentAccount = await _accountManager.GetUserByEmailAsync(student.CurrentEmail);
                    if(studentAccount == null)
                        throw new Exception("Student not found.");
                    else
                    {
                        await _accountManager.UpdateEmailAsync(studentAccount, student.NewEmail);
                    }
                }
                else
                {
                    if (id != dto.Id)
                    {
                        throw new Exception("Update not allowed.");
                    }

                    var studentAccount = await _accountManager.GetUserByEmailAsync(student.NewEmail);
                    if (studentAccount != null)
                        throw new Exception("Email is already used.");

                    await this._studentService.UpdateStudentEmail(dto.Id, student.NewEmail);

                    if (dto.UserId.HasValue)
                    {
                        ApplicationUser appUser = await _accountManager.GetUserByIdAsync(dto.UserId.Value);

                        await _accountManager.UpdateEmailAsync(appUser, student.NewEmail);
                    }
                }

                return Ok(new { Error = "", ErrorDescription = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ChangeStudentEmail : {ex.Message}", ex);
                _logger.LogError($"Error ChangeStudentEmail : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("resetpassword/{email}")]
        [AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResetPassword(string email)
        {
            _logger.LogInformation($"ResetPassword Email : {email}");
            try
            {
                ApplicationUser appUser = await _accountManager.GetUserByEmailAsync(email);

                if (appUser != null)
                {

                    var newPassword = PasswordHelper.GenerateRandomPassword();

                    if (appUser.Account != null && appUser.Account.Student != null && !string.IsNullOrWhiteSpace(appUser.Account.Student.newPassword)) newPassword = appUser.Account.Student.newPassword;

                    var result = await _accountManager.ResetPasswordAsync(appUser, newPassword);

                    if (result.Item1)
                    {
                        var title = $"Tappee Login Information.";
                        var content = $"Hi {appUser.Email}.\n" +
                            "Please find below your login information for Tappee.\n" +
                            $"Username: {appUser.Email}\n" +
                            $"Password: {newPassword}\n\n" +
                            ""
                            ;

                        var emailBody = EmailTemplates.GetPlainTextTestEmail(DateTime.Now);

                        var isSuccess = await _emailSender.SendEmailAsync("Tappee", "smv.notification@gmail.com", appUser.FullName, appUser.Email, title, content);

                        //int uId = 0;
                        //Int32.TryParse(appUser.Id, out uId);
                        await this.UnblockUser(appUser.Id);


                        return Ok(new { Error = "", ErrorDescription = "" });
                    }
                    else
                    {
                        return BadRequest(new { Error = "Failed", ErrorDescription = "Reset Password Failed, Please Try Again." });
                    }
                }
                else
                {
                    return BadRequest(new { Error = "Email not found.", ErrorDescription = "User with that email does not exist." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ChangePassword : {ex.Message}", ex);
                _logger.LogError($"Error ChangePassword : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }


        [HttpPatch("users/me")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] JsonPatchDocument<UserPatchViewModel> patch)
        {
            return await UpdateUser(Utilities.GetUserId(this.User), patch);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPatch("users/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] JsonPatchDocument<UserPatchViewModel> patch)
        {
            string dataJSON = JsonConvert.SerializeObject(patch);
            _logger.LogInformation($"UpdateUser patch : {dataJSON}");
            _logger.LogInformation($"UpdateUser Id : {id}");

            try
            {
                if (!(await _authorizationService.AuthorizeAsync(this.User, id, AccountManagementOperations.Update)).Succeeded)
                    return new ChallengeResult();


                if (ModelState.IsValid)
                {
                    if (patch == null)
                        return BadRequest($"{nameof(patch)} cannot be null");


                    ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

                    if (appUser == null)
                        return NotFound(id);


                    UserPatchViewModel userPVM = _mapper.Map<UserPatchViewModel>(appUser);
                    patch.ApplyTo(userPVM, ModelState);


                    if (ModelState.IsValid)
                    {
                        _mapper.Map<UserPatchViewModel, ApplicationUser>(userPVM, appUser);

                        var result = await _accountManager.UpdateUserAsync(appUser);
                        if (result.Item1)
                        {
                            //broadcast newly updated user
                            UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);
                            await _userHub.Clients.All.SendAsync("BroadcastUpdatedUser", userVM);
                            return NoContent();
                        }


                        AddErrors(result.Item2);
                    }
                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error UpdateUser : {ex.Message}", ex);
                _logger.LogError($"Error UpdateUser : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("users")]
        [Authorize(Authorization.Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(201, Type = typeof(UserViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Register([FromBody] UserEditViewModel user)
        {
            if (!(await _authorizationService.AuthorizeAsync(this.User, Tuple.Create(user.Roles, new string[] { }), Authorization.Policies.AssignAllowedRolesPolicy)).Succeeded)
                return new ChallengeResult();

            string userRequest = JsonConvert.SerializeObject(user);
            _logger.LogInformation($"UserEditViewModel requested by : {this.User.Identity.Name}");
            _logger.LogInformation($"UserEditViewModel object : {userRequest}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (user == null)
                        return BadRequest($"{nameof(user)} cannot be null");

                    if (string.IsNullOrEmpty(user.UserName)) user.UserName = user.Email;
                    ApplicationUser appUser = _mapper.Map<ApplicationUser>(user);

                    user.IsChangePassword = true;
                    var result = await _accountManager.CreateUserAsync(appUser, user.Roles, user.NewPassword);
                    if (result.Item1)
                    {
                        appUser = await _userManager.FindByEmailAsync(user.Email);
                        UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);

                        //broadcast newly added user
                        await _userHub.Clients.All.SendAsync("BroadcastAddedUser", userVM);

                        if (!appUser.EmailConfirmed)
                        {
                            //send email confirmation
                            string enableOnboardingEmail = _configuration["AppSettings:ONBOARDING_EMAIL_ENABLED"];
                            if (enableOnboardingEmail == "Y")
                            {
                                var code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                                string baseUrl = _configuration["AppSettings:ONBOARDING_BASE_URL"];
                                var callbackUrl = Url.Action("ConfirmEmail", "Authorization", new { userId = appUser.Id, code = code }, protocol: HttpContext.Request.Scheme, host: baseUrl);
                                var portalUrl = _configuration["AppSettings:ORDER_PORTAL_URL"];
                                var imgUrl = _configuration["AppSettings:ORDER_PORTAL_ONBOARDING_IMG_URL"];
                                await _emailSender.SendEmailAsync(user.FullName, user.Email, "Confirm your account",
                                    EmailTemplates.GetConfirmationEmail(portalUrl, user.Email, callbackUrl, imgUrl));
                            }
                        }

                        _logger.LogInformation($"User Successfully created");
                        return CreatedAtAction(GetUserByIdActionName, new { id = userVM.Id }, userVM);
                    }

                    AddErrors(result.Item2);
                }
                _logger.LogInformation($"Error Model State");
                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error Register : {ex.Message}", ex);
                _logger.LogError($"Error Register : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }


        [HttpDelete("users/{id}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation($"DeleteUser Id : {id}");
            //if (!(await _authorizationService.AuthorizeAsync(this.User, id, AccountManagementOperations.Delete)).Succeeded)
            //    return new ChallengeResult();

            if (!await _accountManager.TestCanDeleteUserAsync(id))
                return BadRequest("User has active events and cannot be deleted.");


            UserViewModel userVM = null;
            ApplicationUser appUser = await this._accountManager.GetUserByIdAsync(id);

            if (appUser != null)
                userVM = await GetUserViewModelHelper(appUser.Id);


            if (userVM == null)
                return NotFound(id);

            var result = await this._accountManager.DeleteUserAsync(appUser);
            if (!result.Item1)
            {
                _logger.LogDebug($"Error DeleteUser : {string.Join(", ", result.Item2)}");
                throw new Exception("The following errors occurred while deleting user: " + string.Join(", ", result.Item2));
            }
                


            //broadcast newly deleted user
            await _userHub.Clients.All.SendAsync("BroadcastDeletedUser", userVM);
            return Ok(userVM);
        }


        [HttpPut("users/unblock/{id}")]
        [Authorize(Authorization.Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UnblockUser(int id)
        {
            ApplicationUser appUser = await this._accountManager.GetUserByIdAsync(id);

            if (appUser == null)
                return NotFound(id);

            appUser.LockoutEnd = null;
            var result = await _accountManager.UpdateUserAsync(appUser);
            if (!result.Item1)
                throw new Exception("The following errors occurred while unblocking user: " + string.Join(", ", result.Item2));


            return NoContent();
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("users/me/preferences")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UserPreferences()
        {
            var userId = Utilities.GetUserId(this.User);
            ApplicationUser appUser = await this._accountManager.GetUserByIdAsync(userId);

            if (appUser != null)
                return Ok(appUser.Configuration);
            else
                return NotFound(userId);
        }


        [HttpPut("users/me/preferences")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UserPreferences([FromBody] string data)
        {
            var userId = Utilities.GetUserId(this.User);
            ApplicationUser appUser = await this._accountManager.GetUserByIdAsync(userId);

            if (appUser == null)
                return NotFound(userId);

            appUser.Configuration = data;
            var result = await _accountManager.UpdateUserAsync(appUser);
            if (!result.Item1)
                throw new Exception("The following errors occurred while updating User Configurations: " + string.Join(", ", result.Item2));


            return NoContent();
        }





        [HttpGet("roles/{id}", Name = GetRoleByIdActionName)]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var appRole = await _accountManager.GetRoleByIdAsync(id);

            if (!(await _authorizationService.AuthorizeAsync(this.User, appRole?.Name ?? "", Authorization.Policies.ViewRoleByRoleNamePolicy)).Succeeded)
                return new ChallengeResult();

            if (appRole == null)
                return NotFound(id);

            return await GetRoleByName(appRole.Name);
        }


        [HttpGet("roles/name/{name}")]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            if (!(await _authorizationService.AuthorizeAsync(this.User, name, Authorization.Policies.ViewRoleByRoleNamePolicy)).Succeeded)
                return new ChallengeResult();


            RoleViewModel roleVM = await GetRoleViewModelHelper(name);

            if (roleVM == null)
                return NotFound(name);

            return Ok(roleVM);
        }

        [HttpPost("roles/permissions")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRolePermissionsByName([FromBody] List<string> roleNames)
        {
            if(roleNames == null)
                return NotFound();

            var permissions = await _accountManager.GetRolePermissionsByRoleName(roleNames);

            return Ok(permissions);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("roles")]
        //[Authorize(Authorization.Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public async Task<IActionResult> GetRoles(int? institutionId = null)
        {
            return await GetRoles(-1, -1, institutionId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("roles/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public async Task<IActionResult> GetRoles(int pageNumber, int pageSize, int? institutionId = null)
        {
            var roles = await _accountManager.GetRolesLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(_mapper.Map<List<RoleViewModel>>(roles));
        }


        [HttpPut("roles/{id}")]
        //[Authorize(Authorization.Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                if (role == null)
                    return BadRequest($"{nameof(role)} cannot be null");

                if (id != role.Id)
                    return BadRequest("Conflicting role id in parameter and model data");



                ApplicationRole appRole = await _accountManager.GetRoleByIdAsync(id);

                if (appRole == null)
                    return NotFound(id);


                _mapper.Map<RoleViewModel, ApplicationRole>(role, appRole);

                var result = await _accountManager.UpdateRoleAsync(appRole, role.Permissions?.Select(p => p.Value).ToArray());
                if (result.Item1)
                    return NoContent();

                AddErrors(result.Item2);

            }

            return BadRequest(ModelState);
        }


        [HttpPost("roles")]
        //[Authorize(Authorization.Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(201, Type = typeof(RoleViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRole([FromBody] RoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                if (role == null)
                    return BadRequest($"{nameof(role)} cannot be null");


                ApplicationRole appRole = _mapper.Map<ApplicationRole>(role);

                var result = await _accountManager.CreateRoleAsync(appRole, role.Permissions?.Select(p => p.Value).ToArray());
                if (result.Item1)
                {
                    RoleViewModel roleVM = await GetRoleViewModelHelper(appRole.Name);
                    return CreatedAtAction(GetRoleByIdActionName, new { id = roleVM.Id }, roleVM);
                }

                AddErrors(result.Item2);
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("roles/{id}")]
        //[Authorize(Authorization.Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRole(int id)
        {
            _logger.LogInformation($"DeleteRole Id : {id}");
            if (!await _accountManager.TestCanDeleteRoleAsync(id))
                return BadRequest("Role cannot be deleted. Remove all users from this role and try again");


            RoleViewModel roleVM = null;
            ApplicationRole appRole = await this._accountManager.GetRoleByIdAsync(id);

            if (appRole != null)
                roleVM = await GetRoleViewModelHelper(appRole.Name);


            if (roleVM == null)
                return NotFound(id);

            var result = await this._accountManager.DeleteRoleAsync(appRole);
            if (!result.Item1)
            {
                _logger.LogDebug($"Error DeleteRole : {string.Join(", ", result.Item2)}");
                throw new Exception("The following errors occurred while deleting role: " + string.Join(", ", result.Item2));
            }
                


            return Ok(roleVM);
        }


        //[HttpGet("permissions")]
        //[Authorize(Authorization.Policies.ViewAllRolesPolicy)]
        //[ProducesResponseType(200, Type = typeof(List<PermissionViewModel>))]
        //public IActionResult GetAllPermissions()
        //{
        //    return Ok(_mapper.Map<List<PermissionViewModel>>(ApplicationPermissions.AllPermissions));
        //}

        [HttpGet("permissions")]
        [Authorize(Authorization.Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<ApplicationPermissionsTree>))]
        public IActionResult GetAllPermissions()
        {
            var aclPath = _configuration["AppSettings:ACL_ROLE_REPORT_PATH"];
            var tree = ApplicationPermissionsTrees.TreeFromJson(aclPath);
            return Ok(ApplicationPermissionsTrees.ConvertTreeToList(tree));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("permissionstree")]
        //[Authorize(Authorization.Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PermissionTreeViewModel>))]
        public IActionResult GetAllPermissionsTree()
        {
            //return Ok(_mapper.Map<SimpleApiTreeResult>(ApplicationPermissionsTrees.AllPermissionsTree));
            //return Ok(_mapper.Map<SimpleApiTreeResult>(ApplicationPermissionsTrees.Tree()));
            var aclPath = _configuration["AppSettings:ACL_PATH"];
            return Ok(ApplicationPermissionsTrees.TreeFromJson(aclPath));
        }


        #region API
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("users/update/{id}")]
        //[AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ApiUpdateUser(int id, [FromBody] UserEditViewModel user)
        {
            string dataJSON = JsonConvert.SerializeObject(user);
            _logger.LogInformation($"ApiUpdateUser UserEditViewModel : {dataJSON}");
            _logger.LogInformation($"ApiUpdateUser Id : {id}");

            try
            {
                ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);
                string[] currentRoles = appUser != null ? (await _accountManager.GetUserRolesAsync(appUser)).ToArray() : null;

                if (ModelState.IsValid)
                {
                    if (user == null)
                        return BadRequest($"{nameof(user)} cannot be null");

                    if (id != user.Id)
                        return BadRequest("Conflicting user id in parameter and model data");

                    if (appUser == null)
                        return NotFound(id);


                    if (Utilities.GetUserId(this.User) == id && string.IsNullOrWhiteSpace(user.CurrentPassword))
                    {
                        if (!string.IsNullOrWhiteSpace(user.NewPassword))
                            return BadRequest("Current password is required when changing your own password");

                        if (appUser.UserName != user.UserName)
                            return BadRequest("Current password is required when changing your own username");
                    }


                    bool isValid = true;

                    if (Utilities.GetUserId(this.User) == id && (appUser.UserName != user.UserName || !string.IsNullOrWhiteSpace(user.NewPassword)))
                    {
                        if (!await _accountManager.CheckPasswordAsync(appUser, user.CurrentPassword))
                        {
                            isValid = false;
                            AddErrors(new string[] { "The username/password is invalid." });
                        }
                    }

                    if (isValid)
                    {
                        _mapper.Map<UserViewModel, ApplicationUser>(user, appUser);

                        var result = await _accountManager.UpdateUserAsync(appUser, user.Roles);

                        if (result.Item1)
                        {
                            if (!string.IsNullOrWhiteSpace(user.NewPassword))
                            {
                                if (!string.IsNullOrWhiteSpace(user.CurrentPassword))
                                    result = await _accountManager.UpdatePasswordAsync(appUser, user.CurrentPassword, user.NewPassword);
                                else
                                    result = await _accountManager.ResetPasswordAsync(appUser, user.NewPassword);
                            }

                            if (result.Item1)
                            {
                                var response = new BaseOperationResponse();
                                response.IsSuccess = result.Item1;
                                response.Message = "Successfully updated the information!";
                                return Ok(response);
                            }
                        }

                        AddErrors(result.Item2);
                    }
                }

                //var errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                //              .Select(m => m.ErrorMessage).ToArray();

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error DeleteUser : {ex.Message}", ex);
                _logger.LogError($"Error DeleteUser : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
            
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("users/register")]
        //[AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(UserViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ApiRegister([FromBody] UserEditViewModel user)
        {
            string dataJSON = JsonConvert.SerializeObject(user);
            _logger.LogInformation($"ApiRegister UserEditViewModel : {dataJSON}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (user == null)
                        return BadRequest($"{nameof(user)} cannot be null");

                    var institution = await _unitOfWork.Institutions.GetByCodeAsync(user.InstitutionCode);
                    if (institution == null)
                        return BadRequest($"Institution is incorrect.");

                    var department = await _unitOfWork.Departments.GetByCode(institution.Id, user.RegisteredDepartment);
                    if (department != null)
                    {
                        user.DepartmentId = department.Id;
                    }

                    if (string.IsNullOrEmpty(user.UserName)) user.UserName = user.Email;
                    ApplicationUser appUser = _mapper.Map<ApplicationUser>(user);
                    appUser.InstitutionId = institution.Id;
                    if (!string.IsNullOrEmpty(user.Provider) && !string.IsNullOrEmpty(user.Key))
                    {
                        appUser.EmailConfirmed = true;
                        var externalLoginResult = await _signInManager.ExternalLoginSignInAsync(user.Provider, user.Key, isPersistent: false);
                        if (!externalLoginResult.Succeeded)
                        {
                            var createResult = await _userManager.CreateAsync(appUser);
                            if (createResult.Succeeded)
                            {
                                await _userManager.AddLoginAsync(appUser, new UserLoginInfo(user.Provider, user.Key, user.Provider));

                                //await _signInManager.SignInAsync(newUser, isPersistent: false);
                                //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                                ApplicationUser user1 = await _userManager.FindByLoginAsync(user.Provider, user.Key);

                                UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);
                                //broadcast newly added user
                                await _userHub.Clients.All.SendAsync("BroadcastAddedUser", userVM);

                                return CreatedAtAction(GetUserByIdActionName, new { id = userVM.Id }, userVM);
                            }
                        }
                        else
                        {
                            AddErrors(new string[] { "Cannot login using your external account. Provider-key combination is incorrect." });
                        }
                    }
                    else
                    {
                        //TODO:
                        if (string.IsNullOrEmpty(user.NewPassword))
                        {
                            //generate dummy password
                            user.NewPassword = user.Pin;
                        }

                        user.IsChangePassword = true;
                        var result = await _accountManager.CreateUserAsync(appUser, user.Roles, user.NewPassword);
                        if (result.Item1)
                        {
                            UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);
                            //broadcast newly added user
                            await _userHub.Clients.All.SendAsync("BroadcastAddedUser", userVM);
                            string enableOnboardingEmail = _configuration["AppSettings:ONBOARDING_EMAIL_ENABLED"];
                            if (enableOnboardingEmail == "Y")
                            {
                                //send email confirmation
                                var code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                                string passwordSetCode = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                                string baseUrl = _configuration["AppSettings:ONBOARDING_BASE_URL"];
                                var callbackUrl = Url.Action("ConfirmEmail", "Authorization", new { userId = appUser.Id, code = code, passwordSetCode = passwordSetCode }, protocol: HttpContext.Request.Scheme, host: baseUrl);
                                var portalUrl = _configuration["AppSettings:ORDER_PORTAL_URL"];
                                var imgUrl = _configuration["AppSettings:ORDER_PORTAL_ONBOARDING_IMG_URL"];
                                await _emailSender.SendEmailAsync(user.FullName, user.Email, "Confirm your account",
                                    EmailTemplates.GetConfirmationEmail(portalUrl, user.Email, callbackUrl, imgUrl));
                            }

                            return CreatedAtAction(GetUserByIdActionName, new { id = userVM.Id }, userVM);
                        }

                        AddErrors(result.Item2);
                    }
                }

                //var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error ApiRegister : {ex.Message}", ex);
                _logger.LogError($"Error ApiRegister : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiForgotPassword(string email)
        {
            _logger.LogInformation($"ApiForgotPassword email : {email}");
            var result = new BaseOperationResponse();
            try
            {
                var user = await _userManager.FindByNameAsync(email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //var callbackUrl = Url.Action("resetpassword", "", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);//HttpUtility.UrlEncode(code)
                    var callbackUrl = string.Format("{0}/resetpassword?userId={1}&code={2}", _configuration["AppSettings:baseUrl"], user.Id, HttpUtility.UrlEncode(code));
                    await _emailSender.SendEmailAsync(user.FullName, user.Email, "Reset your password",
                        EmailTemplates.GetForgotPasswordEmail(user.Email, callbackUrl));
                }

                result.Message = "Please check the link sent to your email to reset your password";
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Create a User : {ex.Message}", ex);
                _logger.LogError($"Error Create a User : {ex.StackTrace}", ex);
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
            }

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("resetpassword")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ApiResetPassword([FromBody] ResetPasswordViewModel model)
        {
            string dataJson = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"ApiResetPassword ResetPasswordViewModel : {dataJson}");
            var result = new BaseOperationResponse();
            try
            {

                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user != null)
                {
                    var resetResult = await _userManager.ResetPasswordAsync(user, HttpUtility.UrlDecode(model.Code), model.NewPassword);
                    if (resetResult.Succeeded)
                    {
                        int uId = 0;
                        Int32.TryParse(model.UserId, out uId);
                        await this.UnblockUser(uId);


                        result.Message = "You have successfully reset your password";

                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.Message = "Please make sure you have a correct link to reset your password";
                    }
                }
                else
                {
                    result.Message = "Please make sure you have a correct link to reset your password";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                _logger.LogError($"Error ApiResetPassword : {ex.Message}", ex);
                _logger.LogError($"Error ApiResetPassword : {ex.StackTrace}", ex);
            }

            return Ok(result);
        }

        //tappee forgot or reset password
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("users/forgotpassword")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> UserForgotPassword([FromBody] string email)
        {
            var result = new BaseOperationResponse();
            _logger.LogInformation($"UserForgotPassword email : {email}");
            try
            {
                var user = await _userManager.FindByNameAsync(email) ?? await _userManager.FindByEmailAsync(email);
                if (user != null && user.IsActive)
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    string baseUrl = _configuration["AppSettings:ONBOARDING_BASE_URL"];
                    //var callbackUrl = Url.Action("resetpassword", "", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);//HttpUtility.UrlEncode(code)
                    var callbackUrl = Url.Action("ResetPassword", "Authorization", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme, host: baseUrl);
                    var imgUrl = _configuration["AppSettings:ORDER_PORTAL_ONBOARDING_IMG_URL"];
                    //var callbackUrl = string.Format("{0}/resetpassword?userId={1}&code={2}", _configuration["AppSettings:baseUrl"], user.Id, HttpUtility.UrlEncode(code));
                    await _emailSender.SendEmailAsync(user.FullName, user.Email, "Reset your password",
                        EmailTemplates.GetForgotPasswordEmail(user.Email, callbackUrl, imgUrl));
                }

                result.Message = "Please check the link sent to your email to reset your password";
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                _logger.LogError($"Error ChangePassword : {ex.Message}", ex);
                _logger.LogError($"Error ChangePassword : {ex.StackTrace}", ex);
            }

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("users/{id}/student-link-invite")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> UserStudentLinkInvite(int id, string email)
        {
            var result = new BaseOperationResponse();
            _logger.LogInformation($"UserStudentLinkInvite Id : {id}");
            _logger.LogInformation($"UserStudentLinkInvite Email : {email}");
            try
            {
                var dto = await this._studentService.GetStudentByIdAsync(id);
                if (dto == null)
                {
                    throw new Exception("Student Id not found.");
                }

                string baseUrl = _configuration["AppSettings:ONBOARDING_BASE_URL"];
                var imgUrl = _configuration["AppSettings:ORDER_PORTAL_ONBOARDING_IMG_URL"];
                bool emailSent = false;

                var user = await _userManager.FindByNameAsync(email) ?? await _userManager.FindByEmailAsync(email);
                if (user != null && user.IsActive)
                {
                    // check if email is a student
                    var student = _studentService.GetStudentsByUserAsync(user.Id);
                    if(student != null)
                    {
                        throw new Exception("Cannot link one student account to another.");
                    }

                    var callbackUrl = Url.Action("StudentLinkAccount", "Authorization", new { studentId = id, userId = user.Id }, protocol: HttpContext.Request.Scheme, host: baseUrl);
                    var resp = await _emailSender.SendEmailAsync(user.FullName, user.Email, "Tappee Invite to link Student Account",
                        EmailTemplates.GetLinkStudentAccountForExistingUserEmail(user.Email, dto.Name, callbackUrl, imgUrl));
                    emailSent = resp.success;
                }
                else
                {
                    // a new user
                    var callbackUrl = Url.Action("StudentLinkAccount", "Authorization", new { studentId = id, email = email }, protocol: HttpContext.Request.Scheme, host: baseUrl);
                    var resp = await _emailSender.SendEmailAsync(email, email, "Tappee Invite to link Student Account",
                        EmailTemplates.GetLinkStudentAccountForNewUserEmail(email, dto.Name, callbackUrl, imgUrl));
                    emailSent = resp.success;
                }

                await _studentService.AddStudentAccountLinkRequestAsync(id, email, emailSent);

                result.Message = "Email invite has been sent!";
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "An error occurred: " + ex.Message;
                _logger.LogError($"Error ChangePassword : {ex.Message}", ex);
                _logger.LogError($"Error ChangePassword : {ex.StackTrace}", ex);
            }

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("users/get/{id}")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserEditViewModel))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> ApiGetUserById(int id)
        {
            //UserViewModel userVM = await GetUserViewModelHelper(id);
            var userAndRoles = await _accountManager.GetUserAndRolesAsync(id);
            if (userAndRoles == null)
                return null;

            var userVM = _mapper.Map<UserEditViewModel>(userAndRoles.Item1);
            userVM.Roles = userAndRoles.Item2;

            if (userVM != null)
                return Ok(userVM);
            else
                return NotFound(id);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("ldap/sync")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllContactGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> SyncLdap()
        {
            var response = await _accountManager.SyncLdap(false);
            return Ok(response);
        }
        #endregion

        #region Phonebooks

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("phonebooks/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetPhonebooks(int? userId = null)
        {
            return await GetPhonebooks(-1, -1, userId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("phonebooks/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<UserPhonebookViewModel>))]
        public async Task<IActionResult> GetPhonebooks(int pageNumber, int pageSize, int? userId = null)
        {
            var phoneBooks = await _accountManager.GetUserPhonebooksLoadRelatedAsync(pageNumber, pageSize, userId);

            var userPhonebookVM = _mapper.Map<List<UserPhonebookViewModel>>(phoneBooks);

            return Ok(userPhonebookVM);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("phonebooks")]
        //[Authorize(Authorization.Policies.ManageAllUserPhonebooksPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(UserPhonebookViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUserPhonebook([FromBody] UserPhonebookViewModel userPhonebook)
        {
            string dataJSON = JsonConvert.SerializeObject(userPhonebook);
            _logger.LogInformation($"CreateUserPhonebook UserPhonebookViewModel : {dataJSON}");
            try
            {
                if (ModelState.IsValid)
                {
                    if (userPhonebook == null)
                        return BadRequest($"{nameof(userPhonebook)} cannot be null");


                    var pb = _mapper.Map<UserPhonebook>(userPhonebook);

                    var result = await _accountManager.CreateUserPhonebookAsync(pb, userPhonebook.FilePath);
                    if (result.IsSuccess)
                    {
                        UserPhonebookViewModel userPhonebookVM = _mapper.Map<UserPhonebookViewModel>(result.Data);
                        return CreatedAtAction("GetUserPhonebookById", new { id = userPhonebookVM.Id }, userPhonebookVM);
                    }

                    AddErrors(new string[] { result.Message });
                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error CreateUserPhonebook : {ex.Message}", ex);
                _logger.LogError($"Error CreateUserPhonebook : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("phonebooks/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllUserPhonebooksPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserPhonebookViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUserPhonebook(int id)
        {
            _logger.LogInformation($"DeleteUserPhonebook id : {id}");
            var testDeleteResult = await _accountManager.TestCanDeleteUserPhonebookAsync(id);
            if (!testDeleteResult.IsDeletable)
                return BadRequest(string.Format("Contact cannot be deleted. {0}", testDeleteResult.Message));


            var userPhonebook = await _accountManager.GetUserPhonebookByIdAsync(id);

            UserPhonebookViewModel userPhonebookVM = _mapper.Map<UserPhonebookViewModel>(userPhonebook);
            if (userPhonebookVM == null)
                return NotFound(id);

            var result = await _accountManager.DeleteUserPhonebookAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogError($"Error DeleteUserPhonebook : {string.Join(", ", result.Message)}");
                throw new Exception("The following errors occurred while deleting userPhonebook: " + string.Join(", ", result.Message));
            }


            return Ok(userPhonebookVM);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("phonebooks/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllUserPhonebooksPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserPhonebook(string id, [FromBody] UserPhonebookViewModel model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"UpdateUserPhonebook UserEditViewModel : {dataJSON}");
            _logger.LogInformation($"UpdateUserPhonebook Id : {id}");
            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.Id == 0)
                        return BadRequest("Conflicting type id in parameter and model data");



                    var userPhonebook = await _accountManager.GetUserPhonebookByIdAsync(model.Id);

                    UserPhonebookViewModel userPhonebookVM = _mapper.Map<UserPhonebookViewModel>(userPhonebook);
                    if (userPhonebookVM == null)
                        return NotFound(id);

                    var updatedModel = _mapper.Map<UserPhonebook>(model);
                    var result = await _accountManager.UpdateUserPhonebookAsync(updatedModel, model.FilePath);
                    if (result.IsSuccess)
                        return NoContent();

                    AddErrors(new string[] { result.Message });

                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error UpdateUserPhonebook : {ex.Message}", ex);
                _logger.LogError($"Error UpdateUserPhonebook : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        #endregion

        #region Vehicles

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("vehicles/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetVehicles(int? userId = null, string status = "")
        {
            return await GetVehicles(-1, -1, userId, status);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("vehicles/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<UserVehicleViewModel>))]
        public async Task<IActionResult> GetVehicles(int pageNumber, int pageSize, int? userId = null, string status = "")
        {
            var vehicles = await _accountManager.GetUserVehiclesLoadRelatedAsync(pageNumber, pageSize, userId, status);

            var userVehicleVM = _mapper.Map<List<UserVehicleViewModel>>(vehicles);

            return Ok(userVehicleVM);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("vehicles/list/group/user")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserVehicleGroupViewModel>))]
        public async Task<IActionResult> GetVehiclesGroupByUser(int? userId = null, string status = "")
        {
            var vehicles = await _accountManager.GetUserVehiclesLoadRelatedAsync(-1, -1, userId, status);

            var userVehicles = vehicles.GroupBy(e => e.UserId).Select(e => new UserVehicleGroupViewModel
            {
                UserId = e.Key,
                UserName = e.First().User != null ? e.First().User.FriendlyName : string.Empty,
                Vechicles = _mapper.Map<List<UserVehicleViewModel>>(e)

            });

            return Ok(userVehicles);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("vehicles")]
        //[Authorize(Authorization.Policies.ManageAllUserVehiclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(UserVehicleViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUserVehicle([FromBody] UserVehicleViewModel userVehicle)
        {
            string dataJSON = JsonConvert.SerializeObject(userVehicle);
            _logger.LogInformation($"CreateUserVehicle userVehicle : {dataJSON}");
            try
            {
                if (ModelState.IsValid)
                {
                    if (userVehicle == null)
                        return BadRequest($"{nameof(userVehicle)} cannot be null");


                    var pb = _mapper.Map<UserVehicle>(userVehicle);
                    if (string.IsNullOrEmpty(pb.VehicleStatus))
                    {
                        pb.VehicleStatus = VehicleStatus.PENDING.ToString();
                    }

                    var result = await _accountManager.CreateUserVehicleAsync(pb);
                    if (result.IsSuccess)
                    {
                        UserVehicleViewModel userVehicleVM = _mapper.Map<UserVehicleViewModel>(result.Data);
                        return CreatedAtAction("GetUserVehicleById", new { id = userVehicleVM.Id }, userVehicleVM);
                    }

                    AddErrors(new string[] { result.Message });
                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error DeleteUser : {ex.Message}", ex);
                _logger.LogError($"Error DeleteUser : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("vehicles/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllUserVehiclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserVehicleViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUserVehicle(int id)
        {
            var testDeleteResult = await _accountManager.TestCanDeleteUserVehicleAsync(id);
            if (!testDeleteResult.IsDeletable)
                return BadRequest(string.Format("Contact cannot be deleted. {0}", testDeleteResult.Message));


            var userVehicle = await _accountManager.GetUserVehicleByIdAsync(id);

            UserVehicleViewModel userVehicleVM = _mapper.Map<UserVehicleViewModel>(userVehicle);
            if (userVehicleVM == null)
                return NotFound(id);

            var result = await _accountManager.DeleteUserVehicleAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"Error DeleteUserVehicle : {string.Join(", ", result.Message)}");
                throw new Exception("The following errors occurred while deleting userVehicle: " + string.Join(", ", result.Message));
            }
            return Ok(userVehicleVM);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("vehicles/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllUserVehiclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserVehicle(string id, [FromBody] UserVehicleViewModel model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"UpdateUserVehicle UserVehicleViewModel : {dataJSON}");
            _logger.LogInformation($"UpdateUserVehicle Id : {id}");
            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.Id == 0)
                        return BadRequest("Conflicting type id in parameter and model data");



                    var userVehicle = await _accountManager.GetUserVehicleByIdAsync(model.Id);

                    UserVehicleViewModel userVehicleVM = _mapper.Map<UserVehicleViewModel>(userVehicle);
                    if (userVehicleVM == null)
                        return NotFound(id);

                    var updatedModel = _mapper.Map<UserVehicle>(model);
                    if (userVehicleVM.VehicleStatus != VehicleStatus.APPROVED.ToString() && model.IsApprove)
                    {
                        updatedModel.VehicleStatus = VehicleStatus.APPROVED.ToString();
                    }

                    //if anything is changed, status will be back to PENDING
                    if (userVehicle.PlateNumber != updatedModel.PlateNumber ||
                        userVehicle.UserId != updatedModel.UserId)
                    {
                        updatedModel.VehicleStatus = VehicleStatus.PENDING.ToString();
                    }

                    var result = await _accountManager.UpdateUserVehicleAsync(updatedModel);
                    if (result.IsSuccess)
                        return NoContent();

                    AddErrors(new string[] { result.Message });

                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error UpdateUserVehicle : {ex.Message}", ex);
                _logger.LogError($"Error UpdateUserVehicle : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        #endregion

        #region CardId

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("cardids/list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetCardIds(int? userId = null, string status = "")
        {
            return await GetCardIds(-1, -1, userId, status);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("cardids/list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<UserCardIdViewModel>))]
        public async Task<IActionResult> GetCardIds(int pageNumber, int pageSize, int? userId = null, string status = "")
        {
            var cardIds = await _accountManager.GetUserCardIdsLoadRelatedAsync(pageNumber, pageSize, userId, status);

            var userCardIdVM = _mapper.Map<List<UserCardIdViewModel>>(cardIds);

            return Ok(userCardIdVM);
        }

        //[HttpGet("vehicles/list/group/user")]
        //[AllowAnonymous]
        ////[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        //[ProducesResponseType(200, Type = typeof(List<UserVehicleGroupViewModel>))]
        //public async Task<IActionResult> GetVehiclesGroupByUser(int? userId = null, string status = "")
        //{
        //    var vehicles = await _accountManager.GetUserVehiclesLoadRelatedAsync(-1, -1, userId, status);

        //    var userVehicles = vehicles.GroupBy(e => e.UserId).Select(e => new UserVehicleGroupViewModel
        //    {
        //        UserId = e.Key,
        //        UserName = e.First().User != null ? e.First().User.FriendlyName : string.Empty,
        //        Vechicles = _mapper.Map<List<UserVehicleViewModel>>(e)

        //    });

        //    return Ok(userVehicles);
        //}

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("cardids/getUser")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserCardIdViewModel))]
        public async Task<IActionResult> GetUserByCardId(string cardId)
        {
            var cardIds = await _accountManager.GetUserByCardIdAsync(cardId);

            if (cardIds == null)
            {
                return NotFound("Card Id: " + cardId + " is not found or Inactive");
            }

            var userCardIdVM = _mapper.Map<UserViewModel>(cardIds.User);

            return Ok(userCardIdVM);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("cardids/getactive")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(UserCardIdViewModel))]
        public async Task<IActionResult> GetActiveCardIds(int userId)
        {

            var cardIds = await _accountManager.GetActiveUserCardId(userId);

            var userCardIdVM = _mapper.Map<UserCardIdViewModel>(cardIds);

            return Ok(userCardIdVM);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("cardids/activate")]
        //[Authorize(Authorization.Policies.ManageAllUserVehiclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(UserCardIdViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUserCardIdActivate([FromBody] UserCardIdViewModel userCardId)
        {
            string dataJSON = JsonConvert.SerializeObject(userCardId);
            _logger.LogInformation($"CreateUserCardIdActivate UserCardIdViewModel : {dataJSON}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (userCardId == null)
                        return BadRequest($"{nameof(userCardId)} cannot be null");


                    var pb = _mapper.Map<UserCardId>(userCardId);
                    pb.Status = CardIdStatus.ACTIVE.ToString();

                    var result = await _accountManager.CreateUserCardIdActivateAsync(pb);
                    if (result.IsSuccess)
                    {
                        UserCardIdViewModel userCardIdVM = _mapper.Map<UserCardIdViewModel>(result.Data);
                        return CreatedAtAction("GetUserCardIdById", new { id = userCardIdVM.Id }, userCardIdVM);
                    }

                    AddErrors(new string[] { result.Message });
                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error CreateUserCardIdActivate : {ex.Message}", ex);
                _logger.LogError($"Error CreateUserCardIdActivate : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("cardids")]
        //[Authorize(Authorization.Policies.ManageAllUserVehiclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(UserCardIdViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUserCardId([FromBody] UserCardIdViewModel userCardId)
        {
            string dataJSON = JsonConvert.SerializeObject(userCardId);
            _logger.LogInformation($"UserCardIdViewModel UserEditViewModel : {dataJSON}");
            try
            {
                if (ModelState.IsValid)
                {
                    if (userCardId == null)
                        return BadRequest($"{nameof(userCardId)} cannot be null");


                    var pb = _mapper.Map<UserCardId>(userCardId);
                    if (string.IsNullOrEmpty(pb.Status))
                    {
                        pb.Status = CardIdStatus.INACTIVE.ToString();
                    }

                    var result = await _accountManager.CreateUserCardIdAsync(pb);
                    if (result.IsSuccess)
                    {
                        UserCardIdViewModel userCardIdVM = _mapper.Map<UserCardIdViewModel>(result.Data);
                        return CreatedAtAction("GetUserCardIdById", new { id = userCardIdVM.Id }, userCardIdVM);
                    }

                    AddErrors(new string[] { result.Message });
                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error CreateUserCardId : {ex.Message}", ex);
                _logger.LogError($"Error CreateUserCardId : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
            
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("cardids/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllUserVehiclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserCardIdViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUserCardId(int id)
        {
            _logger.LogInformation($"DeleteUserCardId Id : {id}");
            var testDeleteResult = await _accountManager.TestCanDeleteUserCardIdAsync(id);
            if (!testDeleteResult.IsDeletable)
                return BadRequest(string.Format("Contact cannot be deleted. {0}", testDeleteResult.Message));


            var userCardId = await _accountManager.GetUserCardIdByIdAsync(id);

            UserCardIdViewModel userCardIdVM = _mapper.Map<UserCardIdViewModel>(userCardId);
            if (userCardIdVM == null)
                return NotFound(id);

            var result = await _accountManager.DeleteUserCardIdAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogDebug($"Error DeleteUserCardId : {string.Join(", ", result.Message)}");
                throw new Exception("The following errors occurred while deleting user Card Id: " + string.Join(", ", result.Message));
            }
                


            return Ok(userCardIdVM);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("cardids/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllUserVehiclesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserCardId(string id, [FromBody] UserCardIdViewModel model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"UpdateUserCardId UserCardIdViewModel : {dataJSON}");
            _logger.LogInformation($"UpdateUserCardId Id : {id}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.Id == 0)
                        return BadRequest("Conflicting type id in parameter and model data");



                    var userCardId = await _accountManager.GetUserCardIdByIdAsync(model.Id);

                    UserCardIdViewModel userCardIdVM = _mapper.Map<UserCardIdViewModel>(userCardId);
                    if (userCardIdVM == null)
                        return NotFound(id);

                    var updatedModel = _mapper.Map<UserCardId>(model);
                    //if (userCardIdVM.Status != CardIdStatus.INACTIVE.ToString())
                    //{
                    //    updatedModel.Status = CardIdStatus.INACTIVE.ToString();
                    //}

                    var result = await _accountManager.UpdateUserCardIdAsync(updatedModel);
                    if (result.IsSuccess)
                        return NoContent();

                    AddErrors(new string[] { result.Message });

                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error UpdateUserCardId : {ex.Message}", ex);
                _logger.LogError($"Error UpdateUserCardId : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        #endregion

        #region Wallet

        #region Sieved

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("wallet/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllAssetTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetWallets(BaseFilter filter)
        {
            var results = await this._walletService.GetWalletsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<WalletDTO>>(results));
        }

        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("user/{email}")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ApiKeyAuthorize]
        //[AllowAnonymous]
        public async Task<IActionResult> GetUser(string email)
        {
            ApplicationUser appUser = await _accountManager.GetUserByUserNameAsync(email);

            if (appUser == null)
            {
                appUser = await _accountManager.GetUserByEmailAsync(email);
            }

            if (appUser == null)
                return NotFound(null);
            var userVM = _mapper.Map<UserViewModel>(appUser);

            if (userVM != null)
                return Ok(userVM);
            else
                return NotFound(null);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("wallet/user/{userId}")]
        [ProducesResponseType(200, Type = typeof(List<WalletDTO>))]
        [ProducesResponseType(403)]
        //[AllowAnonymous]
        public async Task<IActionResult> GetWalletByUserId(int userId)
        {
            var result = await this._walletService.GetByUserIdAsync(userId);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("wallet/{id}")]
        [ProducesResponseType(200, Type = typeof(WalletDTO))]
        [ProducesResponseType(403)]
        //[AllowAnonymous]
        public async Task<IActionResult> GetWallet(int id)
        {
            var result = await this._walletService.GetByIdAsync(id);
            return Ok(_mapper.Map<WalletDTO>(result));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("wallet/transactions/{walletId}")]
        [ProducesResponseType(200, Type = typeof(List<WalletTransactionDTO>))]
        [ProducesResponseType(403)]
        //[AllowAnonymous]
        public async Task<IActionResult> GetWalletTransactions(int walletId)
        {
            var result = await this._walletService.GetWalletTransactionByIdAsync(walletId);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpPut("wallet/topup/{id}")]
        //[Authorize(Authorization.Policies.ManageAllAssetsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TopUpWallet(string id, [FromBody] WalletTopUpDTO model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"TopUpWallet WalletTopUpDTO : {dataJSON}");
            _logger.LogInformation($"TopUpWallet Id : {id}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.WalletId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");


                    var dto = await this._walletService.GetByIdAsync(model.WalletId);

                    if (dto == null)
                        return NotFound(id);

                    var result = await this._walletService.TopUpAsync(model);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error TopUpWallet : {ex.Message}", ex);
                _logger.LogError($"Error TopUpWallet : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpPut("wallet/operation/{walletId}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllAssetsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> WalletTransaction(string walletId, [FromBody] WalletOperationDTO model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"WalletTransaction WalletOperationDTO : {dataJSON}");
            _logger.LogInformation($"WalletTransaction walletId : {walletId}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.WalletId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");


                    var dto = await this._walletService.GetByIdAsync(model.WalletId);

                    if (dto == null)
                        return NotFound(walletId);

                    var result = await this._walletService.WalletOperationAsync(model);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error WalletTransaction : {ex.Message}", ex);
                _logger.LogError($"Error WalletTransaction : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        #endregion

        #region Rewards

        #region Sieved
        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("reward/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllAssetTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRewards(BaseFilter filter)
        {
            var results = await this._rewardService.GetRewardsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<RewardDTO>>(results));
        }

        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("reward/user/{userId}")]
        [ProducesResponseType(200, Type = typeof(List<RewardDTO>))]
        [ProducesResponseType(403)]
        //[AllowAnonymous]
        public async Task<IActionResult> GetRewardByUserId(int userId)
        {
            var result = await this._rewardService.GetByUserIdAsync(userId);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        //[AllowAnonymous]
        [HttpGet("reward/{id}")]
        [ProducesResponseType(200, Type = typeof(RewardDTO))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetReward(int id)
        {
            var result = await this._rewardService.GetByIdAsync(id);
            return Ok(_mapper.Map<RewardDTO>(result));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("reward/transactions/{rewardId}")]
        [ProducesResponseType(200, Type = typeof(List<RewardTransactionDTO>))]
        [ProducesResponseType(403)]
        //[AllowAnonymous]
        public async Task<IActionResult> GetRewardTransactions(int rewardId)
        {
            var result = await this._rewardService.GetRewardTransactionByIdAsync(rewardId);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpPut("reward/operation/{rewardId}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllAssetsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RewardTransaction(string rewardId, [FromBody] RewardOperationDTO model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"RewardTransaction RewardOperationDTO : {dataJSON}");
            _logger.LogInformation($"RewardTransaction Id : {rewardId}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.RewardId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");


                    var dto = await this._rewardService.GetByIdAsync(model.RewardId);

                    if (dto == null)
                        return NotFound(rewardId);

                    var result = await this._rewardService.RewardOperationAsync(model);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error RewardTransaction : {ex.Message}", ex);
                _logger.LogError($"Error RewardTransaction : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        #endregion

        #region User Connections
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("userconnections/list")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserConnectionStatusDTO))]
        public async Task<IActionResult> ApiGetUserConnections(int pageNumber = -1, int pageSize = -1)
        {
            var connections = await _accountManager.GetUserConnectionsAsync(pageNumber, pageSize);
            return Ok(connections);
        }

        #endregion
        #region private methods
        private async Task<UserViewModel> GetUserViewModelHelper(int userId)
        {
            var userAndRoles = await _accountManager.GetUserAndRolesAsync(userId);
            if (userAndRoles == null)
                return null;

            var userVM = _mapper.Map<UserViewModel>(userAndRoles.Item1);
            userVM.Roles = userAndRoles.Item2;

            return userVM;
        }


        private async Task<RoleViewModel> GetRoleViewModelHelper(string roleName)
        {
            var role = await _accountManager.GetRoleLoadRelatedAsync(roleName);
            if (role != null)
                return _mapper.Map<RoleViewModel>(role);


            return null;
        }

        #endregion
    }
}
