using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface IOutletTermRepository
    {
        Task<BaseOperationResponse> CreateAsync(OutletTerm data);
        Task<BaseOperationResponse> Delete(OutletTerm data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<OutletTerm> GetByIdAsync(int id);
        Task<PagedEntity<OutletTerm>> GetOutletTermsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(OutletTerm data);
    }
}