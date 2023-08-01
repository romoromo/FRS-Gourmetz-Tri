using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IStaffRepository
    {
        Task<BaseOperationResponse> CreateAsync(IAccountManager accountManager, Staff student, ApplicationUser user, string newPassword, List<UserCardId> cards);
        Task<BaseOperationResponse> Delete(Staff student);
        Task<BaseOperationResponse> DeleteAsync(int studentId);
        Task<Staff> GetByIdAsync(int id);
        Task<PagedEntity<Staff>> GetStaffsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(IAccountManager accountManager, Staff student, ApplicationUser user, string currentPassword, string newPassword, List<UserCardId> cards);
    }
}