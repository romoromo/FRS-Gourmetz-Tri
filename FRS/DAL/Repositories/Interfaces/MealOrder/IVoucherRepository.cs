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
    public interface IVoucherRepository : IRepository<Voucher>
    {
        Task<BaseOperationResponse> CreateAsync(Voucher voucher);
        Task<BaseOperationResponse> DeleteAsync(int voucherId);
        Task<Voucher> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Voucher voucher);
        Task<PagedEntity<Voucher>> GetVouchersAsync(BaseFilter filter);
        Task<BaseOperationResponse> ValidateVoucher(int studentId, string code);
        Task<List<Voucher>> GetAllValidVoucher(int studentId);
    }
}
