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
        private IUserActivityRepository _userActivityRepository;
        public StudentCardRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId,
            IUserActivityRepository userActivityRepository) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
            _userActivityRepository = userActivityRepository;
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
                    result.Message = "Insufficient vouchers for the number of students!";
                    return result;
                }

                var currentStudentVoucherCount = await _appContext.StudentVouchers
                    .CountAsync(x => x.VoucherId == voucher.Id && x.StudentId == studentId);
                if(currentStudentVoucherCount >= voucher.MaxDistribution)
                {
                    result.IsSuccess = false;
                    result.Message = "Maximum allocation per user reached for this student.";
                    return result;
                }


                if (student.Vouchers != null)
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
                    string message = $"{student.Id} {student.Name} : Assigned for this voucher : {voucher.Code}";
                    await _userActivityRepository.CreateAsync(message, _currentUserId);


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
            return await vouchers.ToListAsync();
        }

        public async Task<Dictionary<string,int>> GetVoucherUsedCountAsync(int studentId,List<string> voucherCodes)
        {
            var returnData = await _appContext.StudentVouchers
            .Where(sv => 
                sv.IsActive 
                && sv.StudentId == studentId 
                && sv.Status == "USED" 
                && voucherCodes.Contains(sv.Voucher.Code)
            )
            .GroupBy(sv => sv.Voucher.Code)
            .Select(g => new { Code = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Code, g => g.Count);
            return returnData;
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

            var existingVoucherCounts = await _appContext.StudentVouchers
                .Include(x => x.Student)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(x => x.IsActive && studentIds.Contains(x.StudentId) && x.VoucherId == voucher.Id)
                .GroupBy(x => x.StudentId)
                .Select(g => new {
                    StudentId = g.Key,
                    StudentName = g.First().Student.Name,
                    Count = g.Count()
                })
                .ToDictionaryAsync(g => g.StudentId, g => new { g.StudentName, g.Count });

            List<string> messageData = new List<string>();
            List<int> dataToInsert = new List<int>();
            foreach (var studentIdData in studentIds)
            {

                if (existingVoucherCounts.TryGetValue(studentIdData, out var usedCountData) && usedCountData.Count >= voucher.MaxDistribution)
                {
                    string message = $"{studentIdData} {usedCountData.StudentName} : Maximum allocation per user reached for this voucher : {voucher.Code}";
                    messageData.Add($"{studentIdData} {usedCountData.StudentName} : Maximum allocation per user reached for this voucher");
                    await _userActivityRepository.CreateAsync(message, _currentUserId);
                }
                else
                {
                    var dataObject = new StudentVoucher
                    {
                        StudentId = studentIdData,
                        VoucherId = voucher.Id,
                        Status = "NEW"
                    };
                    dataToInsert.Add(studentIdData);
                    await _appContext.StudentVouchers.AddAsync(dataObject);
                    string message = $"{studentIdData} {usedCountData.StudentName} : Assigned for this voucher : {voucher.Code}";
                    await _userActivityRepository.CreateAsync(message, _currentUserId);
                }


            }

            if (dataToInsert.Any())
            {
                voucher.UsageQuantityUsed += dataToInsert.Count();
                _appContext.Vouchers.Update(voucher);
            }
            await _appContext.SaveChangesAsync();

            result.IsSuccess = true;
            result.Data = messageData;
            result.Message = "Vouchers assigned successfully.";
            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
