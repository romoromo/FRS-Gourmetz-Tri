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
        Task<BaseOperationResponse> StudentWalletTransactionAsync(StudentWalletTransactionDTO dto);
        Task<List<StudentWalletTransactionDTO>> GetWalletTransactionByIdAsync(int id);
        Task<BaseOperationResponse> TopupWalletBalanceByStudentGroupIdAsync(int studentGroupId, double amount);
    }
}
