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
    public interface IWaiverRepository : IRepository<Waiver>
    {
        Task<BaseOperationResponse> CreateAsync(Waiver waiver);
        Task<BaseOperationResponse> DeleteAsync(int waiverId);
        Task<Waiver> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Waiver waiver);
        Task<PagedEntity<Waiver>> GetWaiversAsync(BaseFilter filter);
    }
}
