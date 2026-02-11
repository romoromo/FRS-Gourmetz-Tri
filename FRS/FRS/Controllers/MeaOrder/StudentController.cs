using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Helpers;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using FRS.Attributes;
using FRS.Authorization;
using FRS.Helpers;
using FRS.Hubs;
using FRS.ViewModels;
using FRS.ViewModels.MealOrder;
using MealOrderPayments.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OpenIddict.Validation.AspNetCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace FRS.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class StudentController : BaseController
    {
        private IStudentService _service;
        readonly ILogger _logger;
        private readonly IAccountManager _accountManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationUserManager _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        private IStudentWalletService _walletService;
        private IStudentPointService _pointService;
        private OrderController _orderController;

        public StudentController(IStudentService service, ILogger<StudentController> logger, IAccountManager accountManager, IEmailSender emailSender, ApplicationUserManager userManager, IConfiguration configuration, IMapper mapper
            , IStudentWalletService walletService, IStudentPointService pointService, OrderController orderController)
        {
            _service = service;
            _logger = logger;
            _accountManager = accountManager;
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _walletService = walletService;
            _pointService = pointService;
            _orderController = orderController;
        }

        #region Students

        #region Sieved
        //[ApiKeyAuthorize]
        [HttpGet("students/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<StudentDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStudents(BaseFilter filter, bool noAccount = false)
        {
            var results = await this._service.GetStudentsAsync(filter, noAccount);
            return Ok(_mapper.Map<PagedEntityViewModel<StudentDTO>>(results));
        }

        //[ApiKeyAuthorize]
        [HttpGet("students/simple/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<StudentSimpleDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetSimpleStudents(BaseFilter filter, bool noAccount = false)
        {
            var results = await this._service.GetSimpleStudentsAsync(filter, noAccount);
            return Ok(_mapper.Map<PagedEntityViewModel<StudentSimpleDTO>>(results));
        }

        #endregion

        [HttpGet("students/get/user/{userId}")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<StudentDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStudentsByUser(int userId)
        {
            try
            {
                var results = await this._service.GetStudentsByUserAsync(userId);

                foreach (var student in results)
                {
                    await _orderController.QueryAndUpdateWalletPaymentStatus(student.Id);
                }

                results = await this._service.GetStudentsByUserAsync(userId);

                return Ok(_mapper.Map<List<StudentDTO>>(results));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("students/get/id/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(StudentDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStudent(int id)
        {
            var dto = await this._service.GetStudentByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("students")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(201, Type = typeof(StudentDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateStudentAsync(dto);
                if (result.IsSuccess)
                {
                    StudentDTO vm = _mapper.Map<StudentDTO>(result.Data);
                    return CreatedAtAction("GetStudentById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("students/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(StudentDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var dto = await this._service.GetStudentByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteStudentAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("students/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] StudentDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetStudentByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                await SaveStudentPicture(model);

                var result = await this._service.UpdateStudentAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        private async Task SaveStudentPicture(StudentDTO model)
        {
            if (!string.IsNullOrEmpty(model.ImgUrl))
            {
                var folderName = Path.Combine("Resources", "StudentPictures", model.Id.ToString());
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                var base64Image = model.ImgUrl;
                var offset = base64Image.Substring(base64Image.IndexOf(',') + 1);

                var imageInBytes = Convert.FromBase64String(offset);
                using (var ms = new MemoryStream(imageInBytes))
                {
                    var fullPath = Path.Combine(pathToSave, model.PhotoName);
                    var dbPath = Path.Combine(folderName, model.PhotoName);

                    model.PhotoPath = dbPath;

                    using (FileStream file = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                    {
                        ms.WriteTo(file);
                    }
                }
            }
        }

        [HttpPut("students/transfer-class/{originClassId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TransferClass([FromRoute] int originClassId, [FromBody] StudentDTO model)
        {
            try
            {
                string dataJson = JsonConvert.SerializeObject(model);
                _logger.LogInformation($"TransferClass UserEditViewModel : {dataJson}");
                _logger.LogInformation($"TransferClass originClassId : {originClassId}");


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(await _service.UpdateStudentClassByClassId(model, originClassId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error TransferClass originClassId : {originClassId}");
                _logger.LogError($"Error TransferClass : {ex.Message}", ex);
                _logger.LogError($"Error TransferClass : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [HttpPost("students/createaccount")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateAccount([FromBody] List<int> ids)
        {
            var result = await this._service.CreateAccountAsync(ids, _configuration["AppSettings:ONBOARDING_GENERATE_PASSWORD"] == "Y", _configuration["AppSettings:ONBOARDING_DEFAULT_PASSWORD"]);

            string enableOnboardingEmail = _configuration["AppSettings:ONBOARDING_EMAIL_ENABLED"];
            foreach (var id in ids)
            {
                var student = await this._service.GetStudentByIdAsync(id);

                var title = $"GOe Login Information.";
                var content = $"Hi {student.Name}.\n" +
                    "Please find below your login information for GOe.\n" +
                    $"Username: {student.Email}\n" +
                    $"Password: {student.NewPassword}\n\n" +
                    ""
                    ;

                var user = await _userManager.FindByIdAsync(student.UserId?.ToString());

                if (enableOnboardingEmail == "Y")
                {
                    if (user != null)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        string baseUrl = _configuration["AppSettings:ONBOARDING_BASE_URL"];
                        var callbackUrl = Url.Action("ConfirmEmail", "Authorization", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme, host: baseUrl);
                        var portalUrl = _configuration["AppSettings:ORDER_PORTAL_URL"];
                        var imgUrl = _configuration["AppSettings:ORDER_PORTAL_ONBOARDING_IMG_URL"];
                        await _emailSender.SendEmailAsync(student.Name, student.Email, "Confirm your account",
                            EmailTemplates.GetConfirmationEmail(portalUrl, student.Email, callbackUrl, imgUrl));
                    }
                    else
                    {
                        var emailBody = EmailTemplates.GetPlainTextTestEmail(DateTime.Now);
                        var isSuccess = await _emailSender.SendEmailAsync(student.Name, student.Email, title, content);
                    }
                }
            }

            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while create account: " + string.Join(", ", result.Message));

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("students/import"), DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<IActionResult> Import()
        {

            var file = Request.Form.Files[0];
            var folderName = Path.Combine("Resources", "Excel");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            Directory.CreateDirectory(pathToSave);

            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                fileName = Guid.NewGuid().ToString() + fileName;
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var resp = MapExcel(fullPath);

                if (resp.Item1)
                {
                    //start importing to DB
                    return Ok(await _service.ImportStudentAsync(resp.Item3));
                }
                return Ok(new { IsSuccess = resp.Item1, Message = resp.Item2, Data = resp.Item3 });
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPost("students/report/export")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateStudentReportXls(BaseFilter filter)
        {
            var xls = await _service.GenerateStudentReportXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_StudentReport.xlsx";

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

        [HttpPost("students/list/export/{outletId:int}/{studentGroupId:int}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateStudentListXls([FromRoute] int outletId, [FromRoute] int studentGroupId)
        {
            var xls = await _service.GenerateStudentListXls(outletId, studentGroupId);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_StudentList.xlsx";

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

        [HttpPost("students/cards/import/template-with-details")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateStudentReportForImportXls(BaseFilter filter)
        {
            var xls = await _service.GenerateStudentReportForImportXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_Template with Student Information.xlsx";

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

        private Tuple<bool, string, List<StudentImportDTO>> MapExcel(string filePath)
        {
            try
            {
                IWorkbook wb = new XSSFWorkbook(new FileStream(filePath, FileMode.Open));
                var sheet = (XSSFSheet)wb.GetSheetAt(0);
                int colCount = sheet.GetRow(1).PhysicalNumberOfCells;

                if (colCount != 8) //number of columns required
                {
                    return new Tuple<bool, string, List<StudentImportDTO>>(false, "There is a mismatch on the number of columns required. Please check the file.", null);
                }
                else
                {
                    var row = new List<StudentImportDTO>();
                    int rowCount = sheet.PhysicalNumberOfRows;

                    for (int i = 1; i < rowCount; i++)
                    {
                        var dto = new StudentImportDTO();
                        var fRow = sheet.GetRow(i);

                        if (fRow != null)
                        {
                            int c = 0;
                            var fname = fRow.GetCell(c++);
                            dto.Name = fname != null ? fname.ToString().Trim() : "";

                            var sClass = fRow.GetCell(c++);
                            dto.Class = sClass != null ? sClass.ToString().Trim() : "";

                            var batch = fRow.GetCell(c++);
                            dto.Batch = batch != null ? batch.ToString().Trim() : "";

                            var email = fRow.GetCell(c++);
                            dto.Email = email != null ? email.ToString().Trim() : "";

                            var gender = fRow.GetCell(c++);
                            dto.Gender = gender != null ? gender.ToString().Trim() : "";

                            var isFAS = fRow.GetCell(c++);
                            dto.IsFAS = isFAS != null && isFAS.ToString().Trim() == "Y" ? true : false;

                            var weightCell = fRow.GetCell(c++);
                            float weight = 0;

                            if (weightCell != null)
                            {
                                float.TryParse(weightCell.ToString(), out weight);
                            }
                            dto.Weight = weight;

                            var heightCell = fRow.GetCell(c++);
                            float height = 0;

                            if (heightCell != null)
                            {
                                float.TryParse(heightCell.ToString(), out height);
                            }

                            dto.Height = height;
                            row.Add(dto);
                        }

                    }

                    return new Tuple<bool, string, List<StudentImportDTO>>(true, "File format is correct", row);
                }

            }
            catch (Exception e)
            {
                return new Tuple<bool, string, List<StudentImportDTO>>(false, e.Message, null);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("students/import/studentgroup"), DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<IActionResult> ImportStudentGroup()
        {
            try
            {
                var file = Request.Form.Files[0];
                string studentGroupParam = Request.Form["studentGroupId"];
                int studentGroupId = 0;
                bool isValidStudentGroupId = int.TryParse(studentGroupParam, out studentGroupId);

                string userIdParam = Request.Form["userId"];
                int userId = 0;
                bool isValidUserId = int.TryParse(userIdParam, out userId);

                if (string.IsNullOrEmpty(studentGroupParam) && !isValidStudentGroupId)
                    return BadRequest("Student Group Id is missing.");

                if (string.IsNullOrEmpty(userIdParam) && !isValidUserId)
                    return BadRequest("User Id is missing.");

                var folderName = Path.Combine("Resources", "Excel");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                Directory.CreateDirectory(pathToSave);

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    fileName = Guid.NewGuid().ToString() + fileName;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    IWorkbook wb = new XSSFWorkbook(new FileStream(fullPath, FileMode.Open));
                    var studentIds = new List<int>();
                    int startRow = 1;
                    for (int x = 0; x < wb.NumberOfSheets; x++)
                    {
                        var sheet = (XSSFSheet)wb.GetSheetAt(x);
                        int reqNumCells = startRow - 1;
                        int colCount = sheet.GetRow(reqNumCells).PhysicalNumberOfCells;

                        int rowCount = sheet.PhysicalNumberOfRows;
                        for (int i = startRow; ExcelUtility.GetRowWithNonEmptyCell(sheet, i) != null; i++)
                        {
                            var fRow = sheet.GetRow(i);

                            if (fRow != null)
                            {
                                int c = 0;
                                var fname = fRow.GetCell(c++);
                                if (fname != null && int.TryParse(fname.ToString(), out int idGroup))
                                    studentIds.Add(idGroup);
                            }

                        }
                    }
                    bool isSuccess = await _service.ImportStudentGroupAsync(studentIds, studentGroupId, userId);
                    return Ok(new { IsSuccess = isSuccess, Message = isSuccess ? "File Imported!" : "Import Failed!" });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ImportStudentGroup Id : {Request.Form["studentGroupId"]}");
                _logger.LogError($"Error ImportStudentGroup : {ex.Message}", ex);
                _logger.LogError($"Error ImportStudentGroup : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }

        }

        [HttpGet("students/byoutlet")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<StudentLiteDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStudentByOutlet(int outletId)
        {
            var results = await this._service.GetStudentByOutletAsync(outletId);
            return Ok(results);
        }

        [HttpGet("students/byoutletwithclass")]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<StudentLiteWithClassDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStudentByOutletWithClass(int outletId)
        {
            var results = await this._service.GetStudentByOutletWithClassAsync(outletId);
            return Ok(results);
        }

        #endregion

        #region Student Cards

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("studentcards/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentCardsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStudentCards(BaseFilter filter)
        {
            var results = await this._service.GetStudentCardsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<StudentCardDTO>>(results));
        }

        #endregion

        [HttpGet("studentcards/get")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentCardsPolicy)]
        [ProducesResponseType(200, Type = typeof(StudentCardDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStudentCard(int? id, string cardId)
        {
            var dto = await this._service.GetStudentCardByIdAsync(id.GetValueOrDefault(), cardId);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpGet("studentcards/get/student")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(StudentDTO))]
        public async Task<IActionResult> GetStudentByCardIdAsync(string cardId)
        {
            var student = await _service.GetStudentByCardIdAsync(cardId);

            if (student == null)
            {
                return NotFound("Card Id: " + cardId + " is not found or Inactive");
            }

            return Ok(student);
        }

        [HttpGet("studentcards/get/studentweb")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(StudentDTO))]
        public async Task<IActionResult> GetStudentByCardIdWebAsync(string cardId)
        {
            var student = await _service.GetStudentByCardIdAsync(cardId);

            return Ok(student);
        }

        [HttpGet("studentcards/activate")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        public async Task<IActionResult> ActivateStudentByCardIdAsync(string cardId)
        {
            var result = await _service.ActivateStudentCardByIdAsync(cardId);

            return Ok(result);
        }

        [HttpPost("studentcards")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentCardsPolicy)]
        [ProducesResponseType(201, Type = typeof(StudentDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateStudentCard([FromBody] StudentCardDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");

                var card = await this._service.GetStudentCardByIdIncludeNonActiveAsync(0, dto.CardId);
                if (card != null)
                {
                    dto.Id = card.Id;
                    var result2 = await this._service.UpdateStudentCardAsync(dto);
                    if (result2.IsSuccess)
                    {
                        StudentCardDTO vm = _mapper.Map<StudentCardDTO>(result2.Data);
                        return CreatedAtAction("UpdatedExistingCard", new { id = vm.Id }, vm);
                    }
                }

                var result = await this._service.CreateStudentCardAsync(dto);
                if (result.IsSuccess)
                {
                    StudentCardDTO vm = _mapper.Map<StudentCardDTO>(result.Data);
                    return CreatedAtAction("GetStudentCardById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("studentcards/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentCardsPolicy)]
        [ProducesResponseType(200, Type = typeof(StudentDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStudentCard(int id)
        {
            var dto = await this._service.GetStudentCardByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteStudentCardAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("studentcards/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentCardsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStudentCard(string id, [FromBody] StudentCardDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetStudentCardByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateStudentCardAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("students/cards/import"), DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<IActionResult> ImportStudentCard()
        {

            var file = Request.Form.Files[0];
            string outletId = Request.Form["outletId"];
            string batch = Request.Form["batch"];
            if (string.IsNullOrEmpty(outletId))
                return BadRequest("Outlet Id is missing.");

            if (string.IsNullOrEmpty(batch))
                return BadRequest("Batch is missing.");

            var folderName = Path.Combine("Resources", "Excel");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            Directory.CreateDirectory(pathToSave);

            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                fileName = Guid.NewGuid().ToString() + fileName;
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var resp = MapStudentCardExcel(fullPath, Convert.ToInt32(outletId), Convert.ToInt32(batch));

                if (resp.Item1)
                {
                    //start importing to DB
                    return Ok(await _service.ImportStudentCardAsync(resp.Item3));
                }
                return Ok(new { IsSuccess = resp.Item1, Message = resp.Item2, Data = resp.Item3 });
            }
            else
            {
                return BadRequest();
            }

        }

        private Tuple<bool, string, List<StudentCardImportDTO>> MapStudentCardExcel(string filePath, int outletId, int batch)
        {
            try
            {
                IWorkbook wb = new XSSFWorkbook(new FileStream(filePath, FileMode.Open));
                var rows = new List<StudentCardImportDTO>();
                int startRow = 5;
                for (int x = 0; x < wb.NumberOfSheets; x++)
                {
                    var sheet = (XSSFSheet)wb.GetSheetAt(x);
                    int reqNumCells = startRow - 1;
                    int colCount = sheet.GetRow(reqNumCells).PhysicalNumberOfCells;

                    if (colCount < 7) //number of columns required
                    {
                        string errMsg = string.Format("There is a mismatch on the number of columns required. Please check the file. Sheet: {0}", x + 1);
                        return new Tuple<bool, string, List<StudentCardImportDTO>>(false, errMsg, null);
                    }
                    else
                    {
                        int rowCount = sheet.PhysicalNumberOfRows;
                        for (int i = startRow; ExcelUtility.GetRowWithNonEmptyCell(sheet, i) != null; i++)
                        {
                            var dto = new StudentCardImportDTO { OutletId = outletId, Batch = batch };
                            var fRow = sheet.GetRow(i);

                            if (fRow != null)
                            {
                                int c = 0;
                                var fname = fRow.GetCell(c++);
                                dto.Name = fname != null ? fname.ToString().Trim() : "";

                                var sClass = fRow.GetCell(c++);
                                dto.Class = sClass != null ? sClass.ToString().Trim() : "";

                                var issueDate = fRow.GetCell(c++);
                                string format = "dd/MM/yyyy";
                                dto.IssueDate = issueDate != null && !string.IsNullOrEmpty(issueDate.ToString()) ? DateTime.ParseExact(issueDate.ToString(), format, CultureInfo.InvariantCulture) : (DateTime?)null;

                                DataFormatter formatter = new DataFormatter();
                                var cardIdCell = fRow.GetCell(c++);
                                string cardId = formatter.FormatCellValue(cardIdCell);
                                dto.CardId = cardId != null ? cardId.ToString().Trim() : "";

                                var cardNumberCell = fRow.GetCell(c++);
                                string cardNumber = formatter.FormatCellValue(cardNumberCell);
                                dto.CardNumber = cardNumber != null ? cardNumber.ToString().Trim() : "";

                                var emailCell = fRow.GetCell(c++);
                                string email = formatter.FormatCellValue(emailCell);
                                dto.Email = email != null ? email.ToString().Trim() : "";

                                var associatedEmailCell = fRow.GetCell(c++);
                                string associatedEmail = formatter.FormatCellValue(associatedEmailCell);
                                dto.AssociatedEmail = associatedEmail != null ? associatedEmail.ToString().Trim() : "";

                                rows.Add(dto);
                            }

                        }
                    }
                }

                return new Tuple<bool, string, List<StudentCardImportDTO>>(true, "File format is correct", rows);
            }
            catch (Exception e)
            {
                return new Tuple<bool, string, List<StudentCardImportDTO>>(false, e.Message, null);
            }
        }

        #endregion


        #region Interest Group

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("interestgroups/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllInterestGroupsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetInterestGroups(BaseFilter filter)
        {
            var results = await this._service.GetInterestGroupsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<InterestGroupDTO>>(results));
        }

        #endregion

        [HttpPost("interestgroups")]
        //[Authorize(Authorization.Policies.ManageAllInterestGroupsPolicy)]
        [ProducesResponseType(201, Type = typeof(InterestGroupDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateInterestGroup([FromBody] InterestGroupDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateInterestGroupAsync(dto);
                if (result.IsSuccess)
                {
                    InterestGroupDTO vm = _mapper.Map<InterestGroupDTO>(result.Data);
                    return CreatedAtAction("GetInterestGroupById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("interestgroups/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllInterestGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(InterestGroupDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteInterestGroup(int id)
        {
            var dto = await this._service.GetInterestGroupByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteInterestGroupAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("interestgroups/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllInterestGroupsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateInterestGroup(string id, [FromBody] InterestGroupDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetInterestGroupByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateInterestGroupAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpGet("interestgroups/sendemail/{id}/{templateId}")]
        //[Authorize(Authorization.Policies.ManageAllInterestGroupsPolicy)]
        [ProducesResponseType(200, Type = typeof(InterestGroupDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SendEmailToInterestGroup(int templateId, int id)
        {
            var dto = await this._service.GetInterestGroupByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.SendEmailToInterestGroup(templateId, id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while sending emails: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        #endregion

        #region Student Group

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("studentgroups/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStudentGroups(BaseFilter filter)
        {
            var results = await this._service.GetStudentGroupsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<StudentGroupDTO>>(results));
        }

        [ApiKeyAuthorize]
        [HttpGet("studentgroups/simple/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetSimpleStudentGroups(BaseFilter filter)
        {
            var results = await this._service.GetSimpleStudentGroups(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<StudentGroupSimpleDTO>>(results));
        }

        #endregion

        [HttpGet("studentgroups/generatecode")]
        //[Authorize(Authorization.Policies.ManageAllDishesPolicy)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        //[AllowAnonymous]
        public async Task<IActionResult> GenerateCode(int catererId)
        {
            var code = await this._service.GenerateCode(catererId);
            return Ok(new { code });
        }

        [HttpGet("studentgroups/get/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(StudentGroupDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStudentGroup(int id)
        {
            var dto = await this._service.GetStudentGroupByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            return Ok(dto);
        }

        [HttpPost("studentgroups")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(201, Type = typeof(StudentGroupDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateStudentGroup([FromBody] StudentGroupDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateStudentGroupAsync(dto);
                if (result.IsSuccess)
                {
                    StudentGroupDTO vm = _mapper.Map<StudentGroupDTO>(result.Data);
                    return CreatedAtAction("GetStudentGroupById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("studentgroups/delete/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(200, Type = typeof(StudentGroupDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStudentGroup(int id)
        {
            var dto = await this._service.GetStudentGroupByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteStudentGroupAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("studentgroups/update/{id}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllStudentsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStudentGroup(string id, [FromBody] StudentGroupDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetStudentGroupByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateStudentGroupAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Student Vouchers
        [HttpGet("vouchers/add")]
        //[Authorize(Authorization.Policies.ViewAllVoucherPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddVoucher(int studentId, string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Invalid code.");

            return Ok(await this._service.ActivateStudentVoucher(studentId, code));
        }

        [HttpGet("vouchers/get")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<StudentVoucherDTO>))]
        public async Task<IActionResult> GetStudentVouchersAsync(int studentId)
        {
            var vouchers = await _service.GetStudentVouchersAsync(studentId);
            return Ok(vouchers);
        }

        [HttpGet("vouchers/fetch")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<VoucherDTO>))]
        public async Task<IActionResult> GetVouchersAsync(int studentId)
        {
            var vouchers = await _service.GetVouchersAsync(studentId);
            return Ok(vouchers);
        }

        [HttpGet("vouchers/addbystudentgroup/{studentGroupId:int}/{code}")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddVoucherByStudentGroup([FromRoute] int studentGroupId, [FromRoute] string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return BadRequest("Invalid code.");

                return Ok(await _service.AssignVoucherByStudentGroup(studentGroupId, code));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error AddVoucherByStudentGroup Id : {studentGroupId}");
                _logger.LogError($"Error AddVoucherByStudentGroup : {ex.Message}", ex);
                _logger.LogError($"Error AddVoucherByStudentGroup : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }
        #endregion

        #region Outlet Terms

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("outlets/terms/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOutletTerms(BaseFilter filter)
        {
            var results = await this._service.GetOutletTermsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<OutletTermDTO>>(results));
        }

        #endregion

        #endregion

        #region Wallet

        #region Sieved
        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("wallet/transactions/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<StudentWalletTransactionDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllWalletTransactions(BaseFilter filter)
        {
            var results = await this._walletService.GetWalletTransactionsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<StudentWalletTransactionDTO>>(results));
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("wallet/simple-transactions/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<StudentWalletTransactionSimpleDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetWalletTransactionsSimpleAsync(BaseFilter filter)
        {
            var results = await this._walletService.GetWalletTransactionsSimpleAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<StudentWalletTransactionSimpleDTO>>(results));
        }
        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("wallet/transactions/{studentId}")]
        [ProducesResponseType(200, Type = typeof(List<StudentWalletTransactionDTO>))]
        [ProducesResponseType(403)]
        //[AllowAnonymous]
        public async Task<IActionResult> GetWalletTransactions(int studentId)
        {
            var result = await this._walletService.GetWalletTransactionByIdAsync(studentId);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpPut("wallet/transactions/update/{id}")]
        [ProducesResponseType(200, Type = typeof(StudentWalletTransactionDTO))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> StudentWalletTransaction(string id, [FromBody] StudentWalletTransactionDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");

                var result = await this._walletService.UpdateStudentWalletTransaction(dto);
                if (result.IsSuccess)
                    return NoContent();
                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[ApiKeyAuthorize]
        //[HttpPut("wallet/topup/{id}")]
        ////[Authorize(Authorization.Policies.ManageAllAssetsPolicy)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> TopUpWallet(string id, [FromBody] WalletTopUpDTO model)
        //{
        //    string dataJSON = JsonConvert.SerializeObject(model);
        //    _logger.LogInformation($"TopUpWallet WalletTopUpDTO : {dataJSON}");
        //    _logger.LogInformation($"TopUpWallet Id : {id}");

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (model == null)
        //                return BadRequest($"{nameof(model)} cannot be null");

        //            if (model.WalletId == 0)
        //                return BadRequest("Conflicting type id in parameter and model data");


        //            var dto = await this._walletService.GetByIdAsync(model.WalletId);

        //            if (dto == null)
        //                return NotFound(id);

        //            var result = await this._walletService.TopUpAsync(model);
        //            return Ok(result);

        //        }

        //        return BadRequest(ModelState);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error TopUpWallet : {ex.Message}", ex);
        //        _logger.LogError($"Error TopUpWallet : {ex.StackTrace}", ex);
        //        return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
        //    }
        //}

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpPut("wallet/operation/{studentId}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllAssetsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> WalletTransaction(string studentId, [FromBody] StudentWalletTransactionDTO model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"WalletTransaction WalletOperationDTO : {dataJSON}");
            _logger.LogInformation($"WalletTransaction studentId : {studentId}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.StudentId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");


                    var dto = await this._service.GetStudentByIdAsync(model.StudentId);

                    if (dto == null)
                        return NotFound(studentId);

                    var result = await this._walletService.StudentWalletTransactionAsync(model);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error WalletTransaction : {ex.Message}", ex);
                _logger.LogError($"Error WalletTransaction : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiKeyAuthorize]
        [HttpPost("wallet/studentgroup")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> WalletTopupForStudentGroup([FromBody] StudentGroupWalletRequestViewModel model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"WalletTopupForStudentGroup StudentGroupWalletRequestViewModel : {dataJSON}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.StudentGroupId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");

                    var result = await this._walletService.TopupWalletBalanceByStudentGroupIdAsync(model.StudentGroupId, model.Amount, model.UserId, model.Type);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error WalletTopupForStudentGroup : {ex.Message}", ex);
                _logger.LogError($"Error WalletTopupForStudentGroup : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiKeyAuthorize]
        [HttpPost("wallet/student")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> WalletTopupForStudent([FromBody] StudentGroupWalletRequestViewModel model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"WalletTopupForStudent StudentGroupWalletRequestViewModel : {dataJSON}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.StudentGroupId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");

                    var result = await this._walletService.TopupWalletBalanceByStudentIdAsync(model.StudentGroupId, model.Amount, model.UserId, model.Type, null);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error WalletTopupForStudent : {ex.Message}", ex);
                _logger.LogError($"Error WalletTopupForStudent : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiKeyAuthorize]
        [HttpPost("wallet/student-off-boarding")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> OffBoardingStudent([FromBody] StudentGroupWalletRequestViewModel model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"OffBoardingStudent StudentGroupWalletRequestViewModel : {dataJSON}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.StudentGroupId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");
                    var result = await this._walletService.OffBoardingStudent(model.StudentGroupId, model.UserId);
                    if (result.IsSuccess)
                    {
                        await _service.DeleteStudentAsync(model.StudentGroupId);
                    }
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error OffBoardingStudent : {ex.Message}", ex);
                _logger.LogError($"Error OffBoardingStudent : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiKeyAuthorize]
        [HttpPut("wallet/student/dailylimit")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDailyLimit([FromBody] UpdateDailyLimitViewModel model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"UpdateDailyLimit UpdateDailyLimitViewModel : {dataJSON}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.StudentId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");

                    var result = await this._service.UpdateDailyLimit(model.StudentId, model.DailyLimit);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error UpdateDailyLimit : {ex.Message}", ex);
                _logger.LogError($"Error UpdateDailyLimit : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiKeyAuthorize]
        [HttpPut("wallet/student/walletfreeze")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateWalletFreze([FromBody] UpdateIsWalletFreezeViewModel model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"UpdateWalletFreze UpdateIsWalletFreezeViewModel : {dataJSON}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.StudentId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");

                    var result = await this._service.UpdateWalletFreze(model.StudentId, model.IsWalletFreeze);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error UpdateWalletFreze : {ex.Message}", ex);
                _logger.LogError($"Error UpdateWalletFreze : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [HttpPost("wallet/exportwallettransaction")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateWalletTransactionXls(BaseFilter filter)
        {
            var xls = await this._walletService.GenerateXls(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_AuthLogs.xlsx";

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

        [ApiKeyAuthorize]
        [HttpPost("wallet/transfer")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TransferWallet([FromBody] StudentWalletTransfer model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"TransferWallet Model : {dataJSON}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    var result = await this._walletService.WalletTransfer(model.StudentIdFrom, model.StudentIdTo, model.Amount, model.UserId);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error TransferWallet : {ex.Message}", ex);
                _logger.LogError($"Error TransferWallet : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }
        #endregion

        #region point

        #region Sieved
        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("point/transactions/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<StudentPointTransactionDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllPointTransactions(BaseFilter filter)
        {
            var results = await this._pointService.GetPointTransactionsAsync(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<StudentPointTransactionDTO>>(results));
        }
        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpGet("point/transactions/{studentId}")]
        [ProducesResponseType(200, Type = typeof(List<StudentPointTransactionDTO>))]
        [ProducesResponseType(403)]
        //[AllowAnonymous]
        public async Task<IActionResult> GetPointTransactions(int studentId)
        {
            var result = await this._pointService.GetPointTransactionByIdAsync(studentId);
            return Ok(result);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[ApiKeyAuthorize]
        //[HttpPut("wallet/topup/{id}")]
        ////[Authorize(Authorization.Policies.ManageAllAssetsPolicy)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> TopUpWallet(string id, [FromBody] WalletTopUpDTO model)
        //{
        //    string dataJSON = JsonConvert.SerializeObject(model);
        //    _logger.LogInformation($"TopUpWallet WalletTopUpDTO : {dataJSON}");
        //    _logger.LogInformation($"TopUpWallet Id : {id}");

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (model == null)
        //                return BadRequest($"{nameof(model)} cannot be null");

        //            if (model.WalletId == 0)
        //                return BadRequest("Conflicting type id in parameter and model data");


        //            var dto = await this._walletService.GetByIdAsync(model.WalletId);

        //            if (dto == null)
        //                return NotFound(id);

        //            var result = await this._walletService.TopUpAsync(model);
        //            return Ok(result);

        //        }

        //        return BadRequest(ModelState);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error TopUpWallet : {ex.Message}", ex);
        //        _logger.LogError($"Error TopUpWallet : {ex.StackTrace}", ex);
        //        return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
        //    }
        //}

        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthorize]
        [HttpPut("point/operation/{studentId}")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllAssetsPolicy)]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PointTransaction(string studentId, [FromBody] StudentPointTransactionDTO model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"PointTransaction WalletOperationDTO : {dataJSON}");
            _logger.LogInformation($"PointTransaction studentId : {studentId}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.StudentId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");


                    var dto = await this._service.GetStudentByIdAsync(model.StudentId);

                    if (dto == null)
                        return NotFound(studentId);

                    var result = await this._pointService.StudentPointTransactionAsync(model);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error PointTransaction : {ex.Message}", ex);
                _logger.LogError($"Error PointTransaction : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiKeyAuthorize]
        [HttpPost("point/studentgroup")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PointTopupForStudentGroup([FromBody] StudentGroupPointRequestViewModel model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"PointTopupForStudentGroup StudentGroupPointRequestViewModel : {dataJSON}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.StudentGroupId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");

                    var result = await this._pointService.TopupPointBalanceByStudentGroupIdAsync(model.StudentGroupId, model.Amount, model.UserId, model.Type);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error PointTopupForStudentGroup : {ex.Message}", ex);
                _logger.LogError($"Error PointTopupForStudentGroup : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiKeyAuthorize]
        [HttpPost("point/student")]
        [ProducesResponseType(200, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PointTopupForStudent([FromBody] StudentGroupPointRequestViewModel model)
        {
            string dataJSON = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"PointTopupForStudent StudentGroupPointRequestViewModel : {dataJSON}");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return BadRequest($"{nameof(model)} cannot be null");

                    if (model.StudentGroupId == 0)
                        return BadRequest("Conflicting type id in parameter and model data");

                    var result = await this._pointService.TopupPointBalanceByStudentIdAsync(model.StudentGroupId, model.Amount, model.UserId, model.Type);
                    return Ok(result);

                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error PointTopupForStudent : {ex.Message}", ex);
                _logger.LogError($"Error PointTopupForStudent : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }
        #endregion

        #region API Gourmetz
        [HttpPost("students/v2")]
        [ProducesResponseType(201, Type = typeof(BaseOperationResponse))]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<IActionResult> CreateStudentV2([FromBody] CreateStudentLiteRequestDto dto)
        {
            try
            {
                string dataJson = JsonConvert.SerializeObject(dto);
                _logger.LogInformation($"CreateStudentV2 CreateStudentLiteRequestDto : {dataJson}");
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                StudentDTO vm = _mapper.Map<CreateStudentLiteRequestDto, StudentDTO>(dto);
                vm.StudentCards = new List<StudentCardDTO>();
                if (!string.IsNullOrEmpty(dto.CardId))
                    vm.StudentCards.Add(new StudentCardDTO { CardId = dto?.CardId, IssueDate = dto?.CardIssueDate, Status = "ACTIVE" });

                if (dto.CurrentUserId != null)
                {
                    vm.Users = new List<StudentManageAccountDTO>();
                    vm.Users.Add(new StudentManageAccountDTO()
                    {
                        UserId = dto.CurrentUserId
                    });
                }

                await SaveStudentPicture(vm);

                var response = await _service.CreateStudentAsync(vm);
                var studentData = (Student)response.Data;
                var responseObject = new BaseOperationResponse
                {
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Data = studentData?.Id ?? 0
                };

                if (!response.IsSuccess)
                {
                    _logger.LogInformation($"Error CreateStudentV2 Service : {response.Message}");
                    return BadRequest(responseObject);
                }

                return Ok(responseObject);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error CreateStudentV2 : {ex.Message}", ex);
                _logger.LogError($"Error CreateStudentV2 : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("sendemailconfirm")]
        [AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(201, Type = typeof(UserViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> SendEmailConfirm([FromBody] EmailConfirmDTO ec)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    if (ec == null)
                        return BadRequest($"{nameof(ec)} cannot be null");

                    EmailConfirm ecModel = _mapper.Map<EmailConfirm>(ec);

                    ecModel.Date = DateTime.Now;

                    Random generator = new Random();
                    String r = generator.Next(0, 1000000).ToString("D6");

                    ec.ConfirmationCode = r;
                    ecModel.ConfirmationCode = r;

                    var result = await _service.CreateEmailConfirm(ecModel);
                    if (result.IsSuccess)
                    {
                        var isSuccess = await _emailSender.SendEmailAsync("Gourmetz meal system", "smv_noreply@realtimesys.my.id", ec.Email, ec.Email, "Confirmation Code", $"Confirmation Code is {ec.ConfirmationCode}, \n\nDo not give the code to anyone, including system admin.");

                        _logger.LogInformation($"Email Successfully sent");
                        return CreatedAtAction("Email Confirmationn", new { id = ecModel.Id }, ecModel);
                    }

                }
                _logger.LogInformation($"Error Model State");
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Confirm Email : {ex.Message}", ex);
                _logger.LogError($"Error Confirm email : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("emailConfirm/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ConfirmUserToken(int id, [FromBody] EmailConfirmDTO ec)
        {
            try
            {
                EmailConfirm ecModel = await _service.GetEmailConfirm(id);

                if (ModelState.IsValid)
                {
                    if (ec == null)
                        return BadRequest($"{nameof(ec)} cannot be null");

                    if (ecModel == null)
                        return NotFound(id);

                    bool isValid = true;

                    if (ecModel.ConfirmationCode != ec.ConfirmationCode)
                    {
                        isValid = false;
                        AddErrors(new string[] { "The confirmation code is invalid." });
                    }

                    if (isValid)
                    {
                        return NoContent();
                    }
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error UpdateUser : {ex.Message}", ex);
                _logger.LogError($"Error UpdateUser : {ex.StackTrace}", ex);
                return BadRequest(new { Error = "Error", ErrorDescription = ex.GetBaseException().Message });
            }
        }
        #endregion

        [HttpPost("students/generate-wallet-transaction")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateWalletTransaction(string studentId)
        {
            var xls = await _service.GenerateWalletTransaction(studentId);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + Constants.Student_Wallet_Transactions + ".xlsx";

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

        [HttpPost("students/generate-wallet-transactions")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateWalletTransactions(WalletTransactionFilter filter)
        {
            var xls = await _service.GenerateWalletTransactions(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + Constants.Student_Wallet_Transactions + ".xlsx";

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

        [HttpGet("students/get-wallet-transactions")]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<WalletTransactionReportRow>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetWalletTransactions(WalletTransactionFilter filter)
        {
            var datas = await _service.GetWalletTransactions(filter);
            return Ok(_mapper.Map<PagedEntityViewModel<WalletTransactionReportRow>>(datas));
        }

        [HttpPost("wallet/exportwallettransactionbystudent")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateWalletTransactionByStudent(BaseFilter filter)
        {
            var xls = await this._walletService.GenerateWalletTransactionByStudent(filter);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_AuthLogsByStudent.xlsx";

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

        [HttpGet("wallet/pos-details")]
        public async Task<IActionResult> GetPosDetails(string invoiceId)
        {
            try
            {
                var httpClient = new HttpClient();
                string encodedId = System.Web.HttpUtility.UrlEncode(invoiceId);

                var posUrl = _configuration["AppSettings:POS_URL"];
                if (string.IsNullOrEmpty(posUrl))
                    posUrl = "http://byod.southeastasia.cloudapp.azure.com:8082";

                string url = $"{posUrl}/POS/anon_api/ajaxGetSalesByInvoiceId?invoice_id={encodedId?.Trim()}";

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }

                return BadRequest("Failed to fetch data from POS system");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}