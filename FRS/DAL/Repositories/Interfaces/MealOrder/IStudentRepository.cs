using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IStudentRepository
    {
        Task<IQueryable<Student>> GetAllStudentsAsync();
        Task<BaseOperationResponse> CreateAsync(IAccountManager accountManager, Student student, ApplicationUser user, string newPassword, List<UserCardId> cards, List<StudentCard> studentCards);
        Task<BaseOperationResponse> Delete(Student student);
        Task<BaseOperationResponse> DeleteAsync(int studentId);
        Task<BaseOperationResponse> CreateAccountAsync(List<int> id, bool generateRandomPassword, IAccountManager accountManager, string defaultPassword);
        Task<Student> GetByIdAsync(int id);
        Task<BaseOperationResponse> ImportStudentAsync(IAccountManager accountManager, List<StudentImportDTO> dto);
        Task<PagedEntity<Student>> GetStudentsAsync(BaseFilter filter, bool noAccount = false);
        Task<List<Student>> GetStudentsByUserAsync(int userId);
        Task<BaseOperationResponse> UpdateAsync(IAccountManager accountManager, Student student, ApplicationUser user, string currentPassword, string newPassword, List<UserCardId> cards, List<StudentCard> studentCards, List<StudentRestriction> restrictions, List<StudentInterestGroup> interestGroups);
        Task<BaseOperationResponse> ImportStudentCardAsync(IAccountManager accountManager, List<StudentCardImportDTO> dto);
        Task<List<Student>> GetByInterestGroupIdAsync(int id);
        Task<List<Student>> GetStudentsWithNoOrder(DateTime from, DateTime to);
        Task<List<Student>> GetStudentsWithAbandonedCart1(int hoursLeft);
        Task<List<Student>> GetStudentsWithAbandonedCart2(int daysBeforeCutOff);
        Task<List<StudentOrderModel>> GetStudentsWithOrdersNotCollected(int daysPassed);
        Task<BaseOperationResponse> AddStudentLinksByUserAsync(int userId, int studentId);
    }
}