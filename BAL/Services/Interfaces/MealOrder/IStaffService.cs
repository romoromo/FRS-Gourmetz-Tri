using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IStaffService
    {
        Task<BaseOperationResponse> CreateStaffAsync(StaffDTO dto);
        Task<BaseOperationResponse> DeleteStaffAsync(int id);
        Task<StaffDTO> GetStaffByIdAsync(int id);
        Task<PagedEntity<StaffDTO>> GetStaffsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateStaffAsync(StaffDTO dto);
    }
}