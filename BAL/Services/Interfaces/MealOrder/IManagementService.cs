using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces
{
    public interface IManagementService
    {
        Task<BaseOperationResponse> CreateFaqSubjectAsync(FaqSubjectDTO dto);
        Task<BaseOperationResponse> DeleteFaqSubjectAsync(int id);
        Task<FaqSubjectDTO> GetFaqSubjectByIdAsync(int id);
        Task<PagedEntity<FaqSubjectDTO>> GetFaqSubjectsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateFaqSubjectAsync(FaqSubjectDTO dto);
        Task<BaseOperationResponse> OrderFaqSubjectAsync(int id, bool isAsc);
        Task<BaseOperationResponse> CreateFaqDetailAsync(FaqDetailDTO dto);
        Task<BaseOperationResponse> DeleteFaqDetailAsync(int id);
        Task<FaqDetailDTO> GetFaqDetailByIdAsync(int id);
        Task<PagedEntity<FaqDetailDTO>> GetFaqDetailsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateFaqDetailAsync(FaqDetailDTO dto);
        Task<BaseOperationResponse> OrderFaqDetailAsync(int id, bool isAsc);
    }
}