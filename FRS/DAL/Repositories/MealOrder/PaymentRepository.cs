using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL.Models.MealOrder;

namespace DAL.Repositories.MealOrder
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public PaymentRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Payment>> GetPaymentsAsync(BaseFilter filter)
        {
            IQueryable<Payment> query = _appContext.Payments.Where(p => p.Status != "CLOSED")
                .Include(e => e.Institution);

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<Payment>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion

        public async Task<List<Payment>> GetCreatedPaymentsAsync(int? studentId = null)
        {
            IQueryable<Payment> query = _appContext.Payments.Where(d => d.Status == "CREATED" && d.CreatedDate >= DateTime.Now.AddDays(-7))
                .Include(e => e.Institution);

            if(studentId != null) query = query.Where(d => d.StudentId == studentId);
           

            return query.ToList();
        }

        public async Task<Payment> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(Payment Payment)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(Payment);

            
            

            if (await _appContext.SaveChangesAsync() > 0)
            {
                if (Payment.Status == "SUCCESS")
                {
                    var studentVoucher = _appContext.StudentVouchers.FirstOrDefault(a => a.IsActive && a.StudentId == Payment.StudentId && a.VoucherId == Payment.VoucherId);

                    if (studentVoucher != null)
                    {
                        studentVoucher.Status = "USED";
                        _appContext.StudentVouchers.Update(studentVoucher);
                        _appContext.SaveChanges();
                    }


                    foreach (var t in f.TokenOrders)
                    {
                        var tOrder = _appContext.TokenOrders.FirstOrDefault(a => a.Id == t.Id);
                        if (tOrder != null)
                        {
                            tOrder.Status = tOrder.Status == "cancelled" ? tOrder.Status : "paid";
                            _appContext.TokenOrders.Update(tOrder);
                            _appContext.SaveChanges();
                        }
                    }

                    foreach (var t in f.MealPlanOrders)
                    {
                        var tOrder = _appContext.MealPlanOrders.FirstOrDefault(a => a.Id == t.Id);
                        if (tOrder != null)
                        {
                            tOrder.Status = tOrder.Status == "cancelled" ? tOrder.Status : "paid";
                            _appContext.MealPlanOrders.Update(tOrder);
                            _appContext.SaveChanges();
                        }
                    }
                }


                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save payment type!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Payment Payment)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == Payment.Id);

            f.CopyFrom(Payment);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                if (Payment.Status == "SUCCESS")
                {
                    var studentVoucher = _appContext.StudentVouchers.FirstOrDefault(a => a.IsActive && a.StudentId == Payment.StudentId && a.VoucherId == Payment.VoucherId);
                    
                    if (studentVoucher != null)
                    {
                        studentVoucher.Status = "USED";
                        _appContext.StudentVouchers.Update(studentVoucher);
                        _appContext.SaveChanges();
                    }


                    foreach (var t in f.TokenOrders)
                    {
                        var tOrder = _appContext.TokenOrders.FirstOrDefault(a => a.Id == t.Id);
                        if (tOrder != null)
                        {
                            tOrder.Status = tOrder.Status == "cancelled" ? tOrder.Status : "paid";
                            _appContext.TokenOrders.Update(tOrder);
                            _appContext.SaveChanges();
                        }
                    }

                    foreach (var t in f.MealPlanOrders)
                    {
                        var tOrder = _appContext.MealPlanOrders.FirstOrDefault(a => a.Id == t.Id);
                        if (tOrder != null)
                        {
                            tOrder.Status = tOrder.Status == "cancelled" ? tOrder.Status : "paid";
                            _appContext.MealPlanOrders.Update(tOrder);
                            _appContext.SaveChanges();
                        }
                    }
                }

                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save payment type!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int PaymentId)
        {
            var result = new BaseOperationResponse();
            var Payment = await GetSingleOrDefaultAsync(r => r.Id == PaymentId);

            if (Payment != null)
                return await Delete(Payment);

            result.IsSuccess = false;
            result.Message = "Payment type not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Payment Payment)
        {
            var result = new BaseOperationResponse();
            SoftDelete(Payment);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete payment type!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
