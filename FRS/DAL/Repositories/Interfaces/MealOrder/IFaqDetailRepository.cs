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
    public interface IFaqDetailRepository : IRepository<FaqDetail>
    {
        Task<BaseOperationResponse> CreateAsync(FaqDetail faqDetail);
        Task<BaseOperationResponse> DeleteAsync(int faqDetailId);
        Task<FaqDetail> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(FaqDetail faqDetail);
        Task<PagedEntity<FaqDetail>> GetFaqDetailsAsync(BaseFilter filter);
        Task<BaseOperationResponse> OrderAsync(int id, bool isAsc);
    }
}
