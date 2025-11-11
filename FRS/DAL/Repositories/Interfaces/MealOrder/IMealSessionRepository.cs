using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IMealSessionRepository
    {
        Task<BaseOperationResponse> CreateAsync(MealSession mealSession);
        Task<BaseOperationResponse> BulkCreateAsync(List<MealSession> mealSessions);
        Task<BaseOperationResponse> Delete(MealSession mealSession);
        Task<BaseOperationResponse> DeleteAsync(int mealSessionId);
        Task<MealSession> GetByIdAsync(int id);
        Task<PagedEntity<MealSession>> GetMealSessionsAsync(BaseFilter filter);
        List<MealSessionMealPeriod> GetMealSessionsByMealPeriod(int outletId, int catererId, int outletProfileId);
        List<MealSessionMealPeriod> GetMealSessionsByMealPeriodByOutlet(int outletId);
        Task<BaseOperationResponse> UpdateAsync(MealSession mealSession);
        Task<List<MealSessionLiteDto>> GetLiteByOutletId(int outletId);
        Task<List<MealSessionDetail>> GetMealSessionByOutletId(int outletId);
    }
}