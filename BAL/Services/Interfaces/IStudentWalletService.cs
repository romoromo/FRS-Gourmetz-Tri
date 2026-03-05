using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IStudentWalletService
    {
        Task<PagedEntity<StudentWalletTransactionDTO>> GetWalletTransactionsAsync(EWalletTransactionFilter filter);
        Task<PagedEntity<StudentWalletTransactionSimpleDTO>> GetWalletTransactionsSimpleAsync(BaseFilter filter);
        Task<BaseOperationResponse> StudentWalletTransactionAsync(StudentWalletTransactionDTO dto);
        Task<List<StudentWalletTransactionDTO>> GetWalletTransactionByIdAsync(int id);
        Task<BaseOperationResponse> TopupWalletBalanceByStudentGroupIdAsync(int studentGroupId, double amount, int userId, WalletType walletTypeData);
        Task<BaseOperationResponse> TopupWalletBalanceByStudentIdAsync(int studentId, double amount, int userid, WalletType walletType, int? walletPaymentId);
        Task<BaseOperationResponse> RefundToWalletBalanceAsync(int studentId, double amount, int userid, WalletType walletType, int? tokenOrderId);
        Task<BaseOperationResponse> OffBoardingStudent(int studentId, int userId);

        Task<byte[]> GenerateXls(EWalletTransactionFilter filter);
        Task<byte[]> GenerateWalletTransactionByStudent(BaseFilter filter);
        Task<BaseOperationResponse> WalletTransfer(int studentIdFrom, int studentIdTo, double amount, int userId);

        Task<BaseOperationResponse> UpdateStudentWalletTransaction(StudentWalletTransactionDTO dto);

        Task<PagedEntity<FASMonthlyBillingReportDTO>> GetFASMonthlyBillingReport(FASMonthlyBillingFilter filter);
        Task<byte[]> GenerateFASMonthlyBillingReport(FASMonthlyBillingFilter filter);

        Task<object> GetInvoiceDetail(int id);

        Task<PagedEntity<DetailedBasicWalletTopUpReport>> GetDetailedBasicWalletTopUpReport(DetailedBasicWalletTopUpFilter filter);
        Task<byte[]> GenerateGetDetailedBasicWalletTopUpReport(DetailedBasicWalletTopUpFilter filter);
    }
}
