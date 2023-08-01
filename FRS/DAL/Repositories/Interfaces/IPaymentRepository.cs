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
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<BaseOperationResponse> CreateAsync(Payment Payment);
        Task<BaseOperationResponse> DeleteAsync(int PaymentId);
        Task<Payment> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Payment Payment);
        Task<PagedEntity<Payment>> GetPaymentsAsync(BaseFilter filter);
        Task<List<Payment>> GetCreatedPaymentsAsync(int? studentId = null);
    }
}
