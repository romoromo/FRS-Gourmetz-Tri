using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using System;

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

        public async Task<BaseOperationResponse> TopupWalletBalanceByStudentGroupIdAsync(int studentGroupId, double amount, int userId)
        {
            var result = new BaseOperationResponse();

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid top-up amount: {amount}. Amount must be greater than zero.";
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

            var studentData = await _appContext.Students
                .Where(e => e.IsActive && studentIds.Contains(e.Id))
                .ToListAsync();

            if (!studentData.Any())
            {
                result.IsSuccess = false;
                result.Message = $"No active students matched in Students table for Student Group Id={studentGroupData.Id}.";
                return result;
            }

            foreach (var student in studentData)
            {
                student.WalletBalance += amount;

                var transaction = new StudentWalletTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = student.Id,
                    Description = $"Top-up by Student Group Id={studentGroupData.Id}, Group Name={studentGroupData.Name}",
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                await _appContext.StudentWalletTransactions.AddAsync(transaction);
            }

            await _appContext.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = $"Successfully topped up {amount:C} to {studentData.Count} students in group '{studentGroupData.Name}'";
            return result;
        }

        public async Task<BaseOperationResponse> TopupWalletBalanceByStudentIdAsync(int studentId, double amount, int userId)
        {
            var result = new BaseOperationResponse();

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid top-up amount: {amount}. Amount must be greater than zero.";
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

                var oldBalance = studentData.WalletBalance;
                studentData.WalletBalance += amount;

                var transaction = new StudentWalletTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = studentData.Id,
                    Description = $"Top-up by Student Id={studentData.Id}, Name={studentData.Name}",
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                await _appContext.StudentWalletTransactions.AddAsync(transaction);
                await _appContext.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = $"Successfully topped up {amount:C} for students : '{studentData.Name}'";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error while topping up wallet for StudentId={studentId}. Details: {ex.Message}";
            }

            return result;
        }



        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
