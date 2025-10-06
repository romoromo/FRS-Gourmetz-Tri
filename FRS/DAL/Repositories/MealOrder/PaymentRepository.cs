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
using System.Transactions;
using DAL.Core.Helpers;

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

        public async Task<List<WalletPayment>> GetCreatedWalletPaymentsAsync(int? studentId = null)
        {
            IQueryable<WalletPayment> query = _appContext.WalletPayments.Where(d => d.Status == "CREATED" && d.CreatedDate >= DateTime.Now.AddDays(-7))
                .Include(e => e.Institution);

            if (studentId != null) query = query.Where(d => d.StudentId == studentId);


            return query.ToList();
        }

        public async Task<Payment> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(Payment Payment)
        {
            var result = new BaseOperationResponse();
            Payment f = new Payment();

            bool useVoucher = await _appContext.Vouchers.AnyAsync(x => x.Id == Payment.VoucherId);

            int resultSaveChange = 0;
            if (useVoucher)
            {
                var voucherData = await _appContext.Vouchers
                    .Where(x => x.Id == Payment.VoucherId)
                    .Select(x => new
                    {
                        x.Id,
                        x.MaxRedeemCheckout,
                        x.MaxRedeemCheckoutMessage
                    })
                    .FirstOrDefaultAsync();

                if (voucherData?.MaxRedeemCheckout > 0)
                {
                    using var transaction = await _appContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
                    var getUsedVoucher = await _appContext.StudentVouchers.CountAsync(x => x.VoucherId == voucherData.Id && x.Status == "USED");
                    if(getUsedVoucher >= voucherData.MaxRedeemCheckout)
                    {
                        string defaultMessage = "Sorry the voucher is limited to some users and has been fully redeemed";
                        if (!string.IsNullOrEmpty(voucherData?.MaxRedeemCheckoutMessage))
                        {
                            defaultMessage = voucherData?.MaxRedeemCheckoutMessage;
                        }
                        result.IsSuccess = false;
                        result.Message = defaultMessage;
                        return result;
                    }
                    else
                    {
                        _appContext.AuditUserActivityType = new AuditUserActivityType
                        {
                            GroupId = Common.GenerateUniqueStringId(),
                            ActionName = UserActivityType.PAYMENT_CREATE.ToString(),
                            Remarks = "Payment was created."
                        };
                        f = await AddAsync(Payment);
                        resultSaveChange = await _appContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }

                }
                else
                {
                    _appContext.AuditUserActivityType = new AuditUserActivityType
                    {
                        GroupId = Common.GenerateUniqueStringId(),
                        ActionName = UserActivityType.PAYMENT_CREATE.ToString(),
                        Remarks = "Payment was created."
                    };

                    f = await AddAsync(Payment);
                    resultSaveChange = await _appContext.SaveChangesAsync();
                }
            }
            else
            {
                _appContext.AuditUserActivityType = new AuditUserActivityType
                {
                    GroupId = Common.GenerateUniqueStringId(),
                    ActionName = UserActivityType.PAYMENT_CREATE.ToString(),
                    Remarks = "Payment was created."
                };

                f = await AddAsync(Payment);
                resultSaveChange = await _appContext.SaveChangesAsync();
            }

            if (resultSaveChange > 0)
            {
                if (Payment.Status == "SUCCESS")
                {
                    var studentVoucher = _appContext.StudentVouchers.FirstOrDefault(a => a.IsActive && a.StudentId == Payment.StudentId && a.VoucherId == Payment.VoucherId && a.Status == "NEW");

                    if (studentVoucher != null)
                    {
                        studentVoucher.Status = "USED";
                        _appContext.StudentVouchers.Update(studentVoucher);
                        _appContext.SaveChanges();
                    }

                    if(f?.TokenOrders?.Any() ?? false)
                    {
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
                    }

                    if(f?.MealPlanOrders?.Any() ?? false)
                    {
                        foreach (var t in f.MealPlanOrders)
                        {
                            var tOrder = _appContext.MealPlanOrders.FirstOrDefault(a => a.Id == t.Id);
                            if (tOrder != null)
                            {
                                tOrder.Status = tOrder.Status == "cancelled" ? tOrder.Status : "paid";
                                _appContext.MealPlanOrders.Update(tOrder);
                                _appContext.SaveChanges();

                                if (t.StudentGroupId.HasValue && t.ProfileId.HasValue) await CreateOrUpdateStudentGroupDetailAsync(t.StudentGroupId.Value, t.ProfileId.Value, true);
                            }
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

            _appContext.ResetAuditUserAction();
            return result;
        }

        public async Task<BaseOperationResponse> CreateWalletPaymentAsync(WalletPayment Payment)
        {
            var result = new BaseOperationResponse();
            WalletPayment f = new WalletPayment();

            int resultSaveChange = 0;
            
                _appContext.AuditUserActivityType = new AuditUserActivityType
                {
                    GroupId = Common.GenerateUniqueStringId(),
                    ActionName = UserActivityType.PAYMENT_CREATE.ToString(),
                    Remarks = "Payment was created."
                };

                _appContext.WalletPayments.Add(Payment);
                f = Payment;
                resultSaveChange = await _appContext.SaveChangesAsync();
            

            if (resultSaveChange > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save payment type!";
                result.IsSuccess = false;
            }

            _appContext.ResetAuditUserAction();
            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Payment Payment)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.PAYMENT_UPDATE.ToString(),
                Remarks = "Payment was updated."
            };

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

                            if(t.StudentGroupId.HasValue && t.ProfileId.HasValue) await CreateOrUpdateStudentGroupDetailAsync(t.StudentGroupId.Value, t.ProfileId.Value, true);
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
            _appContext.ResetAuditUserAction();

            return result;
        }

        public async Task<BaseOperationResponse> UpdateWalletPaymentAsync(WalletPayment Payment)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.PAYMENT_UPDATE.ToString(),
                Remarks = "Payment was updated."
            };

            var result = new BaseOperationResponse();

            var f = await _appContext.WalletPayments.FirstOrDefaultAsync(e => e.Id == Payment.Id);

            f.CopyFrom(Payment);
            _appContext.WalletPayments.Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save payment type!";
                result.IsSuccess = false;
            }
            _appContext.ResetAuditUserAction();

            return result;
        }

        public async Task<bool> CreateOrUpdateStudentGroupDetailAsync(int StudentGroupId, int StudentId, bool IsActive)
        {

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var ori = await _appContext.StudentGroupDetails.FirstOrDefaultAsync(e => e.StudentGroupId == StudentGroupId && e.StudentId == StudentId);

                if (ori == null)
                {
                    ori = new StudentGroupDetail();
                    ori.StudentGroupId = StudentGroupId;
                    ori.StudentId = StudentId;
                    ori.IsActive = IsActive;

                    var f = await _appContext.StudentGroupDetails.AddAsync(ori);
                }
                else
                {
                    ori.IsActive = IsActive;

                    _appContext.StudentGroupDetails.Update(ori);
                }


                if (await _appContext.SaveChangesAsync() > 0)
                {
                    scope.Complete();
                    return true;
                }
                else
                {
                    return false;
                }
            }

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
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.PAYMENT_DELETE.ToString(),
                Remarks = "Payment was deleted."
            };

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

            _appContext.ResetAuditUserAction();
            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
