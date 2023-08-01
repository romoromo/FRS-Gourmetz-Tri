using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IRestrictionService
    {
        Task<BaseOperationResponse> CreateRestrictionAsync(RestrictionDTO dto);
        Task<BaseOperationResponse> DeleteRestrictionAsync(int id);
        Task<RestrictionDTO> GetRestrictionByIdAsync(int id);
        Task<PagedEntity<RestrictionDTO>> GetRestrictionsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateRestrictionAsync(RestrictionDTO dto);

        Task<BaseOperationResponse> CreateRestrictionTypeAsync(RestrictionTypeDTO dto);
        Task<BaseOperationResponse> DeleteRestrictionTypeAsync(int id);
        Task<RestrictionTypeDTO> GetRestrictionTypeByIdAsync(int id);
        Task<PagedEntity<RestrictionTypeDTO>> GetRestrictionTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateRestrictionTypeAsync(RestrictionTypeDTO dto);
    }
}