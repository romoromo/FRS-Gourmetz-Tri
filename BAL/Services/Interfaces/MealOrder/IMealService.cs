using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IMealService
    {
        Task<BaseOperationResponse> CreateMealPeriodAsync(MealPeriodDTO dto);
        Task<BaseOperationResponse> CreateMealTypeAsync(MealTypeDTO dto);
        Task<BaseOperationResponse> DeleteMealPeriodAsync(int id);
        Task<BaseOperationResponse> DeleteMealTypeAsync(int id);
        Task<MealPeriodDTO> GetMealPeriodByIdAsync(int id);
        Task<PagedEntity<MealPeriodDTO>> GetMealPeriodsAsync(BaseFilter filter);
        Task<MealTypeDTO> GetMealTypeByIdAsync(int id);
        Task<PagedEntity<MealTypeDTO>> GetMealTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateMealPeriodAsync(MealPeriodDTO dto);
        Task<BaseOperationResponse> UpdateMealTypeAsync(MealTypeDTO dto);

        Task<BaseOperationResponse> BulkCreateMealSessionAsync(List<MealSessionDTO> dto);
        Task<BaseOperationResponse> CreateMealSessionAsync(MealSessionDTO dto);
        Task<BaseOperationResponse> DeleteMealSessionAsync(int id);
        Task<MealSessionDTO> GetMealSessionByIdAsync(int id);
        Task<PagedEntity<MealSessionDTO>> GetMealSessionsAsync(BaseFilter filter);
        Task<List<MealSessionMealPeriodDTO>> GetMealSessionsByMealPeriod(int outletId, int catererId, int outletProfileId);
        Task<List<MealSessionMealPeriodDTO>> GetAllMealSessionsByMealPeriod(int outletId, int catererId, int outletProfileId);
        Task<List<MealSessionMealPeriodDTO>> GetMealSessionsByMealPeriodByOutlet(int outletId);
        Task<BaseOperationResponse> UpdateMealSessionAsync(MealSessionDTO dto);
        Task<List<MealSessionLiteDto>> GetMealSessionLiteByOutletId(int outletId);
    }
}