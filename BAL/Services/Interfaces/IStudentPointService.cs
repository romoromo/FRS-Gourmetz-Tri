using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IStudentPointService
    {
        Task<PagedEntity<StudentPointTransactionDTO>> GetPointTransactionsAsync(BaseFilter filter);
        Task<BaseOperationResponse> StudentPointTransactionAsync(StudentPointTransactionDTO dto);
        Task<List<StudentPointTransactionDTO>> GetPointTransactionByIdAsync(int id);
        Task<BaseOperationResponse> TopupPointBalanceByStudentGroupIdAsync(int studentGroupId, double amount);
        Task<BaseOperationResponse> TopupPointBalanceByStudentIdAsync(int studentId, double amount);
    }
}
