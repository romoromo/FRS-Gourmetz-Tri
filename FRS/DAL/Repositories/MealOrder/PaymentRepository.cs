using DAL.Core;
using DAL.Core.Helpers;
using DAL.Core.Logging;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace DAL.Repositories.MealOrder
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        private ILogger _logger;
        public PaymentRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId, ILogger<PaymentRepository> logger) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
            this._logger = logger;
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

            if (studentId != null) query = query.Where(d => d.StudentId == studentId);


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
            _logger.LogInformation(
                $"[PAYMENT][START] " +
                $"PaymentId={Payment?.Id}, StudentId={Payment?.StudentId}, VoucherId={Payment?.VoucherId}, " +
                $"PaymentTypeId={Payment?.PaymentTypeId}, Invoice={Payment?.InvoiceNumber}, Total={Payment?.total}, UserId={Payment?.UserId}"
            );

            var result = new BaseOperationResponse();
            Payment f = new Payment();

            _logger.LogInformation("[PAYMENT] Checking flags useVoucher/useWallet...");

            bool useVoucher = await _appContext.Vouchers.AnyAsync(x => x.Id == Payment.VoucherId);
            bool useWallet = await _appContext.PaymentTypes.AnyAsync(x => x.Name == "Wallet" && x.Id == Payment.PaymentTypeId);

            _logger.LogInformation("[PAYMENT] Flags resolved: useVoucher={UseVoucher}, useWallet={UseWallet}",
            useVoucher, useWallet);

            int resultSaveChange = 0;
            if (useVoucher)
            {
                _logger.LogInformation("[PAYMENT] Voucher flow started. VoucherId={VoucherId}", Payment.VoucherId);

                var voucherData = await _appContext.Vouchers
                    .Where(x => x.Id == Payment.VoucherId)
                    .Select(x => new
                    {
                        x.Id,
                        x.MaxRedeemCheckout,
                        x.MaxRedeemCheckoutMessage
                    })
                    .FirstOrDefaultAsync();

                _logger.LogInformation("[PAYMENT] Voucher fetched. VoucherId={VoucherId}, MaxRedeemCheckout={MaxRedeem}",
                voucherData?.Id, voucherData?.MaxRedeemCheckout);

                if (voucherData?.MaxRedeemCheckout > 0)
                {
                    _logger.LogInformation("[PAYMENT] MaxRedeemCheckout enabled. Begin SERIALIZABLE transaction.");

                    using var transaction = await _appContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
                    var getUsedVoucher = await _appContext.StudentVouchers.CountAsync(x => x.VoucherId == voucherData.Id && x.Status == "USED");

                    _logger.LogInformation("[PAYMENT] USED voucher count={UsedCount}, MaxAllowed={MaxAllowed}",
                   getUsedVoucher, voucherData.MaxRedeemCheckout);


                    if (getUsedVoucher >= voucherData.MaxRedeemCheckout)
                    {
                        string defaultMessage = "Sorry the voucher is limited to some users and has been fully redeemed";
                        if (!string.IsNullOrEmpty(voucherData?.MaxRedeemCheckoutMessage))
                        {
                            defaultMessage = voucherData?.MaxRedeemCheckoutMessage;
                        }

                        _logger.LogWarning("[PAYMENT] Voucher fully redeemed. Rejecting payment. Message={Message}",
                        defaultMessage);

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

                        _logger.LogInformation("[PAYMENT] SaveChangesAsync result={Result}", resultSaveChange);

                        await transaction.CommitAsync();

                        _logger.LogInformation("[PAYMENT] Voucher transaction committed successfully.");
                    }

                }
                else
                {
                    _logger.LogInformation("[PAYMENT] Voucher has no MaxRedeemCheckout. Continue normally.");

                    _appContext.AuditUserActivityType = new AuditUserActivityType
                    {
                        GroupId = Common.GenerateUniqueStringId(),
                        ActionName = UserActivityType.PAYMENT_CREATE.ToString(),
                        Remarks = "Payment was created."
                    };

                    f = await AddAsync(Payment);
                    resultSaveChange = await _appContext.SaveChangesAsync();

                    _logger.LogInformation("[PAYMENT] SaveChangesAsync result={Result}", resultSaveChange);
                }
            }
            else if (useWallet)
            {
                _logger.LogInformation("[PAYMENT] Wallet flow started. StudentId={StudentId}. UserId={UseriD}. Payment Amount={total}", Payment.StudentId, Payment.UserId, Payment.total);

                var student = await this._appContext.Students
                .FirstOrDefaultAsync(e => e.IsActive && e.Id == Payment.StudentId);

                if (student == null)
                {
                    result.IsSuccess = false;
                    result.Message = $"Student with Id={Payment.StudentId} not found or inactive.";

                    _logger.LogWarning("[PAYMENT] Wallet validation failed: {Message}", result.Message);

                    return result;
                }


                if (student.IsWalletFreeze)
                {
                    result.IsSuccess = false;
                    result.Message = "Wallet is Freezed";

                    _logger.LogWarning("[PAYMENT] {Message}", result.Message);

                    return result;
                }



                _logger.LogInformation($"[PAYMENT] Loading wallets...");

                var wallets = await _appContext.StudentWallets.Where(w => w.StudentId == student.Id).ToListAsync();
                var fasWallet = wallets.FirstOrDefault(x => x.Type == WalletType.FAS.ToString());
                var normalWallet = wallets.FirstOrDefault(x => x.Type == WalletType.BASIC.ToString());

                if (fasWallet == null)
                {
                    _logger.LogInformation($"[PAYMENT] FAS wallet not found. Creating new FAS wallet...");
                    fasWallet = new StudentWallet
                    {
                        StudentId = student.Id,
                        Type = WalletType.FAS.ToString(),
                        Balance = 0,
                        CreatedBy = Payment.UserId,
                        UpdatedBy = Payment.UserId
                    };
                    await _appContext.StudentWallets.AddAsync(fasWallet);
                }

                if (normalWallet == null)
                {
                    _logger.LogWarning($"[PAYMENT] Basic wallet not found. Creating new Basic wallet...");
                    normalWallet = new StudentWallet
                    {
                        StudentId = student.Id,
                        Type = WalletType.BASIC.ToString(),
                        Balance = 0,
                        CreatedBy = Payment.UserId,
                        UpdatedBy = Payment.UserId
                    };
                    await _appContext.StudentWallets.AddAsync(normalWallet);
                }


                var totalWallet = fasWallet.Balance + normalWallet.Balance;
                var fasBalance = fasWallet.Balance;
                var basicBalance = normalWallet.Balance;

                if (totalWallet - Decimal.ToDouble(Payment.total) < 0)
                {
                    result.IsSuccess = false;
                    result.Message = "Insufficient balance.";

                    _logger.LogWarning("[PAYMENT] {Message} Student Wallet Balance: {WalletBalance}, Basic Balance: {basicBalance}, Fas Balance: {fasBalance}, Payment: {total}", result.Message, student.WalletBalance, basicBalance, fasBalance, Payment.total);

                    return result;
                }

                if (student.WalletDailyLimit > 0)
                {
                    _logger.LogInformation($"[PAYMENT] Daily limit enabled. Limit={student.WalletDailyLimit}");


                    if (Decimal.ToDouble(Payment.total) > student.WalletDailyLimit)
                    {
                        result.IsSuccess = false;
                        result.Message = "The payment exceed the wallet daily limit";

                        _logger.LogWarning("[PAYMENT] {Message} {total} {WalletDailyLimit}", result.Message, Payment.total, student.WalletDailyLimit);

                        return result;
                    }
                    else
                    {
                        DateTime today = DateTime.Today;

                        _logger.LogInformation($"[PAYMENT] Loading today's transactions for daily limit calc. Date={today:yyyy-MM-dd}");


                        var todayTrans = await _appContext.StudentWalletTransactions.Where(t => t.StudentId == student.Id && t.TransactionType == WalletTransactionType.DEBIT.ToString() && t.CreatedDate.Date == today).ToListAsync();

                        var totalTrans = 0.0;
                        foreach (var trans in todayTrans)
                        {
                            totalTrans += trans.Amount;
                        }

                        _logger.LogInformation($"[PAYMENT] Daily spend so far={totalTrans}, " +
                                $"After this payment={totalTrans}, Limit={student.WalletDailyLimit}");

                        if ((totalTrans + Decimal.ToDouble(Payment.total)) > student.WalletDailyLimit)
                        {
                            result.IsSuccess = false;
                            result.Message = "The payment exceed the wallet daily limit";

                            _logger.LogInformation($"[PAYMENT][END] Returning failure (daily limit exceeded cumulative) {totalTrans} - {Payment.total} - {student.WalletDailyLimit}");

                            return result;
                        }

                    }
                }

                double originalAmount = decimal.ToDouble(Payment.total);
                double remainingAmount = originalAmount;
                double oldFasBalance = fasWallet.Balance;
                double oldNormalBalance = normalWallet.Balance;

                _logger.LogInformation($"[PAYMENT] Wallet balances BEFORE deduction. FAS={oldFasBalance}, BASIC={oldNormalBalance}, Amount={originalAmount}");

                if (remainingAmount <= fasWallet.Balance)
                {
                    fasWallet.Balance -= remainingAmount;
                    remainingAmount = 0;
                }
                else
                {
                    remainingAmount -= fasWallet.Balance;
                    fasWallet.Balance = 0;
                    if (normalWallet.Balance >= remainingAmount)
                    {
                        normalWallet.Balance -= remainingAmount;
                        remainingAmount = 0;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "Insufficient balance.";

                        _logger.LogWarning($"[PAYMENT] Wallet validation failed after split deduction: {result.Message}. NormalWallet={normalWallet.Balance} Remaining={remainingAmount}");

                        return result;
                    }
                }

                student.WalletBalance = fasWallet.Balance + normalWallet.Balance;

                _logger.LogInformation($"[PAYMENT] Payment succeded" +
                        $"FAS={fasWallet.Balance}, BASIC={normalWallet.Balance}, StudentWalletBalance={student.WalletBalance}, Remaining={remainingAmount}");

                StudentWalletTransactionDetail fasDetail = new StudentWalletTransactionDetail
                {
                    Amount = oldFasBalance - fasWallet.Balance,
                    AmountRefunded = 0,
                    Type = WalletType.FAS.ToString()
                };

                StudentWalletTransactionDetail basicDetail = new StudentWalletTransactionDetail
                {
                    Amount = oldNormalBalance - normalWallet.Balance,
                    AmountRefunded = 0,
                    Type = WalletType.BASIC.ToString()
                };




                var transaction = new StudentWalletTransaction
                {
                    Amount = originalAmount,
                    TransactionType = WalletTransactionType.DEBIT.ToString(),
                    StudentId = Payment.StudentId.Value,
                    Description =
                        $"Payment for Order (Invoice={Payment.InvoiceNumber}). " +
                        $"Deducted {originalAmount:C}. " +
                        $"FAS: {oldFasBalance:C} -> {fasWallet.Balance:C}. " +
                        $"Normal: {oldNormalBalance:C} -> {normalWallet.Balance:C}.",
                    CreatedBy = Payment.UserId,
                    UpdatedBy = Payment.UserId,
                    Payment = Payment,
                    Details = new List<StudentWalletTransactionDetail>(),
                };

                transaction.Details.Add(fasDetail);
                transaction.Details.Add(basicDetail);

                await _appContext.StudentWalletTransactions.AddAsync(transaction);

                _appContext.AuditUserActivityType = new AuditUserActivityType
                {
                    GroupId = Common.GenerateUniqueStringId(),
                    ActionName = UserActivityType.PAYMENT_CREATE.ToString(),
                    Remarks = "Payment was created."
                };

                Payment.Status = "SUCCESS";

                f = await AddAsync(Payment);

                resultSaveChange = await _appContext.SaveChangesAsync();

                //result = await this._uow.Students.UpdateAsync(student);
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

                    if (f?.TokenOrders?.Any() ?? false)
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

                    if (f?.MealPlanOrders?.Any() ?? false)
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

                            if (t.StudentGroupId.HasValue && t.ProfileId.HasValue) await CreateOrUpdateStudentGroupDetailAsync(t.StudentGroupId.Value, t.ProfileId.Value, true);
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
