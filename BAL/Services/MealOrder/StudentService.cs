using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL;
using BAL.Services.Interfaces;
using AutoMapper;
using BAL.Services.Interfaces.MealOrder;
using BAL.DTO.MealOrder;
using DAL.Models.MealOrder;
using DAL.Core.Interfaces;
using DAL.Core.DTO;
using System.ComponentModel.DataAnnotations;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

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

        public async Task<StudentDTO> GetStudentByEmailOrIdAsync(string email,int id)
        {
            if(string.IsNullOrEmpty(email))
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
                    var row = sheet.CreateRow(rowCount);var borderedHeaderStyle = wb.CreateCellStyle();
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
                    cell.SetCellValue("SATS Food Services Pte Ltd");
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

        public async Task<byte[]> GenerateStudentListXls(int outletId,int studentGroupId)
        {
            var studentGroup = await _uow.StudentGroups.GetAllStudentGroupDetailByIdAsync(studentGroupId);
            var studentIdLinked = studentGroup.Select(x => x.StudentId).Distinct().ToList();
            var students = await _uow.Students.GetStudentsLiteByIdsAsync(studentIdLinked);

            using (var stream = new System.IO.MemoryStream())
            {
                var wb = new XSSFWorkbook();
                var rowCount = 0;
                var sheet = (XSSFSheet)wb.CreateSheet("Students");
                var headers = new string[] { "Student ID","Name"};

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

        public async Task<bool> ImportStudentGroupAsync(List<int> studentids,int studentGroupId,int userId)
        {
            var result = await _uow.Students.ImportStudentGroupAsync(studentids, studentGroupId,userId);
            return result;
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
            return _mapper.Map< List<StudentVoucherDTO>>(await this._uow.StudentCards.GetStudentVouchersAsync(studentId));
        }

        public async Task<List<VoucherDTO>> GetVouchersAsync(int studentId)
        {
            var svs = await this._uow.StudentCards.GetStudentVouchersAsync(studentId);
            var voucherCode = svs.Select(x => x.Voucher.Code).ToList();
            var countData = await _uow.StudentCards.GetVoucherUsedCountAsync(studentId, voucherCode);

            var result = svs
            .Select(sv =>
            {
                var dto = _mapper.Map<VoucherDTO>(sv.Voucher);
                dto.UsedCount = countData.TryGetValue(sv.Voucher.Code, out var count) ? count : 0;
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
    }
}
