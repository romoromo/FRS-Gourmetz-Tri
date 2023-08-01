using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IUserService
    {
        Task<BaseOperationResponse> CreateStaffTypeAsync(StaffTypeDTO dto);
        Task<BaseOperationResponse> DeleteStaffTypeAsync(int id);
        Task<StaffTypeDTO> GetStaffTypeByIdAsync(int id);
        Task<PagedEntity<StaffTypeDTO>> GetStaffTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateStaffTypeAsync(StaffTypeDTO dto);
    }
}