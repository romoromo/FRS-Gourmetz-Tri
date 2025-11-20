using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using DAL.Models;
using DAL.Core;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using FRS.ViewModels;
using System.Web;
using BAL.Services;
using BAL.Services.Interfaces;
using BAL.DTO;
using Microsoft.AspNetCore.Http;
using UAParser;
using Microsoft.Extensions.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using FRS.Helpers;
using Microsoft.AspNetCore.Authorization;
using FRS.Pages;
using BAL.Services.Interfaces.MealOrder;
using DAL.Models.MealOrder;
using DAL.Core.Helpers;
using DAL.Core.Interfaces;
using Microsoft.AspNetCore;
using OpenIddict.Server.AspNetCore;
using System.Security.Principal;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Twilio.TwiML.Voice;


// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860


namespace FRS.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IAuthenticationLogService _authLogService;
        private IHttpContextAccessor _httpAccessor;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IAuditLogService _auditLogService;
        private readonly IStudentService _studentService;
        private readonly IAccountManager _accountManager;

        public AuthorizationController(
            IOptions<IdentityOptions> identityOptions,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationUserManager userManager,
            IAuthenticationLogService authLogService,
            IHttpContextAccessor httpAccessor,
            IConfiguration configuration,
            IEmailSender emailSender,
            IAuditLogService auditLogService,
            IStudentService studentService,
            IAccountManager accountManager)
        {
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
            _authLogService = authLogService;
            _httpAccessor = httpAccessor;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _auditLogService = auditLogService;
            _studentService = studentService;
            _accountManager = accountManager;
        }

        [HttpPost("~/updatefirstlogin")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateFirstLogin(string id, bool confirmReadTermsConditions, bool consentDataCollection, bool receivePromotionalMaterials)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                user.NotFirstLogin = true;
                user.ConfirmReadTermsConditions = confirmReadTermsConditions;
                user.ConsentDataCollection = consentDataCollection;
                user.ReceivePromotionalMaterials = receivePromotionalMaterials;

                await _userManager.UpdateAsync(user);
                return Ok("Success");
            } catch (Exception ex)
            {
                return StatusCode(500, "Internal server error, " + ex.Message);
            }
         }

        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange(string institutionCode = null, bool isExternal = false, bool isExternalLogin = false, bool isAD = false, bool needConfirmationCode = false, string appId = null, bool mfa = false, bool mfaValidation = false)
        {
            var request = HttpContext.GetOpenIddictServerRequest();

            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(_httpAccessor.HttpContext.Request.Headers["User-Agent"]);
            string message = string.Format(" sign in with access using {0}", isExternal ? "Mobile App" : isExternalLogin ? "Third party login" : "Web");
            string ip = _httpAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (c.Device != null && !string.IsNullOrEmpty(c.Device.Family))
            {
                message += string.Format(" .Device: {0}, OS: {1}, Agent: {2}, IP: {3}", c.Device.ToString(), c.OS.ToString(), c.UserAgent.ToString(), ip);
            }
            else
            {
                message += string.Format(" with IP {0}", ip);
            }


            var log = new AuthenticationLogDTO
            {
                CreatedDate = DateTime.Now,
                InstitutionCode = institutionCode,
                UserName = request.Username
            };

            if (request.IsPasswordGrantType() || isExternalLogin)
            {
                _userManager.InstitutionCode = institutionCode;
                ApplicationUser user = null;

                user = await _userManager.FindByEmailAsync(request.Username) ?? await _userManager.FindByNameAsync(request.Username);

                if (user == null)
                {
                    //someone tries to login
                    log.Message = "Failed " + message + " Error: You have entered an invalid user ID or password. Please note that your password is case-sensitive." + " Account was not found." + request.Username;
                    await _authLogService.CreateAsync(log);
                    return BadRequest(new OpenIddictResponse
                    {
                        Error = OpenIddictConstants.Errors.InvalidGrant,
                        ErrorDescription = "You have entered an invalid user ID or password. Please note that your password is case-sensitive."
                    });
                }

                // Ensure the user is enabled.
                if (!user.IsEnabled)
                {
                    log.Message = "Failed " + message + " Error: Pending account activation from Admin.";
                    await _authLogService.CreateAsync(log);
                    return BadRequest(new OpenIddictResponse
                    {
                        Error = OpenIddictConstants.Errors.InvalidGrant,
                        ErrorDescription = "Pending account activation from Admin."
                    });
                }

                // Ensure the user is active.
                if (!user.IsActive)
                {
                    log.Message = "Failed " + message + " Error: Account was not found or was deactivated in the system.";
                    await _authLogService.CreateAsync(log);

                    return BadRequest(new OpenIddictResponse
                    {
                        Error = OpenIddictConstants.Errors.InvalidGrant,
                        ErrorDescription = "Account was not found or was deactivated in the system."
                    });
                }


                if (!isExternalLogin)
                {
                    if (user.IsAD)
                    {
                        if (!ValidateADAccount(request.Username, request.Password))
                        {
                            log.Message = "Failed " + message + " Error: You have entered an invalid user ID or password. Please note that your password is case-sensitive.";
                            await _authLogService.CreateAsync(log);
                            return BadRequest(new OpenIddictResponse
                            {
                                Error = OpenIddictConstants.Errors.InvalidGrant,
                                ErrorDescription = "You have entered an invalid user ID or password. Please note that your password is case-sensitive."
                            });
                        }
                    }
                    else
                    {
                        // Validate the username/password parameters and ensure the account is not locked out.
                        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

                        // Ensure the user is not already locked out.
                        if (result.IsLockedOut)
                        {
                            log.Message = "Failed " + message + " Error: The specified user account has been suspended.";
                            await _authLogService.CreateAsync(log);

                            return BadRequest(new OpenIddictResponse
                            {
                                Error = OpenIddictConstants.Errors.InvalidGrant,
                                ErrorDescription = "The specified user account has been suspended"
                            });
                        }

                        // Reject the token request if two-factor authentication has been enabled by the user.
                        if (result.RequiresTwoFactor)
                        {
                            log.Message = "Failed " + message + " Error: Invalid login procedure.";
                            await _authLogService.CreateAsync(log);

                            return BadRequest(new OpenIddictResponse
                            {
                                Error = OpenIddictConstants.Errors.InvalidGrant,
                                ErrorDescription = "Invalid login procedure"
                            });
                        }
                        // Ensure the user is allowed to sign in.
                        if (result.IsNotAllowed)
                        {
                            log.Message = "Failed " + message + " Error: The specified user is not allowed to sign in.";
                            await _authLogService.CreateAsync(log);

                            return BadRequest(new OpenIddictResponse
                            {
                                Error = OpenIddictConstants.Errors.InvalidGrant,
                                ErrorDescription = "The specified user is not allowed to sign in"
                            });
                        }

                        if (!result.Succeeded)
                        {
                            log.Message = "Failed " + message + " Error: You have entered an invalid user ID or password. Please note that your password is case-sensitive.";
                            await _authLogService.CreateAsync(log);

                            return BadRequest(new OpenIddictResponse
                            {
                                Error = OpenIddictConstants.Errors.InvalidGrant,
                                ErrorDescription = "You have entered an invalid user ID or password. Please note that your password is case-sensitive."
                            });
                        }
                    }


                    if (_signInManager.Options.SignIn.RequireConfirmedEmail && !(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                        log.Message = "Failed " + message + " Error: Please confirm your email first.";
                        await _authLogService.CreateAsync(log);

                        return BadRequest(new OpenIddictResponse
                        {
                            Error = OpenIddictConstants.Errors.InvalidGrant,
                            ErrorDescription = "Please confirm your email first"
                        });
                    }
                }

                // Create a new authentication ticket.
                //var ticket = await CreateTicketAsync(request, user);

                user.LastLoginTime = DateTime.Now;
                await _userManager.UpdateAsync(user);

                log.Message = "Successful " + message;
                await _authLogService.CreateAsync(log);

                if (needConfirmationCode && !string.IsNullOrEmpty(user.Email))
                {
                    Random generator = new Random();
                    String r = generator.Next(0, 1000000).ToString("D6");

                    user.ConfirmationCode = r;

                    var externalAppLog = new ExternalAppLoginLogDTO
                    {
                        AppId = appId,
                        Email = user.Email,
                        EventDateTime = DateTime.Now,
                        UserId = user.Id,
                        Message = log.Message,
                        Username = user.UserName
                    };

                    await _auditLogService.CreateExternalLoginLogAsync(externalAppLog);
                    await _userManager.UpdateAsync(user);

                    if (mfaValidation)
                    {
                        if (mfa)
                            await _emailSender.SendEmailAsync("Web Admin Portal", "smv.notification@gmail.com", user.FullName, user.Email, "Web Admin Portal Confirmation Code", $"Confirmation Code is {user.ConfirmationCode}, \n\nDo not give the code to anyone, including system admin.");
                    }
                    else
                    {
                        var isSuccess = await _emailSender.SendEmailAsync("GOe", "smv.notification@gmail.com", user.FullName, user.Email, "GOe Confirmation Code", $"Confirmation Code is {user.ConfirmationCode}, \n\nDo not give the code to anyone, including system admin.");
                    }

                    //await _emailController.SendEmailFromQueue();
                }

                if (isExternal)
                {


                    return new JsonResult(new
                    {
                        IsValidUser = true,
                        UserId = user.Id,
                        InstitutionId = user.InstitutionId,
                        FullName = user.FullName,
                        UserName = user.UserName,
                        InstitutionName = user.Institution.Name,
                        Pin = user.Pin,
                        Email = user.Email,
                        Status = !string.IsNullOrEmpty(user.Status) ? user.Status : UserConnectionStatus.OFFDUTY.ToString(),
                        FirebaseToken = user.FirebaseToken,
                        ConfirmationCode = needConfirmationCode ? user.ConfirmationCode : "",
                        NotFirstLogin = user.NotFirstLogin,
                        ConfirmReadTermsConditions = user.ConfirmReadTermsConditions,
                        ConsentDataCollection = user.ConsentDataCollection,
                        ReceivePromotionalMaterials = user.ReceivePromotionalMaterials
                    });
                }


                var principal = await CreatePrincipal(request, user);
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                //var ticket2 = await CreateTicketAsync(request, user);

                //return SignIn(ticket2.Principal, ticket2.Properties, ticket2.AuthenticationScheme);
            }
            else if (request.IsRefreshTokenGrantType())
            {
                // Retrieve the claims principal stored in the refresh token.
                var info = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Retrieve the user profile corresponding to the refresh token.
                // Note: if you want to automatically invalidate the refresh token
                // when the user password/roles change, use the following line instead:
                // var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
                var user = await _userManager.GetUserAsync(info.Principal);
                if (user == null)
                {
                    log.Message = "Failed " + message;
                    await _authLogService.CreateAsync(log);

                    return BadRequest(new OpenIddictResponse
                    {
                        Error = OpenIddictConstants.Errors.InvalidGrant,
                        ErrorDescription = "The refresh token is no longer valid"
                    });
                }

                // Ensure the user is still allowed to sign in.
                if (!await _signInManager.CanSignInAsync(user))
                {
                    log.Message = "Failed " + message;
                    await _authLogService.CreateAsync(log);

                    return BadRequest(new OpenIddictResponse
                    {
                        Error = OpenIddictConstants.Errors.InvalidGrant,
                        ErrorDescription = "The user is no longer allowed to sign in"
                    });
                }

                // Create a new authentication ticket, but reuse the properties stored
                // in the refresh token, including the scopes originally granted.
                //var ticket = await CreateTicketAsync(request, user);

                user.LastLoginTime = DateTime.Now;
                await _userManager.UpdateAsync(user);

                log.Message = "Successful " + message;
                await _authLogService.CreateAsync(log);
                
                if (isExternal)
                {
                    return new JsonResult(new
                    {
                        IsValidUser = true,
                        UserId = user.Id,
                        InstitutionId = user.InstitutionId,
                        FullName = user.FullName,
                        UserName = user.UserName,
                        InstitutionName = user.Institution.Name,
                        Pin = user.Pin,
                        Email = user.Email,
                        Status = !string.IsNullOrEmpty(user.Status) ? user.Status : UserConnectionStatus.OFFDUTY.ToString(),
                        FirebaseToken = user.FirebaseToken
                    });
                }

                var principal = await CreatePrincipal(request, user);
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                //return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
            }

            log.Message = "Failed " + message;
            await _authLogService.CreateAsync(log);

            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported"
            });
        }

        [HttpPost("~/login/multi-fa")]
        [Produces("application/json")]
        public async Task<IActionResult> ValidateLogin2FA(string userId, string code)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new Exception("User not found.");

                if (user.ConfirmationCode == code)
                {
                    user.Last2FAValidatedTime = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    return Ok(new { validated = true });
                }
                else
                {
                    return BadRequest(new OpenIddictResponse
                    {
                        Error = OpenIddictConstants.Errors.InvalidToken,
                        ErrorDescription = "Invalid code."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("~/login/multi-fa/resend")]
        [Produces("application/json")]
        public async Task<IActionResult> Resend2FACode(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new Exception("User not found.");

                Random generator = new Random();
                string code = generator.Next(0, 1000000).ToString("D6");

                user.ConfirmationCode = code;
                await _userManager.UpdateAsync(user);

                var response = await _emailSender.SendEmailAsync("Web Admin Portal", "smv.notification@gmail.com", user.FullName, user.Email, "Web Admin Portal Confirmation Code", $"Confirmation Code is {user.ConfirmationCode}, \n\nDo not give the code to anyone, including system admin.");

                return Ok(new { sent = response.success, errorMsg = response.errorMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> MultiFactorLogin(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if(user == null)
            {
                return BadRequest(new OpenIddictResponse
                {
                    Error = OpenIddictConstants.Errors.RequestNotSupported,
                    ErrorDescription = "User not found."
                });
            }

            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(_httpAccessor.HttpContext.Request.Headers["User-Agent"]);
            string message = string.Format(" sign in with access to the web admin portal");
            string ip = _httpAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (c.Device != null && !string.IsNullOrEmpty(c.Device.Family))
            {
                message += string.Format(" .Device: {0}, OS: {1}, Agent: {2}, IP: {3}", c.Device.ToString(), c.OS.ToString(), c.UserAgent.ToString(), ip);
            }
            else
            {
                message += string.Format(" with IP {0}", ip);
            }

            Random generator = new Random();
            string code = generator.Next(0, 1000000).ToString("D6");

            user.ConfirmationCode = code;

            var externalAppLog = new ExternalAppLoginLogDTO
            {
                Email = user.Email,
                EventDateTime = DateTime.Now,
                UserId = user.Id,
                Message = "Successful",
                Username = user.UserName
            };

            await _auditLogService.CreateExternalLoginLogAsync(externalAppLog);
            await _userManager.UpdateAsync(user);

            var isSuccess = await _emailSender.SendEmailAsync("Web Admin Portal", "smv.notification@gmail.com", user.FullName, user.Email, "Web Admin Portal Confirmation Code", $"Confirmation Code is {user.ConfirmationCode}, \n\nDo not give the code to anyone, including system admin.");


            if (!isSuccess.success)
                return View("Error");

            var model = new AdminPortal2FA { Code = code, UserId = userId };

            return View("AdminPortal2FA", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MultiFactorLogin(AdminPortal2FA model)
        {
            if (!ModelState.IsValid)
                return View("AdminPortal2FA", model);

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                RedirectToAction(nameof(MultiFactorLogin));

            if(user.ConfirmationCode == model.Code)
            {
                user.Last2FAValidatedTime = DateTime.Now;
                await _userManager.UpdateAsync(user);

                return Redirect("/");
            }
            else
            {
                ModelState.TryAddModelError("602", "Invalid code");
                model.Retry++;
                if(model.Retry > 5)
                {
                    return Redirect("/");
                }

                return View("AdminPortal2FA", model);
            }
        }

        public async Task<ActionResult> ConfirmEmail(string userId, string code, string passwordSetCode)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest(new OpenIddictResponse
                {
                    Error = OpenIddictConstants.Errors.InvalidGrant,
                    ErrorDescription = "Please make sure you have a correct link to confirm your email"
                });
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (result.Succeeded && !string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("ResetPassword", "Authorization", new { userId = userId, code = token, firstPassword = true });
                }

                return Ok("Thank you for confirming your email. A separate email will be sent to you when your account is ready.");
                //return View("ConfirmEmail");
            }

            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = "Please make sure you have a correct link to confirm your email"
            });
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(int userId, string code, bool firstPassword = false)
        {
            if (code == null)
                return View("Error");

            var model = new ResetPasswordModel { Token = code, UserId = userId };

            return firstPassword ? View("FirstPassword", model) : View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordModel);

            var user = await _userManager.FindByIdAsync(resetPasswordModel.UserId.ToString());
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View("ResetPassword");
            }
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> StudentLinkAccount(int studentId, int? userId, string email)
        {
            if (!userId.HasValue && string.IsNullOrEmpty(email))
                return View("Error");

            if (userId.HasValue)
            {
                var resp = await _studentService.AddStudentLinksByUserAsync(userId.Value, studentId);
                if (resp.IsSuccess)
                {
                    await _studentService.UpdateStudentAccountLinkRequestAsync(studentId, email);

                    //existing tappee user
                    var url = _configuration["AppSettings:TAPPEE_URL"];
                    ViewBag.TappeeUrl = url ?? "/";
                    return View("StudentLinkAccountConfirmation");
                }
                else
                {
                    ViewBag.ErrorMessage = resp.Message;
                    return View("Error");
                }
            }
            else
            {
                var model = new StudentLinkAccountModel { Email = email, StudentId = studentId };
                return View("StudentLinkAccount", model);
            }

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentLinkAccount(StudentLinkAccountModel model)
        {
            if (!ModelState.IsValid)
                return View("StudentLinkAccount", model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                ViewBag.ErrorMessage = "Email already exist.";
                return View("Error");
            }

            user = new ApplicationUser();
            user.IsEnabled = true;
            user.EmailConfirmed = true;
            user.UserName = model.Email;
            user.Email = model.Email;
            user.IsActive = true;
            var createUserResult = await _accountManager.CreateUserWithPasswordAsync(user, new List<string>(), model.Password);

            if (createUserResult.Item1)
            {
                return RedirectToAction(nameof(StudentLinkAccount), new { studentId = model.StudentId, userId = user.Id, email = model.Email });
            }
            else
            {
                foreach(var error in createUserResult.Item2)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                
                return View("StudentLinkAccount", model);
            }
        }

        public IActionResult SignInWithGoogle(string institutionCode)
        {
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Google", Url.Action(nameof(HandleExternalLogin), new { institutionCode = institutionCode }));
            return Challenge(authenticationProperties, "Google");
        }

        public async Task<IActionResult> HandleExternalLogin(string institutionCode)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            ApplicationUser user1 = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (!result.Succeeded) //user does not exist yet
            {
                //var newUser = new ApplicationUser
                //{
                //    UserName = email,
                //    Email = email,
                //    EmailConfirmed = true
                //};
                //var createResult = await _userManager.CreateAsync(newUser);
                //if (!createResult.Succeeded)
                //    throw new Exception(createResult.Errors.Select(e => e.Description).Aggregate((errors, error) => $"{errors}, {error}"));

                //await _userManager.AddLoginAsync(newUser, info);

                //var newUserClaims = info.Principal.Claims.Append(new Claim("Id", newUser.Id.ToString()));
                //await _userManager.AddClaimsAsync(newUser, newUserClaims);
                //await _signInManager.SignInAsync(newUser, isPersistent: false);
                //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                return Redirect(string.Format("/register?email={0}&provider={1}&key={2}{3}", HttpUtility.UrlEncode(email), info.LoginProvider, info.ProviderKey, !string.IsNullOrEmpty(institutionCode) ? "&institutionCode=" + institutionCode : string.Empty));
            }

            _userManager.InstitutionCode = institutionCode;
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && !user.InstitutionId.HasValue)
            {
                return Redirect(string.Format("/register?email={0}&provider={1}&key={2}", HttpUtility.UrlEncode(email), info.LoginProvider, info.ProviderKey));
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            //var request = new OpenIdConnectRequest { Scope = "openid email phone profile offline_access roles" };
            //var ticket = await CreateTicketAsync(request, user);

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            //SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);

            //return await Exchange(request);
            return Redirect("/externallogin?email=" + HttpUtility.UrlEncode(email));
        }

        public async Task<IActionResult> AttendanceSignIn()
        {
            return Redirect("/attendancesignin");
        }

        public async Task<IActionResult> ChangePassword()
        {
            return Redirect("/changepassword");
        }

        [HttpGet("~/logotp")]
        [HttpPost("~/logotp")]
        [Produces("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> LogOtpAttempt(string username, bool isSuccessful)
        {
            var log = new AuthenticationLogDTO
            {
                CreatedDate = DateTime.Now,
                UserName = username,
                Message = isSuccessful ? $"Successfully validated the OTP for {username}" : $"Failed OTP Validation Error: Incorrect OTP for {username}"
            };

            var response = await _authLogService.CreateAsync(log);

            return Ok(response);
        }

        private async Task<AuthenticationTicket> CreateTicketAsync(OpenIddictRequest request, ApplicationUser user)
        {
            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            //var principalIdentity = principal.Identity as ClaimsIdentity;

            //if (this._userManager.SupportsUserRole && user.Roles != null)
            //{
            //    foreach (var role in user.Roles.Select(e => e.Role))
            //    {
            //        var roleClaims = await this._roleManager.GetClaimsAsync(role);
            //        principalIdentity.AddClaims(roleClaims);
            //    }
            //}

            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            //if (!request.IsRefreshTokenGrantType())
            //{
            // Set the list of scopes granted to the client application.
            // Note: the offline_access scope must be granted
            // to allow OpenIddict to return a refresh token.
            ticket.Principal.SetScopes(new[]
            {
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Phone,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Roles
            }.Intersect(request.GetScopes()));
            //}

            //ticket.SetResources("frs_api");
            // Uncomment if cookie affinity doesn't fix the issue
            //ticket.SetAudiences(_configuration.GetSection("Jwt:Audience").Get<string>());

            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            foreach (var claim in ticket.Principal.Claims)
            {
                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType || claim.Type == CustomClaimTypes.Permission)
                    continue;

                var destinations = new List<string> { OpenIddictConstants.Destinations.AccessToken }; // Default to access_token

                // Handle specific claim types
                if (claim.Type == OpenIddictConstants.Claims.Subject && ticket.Principal.HasScope(OpenIddictConstants.Scopes.OpenId))
                {
                    destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
                }
                else if (claim.Type == OpenIddictConstants.Claims.Name && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Profile))
                {
                    destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
                }
                else if (claim.Type == OpenIddictConstants.Claims.Role && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Roles))
                {
                    destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
                }
                //else if (claim.Type == CustomClaimTypes.Permission)
                //{
                //    // Only add permission claims to identity token
                //    //destinations.Clear(); // Remove any previously added destinations
                //    destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
                //}
                //else
                //{
                //    // Add other claims to access_token by default
                //    destinations.Add(OpenIddictConstants.Destinations.AccessToken);
                //}

                // Set claim destinations
                claim.SetDestinations(destinations.Distinct().ToList());

                //var destinations = new List<string> { OpenIddictConstants.Destinations.AccessToken };

                //// Only add the iterated claim to the id_token if the corresponding scope was granted to the client application.
                //// The other claims will only be added to the access_token, which is encrypted when using the default format.
                //if ((claim.Type == OpenIddictConstants.Claims.Subject && ticket.Principal.HasScope(OpenIddictConstants.Scopes.OpenId)) ||
                //    (claim.Type == OpenIddictConstants.Claims.Name && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Profile)) ||
                //    (claim.Type == OpenIddictConstants.Claims.Role && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Roles)) ||
                //    (claim.Type == CustomClaimTypes.Permission && IsIncludedCustomClaim(claim.Value)))
                //    //(claim.Type == CustomClaimTypes.Permission && ticket.HasScope(OpenIddictConstants.Claims.Roles)))
                //{
                //    //claim.SetDestinations(destinations);
                //    destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
                //}
                ////else
                ////{
                ////    claim.SetDestinations(new List<string> { OpenIddictConstants.Destinations.IdentityToken });
                ////    //destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
                ////}

                //claim.SetDestinations(destinations);
            }


            var identity = ticket.Principal.Identity as ClaimsIdentity;

            identity.AddClaim(OpenIddictConstants.Claims.Audience, _configuration.GetSection("Jwt:Audience").Get<string>(), OpenIddictConstants.Destinations.AccessToken);
            identity.AddClaim(OpenIddictConstants.Claims.Audience, _configuration.GetSection("Jwt:Audience").Get<string>(), OpenIddictConstants.Destinations.IdentityToken);

            if (ticket.Principal.HasScope(OpenIddictConstants.Scopes.Profile))
            {
                
                identity.AddClaim(CustomClaimTypes.UserId, user.Id + "", OpenIddictConstants.Destinations.IdentityToken);

                if (!string.IsNullOrWhiteSpace(user.ConfirmationCode))
                    identity.AddClaim(CustomClaimTypes.ConfirmationCode, user.ConfirmationCode, OpenIddictConstants.Destinations.IdentityToken);

                
                    identity.AddClaim(CustomClaimTypes.NotFirstLogin, user.NotFirstLogin ? "true" : "false" + "", OpenIddictConstants.Destinations.IdentityToken);

                identity.AddClaim(CustomClaimTypes.StudentNumber, user.Students.Where(s => s.IsActive).Count(), OpenIddictConstants.Destinations.IdentityToken);


                if (!string.IsNullOrWhiteSpace(user.JobTitle))
                    identity.AddClaim(CustomClaimTypes.JobTitle, user.JobTitle, OpenIddictConstants.Destinations.IdentityToken);

                if (!string.IsNullOrWhiteSpace(user.FullName))
                    identity.AddClaim(CustomClaimTypes.FullName, user.FullName, OpenIddictConstants.Destinations.IdentityToken);

                if (!string.IsNullOrWhiteSpace(user.Configuration))
                    identity.AddClaim(CustomClaimTypes.Configuration, user.Configuration, OpenIddictConstants.Destinations.IdentityToken);

                if (user.InstitutionId.HasValue)
                    identity.AddClaim(CustomClaimTypes.InstitutionId, user.InstitutionId.ToString(), OpenIddictConstants.Destinations.IdentityToken);
            }

            if (ticket.Principal.HasScope(OpenIddictConstants.Scopes.Email))
            {
                if (!string.IsNullOrWhiteSpace(user.Email))
                    identity.AddClaim(CustomClaimTypes.Email, user.Email, OpenIddictConstants.Destinations.IdentityToken);
            }

            if (ticket.Principal.HasScope(OpenIddictConstants.Scopes.Phone))
            {
                if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                    identity.AddClaim(CustomClaimTypes.Phone, user.PhoneNumber, OpenIddictConstants.Destinations.IdentityToken);
            }

            ticket.Principal.SetDestinations(claim =>
            {
                // Initialize the destinations with the default value
                //var destinations = new[] { OpenIddictConstants.Destinations.AccessToken };

                //// Check if the claim type matches specific criteria
                //if (claim.Type == OpenIddictConstants.Claims.Audience)
                //{
                //    destinations = new[]
                //    {
                //            OpenIddictConstants.Destinations.AccessToken,
                //            OpenIddictConstants.Destinations.IdentityToken
                //        };
                //}
                //else if (claim.Type == "userId")
                //{
                //    destinations = new[]
                //    {
                //        OpenIddictConstants.Destinations.IdentityToken
                //    };
                //}

                // Return the determined destinations
                return claim.GetDestinations();
            });

            return ticket;
        }

        private async Task<AuthenticationTicket> CreateTicketAsync2(OpenIddictRequest request, ApplicationUser user)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Define scopes
            var scopes = new[]
            {
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Email,
                OpenIddictConstants.Scopes.Phone,
                OpenIddictConstants.Scopes.Profile,
                OpenIddictConstants.Scopes.OfflineAccess,
                OpenIddictConstants.Scopes.Roles
            };

            // Set the list of scopes granted to the client application
            ticket.Principal.SetScopes(scopes.Intersect(request.GetScopes()));

            // Add custom claims
            var identity = ticket.Principal.Identity as ClaimsIdentity;

            if (identity != null)
            {
                // Add claims based on scopes
                foreach (var claim in ticket.Principal.Claims)
                {
                    var destinations = new List<string> { OpenIddictConstants.Destinations.AccessToken };

                    if (claim.Type == OpenIddictConstants.Claims.Subject && ticket.Principal.HasScope(OpenIddictConstants.Scopes.OpenId))
                    {
                        destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
                    }
                    else if (claim.Type == OpenIddictConstants.Claims.Name && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Profile))
                    {
                        destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
                    }
                    else if (claim.Type == OpenIddictConstants.Claims.Role && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Roles))
                    {
                        destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
                    }

                    claim.SetDestinations(destinations.Distinct().ToList());
                }

                identity.AddClaim(OpenIddictConstants.Claims.Audience, _configuration.GetSection("Jwt:Audience").Get<string>(), OpenIddictConstants.Destinations.AccessToken);
                identity.AddClaim(OpenIddictConstants.Claims.Audience, _configuration.GetSection("Jwt:Audience").Get<string>(), OpenIddictConstants.Destinations.IdentityToken);
                identity.AddClaim(CustomClaimTypes.UserId, user.Id + "", OpenIddictConstants.Destinations.IdentityToken);

                //ticket.Principal.SetResources(_configuration.GetSection("Jwt:Audience").Get<string>());
            }

            ticket.Principal.SetDestinations(claim =>
            {
                // Initialize the destinations with the default value
                var destinations = new[] { OpenIddictConstants.Destinations.AccessToken };

                // Check if the claim type matches specific criteria
                if (claim.Type == OpenIddictConstants.Claims.Audience)
                {
                    destinations = new[]
                    {
                            OpenIddictConstants.Destinations.AccessToken,
                            OpenIddictConstants.Destinations.IdentityToken
                        };
                }
                else if (claim.Type == "userId")
                {
                    destinations = new[]
                    {
                        OpenIddictConstants.Destinations.IdentityToken
                    };
                }

                // Return the determined destinations
                return destinations;
            });

            foreach (var claim in identity.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            return ticket;
        }

        private async Task<ClaimsPrincipal> CreatePrincipal(OpenIddictRequest request, ApplicationUser user)
        {
            var principalIdentity = await _signInManager.CreateUserPrincipalAsync(user);
            var principal = new ClaimsPrincipal(principalIdentity.Identity);
            principal.SetScopes(new[]
            {
                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Scopes.Phone,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.OfflineAccess,
                        OpenIddictConstants.Scopes.Roles
                }.Intersect(request.GetScopes()));

            var audience = _configuration.GetSection("Jwt:Audience").Get<string>();
            principalIdentity.AddClaim(OpenIddictConstants.Claims.Audience, audience, OpenIddictConstants.Destinations.AccessToken);
            principalIdentity.AddClaim(OpenIddictConstants.Claims.Audience, audience, OpenIddictConstants.Destinations.IdentityToken);
            //principalIdentity.AddClaim("userId", user.Id.ToString(), OpenIddictConstants.Destinations.IdentityToken);

            if (principalIdentity.HasScope(OpenIddictConstants.Scopes.Profile))
            {
                principalIdentity.AddClaim(CustomClaimTypes.UserId, user.Id + "", OpenIddictConstants.Destinations.IdentityToken);

                if (!string.IsNullOrWhiteSpace(user.ConfirmationCode))
                    principalIdentity.AddClaim(CustomClaimTypes.ConfirmationCode, user.ConfirmationCode, OpenIddictConstants.Destinations.IdentityToken);


                principalIdentity.AddClaim(CustomClaimTypes.NotFirstLogin, user.NotFirstLogin ? "true" : "false", OpenIddictConstants.Destinations.IdentityToken);

                principalIdentity.AddClaim(CustomClaimTypes.StudentNumber, user.Students.Where(s => s.IsActive).Count(), OpenIddictConstants.Destinations.IdentityToken);


                if (!string.IsNullOrWhiteSpace(user.JobTitle))
                    principalIdentity.AddClaim(CustomClaimTypes.JobTitle, user.JobTitle, OpenIddictConstants.Destinations.IdentityToken);

                if (!string.IsNullOrWhiteSpace(user.FullName))
                    principalIdentity.AddClaim(CustomClaimTypes.FullName, user.FullName, OpenIddictConstants.Destinations.IdentityToken);

                if (!string.IsNullOrWhiteSpace(user.Configuration))
                    principalIdentity.AddClaim(CustomClaimTypes.Configuration, user.Configuration, OpenIddictConstants.Destinations.IdentityToken);

                if (user.InstitutionId.HasValue)
                    principalIdentity.AddClaim(CustomClaimTypes.InstitutionId, user.InstitutionId.ToString(), OpenIddictConstants.Destinations.IdentityToken);
            }

            if (principalIdentity.HasScope(OpenIddictConstants.Scopes.Email))
            {
                if (!string.IsNullOrWhiteSpace(user.Email))
                    principalIdentity.AddClaim(CustomClaimTypes.Email, user.Email, OpenIddictConstants.Destinations.IdentityToken);
            }

            if (principalIdentity.HasScope(OpenIddictConstants.Scopes.Phone))
            {
                if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                    principalIdentity.AddClaim(CustomClaimTypes.Phone, user.PhoneNumber, OpenIddictConstants.Destinations.IdentityToken);
            }

            //principal.SetScopes(request.GetScopes());
            //principal.SetResources("resource_server");

            principal.SetDestinations(claim =>
            {
                // Initialize the destinations with the default value
                var destinations = new[] { OpenIddictConstants.Destinations.AccessToken };

                // Check if the claim type matches specific criteria
                if (claim.Type == OpenIddictConstants.Claims.Audience || claim.Type == OpenIddictConstants.Claims.Subject && principal.HasScope(OpenIddictConstants.Scopes.OpenId) ||
                    claim.Type == OpenIddictConstants.Claims.Name && principal.HasScope(OpenIddictConstants.Scopes.Profile) ||
                    claim.Type == OpenIddictConstants.Claims.Role && principal.HasScope(OpenIddictConstants.Scopes.Roles))
                {
                    destinations = new[]
                    {
                            OpenIddictConstants.Destinations.AccessToken,
                            OpenIddictConstants.Destinations.IdentityToken
                        };
                }
                else if (claim.Type == CustomClaimTypes.UserId || claim.Type == CustomClaimTypes.ConfirmationCode ||
                    claim.Type == CustomClaimTypes.NotFirstLogin ||
                    claim.Type == CustomClaimTypes.JobTitle ||
                    claim.Type == CustomClaimTypes.FullName ||
                    claim.Type == CustomClaimTypes.Configuration ||
                    claim.Type == CustomClaimTypes.InstitutionId ||
                    claim.Type == CustomClaimTypes.Email ||
                    claim.Type == CustomClaimTypes.Phone)
                {
                    destinations = new[]
                    {
                            OpenIddictConstants.Destinations.IdentityToken
                        };
                }

                // Return the determined destinations
                return destinations;
            });

            return principal;
        }

        private bool IsIncludedCustomClaim(string value)
        {
            var customClaims = new List<string>
            {
                ApplicationPermissionsTrees.SSUserMenu,
                ApplicationPermissionsTrees.SSManageUserMenu,
                ApplicationPermissionsTrees.SSRoleMenu,
                ApplicationPermissionsTrees.SSAssignRoleMenu,
                ApplicationPermissionsTrees.SSManageRoleMenu,
                ApplicationPermissionsTrees.SSDepartmentMenu,
                ApplicationPermissionsTrees.SSDepartmentManage
            };

            return customClaims.Contains(value);
        }
        private bool ValidateADAccount(string username, string userpassword)
        {
            var ldapServer = _configuration["AppSettings:AD_Server"];
            var uname = _configuration["AppSettings:AD_Username"];
            var password = _configuration["AppSettings:AD_Password"];

            using (var context =
                (!string.IsNullOrWhiteSpace(uname) && !string.IsNullOrWhiteSpace(password))
                ? new PrincipalContext(ContextType.Domain, ldapServer, uname, password)
                : new PrincipalContext(ContextType.Domain, ldapServer))
            {
                return context.ValidateCredentials(username, userpassword, ContextOptions.Negotiate);
            }
        }
    }
}
