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
    public class TokenLabelRepository : Repository<TokenLabel>, ITokenLabelRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;

        public TokenLabelRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<TokenLabel>> GetTokenLabelsAsync(BaseFilter filter)
        {
            IQueryable<TokenLabel> query = _appContext.TokenLabels.Include(e => e.dishes);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<TokenLabel> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(TokenLabel order, List<TokenDishLabel> tokenOrders)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var f = await AddAsync(order);
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

        public async Task<BaseOperationResponse> UpdateAsync(TokenLabel order, List<TokenDishLabel> tokenOrders)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == order.Id);

                var tokensToDelete = this._appContext.TokenDishLabels.Where(x => x.token_label_id == f.Id &&
                                    (tokenOrders == null || !tokenOrders.Any(a => a.dish_id == x.dish_id)));

                this._appContext.TokenDishLabels.RemoveRange(tokensToDelete);

                if (tokenOrders != null)
                {
                    tokenOrders.ForEach(e =>
                    {
                        if (e.dish_id != null && e.dish_id > 0)
                        {

                            var sc = this._appContext.TokenDishLabels.FirstOrDefault(x => x.Id == e.Id);
                            if (sc != null)
                            {
                                sc.token_id = e.token_id;
                                sc.token_name = e.token_name;
                                sc.a_qty = e.a_qty;
                                sc.o_qty = e.o_qty;
                                sc.p_qty = e.p_qty;
                                sc.t_qty = e.t_qty;
                                sc.dish_id = e.dish_id;
                                sc.dish_code = e.dish_code;
                                sc.dish_name = e.dish_name;
                                sc.token_label_id = order.Id;
                                this._appContext.TokenDishLabels.Update(sc);
                            }
                            else
                            {
                                this._appContext.TokenDishLabels.Add(e);
                            }
                        }
                    });
                }

                f.CopyFrom(order);

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

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int orderId)
        {
            var result = new BaseOperationResponse();
            var order = await GetSingleOrDefaultAsync(r => r.Id == orderId);

            if (order != null)
                return await Delete(order);

            result.IsSuccess = false;
            result.Message = "Order not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(TokenLabel order)
        {
            var result = new BaseOperationResponse();
            SoftDelete(order);
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
