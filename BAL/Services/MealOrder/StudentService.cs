using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL;
using BAL.Services.Interfaces;
using BAL.DTO;
using AutoMapper;
using DAL.Repositories.Interfaces;
using System.IO;
using NPOI.HSSF.UserModel;
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

        public StudentService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager, IEmailSender emailSender)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
            _emailSender = emailSender;
        }

        #region Student

        public async Task<PagedEntity<StudentDTO>> GetStudentsAsync(BaseFilter filter, bool noAccount = false)
        {
            var result = Mapper.Map<PagedEntity<StudentDTO>>(await this._uow.Students.GetStudentsAsync(filter, noAccount));
            return result;
        }

        public async Task<List<StudentDTO>> GetStudentsByUserAsync(int userId)
        {
            var result = Mapper.Map<List<StudentDTO>>(await this._uow.Students.GetStudentsByUserAsync(userId));
            return result;
        }

        public async Task<List<StudentDTO>> GetStudentsWithNoOrder(DateTime from, DateTime to)
        {
            var result = Mapper.Map<List<StudentDTO>>(await this._uow.Students.GetStudentsWithNoOrder(from, to));
            return result;
        }

        public async Task<List<StudentDTO>> GetStudentsWithAbandonedCart1(int hoursLeft)
        {
            var result = Mapper.Map<List<StudentDTO>>(await this._uow.Students.GetStudentsWithAbandonedCart1(hoursLeft));
            return result;
        }

        public async Task<List<StudentDTO>> GetStudentsWithAbandonedCart2(int daysBeforeCutOff)
        {
            var result = Mapper.Map<List<StudentDTO>>(await this._uow.Students.GetStudentsWithAbandonedCart2(daysBeforeCutOff));
            return result;
        }

        public async Task<List<StudentOrderDTO>> GetStudentsWithOrdersNotCollected(int daysPassed)
        {
            var result = Mapper.Map<List<StudentOrderDTO>>(await this._uow.Students.GetStudentsWithOrdersNotCollected(daysPassed));
            return result;
        }

        public async Task<StudentDTO> GetStudentByIdAsync(int id)
        {
            return Mapper.Map<StudentDTO>(await this._uow.Students.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateStudentAsync(StudentDTO dto)
        {
            var result = new BaseOperationResponse();
            var student = Mapper.Map<Student>(dto);
            var user = Mapper.Map<ApplicationUser>(dto);
            var cards = Mapper.Map<List<UserCardId>>(dto.Cards);
            var studentCards = Mapper.Map<List<StudentCard>>(dto.StudentCards);
            result = await this._uow.Students.CreateAsync(this._accountManager, student, user, dto.NewPassword, cards, studentCards);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStudentAsync(StudentDTO dto)
        {
            var result = new BaseOperationResponse();
            var student = Mapper.Map<Student>(dto);
            var user = Mapper.Map<ApplicationUser>(dto);
            var cards = Mapper.Map<List<UserCardId>>(dto.Cards);
            var studentCards = Mapper.Map<List<StudentCard>>(dto.StudentCards);
            var studentRestrictions = Mapper.Map<List<StudentRestriction>>(dto.Restrictions);
            var studentInterestGroups = Mapper.Map<List<StudentInterestGroup>>(dto.InterestGroups);
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
                    var row = sheet.CreateRow(rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
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

        #endregion

        #region Student Card

        public async Task<BaseOperationResponse> ImportStudentCardAsync(List<StudentCardImportDTO> dto)
        {
            var result = await this._uow.Students.ImportStudentCardAsync(_accountManager, dto);
            return result;
        }

        public async Task<PagedEntity<StudentCardDTO>> GetStudentCardsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<StudentCardDTO>>(await this._uow.StudentCards.GetStudentCardsAsync(filter));
            return result;
        }

        public async Task<StudentCardDTO> GetStudentCardByIdAsync(int id, string cardId = null)
        {
            return Mapper.Map<StudentCardDTO>(await this._uow.StudentCards.GetByIdAsync(id, cardId));
        }

        public async Task<StudentDTO> GetStudentByCardIdAsync(string cardId)
        {
            return Mapper.Map<StudentDTO>(await this._uow.StudentCards.GetStudentByCardIdAsync(cardId));
        }

        public async Task<BaseOperationResponse> ActivateStudentCardByIdAsync(string cardId)
        {
            return await this._uow.StudentCards.ActivateStudentCardByIdAsync(cardId);
        }

        public async Task<BaseOperationResponse> CreateStudentCardAsync(StudentCardDTO dto)
        {
            var result = await this._uow.StudentCards.CreateAsync(Mapper.Map<StudentCard>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStudentCardAsync(StudentCardDTO dto)
        {
            var result = await this._uow.StudentCards.UpdateAsync(Mapper.Map<StudentCard>(dto));
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
            var result = Mapper.Map<PagedEntity<InterestGroupDTO>>(await this._uow.InterestGroups.GetInterestGroupsAsync(filter));
            return result;
        }

        public async Task<InterestGroupDTO> GetInterestGroupByIdAsync(int id)
        {
            return Mapper.Map<InterestGroupDTO>(await this._uow.InterestGroups.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateInterestGroupAsync(InterestGroupDTO dto)
        {
            return await this._uow.InterestGroups.CreateAsync(Mapper.Map<InterestGroup>(dto));
        }

        public async Task<BaseOperationResponse> UpdateInterestGroupAsync(InterestGroupDTO dto)
        {
            return await this._uow.InterestGroups.UpdateAsync(Mapper.Map<InterestGroup>(dto));
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

        public async Task<PagedEntity<StudentGroupDTO>> GetStudentGroupsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<StudentGroupDTO>>(await this._uow.StudentGroups.GetStudentGroupsAsync(filter));
            return result;
        }

        public async Task<StudentGroupDTO> GetStudentGroupByIdAsync(int id)
        {
            return Mapper.Map<StudentGroupDTO>(await this._uow.StudentGroups.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateStudentGroupAsync(StudentGroupDTO dto)
        {
            var result = new BaseOperationResponse();
            var group = Mapper.Map<StudentGroup>(dto);
            var details = Mapper.Map<List<StudentGroupDetail>>(dto.Sgdetails);
            result = await this._uow.StudentGroups.CreateAsync(group, details);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStudentGroupAsync(StudentGroupDTO dto)
        {
            var result = new BaseOperationResponse();
            var group = Mapper.Map<StudentGroup>(dto);
            var details = Mapper.Map<List<StudentGroupDetail>>(dto.Sgdetails);
            result = await this._uow.StudentGroups.UpdateAsync(group, details);
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
            return Mapper.Map< List<StudentVoucherDTO>>(await this._uow.StudentCards.GetStudentVouchersAsync(studentId));
        }

        public async Task<List<VoucherDTO>> GetVouchersAsync(int studentId)
        {
            var svs = await this._uow.StudentCards.GetStudentVouchersAsync(studentId);

            return Mapper.Map<List<VoucherDTO>>(svs.Select(a => a.Voucher));
        }
        #endregion

        #region Outlet Terms

        public async Task<PagedEntity<OutletTermDTO>> GetOutletTermsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<OutletTermDTO>>(await this._uow.StudentGroups.GetOutletTermsAsync(filter));
            return result;
        }

        #endregion
    }
}
