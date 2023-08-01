using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IContactUsDetailRepository : IRepository<ContactUsDetail>
    {
        Task<BaseOperationResponse> CreateAsync(ContactUsDetail contactUsDetail);
        Task<BaseOperationResponse> DeleteAsync(int contactUsDetailId);
        Task<ContactUsDetail> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(ContactUsDetail contactUsDetail);
        Task<PagedEntity<ContactUsDetail>> GetContactUsDetailsAsync(BaseFilter filter);
    }
}
