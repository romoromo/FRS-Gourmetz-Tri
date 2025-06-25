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
using DAL.Core.DTO;

namespace DAL.Repositories.MealOrder
{
    public class VoucherRepository : Repository<Voucher>, IVoucherRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        private IUserActivityRepository _userActivityRepository;
        public VoucherRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId, IUserActivityRepository userActivityRepository) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
            _userActivityRepository = userActivityRepository;
        }

        #region Sieved
        public async Task<PagedEntity<Voucher>> GetVouchersAsync(BaseFilter filter)
        {
            IQueryable<Voucher> query = _appContext.Vouchers
                .Include(e => e.Institution);

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<Voucher>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        public async Task<PagedEntity<VoucherStudentLiteDTO>> GetVouchersStudentAsync(BaseFilter filter)
        {
            var query = _appContext.StudentVouchers
                .Include(e => e.Voucher).ThenInclude(x => x.VoucherType)
                .Include(x => x.Student)
                .AsSplitQuery()
                .Where(x => x.IsActive)
                .AsNoTracking()
                .Select(x => new VoucherStudentLiteDTO
                {
                    StudentVoucherId = x.Id,
                    VoucherId = x.VoucherId,
                    StudentId = x.StudentId,
                    Code = x.Voucher.Code,
                    Name = x.Voucher.Name,
                    VoucherTypeName = x.Voucher.VoucherType.Name,
                    StudentName = x.Student.Name,
                    Status = x.Status

                });

            query = _sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<VoucherStudentLiteDTO>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion
        public async Task<Voucher> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(Voucher voucher)
        {
            var result = new BaseOperationResponse();
            //validate if code already exists
            var similarCode = await GetFirstOrDefaultAsync(e =>
                                e.IsActive &&
                                e.Id != voucher.Id &&
                                e.Code.Trim().ToLower() == voucher.Code.Trim().ToLower());

            if (similarCode != null)
            {
                result.Message = "Failed to save voucher! Voucher code must be unique.";
                result.IsSuccess = false;
                return result;
            }

            var f = await AddAsync(voucher);
            await _userActivityRepository.CreateAsync($"Voucher created: {voucher.Code}", _currentUserId);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save voucher!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Voucher voucher)
        {
            var result = new BaseOperationResponse();

            //validate if code already exists
            var similarCode = await GetFirstOrDefaultAsync(e =>
                                e.IsActive &&
                                e.Id != voucher.Id &&
                                e.Code.Trim().ToLower() == voucher.Code.Trim().ToLower());

            if (similarCode != null)
            {
                result.Message = "Failed to save voucher! Voucher code must be unique.";
                result.IsSuccess = false;
                return result;
            }

            var f = await GetSingleOrDefaultAsync(e => e.Id == voucher.Id);

            var mealPeriodIds = voucher.VoucherMealPeriods != null ? voucher.VoucherMealPeriods.Select(a => a.MealPeriodId) : new List<int>();
            //update periods
            var periodsToDelete = _appContext.VoucherMealPeriods.Where(x => x.VoucherId == f.Id &&
                                    (!mealPeriodIds.Any() || !mealPeriodIds.Any(a => a == x.MealPeriodId)));


            this._appContext.VoucherMealPeriods.RemoveRange(periodsToDelete);

            if (voucher.VoucherMealPeriods != null)
            {
                voucher.VoucherMealPeriods.ToList().ForEach(e =>
                {
                    var sr = this._appContext.VoucherMealPeriods.FirstOrDefault(x => x.VoucherId == e.VoucherId && x.MealPeriodId == e.MealPeriodId);
                    if (sr != null)
                    {
                        sr.IsActive = true;
                        this._appContext.VoucherMealPeriods.Update(sr);
                    }
                    else
                    {
                        this._appContext.VoucherMealPeriods.Add(e);
                    }
                });
            }

            f.CopyFrom(voucher);
            f.VoucherDishes.Clear();
            if (voucher.VoucherDishes.Count > 0) {
                foreach (var item in voucher.VoucherDishes)
                {
                    f.VoucherDishes.Add(new VoucherDish
                    {
                        DishId = item.DishId,
                    });
                }
            }
            Update(f);
            await _userActivityRepository.CreateAsync($"Voucher Updated: {voucher.Code}", _currentUserId);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save voucher!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int voucherId)
        {
            var result = new BaseOperationResponse();
            var voucher = await GetSingleOrDefaultAsync(r => r.Id == voucherId);

            if (voucher != null)
                return await Delete(voucher);

            result.IsSuccess = false;
            result.Message = "Voucher not found.";
            return result;
        }

        public async Task<BaseOperationResponse> DeleteStudentVoucherAndUpdateCountVoucher(int studentVoucherId)
        {
            var result = new BaseOperationResponse();

            var selectedData = await _appContext.StudentVouchers
                .Include(x => x.Student)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == studentVoucherId);
            if (selectedData == null)
            {
                result.Message = "Student voucher not found!";
                result.IsSuccess = false;
                return result;
            }

            _appContext.StudentVouchers.Remove(selectedData);

            var selectedVoucher = await _appContext.Vouchers.FindAsync(selectedData.VoucherId);
            if (selectedVoucher != null)
            {
                selectedVoucher.UsageQuantityUsed = Math.Max(0, selectedVoucher.UsageQuantityUsed - 1);
            }

            string message = $"{selectedData.StudentId} {selectedData?.Student?.Name} : Unassigned from this voucher : {selectedVoucher.Code}";
            await _userActivityRepository.CreateAsync(message, _currentUserId);

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete student voucher!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> Delete(Voucher voucher)
        {
            var result = new BaseOperationResponse();
            SoftDelete(voucher);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete voucher!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> ValidateVoucher(int studentId, string code)
        {
            var result = new BaseOperationResponse();
            var now = DateTime.Now;
            //var voucher = _appContext.StudentVouchers.Where(r => r.IsActive && r.StudentId == studentId
            //                        && r.Voucher.Code.Trim().Equals(code.Trim(), StringComparison.OrdinalIgnoreCase)
            //                        && now >= r.Voucher.StartDateTime && now <= r.Voucher.EndDateTime && r.IsActive);

            var voucher = _appContext.StudentVouchers.Where(r =>
                            r.IsActive &&
                            r.StudentId == studentId &&
                            r.Voucher.Code.Trim().ToLower() == code.Trim().ToLower() &&
                            now >= r.Voucher.StartDateTime &&
                            now <= r.Voucher.EndDateTime);


            result.IsSuccess = voucher != null;
            return result;
        }

        public async Task<List<Voucher>> GetAllValidVoucher(int studentId)
        {
            var result = new List<Voucher>();
            var now = DateTime.Now;
            var voucher = _appContext.StudentVouchers.Where(r => r.IsActive && r.StudentId == studentId
                                    && now >= r.Voucher.StartDateTime && now <= r.Voucher.EndDateTime && r.IsActive);

            result = voucher.ToList().Select(x => x.Voucher).ToList();
            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
