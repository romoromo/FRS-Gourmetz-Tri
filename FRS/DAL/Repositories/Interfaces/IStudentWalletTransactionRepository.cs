using DAL.Core;
using DAL.Filters;
using DAL.Models;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IStudentWalletTransactionRepository : IRepository<StudentWalletTransaction>
    {
        Task<BaseOperationResponse> CreateAsync(StudentWalletTransaction walletTransaction);
        Task<BaseOperationResponse> DeleteAsync(int walletTransactionId);
        Task<StudentWalletTransaction> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(StudentWalletTransaction walletTransaction);
        Task<PagedEntity<StudentWalletTransaction>> GetWalletTransactionsAsync(BaseFilter filter);
        Task<BaseOperationResponse> TopupWalletBalanceByStudentGroupIdAsync(int studentGroupId, double amount, int userId, WalletType walletTypeData);
        Task<BaseOperationResponse> TopupWalletBalanceByStudentIdAsync(int studentId, double amount, int userId, WalletType walletType, int? walletPaymentId);
        Task<BaseOperationResponse> RefundToWalletBalanceAsync(int studentId, double amount, int userId, WalletType walletType, int? tokenOrderId);
        Task<BaseOperationResponse> OffBoardingStudent(int studentId, int userId);
        Task<BaseOperationResponse> WalletTransfer(int studentIdFrom, int studentIdTo, double amount, int userId);
        Task<BaseOperationResponse> UpdateStudentWalletTransaction(StudentWalletTransaction walletTransaction);

        Task FASRechargeable(CancellationToken ct = default);

        Tuple<double, double, double> GetWalletTransactions(int studentId, WalletTransactionType walletTransaction);
        Task<DataTable> GetWalletTransactionForReport(DateTime startDate, DateTime endDate, List<int> outletIds, List<int> classLevelIds);
    }
}
