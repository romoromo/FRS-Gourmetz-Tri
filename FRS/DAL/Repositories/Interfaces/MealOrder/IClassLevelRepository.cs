using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IClassLevelRepository
    {
        Task<BaseOperationResponse> CreateAsync(ClassLevel classLevel);
        Task<BaseOperationResponse> Delete(ClassLevel classLevel);
        Task<BaseOperationResponse> DeleteAsync(int classLevelId);
        Task<ClassLevel> GetByIdAsync(int id);
        Task<PagedEntity<ClassLevel>> GetClassLevelsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(ClassLevel classLevel);
        Task<List<ClassLevel>> GetClassLevelsByOutletIdAsync(int outletId);
        Task<List<MealSessionDetail>> GetMealSessionDetail(int id, int outletId);
    }
}