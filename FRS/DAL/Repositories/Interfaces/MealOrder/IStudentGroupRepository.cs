using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IStudentGroupRepository
    {
        Task<BaseOperationResponse> CreateAsync(StudentGroup group, List<StudentGroupDetail> groupDetails);
        Task<BaseOperationResponse> Delete(StudentGroup group);
        Task<BaseOperationResponse> DeleteAsync(int groupId);
        Task<StudentGroup> GetByIdAsync(int id);
        Task<PagedEntity<StudentGroup>> GetStudentGroupsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(StudentGroup group, List<StudentGroupDetail> groupDetails);
    }
}