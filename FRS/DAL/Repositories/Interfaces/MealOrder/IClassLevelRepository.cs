using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

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
    }
}