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
    public interface IFaqSubjectRepository : IRepository<FaqSubject>
    {
        Task<BaseOperationResponse> CreateAsync(FaqSubject faqSubject);
        Task<BaseOperationResponse> DeleteAsync(int faqSubjectId);
        Task<FaqSubject> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(FaqSubject faqSubject);
        Task<PagedEntity<FaqSubject>> GetFaqSubjectsAsync(BaseFilter filter);
        Task<BaseOperationResponse> OrderAsync(int id, bool isAsc);
    }
}
