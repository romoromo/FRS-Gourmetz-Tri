using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;

namespace DAL.Repositories
{
    public class StudentPointTransactionRepository : Repository<StudentPointTransaction>, IStudentPointTransactionRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public StudentPointTransactionRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<StudentPointTransaction>> GetPointTransactionsAsync(BaseFilter filter)
        {
            IQueryable<StudentPointTransaction> query = _appContext.StudentPointTransactions;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<StudentPointTransaction> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(StudentPointTransaction pointTransaction)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(pointTransaction);
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

        public async Task<BaseOperationResponse> UpdateAsync(StudentPointTransaction pointTransaction)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == pointTransaction.Id);

            f.CopyFrom(pointTransaction);

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


        public async Task<BaseOperationResponse> DeleteAsync(int pointTransactionId)
        {
            var result = new BaseOperationResponse();
            var pointTransaction = await GetSingleOrDefaultAsync(r => r.Id == pointTransactionId);

            if (pointTransaction != null)
                return await Delete(pointTransaction);

            result.IsSuccess = false;
            result.Message = "Point Transaction not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(StudentPointTransaction pointTransaction)
        {
            var result = new BaseOperationResponse();
            SoftDelete(pointTransaction);
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

        public async Task<BaseOperationResponse> TopupPointBalanceByStudentGroupIdAsync(int studentGroupId, double amount,int userId)
        {
            var result = new BaseOperationResponse();

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid top-up point: {amount}. Point must be greater than zero.";
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
                student.PointBalance += amount;

                var transaction = new StudentPointTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = student.Id,
                    Description = $"Top-up Point by Student Group Id={studentGroupData.Id}, Group Name={studentGroupData.Name}",
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                await _appContext.StudentPointTransactions.AddAsync(transaction);
            }

            await _appContext.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = $"Successfully topped up {amount} point to {studentData.Count} students in group '{studentGroupData.Name}'";
            return result;
        }

        public async Task<BaseOperationResponse> TopupPointBalanceByStudentIdAsync(int studentId, double amount,int userId)
        {
            var result = new BaseOperationResponse();

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid top-up point amount: {amount}. point must be greater than zero.";
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

                var oldBalance = studentData.PointBalance;
                studentData.PointBalance += amount;

                var transaction = new StudentPointTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = studentData.Id,
                    Description = $"Top-up point by Student Id={studentData.Id}, Name={studentData.Name}",
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                await _appContext.StudentPointTransactions.AddAsync(transaction);
                await _appContext.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = $"Successfully topped up {amount} point for students : '{studentData.Name}'";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error while topping up Point for StudentId={studentId}. Details: {ex.Message}";
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
