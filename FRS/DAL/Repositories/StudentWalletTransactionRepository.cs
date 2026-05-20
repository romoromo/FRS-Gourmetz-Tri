using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Helpers;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Pkcs;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class StudentWalletTransactionRepository : Repository<StudentWalletTransaction>, IStudentWalletTransactionRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        readonly ILogger _logger;
        private readonly ISqlAppLock _sqlAppLock;
        private readonly IConfiguration _configuration;

        public StudentWalletTransactionRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId, ILogger<StudentWalletTransactionRepository> logger,
            ISqlAppLock sqlAppLock, IConfiguration configuration) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentUserId;
            this._logger = logger;
            this._sqlAppLock = sqlAppLock;
            this._configuration = configuration;
        }

        #region Sieved
        public async Task<PagedEntity<StudentWalletTransaction>> GetWalletTransactionsAsync(EWalletTransactionFilter filter)
        {
            //var cutOffDate = new DateTime(2025, 10, 1);
            //IQueryable<StudentWalletTransaction> query = _appContext.StudentWalletTransactions
            //.Include(m => m.student).ThenInclude(m => m.ClassLevel)
            //.Where(m => m.student.IsActive ||
            //			(!m.student.IsActive && m.student.UpdatedDate >= cutOffDate &&
            //			filter.IncludeOffboarded));
            //.Where(m => m.student.IsActive);

            var cutOffDate = new DateTime(2025, 10, 1);
            IQueryable<StudentWalletTransaction> query = _appContext.StudentWalletTransactions
                    .Include(m => m.student).ThenInclude(m => m.ClassLevel)
                    .Where(m => m.student != null);

            // Filter by StudentType: All / Active / Offboarded
            if (!string.IsNullOrEmpty(filter.StudentType) && filter.StudentType != "All")
            {
                if (filter.StudentType == "Active")
                    query = query.Where(m => m.student.IsActive);
                else if (filter.StudentType == "Offboarded")
                    query = query.Where(m => !m.student.IsActive && m.student.UpdatedDate >= cutOffDate);
			}
			// If StudentType == "All" or null/empty: does not filter IsActive at all			
				
            if (!string.IsNullOrEmpty(filter.PosInvoiceId))
            {
                query = query.Where(m => m.Payment.PosInvoiceId.Contains(filter.PosInvoiceId));
            }

            if (!string.IsNullOrEmpty(filter.Source) && filter.Source != "ALL")
                query = query.Where(m => m.Source == filter.Source);

            if (filter.OutletId != null && filter.OutletId.Count != 0)
            {
                query = query.Where(m => filter.OutletId.Contains(m.student.OutletId.Value));
            }

            if (filter.IsFAS != null)
            {
                query = query.Where(m => m.student.IsFAS == filter.IsFAS);
            }
            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        public async Task<PagedEntity<StudentWalletTransaction>> GetWalletTransactionsSimpleAsync(BaseFilter filter)
        {
            IQueryable<StudentWalletTransaction> query = _appContext.StudentWalletTransactions
                .Where(m => m.student.IsActive)
                .Select(m => new StudentWalletTransaction
                {
                    Id = m.Id,
                    CreatedDate = m.CreatedDate,
                    StudentId = m.StudentId,
                    Amount = m.Amount,
                    TransactionType = m.TransactionType,
                    Description = m.Description,
                    Remarks = m.Remarks,
                    student = new Student
                    {
                        Name = m.student.Name,
                        ClassLevel = new ClassLevel
                        {
                            Name = m.student.ClassLevel.Name,
                            Amount = m.student.ClassLevel.Amount
                        }
                    },
                    WalletPayment = new WalletPayment
                    {
                        fomoid = m.WalletPayment.fomoid
                    },
                    CreatedByUser = new ApplicationUser
                    {
                        UserName = m.CreatedByUser.UserName
                    },

                    Payment = new Payment
                    {
                        PosInvoiceId = m.Payment == null ? string.Empty : m.Payment.PosInvoiceId,
                        TokenOrders = m.Payment.TokenOrders,
                        version = m.Payment.version,
                        InvoiceNumber = m.Payment.InvoiceNumber
                    },
                });

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<StudentWalletTransaction> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(StudentWalletTransaction walletTransaction)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(walletTransaction);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(StudentWalletTransaction walletTransaction)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == walletTransaction.Id);

            var details = walletTransaction.Details.ToList();

            if (details != null)
            {
                details.ForEach(d =>
                {
                    var td = this._appContext.StudentWalletTransactionDetails.FirstOrDefault(x => x.TransactionId == d.TransactionId && x.Type == d.Type);
                    if (td != null)
                    {
                        td.IsActive = true;
                        td.CopyFrom(d);
                        this._appContext.StudentWalletTransactionDetails.Update(td);
                    }
                    else
                    {
                        this._appContext.StudentWalletTransactionDetails.Add(d);
                    }
                });
            }


            f.CopyFrom(walletTransaction);

            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int walletTransactionId)
        {
            var result = new BaseOperationResponse();
            var walletTransaction = await GetSingleOrDefaultAsync(r => r.Id == walletTransactionId);

            if (walletTransaction != null)
                return await Delete(walletTransaction);

            result.IsSuccess = false;
            result.Message = "WalletTransaction not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(StudentWalletTransaction walletTransaction)
        {
            var result = new BaseOperationResponse();
            SoftDelete(walletTransaction);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> StudentWalletTransactionAsync(StudentWalletTransaction dto, int userId)
        {
            var result = new BaseOperationResponse();

            try
            {
                var student = await this._appContext.Students
                .FirstOrDefaultAsync(e => e.IsActive && e.Id == dto.StudentId);

                if (student == null)
                {
                    result.IsSuccess = false;
                    result.Message = $"Student with Id={dto.StudentId} not found or inactive.";
                    return result;
                }

                if (string.IsNullOrEmpty(dto.TransactionType) ||
                    (!dto.TransactionType.Equals(RewardTransactionType.CREDIT.ToString(), StringComparison.OrdinalIgnoreCase) &&
                    !dto.TransactionType.Equals(RewardTransactionType.DEBIT.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    result.IsSuccess = false;
                    result.Message = "Transaction type is missing or invalid.";
                    return result;
                }

                if (dto.TransactionType.Equals(WalletTransactionType.CREDIT.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    student.WalletBalance += dto.Amount;
                }
                else if (dto.TransactionType.Equals(WalletTransactionType.DEBIT.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    if (student.WalletBalance - dto.Amount < 0)
                    {
                        result.IsSuccess = false;
                        result.Message = "Insufficient balance.";
                        return result;
                    }
                    else if (student.IsWalletFreeze)
                    {
                        result.IsSuccess = false;
                        result.Message = "Wallet is Freezed";
                        return result;
                    }
                    else
                    {
                        student.WalletBalance -= dto.Amount;
                    }
                }

                //result = await this._uow.Students.UpdateAsync(student);
                if (result.IsSuccess)
                {
                    var transaction = new StudentWalletTransaction
                    {
                        Amount = dto.Amount,
                        TransactionType = dto.TransactionType,
                        StudentId = dto.StudentId,
                        Description = dto.Description,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };

                    await _appContext.StudentWalletTransactions.AddAsync(transaction);
                }


                await _appContext.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = $"Successfully paid {dto.Amount:D} for students : '{student.Name}'";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error while processing wallet transaction for StudentId={dto.StudentId}. Details: {ex.Message}";
            }

            return result;
        }

        public async Task<BaseOperationResponse> TopupWalletBalanceByStudentGroupIdAsync(
            int studentGroupId,
            double amount,
            int userId,
            WalletType walletTypeData)
        {
            var result = new BaseOperationResponse();

            _logger.LogInformation("Top up by student group. GroupId: {studentGroupId}. Amount: {amount}. UserId: {userId}. WalletType: {walletTypeData}", studentGroupId, amount, userId, walletTypeData);

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid top-up amount: {amount}. Amount must be greater than zero.";
                _logger.LogInformation(result.Message);

                return result;
            }

            string walletType = walletTypeData.ToString();
            if (string.IsNullOrWhiteSpace(walletType))
            {
                result.IsSuccess = false;
                result.Message = "Wallet type must be provided.";
                _logger.LogInformation(result.Message);
                return result;
            }

            var studentGroupData = await _appContext.StudentGroups
                .AsNoTracking()
                .Where(e => e.IsActive && e.Id == studentGroupId)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    StudentIds = x.Sgdetails
                        .Where(d => d.IsActive)
                        .Select(d => d.StudentId)
                })
                .FirstOrDefaultAsync();

            if (studentGroupData == null)
            {
                result.IsSuccess = false;
                result.Message = $"Student Group with Id={studentGroupId} not found or inactive.";
                _logger.LogInformation(result.Message);
                return result;
            }

            var studentIds = studentGroupData.StudentIds.ToList();
            if (!studentIds.Any())
            {
                result.IsSuccess = false;
                result.Message = $"No active students found in Student Group Id={studentGroupData.Id} ({studentGroupData.Name}).";
                _logger.LogInformation(result.Message);
                return result;
            }

            var studentDatas = await _appContext.Students
                .Where(e => e.IsActive && studentIds.Contains(e.Id))
                .ToListAsync();

            if (!studentDatas.Any())
            {
                result.IsSuccess = false;
                result.Message = $"No active students matched in Students table for Student Group Id={studentGroupData.Id}.";
                _logger.LogInformation(result.Message);
                return result;
            }

            var wallets = await _appContext.StudentWallets
                .Where(w => studentIds.Contains(w.StudentId))
                .ToListAsync();
            var transactions = new List<StudentWalletTransaction>();
            foreach (var studentData in studentDatas)
            {
                var studentId = studentData.Id;
                var wallet = wallets.FirstOrDefault(w => w.StudentId == studentId && w.Type == walletType); ;

                double oldBalance = 0;
                if (wallet == null)
                {
                    wallet = new StudentWallet
                    {
                        StudentId = studentId,
                        Balance = amount,
                        Type = walletType,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };
                    await _appContext.StudentWallets.AddAsync(wallet);
                    wallets.Add(wallet);
                }
                else
                {
                    oldBalance = wallet.Balance;
                    wallet.Balance += amount;
                    wallet.UpdatedBy = userId;
                }

                double fasTopup = 0;
                double normalTopup = 0;
                string source = "Others";

                if (walletTypeData == WalletType.BASIC)
                {
                    normalTopup = amount;
                } else if (walletTypeData == WalletType.FAS)
                {
                    fasTopup = amount;
                }



                transactions.Add(new StudentWalletTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = studentId,
                    Description = $"Top-up {walletType} wallet by {amount:C} for student '{studentData.Name}' (ID={studentId}). " +
                                  $"Old Balance: {oldBalance:C}, New Balance: {wallet.Balance:C}, via Group Id={studentGroupData.Id} ({studentGroupData.Name}).",
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    FasTopup = fasTopup,
                    NormalTopup = normalTopup,
                    Source = source
                });

                studentData.WalletBalance = wallets
                    .Where(w => w.StudentId == studentId)
                    .Sum(w => w.Balance);
            }

            await _appContext.StudentWalletTransactions.AddRangeAsync(transactions);
            await _appContext.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = $"Successfully topped up {amount:C} to {studentDatas.Count} students in group '{studentGroupData.Name}'";
            _logger.LogInformation(result.Message);
            return result;
        }

        public async Task<BaseOperationResponse> TopupWalletBalanceByStudentIdAsync(
            int studentId,
            double amount,
            int userId,
            WalletType walletTypeData,
            int? walletPaymentId)
        {
            var result = new BaseOperationResponse();

            _logger.LogInformation("Top up by student Id. StudentId: {studentId}. Amount: {amount}. UserId: {userId}. WalletType: {walletTypeData}", studentId, amount, userId, walletTypeData);


            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid top-up amount: {amount}. Amount must be greater than zero.";
                _logger.LogInformation(result.Message);
                return result;
            }

            string walletType = walletTypeData.ToString();
            if (string.IsNullOrWhiteSpace(walletType))
            {
                result.IsSuccess = false;
                result.Message = "Wallet type must be provided.";
                _logger.LogInformation(result.Message);
                return result;
            }

            try
            {
                var studentData = await _appContext.Students
                    .FirstOrDefaultAsync(e => e.IsActive && e.Id == studentId);

                if (studentData == null)
                {
                    result.IsSuccess = false;
                    result.Message = $"Student with Id={studentId} not found or inactive.";
                    _logger.LogInformation(result.Message);
                    return result;
                }

                var wallet = await _appContext.StudentWallets
                    .FirstOrDefaultAsync(w => w.StudentId == studentId && w.Type == walletType);

                double oldBalance = 0;
                if (wallet == null)
                {
                    wallet = new StudentWallet
                    {
                        StudentId = studentId,
                        Balance = amount,
                        Type = walletType,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };
                    await _appContext.StudentWallets.AddAsync(wallet);
                }
                else
                {
                    oldBalance = wallet.Balance;
                    wallet.Balance += amount;
                    wallet.UpdatedBy = userId;
                }

                double fasTopup = 0;
                double normalTopup = 0;
                string source = "Others";

                if (walletTypeData == WalletType.BASIC)
                {
                    normalTopup = amount;
                }
                else if (walletTypeData == WalletType.FAS)
                {
                    fasTopup = amount;
                }

                StudentWalletTransaction existingPaymentInWalletTransaction = null;

                if (walletPaymentId != null && walletPaymentId != 0)
                {
                    source = "GOe Portal";

                    //check existing payment to prevent double topup for same payment
                    existingPaymentInWalletTransaction = await _appContext.StudentWalletTransactions
                        .FirstOrDefaultAsync(e => e.WalletPaymentId == walletPaymentId);
                }


                if (existingPaymentInWalletTransaction != null)
                {
                    result.IsSuccess = false;
                    result.Message = $"Payment with Id={walletPaymentId} already used for top up.";
                    _logger.LogInformation(result.Message);
                    return result;
                }


                var transaction = new StudentWalletTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = studentId,
                    Description = $"Top-up {walletType} wallet by {amount:C} for student '{studentData.Name}' (ID={studentId}). " +
                                  $"Old Balance: {oldBalance:C}, New Balance: {wallet.Balance:C}.",
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    WalletPaymentId = walletPaymentId,
                    FasTopup = fasTopup,
                    NormalTopup = normalTopup,
                    Source = source
                };

                var totalWalletBalance = await _appContext.StudentWallets
                    .AsNoTracking()
                    .Where(w => w.StudentId == studentId && w.Type != walletType)
                    .SumAsync(w => (double?)w.Balance) ?? 0;
                studentData.WalletBalance = totalWalletBalance + wallet.Balance;

                await _appContext.StudentWalletTransactions.AddAsync(transaction);
                await _appContext.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = $"Successfully topped up {amount:C} to '{walletType}' wallet for student '{studentData.Name}'. " +
                                 $"New balance: {wallet.Balance:C}";
                _logger.LogInformation(result.Message);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error while topping up '{walletType}' wallet for StudentId={studentId}. Details: {ex.Message}";
                _logger.LogInformation(result.Message);
            }

            return result;
        }


        public async Task<BaseOperationResponse> RefundToWalletBalanceAsync(
            int studentId,
            double amount,
            int userId,
            WalletType walletTypeData,
            int? tokenOrderId)
        {
            var result = new BaseOperationResponse();

            _logger.LogInformation("Refund to Wallet Balance. StudentId: {studentId}. Amount: {amount}. UserId: {userId}. WalletType: {walletTypeData}. OrderId: {tokenOrderId}", studentId, amount, userId, walletTypeData, tokenOrderId);


            amount = Math.Round(amount, 2);

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid Refund amount: {amount}. Amount must be greater than zero.";
                _logger.LogInformation(result.Message);
                return result;
            }

            string walletType = walletTypeData.ToString();

            try
            {
                var studentData = await _appContext.Students
                    .FirstOrDefaultAsync(e => e.IsActive && e.Id == studentId);

                if (studentData == null)
                {
                    result.IsSuccess = false;
                    result.Message = $"Student with Id={studentId} not found or inactive.";
                    _logger.LogInformation(result.Message);
                    return result;
                }

                var order = await _appContext.TokenOrders
                    .FirstOrDefaultAsync(e => e.IsActive && e.Id == tokenOrderId);


                var paymentTransaction = await _appContext.StudentWalletTransactions
                    .FirstOrDefaultAsync(e => e.IsActive && e.TransactionType == WalletTransactionType.DEBIT.ToString() && e.PaymentId == order.PaymentId);

                //add old flow back when details is null 

                if (paymentTransaction == null || paymentTransaction.Details == null || !paymentTransaction.Details.Any())
                {

                    if (string.IsNullOrWhiteSpace(walletType))
                    {
                        result.IsSuccess = false;
                        result.Message = "Wallet type must be provided.";
                        _logger.LogInformation(result.Message);
                        return result;
                    }

                    try
                    {
                        var wallet = await _appContext.StudentWallets
                            .FirstOrDefaultAsync(w => w.StudentId == studentId && w.Type == walletType);

                        double oldBalance = 0;
                        if (wallet == null)
                        {
                            wallet = new StudentWallet
                            {
                                StudentId = studentId,
                                Balance = amount,
                                Type = walletType,
                                CreatedBy = userId,
                                UpdatedBy = userId
                            };
                            await _appContext.StudentWallets.AddAsync(wallet);
                        }
                        else
                        {
                            oldBalance = wallet.Balance;
                            wallet.Balance += amount;
                            wallet.UpdatedBy = userId;
                        }

                        var oldTransaction = new StudentWalletTransaction
                        {
                            Amount = amount,
                            TransactionType = WalletTransactionType.CREDIT.ToString(),
                            StudentId = studentId,
                            Description = $"Refund to {walletType} wallet by {amount:C} for student '{studentData.Name}' (ID={studentId}). " +
                                          $"Old Balance: {oldBalance:C}, New Balance: {wallet.Balance:C}.",
                            CreatedBy = userId,
                            UpdatedBy = userId,
                            TokenOrderId = tokenOrderId,
                            NormalTopup = amount,
                            Source = "Others"
                        };

                        var totalWalletBalance = await _appContext.StudentWallets
                            .AsNoTracking()
                            .Where(w => w.StudentId == studentId && w.Type != walletType)
                            .SumAsync(w => (double?)w.Balance) ?? 0;
                        studentData.WalletBalance = totalWalletBalance + wallet.Balance;

                        await _appContext.StudentWalletTransactions.AddAsync(oldTransaction);
                        await _appContext.SaveChangesAsync();

                        result.IsSuccess = true;
                        result.Message = $"Successfully refund to {amount:C} to '{walletType}' wallet for student '{studentData.Name}'. " +
                                         $"New balance: {wallet.Balance:C}";
                        _logger.LogInformation(result.Message);
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = $"Error while Refund '{walletType}' wallet for StudentId={studentId}. Details: {ex.Message}";
                        _logger.LogInformation(result.Message);
                    }

                    return result;
                }

                var fasTrans = paymentTransaction.Details.FirstOrDefault(x => x.Type == WalletType.FAS.ToString());
                var normalTrans = paymentTransaction.Details.FirstOrDefault(x => x.Type == WalletType.BASIC.ToString());

                double fasRefund = 0;
                double normalRefund = 0;

                if (fasTrans != null && normalTrans != null)
                {
                    double remainingAmount = amount;
                    double fasBalance = Math.Round(fasTrans.Amount, 2) - fasTrans.AmountRefunded;
                    double normalBalance = Math.Round(normalTrans.Amount, 2) - normalTrans.AmountRefunded;

                    if (remainingAmount <= fasBalance)
                    {
                        fasTrans.AmountRefunded += remainingAmount;
                        fasRefund += remainingAmount;
                        remainingAmount = 0;
                    }
                    else
                    {
                        remainingAmount -= fasBalance;
                        fasTrans.AmountRefunded = fasTrans.Amount;
                        fasRefund += fasBalance;
                        if (normalBalance > 0)
                        {
                            normalTrans.AmountRefunded += remainingAmount;
                            normalRefund += remainingAmount;
                            remainingAmount = 0;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = "Insufficient payment to refund.";
                            _logger.LogInformation(result.Message + "Fas Trans balance: {fasBalance}. Basic trans balance: {normalBalance}. amount refund: {amount}", fasBalance, normalBalance, amount);
                            return result;
                        }
                    }

                    await this.UpdateAsync(paymentTransaction);
                }
                double oldFasBalance = 0;
                double oldNormalBalance = 0;


                var description = $"Refund to wallet by {amount:C} for student '{studentData.Name}' (ID={studentId}). ";

                var fasWallet = await _appContext.StudentWallets
                .FirstOrDefaultAsync(w => w.StudentId == studentId && w.Type == WalletType.FAS.ToString());

                if (fasRefund > 0)
                {
                    if (fasWallet == null)
                    {
                        fasWallet = new StudentWallet
                        {
                            StudentId = studentId,
                            Balance = amount,
                            Type = WalletType.FAS.ToString(),
                            CreatedBy = userId,
                            UpdatedBy = userId
                        };
                        await _appContext.StudentWallets.AddAsync(fasWallet);
                    }
                    else
                    {
                        oldFasBalance = fasWallet.Balance;
                        fasWallet.Balance += fasRefund;
                        fasWallet.UpdatedBy = userId;
                    }

                    description += $"Old Fas Balance: {oldFasBalance:C}, New Balance: {fasWallet.Balance:C}. ";
                }

                var normalWallet = await _appContext.StudentWallets
                    .FirstOrDefaultAsync(w => w.StudentId == studentId && w.Type == WalletType.BASIC.ToString());

                if (normalRefund > 0)
                {
                    if (normalWallet == null)
                    {
                        normalWallet = new StudentWallet
                        {
                            StudentId = studentId,
                            Balance = amount,
                            Type = WalletType.BASIC.ToString(),
                            CreatedBy = userId,
                            UpdatedBy = userId
                        };
                        await _appContext.StudentWallets.AddAsync(normalWallet);
                    }
                    else
                    {
                        oldNormalBalance = normalWallet.Balance;
                        normalWallet.Balance += normalRefund;
                        normalWallet.UpdatedBy = userId;
                    }

                    description += $"Old Basic Balance: {oldNormalBalance:C}, New Balance: {normalWallet.Balance:C}. ";
                }

                _logger.LogInformation(description);

                var transaction = new StudentWalletTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = studentId,
                    Description = description,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    TokenOrderId = tokenOrderId,
                    NormalTopup = normalRefund,
                    FasTopup = fasRefund,
                    Source = "Others"
                };

                //var totalWalletBalance = await _appContext.StudentWallets
                //    .AsNoTracking()
                //    .Where(w => w.StudentId == studentId && w.Type != walletType)
                //    .SumAsync(w => (double?)w.Balance) ?? 0;

                studentData.WalletBalance = fasWallet.Balance + normalWallet.Balance;

                if (studentData.isNotifCancellationRequestStatus)
                {
                    var notificationSetting = await _appContext.NotificationSettings.FirstOrDefaultAsync(m => m.Type == NotificationSettingType.CANCEL_REQUEST_APPROVED && m.IsActive);
                    if (notificationSetting != null)
                    {
                        var notification = new Notification
                        {
                            Header = notificationSetting.Subject,
                            Body = notificationSetting.Template.Replace("{user}", studentData.Name),
                            IsRead = false,
                            CreatedBy = userId,
                            UpdatedBy = userId
                        };
                        await _appContext.Notifications.AddAsync(notification);
                    }
                }

                await _appContext.StudentWalletTransactions.AddAsync(transaction);
                await _appContext.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = description;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error while Refund wallet for StudentId={studentId}. Details: {ex.Message}";
                _logger.LogInformation(result.Message);
            }

            return result;
        }

        public async Task<BaseOperationResponse> OffBoardingStudent(int studentId, int userId)
        {
            var result = new BaseOperationResponse();

            try
            {
                var studentData = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive && e.Id == studentId);
                if (studentData == null)
                {
                    result.IsSuccess = false;
                    result.Message = $"Student with Id={studentId} not found or is inactive.";
                    _logger.LogInformation(result.Message);
                    return result;
                }

                double basicWalletBalance = 0;
                double fasWalletBalance = 0;

                // Reset Student Wallets to zero for BASIC
                var basicWallet = await _appContext.StudentWallets
                    .FirstOrDefaultAsync(w => w.StudentId == studentId && w.IsActive && w.Type == WalletType.BASIC.ToString());
                if (basicWallet != null)
                {
                    basicWalletBalance = basicWallet.Balance;

                    basicWallet.Balance = 0;
                    _appContext.StudentWallets.Update(basicWallet);
                }

                // Reset Student Wallets to zero for FAS
                var fasWallet = await _appContext.StudentWallets
                    .FirstOrDefaultAsync(w => w.StudentId == studentId && w.IsActive && w.Type == WalletType.FAS.ToString());
                if (fasWallet != null)
                {
                    fasWalletBalance = fasWallet.Balance;

                    fasWallet.Balance = 0;
                    _appContext.StudentWallets.Update(fasWallet);
                }

                var oldWalletBalance = studentData.WalletBalance;
                var oldPointBalance = studentData.PointBalance;

                studentData.WalletBalance = 0;
                studentData.PointBalance = 0;

                var walletTransaction = new StudentWalletTransaction
                {
                    Amount = oldWalletBalance,
                    TransactionType = WalletTransactionType.DEBIT.ToString(),
                    StudentId = studentData.Id,
                    Description = $"Offboarding adjustment by userId={userId}. Previous balance: {oldWalletBalance} (From FAS: {fasWalletBalance}, From BASIC: {basicWalletBalance})",
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    FasExpensed = fasWalletBalance,
                    NormalExpensed = basicWalletBalance,
                    Source = "Others"
                };
                await _appContext.StudentWalletTransactions.AddAsync(walletTransaction);

                var pointTransaction = new StudentPointTransaction
                {
                    Amount = oldPointBalance,
                    TransactionType = WalletTransactionType.DEBIT.ToString(),
                    StudentId = studentData.Id,
                    Description = $"Offboarding adjustment by userId={userId}. Previous points: {oldPointBalance}",
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                await _appContext.StudentPointTransactions.AddAsync(pointTransaction);
                await _appContext.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = $"Student (Id={studentId}, Name={studentData.Name}) successfully offboarded. " +
                                 $"Wallet reset from {oldWalletBalance} to 0, Points reset from {oldPointBalance} to 0.";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error while offboarding Student Id={studentId}. " +
                                 $"Details: {ex.Message}" +
                                 (ex.InnerException != null ? $" | Inner: {ex.InnerException.Message}" : string.Empty);
            }

            return result;
        }

        public async Task<BaseOperationResponse> WalletTransfer(int studentIdFrom, int studentIdTo, double amount, int userId)
        {
            var result = new BaseOperationResponse();

            if (amount <= 0)
            {
                return new BaseOperationResponse
                {
                    IsSuccess = false,
                    Message = "Transfer amount must be greater than zero."
                };
            }

            if (studentIdFrom == studentIdTo)
            {
                return new BaseOperationResponse
                {
                    IsSuccess = false,
                    Message = "Source and destination students must be different."
                };
            }

            var studentSource = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive && e.Id == studentIdFrom);
            if (studentSource == null)
            {
                return new BaseOperationResponse
                {
                    IsSuccess = false,
                    Message = $"Source student (Id={studentIdFrom}) not found or inactive."
                };
            }

            var studentDestination = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive && e.Id == studentIdTo);
            if (studentDestination == null)
            {
                return new BaseOperationResponse
                {
                    IsSuccess = false,
                    Message = $"Destination student (Id={studentIdTo}) not found or inactive."
                };
            }

            using var transaction = await _appContext.Database.BeginTransactionAsync();
            try
            {
                _appContext.AuditUserActivityType = new AuditUserActivityType
                {
                    GroupId = Common.GenerateUniqueStringId(),
                    ActionName = UserActivityType.STUDENT_WALLET_TRANSFER.ToString(),
                    Remarks = $"Transfer {amount} from StudentId {studentIdFrom} to StudentId {studentIdTo}"
                };

                var sourceWalletBalance = await _appContext.StudentWallets.FirstOrDefaultAsync(m => m.StudentId == studentIdFrom && m.IsActive && m.Type == WalletType.BASIC.ToString());
                if (sourceWalletBalance == null || sourceWalletBalance?.Balance < amount)
                {
                    return new BaseOperationResponse
                    {
                        IsSuccess = false,
                        Message = $"Student '{studentSource.Name}' does not have sufficient balance."
                    };
                }

                //Debit from source student
                sourceWalletBalance.Balance -= amount;
                sourceWalletBalance.UpdatedBy = userId;

                studentSource.WalletBalance -= amount;

                //Credit to destination student
                var totalWalletBalance = await _appContext.StudentWallets.Where(m => m.StudentId == studentIdTo && m.IsActive).SumAsync(w => (double?)w.Balance) ?? 0;

                var destinationWallet = await _appContext.StudentWallets.FirstOrDefaultAsync(m => m.StudentId == studentIdTo && m.IsActive && m.Type == WalletType.BASIC.ToString());
                if (destinationWallet == null)
                {
                    destinationWallet = new StudentWallet
                    {
                        StudentId = studentIdTo,
                        Balance = amount,
                        Type = WalletType.BASIC.ToString(),
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };
                    await _appContext.StudentWallets.AddAsync(destinationWallet);
                }
                else
                {
                    destinationWallet.Balance += amount;
                    destinationWallet.UpdatedBy = userId;
                }


                studentDestination.WalletBalance = totalWalletBalance + amount;

                await _appContext.StudentWalletTransactions.AddRangeAsync(
                            new StudentWalletTransaction
                            {
                                StudentId = studentIdFrom,
                                Amount = amount,
                                TransactionType = WalletTransactionType.DEBIT.ToString(),
                                Description = $"Transfer to StudentId {studentIdTo} - amount: {amount}",
                                CreatedBy = userId,
                                NormalExpensed  = amount,
                                Source = "Others"
                            },
                            new StudentWalletTransaction
                            {
                                StudentId = studentIdTo,
                                Amount = amount,
                                TransactionType = WalletTransactionType.CREDIT.ToString(),
                                Description = $"Transfer from StudentId {studentIdFrom} - amount: {amount}",
                                CreatedBy = userId,
                                NormalTopup = amount,
                                Source = "Others"
                            }
                        );
                await _appContext.SaveChangesAsync();
                await transaction.CommitAsync();

                result.IsSuccess = true;
                result.Message = $"Successfully transfer balance {amount} from '{studentIdFrom} - {studentSource.Name}' wallet to student '{studentIdTo} - {studentDestination.Name}'. ";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                result.IsSuccess = false;
                result.Message = $"Error while transfer wallet from StudentId={studentIdFrom} to StudentId={studentIdTo}. Details: {ex.Message}";
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateStudentWalletTransaction(StudentWalletTransaction walletTransaction)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == walletTransaction.Id);

            f.CopyFrom(walletTransaction);

            if (walletTransaction.File != null && !string.IsNullOrEmpty(walletTransaction.File.Path))
            {
                if (!f.FileId.HasValue)
                {
                    f.File = walletTransaction.File;
                }
                else
                {
                    if (f.File == null)
                    {
                        //TODO: check why EF Core is not loading the Icon property; interim solution
                        var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == f.FileId);
                        if (icon == null)
                        {
                            f.File = new Models.File();
                        }
                        else
                        {
                            f.File = icon;
                            f.FileId = icon.Id;
                        }
                    }

                    f.File.Path = walletTransaction.File.Path;
                    f.File.FileName = walletTransaction.File.FileName ?? System.IO.Path.GetFileName(walletTransaction.File.Path);
                }
            }

            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task FASRechargeable(CancellationToken ct = default)
        {
            var serverDateTimeNow = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Asia/Singapore"));
            var day = (int)serverDateTimeNow.DayOfWeek;
            var nowTime = new TimeSpan(serverDateTimeNow.Hour, serverDateTimeNow.Minute, 0);
            var timeHHmm = $"{serverDateTimeNow:HHmm}";

            var classLevels = await _appContext.ClassLevels
                .AsNoTracking()
                .Where(m =>
                    m.IsActive &&
                    m.Outlet.IsActive &&
                    m.FASRechargeable == true &&
                    m.Day != null && (int)m.Day.Value == day &&
                    m.Time != null && m.Time.Value == nowTime &&
                    m.Amount != null && m.Amount > 0)
                .Select(m => new
                {
                    m.Id,
                    Amount = m.Amount!.Value
                })
                .ToListAsync(ct);

            if (classLevels.Count == 0) return;

            _logger.LogInformation($"FAS Rechargeable starting...{serverDateTimeNow}");

            foreach (var classLevel in classLevels)
            {
                await using var lockHandle = await _sqlAppLock.TryAcquireAsync(resource: $"lock:fas:classlevel:{classLevel.Id}", timeoutMs: 0, ct: ct);
                if (lockHandle is null)
                {
                    _logger.LogInformation("Skip ClassLevel {ClassLevelId}: lock held (another server/job running).", classLevel.Id);
                    continue;
                }
                var runKey = $"{serverDateTimeNow:yyyyMMdd}-{timeHHmm}-{classLevel.Id}";

                await using var tx = await _appContext.Database.BeginTransactionAsync(ct);

                try
                {
                    var alreadyRun = await _appContext.Set<FasRunLog>().AnyAsync(x => x.RunKey == runKey, ct);
                    if (alreadyRun)
                    {
                        _logger.LogInformation("Skip ClassLevel {ClassLevelId}: already ran RunKey {RunKey}", classLevel.Id, runKey);
                        await tx.RollbackAsync(ct);
                        continue;
                    }

                    _logger.LogInformation($"Loop Class Level : {classLevel.Id}");

                    var students = _appContext.Students.Where(s => s.IsActive && s.ClassLevelId == classLevel.Id && s.IsFAS);

                    await foreach (var student in students.AsAsyncEnumerable().WithCancellation(ct))
                    {
                        _logger.LogInformation($"Loop Class Level : {classLevel.Id} for Student {student.Name}");

                        _appContext.AuditUserActivityType = new AuditUserActivityType
                        {
                            GroupId = Common.GenerateUniqueStringId(),
                            ActionName = "AUTO DEBIT FAS amount to $0",
                            Remarks = $"Auto Debit FAS amount to $0 {student.Name}"
                        };

                        var fasWallet = await _appContext.StudentWallets
                            .FirstOrDefaultAsync(w => w.StudentId == student.Id && w.IsActive && w.Type == WalletType.FAS.ToString(), ct);
                        if (fasWallet != null)
                        {
                            var oldBalance = fasWallet.Balance;
                            if (oldBalance != 0)
                            {
                                fasWallet.Balance = 0;

                                await _appContext.StudentWalletTransactions.AddAsync(new StudentWalletTransaction
                                {
                                    StudentId = student.Id,
                                    Amount = oldBalance,
                                    TransactionType = WalletTransactionType.DEBIT.ToString(),
                                    Description = $"Auto Debit FAS from {oldBalance} to 0 at {serverDateTimeNow:yyyy-MM-dd HH:mm} for {student.Name}",
                                    CreatedBy = null,
                                    FasExpensed = oldBalance,
                                    Source = "Others"
                                }, ct);

                                _logger.LogInformation($"Loop Class Level : {classLevel.Id} for Student {student.Name}. Auto Debit FAS amount to $0");
                            }

                            await _appContext.StudentWalletTransactions.AddAsync(new StudentWalletTransaction
                            {
                                StudentId = student.Id,
                                Amount = classLevel.Amount,
                                TransactionType = WalletTransactionType.CREDIT.ToString(),
                                Description = $"Auto Credit FAS {classLevel.Amount} at {serverDateTimeNow:yyyy-MM-dd HH:mm} for {student.Name}",
                                CreatedBy = null,
                                FasTopup = classLevel.Amount,
                                Source = "Others"
                            }, ct);

                            fasWallet.Balance = classLevel.Amount;
                            _appContext.StudentWallets.Update(fasWallet);

                            _logger.LogInformation($"Loop Class Level : {classLevel.Id} for Student {student.Name}. Auto Credit FAS amount to ${classLevel.Amount}");
                        }
                        else
                        {
                            var newWallet = new StudentWallet
                            {
                                StudentId = student.Id,
                                Balance = classLevel.Amount,
                                Type = WalletType.FAS.ToString(),
                                CreatedBy = null,
                                UpdatedBy = null
                            };
                            await _appContext.StudentWallets.AddAsync(newWallet, ct);

                            await _appContext.StudentWalletTransactions.AddAsync(new StudentWalletTransaction
                            {
                                StudentId = student.Id,
                                Amount = classLevel.Amount,
                                TransactionType = WalletTransactionType.CREDIT.ToString(),
                                Description = $"Auto Credit FAS {classLevel.Amount} at {serverDateTimeNow:yyyy-MM-dd HH:mm} for {student.Name}",
                                CreatedBy = null,
                                FasTopup = classLevel.Amount,
                                Source = "Others"
                            }, ct);

                            _logger.LogInformation($"Loop Class Level : {classLevel.Id} for Student {student.Name}. Auto Credit FAS amount to ${classLevel.Amount}");
                        }

                        var totalWalletBalance = await _appContext.StudentWallets.FirstOrDefaultAsync(m => m.StudentId == student.Id && m.IsActive && m.Type == WalletType.BASIC.ToString());
                        student.WalletBalance = (totalWalletBalance?.Balance ?? 0) + classLevel.Amount;
                        _appContext.Students.Update(student);
                        _logger.LogInformation($"Loop Class Level : {classLevel.Id} for Student {student.Name}. Total Wallet Balance ${totalWalletBalance}");
                    }

                    _appContext.Set<FasRunLog>().Add(new FasRunLog
                    {
                        ClassLevelId = classLevel.Id,
                        RunKey = runKey,
                        CreatedAt = DateTime.UtcNow
                    });

                    await _appContext.SaveChangesAsync(ct);
                    await tx.CommitAsync(ct);

                    _logger.LogInformation("Finish ClassLevel {ClassLevelId}", classLevel.Id);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync(ct);
                    _logger.LogError(ex, "FAS Rechargeable FAILED for ClassLevel {ClassLevelId}", classLevel.Id);
                }
            }

            _logger.LogInformation("FAS Rechargeable job finished");
        }

        private bool IsValidBasicTopUp(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return false;

            if (description.Contains("Top-up", StringComparison.OrdinalIgnoreCase))
            {
                return description.Contains("BASIC", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public async Task<(double TopUp, double Refund, double Balance, double Redemption)> GetWalletTransactions(int studentId, WalletTransactionType walletTransaction)
        {
            var tx = await _appContext.StudentWalletTransactions
                    .AsNoTracking()
                    .Where(e => e.IsActive
                        && e.StudentId == studentId
                        && e.TransactionType == WalletTransactionType.CREDIT.ToString()
                        && e.Description != null
                        && (e.Description.Contains("Top-up BASIC wallet") || e.Description.Contains("Refund to BASIC")))
                    .Select(e => new { e.Amount, e.Description })
                    .ToListAsync();

            double topUp = tx
                .Where(x => x.Description.Contains("Top-up BASIC wallet"))
                .Sum(x => (double)x.Amount);

            double refund = tx
                .Where(x => x.Description.Contains("Refund to BASIC"))
                .Sum(x => (double)x.Amount);

            double balance = await _appContext.StudentWallets
                .AsNoTracking()
                .Where(w => w.IsActive
                    && w.StudentId == studentId
                    && w.Type == WalletType.BASIC.ToString())
                .Select(w => (double?)w.Balance)
                .FirstOrDefaultAsync() ?? 0d;

            double redemption = (topUp + refund) - balance;

            return (topUp, refund, balance, redemption);
        }

        public async Task<DataTable> GetWalletTransactionForReport(DateTime startDate, DateTime endDate, List<int> outletIds, List<int> classLevelIds, bool isFAS)
        {
            var fromDate = startDate.Date;
            var toExclusive = endDate.Date.AddDays(1);

            var outletFilter = outletIds?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();
            var classLevelFilter = classLevelIds?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();

            var studentsQ = _appContext.Students
                .AsNoTracking()
                .Where(s => s.IsActive && s.IsFAS == isFAS);

            if (outletFilter.Length > 0)
                studentsQ = studentsQ.Where(s => s.OutletId.HasValue && outletFilter.Contains(s.OutletId.Value));

            if (classLevelFilter.Length > 0)
                studentsQ = studentsQ.Where(s => s.ClassLevelId.HasValue && classLevelFilter.Contains(s.ClassLevelId.Value));

            var queryStudentWalletTransactions =
                from tx in _appContext.StudentWalletTransactions.AsNoTracking()
                join s in studentsQ on tx.StudentId equals s.Id
                where tx.IsActive
                      && tx.CreatedDate >= fromDate
                      && tx.CreatedDate < toExclusive
                group tx by new
                {
                    StudentId = s.Id,
                    s.Name,
                    OutletId = s.OutletId ?? 0,
                    OutletName = s.Outlet != null ? s.Outlet.Name : "",
                    ClassLevelId = s.ClassLevelId ?? 0,
                    ClassLevelName = s.ClassLevel != null ? s.ClassLevel.Name : "",
                    ClassId = s.ClassId,
                    ClassName = s.Class != null ? s.Class.Name : "",
                    FAS = s.IsFAS,
                    IsActive = s.IsActive
                }
                into g
                select new
                {
                    g.Key.StudentId,
                    StudentName = g.Key.Name,
                    g.Key.OutletId,
                    g.Key.OutletName,
                    g.Key.ClassLevelId,
                    g.Key.ClassLevelName,
                    g.Key.ClassId,
                    g.Key.ClassName,
                    g.Key.FAS,
                    IsActive = g.Key.IsActive,

                    TotalTopUpNormalAccount =
                        g.Where(x => x.TransactionType == WalletTransactionType.CREDIT.ToString()
                                     && x.Description != null
                                     && x.Description.Contains("Top-up BASIC wallet"))
                         .Sum(x => (double?)x.Amount) ?? 0d,

                    TotalRefundBasic =
                        g.Where(x => x.TransactionType == WalletTransactionType.CREDIT.ToString()
                                    && x.Description != null
                                    && x.Description.Contains("Refund to wallet")
                                    && x.Description.Contains("Old Basic Balance"))
                        .Sum(x => (double?)x.Amount) ?? 0d,


                    //FAS
                    TotalToupupFASAccount =
                        g.Where(x => x.TransactionType == WalletTransactionType.CREDIT.ToString()
                                     && x.Description != null
                                     && (x.Description.Contains("Top-up FAS") || x.Description.Contains("Auto Credit FAS"))
                                     && !x.Description.Contains("Auto Credit FAS 22.4 at 2026-01-17 17:00"))
                         .Sum(x => (double?)x.Amount) ?? 0d,
                    TotalRefundFAS =
                        g.Where(x => x.TransactionType == WalletTransactionType.CREDIT.ToString()
                                    && x.Description != null
                                    && x.Description.Contains("Refund to Wallet"))
                        .Sum(x => (double?)x.Amount) ?? 0d,
                    TotalFASRedemption =
                        g.Where(x => x.TransactionType == WalletTransactionType.DEBIT.ToString()
                                    && x.Description != null
                                    && x.Description.Contains("Payment for Order"))
                        .Sum(x => (double?)x.Amount) ?? 0d,
                };

            var txAgg = await queryStudentWalletTransactions.ToListAsync();

            var balanceQ =
                from w in _appContext.StudentWallets.AsNoTracking()
                join s in studentsQ on w.StudentId equals s.Id
                where w.IsActive //&& w.Type == WalletType.BASIC.ToString()
                select new { w.StudentId, w.Type, Balance = (double?)w.Balance ?? 0d };

            var balances = await balanceQ.ToListAsync();
            var balanceMap = balances
                .GroupBy(x => x.StudentId)
                .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.Type, x => x.Balance));

            var dt = new DataTable("WalletTransactionReport");
            dt.Columns.Add("OutletName", typeof(string));
            dt.Columns.Add("ClassLevelName", typeof(string));
            dt.Columns.Add("StudentId", typeof(int));
            dt.Columns.Add("StudentName", typeof(string));
            dt.Columns.Add("ClassName", typeof(string));
            dt.Columns.Add("TotalTopUpNormalAccount", typeof(double));
            dt.Columns.Add("TotalRedemption", typeof(double));
            dt.Columns.Add("NormalWalletBalance", typeof(double));
            dt.Columns.Add("TotalRefundBasic", typeof(double));

            //FAS
            dt.Columns.Add("IsFAS", typeof(bool));
            dt.Columns.Add("StudentStatus", typeof(string));
            dt.Columns.Add("TotalToupupFASAccount", typeof(double));
            dt.Columns.Add("TotalRefundFAS", typeof(double));
            dt.Columns.Add("TotalFASRedemption", typeof(double));
            dt.Columns.Add("FASWalletBalance", typeof(double));

            foreach (var x in txAgg
                .OrderBy(r => r.OutletName)
                .ThenBy(r => r.ClassLevelName)
                .ThenBy(r => r.ClassName)
                .ThenBy(r => r.StudentName))
            {
                double basicBalance = 0d;
                double fasBalance = 0d;

                if (balanceMap.TryGetValue(x.StudentId, out var walletBalances))
                {
                    walletBalances.TryGetValue(WalletType.BASIC.ToString(), out basicBalance);
                    walletBalances.TryGetValue(WalletType.FAS.ToString(), out fasBalance);
                }

                dt.Rows.Add(
                    x.OutletName ?? "",
                    x.ClassLevelName ?? "",
                    x.StudentId,
                    x.StudentName ?? "",
                    x.ClassName ?? "",
                    x.TotalTopUpNormalAccount,
                    (x.TotalRefundBasic + x.TotalTopUpNormalAccount == 0 ? fasBalance : x.TotalRefundBasic + x.TotalTopUpNormalAccount) - fasBalance,
                    fasBalance,
                    x.TotalRefundBasic,

                    //FAS
                    x.FAS,
                    x.IsActive ? "Active" : "Offboarded",
                    x.TotalToupupFASAccount,
                    x.TotalRefundFAS,
                    x.TotalFASRedemption,
                    fasBalance
                );
            }

            return dt;
        }

        public async Task<PagedEntity<WalletTransactionReportRow>> GetWalletTransactions(WalletTransactionFilter filter)
        {
            var fromDate = filter.startDate.Date;
            var toExclusive = filter.endDate.Date.AddDays(1).AddSeconds(-1);
            if (filter.endDate == DateTime.MinValue)
                toExclusive = DateTime.MaxValue;

            var outletFilter = filter.OutletId?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();
            var classLevelFilter = filter.ClassLevelIds?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();

            /*var studentsQ = _appContext.Students.AsNoTracking()
                .Where(s => s.IsActive &&
                            s.Class.IsActive &&
                            s.ClassLevel.IsActive &&
                            s.Outlet.IsActive);*/

            var cutOffDate = new DateTime(2025, 10, 1);
            var studentsQ = _appContext.Students.AsNoTracking()
                .Where(s => s.Class.IsActive &&
                            s.ClassLevel.IsActive &&
                            s.Outlet.IsActive &&
                            (s.IsActive ||  // Active students always appear
                             (!s.IsActive && s.UpdatedDate >= cutOffDate))); // Offboarded since 1 Oct 2025

            if (outletFilter.Any())
                studentsQ = studentsQ.Where(s => s.OutletId.HasValue && outletFilter.Contains(s.OutletId.Value));

            if (classLevelFilter.Any())
                studentsQ = studentsQ.Where(s => s.ClassLevelId.HasValue && classLevelFilter.Contains(s.ClassLevelId.Value));

            if (filter.StudentId != 0)
                studentsQ = studentsQ.Where(s => s.Id == filter.StudentId);

            if (filter.isFAS.HasValue)
                studentsQ = studentsQ.Where(s => s.IsFAS == filter.isFAS.Value);
			
			// Filter by StudentType: All / Active / Offboarded
            if (!string.IsNullOrEmpty(filter.StudentType) && filter.StudentType != "All")
            {
                if (filter.StudentType == "Active")
                    studentsQ = studentsQ.Where(s => s.IsActive);
                else if (filter.StudentType == "Offboarded")
                    studentsQ = studentsQ.Where(s => !s.IsActive && s.UpdatedDate >= cutOffDate);
            }
            // StudentType == "All": the initial query already includes active + offboarded since cutOffDate

            var totalStudents = await studentsQ.CountAsync();

            var page = filter.Page ?? 1;
            var pageSize = filter.PageSize ?? int.MaxValue;
            var skip = (page - 1) * pageSize;

            var pagedStudents = await studentsQ
                .Include(s => s.Outlet)
                .Include(s => s.Class)
                .Include(s => s.ClassLevel)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var studentIds = pagedStudents.Select(s => s.Id).ToList();

            var transactions = await _appContext.StudentWalletTransactions.AsNoTracking()
                .Where(tx => tx.IsActive &&
                             studentIds.Contains(tx.StudentId) && !tx.Remarks.Contains("VOID") &&
                             tx.CreatedDate >= fromDate &&
                             tx.CreatedDate < toExclusive)
                .ToListAsync();

            var lastTransactionIds = await _appContext.StudentWalletTransactions.AsNoTracking()
                .Where(tx => tx.IsActive &&
                             studentIds.Contains(tx.StudentId) && !tx.Remarks.Contains("VOID") &&
                             tx.CreatedDate < toExclusive)
                .GroupBy(tx => tx.StudentId)
                .Select(g => g.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault())
                .ToListAsync();

            var snapshots = await _appContext.StudentWalletTransactions.AsNoTracking()
                .Where(tx => lastTransactionIds.Contains(tx.Id) && !tx.Remarks.Contains("VOID"))
                .ToListAsync();

            var blacklistDate = new DateTime(2026, 1, 17);
            var hasil = pagedStudents.Select(s =>
            {
                var stTx = transactions.Where(t => t.StudentId == s.Id)
                       //.OrderByDescending(x => x.CreatedDate.Date).ThenBy(m => m.TransactionType)
                       .OrderByDescending(x => x.CreatedDate)
                       .ThenByDescending(x => x.Id).ToList();

                decimal? lastFas = null;
                decimal? lastBasic = null;
                double utilizedWrongFasAmount = 0;

                var auditDate = new DateTime(2026, 1, 17);
                var startTime = auditDate.AddHours(8);  // 08:00:00
                var endTime = auditDate.AddHours(17);   // 17:00:00

                foreach (var tx in stTx)
                {
                    if (tx.Description.Contains("Auto Debit FAS from", StringComparison.OrdinalIgnoreCase) &&
                        tx.Description.Contains("to 0", StringComparison.OrdinalIgnoreCase) || tx.Description.Contains("VOID", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    lastFas ??= WalletDescriptionHelper.GetAfterBalance(tx.Description, "FAS");
                    lastBasic ??= WalletDescriptionHelper.GetAfterBalance(tx.Description, "Basic");
                    if (lastFas != null && lastBasic != null) break;
                }

                if (lastFas == null || lastBasic == null)
                {
                    var snap = snapshots.FirstOrDefault(x => x.StudentId == s.Id);
                    if (snap != null)
                    {
                        lastFas ??= WalletDescriptionHelper.GetAfterBalance(snap.Description, "FAS");
                        lastBasic ??= WalletDescriptionHelper.GetAfterBalance(snap.Description, "Basic");
                    }
                }

                var paymentTx = stTx.Where(x => x.TransactionType == WalletTransactionType.DEBIT.ToString()
                             && WalletDescriptionHelper.IsPaymentForOrder(x.Description)).ToList();

                var paymentTxFAS = paymentTx.ToList();
                var refundTx = stTx.Where(x => x.TransactionType == WalletTransactionType.CREDIT.ToString()
                            && (WalletDescriptionHelper.IsRefundBasic(x.Description)
                                || WalletDescriptionHelper.IsRefundFAS(x.Description))).ToList();
                var reundFas = refundTx.ToList();

                if (s.IsFAS)
                {
                    var auditWindowTx = stTx.Where(x => x.CreatedDate >= startTime && x.CreatedDate <= endTime).ToList();

                    var totalWrongCredit = (double)auditWindowTx
                        .Where(x => x.TransactionType == WalletTransactionType.CREDIT.ToString()
                               && WalletDescriptionHelper.IsTopUpFAS(x.Description))
                        .Sum(x => x.Amount);

                    var totalUsedDebit = (double)auditWindowTx
                        .Where(x => x.TransactionType == WalletTransactionType.DEBIT.ToString()
                               && WalletDescriptionHelper.IsFASPayment(x.Description))
                        .Sum(x => WalletDescriptionHelper.GetDeductedAmount(x.Description, "FAS"));

                    //utilizedWrongFasAmount = Math.Min(totalWrongCredit, totalUsedDebit);
                    utilizedWrongFasAmount = totalUsedDebit;
                }
                else
                {
                    reundFas = refundTx.Where(x => !(x.CreatedDate >= startTime && x.CreatedDate <= endTime)).ToList();
                    paymentTxFAS = paymentTxFAS.Where(x => !(x.CreatedDate >= startTime && x.CreatedDate <= endTime)).ToList();
                }

                return new WalletTransactionReportRow
                {
                    StudentId = s.Id,
                    StudentName = s.Name,
                    OutletName = s.Outlet?.Name ?? "",
                    ClassLevelName = s.ClassLevel?.Name ?? "",
                    ClassName = s.Class?.Name ?? "",
                    IsFAS = s.IsFAS,

                    TotalTopUpBasic = (double)stTx.Where(x => x.TransactionType == WalletTransactionType.CREDIT.ToString()
                                                           && WalletDescriptionHelper.IsTopUpBasic(x.Description)).Sum(x => x.Amount),

                    TotalRefundBasic = (double)refundTx.Sum(x => WalletDescriptionHelper.GetRefundAmount(x.Description, "Basic")),

                    TotalBasicRedemption = (double)paymentTx.Sum(x => WalletDescriptionHelper.GetDeductedAmount(x.Description, "Normal")),
                    BasicWalletBalance = (double)(lastBasic ?? 0m),


                    TotalTopUpFAS = !s.IsFAS ? 0 : (double)stTx.Where(x => x.TransactionType == WalletTransactionType.CREDIT.ToString()
                                                         && WalletDescriptionHelper.IsTopUpFAS(x.Description)).Sum(x => x.Amount),

                    TotalRefundFAS = !s.IsFAS ? 0 : (double)reundFas.Sum(x => WalletDescriptionHelper.GetRefundAmount(x.Description, "FAS")),

                    TotalFASRedemption = !s.IsFAS ? 0 : (double)paymentTxFAS.Sum(x => WalletDescriptionHelper.GetDeductedAmount(x.Description, "FAS")),

                    TotalAutoDebitFAS = !s.IsFAS ? 0 : (double)stTx.Where(x => x.TransactionType == WalletTransactionType.DEBIT.ToString()
                                            && WalletDescriptionHelper.IsAutoDebitFAS(x.Description)).Sum(x => x.Amount),

                    FASWalletBalance = !s.IsFAS ? 0 : (double)(lastFas ?? 0m),
                    WrongTopupFASCredit = !s.IsFAS ? 0 : utilizedWrongFasAmount,
                    StudentStatus = s.IsActive ? "Active" : "Offboarded"
                };
            }).ToList();

            return new PagedEntity<WalletTransactionReportRow>
            {
                TotalCount = totalStudents,
                PagedData = hasil,
                Filter = filter,
                PageSize = pageSize,
                CurrentPage = page,
                PageCount = (int)Math.Ceiling((double)totalStudents / pageSize)
            };
        }

        public async Task<PagedEntity<FASMonthlyBillingReportDTO>> GetFASMonthlyBillingReport(FASMonthlyBillingFilter filter)
        {
            var cutOffDate = new DateTime(2025, 10, 1);
            var txQuery = _appContext.StudentWalletTransactions.AsNoTracking()
                .Where(m => m.IsActive && m.TransactionType == "DEBIT" && m.student.IsFAS && m.PaymentId.HasValue &&
                            (m.student.IsActive ||
                             (!m.student.IsActive && m.student.UpdatedDate >= cutOffDate)));
                //             (!m.student.IsActive && m.student.UpdatedDate >= cutOffDate &&
                //              filter.IncludeOffboarded)));
							  
			// Filter by StudentType: All / Active / Offboarded
            if (!string.IsNullOrEmpty(filter.StudentType) && filter.StudentType != "All")
            {
                if (filter.StudentType == "Active")
                    txQuery = txQuery.Where(m => m.student.IsActive);
                else if (filter.StudentType == "Offboarded")
                    txQuery = txQuery.Where(m => !m.student.IsActive && m.student.UpdatedDate >= cutOffDate);
            }
			else
            {
                // StudentType == "All": force include offboarded without needing to check the checkbox
                txQuery = txQuery.Where(m => m.student.IsActive ||
                            (!m.student.IsActive && m.student.UpdatedDate >= cutOffDate));
            }			

            var outletFilter = filter.OutletId?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();
            var classLevelFilter = filter.ClassLevelIds?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();

            if (outletFilter.Any())
                txQuery = txQuery.Where(m => m.student.OutletId.HasValue && outletFilter.Contains(m.student.OutletId.Value));

            if (classLevelFilter.Any())
                txQuery = txQuery.Where(m => m.student.ClassLevelId.HasValue && classLevelFilter.Contains(m.student.ClassLevelId.Value));

            var fromDate = filter.StartDate.Date;
            var toExclusive = filter.EndDate.Date.AddDays(1).AddSeconds(-1);

            var query = from walletTx in txQuery

                        join payment in _appContext.Payments.AsNoTracking()
                            on walletTx.PaymentId equals payment.Id into paymentGroup
                        from currentPayment in paymentGroup.DefaultIfEmpty()

                        join order in _appContext.TokenOrders.AsNoTracking().Where(m => m.Status == "paid")
                            on currentPayment.Id equals order.PaymentId into orderGroup
                        from currentOrder in orderGroup.DefaultIfEmpty()

                        join orderItem in _appContext.TokenOrdereds.AsNoTracking()
                            on currentOrder.Id equals orderItem.OrderId into itemGroup
                        from currentItem in itemGroup.DefaultIfEmpty()

                        join orderDish in _appContext.TokenOrderDishes.AsNoTracking()
                            on currentItem.Id equals orderDish.TokenOrderedId into dishGroup
                        from currentDish in dishGroup.DefaultIfEmpty()

                        join pos in _appContext.POSSales.AsNoTracking()
                             on currentPayment.Id equals pos.PaymentID into posGroup
                        from currentPos in posGroup.DefaultIfEmpty()

                        join posItem in _appContext.POSSalesItem.AsNoTracking()
                            on currentPos.id_penjualan equals posItem.id_penjualan into posItemGroup
                        from currentPosItem in posItemGroup.DefaultIfEmpty()

                        let calculatedDeliveryDate = currentOrder != null ? currentOrder.DeliveryDate :
                                         (currentPos != null ? currentPos.tgl_penjualan : DateTime.MinValue)

                        where (calculatedDeliveryDate >= fromDate || fromDate == DateTime.MinValue) &&
                              (calculatedDeliveryDate <= toExclusive || toExclusive == DateTime.MaxValue) &&
                              (currentOrder != null || currentPos != null)
                        select new FASMonthlyBillingReportDTO
                        {
                            ID = walletTx.Id,
                            StudentID = walletTx.StudentId,
                            StudentName = walletTx.student.Name,
                            OutletID = walletTx.student.OutletId,
                            OutletName = walletTx.student.Outlet.Name,
                            ClasslevelID = walletTx.student.ClassLevelId ?? 0,
                            ClassLevel = walletTx.student.ClassLevel != null ? walletTx.student.ClassLevel.Name : "",
                            ClassID = walletTx.student.ClassId,
                            Class = walletTx.student.Class != null ? walletTx.student.Class.Name : "",
                            FASStudent = walletTx.student.IsFAS,
                            DeliveryDate = currentOrder != null ? currentOrder.DeliveryDate : (currentPos != null ? currentPos.tgl_penjualan : DateTime.MinValue),

                            MealType = currentItem != null ? currentItem.TokenDesc : "POS Sale",
                            MealName = currentDish != null ? currentDish.Dish.Label : (currentPosItem != null ? currentPosItem.nama_barang : ""),
                            Qty = currentItem != null ? currentItem.Qty.ToString("N", CultureInfo.InvariantCulture) : (currentPosItem != null ? currentPosItem.qty : ""),
                            Price = currentOrder != null ? currentOrder.TotalAmount.ToString("N", CultureInfo.InvariantCulture) : (currentPosItem != null ? currentPosItem.harga_satuan : ""),
                            InvoiceNumber = currentPayment != null ? currentPayment.InvoiceNumber : "",
                            POSInvoiceNumber = currentPayment != null ? (currentPayment.PosInvoiceId ?? (currentPos != null ? currentPos.no_invoice : "")) : "",
                            CollectionTime = currentOrder != null ? currentOrder.CollectionTime : (currentPos != null ? currentPos.tgl_penjualan : null),
                            IsStudentActive = walletTx.student.IsActive,
                        };

            var total = await query.CountAsync();

            var page = filter.Page ?? 1;
            var pageSize = filter.PageSize ?? int.MaxValue;
            var skip = (page - 1) * pageSize;

            var queryPaged = await query
                .OrderByDescending(m => m.OutletName).ThenBy(m => m.StudentName)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedEntity<FASMonthlyBillingReportDTO>()
            {
                TotalCount = total,
                PagedData = queryPaged,
                Filter = filter,
                CurrentPage = page,
                PageCount = (int)Math.Ceiling((double)total / pageSize)
            };
        }


        public async Task SyncWithPOSSales(ILogger logger, CancellationToken ct = default)
        {
            var syncDate = DateTime.Now;
            logger.LogInformation(">>> [SYNC START] SyncWithPOSSales started at {syncDate}", syncDate);

            // 1. Configuration Check
            var posSalesURL = _configuration["AppSettings:POS_URL"];
            if (string.IsNullOrEmpty(posSalesURL))
            {
                logger.LogError("!!! [CONFIG ERROR] AppSettings:POS_URL is missing in appsettings.json. Aborting sync.");
                return;
            }
            logger.LogInformation(">>> [CONFIG] POS_URL retrieved: {posSalesURL}", posSalesURL);

            // 2. Fetch Local Transactions (Unsynced)
            logger.LogInformation("--- [STEP 1] Querying local Wallet transactions that have not been synced...");
            var startCorrectTransaction = new DateTime(2026, 1, 9);

            var queryData = await (
                            from walletTx in _appContext.StudentWalletTransactions.AsNoTracking()
                            where walletTx.IsActive &&
                                walletTx.TransactionType == "DEBIT" &&
                                walletTx.CreatedDate >= startCorrectTransaction &&
                                walletTx.PaymentId != null

                            join payment in _appContext.Payments.AsNoTracking()
                                on walletTx.PaymentId equals payment.Id

                            join pos in _appContext.POSSales.AsNoTracking()
                                on payment.Id equals pos.PaymentID into posGroup
                            from posSale in posGroup.DefaultIfEmpty()
                            where posSale == null &&
                                  payment.version == "SUCCESS" &&
                                  walletTx.IsActive &&
                                  walletTx.Remarks != "VOID" &&
                                  walletTx.TransactionType == "DEBIT" &&
                                  walletTx.CreatedDate >= startCorrectTransaction
                            orderby walletTx.CreatedDate descending
                            select new
                            {
                                walletTx.StudentId,
                                walletTx.Amount,
                                walletTx.CreatedDate,
                                payment.Id
                            }).Take(500).ToListAsync(ct);

            logger.LogInformation(">>> [LOCAL DATA] Found {count} local transactions eligible for syncing.", queryData.Count);

            if (!queryData.Any())
            {
                logger.LogInformation(">>> [SYNC END] No pending transactions found. Job finished.");
                return;
            }

            // Prepare metadata for API request
            var paymentIds = queryData.Select(q => q.Id).ToList();
            var paymentsToUpdateMap = await _appContext.Payments
                .Where(p => paymentIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, ct);

            var studentIDs = queryData.Select(q => q.StudentId).Distinct().ToList();
            var startDate = queryData.Min(q => q.CreatedDate);
            var endDate = queryData.Max(q => q.CreatedDate);

            logger.LogInformation("--- [STEP 2] Processing {studentCount} distinct students for date range {start} to {end}",
                studentIDs.Count, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            // 3. API Integration
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
            var studentBatches = studentIDs.Chunk(50);
            int batchCounter = 1;

            foreach (var batch in studentBatches)
            {
                logger.LogInformation("--- [BATCH {batch}] Requesting external POS data...", batchCounter++);
                string url = $"{posSalesURL}/POS/anon_api/ajaxGetWalletSalesByDate?dateStart={startDate.Date:yyyy-MM-dd}&dateEnd={endDate.Date:yyyy-MM-dd}&StudentId[]={string.Join("&StudentId[]=", batch)}";

                var response = await httpClient.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("!!! [API WARNING] Batch failed. HTTP Status: {code}. URL: {url}", response.StatusCode, url);
                    continue;
                }

                var content = await response.Content.ReadAsStringAsync();
                var posResponse = JsonConvert.DeserializeObject<POSResponse>(content);

                if (posResponse?.data?.sales == null || !posResponse.data.sales.Any())
                {
                    logger.LogInformation("--- [API INFO] No sales data returned from API for this batch.");
                    continue;
                }

                logger.LogInformation(">>> [API DATA] Received {count} sales records from POS API.", posResponse.data.sales.Count);

                // 4. In-Memory Matching Logic
                var salesToInsert = new List<POSSales>();
                var salesLookup = posResponse.data.sales
                    .Select(s =>
                    {
                        decimal.TryParse(s.sub_total, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amt);
                        return new { Original = s, Amt = amt };
                    })
                    .ToLookup(x => new { x.Original.student_id, x.Amt, Tgl = x.Original.tgl_penjualan.Date });

                // Optimization: Fetch existing id_penjualan once per batch to avoid duplicates
                var incomingIdPenjualans = posResponse.data.sales.Select(s => s.id_penjualan).ToList();
                var alreadyInDb = await _appContext.POSSales
                    .AsNoTracking()
                    .Where(s => incomingIdPenjualans.Contains(s.id_penjualan))
                    .Select(s => s.id_penjualan).ToListAsync();

                var usedIdInThisJob = new HashSet<int>();
                int matchCount = 0;

                foreach (var local in queryData)
                {
                    var key = new { student_id = local.StudentId, Amt = (decimal)local.Amount, Tgl = local.CreatedDate.Date };

                    // Deduplication logic: Ensure the record isn't in DB and hasn't been used in this batch
                    var match = salesLookup[key]
                        .Select(m => m.Original)
                        .FirstOrDefault(m => !alreadyInDb.Contains(m.id_penjualan) && !usedIdInThisJob.Contains(m.id_penjualan));

                    if (match != null)
                    {
                        matchCount++;
                        usedIdInThisJob.Add(match.id_penjualan);

                        var newSale = new POSSales
                        {
                            // Copy identity properties
                            id_penjualan = match.id_penjualan,
                            no_invoice = match.no_invoice,
                            student_id = match.student_id,
                            card_serial_number = match.card_serial_number,
                            id_jenis_harga = match.id_jenis_harga,
                            id_pos = match.id_pos,
                            jenis_bayar = match.jenis_bayar,
                            neto = match.neto,
                            total_bayar = match.total_bayar,
                            total_qty = match.total_qty,
                            id_gudang = match.id_gudang,
                            outlet_name = match.outlet_name,
                            tgl_invoice = match.tgl_invoice,
                            tgl_penjualan = match.tgl_penjualan,
                            sub_total = match.sub_total,
                            nama_customer = match.nama_customer,

                            // Link to local system
                            PaymentID = local.Id,
                            SyncDate = syncDate,

                            // Map items
                            sales_items = match.sales_items.Select(i => new Models.SalesItem
                            {
                                id_barang = i.kode_barang,
                                nama_barang = i.nama_barang,
                                qty = i.qty,
                                harga_satuan = i.harga_satuan,
                                harga_total = i.harga_total,
                                deskripsi = i.deskripsi,
                                diskon = i.diskon,
                                id_penjualan = i.id_penjualan,
                                id_penjualan_detail = i.id_penjualan_detail,
                                kode_barang = i.kode_barang
                            }).ToList()
                        };

                        salesToInsert.Add(newSale);

                        // Update payment mapping
                        if (paymentsToUpdateMap.TryGetValue(local.Id, out var p))
                        {
                            p.PosInvoiceId = match.no_invoice;
                        }
                    }
                }

                logger.LogInformation("--- [MATCHING] Successfully matched {matchCount} out of {localCount} local records in this batch.",
                    matchCount, queryData.Count);

                // 5. Persistence
                if (salesToInsert.Any())
                {
                    using var transaction = await _appContext.Database.BeginTransactionAsync(ct);
                    try
                    {
                        logger.LogInformation("--- [DB SAVE] Bulk inserting {count} POSSales records...", salesToInsert.Count);

                        await _appContext.POSSales.AddRangeAsync(salesToInsert, ct);

                        // Save only tracked payments that were actually matched
                        await _appContext.SaveChangesAsync(ct);
                        await transaction.CommitAsync(ct);

                        logger.LogInformation(">>> [SUCCESS] Database transaction committed successfully.");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(ct);
                        logger.LogError(ex, "!!! [DB ERROR] Failed to save sync batch. Error: {msg}", ex.Message);
                    }
                }

                // Clean up tracker to prevent memory bloat during large batches
                _appContext.ChangeTracker.Clear();
            }

            _logger.LogInformation(">>> [SYNC COMPLETED] Job finished successfully at {time}", DateTime.Now);
        }

        public object GetInvoiceDetail(int id)
        {
            var rawData = (from walletTx in _appContext.StudentWalletTransactions.AsNoTracking()
                           where walletTx.Id == id
                           join payment in _appContext.Payments.AsNoTracking() on walletTx.PaymentId equals payment.Id into paymentGroup
                           from currentPayment in paymentGroup.DefaultIfEmpty()
                           join order in _appContext.TokenOrders.AsNoTracking() on currentPayment.Id equals order.PaymentId into orderGroup
                           from currentOrder in orderGroup.DefaultIfEmpty()
                           join orderItem in _appContext.TokenOrdereds.AsNoTracking() on currentOrder.Id equals orderItem.OrderId into itemGroup
                           from currentItem in itemGroup.DefaultIfEmpty()
                           join orderDish in _appContext.TokenOrderDishes.AsNoTracking() on currentItem.Id equals orderDish.TokenOrderedId into dishGroup
                           from currentDish in dishGroup.DefaultIfEmpty()
                           select new
                           {
                               walletTx.student.Name,
                               currentPayment.InvoiceNumber,
                               currentPayment.CreatedDate,
                               currentPayment.total,
                               OutletName = walletTx.student.Outlet.Name,
                               DishLabel = currentDish != null ? currentDish.Dish.Label : null,
                               Qty = currentDish != null ? currentDish.Qty : 0,
                               TotalAmount = currentOrder != null ? currentOrder.TotalAmount : 0,
                               DeliveryDate = currentOrder != null ? currentOrder.DeliveryDate : DateTime.MinValue,
                           }).ToList();

            if (!rawData.Any()) return new InvoiceResponseDto { Data = new InvoiceDataDto { Sales = null, SalesItems = new List<Core.DTO.SalesItem>() }, Error = false };

            var header = rawData.First();
            return new InvoiceResponseDto
            {
                Error = false,
                Data = new InvoiceDataDto
                {
                    Sales = new POSSalesDTO
                    {
                        nama_customer = header.Name,
                        no_invoice = header.InvoiceNumber,
                        tgl_penjualan = header.CreatedDate,
                        outlet_name = header.OutletName,
                        sub_total = header.total.ToString(),
                        neto = header.total.ToString()
                    },
                    SalesItems = rawData.Where(x => x.DishLabel != null).Select(x => new Core.DTO.SalesItem
                    {
                        delivery_date = header.DeliveryDate == DateTime.MinValue ? null : header.DeliveryDate.ToString("dd/MM/yyyy HH:mm:ss"),
                        nama_barang = x.DishLabel,
                        qty = x.Qty.Value.ToString(),
                        harga_satuan = x.TotalAmount.ToString(),
                        diskon = "0.00",
                        harga_total = (x.Qty * x.TotalAmount).ToString()
                    }).ToList()
                }
            };
        }

        public async Task<PagedEntity<DetailedBasicWalletTopUpReport>> GetDetailedBasicWalletTopUpReport(DetailedBasicWalletTopUpFilter filter)
        {
            var outletFilter = filter.OutletId?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();
            var classLevelFilter = filter.ClassLevelIds?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();

            var txQuery = _appContext.StudentWalletTransactions.AsNoTracking()
                .Where(m => m.IsActive && m.TransactionType == "CREDIT" && m.Description.Contains("Top-up BASIC wallet"));

            if (outletFilter.Any())
                txQuery = txQuery.Where(m => m.student.OutletId.HasValue && outletFilter.Contains(m.student.OutletId.Value));

            if (classLevelFilter.Any())
                txQuery = txQuery.Where(m => m.student.ClassLevelId.HasValue && classLevelFilter.Contains(m.student.ClassLevelId.Value));

            var fromDate = filter.StartDate.Date;
            var toExclusive = filter.EndDate.Date.AddDays(1).AddSeconds(-1);
            txQuery = txQuery.Where(m => m.CreatedDate >= fromDate && m.CreatedDate <= toExclusive);

            var query = from walletTx in txQuery
                        join walletPay in _appContext.WalletPayments.AsNoTracking()
                            on walletTx.WalletPaymentId equals walletPay.Id
                        orderby walletTx.CreatedDate descending
                        select new DetailedBasicWalletTopUpReport
                        {
                            ID = walletTx.Id,
                            TransactionDate = walletTx.CreatedDate,
                            StudentId = walletTx.StudentId,
                            StudentName = walletTx.student.Name,
                            Outlet = walletTx.student.Outlet.Name,
                            ClassLevel = walletTx.student.ClassLevel.Name,
                            Amount = walletTx.Amount,
                            Source = walletTx.Source,
                            RefId = walletPay != null ? walletPay.fomoid : null,
                            ProcessedBy = walletTx.CreatedByUser.Email
                        };

            var total = await query.CountAsync();

            var page = filter.Page ?? 1;
            var pageSize = filter.PageSize ?? int.MaxValue;
            var skip = (page - 1) * pageSize;

            var queryPaged = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedEntity<DetailedBasicWalletTopUpReport>()
            {
                TotalCount = total,
                PagedData = queryPaged,
                Filter = filter,
                CurrentPage = page,
                PageCount = (int)Math.Ceiling((double)total / pageSize)
            };
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}