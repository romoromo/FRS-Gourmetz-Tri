using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IEmailTemplateRepository : IRepository<EmailTemplate>
    {
        Task<BaseOperationResponse> CreateAsync(EmailTemplate assetType);
        Task<BaseOperationResponse> DeleteAsync(int assetTypeId);
        Task<EmailTemplate> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(EmailTemplate assetType);
        Task<PagedEntity<EmailTemplate>> GetEmailTemplatesAsync(BaseFilter filter);
    }
}
