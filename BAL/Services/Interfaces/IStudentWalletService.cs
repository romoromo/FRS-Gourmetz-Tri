using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
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
        Task<PagedEntity<StudentWalletTransactionDTO>> GetWalletTransactionsAsync(BaseFilter filter);
        Task<PagedEntity<StudentWalletTransactionSimpleDTO>> GetWalletTransactionsSimpleAsync(BaseFilter filter);
        Task<BaseOperationResponse> StudentWalletTransactionAsync(StudentWalletTransactionDTO dto);
        Task<List<StudentWalletTransactionDTO>> GetWalletTransactionByIdAsync(int id);
        Task<BaseOperationResponse> TopupWalletBalanceByStudentGroupIdAsync(int studentGroupId, double amount, int userId, WalletType walletTypeData);
        Task<BaseOperationResponse> TopupWalletBalanceByStudentIdAsync(int studentId, double amount, int userid, WalletType walletType, int? walletPaymentId);
        Task<BaseOperationResponse> RefundToWalletBalanceAsync(int studentId, double amount, int userid, WalletType walletType, int? tokenOrderId);
        Task<BaseOperationResponse> OffBoardingStudent(int studentId, int userId);

        Task<byte[]> GenerateXls(BaseFilter filter);
        Task<byte[]> GenerateWalletTransactionByStudent(BaseFilter filter);
        Task<BaseOperationResponse> WalletTransfer(int studentIdFrom, int studentIdTo, double amount, int userId);

        Task<BaseOperationResponse> UpdateStudentWalletTransaction(StudentWalletTransactionDTO dto);
    }
}
