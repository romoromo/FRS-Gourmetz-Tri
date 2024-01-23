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
using DAL.Core.Logging;
using Microsoft.Extensions.Logging;
using DAL.Models.StoredProcedures;
using System.Data.SqlClient;
using static DAL.Core.Constants;
using NPOI.SS.Formula.Functions;

namespace DAL.Repositories.MealOrder
{
    public class TokenOrderRepository : Repository<TokenOrder>, ITokenOrderRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        private readonly ILogger _logger;

        public TokenOrderRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
            _logger = Logger.CreateLogger<TokenOrderRepository>();
        }

        #region Sieved
        public async Task<List<spSalesOrderReport>> GetSalesOrders(SalesOrderReportFilter filter)
        {
            var from = new SqlParameter("@ReportDateFrom", System.Data.SqlDbType.Date);
            var to = new SqlParameter("@ReportDateTo", System.Data.SqlDbType.Date);
            var status = new SqlParameter("@Status", System.Data.SqlDbType.VarChar);
            var isFas = new SqlParameter("@IsFAS", System.Data.SqlDbType.Bit);
            var page = new SqlParameter("@Page", System.Data.SqlDbType.Int);
            var pageSize = new SqlParameter("@PageSize", System.Data.SqlDbType.Int);
            var keywords = new SqlParameter("@Keywords", System.Data.SqlDbType.VarChar);
            var sortByCol = new SqlParameter("@SortBy", System.Data.SqlDbType.VarChar);
            var sortBy = new SqlParameter("@SortDirection", System.Data.SqlDbType.Bit);
            var reportType = new SqlParameter("@ReportType", System.Data.SqlDbType.Int);
            var studentGroupId = new SqlParameter("@StudentGroupId", System.Data.SqlDbType.Int);
            var orderType = new SqlParameter("@OrderType", System.Data.SqlDbType.VarChar);

            from.Value = filter.ReportDateFrom;
            to.Value = filter.ReportDateTo;
            status.Value = (object)filter.Status ?? DBNull.Value;
            isFas.Value = (object)filter.IsFas ?? DBNull.Value;
            page.Value = (object)filter.Page ?? 1;
            pageSize.Value = (object)filter.PageSize ?? int.MaxValue;
            keywords.Value = (object)filter.Keyword ?? DBNull.Value;
            reportType.Value = (object)filter.ReportType ?? 1;
            studentGroupId.Value = (object)filter.StudentGroupId ?? DBNull.Value;
            orderType.Value = (object)filter.OrderType ?? DBNull.Value;

            bool isDesc = filter.Sorts.Contains("-");
            sortByCol.Value = isDesc ? filter.Sorts.Substring(1) : filter.Sorts;
            sortBy.Value = isDesc;

            var orders = await _appContext.spSalesOrderReport
                            .FromSql($"exec spSalesOrderReport @ReportDateFrom, @ReportDateTo, @Status, @IsFAS, @Page, @PageSize, @Keywords, @SortBy, @SortDirection, @ReportType, @StudentGroupId, @OrderType",
                                    from, to, status, isFas, page, pageSize, keywords, sortByCol, sortBy, reportType, studentGroupId, orderType).ToListAsync();

            return orders;
        }

        //public async Task<List<spSalesOrderReport>> GetOrderCollection(OrderCollectionReportFilter filter)
        //{
        //    var from = new SqlParameter("@ReportDateFrom", System.Data.SqlDbType.Date);
        //    var to = new SqlParameter("@ReportDateTo", System.Data.SqlDbType.Date);
        //    var status = new SqlParameter("@Status", System.Data.SqlDbType.VarChar);
        //    var isFas = new SqlParameter("@IsFAS", System.Data.SqlDbType.Bit);
        //    var page = new SqlParameter("@Page", System.Data.SqlDbType.Int);
        //    var pageSize = new SqlParameter("@PageSize", System.Data.SqlDbType.Int);
        //    var keywords = new SqlParameter("@Keywords", System.Data.SqlDbType.VarChar);
        //    var sortByCol = new SqlParameter("@SortBy", System.Data.SqlDbType.VarChar);
        //    var sortBy = new SqlParameter("@SortDirection", System.Data.SqlDbType.Bit);
        //    var reportType = new SqlParameter("@ReportType", System.Data.SqlDbType.Int);
        //    var studentGroupId = new SqlParameter("@StudentGroupId", System.Data.SqlDbType.Int);

        //    from.Value = filter.ReportDateFrom;
        //    to.Value = filter.ReportDateTo;
        //    status.Value = (object)filter.Status ?? DBNull.Value;
        //    isFas.Value = (object)filter.IsFas ?? DBNull.Value;
        //    page.Value = (object)filter.Page ?? 1;
        //    pageSize.Value = (object)filter.PageSize ?? int.MaxValue;
        //    keywords.Value = (object)filter.Keyword ?? DBNull.Value;
        //    reportType.Value = (object)filter.ReportType ?? 1;
        //    studentGroupId.Value = (object)filter.StudentGroupId ?? 0;

        //    bool isDesc = filter.Sorts.Contains("-");
        //    sortByCol.Value = isDesc ? filter.Sorts.Substring(1) : filter.Sorts;
        //    sortBy.Value = isDesc;

        //    var orders = await _appContext.spSalesOrderReport
        //                    .FromSql($"exec spOrderCollectionReport @ReportDateFrom, @ReportDateTo, @Status, @IsFAS, @Page, @PageSize, @Keywords, @SortBy, @SortDirection, @ReportType, @StudentGroupId",
        //                            from, to, status, isFas, page, pageSize, keywords, sortByCol, sortBy, reportType, studentGroupId).ToListAsync();

        //    return orders;
        //}

        public async Task<PagedEntity<TokenOrder>> GetTokenOrdersAsync(BaseFilter filter)
        {
            IQueryable<TokenOrder> query = _appContext.TokenOrders.Include(e => e.Tokens);
            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        public async Task<PagedEntity<TokenOrder>> GetCancellationOrdersAsync(OrderCancellationFilter filter)
        {
            IQueryable<TokenOrder> query = _appContext.TokenOrders.Include(e => e.Tokens);

            if (filter.OutletId.HasValue)
            {
                query = query.Where(e => e.Student.OutletId == filter.OutletId);
            }

            if (!string.IsNullOrEmpty(filter.InvoiceNumber))
            {
                query = query.Where(e => e.Payment.InvoiceNumber.Trim() == filter.InvoiceNumber.Trim());
            }

            if (!string.IsNullOrEmpty(filter.OrderNumber))
            {
                query = query.Where(e => e.Payment.PaymentNumber.Trim() == filter.OrderNumber.Trim());
            }

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(e => e.Student.Name.Trim().Contains(filter.Keyword.Trim()) ||
                             e.Student.Email.Trim().Contains(filter.Keyword.Trim()));
            }

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        public async Task<PagedEntity<TokenOrder>> GetStudentOrdersAsync(StudentOrderFilter filter)
        {
            IQueryable<TokenOrder> query = _appContext.TokenOrders.Where(e => e.ProfileId == filter.StudentId)
                                        .Include(e => e.Tokens);

            if (filter.OutletId.HasValue)
            {
                query = query.Where(e => e.Student.OutletId == filter.OutletId);
            }

            if (!string.IsNullOrEmpty(filter.InvoiceNumber))
            {
                query = query.Where(e => e.Payment.InvoiceNumber.Trim() == filter.InvoiceNumber.Trim());
            }

            if (!string.IsNullOrEmpty(filter.OrderNumber))
            {
                query = query.Where(e => e.Payment.PaymentNumber.Trim() == filter.OrderNumber.Trim());
            }

            //if (!string.IsNullOrEmpty(filter.Keyword))
            //{
            //    query = query.Where(e => e.Student.Name.Trim().Contains(filter.Keyword.Trim()) ||
            //                 e.Student.Email.Trim().Contains(filter.Keyword.Trim()));
            //}

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion

        public async Task<BaseOperationResponse> CancelOrders(List<int> orderIds, int cancelledById, string reason)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.CANCEL_ORDER_CREATE.ToString(),
                Remarks = $"Orders {string.Join(",", orderIds)} were cancelled by {cancelledById}."
            };

            var result = new BaseOperationResponse();

            IQueryable<TokenOrder> orders = _appContext.TokenOrders.Where(t => orderIds.Any(f => f == t.Id));

            foreach (var order in orders)
            {
                order.Status = "cancelled";
                order.CancelledById = cancelledById;
                order.CancelledOn = DateTime.Now;
                order.CancellationReason = reason;
                Update(order);
            }

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            _appContext.ResetAuditUserAction();
            return result;
        }

        public async Task<BaseOperationResponse> BulkCancelCart()
        {
            var result = new BaseOperationResponse();
            IQueryable<TokenOrder> orders = _appContext.TokenOrders.Where(t => t.IsActive && t.Status == "pending" && t.Payment == null &&
                                            (DateTime.Now.Date.Subtract(t.DeliveryDate.Date).TotalDays > 3 ||
                                            t.DeliveryDate.Date.Subtract(DateTime.Now.Date).TotalDays <= 3));

            foreach(var order in orders)
            {
                order.Status = "cancelled";
                order.CancelledOn = DateTime.Now;
                order.CancellationReason = "cancelled by the system - Payment Not made before cutoff Time";
                Update(order);
            }

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            result.IsSuccess = true;
            return result;
        }

        public async Task<BaseOperationResponse> AmendOrder(int id, string status, string invoiceNumber, string fomoId, int? updatedById, string reason)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.ORDER_UPDATE.ToString(),
                Remarks = $"Order {id} was updated by {updatedById}."
            };

            var result = new BaseOperationResponse();

            var order = _appContext.TokenOrders.Include(e => e.Payment).FirstOrDefault(t => t.Id == id);

            if (order != null)
            {
                order.Status = status;

                if(!string.IsNullOrEmpty(invoiceNumber) || !string.IsNullOrEmpty(fomoId))
                {
                    if(order.Payment == null)
                    {
                        result.Message = "This order has no payment record.";
                        result.IsSuccess = false;
                        return result;
                    }
                }

                if (order.Payment != null)
                {
                    order.Payment.InvoiceNumber = invoiceNumber;
                    order.Payment.fomoid = fomoId;
                }

                order.UpdatedBy = updatedById;
                order.UpdatedDate = DateTime.Now;
                order.AmendReason += "\n" + reason;
                Update(order);

                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                }
                else
                {
                    result.Message = "Failed to save!";
                    result.IsSuccess = false;
                }
            }
            else
            {
                result.Message = "Order not found";
                result.IsSuccess = false;
            }

            _appContext.ResetAuditUserAction();
            return result;
        }

        public async Task<BaseOperationResponse> CreatePrepaidOrderAsync(PrepaidOrderDTO order)
        {
            var result = new BaseOperationResponse();
            string invoiceNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                if(order.Tokens == null || !order.Tokens.Any())
                {
                    result.Message = "Please select a dish.";
                    return result;
                }

                var outlet = await _appContext.Outlets.FindAsync(order.OutletId);
                if (outlet == null)
                {
                    result.Message = "Outlet not found!";
                    return result;
                }

                bool hasOrder = _appContext.TokenOrders.Any(e =>
                                            order.ProfileId == e.ProfileId.GetValueOrDefault() &&
                                            e.IsActive && e.Status != "cancelled" &&
                                            e.DeliveryDate.Date >= order.DeliveryDate.Date &&
                                            e.DeliveryDate.Date <= order.DeliveryDate.Date &&
                                            e.StoreId == order.StoreId &&
                                            e.MealSessionDetailId == order.MealSessionDetailId);

                if (hasOrder)
                {
                    result.Message = "Student has an existing order. Please cancel the order first before creating a new one.";
                    return result;
                }

                _appContext.AuditUserActivityType = new AuditUserActivityType
                {
                    GroupId = Common.GenerateUniqueStringId(),
                    ActionName = UserActivityType.ORDER_CREATE.ToString(),
                    Remarks = "Order was created."
                };

                var dish = order.Tokens.FirstOrDefault();

                if (!dish.MeaTypeId.HasValue)
                {
                    var mealType = await _appContext.MealTypes.FirstOrDefaultAsync();
                    dish.MeaTypeId = mealType?.Id ?? 0;
                }


                var tokenOrder = new TokenOrder
                {
                    DeliveryDate = order.DeliveryDate,
                    TransactionTime = DateTime.Now,
                    MealSessionDetailId = order.MealSessionDetailId,
                    ProfileId = order.ProfileId,
                    Status = "paid",
                    StoreId = order.StoreId,
                    CreatedBy = order.CreatedBy,
                    TotalAmount = order.TotalAmount,
                    TotalPayment = order.TotalAmount,
                    Tokens = new List<TokenOrdered>
                            {
                                new TokenOrdered
                                {
                                    TokenId = dish.MeaTypeId.Value,
                                    Qty = dish.Qty,
                                    TokenDesc = dish.TokenDesc,
                                    SelectedDishes = new List<TokenOrderDish>
                                    {
                                            new TokenOrderDish
                                            {
                                                DishId = dish.DishId,
                                                Qty = dish.Qty
                                            }
                                    }
                                }
                            }
                };

                var payment = new Payment
                {
                    StudentId = order.ProfileId,
                    email = order.StudentEmail,
                    subtotal = (decimal)order.TotalAmount,
                    total = (decimal)order.TotalAmount,
                    UserId = order.CreatedBy,
                    InvoiceNumber = invoiceNumber,
                    Status = "SUCCESS"
                };

                tokenOrder.Payment = payment;

                var f = await AddAsync(tokenOrder);
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

            _appContext.ResetAuditUserAction();
            return result;

        }

        public async Task<List<TokenOrder>> GetUnupdatedTokenOrdersAsync()
        {
            IQueryable<TokenOrder> query = _appContext.TokenOrders.Where(t => t.Status != "paid" && t.Status != "cancelled" && t.Status != "deleted" && t.Payment != null && t.Payment.Status == "SUCCESS");

            return query.ToList();
        }

        public async Task<List<MealPlanOrder>> GetUnupdatedMealPlanOrdersAsync()
        {
            IQueryable<MealPlanOrder> query = _appContext.MealPlanOrders.Where(t => t.Status != "paid" && t.Status != "cancelled" && t.Status != "deleted" && t.Payment != null && t.Payment.Status == "SUCCESS");

            return query.ToList();
        }

        public async Task<TokenOrder> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync( TokenOrder order, List<TokenOrdered> tokenOrders)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                _appContext.AuditUserActivityType = new AuditUserActivityType
                {
                    GroupId = Common.GenerateUniqueStringId(),
                    ActionName = UserActivityType.ORDER_CREATE.ToString(),
                    Remarks = "Order was created."
                };

                if (order.Status == "paid" && Math.Abs(order.TotalPayment) < 0.01) order.TotalPayment = order.TotalAmount;

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

            _appContext.ResetAuditUserAction();
            return result;

        }

        public async Task<BaseOperationResponse> UpdateAsync(TokenOrder order, List<TokenOrdered> tokenOrders)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                _appContext.AuditUserActivityType = new AuditUserActivityType
                {
                    GroupId = Common.GenerateUniqueStringId(),
                    ActionName = UserActivityType.ORDER_UPDATE.ToString(),
                    Remarks = "Order was updated."
                };

                var f = await GetSingleOrDefaultAsync(e => e.Id == order.Id);

                var tokensToDelete = this._appContext.TokenOrdereds.Where(x => x.OrderId == f.Id &&
                                    (tokenOrders == null || !tokenOrders.Any(a => a.TokenId == x.TokenId)));

                this._appContext.TokenOrdereds.RemoveRange(tokensToDelete);

                if (tokenOrders != null)
                {
                    tokenOrders.ForEach(e =>
                    {
                        if (e.TokenId != null && e.TokenId > 0)
                        {

                            var sc = this._appContext.TokenOrdereds.FirstOrDefault(x => x.Id == e.Id);
                            if (sc != null)
                            {
                                sc.TokenId = e.TokenId;
                                sc.MealTypeId = e.MealTypeId;
                                sc.TokenDesc = e.TokenDesc;
                                sc.Qty = e.Qty;
                                this._appContext.TokenOrdereds.Update(sc);
                            }
                            else
                            {
                                this._appContext.TokenOrdereds.Add(e);
                            }
                            var dishToDelete = this._appContext.TokenOrderDishes.Where(x => x.TokenOrderedId == e.Id &&
                                        (e.SelectedDishes == null || !e.SelectedDishes.Any(a => a.DishId == x.DishId)));

                            this._appContext.TokenOrderDishes.RemoveRange(dishToDelete);

                            var altDishToDelete = this._appContext.TokenAltDishes.Where(x => x.TokenOrderedId == e.Id &&
                                        (e.TokenAltDishes == null || !e.TokenAltDishes.Any(a => a.DishId == x.DishId)));

                            this._appContext.TokenAltDishes.RemoveRange(altDishToDelete);

                            e.SelectedDishes.ToList().ForEach(d =>
                            {
                                if(d.DishId != null && d.DishId > 0)
                                {
                                    var sd = this._appContext.TokenOrderDishes.FirstOrDefault(x => x.Id == d.Id);
                                    if (sd != null)
                                    {
                                        sd.TokenOrderedId = d.TokenOrderedId;
                                        sd.DishId = d.DishId;
                                        sd.Qty = d.Qty;
                                        this._appContext.TokenOrderDishes.Update(sd);
                                    }
                                    else
                                    {
                                        this._appContext.TokenOrderDishes.Add(d);
                                    }
                                }
                            });

                            e.TokenAltDishes.ToList().ForEach(d =>
                            {
                                var ad = this._appContext.TokenAltDishes.FirstOrDefault(x => x.Id == d.Id);
                                if (ad != null)
                                {
                                    ad.TokenOrderedId = d.TokenOrderedId;
                                    ad.DishId = d.DishId;
                                    this._appContext.TokenAltDishes.Update(ad);
                                }
                                else
                                {
                                    this._appContext.TokenAltDishes.Add(d);
                                }
                            });
                        }
                    });
                }

                f.CopyFrom(order);


                if (f.Status == "paid" && Math.Abs(f.TotalPayment) < 0.01) f.TotalPayment = f.TotalAmount;

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

            _appContext.ResetAuditUserAction();
            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(TokenOrder order)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.ORDER_UPDATE.ToString(),
                Remarks = "Order was updated."
            };

            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == order.Id);

            f.CopyFrom(order);

            if (f.Status == "paid" && Math.Abs(f.TotalPayment) < 0.01) f.TotalPayment = f.TotalAmount;

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

            _appContext.ResetAuditUserAction();
            return result;
        }

        public async Task<BaseOperationResponse> UpdateOrderCancelRequestStatus(int orderId, string status)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.CANCEL_ORDER_CREATE.ToString(),
                Remarks = "Cancel Order was submitted."
            };

            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == orderId);

            f.CancelRequestStatus = status;

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

            _appContext.ResetAuditUserAction();
            return result;
        }

        public async Task<BaseOperationResponse> UpdateCancellationStatus(int orderId, bool isApproved, string response)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.CANCEL_ORDER_CREATE.ToString(),
                Remarks = $"Cancel Order Request {(isApproved ? "Approved" : "Rejected")}"
            };

            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == orderId);

            if (!isApproved)
                f.CancelRequestStatus = null;
            else
            {
                f.CancelledOn = DateTime.Now;
                f.CancellationReason = response;
                f.Status = "cancelled";
            }

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

        public async Task<BaseOperationResponse> BulkCollectAsync(List<OrderCollectionDTO> orders)
        {
            var result = new BaseOperationResponse();

            if(orders != null && orders.Any())
            {
                foreach(var order in orders)
                {
                    var f = await GetSingleOrDefaultAsync(e => e.Id == order.OrderId);
                    if(f != null)
                    {
                        f.CollectionTime = order.TimeCollected;
                        Update(f);
                    }
                }

                
            }


            await _appContext.SaveChangesAsync();

            result.Message = "Successfully saved!";
            result.IsSuccess = true;

            return result;
        }

        public async Task<BaseOperationResponse> BulkReturnAsync(List<OrderReturnDTO> orders)
        {
            var result = new BaseOperationResponse();

            if (orders != null && orders.Any())
            {
                foreach (var order in orders)
                {
                    var f = await GetSingleOrDefaultAsync(e => e.Id == order.OrderId);
                    if (f != null)
                    {
                        f.BentoCode = order.BentoCode;
                        f.ReturnTime = order.TimeReturned;
                        Update(f);
                    }
                }


            }


            await _appContext.SaveChangesAsync();

            result.Message = "Successfully saved!";
            result.IsSuccess = true;

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int orderId)
        {
            _appContext.AuditUserActivityType = new AuditUserActivityType
            {
                GroupId = Common.GenerateUniqueStringId(),
                ActionName = UserActivityType.ORDER_DELETE.ToString(),
                Remarks = $"Order {orderId} deleted."
            };

            var result = new BaseOperationResponse();
            var order = await GetSingleOrDefaultAsync(r => r.Id == orderId);

            if (order != null)
                return await Delete(order);

            result.IsSuccess = false;
            result.Message = "Order not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(TokenOrder order)
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

            _appContext.ResetAuditUserAction();

            return result;
        }

        public async Task<BaseOperationResponse> CreateFasTokenOrdersAsync(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool clear)
        {
            var result = new BaseOperationResponse();
            try
            {
                string invoiceNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    var outlet = await _appContext.Outlets.FindAsync(outletId);
                    if (outlet == null)
                    {
                        result.Message = "Outlet not found!";
                        return result;
                    }

                    var fasStudents = outlet.Students.Where(e => e.OutletId == outletId && e.IsActive && e.IsFAS).ToList();
                    var studentIds = fasStudents.Select(e => e.Id).ToList();
                    //bool hasOrder = _appContext.TokenOrders.Any(e =>
                    //                            studentIds.Contains(e.ProfileId.GetValueOrDefault()) &&
                    //                            e.IsActive && e.Status != "cancelled" &&
                    //                            e.DeliveryDate.Date >= deliveryDate.Date &&
                    //                            e.DeliveryDate.Date <= deliveryDateTo.Date &&
                    //                            e.StoreId == storeId &&
                    //                            e.Session.MealSessionId == mealSessionId);

                    //if(hasOrder)
                    //{
                    //    result.Message = "Please cancel previous orders for the selected date range before assigning new orders.";
                    //    return result;
                    //}

                    while (deliveryDate.Date <= deliveryDateTo.Date)
                    {
                        var sessionDetail = _appContext.MealSessionDetails.FirstOrDefault(e => e.IsActive && e.MealSessionId == mealSessionId);

                        var activeDishCycles = _appContext.DishCycles.Where(e => e.IsActive &&
                                            e.OutletProfile.Caterer.CatererOutlets.Any(o => o.IsActive && o.OutletId == outletId) &&
                                            (deliveryDate.Date >= e.StartDate.Date &&
                                            (!e.EndDate.HasValue || e.EndDate.Value.Date >= deliveryDate.Date)) &&
                                            //e.DishTypeId == dishTypeId &&
                                            e.DishCyclePeriods.Any(d => d.MealPeriodId == sessionDetail.MealSession.MealPeriodId) &&
                                            e.CycleType == "Main Menu");

                        if (activeDishCycles == null || !activeDishCycles.Any())
                        {
                            result.Message = "Dish Cycle not found!";
                            return result;
                        }

                        //MealTypeId, DishId, Price, Label
                        var classDishKeyPair = new Dictionary<int, Tuple<int?, int?, float, string>>();
                        foreach (var student in fasStudents)
                        {
                            var existingOrders = _appContext.TokenOrders.Where(e =>
                                                    e.ProfileId == student.Id && 
                                                    e.IsActive && e.Status != "cancelled" &&
                                                    e.DeliveryDate.Date == deliveryDate &&
                                                    e.StoreId == storeId &&
                                                    e.Session.MealSessionId == mealSessionId);

                            if (clear || (existingOrders != null && existingOrders.Any()))
                            {
                                foreach (var existingOrder in existingOrders)
                                {
                                    if (existingOrder.IsFAS)
                                    {
                                        //remove token orders made
                                        existingOrder.Status = "deleted";
                                        if (existingOrder.Payment != null)
                                        {
                                            existingOrder.Payment.IsActive = false;
                                        }

                                        SoftDelete(existingOrder);
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                }

                                if(clear) continue;
                            }

                            bool hasSelectedDish = false;
                            var order = new TokenOrder
                            {
                                DeliveryDate = deliveryDate,
                                TransactionTime = DateTime.Now,
                                MealSessionDetailId = sessionDetail.Id,
                                ProfileId = student.Id,
                                Status = "paid",
                                StoreId = storeId,
                                CreatedBy = createdBy,
                                IsFAS = true
                            };

                            if (!classDishKeyPair.Keys.Any(e => e == student.ClassId))
                            {
                                var allDetailMenus = _appContext.DishCycleScheduleDetailMenus.Where(e => e.IsActive).ToList();
                                foreach (var cycle in activeDishCycles)
                                {
                                    var detailMenus = allDetailMenus.ToList();
                                    if (cycle.StartDate.Date > deliveryDate.Date)
                                        continue;

                                    //identify what day from the date passed
                                    var span = deliveryDate.Date.Subtract(cycle.StartDate.Date);
                                    int day = span.Days + 1;
                                    int d = day == 0 ? 1 : ((day % cycle.NumOfDays) == 0 ? cycle.NumOfDays : (day % cycle.NumOfDays));
                                    //var schedule = _appContext.DishCycleSchedules.FirstOrDefault(e => e.IsActive &&
                                    //                                                         e.DishCycleId == cycle.Id &&
                                    //                                                         e.Day == d);
                                    //get details and loop according to the number of sets
                                    //for (int i = 1; i <= cycle.NumOfSets; i++)
                                    //{
                                    var cycleSets = _appContext.DishCycleScheduleSets.Where(e => e.IsActive &&
                                                                                    e.DishCycleId == cycle.Id && //e.Sequence == i && 
                                                                                    e.DishCycleType.DishTypeId == dishTypeId);

                                    if (cycleSets != null)
                                    {
                                        _logger.LogInformation($"CreateFasTokenOrdersAsync INFO Day: {d}");
                                        foreach (var cycleSet in cycleSets)
                                        {
                                            _logger.LogInformation($"CreateFasTokenOrdersAsync INFO 1: {cycleSet.Label} - {cycleSet.CycleTypeId} -  {cycleSet.CycleTypeSequence}");

                                            var subSchedules = _appContext.DishCycleSchedules.Where(e => e.IsActive && e.DishCycleId == cycleSet.CycleTypeId);

                                            if (subSchedules != null && subSchedules.Any())
                                            {
                                                int subScheduleDays = subSchedules.Count();
                                                _logger.LogInformation($"CreateFasTokenOrdersAsync INFO subSchedules: {subScheduleDays}");

                                                int subDay = d;
                                                if (d > subScheduleDays)
                                                {
                                                    subDay = d % subScheduleDays;
                                                }

                                                _logger.LogInformation($"CreateFasTokenOrdersAsync INFO Sub Day: {subDay}");

                                                var subSchedule = subSchedules.FirstOrDefault(e => e.Day == subDay);
                                                if (subSchedule != null && subSchedule.Details != null)
                                                {
                                                    var subScheduleDetail = subSchedule.Details.FirstOrDefault(e => e.Sequence == cycleSet.CycleTypeSequence);
                                                    if (subScheduleDetail != null)
                                                    {
                                                        detailMenus = subScheduleDetail.Menus.ToList();
                                                        _logger.LogInformation($"CreateFasTokenOrdersAsync INFO 2: {subScheduleDetail.Label} - {subScheduleDetail.DishCycleId} - {subScheduleDetail.DishCycleScheduleId} - Menu COUNT: {detailMenus.Count}");
                                                        _logger.LogInformation($"CreateFasTokenOrdersAsync INFO 2a: Menus: {string.Join(",", detailMenus.Select(e => e.Dish.Label))}");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                _logger.LogInformation($"CreateFasTokenOrdersAsync INFO subSchedules: subSchedules is null or empty");
                                            }


                                            if (detailMenus.Any())
                                            {
                                                var tuple = new Tuple<int?, int?, float, string>(cycle.MealTypeId, detailMenus.First().DishId, (float)cycleSet.Price, cycleSet.Label);
                                                classDishKeyPair.Add(student.ClassId, tuple);
                                                hasSelectedDish = true;
                                                break;
                                            }
                                        }

                                    }

                                    if (hasSelectedDish) break;
                                }
                            }

                            if (classDishKeyPair.Keys.Any(e => e == student.ClassId))
                            {
                                var kp = classDishKeyPair[student.ClassId];
                                order.TotalAmount = kp.Item3;
                                order.TotalPayment = kp.Item3;
                                order.Tokens = new List<TokenOrdered>
                            {
                                new TokenOrdered
                                {
                                    TokenId = kp.Item1.Value,
                                    Qty = 1,
                                    TokenDesc = kp.Item4,
                                    SelectedDishes = new List<TokenOrderDish>
                                    {
                                            new TokenOrderDish
                                            {
                                                DishId = kp.Item2,
                                                Qty = 1
                                            }
                                    }
                                }
                            };

                                var payment = new Payment
                                {
                                    StudentId = order.ProfileId,
                                    email = student.Email,
                                    subtotal = (decimal)order.TotalAmount,
                                    total = (decimal)order.TotalAmount,
                                    UserId = createdBy,
                                    InvoiceNumber = invoiceNumber,
                                    Status = "SUCCESS"
                                };

                                order.Payment = payment;
                                await _appContext.TokenOrders.AddAsync(order);
                            }
                        }

                        deliveryDate = deliveryDate.Date.AddDays(1);
                    }
                    

                    await _appContext.SaveChangesAsync();

                    result.Message = "Successfully processed!";
                    result.IsSuccess = true;
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                result.Message = "Failed to process orders!";
                _logger.LogError($"CreateFasTokenOrdersAsync EXCEPTION : {ex.InnerException?.StackTrace} - {ex.Message} - {ex.StackTrace}");
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<List<spGetFasTokenOrderSummary>> GetFasTokenOrders(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, IEnumerable<int> mealSessionIds)
        {
            var from = new SqlParameter("@deliveryDate", System.Data.SqlDbType.Date);
            var to = new SqlParameter("@deliveryDateTo", System.Data.SqlDbType.Date);
            var pOutletId = new SqlParameter("@outletId", System.Data.SqlDbType.Int);
            var pStoreId = new SqlParameter("@storeId", System.Data.SqlDbType.Int);
            var pMealSessionIds = new SqlParameter("@mealSessionIds", System.Data.SqlDbType.VarChar);

            from.Value = deliveryDate;
            to.Value = deliveryDateTo;
            pOutletId.Value = outletId;
            pStoreId.Value = storeId;
            pMealSessionIds.Value = string.Join(",", mealSessionIds);

            var orders = await _appContext.spGetFasTokenOrderSummary
                            .FromSql($"exec spGetFasTokenOrderSummary @outletId, @storeId, @deliveryDate, @deliveryDateTo, @mealSessionIds",
                                    from, to, pOutletId, pStoreId, pMealSessionIds).ToListAsync();

            return orders;
        }

        public async Task<FasTokenOrderSummaryDTO> GetFasTokenOrderSummaryAsync(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, List<MealSessionDetail> mealSessionDetails)
        {
            var mealSessionIds = mealSessionDetails.Select(e => e.MealSessionId).ToList();
            var mealSessions = _appContext.MealSessions.Where(e => mealSessionIds.Any(f => f == e.Id)).Select(e => e.Name).ToList();
            var orders = await GetFasTokenOrders(outletId, storeId, deliveryDate, deliveryDateTo, mealSessionIds);
            var grpOrders = orders.GroupBy(e => new { e.DeliveryDate });
            //var colspan = grpOrders.Select(e => e.FirstOrDefault().DishTypeName).Count();
            var dishTypes = _appContext.DishTypes.Where(e => e.IsActive == e.Caterer.CatererOutlets.Any(f => f.OutletId == outletId)).OrderBy(e => e.Name).ToList();
            var colspan = dishTypes.Count();
            string dateFormat = "dd/MM/yy";

            var summary = new FasTokenOrderSummaryDTO { Total = new FasTokenOrderRowDTO { Cells = new List<FasTokenOrderCellDTO>() } };

            // build header
            var row = new FasTokenOrderRowDTO();
            row.Cells.Add(new FasTokenOrderCellDTO { Val = "Session", Colspan = 1, Rowspan = 2 });
            var currentDate = deliveryDate.Date;
            while (currentDate <= deliveryDateTo.Date)
            {
                row.Cells.Add(new FasTokenOrderCellDTO { Val = string.Format("{0}", currentDate.ToString(dateFormat)), Colspan = colspan, Rowspan = 1 });
                currentDate = currentDate.Date.AddDays(1);
            }

            summary.Headers.Add(row);

            row = new FasTokenOrderRowDTO();
            currentDate = deliveryDate.Date;
            while (currentDate <= deliveryDateTo.Date)
            {
                // build new header
                foreach (var dishType in dishTypes)
                {
                    row.Cells.Add(new FasTokenOrderCellDTO { Val = dishType.Name, Colspan = 1, Rowspan = 1 });
                }

                currentDate = currentDate.Date.AddDays(1);
            }

            summary.Headers.Add(row);

            // build rows
            foreach (var mealSession in mealSessions)
            {
                row = new FasTokenOrderRowDTO();
                row.Cells.Add(new FasTokenOrderCellDTO { Val = mealSession, Colspan = 1, Rowspan = 1 });

                currentDate = deliveryDate.Date;
                while (currentDate <= deliveryDateTo.Date)
                {
                    foreach (var dishType in dishTypes)
                    {
                        var qty = orders.FirstOrDefault(e => e.DeliveryDate.Date == currentDate.Date &&
                                                        e.Session.Equals(mealSession, StringComparison.OrdinalIgnoreCase) &&
                                                        e.DishTypeName.Equals(dishType.Name, StringComparison.OrdinalIgnoreCase))?.Quantity ?? 0;

                        row.Cells.Add(new FasTokenOrderCellDTO { Val = qty.ToString(), Colspan = 1, Rowspan = 1 });
                    }

                    currentDate = currentDate.Date.AddDays(1);
                }

                summary.Rows.Add(row);
            }

            row = new FasTokenOrderRowDTO();
            //currentDate = deliveryDate.Date;
            //while (currentDate <= deliveryDateTo.Date)
            //{
            //    // build new header
            //    foreach (var dishType in dishTypes)
            //    {
            //        row.Cells.Add(new FasTokenOrderCellDTO { Val = dishType.Name, Colspan = 1, Rowspan = 1 });
            //    }

            //    currentDate = currentDate.Date.AddDays(1);
            //}

            summary.Total = row;

            summary.Total.Cells.Add(new FasTokenOrderCellDTO { Val = "Total", Colspan = 1, Rowspan = 1 });
            if (summary.Rows.Any() && summary.Rows.First().Cells.Any())
            {
                for (int i = 1; i < summary.Rows.First().Cells.Count; i++)
                {
                    var totalByCol = summary.Rows.Select(e => int.Parse(e.Cells[i].Val)).Sum();
                    summary.Total.Cells.Add(new FasTokenOrderCellDTO { Val = totalByCol.ToString(), Colspan = 1, Rowspan = 1 });
                }
            }


            return summary;

            //summary.Cols.Add(toCol);

            //foreach (var mealSession in mealSessions)
            //{
            //    var row = new FasTokenOrderRowDTO();
            //    row.Cells.Add(string.Format("{0}", mealSession));

            //    foreach (var dishType in dishTypes)
            //    {
            //        var totalByDishType = tokenOrderSelectedDishes.Where(e => e.TokenOrdered.Order.Session.MealSession.Name == mealSession &&
            //                                                e.Dish.DishTypeId == dishType.Id && e.TokenOrdered.Order.DeliveryDate.Date == deliveryDate.Date).Sum(f => (int)f.Qty);

            //        row.Cells.Add(totalByDishType.ToString());
            //    }

            //    summary.Rows.Add(row);
            //}

            //deliveryDate.Date.AddDays(1);
            //var outlet = await _appContext.Outlets.FindAsync(outletId);
            //var fasStudents = outlet.Students.Where(e => e.OutletId == outletId && e.IsActive && e.IsFAS).ToList();

            //var fasOrders = _appContext.TokenOrders.Where(t => t.StoreId == storeId && t.IsFAS && t.Status != "cancelled" &&
            //                               (t.DeliveryDate.Date >= deliveryDate.Date && t.DeliveryDate.Date <= deliveryDateTo.Date)
            //                               && t.IsActive && fasStudents.Any(f => f.Id == t.ProfileId)).ToList();

            //var dishTypes = _appContext.DishTypes.Where(e => e.IsActive == e.Caterer.CatererOutlets.Any(f => f.OutletId == outletId)).OrderBy(e => e.Name).ToList();
            //var tokenOrderSelectedDishes = _appContext.TokenOrderDishes.Where(e => e.IsActive && fasOrders.Any(f => f.Id == e.TokenOrdered.OrderId)).ToList();

            //TimeSpan difference = deliveryDateTo.Date - deliveryDate.Date;

            //// Get the total number of days
            //int numberOfDays = difference.Days;
            //var summary = new FasTokenOrderSummaryDTO { Total = new FasTokenOrderRowDTO { Cells = new List<string>() } };
            //summary.Cols.Add(new FasTokenOrderColDTO { Header = "Session", Rowspan = 2, Colspan = 1 });

            //while (deliveryDate.Date <= deliveryDateTo.Date)
            //{
            //    var toCol = new FasTokenOrderColDTO
            //    {
            //        Header = deliveryDate.Date.ToShortDateString(),
            //        Rowspan = 1,
            //        Colspan = numberOfDays,
            //        Subheaders = new List<string>()
            //    };

            //    foreach (var dishType in dishTypes)
            //    {
            //        toCol.Subheaders.Add(dishType.Name);
            //    }

            //    summary.Cols.Add(toCol);

            //    foreach (var mealSession in mealSessions)
            //    {
            //        var row = new FasTokenOrderRowDTO();
            //        row.Cells.Add(string.Format("{0}", mealSession));

            //        foreach (var dishType in dishTypes)
            //        {
            //            var totalByDishType = tokenOrderSelectedDishes.Where(e => e.TokenOrdered.Order.Session.MealSession.Name == mealSession &&
            //                                                    e.Dish.DishTypeId == dishType.Id && e.TokenOrdered.Order.DeliveryDate.Date == deliveryDate.Date).Sum(f => (int)f.Qty);

            //            row.Cells.Add(totalByDishType.ToString());
            //        }

            //        summary.Rows.Add(row);
            //    }

            //    deliveryDate.Date.AddDays(1);

            //}

            //summary.Total.Cells.Add("Total Quantity");
            //if (summary.Rows.Any() && summary.Rows.First().Cells.Any())
            //{
            //    for (int i = 1; i < summary.Rows.First().Cells.Count; i++)
            //    {
            //        var totalByCol = summary.Rows.Select(e => int.Parse(e.Cells[i])).Sum();
            //        summary.Total.Cells.Add(totalByCol.ToString());
            //    }
            //}

            //return summary;
        }

        #region Meal Plan

        public async Task<List<StudentGroupMealPlan>> GetStudentMealPlansAsync(int studentId, DateTime? orderDate)
        {
            var date = orderDate != null ? orderDate?.Date : DateTime.Today;

            IQueryable<StudentGroupMealPlan> query = _appContext.StudentGroupMealPlans.Where(t => t.IsActive && t.StudentGroup.IsActive && 
                                                    t.StudentGroup.Sgdetails.Any(x => x.StudentId == studentId) &&
                                                    t.StudentGroup.Type == StudentMealType.MEAL_PLAN && 
                                                    t.StudentGroup.StartDate.HasValue && t.StudentGroup.StartDate.Value <= date &&
                                                    t.StudentGroup.EndDate.HasValue && date <= t.StudentGroup.EndDate.Value);

            return query.ToList();
        }

        public async Task<BaseOperationResponse> CreateMealPlanAsync(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool skip = true)
        {
            var result = new BaseOperationResponse();
            try
            {
                string invoiceNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    var outlet = await _appContext.Outlets.FindAsync(outletId);
                    if (outlet == null)
                    {
                        result.Message = "Outlet not found!";
                        return result;
                    }

                    var studentGroup = await _appContext.StudentGroups.FirstOrDefaultAsync(e => e.Id == studentGroupId);

                    if (studentGroup == null)
                    {
                        result.Message = "Student Group not found!";
                        return result;
                    }

                    //bool hasOrder = _appContext.TokenOrders.Any(e =>
                    //                            //studentIds.Contains(e.ProfileId.GetValueOrDefault()) &&
                    //                            e.IsActive && e.Status != "cancelled" &&
                    //                            e.DeliveryDate.Date >= deliveryDate.Date &&
                    //                            e.DeliveryDate.Date <= deliveryDateTo.Date &&
                    //                            e.StoreId == storeId &&
                    //                            e.Session.MealSessionId == mealSessionId &&
                    //                            //e.StudentGroupId == studentGroupId &&
                    //                            e.IsActive);

                    //if (hasOrder)
                    //{
                    //    result.Message = "Please cancel previous orders for the selected date range before assigning new orders.";
                    //    return result;
                    //}

                    var mealPlans = new List<StudentGroupMealPlan>();
                    var mealPlansToDelete = new List<StudentGroupMealPlan>();
                    var sessionDetail = _appContext.MealSessionDetails.FirstOrDefault(e => e.IsActive && e.MealSessionId == mealSessionId);

                    // select dishes for meal plan first
                    while (deliveryDate.Date <= deliveryDateTo.Date)
                    {
                        var existingMealPlans = _appContext.StudentGroupMealPlans.Where(e => e.IsActive && e.StudentGroupId == studentGroupId &&
                                        e.DeliveryDate.Date == deliveryDate.Date && e.MealSessionId == mealSessionId);

                        if (existingMealPlans != null)
                        {
                            var plans = existingMealPlans.Where(x => x.Dish.DishTypeId == dishTypeId);
                            if (plans.Any())
                            {
                                mealPlans.AddRange(plans);
                                deliveryDate = deliveryDate.Date.AddDays(1);
                                continue;
                            }
                            else
                            {
                                // replace old dish assigned to this date
                                mealPlansToDelete.AddRange(existingMealPlans);
                            }
                        }

                        var activeDishCycles = _appContext.DishCycles.Where(e => e.IsActive &&
                                            e.OutletProfile.Caterer.CatererOutlets.Any(o => o.IsActive && o.OutletId == outletId) &&
                                            (deliveryDate.Date >= e.StartDate.Date &&
                                            (!e.EndDate.HasValue || e.EndDate.Value.Date >= deliveryDate.Date)) &&
                                            e.DishCyclePeriods.Any(d => d.MealPeriodId == sessionDetail.MealSession.MealPeriodId) &&
                                            e.CycleType == "Main Menu");

                        if (activeDishCycles == null || !activeDishCycles.Any())
                        {
                            result.Message = "Dish Cycle not found!";
                            return result;
                        }

                        var allDetailMenus = _appContext.DishCycleScheduleDetailMenus.Where(e => e.IsActive).ToList();
                        
                        foreach (var cycle in activeDishCycles)
                        {
                            var detailMenus = allDetailMenus.ToList();
                            if (cycle.StartDate.Date > deliveryDate.Date)
                                continue;

                            //identify what day from the date passed
                            var span = deliveryDate.Date.Subtract(cycle.StartDate.Date);
                            int day = span.Days + 1;
                            int d = day == 0 ? 1 : ((day % cycle.NumOfDays) == 0 ? cycle.NumOfDays : (day % cycle.NumOfDays));
                            var cycleSets = _appContext.DishCycleScheduleSets.Where(e => e.IsActive &&
                                                                            e.DishCycleId == cycle.Id && //e.Sequence == i && 
                                                                            e.DishCycleType.DishTypeId == dishTypeId);
                            bool hasSelectedDish = false;
                            if (cycleSets != null)
                            {
                                _logger.LogInformation($"CreateMealPlanAsync INFO Day: {d}");
                                foreach (var cycleSet in cycleSets)
                                {
                                    _logger.LogInformation($"CreateMealPlanAsync INFO 1: {cycleSet.Label} - {cycleSet.CycleTypeId} -  {cycleSet.CycleTypeSequence}");

                                    var subSchedules = _appContext.DishCycleSchedules.Where(e => e.IsActive && e.DishCycleId == cycleSet.CycleTypeId);

                                    if (subSchedules != null && subSchedules.Any())
                                    {
                                        int subScheduleDays = subSchedules.Count();
                                        _logger.LogInformation($"CreateMealPlanAsync INFO subSchedules: {subScheduleDays}");

                                        int subDay = d;
                                        if (d > subScheduleDays)
                                        {
                                            subDay = d % subScheduleDays;
                                        }

                                        _logger.LogInformation($"CreateMealPlanAsync INFO Sub Day: {subDay}");

                                        var subSchedule = subSchedules.FirstOrDefault(e => e.Day == subDay);
                                        if (subSchedule != null && subSchedule.Details != null)
                                        {
                                            var subScheduleDetail = subSchedule.Details.FirstOrDefault(e => e.Sequence == cycleSet.CycleTypeSequence);
                                            if (subScheduleDetail != null)
                                            {
                                                detailMenus = subScheduleDetail.Menus.Where(e=> e.Dish.DishTypeId == dishTypeId).ToList();
                                                _logger.LogInformation($"CreateMealPlanAsync INFO 2: {subScheduleDetail.Label} - {subScheduleDetail.DishCycleId} - {subScheduleDetail.DishCycleScheduleId} - Menu COUNT: {detailMenus.Count}");
                                                _logger.LogInformation($"CreateMealPlanAsync INFO 2a: Menus: {string.Join(",", detailMenus.Select(e => e.Dish.Label))}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation($"CreateMealPlanAsync INFO subSchedules: subSchedules is null or empty");
                                    }


                                    if (detailMenus.Any())
                                    {
                                        var tuple = new Tuple<int?, int?, float, string>(cycle.MealTypeId, detailMenus.First().DishId, (float)cycleSet.Price, cycleSet.Label);
                                        mealPlans.Add(new StudentGroupMealPlan
                                        {
                                            DishId = detailMenus.First().DishId.Value,
                                            MealSessionId = mealSessionId,
                                            StudentGroupId = studentGroupId,
                                            MealSessionDetailId = sessionDetail.Id,
                                            Price = (float)cycleSet.Price,
                                            Label = cycleSet.Label,
                                            MealTypeId = cycle.MealTypeId,
                                            StoreId = storeId,
                                            DeliveryDate = deliveryDate.Date
                                        });

                                        hasSelectedDish = true;
                                        break;
                                    }
                                }

                            }

                            if (hasSelectedDish) break;
                        }

                        deliveryDate = deliveryDate.Date.AddDays(1);
                    }

                    //save meal plans - update, create, delete
                    var currentMealPlans = _appContext.StudentGroupMealPlans.Where(e => e.IsActive && e.StudentGroupId == studentGroupId);
                    foreach(var currentMealPlan in currentMealPlans)
                    {
                        //disable first
                        //currentMealPlan.IsActive = false;
                        var mp = mealPlans.FirstOrDefault(e => e.DeliveryDate.Date == currentMealPlan.DeliveryDate.Date && e.MealTypeId == currentMealPlan.MealTypeId &&
                                        e.DishId == currentMealPlan.DishId && e.MealSessionDetailId == currentMealPlan.MealSessionDetailId);

                        if (mp != null)
                        {
                            currentMealPlan.IsActive = true;
                            currentMealPlan.Price = mp.Price;
                            currentMealPlan.Label = mp.Label;
                            _appContext.StudentGroupMealPlans.Update(currentMealPlan);
                        }
                    }

                    foreach (var mp in mealPlansToDelete)
                    {
                        //disable first
                        mp.IsActive = false;
                        _appContext.StudentGroupMealPlans.Update(mp);
                    }

                    //add new ones
                    var newMps = mealPlans.Where(e => !currentMealPlans.Any(f => f.StudentGroupId == e.StudentGroupId && e.DeliveryDate.Date == f.DeliveryDate.Date && e.MealTypeId == f.MealTypeId &&
                                         e.DishId == f.DishId && e.MealSessionDetailId == f.MealSessionDetailId));

                    await _appContext.StudentGroupMealPlans.AddRangeAsync(newMps);

                    foreach (var mealPlan in mealPlans.OrderBy(e => e.DeliveryDate))
                    {
                        foreach (var detail in studentGroup.Sgdetails)
                        {
                            var student = detail.Student;
                            var existingOrders = _appContext.TokenOrders.Where(e =>
                                                    e.ProfileId == student.Id &&
                                                    e.IsActive && e.Status != "cancelled" &&
                                                    e.DeliveryDate.Date == mealPlan.DeliveryDate.Date &&
                                                    e.StoreId == storeId && e.IsActive &&
                                                    e.Session.MealSessionId == mealSessionId);
                            bool _skip = true;
                            if (skip || (existingOrders != null && existingOrders.Any()))
                            {
                                foreach (var existingOrder in existingOrders)
                                {
                                    if (existingOrder.IsMealPlan)
                                    {
                                        //remove token orders made
                                        existingOrder.Status = "deleted";
                                        if (existingOrder.Payment != null)
                                        {
                                            existingOrder.Payment.IsActive = false;
                                        }

                                        SoftDelete(existingOrder);
                                        _skip = false;
                                    }

                                }

                                if (_skip) continue;
                            }

                            var order = new TokenOrder
                            {
                                DeliveryDate = mealPlan.DeliveryDate.Date,
                                TransactionTime = DateTime.Now,
                                MealSessionDetailId = sessionDetail.Id,
                                ProfileId = student.Id,
                                Status = "paid",
                                StoreId = storeId,
                                CreatedBy = createdBy,
                                StudentGroupId = studentGroupId,
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

                            var payment = new Payment
                            {
                                StudentId = order.ProfileId,
                                email = student.Email,
                                subtotal = (decimal)order.TotalAmount,
                                total = (decimal)order.TotalAmount,
                                UserId = createdBy,
                                InvoiceNumber = invoiceNumber,
                                Status = "SUCCESS"
                            };

                            order.Payment = payment;
                            await _appContext.TokenOrders.AddAsync(order);
                        }
                    }

                    await _appContext.SaveChangesAsync();

                    result.Message = "Successfully processed!";
                    result.IsSuccess = true;
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                result.Message = "Failed to process orders!";
                _logger.LogError($"CreateMealPlanAsync EXCEPTION : {ex.InnerException?.StackTrace} - {ex.Message} - {ex.StackTrace}");
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<MealPlanTokenOrderSummaryDTO> GetMealPlanOrderSummaryAsync(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, List<MealSessionDetail> mealSessionDetails)
        {
            var summary = new MealPlanTokenOrderSummaryDTO { Total = new MealPlanTokenOrderRowDTO { Cells = new List<string>() } };
            //var outlet = await _appContext.Outlets.FindAsync(outletId);
            var studentGroup = await _appContext.StudentGroups.FirstOrDefaultAsync(e => e.Id == studentGroupId);

            var students = studentGroup.Sgdetails.Where(e => e.IsActive).Select(e => e.Student).ToList();

            var orders = _appContext.TokenOrders.Where(t => t.StoreId == storeId && t.StudentGroupId == studentGroupId && t.Status != "cancelled" &&
                                           (t.DeliveryDate.Date >= deliveryDate.Date && t.DeliveryDate.Date <= deliveryDateTo.Date)
                                           && t.IsActive && t.IsMealPlan && students.Any(f => f.Id == t.ProfileId)).ToList();

            var dishTypes = _appContext.DishTypes.Where(e => e.IsActive == e.Caterer.CatererOutlets.Any(f => f.OutletId == outletId)).OrderBy(e => e.Name).ToList();
            var tokenOrderSelectedDishes = _appContext.TokenOrderDishes.Where(e => e.IsActive && orders.Any(f => f.Id == e.TokenOrdered.OrderId)).ToList();

            var mealSessionIds = studentGroup.MealSessions.Select(e => e.MealSessionId).ToList();
            var mealSessions = _appContext.MealSessions.Where(e => mealSessionIds.Any(f => f == e.Id)).Select(e => e.Name).ToList();

            var mealPlans = _appContext.StudentGroupMealPlans.Where(e => e.IsActive && e.StudentGroupId == studentGroupId);
            summary.Cols.Add("Session");
            //var dishTypeIds = orders.SelectMany(e => e.Tokens.SelectMany(f => f.SelectedDishes).Select(x => x.Dish.DishTypeId)).Distinct();
            var dishTypeIds = mealPlans.Select(e => e.Dish.DishTypeId).Distinct();
            dishTypes = dishTypes.Where(e => dishTypeIds.Any(f => f == e.Id)).OrderBy(e => e.Name).ToList();
            foreach (var dishType in dishTypes)
            {
                summary.Cols.Add(dishType.Name);
            }


            foreach (var mealSession in mealSessions)
            {
                var row = new MealPlanTokenOrderRowDTO();
                row.Cells.Add(string.Format("{0}", mealSession));

                foreach (var dishType in dishTypes)
                {
                    //var tokens = fasOrders.Where(e => e.MealSessionDetailId == mealSessionDetail.Id).SelectMany(e => e.Tokens);
                    var totalByDishType = tokenOrderSelectedDishes.Where(e => e.TokenOrdered.Order.Session.MealSession.Name == mealSession &&
                                                            e.Dish.DishTypeId == dishType.Id).Sum(f => (int)f.Qty);

                    row.Cells.Add(totalByDishType.ToString());
                }

                summary.Rows.Add(row);
            }

            summary.Total.Cells.Add("Total Quantity");
            if (summary.Rows.Any() && summary.Rows.First().Cells.Any())
            {
                for (int i = 1; i < summary.Rows.First().Cells.Count; i++)
                {
                    var totalByCol = summary.Rows.Select(e => int.Parse(e.Cells[i])).Sum();
                    summary.Total.Cells.Add(totalByCol.ToString());
                }
            }

            return summary;
        }

        #endregion

        #region Student Group Order

        public async Task<BaseOperationResponse> CreateStudentGroupOrderAsync(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool skip = true)
        {
            var result = new BaseOperationResponse();
            try
            {
                string invoiceNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    var outlet = await _appContext.Outlets.FindAsync(outletId);
                    if (outlet == null)
                    {
                        result.Message = "Outlet not found!";
                        return result;
                    }

                    var studentGroup = await _appContext.StudentGroups.FirstOrDefaultAsync(e => e.Id == studentGroupId);

                    if (studentGroup == null)
                    {
                        result.Message = "Student Group not found!";
                        return result;
                    }

                    var paymentType = _appContext.PaymentTypes.FirstOrDefault(e => e.Name == "Adhoc");

                    if (paymentType == null)
                    {
                        result.Message = "Payment type 'Adhoc' not found. Please create the payment type first.";
                        return result;
                    }

                    //bool hasOrder = _appContext.TokenOrders.Any(e =>
                    //                            //studentIds.Contains(e.ProfileId.GetValueOrDefault()) &&
                    //                            e.IsActive && e.Status != "cancelled" &&
                    //                            e.DeliveryDate.Date >= deliveryDate.Date &&
                    //                            e.DeliveryDate.Date <= deliveryDateTo.Date &&
                    //                            e.StoreId == storeId &&
                    //                            e.Session.MealSessionId == mealSessionId);
                    //                            //e.StudentGroupId == studentGroupId &&
                    //                           // e.IsStudentGroupOrder);

                    //if (hasOrder)
                    //{
                    //    result.Message = "Please cancel previous orders for the selected date range before assigning new orders.";
                    //    return result;
                    //}

                    var sessionDetail = _appContext.MealSessionDetails.FirstOrDefault(e => e.IsActive && e.MealSessionId == mealSessionId);
                    var hasExistingOrders = false;
                    // select dishes first
                    while (deliveryDate.Date <= deliveryDateTo.Date)
                    {
                        var activeDishCycles = _appContext.DishCycles.Where(e => e.IsActive &&
                                            e.OutletProfile.Caterer.CatererOutlets.Any(o => o.IsActive && o.OutletId == outletId) &&
                                            (deliveryDate.Date >= e.StartDate.Date &&
                                            (!e.EndDate.HasValue || e.EndDate.Value.Date >= deliveryDate.Date)) &&
                                            //e.DishTypeId == dishTypeId &&
                                            e.DishCyclePeriods.Any(d => d.MealPeriodId == sessionDetail.MealSession.MealPeriodId) &&
                                            e.CycleType == "Main Menu");

                        if (activeDishCycles == null || !activeDishCycles.Any())
                        {
                            result.Message = "Dish Cycle not found!";
                            return result;
                        }

                        //MealTypeId, DishId, Price, Label
                        var classDishKeyPair = new Dictionary<int, Tuple<int?, int?, float, string>>();
                        foreach (var detail in studentGroup.Sgdetails)
                        {
                            var student = detail.Student;
                            var existingOrders = _appContext.TokenOrders.Where(e =>
                                                    e.ProfileId == student.Id &&
                                                    e.IsActive && e.Status != "cancelled" &&
                                                    e.DeliveryDate.Date == deliveryDate &&
                                                    e.StoreId == storeId &&
                                                    e.Session.MealSessionId == mealSessionId);
                            bool _skip = true;
                            if (skip || (existingOrders != null && existingOrders.Any()))
                            {
                                foreach (var existingOrder in existingOrders)
                                {
                                    if (existingOrder.IsStudentGroupOrder)
                                    {
                                        //remove token orders made
                                        existingOrder.Status = "deleted";
                                        if (existingOrder.Payment != null)
                                        {
                                            existingOrder.Payment.IsActive = false;
                                        }

                                        SoftDelete(existingOrder);
                                        _skip = false;
                                    }
                                    else
                                    {
                                        hasExistingOrders = true;
                                        existingOrder.Status = "cancelled";
                                        existingOrder.CancelledById = createdBy;
                                        existingOrder.CancelledOn = DateTime.Now;
                                        existingOrder.CancellationReason = "Cancelled by the system. Adhoc order has been made.";
                                    }

                                }

                                if (_skip) continue;
                            }

                            bool hasSelectedDish = false;
                            var order = new TokenOrder
                            {
                                DeliveryDate = deliveryDate,
                                TransactionTime = DateTime.Now,
                                MealSessionDetailId = sessionDetail.Id,
                                ProfileId = student.Id,
                                Status = "paid",
                                StoreId = storeId,
                                CreatedBy = createdBy,
                                IsStudentGroupOrder = true,
                                StudentGroupId = studentGroupId
                            };

                            if (!classDishKeyPair.Keys.Any(e => e == student.ClassId))
                            {
                                var allDetailMenus = _appContext.DishCycleScheduleDetailMenus.Where(e => e.IsActive).ToList();
                                foreach (var cycle in activeDishCycles)
                                {
                                    var detailMenus = allDetailMenus.ToList();
                                    if (cycle.StartDate.Date > deliveryDate.Date)
                                        continue;

                                    //identify what day from the date passed
                                    var span = deliveryDate.Date.Subtract(cycle.StartDate.Date);
                                    int day = span.Days + 1;
                                    int d = day == 0 ? 1 : ((day % cycle.NumOfDays) == 0 ? cycle.NumOfDays : (day % cycle.NumOfDays));
                                    //var schedule = _appContext.DishCycleSchedules.FirstOrDefault(e => e.IsActive &&
                                    //                                                         e.DishCycleId == cycle.Id &&
                                    //                                                         e.Day == d);
                                    //get details and loop according to the number of sets
                                    //for (int i = 1; i <= cycle.NumOfSets; i++)
                                    //{
                                    var cycleSets = _appContext.DishCycleScheduleSets.Where(e => e.IsActive &&
                                                                                    e.DishCycleId == cycle.Id && //e.Sequence == i && 
                                                                                    e.DishCycleType.DishTypeId == dishTypeId);

                                    if (cycleSets != null)
                                    {
                                        _logger.LogInformation($"CreateFasTokenOrdersAsync INFO Day: {d}");
                                        foreach (var cycleSet in cycleSets)
                                        {
                                            _logger.LogInformation($"CreateFasTokenOrdersAsync INFO 1: {cycleSet.Label} - {cycleSet.CycleTypeId} -  {cycleSet.CycleTypeSequence}");

                                            var subSchedules = _appContext.DishCycleSchedules.Where(e => e.IsActive && e.DishCycleId == cycleSet.CycleTypeId);

                                            if (subSchedules != null && subSchedules.Any())
                                            {
                                                int subScheduleDays = subSchedules.Count();
                                                _logger.LogInformation($"CreateFasTokenOrdersAsync INFO subSchedules: {subScheduleDays}");

                                                int subDay = d;
                                                if (d > subScheduleDays)
                                                {
                                                    subDay = d % subScheduleDays;
                                                }

                                                _logger.LogInformation($"CreateFasTokenOrdersAsync INFO Sub Day: {subDay}");

                                                var subSchedule = subSchedules.FirstOrDefault(e => e.Day == subDay);
                                                if (subSchedule != null && subSchedule.Details != null)
                                                {
                                                    var subScheduleDetail = subSchedule.Details.FirstOrDefault(e => e.Sequence == cycleSet.CycleTypeSequence);
                                                    if (subScheduleDetail != null)
                                                    {
                                                        detailMenus = subScheduleDetail.Menus.ToList();
                                                        _logger.LogInformation($"CreateFasTokenOrdersAsync INFO 2: {subScheduleDetail.Label} - {subScheduleDetail.DishCycleId} - {subScheduleDetail.DishCycleScheduleId} - Menu COUNT: {detailMenus.Count}");
                                                        _logger.LogInformation($"CreateFasTokenOrdersAsync INFO 2a: Menus: {string.Join(",", detailMenus.Select(e => e.Dish.Label))}");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                _logger.LogInformation($"CreateFasTokenOrdersAsync INFO subSchedules: subSchedules is null or empty");
                                            }


                                            if (detailMenus.Any())
                                            {
                                                var tuple = new Tuple<int?, int?, float, string>(cycle.MealTypeId, detailMenus.First().DishId, (float)cycleSet.Price, cycleSet.Label);
                                                classDishKeyPair.Add(student.ClassId, tuple);
                                                hasSelectedDish = true;
                                                break;
                                            }
                                        }

                                    }

                                    if (hasSelectedDish) break;
                                }
                            }

                            if (classDishKeyPair.Keys.Any(e => e == student.ClassId))
                            {
                                var kp = classDishKeyPair[student.ClassId];
                                order.TotalAmount = kp.Item3;
                                order.TotalPayment = kp.Item3;
                                order.Tokens = new List<TokenOrdered>
                            {
                                new TokenOrdered
                                {
                                    TokenId = kp.Item1.Value,
                                    Qty = 1,
                                    TokenDesc = kp.Item4,
                                    SelectedDishes = new List<TokenOrderDish>
                                    {
                                            new TokenOrderDish
                                            {
                                                DishId = kp.Item2,
                                                Qty = 1
                                            }
                                    }
                                }
                            };

                                var payment = new Payment
                                {
                                    StudentId = order.ProfileId,
                                    email = student.Email,
                                    subtotal = (decimal)order.TotalAmount,
                                    total = (decimal)order.TotalAmount,
                                    UserId = createdBy,
                                    InvoiceNumber = invoiceNumber,
                                    Status = "SUCCESS",
                                    PaymentTypeId = paymentType.Id
                                };

                                order.Payment = payment;
                                await _appContext.TokenOrders.AddAsync(order);
                            }
                        }

                        deliveryDate = deliveryDate.Date.AddDays(1);
                    }

                    await _appContext.SaveChangesAsync();

                    result.Message = hasExistingOrders ? "Successfully processed! Existing orders were found and were cancelled automatically." : "Successfully processed!";
                    result.IsSuccess = true;
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                result.Message = "Failed to process orders!";
                _logger.LogError($"CreateStudentGroupOrdersAsync EXCEPTION : {ex.InnerException?.StackTrace} - {ex.Message} - {ex.StackTrace}");
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<StudentGroupTokenOrderSummaryDTO> GetStudentGroupOrderSummaryAsync(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, List<MealSessionDetail> mealSessionDetails)
        {
            var summary = new StudentGroupTokenOrderSummaryDTO { Total = new StudentGroupTokenOrderRowDTO { Cells = new List<string>() } };
            //var outlet = await _appContext.Outlets.FindAsync(outletId);
            var studentGroup = await _appContext.StudentGroups.FirstOrDefaultAsync(e => e.Id == studentGroupId);

            var students = studentGroup.Sgdetails.Where(e => e.IsActive).Select(e => e.Student).ToList();

            var orders = _appContext.TokenOrders.Where(t => t.StoreId == storeId && t.StudentGroupId == studentGroupId && t.Status != "cancelled" &&
                                           (t.DeliveryDate.Date >= deliveryDate.Date && t.DeliveryDate.Date <= deliveryDateTo.Date)
                                           && t.IsActive && t.IsStudentGroupOrder && students.Any(f => f.Id == t.ProfileId)).ToList();

            var dishTypes = _appContext.DishTypes.Where(e => e.IsActive == e.Caterer.CatererOutlets.Any(f => f.OutletId == outletId)).OrderBy(e => e.Name).ToList();
            var tokenOrderSelectedDishes = _appContext.TokenOrderDishes.Where(e => e.IsActive && orders.Any(f => f.Id == e.TokenOrdered.OrderId)).ToList();

            var mealSessionIds = mealSessionDetails.Select(e => e.MealSessionId).ToList();
            var mealSessions = _appContext.MealSessions.Where(e => mealSessionIds.Any(f => f == e.Id)).Select(e => e.Name).ToList();


            summary.Cols.Add("Session");
            var dishTypeIds = orders.SelectMany(e => e.Tokens.SelectMany(f => f.SelectedDishes).Select(x => x.Dish.DishTypeId)).Distinct();
            dishTypes = dishTypes.Where(e => dishTypeIds.Any(f => f == e.Id)).OrderBy(e => e.Name).ToList();
            foreach (var dishType in dishTypes)
            {
                summary.Cols.Add(dishType.Name);
            }


            foreach (var mealSession in mealSessions)
            {
                var row = new StudentGroupTokenOrderRowDTO();
                row.Cells.Add(string.Format("{0}", mealSession));

                foreach (var dishType in dishTypes)
                {
                    //var tokens = fasOrders.Where(e => e.MealSessionDetailId == mealSessionDetail.Id).SelectMany(e => e.Tokens);
                    var totalByDishType = tokenOrderSelectedDishes.Where(e => e.TokenOrdered.Order.Session.MealSession.Name == mealSession &&
                                                            e.Dish.DishTypeId == dishType.Id).Sum(f => (int)f.Qty);

                    row.Cells.Add(totalByDishType.ToString());
                }

                summary.Rows.Add(row);
            }

            summary.Total.Cells.Add("Total Quantity");
            if (summary.Rows.Any() && summary.Rows.First().Cells.Any())
            {
                for (int i = 1; i < summary.Rows.First().Cells.Count; i++)
                {
                    var totalByCol = summary.Rows.Select(e => int.Parse(e.Cells[i])).Sum();
                    summary.Total.Cells.Add(totalByCol.ToString());
                }
            }

            return summary;
        }
        #endregion

        //public async Task<BaseOperationResponse> ImportStudentAsync(IAccountManager accountManager, List<StudentImportDTO> rows)
        //{
        //    var result = new BaseOperationResponse();
        //    try
        //    {
        //        if (rows.Count > 0)
        //        {
        //            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
        //                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = new System.TimeSpan(24, 0, 0) },
        //                    TransactionScopeAsyncFlowOption.Enabled))
        //            {
        //                List<int> studentIds = new List<int>();

        //                foreach (var row in rows)
        //                {
        //                    var sClass = await _appContext.Classes.FirstOrDefaultAsync(e => e.IsActive && e.Name.Equals(row.Class, StringComparison.InvariantCultureIgnoreCase));
        //                    if (sClass == null)
        //                    {
        //                        throw new Exception(string.Format("Class not found. Please check the imported file."));
        //                    }

        //                    var batch = await _appContext.ClassBatches.FirstOrDefaultAsync(e => e.IsActive && e.Name.Equals(row.Batch, StringComparison.InvariantCultureIgnoreCase));

        //                    var student = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive &&
        //                                        e.Name.Equals(row.Name, StringComparison.InvariantCultureIgnoreCase));

        //                    ApplicationUser user = null;
        //                    //check if student exists using account
        //                    if (student == null)
        //                    {
        //                        if (!string.IsNullOrEmpty(row.Email) && new EmailAddressAttribute().IsValid(row.Email))
        //                        {
        //                            user = await accountManager.GetUserByEmailAsync(row.Email);
        //                            if (user != null && user.Account != null)
        //                            {
        //                                student = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive && e.Id == user.Account.StudentId);
        //                            }
        //                        }
        //                    }

        //                    if (student != null)
        //                    {
        //                        //update
        //                        student.Name = row.Name;
        //                        student.ClassBatchId = batch?.Id;
        //                        student.ClassId = sClass.Id;
        //                        student.Gender = row.Gender;
        //                        student.IsFAS = row.IsFAS;
        //                        student.Weight = row.Weight;
        //                        student.Height = row.Height;

        //                        //check if account exists
        //                        if (user == null && !string.IsNullOrEmpty(row.Email) && new EmailAddressAttribute().IsValid(row.Email))
        //                        {
        //                            //create account
        //                            user = await accountManager.GetUserByEmailAsync(row.Email);
        //                            if (user == null)
        //                            {
        //                                if (student.Account != null)
        //                                {
        //                                    if (student.Account.User != null)
        //                                    {
        //                                        student.Account.User.Email = row.Email;
        //                                        _appContext.StudentAccounts.Update(student.Account);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    user = new ApplicationUser();
        //                                    user.IsEnabled = true;
        //                                    user.EmailConfirmed = true;
        //                                    user.UserName = row.Email.Substring(0, row.Email.IndexOf('@'));
        //                                    user.Email = row.Email;
        //                                    string newPassword = PasswordHelper.GenerateRandomPassword();
        //                                    var createUserResult = await accountManager.CreateUserAsync(user, new List<string>(), newPassword);
        //                                    if (createUserResult.Item1)
        //                                    {
        //                                        if (student.Account == null)
        //                                        {
        //                                            student.Account = new StudentAccount();
        //                                        }

        //                                        student.Account.UserId = user.Id;
        //                                        student.Account.StudentId = student.Id;
        //                                        await _appContext.StudentAccounts.AddAsync(student.Account);
        //                                    }
        //                                    else
        //                                    {
        //                                        result.Message = "Failed to create an account!";
        //                                        return result;
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        await _appContext.SaveChangesAsync();
        //                        studentIds.Add(student.Id);
        //                    }
        //                    else
        //                    {
        //                        //no student record and no account yet
        //                        student = new Student
        //                        {
        //                            ClassBatchId = batch?.Id,
        //                            ClassId = sClass.Id,
        //                            Name = row.Name,
        //                            Gender = row.Gender,
        //                            IsFAS = row.IsFAS,
        //                            Weight = row.Weight,
        //                            Height = row.Height
        //                        };

        //                        if (user == null && !string.IsNullOrEmpty(row.Email) && new EmailAddressAttribute().IsValid(row.Email))
        //                        {
        //                            //create account
        //                            user = await accountManager.GetUserByEmailAsync(row.Email);
        //                            if (user == null)
        //                            {
        //                                user = new ApplicationUser();
        //                                user.IsEnabled = true;
        //                                user.EmailConfirmed = true;
        //                                user.UserName = row.Email.Substring(0, row.Email.IndexOf('@'));
        //                                user.Email = row.Email;
        //                                user.IsActive = true;
        //                                user.InstitutionId = (await accountManager.GetCurrentInstitution())?.Id;
        //                                string newPassword = PasswordHelper.GenerateRandomPassword();
        //                                var createUserResult = await accountManager.CreateUserAsync(user, new List<string>(), newPassword);
        //                                if (createUserResult.Item1)
        //                                {
        //                                    if (student.Account == null)
        //                                    {
        //                                        student.Account = new StudentAccount();
        //                                    }

        //                                    student.Account.User = user;
        //                                }
        //                                else
        //                                {
        //                                    result.Message = "Failed to create an account!";
        //                                    return result;
        //                                }
        //                            }
        //                        }

        //                        var stud = await AddAsync(student);
        //                        await _appContext.SaveChangesAsync();
        //                        studentIds.Add(stud.Id);
        //                    }
        //                }

        //                //disable removed students
        //                var studentsToDisable = _appContext.Students.Where(e => studentIds.All(f => f != e.Id));

        //                foreach (var stud in studentsToDisable)
        //                {
        //                    stud.IsActive = false;
        //                    Update(stud);
        //                    await _appContext.SaveChangesAsync();
        //                }

        //                scope.Complete();
        //                result.IsSuccess = true;
        //                result.Message = "File Imported!";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.Message = ex.Message;
        //    }



        //    return result;
        //}

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
