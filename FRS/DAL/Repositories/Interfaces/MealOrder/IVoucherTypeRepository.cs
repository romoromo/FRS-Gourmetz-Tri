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
    public interface IVoucherTypeRepository : IRepository<VoucherType>
    {
        Task<BaseOperationResponse> CreateAsync(VoucherType voucherType);
        Task<BaseOperationResponse> DeleteAsync(int voucherTypeId);
        Task<VoucherType> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(VoucherType voucherType);
        Task<PagedEntity<VoucherType>> GetVoucherTypesAsync(BaseFilter filter);
    }
}
