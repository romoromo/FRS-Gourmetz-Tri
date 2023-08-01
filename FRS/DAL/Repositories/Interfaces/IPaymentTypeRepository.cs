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
    public interface IPaymentTypeRepository : IRepository<PaymentType>
    {
        Task<BaseOperationResponse> CreateAsync(PaymentType paymentType);
        Task<BaseOperationResponse> DeleteAsync(int paymentTypeId);
        Task<PaymentType> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(PaymentType paymentType);
        Task<PagedEntity<PaymentType>> GetPaymentTypesAsync(BaseFilter filter);
    }
}
