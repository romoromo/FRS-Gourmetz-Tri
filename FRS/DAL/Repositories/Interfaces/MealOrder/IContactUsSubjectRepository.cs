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
    public interface IContactUsSubjectRepository : IRepository<ContactUsSubject>
    {
        Task<BaseOperationResponse> CreateAsync(ContactUsSubject contactUsSubject);
        Task<BaseOperationResponse> DeleteAsync(int contactUsSubjectId);
        Task<ContactUsSubject> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(ContactUsSubject contactUsSubject);
        Task<PagedEntity<ContactUsSubject>> GetContactUsSubjectsAsync(BaseFilter filter);
    }
}
