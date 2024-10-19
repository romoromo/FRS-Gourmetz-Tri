using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using BAL.Services.MealOrder;
using DAL;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.Helpers;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OpenIddict.Validation.AspNetCore;
using Microsoft.Extensions.Configuration;
using DAL.Core.Helpers;
using System.Globalization;
using NPOI.SS.Formula.Functions;

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
        public StudentController(IStudentService service, ILogger<StudentController> logger, IAccountManager accountManager, IEmailSender emailSender, ApplicationUserManager userManager, IConfiguration configuration, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _accountManager = accountManager;
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
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

        #endregion

        [HttpGet("students/get/user/{userId}")]
        //[Authorize(Authorization.Policies.ViewAllStudentsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<StudentDTO>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetStudentsByUser(int userId)
        {
            var results = await this._service.GetStudentsByUserAsync(userId);
            return Ok(_mapper.Map<List<StudentDTO>>(results));
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

                var result = await this._service.UpdateStudentAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
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

                var title = $"Tappee Login Information.";
                var content = $"Hi {student.Name}.\n" +
                    "Please find below your login information for Tappee.\n" +
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

                var card = await this._service.GetStudentCardByIdAsync(0, dto.CardId);
                if (card != null)
                    return BadRequest("Card already exists.");

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
            var results = await this._service.GetStudentGroupsAsync(filter);
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
    }
}