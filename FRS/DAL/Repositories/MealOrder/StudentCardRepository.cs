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
using DAL.Repositories.Interfaces.MealOrder;

namespace DAL.Repositories.MealOrder
{
    public class StudentCardRepository : Repository<StudentCard>, IStudentCardRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public StudentCardRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<StudentCard>> GetStudentCardsAsync(BaseFilter filter)
        {
            IQueryable<StudentCard> query = _appContext.StudentCards;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<StudentCard> GetByIdAsync(int id, string cardId)
        {
            var studentCard = id > 0 ? await GetAsync(id) :
                            await _appContext.StudentCards.FirstOrDefaultAsync(e => e.IsActive && e.CardId.ToLower() == cardId.ToLower());
            return studentCard;
        }

        public async Task<Student> GetStudentByCardIdAsync(string cardId)
        {
            var card = await _appContext.StudentCards.FirstOrDefaultAsync(e => e.IsActive && e.CardId.ToLower() == cardId.ToLower());
            return card?.Student;
        }

        public async Task<BaseOperationResponse> CreateAsync(StudentCard studentCard)
        {
            var result = new BaseOperationResponse();

            var cards = _appContext.StudentCards.Select(t => t.IsActive).ToList();
            var cardExists = _appContext.StudentCards.Where(e => e.IsActive && e.CardId.ToLower() == studentCard.CardId.ToLower()).ToList();


            if (cardExists != null && cardExists.Count > 0)
            {
                result.Message = "Failed to save! Card Id already exists.";
                result.IsSuccess = false;
            }
            else
            {
                var otherStudentCards = _appContext.StudentCards.Where(e => e.IsActive && e.StudentId == studentCard.StudentId);
                if (otherStudentCards != null)
                {
                    otherStudentCards.ToList().ForEach(e => {
                        e.Status = StudentCardStatus.INACTIVE.ToString();
                        _appContext.StudentCards.Update(e);
                    });
                }

                studentCard.Status = StudentCardStatus.ACTIVE.ToString();
                var f = await AddAsync(studentCard);
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
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(StudentCard studentCard)
        {
            var result = new BaseOperationResponse();
            var f = await GetSingleOrDefaultAsync(e => e.Id == studentCard.Id);
            //var cardExists = _appContext.StudentCards.Any(e =>
            //                e.IsActive && e.Id != studentCard.Id &&
            //                e.CardId.ToLower() == studentCard.CardId.ToLower());

            var cardExists = _appContext.StudentCards.Where(e => e.IsActive && e.Id != studentCard.Id && e.CardId.ToLower() == studentCard.CardId.ToLower()).ToList();

            if (cardExists != null && cardExists.Count > 0)
            {
                result.Message = "Failed to save! Duplicate card found.";
                result.IsSuccess = false;
            }
            else
            {
                f.CopyFrom(studentCard);

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
            }

            return result;
        }

        public async Task<BaseOperationResponse> ActivateStudentCardByIdAsync(string cardId)
        {
            var result = new BaseOperationResponse();
            var card = await _appContext.StudentCards.FirstOrDefaultAsync(e => e.IsActive && e.CardId.ToLower() == cardId.ToLower());
            if (card != null)
            {
                var otherStudentCards = _appContext.StudentCards.Where(e => e.IsActive && e.StudentId == card.StudentId);
                if (otherStudentCards != null)
                {
                    otherStudentCards.ToList().ForEach(e => {
                        e.Status = StudentCardStatus.INACTIVE.ToString();
                        _appContext.StudentCards.Update(e);
                    });
                }

                card.Status = StudentCardStatus.ACTIVE.ToString();
                Update(card);

                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                }
                else
                {
                    result.Message = "Failed to save!";
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Record not found.";
            }
            
            return result;
        }

        public async Task<BaseOperationResponse> DeleteAsync(int studentCardId)
        {
            var result = new BaseOperationResponse();
            var studentCard = await GetSingleOrDefaultAsync(r => r.Id == studentCardId);

            if (studentCard != null)
                return await Delete(studentCard);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(StudentCard studentCard)
        {
            var result = new BaseOperationResponse();
            SoftDelete(studentCard);
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

        public async Task<BaseOperationResponse> ActivateStudentVoucher(int studentId, string code)
        {
            var result = new BaseOperationResponse();
            var student = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive && e.Id == studentId);

            if (student != null)
            {
                
                var outletProfiles = student.Outlet.CatererOutlets.Select(e => e.OutletProfileId).ToList();
                var now = DateTime.Now;

                var voucher = await _appContext.Vouchers.FirstOrDefaultAsync(r =>
                                r.Code.Trim().ToLower() == code.Trim().ToLower() &&
                                now <= r.EndDateTime &&
                                r.IsActive &&
                                outletProfiles.Contains(r.OutletProfileId));

                if (voucher == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Voucher does not exist!";
                    return result;
                }

                int voucherLeft = voucher.UsageQuantity - voucher.UsageQuantityUsed;
                if (voucherLeft <= 0)
                {
                    result.IsSuccess = false;
                    result.Message = "Insufficient vouchers for the number of students in the group!";
                    return result;
                }

                if (student.Vouchers != null)
                {
                    var studentVoucherId = student.Vouchers.Select(t => t.VoucherId).ToList();

                    if (!studentVoucherId.Contains(voucher.Id))
                    {
                        var studentVoucher = new StudentVoucher
                        {
                            StudentId = student.Id,
                            VoucherId = voucher.Id,
                            Status = "NEW"
                        };

                        await _appContext.StudentVouchers.AddAsync(studentVoucher);
                        voucher.UsageQuantityUsed += 1;
                        _appContext.Vouchers.Update(voucher);


                        if (await _appContext.SaveChangesAsync() > 0)
                        {
                            result.Message = "Successfully saved!";
                            result.IsSuccess = true;
                        }
                        else
                        {
                            result.Message = "Failed to save!";
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "Voucher already applied to the student";
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Record not found.";
            }

            return result;
        }

        public async Task<List<StudentVoucher>> GetStudentVouchersAsync(int studentId)
        {
            var now = DateTime.Now;
            var vouchers = _appContext.StudentVouchers.Where(e => e.IsActive && e.StudentId == studentId
                                    && now <= e.Voucher.EndDateTime && e.Status != "USED");
            return vouchers.ToList();
        }

        public async Task<BaseOperationResponse> AssignVoucherByStudentGroup(int studentGroupId, string code)
        {
            var result = new BaseOperationResponse();

            var studentIds = await _appContext.StudentGroupDetails.AsNoTracking()
                .Where(e => e.IsActive && e.StudentGroupId == studentGroupId)
                .Select(x => x.StudentId)
                .Distinct()
                .ToListAsync();

            if (!studentIds.Any())
            {
                result.IsSuccess = false;
                result.Message = "Students not found.";
                return result;
            }

            var outletProfileIds = await _appContext.Students
                .Where(e => e.IsActive && studentIds.Contains(e.Id))
                .SelectMany(x => x.Outlet.CatererOutlets.Select(y => y.OutletProfileId))
                .Distinct()
                .ToListAsync();

            if (!outletProfileIds.Any())
            {
                result.IsSuccess = false;
                result.Message = "No outlet profiles found for the students.";
                return result;
            }

            var now = DateTime.Now;
            var voucher = await _appContext.Vouchers.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code.Trim().ToLower() == code.Trim().ToLower() &&
                                          now <= r.EndDateTime &&
                                          r.IsActive &&
                                          outletProfileIds.Contains(r.OutletProfileId));

            if (voucher == null)
            {
                result.IsSuccess = false;
                result.Message = "Voucher does not exist!";
                return result;
            }

            int voucherLeft = voucher.UsageQuantity - voucher.UsageQuantityUsed;

            if (studentIds.Count() > voucherLeft)
            {
                result.IsSuccess = false;
                result.Message = "Insufficient vouchers for the number of students in the group!";
                return result;
            }

            var studentVoucherData = await _appContext.StudentVouchers.AsNoTracking()
                .Where(x => x.IsActive && studentIds.Contains(x.StudentId))
                .Select(x => new { x.StudentId, x.VoucherId })
                .ToListAsync();

            var existingVoucherIds = studentVoucherData
                .Where(x => x.VoucherId == voucher.Id)
                .Select(x => x.StudentId)
                .ToHashSet();

            var newStudentVouchers = studentIds
                .Where(studentId => !existingVoucherIds.Contains(studentId))
                .Select(studentId => new StudentVoucher
                {
                    StudentId = studentId,
                    VoucherId = voucher.Id,
                    Status = "NEW"
                })
                .ToList();

            if (newStudentVouchers.Any())
            {
                await _appContext.StudentVouchers.AddRangeAsync(newStudentVouchers);
                voucher.UsageQuantityUsed += newStudentVouchers.Count();
                _appContext.Vouchers.Update(voucher);

                await _appContext.SaveChangesAsync();
            }

            result.IsSuccess = true;
            result.Message = "Vouchers assigned successfully.";
            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
