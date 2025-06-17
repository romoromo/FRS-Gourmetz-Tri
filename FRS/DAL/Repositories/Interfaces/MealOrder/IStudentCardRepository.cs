using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IStudentCardRepository
    {
        Task<BaseOperationResponse> CreateAsync(StudentCard studentCard);
        Task<BaseOperationResponse> Delete(StudentCard studentCard);
        Task<BaseOperationResponse> DeleteAsync(int studentCardId);
        Task<StudentCard> GetByIdAsync(int id, string cardId);
        Task<Student> GetStudentByCardIdAsync(string cardId);
        Task<BaseOperationResponse> ActivateStudentCardByIdAsync(string cardId);
        Task<PagedEntity<StudentCard>> GetStudentCardsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(StudentCard studentCard);
        Task<BaseOperationResponse> ActivateStudentVoucher(int studentId, string code);
        Task<List<StudentVoucher>> GetStudentVouchersAsync(int studentId);
        Task<Dictionary<string, int>> GetVoucherUsedCountAsync(int studentId, List<string> voucherCodes);
        Task<BaseOperationResponse> AssignVoucherByStudentGroup(int studentGroupId, string code);
    }
}