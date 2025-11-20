using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using iTextSharp.text;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IClassRepository
    {
        Task<BaseOperationResponse> CreateAsync(Class classModel);
        Task<BaseOperationResponse> Delete(Class classModel);
        Task<BaseOperationResponse> DeleteAsync(int classId);
        Task<Class> GetByIdAsync(int id);
        Task<PagedEntity<Class>> GetClassesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(Class classModel);
        Task<List<Class>> GetByOutlet(int outletId);
        Task<List<Class>> GetClassByStudentGroupId(int studentGroupId);
    }
}