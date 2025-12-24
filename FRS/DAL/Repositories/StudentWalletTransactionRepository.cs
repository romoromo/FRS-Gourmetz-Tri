using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class StudentWalletTransactionRepository : Repository<StudentWalletTransaction>, IStudentWalletTransactionRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public StudentWalletTransactionRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<StudentWalletTransaction>> GetWalletTransactionsAsync(BaseFilter filter)
        {
            IQueryable<StudentWalletTransaction> query = _appContext.StudentWalletTransactions;

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

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid top-up amount: {amount}. Amount must be greater than zero.";
                return result;
            }

            string walletType = walletTypeData.ToString();
            if (string.IsNullOrWhiteSpace(walletType))
            {
                result.IsSuccess = false;
                result.Message = "Wallet type must be provided.";
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
                return result;
            }

            var studentIds = studentGroupData.StudentIds.ToList();
            if (!studentIds.Any())
            {
                result.IsSuccess = false;
                result.Message = $"No active students found in Student Group Id={studentGroupData.Id} ({studentGroupData.Name}).";
                return result;
            }

            var studentDatas = await _appContext.Students
                .Where(e => e.IsActive && studentIds.Contains(e.Id))
                .ToListAsync();

            if (!studentDatas.Any())
            {
                result.IsSuccess = false;
                result.Message = $"No active students matched in Students table for Student Group Id={studentGroupData.Id}.";
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

                transactions.Add(new StudentWalletTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = studentId,
                    Description = $"Top-up {walletType} wallet by {amount:C} for student '{studentData.Name}' (ID={studentId}). " +
                                  $"Old Balance: {oldBalance:C}, New Balance: {wallet.Balance:C}, via Group Id={studentGroupData.Id} ({studentGroupData.Name}).",
                    CreatedBy = userId,
                    UpdatedBy = userId
                });

                studentData.WalletBalance = wallets
                    .Where(w => w.StudentId == studentId)
                    .Sum(w => w.Balance);
            }

            await _appContext.StudentWalletTransactions.AddRangeAsync(transactions);
            await _appContext.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = $"Successfully topped up {amount:C} to {studentDatas.Count} students in group '{studentGroupData.Name}'";
            return result;
        }

        public async Task<BaseOperationResponse> TopupWalletBalanceByStudentIdAsync(
            int studentId,
            double amount,
            int userId,
            WalletType walletTypeData)
        {
            var result = new BaseOperationResponse();

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid top-up amount: {amount}. Amount must be greater than zero.";
                return result;
            }

            string walletType = walletTypeData.ToString();
            if (string.IsNullOrWhiteSpace(walletType))
            {
                result.IsSuccess = false;
                result.Message = "Wallet type must be provided.";
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

                var transaction = new StudentWalletTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = studentId,
                    Description = $"Top-up {walletType} wallet by {amount:C} for student '{studentData.Name}' (ID={studentId}). " +
                                  $"Old Balance: {oldBalance:C}, New Balance: {wallet.Balance:C}.",
                    CreatedBy = userId,
                    UpdatedBy = userId
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
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error while topping up '{walletType}' wallet for StudentId={studentId}. Details: {ex.Message}";
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

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid Refund amount: {amount}. Amount must be greater than zero.";
                return result;
            }

            string walletType = walletTypeData.ToString();
            //if (string.IsNullOrWhiteSpace(walletType))
            //{
            //    result.IsSuccess = false;
            //    result.Message = "Wallet type must be provided.";
            //    return result;
            //}

            try
            {
                var studentData = await _appContext.Students
                    .FirstOrDefaultAsync(e => e.IsActive && e.Id == studentId);

                if (studentData == null)
                {
                    result.IsSuccess = false;
                    result.Message = $"Student with Id={studentId} not found or inactive.";
                    return result;
                }

                var order = await _appContext.TokenOrders
                    .FirstOrDefaultAsync(e => e.IsActive && e.Id == tokenOrderId);


                var paymentTransaction = await _appContext.StudentWalletTransactions
                    .FirstOrDefaultAsync(e => e.IsActive && e.TransactionType == WalletTransactionType.DEBIT.ToString() && e.PaymentId == order.PaymentId);

                //

                var fasTrans = paymentTransaction.Details.FirstOrDefault(x => x.Type == WalletType.FAS.ToString());
                var normalTrans = paymentTransaction.Details.FirstOrDefault(x => x.Type == WalletType.BASIC.ToString());

                double remainingAmount = amount;
                double fasBalance = fasTrans.Amount - fasTrans.AmountRefunded;
                double normalBalance = normalTrans.Amount - normalTrans.AmountRefunded;

                double fasRefund = 0;
                double normalRefund = 0;


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
                    if (normalBalance >= remainingAmount)
                    {
                        normalTrans.AmountRefunded += remainingAmount;
                        normalRefund += remainingAmount;
                        remainingAmount = 0;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "Insufficient payment to refund.";
                        return result;
                    }
                }

                await this.UpdateAsync(paymentTransaction);

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

                var transaction = new StudentWalletTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = studentId,
                    Description = description,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    TokenOrderId = tokenOrderId
                };

                //var totalWalletBalance = await _appContext.StudentWallets
                //    .AsNoTracking()
                //    .Where(w => w.StudentId == studentId && w.Type != walletType)
                //    .SumAsync(w => (double?)w.Balance) ?? 0;

                studentData.WalletBalance = fasWallet.Balance + normalWallet.Balance;

                await _appContext.StudentWalletTransactions.AddAsync(transaction);
                await _appContext.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = description;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error while Refund wallet for StudentId={studentId}. Details: {ex.Message}";
            }

            return result;
        }

        public async Task<BaseOperationResponse> OffBoardingStudent(int studentId, int userId)
        {
            var result = new BaseOperationResponse();

            try
            {
                var studentData = await _appContext.Students
                    .FirstOrDefaultAsync(e => e.IsActive && e.Id == studentId);

                if (studentData == null)
                {
                    result.IsSuccess = false;
                    result.Message = $"Student with Id={studentId} not found or is inactive.";
                    return result;
                }

                var oldWalletBalance = studentData.WalletBalance;
                var oldPointBalance = studentData.PointBalance;

                studentData.WalletBalance = 0;
                studentData.PointBalance = 0;

                var walletTransaction = new StudentWalletTransaction
                {
                    Amount = -oldWalletBalance,
                    TransactionType = WalletTransactionType.DEBIT.ToString(),
                    StudentId = studentData.Id,
                    Description = $"Offboarding adjustment by userId={userId}. Previous balance: {oldWalletBalance}",
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                await _appContext.StudentWalletTransactions.AddAsync(walletTransaction);

                var pointTransaction = new StudentPointTransaction
                {
                    Amount = -oldPointBalance,
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
                result.IsSuccess = false;
                result.Message = $"Invalid top-up amount: {amount}. Amount must be greater than zero.";
                return result;
            }

            if (studentIdFrom == studentIdTo)
            {
                result.IsSuccess = false;
                result.Message = "Source and destination students must be different.";
                return result;
            }

            var studentSource = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive && e.Id == studentIdFrom);
            if (studentSource == null)
            {
                result.IsSuccess = false;
                result.Message = $"Student with Id={studentIdFrom} not found or inactive.";
                return result;
            }

            var studentDestination = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive && e.Id == studentIdTo);
            if (studentDestination == null)
            {
                result.IsSuccess = false;
                result.Message = $"Student with Id={studentIdTo} not found or inactive.";
                return result;
            }

            var sourceWalletBalance = await _appContext.StudentWallets.FirstOrDefaultAsync(m => m.StudentId == studentIdFrom && m.IsActive && m.Type == WalletType.BASIC.ToString());
            if (sourceWalletBalance == null || sourceWalletBalance?.Balance < amount)
            {
                result.IsSuccess = false;
                result.Message = $"Student with Id={studentIdFrom} does not have enough balance.";
                return result;
            }

            using var transaction = await _appContext.Database.BeginTransactionAsync();
            try
            {
                //Debit from source student
                sourceWalletBalance.Balance -= amount;
                studentSource.WalletBalance -= amount;

                //Credit to destination student
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

                var totalWalletBalance = await _appContext.StudentWallets
                    .AsNoTracking()
                    .Where(w => w.StudentId == studentIdTo)
                    .SumAsync(w => (double?)w.Balance) ?? 0;

                studentDestination.WalletBalance = totalWalletBalance;


                await _appContext.StudentWalletTransactions.AddRangeAsync(
                            new StudentWalletTransaction
                            {
                                StudentId = studentIdFrom,
                                Amount = amount,
                                TransactionType = WalletTransactionType.DEBIT.ToString(),
                                Description = $"Transfer to StudentId {studentIdTo} - amount: {amount: C}",
                                CreatedBy = userId
                            },
                            new StudentWalletTransaction
                            {
                                StudentId = studentIdTo,
                                Amount = amount,
                                TransactionType = WalletTransactionType.CREDIT.ToString(),
                                Description = $"Transfer from StudentId {studentIdFrom} - amount: {amount: C}",
                                CreatedBy = userId
                            }
                        );
                await _appContext.SaveChangesAsync();
                await transaction.CommitAsync();

                result.IsSuccess = true;
                result.Message = $"Successfully transfer balance {amount:C} from '{studentIdFrom} - {studentSource.Name}' wallet to student '{studentIdFrom} - {studentDestination.Name}'. ";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                result.IsSuccess = false;
                result.Message = $"Error while transfer wallet for StudentId={studentIdFrom} to StudentId={studentIdTo}. Details: {ex.Message}";
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
