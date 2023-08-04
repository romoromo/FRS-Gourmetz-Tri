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
using System.Transactions;

namespace DAL.Repositories.MealOrder
{
    public class StudentGroupRepository : Repository<StudentGroup>, IStudentGroupRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public StudentGroupRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<StudentGroup>> GetStudentGroupsAsync(BaseFilter filter)
        {
            IQueryable<StudentGroup> query = _appContext.StudentGroups;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<StudentGroup> GetByIdAsync(int id)
        {
            var group = id > 0 ? await GetAsync(id) :
                            await _appContext.StudentGroups.FirstOrDefaultAsync(e => e.IsActive);
            return group;
        }

        public async Task<BaseOperationResponse> CreateAsync(StudentGroup group, List<StudentGroupDetail> groupDetails)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var f = await AddAsync(group);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                    scope.Complete();
                }
                else
                {
                    result.Message = "Failed to save!";

                }
            }
            return result;

        }

        public async Task<BaseOperationResponse> UpdateAsync(StudentGroup group, List<StudentGroupDetail> groupDetails)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == group.Id);

                //check if meal session or type has been changed
                if (f.Type != group.Type || f.MealSessionId != group.MealSessionId || 
                    (f.DeliveryStartDate.HasValue && group.DeliveryStartDate.HasValue && f.DeliveryStartDate.Value.Date != group.DeliveryStartDate.Value.Date) || 
                    (f.DeliveryEndDate.HasValue && group.DeliveryEndDate.HasValue && f.DeliveryEndDate.Value.Date != group.DeliveryEndDate.Value.Date))
                {
                    //delete orders and meal plans
                    var orders = await _appContext.TokenOrders.Where(x => x.StudentGroupId == group.Id).ToListAsync();
                    foreach (var order in orders)
                    {
                        order.IsActive = false;
                        order.Status = "cancelled";
                        order.CancelledOn = DateTime.Now;
                        order.CancellationReason = "Student Group Meal Session, dates or Type were changed.";
                        _appContext.TokenOrders.Update(order);
                    }
                }

                var detailsToDelete = this._appContext.StudentGroupDetails.Where(x => x.StudentGroupId == f.Id);

                this._appContext.StudentGroupDetails.RemoveRange(detailsToDelete);

                if (groupDetails != null)
                {
                    //delete orders for students removed
                    var orders = await _appContext.TokenOrders.Where(x => x.StudentGroupId == group.Id && x.IsActive && x.IsMealPlan &&
                                        !groupDetails.Any(a => a.StudentId == x.ProfileId)).ToListAsync();
                    foreach (var order in orders)
                    {
                        order.IsActive = false;
                        order.Status = "cancelled";
                        order.CancelledOn = DateTime.Now;
                        order.CancellationReason = $"Student {order.ProfileId} was removed.";
                        _appContext.TokenOrders.Update(order);
                    }

                    string invoiceNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                    foreach (var e in groupDetails)
                    {
                        var sc = this._appContext.StudentGroupDetails.FirstOrDefault(x => x.Id == e.Id);
                        if (sc != null)
                        {
                            sc.StudentId = e.StudentId;
                            sc.StudentGroupId = e.StudentGroupId;
                            this._appContext.StudentGroupDetails.Update(sc);
                        }
                        else
                        {
                            this._appContext.StudentGroupDetails.Add(e);
                        }

                        // create orders using meal plans
                        var mealPlans = await _appContext.StudentGroupMealPlans.Where(x => x.StudentGroupId == group.Id && x.IsActive).ToListAsync();
                        foreach (var mealPlan in mealPlans)
                        {
                            //check if student already has an existing order, skip
                            var hasOrder = await _appContext.TokenOrders.AnyAsync(x => x.StudentGroupId == group.Id && x.ProfileId == e.StudentId && x.IsActive && x.IsMealPlan &&
                                            x.DeliveryDate.Date == mealPlan.DeliveryDate.Date);

                            if (hasOrder) continue;
                            var order = new TokenOrder
                            {
                                DeliveryDate = mealPlan.DeliveryDate.Date,
                                TransactionTime = DateTime.Now,
                                MealSessionDetailId = mealPlan.MealSessionDetailId,
                                ProfileId = e.StudentId,
                                Status = "paid",
                                StoreId = mealPlan.StoreId,
                                StudentGroupId = group.Id,
                                TotalAmount = mealPlan.Price,
                                TotalPayment = mealPlan.Price,
                                IsMealPlan = true,
                                Tokens = new List<TokenOrdered>
                                {
                                    new TokenOrdered
                                    {
                                        TokenId = mealPlan.MealTypeId.Value,
                                        Qty = 1,
                                        TokenDesc = mealPlan.Label,
                                        SelectedDishes = new List<TokenOrderDish>
                                        {
                                                new TokenOrderDish
                                                {
                                                    DishId = mealPlan.DishId,
                                                    Qty = 1
                                                }
                                        }
                                    }
                                }
                            };

                            var student = await _appContext.Students.FirstOrDefaultAsync(x => x.Id == e.StudentId);
                            var payment = new Payment
                            {
                                StudentId = order.ProfileId,
                                email = student?.Email,
                                subtotal = (decimal)order.TotalAmount,
                                total = (decimal)order.TotalAmount,
                                InvoiceNumber = invoiceNumber,
                                Status = "SUCCESS"
                            };

                            order.Payment = payment;
                            await _appContext.TokenOrders.AddAsync(order);
                        }
                    }
                }

                f.CopyFrom(group);

                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    //result.Data = f;

                    scope.Complete();
                }
                else
                {
                    result.Message = "Failed to save!";
                    result.IsSuccess = false;
                }

            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(StudentGroup group)
        {
            var result = new BaseOperationResponse();
            var f = await GetSingleOrDefaultAsync(e => e.Id == group.Id);
            
            f.CopyFrom(group);

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

        public async Task<BaseOperationResponse> DeleteAsync(int groupId)
        {
            var result = new BaseOperationResponse();
            var group = await GetSingleOrDefaultAsync(r => r.Id == groupId);

            if (group != null)
                return await Delete(group);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(StudentGroup group)
        {
            var result = new BaseOperationResponse();
            SoftDelete(group);
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

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
