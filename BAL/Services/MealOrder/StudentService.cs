using AutoMapper;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using DAL;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Helpers;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using Microsoft.AspNetCore.Identity;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;
using ICell = NPOI.SS.UserModel.ICell;

namespace BAL.Services.MealOrder
{
    public class StudentService : IStudentService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private IAccountManager _accountManager;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;


        public StudentService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager, IEmailSender emailSender, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
            _emailSender = emailSender;
            _mapper = mapper;
        }

        #region Student

        public async Task<PagedEntity<StudentDTO>> GetStudentsAsync(BaseFilter filter, bool noAccount = false)
        {
            var result = _mapper.Map<PagedEntity<StudentDTO>>(await this._uow.Students.GetStudentsAsync(filter, noAccount));
            return result;
        }

        public async Task<PagedEntity<StudentSimpleDTO>> GetSimpleStudentsAsync(BaseFilter filter, bool noAccount = false)
        {
            var result = _mapper.Map<PagedEntity<StudentSimpleDTO>>(await this._uow.Students.GetStudentsAsync(filter, noAccount));
            return result;
        }

        public async Task<List<StudentDTO>> GetStudentsByUserAsync(int userId)
        {
            var result = _mapper.Map<List<StudentDTO>>(await this._uow.Students.GetStudentsByUserAsync(userId));
            return result;
        }

        public async Task<BaseOperationResponse> AddStudentLinksByUserAsync(int userId, int studentId)
        {
            var result = await this._uow.Students.AddStudentLinksByUserAsync(userId, studentId);
            return result;
        }

        public async Task<BaseOperationResponse> AddStudentAccountLinkRequestAsync(int studentId, string email, bool emailSent)
        {
            var result = await this._uow.Students.AddStudentAccountLinkRequestAsync(studentId, email, emailSent);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStudentAccountLinkRequestAsync(int studentId, string email)
        {
            var result = await this._uow.Students.UpdateStudentAccountLinkRequestAsync(studentId, email);
            return result;
        }

        public async Task<List<StudentDTO>> GetStudentsWithNoOrder(DateTime from, DateTime to)
        {
            var result = _mapper.Map<List<StudentDTO>>(await this._uow.Students.GetStudentsWithNoOrder(from, to));
            return result;
        }

        public async Task<List<StudentDTO>> GetStudentsWithAbandonedCart1(int hoursLeft)
        {
            var result = _mapper.Map<List<StudentDTO>>(await this._uow.Students.GetStudentsWithAbandonedCart1(hoursLeft));
            return result;
        }

        public async Task<List<StudentDTO>> GetStudentsWithAbandonedCart2(int daysBeforeCutOff)
        {
            var result = _mapper.Map<List<StudentDTO>>(await this._uow.Students.GetStudentsWithAbandonedCart2(daysBeforeCutOff));
            return result;
        }

        public async Task<List<StudentOrderDTO>> GetStudentsWithOrdersNotCollected(int daysPassed)
        {
            var result = _mapper.Map<List<StudentOrderDTO>>(await this._uow.Students.GetStudentsWithOrdersNotCollected(daysPassed));
            return result;
        }

        public async Task<StudentDTO> GetStudentByIdAsync(int id)
        {
            return _mapper.Map<StudentDTO>(await this._uow.Students.GetByIdAsync(id));
        }

        public async Task<StudentDTO> GetStudentByEmailAsync(string email)
        {
            return _mapper.Map<StudentDTO>(await this._uow.Students.GetStudentByEmailAsync(email));
        }

        public async Task<StudentDTO> GetStudentByEmailOrIdAsync(string email, int id)
        {
            if (string.IsNullOrEmpty(email))
                return await GetStudentByIdAsync(id);
            else
                return await GetStudentByEmailAsync(email);
        }

        public async Task<BaseOperationResponse> UpdateStudentEmail(int id, string email)
        {
            return await this._uow.Students.UpdateStudentEmail(id, email);
        }

        public async Task<BaseOperationResponse> UpdateStudentClassByClassId(StudentDTO dto, int originClassId)
        {
            var data = _mapper.Map<Student>(dto);
            return await this._uow.Students.UpdateStudentClassByClassIdAsync(data, originClassId);
        }

        public async Task<BaseOperationResponse> CreateStudentAsync(StudentDTO dto)
        {
            var result = new BaseOperationResponse();
            var student = _mapper.Map<Student>(dto);
            var user = _mapper.Map<ApplicationUser>(dto);
            var cards = _mapper.Map<List<UserCardId>>(dto.Cards);
            var studentCards = _mapper.Map<List<StudentCard>>(dto.StudentCards);
            result = await this._uow.Students.CreateAsync(this._accountManager, student, user, dto.NewPassword, cards, studentCards);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStudentAsync(StudentDTO dto)
        {
            var result = new BaseOperationResponse();
            var student = _mapper.Map<Student>(dto);
            var user = _mapper.Map<ApplicationUser>(dto);
            var cards = _mapper.Map<List<UserCardId>>(dto.Cards);
            var studentCards = _mapper.Map<List<StudentCard>>(dto.StudentCards);
            var studentRestrictions = _mapper.Map<List<StudentRestriction>>(dto.Restrictions);
            var studentInterestGroups = _mapper.Map<List<StudentInterestGroup>>(dto.InterestGroups);
            result = await this._uow.Students.UpdateAsync(this._accountManager, student, user, dto.CurrentPassword, dto.NewPassword, cards, studentCards, studentRestrictions, studentInterestGroups);
            return result;
        }

        public async Task<BaseOperationResponse> CreateAccountAsync(List<int> ids, bool generateRandomPassword, string defaultPassword)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Students.CreateAccountAsync(ids, generateRandomPassword, _accountManager, defaultPassword);
            return result;
        }

        public async Task<BaseOperationResponse> DeleteStudentAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Students.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> ImportStudentAsync(List<StudentImportDTO> dto)
        {
            var result = await this._uow.Students.ImportStudentAsync(_accountManager, dto);
            return result;
        }

        public async Task<byte[]> GenerateStudentReportXls(BaseFilter filter)
        {
            var students = await _uow.Students.GetStudentsAsync(filter);

            var users = students.PagedData.ToList();

            if (users != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Students");
                    var headers = new string[] { "Name", "Batch", "Class Level", "Class", "Gender", "Is FAS", "Email", "Last Login Date/Time", "Vouchers", "Cards" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount); var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    ICell cell;
                    for (var i = 0; i < headers.Length; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();
                    users.ForEach(dt =>
                    {
                        int i = 0;
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.Name);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.ClassBatch?.Name);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.ClassLevel?.Name);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.Class?.Name);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.Gender);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.IsFAS ? "Yes" : "No");
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.Email);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.Account?.User?.LastLoginTime?.ToString("dd/MM/yyyy hh:mm:ss tt"));
                        cell.CellStyle = contentStyle;

                        string vouchers = string.Empty;
                        if (dt.Vouchers != null)
                        {
                            foreach (var voucher in dt.Vouchers.Where(e => e.IsActive))
                            {
                                vouchers += string.Format("-{0}\n", voucher.Voucher?.Code);
                            }
                        }

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(vouchers);
                        cell.CellStyle = contentStyle;

                        string cards = string.Empty;
                        if (dt.StudentCards != null)
                        {
                            foreach (var card in dt.StudentCards.Where(e => e.IsActive))
                            {
                                cards += string.Format("-{0}{1}\n", card.CardId, card.Status?.ToLower() == "active" ? "(Active)" : string.Empty);
                            }
                        }

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(cards);
                        cell.CellStyle = contentStyle;
                    });

                    #endregion

                    for (var i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<byte[]> GenerateStudentReportForImportXls(BaseFilter filter)
        {
            var students = await _uow.Students.GetStudentsAsync(filter);

            var users = students.PagedData.ToList();

            if (users != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Students");
                    var headers = new string[] { "NAME", "CLASS", "ISSUE DATE", "CARD ID", "CARD NUMBER", "EMAIL", "ASSOCIATED EMAIL" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerFont.FontName = "Verdana";
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount);

                    ICell cell;
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Gourmetz Pte Ltd");
                    cell.CellStyle = headerStyle;

                    rowCount += 3;
                    row = sheet.CreateRow(rowCount);

                    cell = row.CreateCell(0);
                    cell.SetCellValue("STUDENTS");
                    cell.CellStyle = headerStyle;

                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    row = sheet.CreateRow(++rowCount);
                    for (var i = 0; i < headers.Length; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();
                    users.ForEach(dt =>
                    {
                        int i = 0;
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.Name);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.Class?.Name);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        var card = dt.StudentCards.OrderBy(e => e.UpdatedDate).FirstOrDefault(e => e.IsActive && e.Status?.ToLower() == "active");
                        var issueDate = card != null ? card.CreatedDate.ToString("dd/MM/yyyy") : string.Empty;
                        cell.SetCellValue(issueDate);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(card != null ? card.CardId : string.Empty);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(string.Empty);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.Email);
                        cell.CellStyle = contentStyle;

                        var associatedUsers = dt.Users.Where(e => e.IsActive);
                        cell = row.CreateCell(i++);
                        cell.SetCellValue(associatedUsers != null ? string.Join(",", associatedUsers.Select(e => e.User.Email).Distinct()) : string.Empty);
                        cell.CellStyle = contentStyle;
                    });

                    #endregion

                    for (var i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<byte[]> GenerateStudentListXls(int outletId, int studentGroupId)
        {
            var studentGroup = await _uow.StudentGroups.GetAllStudentGroupDetailByIdAsync(studentGroupId);
            var studentIdLinked = studentGroup.Select(x => x.StudentId).Distinct().ToList();
            var students = await _uow.Students.GetStudentsLiteByIdsAsync(studentIdLinked);

            using (var stream = new System.IO.MemoryStream())
            {
                var wb = new XSSFWorkbook();
                var rowCount = 0;
                var sheet = (XSSFSheet)wb.CreateSheet("Students");
                var headers = new string[] { "Student ID", "Name" };

                #region Headers

                var headerStyle = wb.CreateCellStyle();
                var headerFont = wb.CreateFont();
                headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                headerStyle.SetFont(headerFont);
                headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                var row = sheet.CreateRow(rowCount); var borderedHeaderStyle = wb.CreateCellStyle();
                borderedHeaderStyle.SetFont(headerFont);
                borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                ICell cell;
                for (var i = 0; i < headers.Length; i++)
                {
                    cell = row.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = borderedHeaderStyle;
                }
                sheet.AutoSizeColumn(0);

                #endregion

                #region Content
                var contentStyle = wb.CreateCellStyle();
                contentStyle.BorderTop = BorderStyle.Thin;
                contentStyle.BorderBottom = BorderStyle.Thin;
                contentStyle.BorderLeft = BorderStyle.Thin;
                contentStyle.BorderRight = BorderStyle.Thin;
                contentStyle.VerticalAlignment = VerticalAlignment.Top;
                contentStyle.Alignment = HorizontalAlignment.Left;
                contentStyle.WrapText = true;
                var dataFormatCustom = wb.CreateDataFormat();
                students.ForEach(dt =>
                {
                    int i = 0;
                    row = sheet.CreateRow(++rowCount);

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.Id);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(dt.Name);
                    cell.CellStyle = contentStyle;
                });

                #endregion

                for (var i = 0; i < headers.Length; i++)
                {
                    sheet.AutoSizeColumn(i, true);
                }

                wb.Write(stream);

                return stream.ToArray();
            }
        }

        public async Task<bool> ImportStudentGroupAsync(List<int> studentids, int studentGroupId, int userId)
        {
            var result = await _uow.Students.ImportStudentGroupAsync(studentids, studentGroupId, userId);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateWalletFreze(int studentId, bool isFreeze)
        {
            return await this._uow.Students.UpdateWalletFreze(studentId, isFreeze);
        }

        public async Task<BaseOperationResponse> UpdateDailyLimit(int studentId, int dailyLimit)
        {
            return await this._uow.Students.UpdateDailyLimit(studentId, dailyLimit);
        }

        public async Task<List<StudentLiteDTO>> GetStudentByOutletAsync(int outletId)
        {
            var query = await this._uow.Students.GetAllStudentsAsync();
            return query.Where(m => m.OutletId == outletId && m.IsActive)
                .Select(m => new StudentLiteDTO
                {
                    Id = m == null ? 0 : m.Id,
                    Name = m == null ? "" : m.Name,
                })?.ToList();
        }

        public async Task<List<StudentLiteWithClassDTO>> GetStudentByOutletWithClassAsync(int outletId)
        {
            var query = await this._uow.Students.GetAllStudentsAsync();
            return query.Where(m => m.OutletId == outletId && m.IsActive)
                .Select(m => new StudentLiteWithClassDTO
                {
                    Id = m == null ? 0 : m.Id,
                    Name = m == null ? "" : m.Name,
                    Class = m == null ? "" : (m.Class == null ? "" : m.Class.Name)
                })?.Distinct()?.OrderBy(m => m.Name)?.ToList();
        }
        #endregion

        #region Student Card

        public async Task<BaseOperationResponse> ImportStudentCardAsync(List<StudentCardImportDTO> dto)
        {
            var result = await this._uow.Students.ImportStudentCardAsync(_accountManager, dto);
            return result;
        }

        public async Task<PagedEntity<StudentCardDTO>> GetStudentCardsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StudentCardDTO>>(await this._uow.StudentCards.GetStudentCardsAsync(filter));
            return result;
        }

        public async Task<StudentCardDTO> GetStudentCardByIdAsync(int id, string cardId = null)
        {
            return _mapper.Map<StudentCardDTO>(await this._uow.StudentCards.GetByIdAsync(id, cardId));
        }

        public async Task<StudentCardDTO> GetStudentCardByIdIncludeNonActiveAsync(int id, string cardId = null)
        {
            return _mapper.Map<StudentCardDTO>(await this._uow.StudentCards.GetByIdIncludeNonActiveAsync(id, cardId));
        }

        public async Task<StudentDTO> GetStudentByCardIdAsync(string cardId)
        {
            return _mapper.Map<StudentDTO>(await this._uow.StudentCards.GetStudentByCardIdAsync(cardId));
        }

        public async Task<BaseOperationResponse> ActivateStudentCardByIdAsync(string cardId)
        {
            return await this._uow.StudentCards.ActivateStudentCardByIdAsync(cardId);
        }

        public async Task<BaseOperationResponse> CreateStudentCardAsync(StudentCardDTO dto)
        {
            var result = await this._uow.StudentCards.CreateAsync(_mapper.Map<StudentCard>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStudentCardAsync(StudentCardDTO dto)
        {
            var result = await this._uow.StudentCards.UpdateAsync(_mapper.Map<StudentCard>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteStudentCardAsync(int id)
        {
            return await this._uow.StudentCards.DeleteAsync(id);
        }

        #endregion

        #region Interest Group

        public async Task<PagedEntity<InterestGroupDTO>> GetInterestGroupsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<InterestGroupDTO>>(await this._uow.InterestGroups.GetInterestGroupsAsync(filter));
            return result;
        }

        public async Task<InterestGroupDTO> GetInterestGroupByIdAsync(int id)
        {
            return _mapper.Map<InterestGroupDTO>(await this._uow.InterestGroups.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateInterestGroupAsync(InterestGroupDTO dto)
        {
            return await this._uow.InterestGroups.CreateAsync(_mapper.Map<InterestGroup>(dto));
        }

        public async Task<BaseOperationResponse> UpdateInterestGroupAsync(InterestGroupDTO dto)
        {
            return await this._uow.InterestGroups.UpdateAsync(_mapper.Map<InterestGroup>(dto));
        }

        public async Task<BaseOperationResponse> DeleteInterestGroupAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.InterestGroups.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> SendEmailToInterestGroup(int templateId, int id)
        {
            var result = new BaseOperationResponse();
            try
            {
                var students = await this._uow.Students.GetByInterestGroupIdAsync(id);
                var template = await this._uow.EmailTemplates.GetByIdAsync(templateId);

                if (template != null && students != null)
                {
                    foreach (var student in students)
                    {
                        if (IsValidEmailAddress(student.Account.User.Email))
                        {
                            await _emailSender.SendEmailAsync(string.Empty, string.Empty, student.Name, student.Account.User.Email, template.Subject, template.Body);
                        }
                    }
                }

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return result;
        }

        public bool IsValidEmailAddress(string address)
        {
            return !string.IsNullOrEmpty(address) && new EmailAddressAttribute().IsValid(address);
        }

        #endregion

        #region Student Group
        public async Task<string> GenerateCode(int id)
        {
            return await this._uow.StudentGroups.GenerateCode(id);
        }

        public async Task<PagedEntity<StudentGroupDTO>> GetStudentGroupsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StudentGroupDTO>>(await this._uow.StudentGroups.GetStudentGroupsAsync(filter));
            return result;
        }

        public async Task<PagedEntity<StudentGroupSimpleDTO>> GetSimpleStudentGroups(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StudentGroupSimpleDTO>>(await this._uow.StudentGroups.GetStudentGroupsAsync(filter));
            return result;
        }

        public async Task<List<GroupedTermStudentGroupDTO>> GetStudentMealPlanAsync(int outletId, DateTime? orderDate)
        {
            var mealPlans = _mapper.Map<List<StudentGroupDTO>>(await _uow.StudentGroups.GetAllStudentGroupsAsync(outletId, orderDate));

            return mealPlans.GroupBy(e => e.OutletTermId).Select(e => new GroupedTermStudentGroupDTO
            {
                OutletTermId = e.Key.Value,
                TermName = e.First().TermName,
                DeliveryStartDate = e.First().DeliveryStartDate,
                DeliveryEndDate = e.First().DeliveryEndDate,
                Plans = e.Select(x => new StudentGroupDTO
                {
                    Code = x.Code,
                    DeliveryEndDate = x.DeliveryEndDate,
                    DeliveryStartDate = x.DeliveryStartDate,
                    Description = x.Description,
                    EndDate = x.EndDate,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    Id = x.Id,
                    IsPublished = x.IsPublished,
                    Name = x.Name,
                    OutletId = x.OutletId,
                    OutletTermId = x.OutletTermId,
                    Price = x.Price,
                    Sequence = x.Sequence,
                    StartDate = x.StartDate,
                    TermName = x.TermName,
                    Type = x.Type
                }).Distinct(new StudentGroupIdEqualityComparer()).OrderBy(x => x.Sequence).ToList()
            }).ToList();
        }

        public async Task<StudentGroupDTO> GetStudentGroupByIdAsync(int id)
        {
            return _mapper.Map<StudentGroupDTO>(await this._uow.StudentGroups.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateStudentGroupAsync(StudentGroupDTO dto)
        {
            var result = new BaseOperationResponse();
            var group = _mapper.Map<StudentGroup>(dto);
            var details = _mapper.Map<List<StudentGroupDetail>>(dto.Sgdetails);
            var sessions = _mapper.Map<List<StudentGroupSession>>(dto.Sessions);
            result = await this._uow.StudentGroups.CreateAsync(group, details, sessions);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStudentGroupAsync(StudentGroupDTO dto)
        {
            var result = new BaseOperationResponse();
            var group = _mapper.Map<StudentGroup>(dto);
            var details = _mapper.Map<List<StudentGroupDetail>>(dto.Sgdetails);
            var sessions = _mapper.Map<List<StudentGroupSession>>(dto.Sessions);
            result = await this._uow.StudentGroups.UpdateAsync(group, details, sessions);
            return result;
        }

        public async Task<BaseOperationResponse> DeleteStudentGroupAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.StudentGroups.DeleteAsync(id);
            return result;
        }

        public async Task<bool> CreateOrUpdateStudentGroupDetailAsync(int StudentGroupId, int StudentId, bool IsActive)
        {

            var result = await this._uow.StudentGroups.CreateOrUpdateStudentGroupDetailAsync(StudentGroupId, StudentId, IsActive);
            return result;
        }

        #endregion

        #region Student Vouchers
        public async Task<BaseOperationResponse> ActivateStudentVoucher(int studentId, string code)
        {
            return await this._uow.StudentCards.ActivateStudentVoucher(studentId, code);
        }

        public async Task<List<StudentVoucherDTO>> GetStudentVouchersAsync(int studentId)
        {
            return _mapper.Map<List<StudentVoucherDTO>>(await this._uow.StudentCards.GetStudentVouchersAsync(studentId));
        }

        public async Task<List<VoucherDTO>> GetVouchersAsync(int studentId)
        {
            var svs = await this._uow.StudentCards.GetStudentVouchersAsync(studentId);
            var voucherCode = svs.Select(x => x.Voucher.Code).ToList();
            var countData = await _uow.StudentCards.GetVoucherUsedCountAsync(studentId, voucherCode);
            var countDataByVoucherCode = await _uow.StudentCards.GetVoucherUsedCountAsync(0, voucherCode);
            var result = svs
            .Select(sv =>
            {
                var dto = _mapper.Map<VoucherDTO>(sv.Voucher);
                countDataByVoucherCode.TryGetValue(sv.Voucher.Code, out var countByCode);
                bool isMaxRedeemed = countByCode >= sv.Voucher.MaxRedeemCheckout;
                dto.UsedCount = countData.TryGetValue(sv.Voucher.Code, out var count) ? count : 0;
                dto.IsMaxRedeemed = isMaxRedeemed;
                return dto;
            })
            .Where(dto => dto.UsedCount <= dto.MaxDistribution)
            .ToList();

            return result;
        }

        public async Task<BaseOperationResponse> AssignVoucherByStudentGroup(int studentGroupId, string code)
        {
            return await this._uow.StudentCards.AssignVoucherByStudentGroup(studentGroupId, code);
        }
        #endregion

        #region Outlet Terms

        public async Task<PagedEntity<OutletTermDTO>> GetOutletTermsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<OutletTermDTO>>(await this._uow.StudentGroups.GetOutletTermsAsync(filter));
            return result;
        }

        #endregion


        public async Task<BaseOperationResponse> CreateEmailConfirm(EmailConfirm ec)
        {
            var existingEc = await _uow.EmailConfirms.GetByEmailAsync(ec.Email);

            if (existingEc != null)
            {
                ec.Id = existingEc.Id;
                existingEc.CopyFrom(ec);
                if (existingEc.IsActive)
                {
                    return await _uow.EmailConfirms.UpdateAsync(existingEc);
                }
                else
                {
                    existingEc.IsActive = true;
                    return await _uow.EmailConfirms.UpdateAsync(existingEc);
                }
            }

            if (existingEc == null)
            {
                existingEc = ec;
            }

            return await _uow.EmailConfirms.CreateAsync(ec);
        }

        public async Task<EmailConfirm> GetEmailConfirm(int id)
        {
            return await this._uow.EmailConfirms.GetByIdAsync(id);
        }

        public async Task<byte[]> GenerateWalletTransaction(string studentId)
        {
            if (string.IsNullOrEmpty(studentId)) return null;
            int studentIdInt = int.TryParse(studentId, out var id) ? id : 0;
            var student = await _uow.Students.GetByIdAsync(studentIdInt);
            if (student == null) return null;

            var result = await _uow.StudentWalletTransactions.GetWalletTransactions(new WalletTransactionFilter
            {
                StudentId = studentIdInt,
                isFAS = null
            });

            using (var stream = new System.IO.MemoryStream())
            {
                var wb = new XSSFWorkbook();
                var rowCount = 0;
                var sheet = (XSSFSheet)wb.CreateSheet(Constants.Student_Wallet_Transactions);
                var headers = new string[] { "Student ID",
                                             "Name",
                                             "School",
                                             "Class Level",
                                             "Class",
                                             "Total Basic Account Top Up",
                                             "Total Refund Basic Account",
                                             "Total Basic Redemption",
                                             "Basic Wallet Balance",

                                             "FAS Student",
                                             "Total FAS Top Up Amount",
                                             "Total FAS Refund Amount",
                                             "Total Auto Debit FAS",
                                             "Total FAS Redemption Amount",
                                             "FAS Wallet Balance",
                                             "Wrong Topup FAS Credit"
                };

                #region Headers

                var headerStyle = wb.CreateCellStyle();
                var headerFont = wb.CreateFont();
                headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                headerStyle.SetFont(headerFont);
                headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                var row = sheet.CreateRow(rowCount); var borderedHeaderStyle = wb.CreateCellStyle();
                borderedHeaderStyle.SetFont(headerFont);
                borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                ICell cell;
                for (var index = 0; index < headers.Length; index++)
                {
                    cell = row.CreateCell(index);
                    cell.SetCellValue(headers[index]);
                    cell.CellStyle = borderedHeaderStyle;
                }
                sheet.AutoSizeColumn(0);

                #endregion

                #region Content
                var contentStyle = wb.CreateCellStyle();
                contentStyle.BorderTop = BorderStyle.Thin;
                contentStyle.BorderBottom = BorderStyle.Thin;
                contentStyle.BorderLeft = BorderStyle.Thin;
                contentStyle.BorderRight = BorderStyle.Thin;
                contentStyle.VerticalAlignment = VerticalAlignment.Top;
                contentStyle.Alignment = HorizontalAlignment.Left;
                var dataFormatCustom = wb.CreateDataFormat();

                int i = 0;
                foreach (var creditTransactions in result.PagedData)
                {
                    row = sheet.CreateRow(++rowCount);

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(creditTransactions.StudentId);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(creditTransactions.StudentName);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(creditTransactions.OutletName);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(creditTransactions.ClassLevelName);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(creditTransactions.ClassName);
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(Common.Round(creditTransactions.TotalTopUpBasic));
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(Common.Round(creditTransactions.TotalRefundBasic));
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(Common.Round(creditTransactions.TotalBasicRedemption));
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(Common.Round(creditTransactions.BasicWalletBalance));
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(creditTransactions.IsFAS ? "Y" : "N");
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(Common.Round(creditTransactions.TotalTopUpFAS));
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(Common.Round(creditTransactions.TotalRefundFAS));
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(Common.Round(creditTransactions.TotalAutoDebitFAS));
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(Common.Round(creditTransactions.TotalFASRedemption));
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(Common.Round(creditTransactions.FASWalletBalance));
                    cell.CellStyle = contentStyle;

                    cell = row.CreateCell(i++);
                    cell.SetCellValue(Common.Round(creditTransactions.WrongTopupFASCredit));
                    cell.CellStyle = contentStyle;

                    #endregion
                }
                for (var index = 0; index < headers.Length; index++)
                {
                    sheet.AutoSizeColumn(index, true);
                }

                wb.Write(stream);

                return stream.ToArray();
            }
        }

        public async Task<byte[]> GenerateWalletTransactions(WalletTransactionFilter filter)
        {
            filter.PageSize = null;
            var result = await _uow.StudentWalletTransactions.GetWalletTransactions(filter);
            using (var stream = new System.IO.MemoryStream())
            {
                var wb = new XSSFWorkbook();
                var rowCount = 0;
                var sheet = (XSSFSheet)wb.CreateSheet(Constants.Student_Wallet_Transactions);
                var headers = new string[] { "Student ID",
                                             "Name",
                                             "School",
                                             "Class Level",
                                             "Class",
                                             "Total Basic Account Top Up",
                                             "Total Refund Basic Account",
                                             "Total Basic Redemption",
                                             "Basic Wallet Balance",

                                             "FAS Student",
                                             "Total FAS Top Up Amount",
                                             "Total FAS Refund Amount",
                                             "Total Auto Debit FAS",
                                             "Total FAS Redemption Amount",
                                             "FAS Wallet Balance",
                                             "Wrong Topup FAS Credit"
                };

                int rowIndex = 0;

                var headerStyle = wb.CreateCellStyle();
                var headerFont = wb.CreateFont();
                headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                headerStyle.SetFont(headerFont);

                var row = sheet.CreateRow(rowCount);
                var borderedHeaderStyle = wb.CreateCellStyle();
                borderedHeaderStyle.SetFont(headerFont);
                borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                ICell cell;

                var contentStyle = wb.CreateCellStyle();
                contentStyle.BorderTop = BorderStyle.Thin;
                contentStyle.BorderBottom = BorderStyle.Thin;
                contentStyle.BorderLeft = BorderStyle.Thin;
                contentStyle.BorderRight = BorderStyle.Thin;
                contentStyle.VerticalAlignment = VerticalAlignment.Top;
                contentStyle.Alignment = HorizontalAlignment.Left;

                var donloadTime = sheet.CreateRow(rowIndex++);
                donloadTime.CreateCell(0).SetCellValue($"Downloaded Data as of {DateTime.Now:dd/MM/yyyy hh:mmtt}");
                donloadTime.GetCell(0).CellStyle = headerStyle;

                var startDate = "-";
                var endDate = "-";
                if (filter.startDate != DateTime.MinValue)
                    startDate = filter.startDate.ToString("dd/MM/yyyy");
                if (filter.endDate != DateTime.MinValue)
                    endDate = filter.endDate.ToString("dd/MM/yyyy");

                donloadTime = sheet.CreateRow(rowIndex++);
                donloadTime.CreateCell(0).SetCellValue($"Date Range {startDate} to {endDate}");
                donloadTime.GetCell(0).CellStyle = headerStyle;
                rowIndex++;

                row = sheet.CreateRow(rowIndex++);
                for (var index = 0; index < headers.Length; index++)
                {
                    cell = row.CreateCell(index);
                    cell.SetCellValue(headers[index]);
                    cell.CellStyle = borderedHeaderStyle;
                }
                sheet.AutoSizeColumn(0);

                foreach (var item in result.PagedData
                    .OrderBy(m => m.StudentId)
                    .ThenBy(m => m.OutletName)
                    .ThenBy(m => m.ClassLevelName)
                    .ThenBy(m => m.ClassName))
                {
                    var dataRow = sheet.CreateRow(rowIndex++);

                    cell = dataRow.CreateCell(0);
                    cell.SetCellValue(item.StudentId);
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(1);
                    cell.SetCellValue(item.StudentName);
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(2);
                    cell.SetCellValue(item.OutletName);
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(3);
                    cell.SetCellValue(item.ClassLevelName);
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(4);
                    cell.SetCellValue(item.ClassName);
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(5);
                    cell.SetCellValue(Common.Round(item.TotalTopUpBasic));
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(6);
                    cell.SetCellValue(Common.Round(item.TotalRefundBasic));
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(7);
                    cell.SetCellValue(Common.Round(item.TotalBasicRedemption));
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(8);
                    cell.SetCellValue(Common.Round(item.BasicWalletBalance));
                    cell.CellStyle = contentStyle;

                    //FAS
                    cell = dataRow.CreateCell(9);
                    cell.SetCellValue(item.IsFAS ? "Y" : "N");
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(10);
                    cell.SetCellValue(Common.Round(item.TotalTopUpFAS));
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(11);
                    cell.SetCellValue(Common.Round(item.TotalRefundFAS));
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(12);
                    cell.SetCellValue(Common.Round(item.TotalAutoDebitFAS));
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(13);
                    cell.SetCellValue(Common.Round(item.TotalFASRedemption));
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(14);
                    cell.SetCellValue(Common.Round(item.FASWalletBalance));
                    cell.CellStyle = contentStyle;

                    cell = dataRow.CreateCell(15);
                    cell.SetCellValue(Common.Round(item.WrongTopupFASCredit));
                    cell.CellStyle = contentStyle;
                }

                for (var index = 0; index < headers.Length; index++)
                {
                    sheet.AutoSizeColumn(index, true);
                }


                wb.Write(stream);
                return stream.ToArray();
            }
        }

        public async Task<PagedEntity<WalletTransactionReportRow>> GetWalletTransactions(WalletTransactionFilter filter)
        {
            return await _uow.StudentWalletTransactions.GetWalletTransactions(filter);
        }
    }
}
