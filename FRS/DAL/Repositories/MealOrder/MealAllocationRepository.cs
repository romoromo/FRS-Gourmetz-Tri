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
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DAL.Core.Interfaces;
using DAL.Core.DTO;
using System.ComponentModel.DataAnnotations;
using DAL.Core.Helpers;

namespace DAL.Repositories.MealOrder
{
    public class MealAllocationRepository : Repository<MealAllocation>, IMealAllocationRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;

        public MealAllocationRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<MealAllocation>> GetMealAllocationsAsync(BaseFilter filter)
        {
            IQueryable<MealAllocation> query = _appContext.MealAllocations.Include(e => e.tokens);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<MealAllocation> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(MealAllocation allocation, List<TokenLabel> tokens)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var check = await GetSingleOrDefaultAsync(e => e.deliveryDate == allocation.deliveryDate && e.mealSessionId == allocation.mealSessionId && e.IsActive == true);

                if (check != null)
                {
                    await DeleteAsync(check.Id);
                }

                var f = await AddAsync(allocation);
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

        public async Task<BaseOperationResponse> UpdateAsync(MealAllocation allocation, List<TokenLabel> tokens)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var f = await GetSingleOrDefaultAsync(e => e.Id == allocation.Id);


                    var tokensToDelete = this._appContext.TokenLabels.Where(x => x.meal_allocation_id == f.Id);

                    this._appContext.TokenLabels.RemoveRange(tokensToDelete);

                    if (tokens != null)
                    {
                        tokens.ForEach(e =>
                        {
                            if (e.token_id != null && e.token_id > 0)
                            {
                                var sc = this._appContext.TokenLabels.FirstOrDefault(x => x.Id == e.Id);
                                if (sc != null)
                                {
                                    //sc.meal_allocation_id = allocation.Id;
                                    sc.token_id = e.token_id;
                                    sc.token_name = e.token_name;
                                    sc.deliveryDate = e.deliveryDate;
                                    sc.qty = e.qty;
                                    sc.qty_dishes = e.qty_dishes;
                                    sc.qty_pdishes = e.qty_pdishes;
                                    sc.qty_tdishes = e.qty_tdishes;

                                    e.dishes.ToList().ForEach(d =>
                                    {
                                        var tdl = this._appContext.TokenDishLabels.FirstOrDefault(x => x.Id == d.Id);
                                        if (tdl != null)
                                        {
                                            //tdl.token_label_id = e.Id;
                                            tdl.token_id = d.token_id;
                                            tdl.token_name = d.token_name;
                                            tdl.a_qty = d.a_qty;
                                            tdl.o_qty = d.o_qty;
                                            tdl.p_qty = d.p_qty;
                                            tdl.t_qty = d.t_qty;
                                            tdl.dish_id = d.dish_id;
                                            tdl.dish_code = d.dish_code;
                                            tdl.dish_name = d.dish_name;
                                            tdl.token_label_id = sc.Id;
                                            this._appContext.TokenDishLabels.Update(tdl);
                                        }
                                        else
                                        {
                                            d.token_label_id = sc?.Id ?? e.Id;
                                            this._appContext.TokenDishLabels.Add(d);
                                        }


                                    });

                                    this._appContext.TokenLabels.Update(sc);
                                }
                                else
                                {
                                    e.meal_allocation_id = allocation.Id;
                                    this._appContext.TokenLabels.Add(e);
                                }
                            }
                        });
                    }

                    f.CopyFrom(allocation);

                    Update(f);
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
                        result.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    var asdasdas = ex;
                    result.Message = "Failed to save!";
                    result.IsSuccess = false;
                }

            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int allocationId)
        {
            var result = new BaseOperationResponse();
            var allocation = await GetSingleOrDefaultAsync(r => r.Id == allocationId);

            if (allocation != null)
                return await Delete(allocation);

            result.IsSuccess = false;
            result.Message = "Meal Allocation not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(MealAllocation allocation)
        {
            var result = new BaseOperationResponse();
            SoftDelete(allocation);
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

        public IQueryable<MealAllocation> GetKioskOrderDish(MealAllocationAdditionalDishFilter filter)
        {
            var today = DateTime.Today;

            IQueryable<MealAllocation> baseQuery = _appContext.MealAllocations
                .Include(e => e.tokens).ThenInclude(t => t.dishes)
                .Include(e => e.MealSessionDetail);

            var todayQuery = baseQuery.Where(e => e.deliveryDate == today);


            // Filtered data
            bool hasDateFrom = filter.DateFrom.HasValue && filter.DateFrom.Value != DateTime.MinValue;
            bool hasDateTo = filter.DateTo.HasValue && filter.DateTo.Value != DateTime.MinValue;
            bool hasOutlet = filter.OutletId > 0;

            IQueryable<MealAllocation> filterQuery = baseQuery;

            if (hasDateFrom || hasDateTo || hasOutlet)
            {
                filterQuery = filterQuery.Where(e =>
                    (!hasOutlet || e.outletId == filter.OutletId) &&
                    (!hasDateFrom || e.deliveryDate >= filter.DateFrom.Value) &&
                    (!hasDateTo || e.deliveryDate <= filter.DateTo.Value)
                );
            }
            else
            {
                // If no filter at all, skip filterQuery (return only todayQuery)
                return todayQuery;
            }

            // Combine today's data with filter data (and avoid duplicate if today is in filter)
            return filterQuery
                .Where(e => e.deliveryDate != today) // exclude today from filterQuery
                .Union(todayQuery);
        }

        public async Task<List<MealAllocation>> GetMealAllocations(int outletId, DateTime orderDate)
        {
            return await _appContext.MealAllocations.Where(m => m.outletId == outletId && m.deliveryDate == orderDate)
                .AsNoTracking()
                .Include(m => m.MealSessionDetail)
                .ToListAsync();
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
