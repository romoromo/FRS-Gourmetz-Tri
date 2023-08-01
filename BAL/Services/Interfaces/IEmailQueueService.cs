using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces
{
    public interface IEmailQueueService
    {
        Task<BaseOperationResponse> CreateEmailQueueAsync(EmailQueueDTO dto);
        Task<BaseOperationResponse> DeleteEmailQueueAsync(int id);
        Task<EmailQueueDTO> GetEmailQueueByIdAsync(int id);
        Task<PagedEntity<EmailQueueDTO>> GetEmailQueuesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateEmailQueueAsync(EmailQueueDTO dto);

        Task<BaseOperationResponse> CreateContactUsSubjectAsync(ContactUsSubjectDTO dto);
        Task<BaseOperationResponse> DeleteContactUsSubjectAsync(int id);
        Task<ContactUsSubjectDTO> GetContactUsSubjectByIdAsync(int id);
        Task<PagedEntity<ContactUsSubjectDTO>> GetContactUsSubjectsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateContactUsSubjectAsync(ContactUsSubjectDTO dto);

        Task<BaseOperationResponse> CreateContactUsDetailAsync(ContactUsDetailDTO dto);
        Task<BaseOperationResponse> DeleteContactUsDetailAsync(int id);
        Task<ContactUsDetailDTO> GetContactUsDetailByIdAsync(int id);
        Task<PagedEntity<ContactUsDetailDTO>> GetContactUsDetailsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateContactUsDetailAsync(ContactUsDetailDTO dto);

        Task<BaseOperationResponse> CreateEmailTemplateAsync(EmailTemplateDTO dto);
        Task<BaseOperationResponse> DeleteEmailTemplateAsync(int id);
        Task<EmailTemplateDTO> GetEmailTemplateByIdAsync(int id);
        Task<PagedEntity<EmailTemplateDTO>> GetEmailTemplatesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateEmailTemplateAsync(EmailTemplateDTO dto);
    }
}