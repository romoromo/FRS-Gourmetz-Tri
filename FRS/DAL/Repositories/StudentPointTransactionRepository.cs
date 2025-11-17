using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<BaseOperationResponse> TopupPointBalanceByStudentGroupIdAsync(
            int studentGroupId,
            double amount,
            int userId,
            PointType pointTypeData)
        {
            var result = new BaseOperationResponse();

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid top-up point: {amount}. point must be greater than zero.";
                return result;
            }

            string pointType = pointTypeData.ToString();
            if (string.IsNullOrWhiteSpace(pointType))
            {
                result.IsSuccess = false;
                result.Message = "point type must be provided.";
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

            var points = await _appContext.StudentPoints
                .Where(w => studentIds.Contains(w.StudentId))
                .ToListAsync();
            var transactions = new List<StudentPointTransaction>();
            foreach (var studentData in studentDatas)
            {
                var studentId = studentData.Id;
                var point = points.FirstOrDefault(w => w.StudentId == studentId && w.Type == pointType); ;

                double oldBalance = 0;
                if (point == null)
                {
                    point = new StudentPoint
                    {
                        StudentId = studentId,
                        Balance = amount,
                        Type = pointType,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };
                    await _appContext.StudentPoints.AddAsync(point);
                    points.Add(point);
                }
                else
                {
                    oldBalance = point.Balance;
                    point.Balance += amount;
                    point.UpdatedBy = userId;
                }

                transactions.Add(new StudentPointTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = studentId,
                    Description = $"Top-up {pointType} point by {amount} for student '{studentData.Name}' (ID={studentId}). " +
                                  $"via Group Id={studentGroupData.Id} ({studentGroupData.Name}).",
                    CreatedBy = userId,
                    UpdatedBy = userId
                });

                studentData.PointBalance = points
                    .Where(w => w.StudentId == studentId)
                    .Sum(w => w.Balance);
            }

            await _appContext.StudentPointTransactions.AddRangeAsync(transactions);
            await _appContext.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = $"Successfully topped up point {amount} to {studentDatas.Count} students in group '{studentGroupData.Name}'";
            return result;
        }

        public async Task<BaseOperationResponse> TopupPointBalanceByStudentIdAsync(
            int studentId,
            double amount,
            int userId,
            PointType pointTypeData)
        {
            var result = new BaseOperationResponse();

            if (amount <= 0)
            {
                result.IsSuccess = false;
                result.Message = $"Invalid top-up point: {amount}. point must be greater than zero.";
                return result;
            }

            string pointType = pointTypeData.ToString();
            if (string.IsNullOrWhiteSpace(pointType))
            {
                result.IsSuccess = false;
                result.Message = "Point type must be provided.";
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

                var point = await _appContext.StudentPoints
                    .FirstOrDefaultAsync(w => w.StudentId == studentId && w.Type == pointType);

                double oldPoint = 0;
                if (point == null)
                {
                    point = new StudentPoint
                    {
                        StudentId = studentId,
                        Balance = amount,
                        Type = pointType,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };
                    await _appContext.StudentPoints.AddAsync(point);
                }
                else
                {
                    oldPoint = point.Balance;
                    point.Balance += amount;
                    point.UpdatedBy = userId;
                }

                var transaction = new StudentPointTransaction
                {
                    Amount = amount,
                    TransactionType = WalletTransactionType.CREDIT.ToString(),
                    StudentId = studentId,
                    Description = $"Top-up {pointType} point by {amount} for student '{studentData.Name}' (ID={studentId}). ",
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                var totalPointBalance = await _appContext.StudentPoints
                    .AsNoTracking()
                    .Where(w => w.StudentId == studentId && w.Type != pointType)
                    .SumAsync(w => (double?)w.Balance) ?? 0;
                studentData.PointBalance = totalPointBalance + point.Balance;

                await _appContext.StudentPointTransactions.AddAsync(transaction);
                await _appContext.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = $"Successfully topped up {amount} to '{pointType}' point for student '{studentData.Name}'.";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error while topping up '{pointType}' point for StudentId={studentId}. Details: {ex.Message}";
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
