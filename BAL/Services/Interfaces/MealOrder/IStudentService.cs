using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IStudentService
    {
        Task<BaseOperationResponse> CreateStudentAsync(StudentDTO dto);
        Task<BaseOperationResponse> DeleteStudentAsync(int id);
        Task<StudentDTO> GetStudentByIdAsync(int id);
        Task<StudentDTO> GetStudentByEmailAsync(string email);
        Task<PagedEntity<StudentDTO>> GetStudentsAsync(BaseFilter filter, bool noAccount = false);
        Task<List<StudentDTO>> GetStudentsByUserAsync(int userId);
        Task<BaseOperationResponse> UpdateStudentAsync(StudentDTO dto);
        Task<BaseOperationResponse> CreateAccountAsync(List<int> ids, bool generateRandomPassword, string defaultPassword);
        Task<BaseOperationResponse> ImportStudentAsync(List<StudentImportDTO> dto);
        Task<byte[]> GenerateStudentReportXls(BaseFilter filter);

        Task<StudentDTO> GetStudentByCardIdAsync(string cardId);
        Task<BaseOperationResponse> ActivateStudentCardByIdAsync(string cardId);
        Task<BaseOperationResponse> CreateStudentCardAsync(StudentCardDTO dto);
        Task<BaseOperationResponse> DeleteStudentCardAsync(int id);
        Task<StudentCardDTO> GetStudentCardByIdAsync(int id, string cardId = null);
        Task<PagedEntity<StudentCardDTO>> GetStudentCardsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateStudentCardAsync(StudentCardDTO dto);
        Task<BaseOperationResponse> ImportStudentCardAsync(List<StudentCardImportDTO> dto);

        Task<BaseOperationResponse> CreateInterestGroupAsync(InterestGroupDTO dto);
        Task<BaseOperationResponse> DeleteInterestGroupAsync(int id);
        Task<InterestGroupDTO> GetInterestGroupByIdAsync(int id);
        Task<PagedEntity<InterestGroupDTO>> GetInterestGroupsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateInterestGroupAsync(InterestGroupDTO dto);

        Task<BaseOperationResponse> CreateStudentGroupAsync(StudentGroupDTO dto);
        Task<BaseOperationResponse> DeleteStudentGroupAsync(int id);
        Task<StudentGroupDTO> GetStudentGroupByIdAsync(int id);
        Task<PagedEntity<StudentGroupDTO>> GetStudentGroupsAsync(BaseFilter filter);
        Task<PagedEntity<StudentGroupSimpleDTO>> GetSimpleStudentGroups(BaseFilter filter);
        Task<BaseOperationResponse> UpdateStudentGroupAsync(StudentGroupDTO dto);

        Task<BaseOperationResponse> ActivateStudentVoucher(int studentId, string code);
        Task<List<StudentVoucherDTO>> GetStudentVouchersAsync(int studentId);
        Task<List<VoucherDTO>> GetVouchersAsync(int studentId);
        Task<BaseOperationResponse> SendEmailToInterestGroup(int templateId, int id);

        Task<List<StudentDTO>> GetStudentsWithNoOrder(DateTime from, DateTime to);
        Task<List<StudentDTO>> GetStudentsWithAbandonedCart1(int hoursLeft);
        Task<List<StudentDTO>> GetStudentsWithAbandonedCart2(int daysBeforeCutOff);
        Task<List<StudentOrderDTO>> GetStudentsWithOrdersNotCollected(int daysPassed);

        Task<PagedEntity<OutletTermDTO>> GetOutletTermsAsync(BaseFilter filter);

        Task<bool> CreateOrUpdateStudentGroupDetailAsync(int StudentGroupId, int StudentId, bool IsActive);
        Task<string> GenerateCode(int id);
        Task<List<GroupedTermStudentGroupDTO>> GetStudentMealPlanAsync(int outletId, DateTime? orderDate);
        Task<byte[]> GenerateStudentReportForImportXls(BaseFilter filter);

        Task<BaseOperationResponse> AddStudentLinksByUserAsync(int userId, int studentId);
        Task<BaseOperationResponse> AddStudentAccountLinkRequestAsync(int studentId, string email, bool emailSent);
        Task<BaseOperationResponse> UpdateStudentAccountLinkRequestAsync(int studentId, string email);
        Task<BaseOperationResponse> UpdateStudentEmail(int id, string email);
    }
}