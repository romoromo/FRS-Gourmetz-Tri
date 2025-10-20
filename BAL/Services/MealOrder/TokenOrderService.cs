using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL;
using BAL.Services.Interfaces;
using BAL.DTO;
using AutoMapper;
using DAL.Repositories.Interfaces;
using System.IO;
using NPOI.HSSF.UserModel;
using BAL.Services.Interfaces.MealOrder;
using BAL.DTO.MealOrder;
using DAL.Models.MealOrder;
using DAL.Core.Interfaces;
using DAL.Core.DTO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.Data.SqlClient;
using System.Data;
using DAL.Models.StoredProcedures;
using System.Drawing;
using NPOI.SS.Util;
using NodaTime.Calendars;
using NPOI.HSSF.Util;
using static System.Net.WebRequestMethods;
using System.Text.RegularExpressions;

namespace BAL.Services.MealOrder
{
    public class TokenOrderService : ITokenOrderService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private IAccountManager _accountManager;
        private ApplicationDbContext _appContext;
        private IClassService _classService;
        private IDeliveryService _deliveryService;
        private IDishService _dishService;
        private readonly IMapper _mapper;

        public TokenOrderService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager, ApplicationDbContext context,
            IClassService classService, IMapper mapper, IDeliveryService deliveryService, IDishService dishService)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
            this._appContext = context;
            this._classService = classService;
            _mapper = mapper;
            this._deliveryService = deliveryService;
            this._dishService = dishService;
        }

        #region TokenOrder

        public async Task<BaseOperationResponse> BulkCancelCart()
        {
            return await this._uow.TokenOrders.BulkCancelCart();
        }

        public async Task<PagedEntity<TokenOrderDTO>> GetTokenOrdersAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<TokenOrderDTO>>(await this._uow.TokenOrders.GetTokenOrdersAsync(filter));
            return result;
        }

        public async Task<PagedEntity<TokenOrderOrderingPortalDTO>> GetTokenOrdersOrderingPortalAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<TokenOrderOrderingPortalDTO>>(await this._uow.TokenOrders.GetTokenOrdersAsync(filter));
            return result;
        }

        public async Task<PagedEntity<TokenOrderDTO>> GetTokenOrdersForCancellationAsync(OrderCancellationFilter filter)
        {
            var result = _mapper.Map<PagedEntity<TokenOrderDTO>>(await this._uow.TokenOrders.GetCancellationOrdersAsync(filter));
            return result;
        }

        public async Task<BaseOperationResponse> CancelOrders(List<int> orderIds, int cancelledById, string reason)
        {
            var result = _mapper.Map<BaseOperationResponse>(await this._uow.TokenOrders.CancelOrders(orderIds, cancelledById, reason));
            return result;
        }

        public async Task<PagedEntity<TokenOrderDTO>> GetStudentOrdersAsync(StudentOrderFilter filter)
        {
            var result = _mapper.Map<PagedEntity<TokenOrderDTO>>(await this._uow.TokenOrders.GetStudentOrdersAsync(filter));
            return result;
        }

        public async Task<BaseOperationResponse> AmendOrder(int id, string status, string invoiceNumber, string fomoId, int? updatedById, string reason, int dishId)
        {
            var result = _mapper.Map<BaseOperationResponse>(await this._uow.TokenOrders.AmendOrder(id, status, invoiceNumber, fomoId, updatedById, reason, dishId));
            return result;
        }

        public async Task<BaseOperationResponse> CreatePrepaidOrderAsync(PrepaidOrderDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.TokenOrders.CreatePrepaidOrderAsync(dto);

            if (result.IsSuccess)
            {
                var to = (TokenOrder)result.Data;
                var history = _mapper.Map<TokensOrderHistoryDTO>(to);
                await CreateTokensOrderHistoryAsync(history, to);
                result.Data = null;
            }

            return result;
        }

        public async Task<List<TokenOrderDTO>> GetUnupdatedTokenOrdersAsync()
        {
            var result = _mapper.Map<List<TokenOrderDTO>>(await this._uow.TokenOrders.GetUnupdatedTokenOrdersAsync());
            return result;
        }

        public async Task<List<MealPlanOrderDTO>> GetUnupdatedMealPlanOrdersAsync()
        {
            var result = _mapper.Map<List<MealPlanOrderDTO>>(await this._uow.TokenOrders.GetUnupdatedMealPlanOrdersAsync());
            return result;
        }

        public async Task<TokenOrderDTO> GetTokenOrderByIdAsync(int id)
        {
            return _mapper.Map<TokenOrderDTO>(await this._uow.TokenOrders.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateTokenOrderAsync(TokenOrderDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<TokenOrder>(dto);
            var tokens = _mapper.Map<List<TokenOrdered>>(dto.Tokens);
            result = await this._uow.TokenOrders.CreateAsync(order, tokens);

            if (result.IsSuccess)
            {
                var to = (TokenOrder)result.Data;
                var history = _mapper.Map<TokensOrderHistoryDTO>(to);
                await CreateTokensOrderHistoryAsync(history, to);
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateTokenOrderAsync(TokenOrderDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<TokenOrder>(dto);
            var tokens = _mapper.Map<List<TokenOrdered>>(dto.Tokens);
            result = await this._uow.TokenOrders.UpdateAsync(order, tokens);

            if (result.IsSuccess)
            {
                var to = (TokenOrder)result.Data;
                var history = _mapper.Map<TokensOrderHistoryDTO>(to);
                await CreateTokensOrderHistoryAsync(history, to);
            }

            return result;
        }

        public async Task<BaseOperationResponse> BulkCollectAsync(List<OrderCollectionDTO> orders)
        {
            return await this._uow.TokenOrders.BulkCollectAsync(orders);
        }

        public async Task<BaseOperationResponse> BulkReturnAsync(List<OrderReturnDTO> orders)
        {
            return await this._uow.TokenOrders.BulkReturnAsync(orders);
        }

        public async Task<BaseOperationResponse> DeleteTokenOrderAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.TokenOrders.DeleteAsync(id);

            if (result.IsSuccess)
            {
                var to = (TokenOrder)result.Data;
                var history = _mapper.Map<TokensOrderHistoryDTO>(to);
                if (history != null)
                {
                    history.Status = "cancelled";
                    await CreateTokensOrderHistoryAsync(history, to);
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> CreateFasTokenOrdersAsync(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool clear)
        {
            var result = await this._uow.TokenOrders.CreateFasTokenOrdersAsync(outletId, storeId, deliveryDate, deliveryDateTo, dishTypeId, mealSessionId, createdBy, clear);

            return result;
        }

        public async Task<FasTokenOrderSummaryDTO> GetFasTokenOrderSummaryAsync(int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, List<MealSessionDetailDTO> mealSessionDetailsDto)
        {
            var mealSessionDetails = _mapper.Map<List<MealSessionDetail>>(mealSessionDetailsDto);
            var result = await this._uow.TokenOrders.GetFasTokenOrderSummaryAsync(outletId, storeId, deliveryDate, deliveryDateTo, mealSessionDetails);

            return result;

        }
        #endregion

        #region Meal Plan
        public async Task<List<GroupedTermStudentGroupMealPlanDTO>> GetStudentMealPlanAsync(int studentId, DateTime? orderDate)
        {
            var mealPlans = _mapper.Map<List<StudentGroupMealPlanDTO>>(await this._uow.TokenOrders.GetStudentMealPlansAsync(studentId, orderDate));
            //var grpMealPlans = mealPlans.GroupBy(e => e.StudentGroupId).Select(e => new GroupedStudentGroupMealPlanDTO
            //{
            //    StudentGroupId = e.Key,
            //    StudentGroupName = e.First().StudentGroup.Name,
            //    OutletTermId = e.First().StudentGroup.OutletTermId.GetValueOrDefault(),
            //    TermName = e.First().StudentGroup.TermName,
            //    Sequence = e.First().StudentGroup.Sequence,
            //    Plans = e.OrderBy(x => x.StudentGroup.Sequence).ToList()
            //}).ToList();

            return mealPlans.GroupBy(e => e.StudentGroup.OutletTermId).Select(e => new GroupedTermStudentGroupMealPlanDTO
            {
                OutletTermId = e.Key.Value,
                TermName = e.First().StudentGroup.TermName,
                DeliveryStartDate = e.First().StudentGroup.DeliveryStartDate,
                DeliveryEndDate = e.First().StudentGroup.DeliveryEndDate,
                Plans = e.Select(x => new StudentGroupDTO
                {
                    Code = x.StudentGroup.Code,
                    DeliveryEndDate = x.StudentGroup.DeliveryEndDate,
                    DeliveryStartDate = x.StudentGroup.DeliveryStartDate,
                    Description = x.StudentGroup.Description,
                    EndDate = x.StudentGroup.EndDate,
                    FileName = x.StudentGroup.FileName,
                    FilePath = x.StudentGroup.FilePath,
                    Id = x.StudentGroup.Id,
                    IsPublished = x.StudentGroup.IsPublished,
                    //MealSessionId = x.StudentGroup.MealSessionId,
                    //MealSessionName = x.StudentGroup.MealSessionName,
                    Name = x.StudentGroup.Name,
                    OutletId = x.StudentGroup.OutletId,
                    OutletTermId = x.StudentGroup.OutletTermId,
                    Price = x.StudentGroup.Price,
                    Sequence = x.StudentGroup.Sequence,
                    StartDate = x.StudentGroup.StartDate,
                    TermName = x.StudentGroup.TermName,
                    Type = x.StudentGroup.Type
                }).Distinct(new StudentGroupIdEqualityComparer()).OrderBy(x => x.Sequence).ToList()
            }).ToList();
        }

        public async Task<BaseOperationResponse> CreateMealPlanAsync(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, bool skip = true)
        {
            var result = await this._uow.TokenOrders.CreateMealPlanAsync(studentGroupId, outletId, storeId, deliveryDate, deliveryDateTo, dishTypeId, mealSessionId, createdBy, skip);

            return result;
        }

        public async Task<MealPlanTokenOrderSummaryDTO> GetMealPlanOrderSummaryAsync(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, List<MealSessionDetailDTO> mealSessionDetailsDto)
        {
            var mealSessionDetails = _mapper.Map<List<MealSessionDetail>>(mealSessionDetailsDto);
            var result = await this._uow.TokenOrders.GetMealPlanOrderSummaryAsync(studentGroupId, outletId, storeId, deliveryDate, deliveryDateTo, mealSessionDetails);

            return result;

        }
        #endregion

        #region Student Group Order
        public async Task<BaseOperationResponse> CreateStudentGroupOrderAsync(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, int dishTypeId, int mealSessionId, int createdBy, string type, bool skip = true)
        {
            var result = await this._uow.TokenOrders.CreateStudentGroupOrderAsync(studentGroupId, outletId, storeId, deliveryDate, deliveryDateTo, dishTypeId, mealSessionId, createdBy, type, skip);

            return result;
        }

        public async Task<StudentGroupTokenOrderSummaryDTO> GetStudentGroupOrderSummaryAsync(int studentGroupId, int outletId, int storeId, DateTime deliveryDate, DateTime deliveryDateTo, List<MealSessionDetailDTO> mealSessionDetailsDto, string type)
        {
            var mealSessionDetails = _mapper.Map<List<MealSessionDetail>>(mealSessionDetailsDto);
            var result = await this._uow.TokenOrders.GetStudentGroupOrderSummaryAsync(studentGroupId, outletId, storeId, deliveryDate, deliveryDateTo, mealSessionDetails, type);

            return result;

        }

        #endregion  
        #region TokensOrderHistoryHistory

        public async Task<PagedEntity<TokensOrderHistoryDTO>> GetTokensOrderHistorysAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<TokensOrderHistoryDTO>>(await this._uow.TokensOrderHistorys.GetTokensOrderHistorysAsync(filter));
            return result;
        }

        public async Task<TokensOrderHistoryDTO> GetTokensOrderHistoryByIdAsync(int id)
        {
            return _mapper.Map<TokensOrderHistoryDTO>(await this._uow.TokensOrderHistorys.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateTokensOrderHistoryAsync(TokensOrderHistoryDTO dto, TokenOrder tokenOrder = null)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<TokensOrderHistory>(dto);

            if (order != null)
            {
                if (tokenOrder != null)
                {
                    order.TokenOrderId = tokenOrder.Id;
                    order.TransactionTime = DateTime.Now;
                    order.Qty = tokenOrder.Tokens?.FirstOrDefault()?.Qty;
                    order.DishId = tokenOrder.Tokens?.FirstOrDefault()?.SelectedDishes?.FirstOrDefault()?.DishId;
                }

                result = await this._uow.TokensOrderHistorys.CreateAsync(order);
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateTokensOrderHistoryAsync(TokensOrderHistoryDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<TokensOrderHistory>(dto);
            result = await this._uow.TokensOrderHistorys.UpdateAsync(order);
            return result;
        }

        public async Task<BaseOperationResponse> DeleteTokensOrderHistoryAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.TokensOrderHistorys.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Token Ordered

        public async Task<PagedEntity<TokenOrderedDTO>> GetTokenOrderedsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<TokenOrderedDTO>>(await this._uow.TokenOrdereds.GetTokenOrderedsAsync(filter));
            return result;
        }

        public async Task<TokenOrderedDTO> GetTokenOrderedByIdAsync(int id)
        {
            return _mapper.Map<TokenOrderedDTO>(await this._uow.TokenOrdereds.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateTokenOrderedAsync(TokenOrderedDTO dto)
        {
            var result = await this._uow.TokenOrdereds.CreateAsync(_mapper.Map<TokenOrdered>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateTokenOrderedAsync(TokenOrderedDTO dto)
        {
            var result = await this._uow.TokenOrdereds.UpdateAsync(_mapper.Map<TokenOrdered>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteTokenOrderedAsync(int id)
        {
            return await this._uow.TokenOrdereds.DeleteAsync(id);
        }

        #endregion

        #region TokenLabel

        public async Task<PagedEntity<TokenLabelDTO>> GetTokenLabelsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<TokenLabelDTO>>(await this._uow.TokenLabels.GetTokenLabelsAsync(filter));
            return result;
        }

        public async Task<TokenLabelDTO> GetTokenLabelByIdAsync(int id)
        {
            return _mapper.Map<TokenLabelDTO>(await this._uow.TokenLabels.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateTokenLabelAsync(TokenLabelDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<TokenLabel>(dto);
            var tokens = _mapper.Map<List<TokenDishLabel>>(dto.dishes);
            result = await this._uow.TokenLabels.CreateAsync(order, tokens);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateTokenLabelAsync(TokenLabelDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<TokenLabel>(dto);
            var tokens = _mapper.Map<List<TokenDishLabel>>(dto.dishes);
            result = await this._uow.TokenLabels.UpdateAsync(order, tokens);
            return result;
        }

        public async Task<BaseOperationResponse> DeleteTokenLabelAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.TokenLabels.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Meal Allocation

        public async Task<PagedEntity<MealAllocationDTO>> GetMealAllocationsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<MealAllocationDTO>>(await this._uow.MealAllocations.GetMealAllocationsAsync(filter));
            return result;
        }

        public async Task<MealAllocationDTO> GetMealAllocationByIdAsync(int id)
        {
            return _mapper.Map<MealAllocationDTO>(await this._uow.MealAllocations.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateMealAllocationAsync(MealAllocationDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<MealAllocation>(dto);
            var tokens = _mapper.Map<List<TokenLabel>>(dto.tokens);
            result = await this._uow.MealAllocations.CreateAsync(order, tokens);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateMealAllocationAsync(MealAllocationDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<MealAllocation>(dto);
            var tokens = _mapper.Map<List<TokenLabel>>(dto.tokens);
            result = await this._uow.MealAllocations.UpdateAsync(order, tokens);
            return result;
        }

        public async Task<BaseOperationResponse> DeleteMealAllocationAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.MealAllocations.DeleteAsync(id);
            return result;
        }

        public async Task<List<KioskOrderDishDTO>> GetKioskOrderDish(MealAllocationAdditionalDishFilter filter)
        {
            var result = _mapper.Map<List<KioskOrderDishDTO>>(await this._uow.MealAllocations.GetKioskOrderDish(filter).ToListAsync());
            return result;
        }

        #endregion

        #region Packing Allocation

        public async Task<PagedEntity<PackingAllocationDTO>> GetPackingAllocationsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<PackingAllocationDTO>>(await this._uow.PackingAllocations.GetPackingAllocationsAsync(filter));
            return result;
        }

        public async Task<PackingAllocationDTO> GetPackingAllocationByIdAsync(int id)
        {
            return _mapper.Map<PackingAllocationDTO>(await this._uow.PackingAllocations.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreatePackingAllocationAsync(PackingAllocationDTO dto)
        {
            var result = new BaseOperationResponse();
            var allocation = _mapper.Map<PackingAllocation>(dto);
            var dishes = _mapper.Map<List<DishAllocation>>(dto.Dishes);
            result = await this._uow.PackingAllocations.CreateAsync(allocation, dishes);
            return result;
        }

        public async Task<BaseOperationResponse> UpdatePackingAllocationAsync(PackingAllocationDTO dto)
        {
            var result = new BaseOperationResponse();
            var allocation = _mapper.Map<PackingAllocation>(dto);
            var dishes = _mapper.Map<List<DishAllocation>>(dto.Dishes);
            result = await this._uow.PackingAllocations.UpdateAsync(allocation, dishes);
            return result;
        }

        public async Task<BaseOperationResponse> DeletePackingAllocationAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.PackingAllocations.DeleteAsync(id);
            return result;
        }

        #endregion

        #region MealPlanOrder

        public async Task<PagedEntity<MealPlanOrderDTO>> GetMealPlanOrdersAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<MealPlanOrderDTO>>(await this._uow.MealPlanOrders.GetMealPlanOrdersAsync(filter));
            return result;
        }

        public async Task<MealPlanOrderDTO> GetMealPlanOrderByIdAsync(int id)
        {
            return _mapper.Map<MealPlanOrderDTO>(await this._uow.MealPlanOrders.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateMealPlanOrderAsync(MealPlanOrderDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<MealPlanOrder>(dto);
            result = await this._uow.MealPlanOrders.CreateAsync(order);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateMealPlanOrderAsync(MealPlanOrderDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<MealPlanOrder>(dto);
            result = await this._uow.MealPlanOrders.UpdateAsync(order);
            return result;
        }

        public async Task<BaseOperationResponse> DeleteMealPlanOrderAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.MealPlanOrders.DeleteAsync(id);
            return result;
        }

        #endregion


        #region updateSession

        public async Task<BaseOperationResponse> UpdateOrderSession()
        {
            var result = new BaseOperationResponse();
            IQueryable<TokenOrder> query = _appContext.TokenOrders;
            var filter = new Sieve.Models.SieveModel();
            filter.Filters = "(DeliveryDate)>=" + DateTime.Now.ToString().Split(' ')[0];
            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            var orders = _mapper.Map<List<TokenOrder>>(await query.ToListAsync());

            if (orders != null)
            {
                foreach (var o in orders)
                {
                    var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId);
                    if (currentOrderMealSession != null && currentOrderMealSession.Id != 0)
                    {
                        o.MealSessionDetailId = currentOrderMealSession.Id;
                    }

                    var tokens = _mapper.Map<List<TokenOrdered>>(o.Tokens);

                    result = await this._uow.TokenOrders.UpdateAsync(o, tokens);
                }
            }

            return result;
        }

        #endregion

        #region report

        public async Task<byte[]> GenerateOrderReport(BaseFilter filter)
        {
            IQueryable<TokenOrder> query = _appContext.TokenOrders.AsNoTracking();

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            var ordersMain = _mapper.Map<List<TokenOrder>>(await query.ToListAsync());
            var orders = ordersMain.Where(x => x.Student.IsActive).ToList();
            if (orders != null)
            {
                var mealSessionDetails = new List<MealSessionDetailByOrderAndClass>();
                var grpOrders = orders.GroupBy(o => new { o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId })
                    .Select(e => new MealSessionDetailByOrderAndClass
                    {
                        ClassId = e.Key.ClassId,
                        OutletId = e.Key.OutletId,
                        DeliveryDate = e.Key.DeliveryDate,
                        MealSessionId = e.Key.MealSessionId
                    });

                foreach (var o in grpOrders)
                {
                    var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.OutletId, o.DeliveryDate, o.MealSessionId, o.ClassId);
                    mealSessionDetails.Add(new MealSessionDetailByOrderAndClass
                    {
                        OutletId = o.OutletId,
                        ClassId = o.ClassId,
                        DeliveryDate = o.DeliveryDate,
                        MealSessionId = o.MealSessionId,
                        MealSessionDetail = currentOrderMealSession
                    });
                }
                using (var stream = new System.IO.MemoryStream())
                {

                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("OrderReport");
                    List<string> list = new List<string>();
                    list.Add("TRANSACTION DATE");
                    list.Add("TRANSACTION TIME");
                    list.Add("DELIVERY DATE");
                    list.Add("DELIVERY TIME");
                    list.Add("CARD ID");
                    list.Add("NAME");
                    list.Add("USER TYPE");
                    list.Add("EMAIL");
                    list.Add("DEPARTMENT/CLASS");
                    list.Add("MEAL PERIOD");
                    list.Add("MEAL SESSION SAVED");
                    list.Add("CURRENT MEAL SESSION");
                    list.Add("MEAL DESCRIPTION");
                    list.Add("MEAL TYPE");
                    list.Add("MEAL OPTION");
                    list.Add("PRICE");
                    list.Add("COLLECTED DATE");
                    list.Add("COLLECTED TIME");
                    list.Add("BENTO CODE");
                    list.Add("RETURNED DATE");
                    list.Add("RETURNED TIME");
                    list.Add("IS FAS");
                    String[] headers = list.ToArray();


                    #region detail info

                    var detailStyle = wb.CreateCellStyle();
                    var detailFont = wb.CreateFont();
                    detailFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    detailStyle.SetFont(detailFont);
                    detailStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

                    var row = sheet.CreateRow(++rowCount);
                    ICell cell;

                    for (int i = 0; i < orders.ToArray().Length; i++)
                    {
                        if (orders[i].Student != null)
                        {
                            cell = row.CreateCell(0);
                            cell.SetCellValue("SATS Food Services Pte Ltd (" + orders[0].Student.Outlet.Name + ")");
                            cell.CellStyle = detailStyle;
                            break;
                        }
                    }

                    row = sheet.CreateRow(++rowCount);

                    cell = row.CreateCell(0);
                    cell.SetCellValue("Generated at " + DateTime.Now.ToString());
                    cell.CellStyle = detailStyle;

                    row = sheet.CreateRow(++rowCount);

                    cell = row.CreateCell(0);
                    cell.SetCellValue("Start Date: " + filter.Filters.Substring(40, 10));
                    cell.CellStyle = detailStyle;

                    row = sheet.CreateRow(++rowCount);

                    cell = row.CreateCell(0);
                    cell.SetCellValue("End Date: " + filter.Filters.Substring(51, 10));
                    cell.CellStyle = detailStyle;

                    rowCount += 2;

                    #endregion

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    row = sheet.CreateRow(++rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    for (var i = 0; i < headers.Length; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region pdfheader
                    //Document document = new Document(PageSize.A4, 15, 15, 30, 30);
                    //PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    //document.Open();

                    //for (int i = 0; i < orders.ToArray().Length; i++)
                    //{
                    //    if (orders[i].Student != null)
                    //    {
                    //        Paragraph para5 = new Paragraph("SATS Food Services Pte Ltd (" + orders[0].Student.Outlet.Name + ")", new Font(Font.FontFamily.HELVETICA, 10));
                    //        para5.Alignment = Element.ALIGN_LEFT;
                    //        document.Add(para5);
                    //        break;
                    //    }
                    //}

                    //Paragraph para6 = new Paragraph("Generated at " + DateTime.Now.ToString(), new Font(Font.FontFamily.HELVETICA, 10));
                    //para6.Alignment = Element.ALIGN_RIGHT;
                    //document.Add(para6);

                    //Paragraph para1 = new Paragraph("MEAL ORDER DETAILS", new Font(Font.FontFamily.HELVETICA, 12));
                    //para1.Alignment = Element.ALIGN_CENTER;
                    //document.Add(para1);

                    //Paragraph para2 = new Paragraph("Meal Type: ", new Font(Font.FontFamily.HELVETICA, 11));
                    //para2.Alignment = Element.ALIGN_LEFT;
                    //document.Add(para2);

                    //Paragraph para3 = new Paragraph("Start Date: " + filter.Filters.Substring(40, 10), new Font(Font.FontFamily.HELVETICA, 11));
                    //para3.Alignment = Element.ALIGN_LEFT;
                    //document.Add(para3);

                    //Paragraph para4 = new Paragraph("End Date: " + filter.Filters.Substring(51, 10), new Font(Font.FontFamily.HELVETICA, 11));
                    //para4.Alignment = Element.ALIGN_LEFT;
                    //para4.SpacingAfter = 10;
                    //document.Add(para4);

                    //PdfPTable table = new PdfPTable(9);
                    //table.WidthPercentage = 100f;
                    //PdfPCell cell1 = new PdfPCell(new Phrase("DATE", new Font(Font.FontFamily.HELVETICA, 10)));
                    //cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                    //cell1.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //cell1.BorderWidth = 1f;
                    //cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell1.VerticalAlignment = Element.ALIGN_CENTER;
                    //table.AddCell(cell1);

                    //PdfPCell cell2 = new PdfPCell(new Phrase("NAME", new Font(Font.FontFamily.HELVETICA, 10)));
                    //cell2.BackgroundColor = BaseColor.LIGHT_GRAY;
                    //cell2.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //cell2.BorderWidth = 1f;
                    //cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell2.VerticalAlignment = Element.ALIGN_CENTER;
                    //table.AddCell(cell2);

                    //PdfPCell cell3 = new PdfPCell(new Phrase("USER TYPE", new Font(Font.FontFamily.HELVETICA, 10)));
                    //cell3.BackgroundColor = BaseColor.LIGHT_GRAY;
                    //cell3.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //cell3.BorderWidth = 1f;
                    //cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell3.VerticalAlignment = Element.ALIGN_CENTER;
                    //table.AddCell(cell3);

                    //PdfPCell cell4 = new PdfPCell(new Phrase("DEPARTMENT/CLASS", new Font(Font.FontFamily.HELVETICA, 10)));
                    //cell4.BackgroundColor = BaseColor.LIGHT_GRAY;
                    //cell4.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //cell4.BorderWidth = 1f;
                    //cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell4.VerticalAlignment = Element.ALIGN_CENTER;
                    //table.AddCell(cell4);

                    //PdfPCell cell8 = new PdfPCell(new Phrase("MEAL PERIOD", new Font(Font.FontFamily.HELVETICA, 10)));
                    //cell8.BackgroundColor = BaseColor.LIGHT_GRAY;
                    //cell8.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //cell8.BorderWidth = 1f;
                    //cell8.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell8.VerticalAlignment = Element.ALIGN_CENTER;
                    //table.AddCell(cell8);

                    //PdfPCell cell9 = new PdfPCell(new Phrase("MEAL SESSION", new Font(Font.FontFamily.HELVETICA, 10)));
                    //cell9.BackgroundColor = BaseColor.LIGHT_GRAY;
                    //cell9.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //cell9.BorderWidth = 1f;
                    //cell9.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell9.VerticalAlignment = Element.ALIGN_CENTER;
                    //table.AddCell(cell9);

                    //PdfPCell cell5 = new PdfPCell(new Phrase("MEAL DESCRIPTION", new Font(Font.FontFamily.HELVETICA, 10)));
                    //cell5.BackgroundColor = BaseColor.LIGHT_GRAY;
                    //cell5.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //cell5.BorderWidth = 1f;
                    //cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell5.VerticalAlignment = Element.ALIGN_CENTER;
                    //table.AddCell(cell5);

                    //PdfPCell cell6 = new PdfPCell(new Phrase("MEAL OPTION", new Font(Font.FontFamily.HELVETICA, 10)));
                    //cell6.BackgroundColor = BaseColor.LIGHT_GRAY;
                    //cell6.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //cell6.BorderWidth = 1f;
                    //cell6.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell6.VerticalAlignment = Element.ALIGN_CENTER;
                    //table.AddCell(cell6);

                    //PdfPCell cell7 = new PdfPCell(new Phrase("PRICE", new Font(Font.FontFamily.HELVETICA, 10)));
                    //cell7.BackgroundColor = BaseColor.LIGHT_GRAY;
                    //cell7.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //cell7.BorderWidth = 1f;
                    //cell7.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell7.VerticalAlignment = Element.ALIGN_CENTER;
                    //table.AddCell(cell7);
                    #endregion

                    #region pdfcontent
                    //orders.ForEach(o => {
                    //    if (o.Student != null)
                    //    {

                    //        foreach (TokenOrdered t in o.Tokens)
                    //        {
                    //            var selectedDishes = t.SelectedDishes.ToArray();
                    //            var selectedCDishes = t.SelectedCombinedDishes.ToArray();
                    //            int j = 0;
                    //            foreach (TokenOrderDish tod in selectedDishes)
                    //            {
                    //                for (j = 0; j < tod.Qty; j++)
                    //                {
                    //                    PdfPCell i_cell_1 = new PdfPCell(new Phrase(o.DeliveryDate.ToString().Split(' ')[0]));
                    //                    PdfPCell i_cell_2 = new PdfPCell(new Phrase(o.Student.Name));
                    //                    PdfPCell i_cell_3 = new PdfPCell(new Phrase("Student"));
                    //                    PdfPCell i_cell_4 = new PdfPCell(new Phrase(o.Student.Class.Name));
                    //                    PdfPCell i_cell_8 = new PdfPCell(new Phrase(o.Session.MealSessionName));
                    //                    PdfPCell i_cell_9 = new PdfPCell(new Phrase(o.Session.Name));
                    //                    PdfPCell i_cell_5 = new PdfPCell(new Phrase(t.Token.Name));
                    //                    //PdfPCell cell_6 = new PdfPCell(new Phrase(" "));
                    //                    PdfPCell i_cell_6 = new PdfPCell(new Phrase(tod.Dish.Label));
                    //                    PdfPCell i_cell_7;
                    //                    if (o.TotalAmount != 0)
                    //                    {
                    //                        i_cell_7 = new PdfPCell(new Phrase("$" + o.TotalAmount.ToString().Replace(',', '.')));
                    //                    }
                    //                    else
                    //                    {
                    //                        i_cell_7 = new PdfPCell(new Phrase("$" + t.Token.Price.ToString().Replace(',', '.')));
                    //                    }

                    //                    i_cell_1.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                    i_cell_2.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                    i_cell_3.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                    i_cell_4.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                    i_cell_8.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                    i_cell_9.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                    i_cell_5.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                    i_cell_6.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                    i_cell_7.HorizontalAlignment = Element.ALIGN_CENTER;

                    //                    table.AddCell(i_cell_1);
                    //                    table.AddCell(i_cell_2);
                    //                    table.AddCell(i_cell_3);
                    //                    table.AddCell(i_cell_4);
                    //                    table.AddCell(i_cell_8);
                    //                    table.AddCell(i_cell_9);
                    //                    table.AddCell(i_cell_5);
                    //                    table.AddCell(i_cell_6);
                    //                    table.AddCell(i_cell_7);
                    //                }
                    //            }
                    //            for (int k = j; k < t.Qty; k++)
                    //            {
                    //                PdfPCell cell_1 = new PdfPCell(new Phrase(o.DeliveryDate.ToString().Split(' ')[0]));
                    //                PdfPCell cell_2 = new PdfPCell(new Phrase(o.Student.Name));
                    //                PdfPCell cell_3 = new PdfPCell(new Phrase("Student"));
                    //                PdfPCell cell_4 = new PdfPCell(new Phrase(o.Student.Class.Name));
                    //                PdfPCell cell_8 = new PdfPCell(new Phrase(o.Session.MealSessionName));
                    //                PdfPCell cell_9 = new PdfPCell(new Phrase(o.Session.Name));
                    //                PdfPCell cell_5 = new PdfPCell(new Phrase(t.Token.Name));
                    //                PdfPCell cell_6 = new PdfPCell(new Phrase(" "));
                    //                //PdfPCell cell_6 = new PdfPCell(new Phrase(tod.Dish.Label));
                    //                PdfPCell cell_7 = new PdfPCell(new Phrase("$" + t.Token.Price.ToString().Replace(',', '.')));

                    //                cell_1.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                cell_2.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                cell_3.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                cell_4.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                cell_8.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                cell_9.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                cell_5.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                cell_6.HorizontalAlignment = Element.ALIGN_CENTER;
                    //                cell_7.HorizontalAlignment = Element.ALIGN_CENTER;

                    //                table.AddCell(cell_1);
                    //                table.AddCell(cell_2);
                    //                table.AddCell(cell_3);
                    //                table.AddCell(cell_4);
                    //                table.AddCell(cell_8);
                    //                table.AddCell(cell_9);
                    //                table.AddCell(cell_5);
                    //                table.AddCell(cell_6);
                    //                table.AddCell(cell_7);
                    //            }
                    //        }
                    //    }

                    //});

                    //document.Add(table);
                    //document.Close();
                    //writer.Close();
                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();

                    foreach (var o in orders)
                    {

                        if (o.Student != null)
                        {

                            foreach (TokenOrdered t in o.Tokens)
                            {
                                var selectedDishes = t.SelectedDishes.ToArray();
                                var selectedCDishes = t.SelectedCombinedDishes.ToArray();
                                int j = 0;
                                foreach (TokenOrderDish tod in selectedDishes)
                                {
                                    for (j = 0; j < tod.Qty; j++)
                                    {
                                        int c = 0;
                                        row = sheet.CreateRow(++rowCount);

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.TransactionTime.ToLocalTime().ToString("dd/MM/yy HH:mm:ss").Split(' ')[0]);
                                        cell.CellStyle = contentStyle;


                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.TransactionTime.ToLocalTime().ToString("dd/MM/yy HH:mm:ss").Split(' ')[1]);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.DeliveryDate.ToString("dd/MM/yy HH:mm:ss").Split(' ')[0]);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.Session.Route.Pickup.ToString("dd/MM/yy HH:mm:ss").Split(' ')[1]);
                                        cell.CellStyle = contentStyle;

                                        var activeCard = o.Student.StudentCards.FirstOrDefault(e => e.Status == "ACTIVE" && e.IsActive == true);

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(activeCard?.CardId);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.Student.Name);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue("Student");
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.Student.Email);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.Student.Class.Name);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.Session.MealSessionName);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.Session.Name);
                                        cell.CellStyle = contentStyle;

                                        var currentOrderMealSession = mealSessionDetails.FirstOrDefault(e =>
                                                   e.OutletId == o.Session?.MealSession?.OutletId &&
                                                   e.DeliveryDate == o.DeliveryDate &&
                                                   e.MealSessionId == o.Session.MealSessionId &&
                                                   e.ClassId == o.Student.ClassId)?.MealSessionDetail;
                                        //var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId);
                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(currentOrderMealSession != null ? currentOrderMealSession.Name : "No Current Session for this Class");
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(t.Token.Name);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(tod.Dish.DishType.Name);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(tod.Dish.Label);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        if (o.TotalAmount != 0)
                                        {
                                            cell.SetCellValue("$" + o.TotalAmount.ToString("n2").Replace(',', '.'));
                                        }
                                        else
                                        {
                                            cell.SetCellValue("$" + t.Token.Price.ToString("n2").Replace(',', '.'));
                                        }
                                        cell.CellStyle = contentStyle;

                                        if (o.CollectionTime != null)
                                        {
                                            cell = row.CreateCell(c++);
                                            cell.SetCellValue(o.CollectionTime?.ToString("dd/MM/yy hh:mm:ss").Split(' ')[0]);
                                            cell.CellStyle = contentStyle;

                                            cell = row.CreateCell(c++);
                                            cell.SetCellValue(o.CollectionTime?.ToString("dd/MM/yy hh:mm:ss").Split(' ')[1]);
                                            cell.CellStyle = contentStyle;
                                        }
                                        else
                                        {
                                            cell = row.CreateCell(c++);
                                            cell.SetCellValue(" ");
                                            cell.CellStyle = contentStyle;

                                            cell = row.CreateCell(c++);
                                            cell.SetCellValue(" ");
                                            cell.CellStyle = contentStyle;
                                        }

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.BentoCode);
                                        cell.CellStyle = contentStyle;

                                        if (o.ReturnTime != null)
                                        {
                                            cell = row.CreateCell(c++);
                                            cell.SetCellValue(o.ReturnTime?.ToString("dd/MM/yy hh:mm:ss").Split(' ')[0]);
                                            cell.CellStyle = contentStyle;

                                            cell = row.CreateCell(c++);
                                            cell.SetCellValue(o.ReturnTime?.ToString("dd/MM/yy hh:mm:ss").Split(' ')[1]);
                                            cell.CellStyle = contentStyle;
                                        }
                                        else
                                        {
                                            cell = row.CreateCell(c++);
                                            cell.SetCellValue(" ");
                                            cell.CellStyle = contentStyle;

                                            cell = row.CreateCell(c++);
                                            cell.SetCellValue(" ");
                                            cell.CellStyle = contentStyle;
                                        }

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.IsFAS ? "Y" : "N");
                                        cell.CellStyle = contentStyle;
                                    }
                                }
                                for (int k = j; k < t.Qty; k++)
                                {

                                    int c = 0;
                                    row = sheet.CreateRow(++rowCount);

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.TransactionTime.ToString("dd/MM/yy hh:mm:ss").Split(' ')[0]);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.TransactionTime.ToString("dd/MM/yy hh:mm:ss").Split(' ')[1]);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.DeliveryDate.ToString("dd/MM/yy hh:mm:ss").Split(' ')[0]);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.Session.Route.Pickup.ToString("dd/MM/yy hh:mm:ss").Split(' ')[1]);
                                    cell.CellStyle = contentStyle;

                                    var activeCard = o.Student.StudentCards.FirstOrDefault(e => e.IsActive == true);

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(activeCard.CardId);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.Student.Name);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue("Student");
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.Student.Email);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.Student.Class.Name);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.Session.MealSessionName);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.Session.Name);
                                    cell.CellStyle = contentStyle;

                                    var currentOrderMealSession = mealSessionDetails.FirstOrDefault(e =>
                                                   e.OutletId == o.Session?.MealSession?.OutletId &&
                                                   e.DeliveryDate == o.DeliveryDate &&
                                                   e.MealSessionId == o.Session.MealSessionId &&
                                                   e.ClassId == o.Student.ClassId)?.MealSessionDetail;
                                    //var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId);
                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(currentOrderMealSession != null ? currentOrderMealSession.Name : "No Current Session for this Class");
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(t.Token.Name);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(" ");
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(" ");
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue("$" + t.Token.Price.ToString("n2").Replace(',', '.'));
                                    cell.CellStyle = contentStyle;

                                    if (o.CollectionTime != null)
                                    {
                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.CollectionTime?.ToString("dd/MM/yy hh:mm:ss").Split(' ')[0]);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.CollectionTime?.ToString("dd/MM/yy hh:mm:ss").Split(' ')[1]);
                                        cell.CellStyle = contentStyle;
                                    }
                                    else
                                    {
                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(" ");
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(" ");
                                        cell.CellStyle = contentStyle;
                                    }

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.BentoCode);
                                    cell.CellStyle = contentStyle;

                                    if (o.ReturnTime != null)
                                    {
                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.ReturnTime?.ToString("dd/MM/yy hh:mm:ss").Split(' ')[0]);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(o.ReturnTime?.ToString("dd/MM/yy hh:mm:ss").Split(' ')[1]);
                                        cell.CellStyle = contentStyle;
                                    }
                                    else
                                    {
                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(" ");
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(" ");
                                        cell.CellStyle = contentStyle;
                                    }

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(o.IsFAS ? "Y" : "N");
                                    cell.CellStyle = contentStyle;
                                }
                            }
                        }

                    }

                    #endregion

                    for (var i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<byte[]> GenerateMealSummaryReport(BaseFilter filter)
        {
            IQueryable<TokenOrder> query = _appContext.TokenOrders.AsNoTracking();

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            var orders = _mapper.Map<List<TokenOrder>>(await query.ToListAsync());

            List<DOReportDTO> DOReports = new List<DOReportDTO>();
            List<DOReportSessionDTO> AllSessions = new List<DOReportSessionDTO>();

            if (orders != null)
            {
                var mealSessionDetails = new List<MealSessionDetailByOrderAndClass>();
                var grpOrders = orders.GroupBy(o => new { o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId })
                    .Select(e => new MealSessionDetailByOrderAndClass
                    {
                        ClassId = e.Key.ClassId,
                        OutletId = e.Key.OutletId,
                        DeliveryDate = e.Key.DeliveryDate,
                        MealSessionId = e.Key.MealSessionId
                    });

                foreach (var o in grpOrders)
                {
                    var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.OutletId, o.DeliveryDate, o.MealSessionId, o.ClassId);
                    mealSessionDetails.Add(new MealSessionDetailByOrderAndClass
                    {
                        OutletId = o.OutletId,
                        ClassId = o.ClassId,
                        DeliveryDate = o.DeliveryDate,
                        MealSessionId = o.MealSessionId,
                        MealSessionDetail = currentOrderMealSession
                    });
                }
                using (var stream = new System.IO.MemoryStream())
                {
                    foreach (var o in orders)
                    {
                        var currentOrderMealSession = mealSessionDetails.FirstOrDefault(e =>
                                                    e.OutletId == o.Session?.MealSession?.OutletId &&
                                                    e.DeliveryDate == o.DeliveryDate &&
                                                    e.MealSessionId == o.Session.MealSessionId &&
                                                    e.ClassId == o.Student.ClassId)?.MealSessionDetail;
                        //var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId);
                        if (currentOrderMealSession != null)
                        {
                            o.Session = _mapper.Map<MealSessionDetail>(currentOrderMealSession);
                        }
                        if (o.Student != null)
                        {
                            var alr = AllSessions.Find(r => r.mealSessionDetailId == o.Session.Id);
                            if (alr == null)
                            {
                                var sess = new DOReportSessionDTO();
                                sess.mealSessionDetailId = o.Session.Id;
                                sess.startTime = o.Session.StartDate.ToString("HHmmss");
                                sess.name = o.Session.Name;
                                AllSessions.Add(sess);
                            }
                            foreach (TokenOrdered t in o.Tokens)
                            {
                                var selectedDishes = t.SelectedDishes.ToArray();
                                int j = 0;
                                foreach (TokenOrderDish tod in selectedDishes)
                                {
                                    var rep = DOReports.Find(r => r.dishID == tod.DishId);
                                    if (rep == null)
                                    {
                                        var repDish = new DOReportDTO();
                                        repDish.dishID = tod.DishId.Value;
                                        repDish.dishLabel = tod.Dish.Label;
                                        repDish.tokenLabel = tod.Dish.DishType.Name;
                                        repDish.totalQty = tod.Qty ?? 0;
                                        repDish.sessions = new List<DOReportSessionDTO>();

                                        var session = new DOReportSessionDTO();
                                        session.mealSessionDetailId = o.Session.Id;
                                        session.name = o.Session.Name;

                                        session.qty = tod.Qty ?? 0;

                                        repDish.sessions.Add(session);

                                        DOReports.Add(repDish);
                                    }
                                    else
                                    {
                                        rep.totalQty += tod.Qty ?? 0;
                                        var session = rep.sessions.Find(r => r.mealSessionDetailId == o.Session.Id);
                                        if (session == null)
                                        {
                                            var sess = new DOReportSessionDTO();
                                            sess.mealSessionDetailId = o.Session.Id;
                                            sess.name = o.Session.Name;
                                            sess.qty = tod.Qty ?? 0;
                                            rep.sessions.Add(sess);
                                        }
                                        else
                                        {
                                            session.qty += tod.Qty ?? 0;

                                        }
                                    }
                                }
                            }
                        }

                    };

                    AllSessions = AllSessions.OrderBy(o => o.startTime).ToList();

                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Delivery");
                    List<string> list = new List<string>();
                    list.Add("ITEM");
                    list.Add("ITEM");
                    AllSessions.ForEach(ar =>
                    {
                        list.Add(ar.name);
                    });
                    list.Add("TOTAL");
                    String[] headers = list.ToArray();

                    #region detail info

                    var detailStyle = wb.CreateCellStyle();
                    var detailFont = wb.CreateFont();
                    detailFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    detailStyle.SetFont(detailFont);
                    detailStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

                    var row = sheet.CreateRow(++rowCount);
                    ICell cell;

                    cell = row.CreateCell(0);
                    cell.SetCellValue("OUTLET:");
                    cell.CellStyle = detailStyle;


                    for (int i = 0; i < orders.ToArray().Length; i++)
                    {
                        if (orders[i].Student != null)
                        {
                            cell = row.CreateCell(1);
                            cell.SetCellValue(orders[0].Student.Outlet.Name);
                            cell.CellStyle = detailStyle;
                            break;
                        }
                    }

                    row = sheet.CreateRow(++rowCount);

                    cell = row.CreateCell(0);
                    cell.SetCellValue("DAY:");
                    cell.CellStyle = detailStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.Filters.Substring(33, 10));
                    cell.CellStyle = detailStyle;

                    rowCount += 2;

                    #endregion

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    row = sheet.CreateRow(++rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    for (var i = 0; i < headers.Length; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();

                    DOReports.ForEach(dor =>
                    {
                        int c = 0;
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(c++);
                        cell.SetCellValue(dor.tokenLabel);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(c++);
                        cell.SetCellValue(dor.dishLabel);
                        cell.CellStyle = contentStyle;

                        AllSessions.ForEach(ar =>
                        {
                            cell = row.CreateCell(c++);
                            var qtyR = dor.sessions.Find(r => r.mealSessionDetailId == ar.mealSessionDetailId);
                            if (qtyR != null)
                            {
                                cell.SetCellValue(qtyR.qty);
                            }
                            else
                            {
                                cell.SetCellValue(0);
                            }
                            cell.CellStyle = contentStyle;
                        });

                        cell = row.CreateCell(c++);
                        cell.SetCellValue(dor.totalQty);
                        cell.CellStyle = contentStyle;
                    });

                    #endregion

                    for (var i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<byte[]> GenerateDeliveryOrderReport(BaseFilter filter)
        {
            IQueryable<TokenOrder> query = _appContext.TokenOrders.AsNoTracking();

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            var orders = _mapper.Map<List<TokenOrder>>(await query.ToListAsync());

            List<DOReportDTO> DOReports = new List<DOReportDTO>();
            List<DOReportRouteDTO> AllRoutes = new List<DOReportRouteDTO>();
            //List<DOReportSessionDTO> AllSessions = new List<DOReportSessionDTO>();

            if (orders != null)
            {
                var mealSessionDetails = new List<MealSessionDetailByOrderAndClass>();
                var grpOrders = orders.GroupBy(o => new { o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId })
                    .Select(e => new MealSessionDetailByOrderAndClass
                    {
                        ClassId = e.Key.ClassId,
                        OutletId = e.Key.OutletId,
                        DeliveryDate = e.Key.DeliveryDate,
                        MealSessionId = e.Key.MealSessionId
                    });

                foreach (var o in grpOrders)
                {
                    var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.OutletId, o.DeliveryDate, o.MealSessionId, o.ClassId);
                    mealSessionDetails.Add(new MealSessionDetailByOrderAndClass
                    {
                        OutletId = o.OutletId,
                        ClassId = o.ClassId,
                        DeliveryDate = o.DeliveryDate,
                        MealSessionId = o.MealSessionId,
                        MealSessionDetail = currentOrderMealSession
                    });
                }
                using (var stream = new System.IO.MemoryStream())
                {
                    foreach (var o in orders)
                    {
                        var currentOrderMealSession = mealSessionDetails.FirstOrDefault(e =>
                                                    e.OutletId == o.Session?.MealSession?.OutletId &&
                                                    e.DeliveryDate == o.DeliveryDate &&
                                                    e.MealSessionId == o.Session.MealSessionId &&
                                                    e.ClassId == o.Student.ClassId)?.MealSessionDetail;
                        //var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId);
                        if (currentOrderMealSession != null)
                        {
                            o.Session = _mapper.Map<MealSessionDetail>(currentOrderMealSession);
                        }
                        if (o.Student != null)
                        {
                            var alr = AllRoutes.Find(r => r.routeId == o.Session.RouteId.Value);
                            //var als = AllSessions.Find(r => r.mealSessionDetailId == o.Session.Id);
                            if (currentOrderMealSession != null)
                            {
                                alr = AllRoutes.Find(r => r.routeId == currentOrderMealSession.RouteId.Value);
                            }

                            //route
                            if (alr == null)
                            {
                                var rot = new DOReportRouteDTO();
                                if (currentOrderMealSession != null)
                                {
                                    rot.routeId = currentOrderMealSession.RouteId.Value;
                                    rot.routeLabel = currentOrderMealSession.RouteName;
                                    rot.startTime = currentOrderMealSession.RouteTime.Value.TimeOfDay;
                                    rot.isFas = false;
                                }
                                else
                                {
                                    rot.routeId = o.Session.RouteId.Value;
                                    rot.routeLabel = o.Session.Route.Label;
                                    rot.startTime = o.Session.Route.Pickup.TimeOfDay;
                                    rot.isFas = false;
                                }

                                rot.sessions = new List<DOReportSessionDTO>();

                                var sess = new DOReportSessionDTO();
                                sess.mealSessionDetailId = o.Session.Id;
                                sess.startTime = o.Session.StartDate.ToString("HHmmss");
                                sess.name = o.Session.Name;
                                sess.isFas = o.IsFAS;
                                if (o.IsFAS)
                                {
                                    rot.isFas = true;
                                }
                                rot.sessions.Add(sess);

                                AllRoutes.Add(rot);
                            }
                            else
                            {
                                var als = alr.sessions.Find(r => (r.mealSessionDetailId == o.Session.Id) && (r.isFas == o.IsFAS));
                                if (als == null)
                                {
                                    var sess = new DOReportSessionDTO();
                                    sess.mealSessionDetailId = o.Session.Id;
                                    sess.startTime = o.Session.StartDate.ToString("HHmmss");
                                    sess.name = o.Session.Name;
                                    sess.isFas = o.IsFAS;
                                    if (o.IsFAS)
                                    {
                                        alr.isFas = true;
                                    }
                                    alr.sessions.Add(sess);
                                }
                            }


                            foreach (TokenOrdered t in o.Tokens)
                            {
                                var selectedDishes = t.SelectedDishes.ToArray();
                                int j = 0;
                                foreach (TokenOrderDish tod in selectedDishes)
                                {
                                    var rep = DOReports.Find(r => r.dishID == tod.DishId);
                                    if (rep == null)
                                    {
                                        var repDish = new DOReportDTO();
                                        repDish.dishID = tod.DishId.Value;
                                        repDish.dishLabel = tod.Dish.Label;
                                        repDish.tokenLabel = tod.Dish.DishType.Name;
                                        repDish.totalQty = tod.Qty ?? 0;
                                        repDish.OrderNumber = tod?.Dish?.DishType?.OrderNumber;
                                        repDish.routes = new List<DOReportRouteDTO>();
                                        repDish.sessions = new List<DOReportSessionDTO>();

                                        //route
                                        var routeIn = new DOReportRouteDTO();
                                        routeIn.isFas = o.IsFAS;
                                        if (currentOrderMealSession != null)
                                        {
                                            routeIn.routeId = currentOrderMealSession.RouteId.Value;
                                            routeIn.routeLabel = currentOrderMealSession.RouteName;
                                        }
                                        else
                                        {
                                            routeIn.routeId = o.Session.RouteId.Value;
                                            routeIn.routeLabel = o.Session.Route.Label;
                                        }

                                        routeIn.qty = tod.Qty ?? 0;

                                        repDish.routes.Add(routeIn);

                                        //session
                                        var session = new DOReportSessionDTO();
                                        session.mealSessionDetailId = o.Session.Id;
                                        session.name = o.Session.Name;
                                        session.isFas = o.IsFAS;

                                        session.qty = tod.Qty ?? 0;

                                        repDish.sessions.Add(session);


                                        DOReports.Add(repDish);
                                    }
                                    else
                                    {

                                        //route
                                        rep.totalQty += tod.Qty ?? 0;
                                        var route = rep.routes.Find(r => (r.routeId == o.Session.RouteId) && (r.isFas == o.IsFAS));
                                        if (currentOrderMealSession != null)
                                        {
                                            route = rep.routes.Find(r => (r.routeId == currentOrderMealSession.RouteId) && (r.isFas == o.IsFAS));
                                        }
                                        if (route == null)
                                        {
                                            var routeIn = new DOReportRouteDTO();
                                            routeIn.isFas = o.IsFAS;
                                            if (currentOrderMealSession != null)
                                            {
                                                routeIn.routeId = currentOrderMealSession.RouteId.Value;
                                                routeIn.routeLabel = currentOrderMealSession.RouteName;
                                            }
                                            else
                                            {
                                                routeIn.routeId = o.Session.RouteId.Value;
                                                routeIn.routeLabel = o.Session.Route.Label;
                                            }
                                            routeIn.qty = tod.Qty ?? 0;
                                            rep.routes.Add(routeIn);
                                        }
                                        else
                                        {
                                            route.qty += tod.Qty ?? 0;

                                        }

                                        //session
                                        var session = rep.sessions.Find(r => (r.mealSessionDetailId == o.Session.Id) && (r.isFas == o.IsFAS));

                                        if (session == null)
                                        {
                                            var sess = new DOReportSessionDTO();
                                            sess.mealSessionDetailId = o.Session.Id;
                                            sess.name = o.Session.Name;
                                            sess.isFas = o.IsFAS;
                                            sess.qty = tod.Qty ?? 0;
                                            rep.sessions.Add(sess);
                                        }
                                        else
                                        {
                                            session.qty += tod.Qty ?? 0;

                                        }
                                    }
                                }
                            }
                        }

                    };

                    //AllSessions = AllSessions.OrderBy(o => o.startTime).ToList();
                    AllRoutes.Sort((x, y) => TimeSpan.Compare(x.startTime, y.startTime));
                    foreach (var item in AllRoutes)
                    {
                        item.sessions = item.sessions.OrderBy(x => x.name).ThenBy(x => x.isFas).ToList();
                    }

                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Delivery");

                    #region detail info

                    var detailStyle = wb.CreateCellStyle();
                    var detailFont = wb.CreateFont();
                    detailFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    detailStyle.SetFont(detailFont);
                    detailStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

                    var row = sheet.CreateRow(++rowCount);
                    ICell cell;

                    cell = row.CreateCell(0);
                    cell.SetCellValue("OUTLET:");
                    cell.CellStyle = detailStyle;


                    for (int idx = 0; idx < orders.ToArray().Length; idx++)
                    {
                        if (orders[idx].Student != null)
                        {
                            cell = row.CreateCell(1);
                            cell.SetCellValue(orders[0].Student.Outlet.Name);
                            cell.CellStyle = detailStyle;
                            break;
                        }
                    }

                    row = sheet.CreateRow(++rowCount);

                    cell = row.CreateCell(0);
                    cell.SetCellValue("DAY:");
                    cell.CellStyle = detailStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.Filters.Substring(33, 10));
                    cell.CellStyle = detailStyle;

                    rowCount += 2;

                    #endregion

                    #region Headers
                    var defaultColor = HSSFColor.Aqua.Index;
                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    row = sheet.CreateRow(++rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    borderedHeaderStyle.FillBackgroundColor = defaultColor;
                    borderedHeaderStyle.FillForegroundColor = defaultColor;
                    borderedHeaderStyle.FillPattern = FillPattern.SolidForeground;

                    var borderedHeaderStyleWrapText = wb.CreateCellStyle();
                    borderedHeaderStyleWrapText.SetFont(headerFont);
                    borderedHeaderStyleWrapText.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyleWrapText.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyleWrapText.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyleWrapText.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyleWrapText.BorderRight = BorderStyle.Thin;
                    borderedHeaderStyleWrapText.FillBackgroundColor = defaultColor;
                    borderedHeaderStyleWrapText.FillForegroundColor = defaultColor;
                    borderedHeaderStyleWrapText.FillPattern = FillPattern.SolidForeground;
                    borderedHeaderStyleWrapText.WrapText = true;

                    cell = row.CreateCell(0);
                    cell.SetCellValue("DISH CATEGORY");
                    cell.CellStyle = borderedHeaderStyle;
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowCount, rowCount + 1, 0, 0));

                    cell = row.CreateCell(1);
                    cell.SetCellValue("Pick-Up Time");
                    cell.CellStyle = borderedHeaderStyle;

                    var i = 2;

                    AllRoutes.ForEach(ar =>
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(ar.routeLabel);
                        int lengthColumn = i + ar.sessions.Count;
                        lengthColumn = lengthColumn + 1;
                        if (ar.isFas)
                            lengthColumn = lengthColumn + 1;
                        var cra = new CellRangeAddress(rowCount, rowCount, i, lengthColumn);
                        cell.CellStyle = borderedHeaderStyle;
                        GenerateBorderMergeCell(wb, sheet, cra);
                        int isFasNumber = (ar.isFas ? 1 : 0);
                        i += (ar.sessions.Count) + 2 + isFasNumber;
                    });

                    var grandTotalStyle = wb.CreateCellStyle();
                    var craGrandTotal = new CellRangeAddress(rowCount, rowCount + 1, i, i);
                    cell = row.CreateCell(i);
                    cell.SetCellValue("Grand Total");
                    cell.CellStyle = borderedHeaderStyle;
                    GenerateBorderMergeCell(wb, sheet, craGrandTotal);

                    row = sheet.CreateRow(++rowCount);

                    cell = row.CreateCell(1);
                    cell.SetCellValue("DISH NAME");
                    cell.CellStyle = borderedHeaderStyle;

                    i = 2;
                    List<int> notAllowedAutoSize = new List<int>();
                    AllRoutes.ForEach(ar =>
                    {
                        ar.sessions = ar.sessions.OrderBy(o => o.startTime).ToList();
                        ar.sessions.ForEach(ars =>
                        {
                            cell = row.CreateCell(i);
                            cell.SetCellValue(ars.name);
                            if (ars.isFas)
                            {
                                notAllowedAutoSize.Add(i);
                                sheet.SetColumnWidth(i, 25 * 256);
                                cell.SetCellValue($"{ars.name}\n(FAS)");
                            }
                            cell.CellStyle = ars.isFas ? borderedHeaderStyleWrapText : borderedHeaderStyle;

                            i += 1;

                            //cell = row.CreateCell(i);
                            //cell.SetCellValue(ars.name + " FAS");
                            //cell.CellStyle = borderedHeaderStyle;
                            //i += 1;
                        });

                        cell = row.CreateCell(i);
                        cell.SetCellValue("Total Meal (Regular)");
                        cell.CellStyle = borderedHeaderStyle;
                        i += 1;

                        if (ar.isFas)
                        {
                            cell = row.CreateCell(i);
                            cell.SetCellValue("Total Meal (FAS)");
                            cell.CellStyle = borderedHeaderStyle;
                            i += 1;
                        }

                        cell = row.CreateCell(i);
                        cell.SetCellValue("Total Meal (Regular & FAS)");
                        cell.CellStyle = borderedHeaderStyle;
                        row.Height = -1;
                        i += 1;

                    });



                    //sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();

                    var contentBackgroundStyle = wb.CreateCellStyle();
                    contentBackgroundStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentBackgroundStyle.Alignment = HorizontalAlignment.Left;
                    contentBackgroundStyle.WrapText = true;
                    contentBackgroundStyle.BorderTop = BorderStyle.Thin;
                    contentBackgroundStyle.BorderBottom = BorderStyle.Thin;
                    contentBackgroundStyle.BorderLeft = BorderStyle.Thin;
                    contentBackgroundStyle.BorderRight = BorderStyle.Thin;
                    contentBackgroundStyle.FillBackgroundColor = defaultColor;
                    contentBackgroundStyle.FillForegroundColor = defaultColor;
                    contentBackgroundStyle.FillPattern = FillPattern.SolidForeground;

                    DOReports.OrderBy(x => x.OrderNumber).ThenBy(x => x.dishLabel).ToList().ForEach(dor =>
                    {
                        int c = 0;
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(c++);
                        cell.SetCellValue(dor.tokenLabel);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(c++);
                        cell.SetCellValue(dor.dishLabel);
                        cell.CellStyle = contentStyle;

                        //route
                        AllRoutes.ForEach(ar =>
                        {

                            ar.sessions.ForEach(ars =>
                            {
                                //non fas qty
                                cell = row.CreateCell(c++);
                                var qtyS = dor.sessions.Find(r => (r.mealSessionDetailId == ars.mealSessionDetailId) && (r.isFas == ars.isFas));
                                if (qtyS != null)
                                {
                                    cell.SetCellValue(qtyS.qty);
                                }
                                else
                                {
                                    cell.SetCellValue(0);
                                }
                                cell.CellStyle = contentStyle;

                                //fas qty
                                //cell = row.CreateCell(c++);
                                //var qtySF = dor.sessions.Find(r => (r.mealSessionDetailId == ars.mealSessionDetailId) && (r.isFas == true));
                                //if (qtySF != null)
                                //{
                                //    cell.SetCellValue(qtySF.qty);
                                //}
                                //else
                                //{
                                //    cell.SetCellValue(0);
                                //}
                                //cell.CellStyle = contentStyle;


                            });

                            // non fas qty
                            cell = row.CreateCell(c++);
                            var qtyR = dor.routes.Find(r => (r.routeId == ar.routeId) && (r.isFas == false));
                            if (qtyR != null)
                            {
                                cell.SetCellValue(qtyR.qty);
                            }
                            else
                            {
                                cell.SetCellValue(0);
                            }
                            cell.CellStyle = contentStyle;

                            //fas qty
                            int totalQtyFas = 0;
                            if (ar.isFas)
                            {
                                cell = row.CreateCell(c++);
                                var qtyF = dor.routes.Find(r => (r.routeId == ar.routeId) && (r.isFas == true));
                                if (qtyF != null)
                                {
                                    cell.SetCellValue(qtyF.qty);
                                    totalQtyFas = qtyF?.qty ?? 0;
                                }
                                else
                                {
                                    cell.SetCellValue(0);
                                }
                                cell.CellStyle = contentStyle;
                            }

                            cell = row.CreateCell(c++);
                            cell.SetCellValue((qtyR?.qty ?? 0) + totalQtyFas);
                            cell.CellStyle = contentBackgroundStyle;
                        });

                        cell = row.CreateCell(c++);
                        cell.SetCellValue(dor.totalQty);
                        cell.CellStyle = contentStyle;
                    });

                    #endregion

                    for (var idx = 0; idx < 50; idx++)
                    {
                        if (!notAllowedAutoSize.Contains(idx))
                        {
                            sheet.AutoSizeColumn(idx, true);
                        }
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        private void GenerateBorderMergeCell(XSSFWorkbook wb, XSSFSheet sheet, CellRangeAddress cra)
        {
            sheet.AddMergedRegion(cra);
            RegionUtil.SetBorderTop(1, cra, sheet, wb);
            RegionUtil.SetBorderBottom(1, cra, sheet, wb);
            RegionUtil.SetBorderRight(1, cra, sheet, wb);
            RegionUtil.SetBorderLeft(1, cra, sheet, wb);
        }

        public async Task<List<SalesDataDTO>> RetrieveSalesData(BaseFilter filter)
        {
            IQueryable<TokenOrder> query = _appContext.TokenOrders;

            List<SalesDataDTO> dtos = new List<SalesDataDTO>();

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            var orders = _mapper.Map<List<TokenOrder>>(await query.ToListAsync());

            if (orders != null)
            {
                var mealSessionDetails = new List<MealSessionDetailByOrderAndClass>();
                var grpOrders = orders.GroupBy(o => new { o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId })
                    .Select(e => new MealSessionDetailByOrderAndClass
                    {
                        ClassId = e.Key.ClassId,
                        OutletId = e.Key.OutletId,
                        DeliveryDate = e.Key.DeliveryDate,
                        MealSessionId = e.Key.MealSessionId
                    });

                foreach (var o in grpOrders)
                {
                    var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.OutletId, o.DeliveryDate, o.MealSessionId, o.ClassId);
                    mealSessionDetails.Add(new MealSessionDetailByOrderAndClass
                    {
                        OutletId = o.OutletId,
                        ClassId = o.ClassId,
                        DeliveryDate = o.DeliveryDate,
                        MealSessionId = o.MealSessionId,
                        MealSessionDetail = currentOrderMealSession
                    });
                }

                foreach (var o in orders)
                {
                    var currentOrderMealSession = mealSessionDetails.FirstOrDefault(e =>
                                                    e.OutletId == o.Session?.MealSession?.OutletId &&
                                                    e.DeliveryDate == o.DeliveryDate &&
                                                    e.MealSessionId == o.Session.MealSessionId &&
                                                    e.ClassId == o.Student.ClassId)?.MealSessionDetail;
                    var ActiveCard = "";
                    if (o.Student != null && o.Student.StudentCards != null)
                    {
                        foreach (StudentCard sc in o.Student.StudentCards)
                        {
                            if (sc.Status == "ACTIVE" && sc.IsActive)
                            {
                                ActiveCard = sc.CardId;
                            }
                        }
                    }

                    if (o.Tokens != null && o.Status == "paid")
                    {
                        foreach (TokenOrdered t in o.Tokens)
                        {
                            if (t.SelectedDishes != null)
                            {
                                foreach (TokenOrderDish tod in t.SelectedDishes)
                                {
                                    for (int j = 0; j < tod.Qty; j++)
                                    {
                                        SalesDataDTO dto = new SalesDataDTO();
                                        dto.cardId = ActiveCard;
                                        dto.DishId = tod.DishId;
                                        dto.DishCode = tod.Dish.Code;
                                        dto.DishName = tod.Dish.Label;
                                        dto.DishType = tod.Dish.DishType.Name;
                                        dto.DishTypeID = tod.Dish.DishTypeId;
                                        dto.DishTypeOrder = tod.Dish.DishType.OrderNumber;
                                        dto.orderId = o.Id;
                                        dto.studentId = o.Student.Id;
                                        dto.studentName = o.Student.Name;
                                        dto.studentClass = o.Student.Class.Name;
                                        if (currentOrderMealSession != null)
                                        {
                                            dto.collectionStart = currentOrderMealSession.StartDate;
                                            if (currentOrderMealSession.StartDate != null)
                                            {
                                                DateTime endTime = currentOrderMealSession.StartDate;
                                                endTime = endTime.AddMinutes((currentOrderMealSession.OverheadInterval + currentOrderMealSession.RouteInterval) * -1);
                                                endTime = endTime.AddHours(4);
                                                dto.collectionEnd = endTime;
                                            }
                                            dto.sessionId = currentOrderMealSession.Id;
                                            dto.sessionLabel = currentOrderMealSession.Name;
                                        }
                                        else
                                        {
                                            dto.collectionStart = o.Session.StartDate;
                                            if (o.Session.StartDate != null)
                                            {
                                                DateTime endTime = o.Session.StartDate;
                                                endTime = endTime.AddMinutes((o.Session.OverheadInterval + o.Session.RouteInterval) * -1);
                                                endTime = endTime.AddHours(4);
                                                dto.collectionEnd = endTime;
                                            }
                                            dto.sessionId = o.MealSessionDetailId;
                                            dto.sessionLabel = o.Session.Name;
                                        }

                                        dto.orderDate = o.DeliveryDate;
                                        dto.mealTypeId = t.TokenId;

                                        dtos.Add(dto);
                                    }
                                }
                            }
                        }
                    }

                }

                return dtos;
            }
            else
            {
                return dtos;
            }
        }

        public async Task<List<TokenOrder>> GetTokenOrderWithCurrentSession(BaseFilter filter, int sessionDetailId)
        {
            IQueryable<TokenOrder> query = _appContext.TokenOrders;

            List<TokenOrder> dtos = new List<TokenOrder>();

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            var orders = _mapper.Map<List<TokenOrder>>(await query.ToListAsync());

            if (orders != null)
            {

                var mealSessionDetails = new List<MealSessionDetailByOrderAndClass>();
                var grpOrders = orders.GroupBy(o => new { o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId })
                    .Select(e => new MealSessionDetailByOrderAndClass
                    {
                        ClassId = e.Key.ClassId,
                        OutletId = e.Key.OutletId,
                        DeliveryDate = e.Key.DeliveryDate,
                        MealSessionId = e.Key.MealSessionId
                    });

                foreach (var o in grpOrders)
                {
                    var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.OutletId, o.DeliveryDate, o.MealSessionId, o.ClassId);
                    mealSessionDetails.Add(new MealSessionDetailByOrderAndClass
                    {
                        OutletId = o.OutletId,
                        ClassId = o.ClassId,
                        DeliveryDate = o.DeliveryDate,
                        MealSessionId = o.MealSessionId,
                        MealSessionDetail = currentOrderMealSession
                    });
                }

                foreach (var o in orders)
                {
                    //var currentOrderMealSession = await this._classService.GetCurrentOrderMealSessionAsync(o.Session?.MealSession?.OutletId, o.DeliveryDate, o.Session.MealSessionId, o.Student.ClassId);
                    var currentOrderMealSession = mealSessionDetails.FirstOrDefault(e =>
                                                    e.OutletId == o.Session?.MealSession?.OutletId &&
                                                    e.DeliveryDate == o.DeliveryDate &&
                                                    e.MealSessionId == o.Session.MealSessionId &&
                                                    e.ClassId == o.Student.ClassId)?.MealSessionDetail;
                    if (currentOrderMealSession != null)
                    {
                        if (o.Tokens != null && o.Status == "paid" && currentOrderMealSession.Id == sessionDetailId)
                        {

                            dtos.Add(o);
                        }
                    }
                    else
                    {
                        if (o.Tokens != null && o.Status == "paid" && o.MealSessionDetailId == sessionDetailId)
                        {

                            dtos.Add(o);
                        }
                    }

                }

                return dtos;
            }
            else
            {
                return dtos;
            }
        }

        public async Task<byte[]> GenerateOrderLabel(TokenLabelDTO[] dto)
        {

            if (dto != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {

                    var folderName = Path.Combine("Resources", "Font");
                    var imageFolderName = Path.Combine("Resources", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var pathToImageSave = Path.Combine(Directory.GetCurrentDirectory(), imageFolderName);

                    var fullPath = Path.Combine(pathToSave, "Aller_Bd.ttf");
                    var fullImagePath = Path.Combine(pathToImageSave, "Halal.png");

                    BaseFont allerfont = BaseFont.CreateFont(fullPath, BaseFont.WINANSI, BaseFont.EMBEDDED);
                    iTextSharp.text.Font aller = new iTextSharp.text.Font(allerfont, 12);

                    var pgSize = new iTextSharp.text.Rectangle(88, 66);
                    Document document = new Document(pgSize, 2, 2, 2, 2);
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    document.Open();


                    for (int i = 0; i < dto.Length; i++)
                    {

                        for (int j = 0; j < dto[i].dishes.ToArray().Length; j++)
                        {
                            for (int k = 0; k < dto[i].dishes[j].t_qty; k++)
                            {

                                DishDTO dish = await this._dishService.GetDishByIdAsync(dto[i].dishes[j].dish_id);

                                string cuisineLicense = dish.CuisineLicenseCode;

                                string toIconFilePath = "";

                                if (dish.CuisineIconFileName != null && dish.CuisineIconFileName != "")
                                {
                                    toIconFilePath =  Path.Combine(pathToImageSave, dish.CuisineIconFileName);
                                }


                                document.NewPage();

                                float margin = document.LeftMargin;
                                float totalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                                float columnWidth = 50;

                                iTextSharp.text.Rectangle leftColumn = new iTextSharp.text.Rectangle(
                                    document.Left,        // x1
                                    document.Bottom,      // y1
                                    document.Left + columnWidth, // x2
                                    document.Top          // y2
                                );

                                // Create a ColumnText for the left column
                                ColumnText columnLeft = new ColumnText(writer.DirectContent);
                                columnLeft.SetSimpleColumn(leftColumn);
                                

                                if(toIconFilePath != "")
                                {
                                    iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(toIconFilePath);
                                    png.ScaleToFit(13, 13);
                                    png.SetAbsolutePosition(63f, 5f);
                                    document.Add(png);
                                }

                                string cleanDate = Regex.Replace(dto[i].deliveryDate, "[^a-zA-Z0-9]", "");

                                string uniqueCode = "B" + cleanDate + dto[i].meal_allocation_id.ToString().PadLeft(5, '0') + dto[i].dishes[j].dish_id.ToString().PadLeft(4, '0') + dto[i].dishes[j].token_id.ToString().PadLeft(3, '0') + k.ToString().PadLeft(3, '0');

                                string qrCodeData = uniqueCode + " Dish : " + dto[i].dishes[j].dish_name + "', Packed: " + dto[i].deliveryDate + " " + dto[i].timePacked.Value.ToString("hh:mm tt") + ", Consume By: " + dto[i].deliveryDate + " " + dto[i].timePacked.Value.AddHours(4).ToString("hh: mm tt") + "\n";
                                BarcodeQRCode barcodeQRCode = new BarcodeQRCode(qrCodeData, 15, 15, null); // width, height, parameters

                                iTextSharp.text.Image qrCodeImage = barcodeQRCode.GetImage();
                                qrCodeImage.ScaleToFit(36, 36);
                                qrCodeImage.SetAbsolutePosition(51f, 20f);
                                document.Add(qrCodeImage);

                                BentoAssetDTO bentoAsset = new BentoAssetDTO();
                                bentoAsset.Code = uniqueCode;
                                bentoAsset.BentoBoxTypeId = 1;
                                bentoAsset.DishId = dto[i].dishes[j].dish_id;
                                bentoAsset.InstitutionId = 1;

                                await this._deliveryService.CreateBentoAssetAsync(bentoAsset);

                                Paragraph para1 = new Paragraph("Gourmetz Pte Ltd", new iTextSharp.text.Font(allerfont, 5));
                                para1.Alignment = Element.ALIGN_CENTER;
                                columnLeft.AddElement(para1);


                                string licenseNo = "PL24L0327";

                                if (cuisineLicense != null && cuisineLicense != "")
                                {
                                    licenseNo = cuisineLicense;
                                }

                                Paragraph para2 = new Paragraph("License No: " + licenseNo, new iTextSharp.text.Font(allerfont, 4));
                                para2.Alignment = Element.ALIGN_CENTER;
                                columnLeft.AddElement(para2);

                                Paragraph para3 = new Paragraph("Date Packed: " + dto[i].deliveryDate, new iTextSharp.text.Font(allerfont, 4));
                                para3.Alignment = Element.ALIGN_CENTER;
                                columnLeft.AddElement(para3);

                                Chunk c = new Chunk("Time Packed: " + dto[i].timePacked.Value.ToString("hh:mm tt"), new iTextSharp.text.Font(allerfont, 4));
                                if (dto[i].color != null && dto[i].color != "")
                                {
                                    c.SetBackground(new BaseColor(ColorTranslator.FromHtml(dto[i].color)));
                                }
                                Paragraph para4 = new Paragraph(c);
                                para4.Alignment = Element.ALIGN_CENTER;
                                columnLeft.AddElement(para4);

                                Paragraph para5 = new Paragraph("Consume By: " + dto[i].deliveryDate, new iTextSharp.text.Font(allerfont, 4));
                                para5.Alignment = Element.ALIGN_CENTER;
                                columnLeft.AddElement(para5);

                                Paragraph para6 = new Paragraph("At: " + dto[i].timePacked.Value.AddHours(4).ToString("hh: mm tt"), new iTextSharp.text.Font(allerfont, 4));
                                para6.Alignment = Element.ALIGN_CENTER;
                                columnLeft.AddElement(para6);

                                var phrase = new Phrase();
                                //phrase.Add(new Chunk("(" + dto[i].dishes[j].dish_code + ") - ", new Font(Font.FontFamily.HELVETICA, 8)));
                                phrase.Add(new Chunk(dto[i].dishes[j].dish_name, new iTextSharp.text.Font(allerfont, 5, iTextSharp.text.Font.BOLD)));

                                //Paragraph para7 = new Paragraph(phrase);
                                Paragraph para7 = new Paragraph(dto[i].dishes[j].dish_name, new iTextSharp.text.Font(allerfont, 5, iTextSharp.text.Font.BOLD));
                                para7.Alignment = Element.ALIGN_CENTER;
                                columnLeft.AddElement(para7);

                                columnLeft.Go();
                            }
                        }
                    }

                    document.Close();
                    writer.Close();


                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }

        }

        public async Task<byte[]> GenerateOrderLogXls(BaseFilter filter)
        {
            IQueryable<TokenOrder> query = _appContext.TokenOrders;
            filter.Page = -1;
            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            var orders = _mapper.Map<List<TokenOrderDTO>>(result.PagedData);

            if (orders != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Orders");
                    var headers = new string[] {
                                            "Class", "Profile", "Email", "FAS", "Invoice No.", "Payment Type", "Payment Number",
                                            "Fomo ID","Subtotal", "Discount", "GST", "Transaction Fee",
                                            "Fixed Transaction Fee", "Total Amount",
                                            "Voucher", "Processed By", "Delivery Date", "Transaction Time", "Session",
                                            "Dish Type", "Meal Description", "Amount", "Status", "Remarks", "Collected Date",
                                            "Collected Time", "Bento Code", "Returned Date", "Returned Time"};

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.FontName = "Arial";
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderRight = BorderStyle.Thin;

                    //ICell cell;
                    //int headerIndex = 0;
                    //for (; headerIndex < headers.Length; headerIndex++)
                    //{
                    //    cell = row.CreateCell(headerIndex);
                    //    cell.SetCellValue(headers[headerIndex]);
                    //    cell.CellStyle = borderedHeaderStyle;
                    //}
                    //sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    var contentFont = wb.CreateFont();
                    contentFont.FontName = "Arial";
                    contentStyle.SetFont(contentFont);
                    //contentStyle.BorderTop = BorderStyle.Thin;
                    //contentStyle.BorderBottom = BorderStyle.Thin;
                    //contentStyle.BorderLeft = BorderStyle.Thin;
                    //contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    //contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();

                    var ordersByProfile = orders.GroupBy(e => new { e.ProfileId }).Select(e => new
                    {
                        ProfileId = e.Key.ProfileId,
                        Orders = e
                    });

                    foreach (var oprofile in ordersByProfile)
                    {
                        var studentProfile = oprofile.Orders.First();
                        var studentHeaders = new List<string> { "Class", "Profile", "Email", "FAS" };
                        row = sheet.CreateRow(++rowCount);
                        CreateCellValues(row, 0, studentHeaders, borderedHeaderStyle);

                        var studentHeaderValues = new List<string>
                        {
                            studentProfile.ClassName,
                            studentProfile.StudentName,
                            studentProfile.StudentEmail,
                            studentProfile.IsFASDisplay
                        };

                        row = sheet.CreateRow(++rowCount);
                        CreateCellValues(row, 0, studentHeaderValues, contentStyle);

                        var ordersByPayment = oprofile.Orders.GroupBy(e => e.PaymentId).Select(e =>
                                                new { PaymentId = e.Key, Orders = e });

                        foreach (var oPayment in ordersByPayment)
                        {
                            var payment = oPayment.Orders.First();

                            //Payment header
                            var paymentHeaders = new List<string> {
                                            "Invoice No.", "Payment Type", "Payment Number",
                                            "Fomo ID","Subtotal", "Discount", "GST", "Transaction Fee",
                                            "Fixed Transaction Fee", "Total Amount",
                                            "Voucher", "Processed By" };

                            var paymentHeaderStyle = wb.CreateCellStyle();
                            paymentHeaderStyle.SetFont(headerFont);
                            paymentHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                            paymentHeaderStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
                            paymentHeaderStyle.FillPattern = FillPattern.SolidForeground;

                            row = sheet.CreateRow(++rowCount);
                            CreateCellValues(row, 1, paymentHeaders, borderedHeaderStyle);

                            var paymentValues = new List<string>
                            {
                                payment.InvoiceNumber,
                                payment.PaymentTypeName,
                                payment.PaymentNumber,
                                payment.FomoId,
                                payment.PaymentSubtotal.ToString(),
                                payment.Discount.ToString(),
                                payment.PaymentGst.ToString(),
                                payment.PaymentTransactionFee.ToString(),
                                payment.PaymentFixedTransactionFee.ToString(),
                                payment.PaymentTotalAmount.ToString(),
                                payment.VoucherCode,
                                payment.ProcessedBy
                            };

                            row = sheet.CreateRow(++rowCount);
                            CreateCellValues(row, 1, paymentValues, contentStyle);

                            // Order header
                            var dishHeaders = new List<string> {
                                            "Delivery Date", "Transaction Time", "Session",
                                            "Dish Type", "Meal Description", "Amount", "Status", "Remarks", "Collected Date",
                                            "Collected Time", "Bento Code", "Returned Date", "Returned Time" };

                            var orderHeaderStyle = wb.CreateCellStyle();
                            orderHeaderStyle.SetFont(headerFont);
                            orderHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                            orderHeaderStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
                            orderHeaderStyle.FillPattern = FillPattern.SolidForeground;

                            row = sheet.CreateRow(++rowCount);
                            CreateCellValues(row, 1, dishHeaders, borderedHeaderStyle);

                            // Order rows
                            foreach (var order in oPayment.Orders)
                            {
                                foreach (var t in payment.Tokens)
                                {
                                    var selectedDishes = t.SelectedDishes.ToArray();

                                    int j = 0, c = 0;
                                    foreach (var tod in selectedDishes)
                                    {
                                        for (j = 0; j < tod.Qty; j++)
                                        {
                                            var orderValues = new List<string>
                                            {
                                                order.DeliveryDate.ToString("dd/MM/yyyy hh:mm:ss"),
                                                order.TransactionTime.ToString("dd/MM/yyyy hh:mm:ss"),
                                                order.MealSessionDetailName,
                                                tod.DishTypeName,
                                                tod.DishLabel,
                                                order.TotalAmount.ToString(),
                                                order.Status,
                                                order.Remarks,
                                                order.CollectionTime != null ? order.CollectionTime.ToString().Split(' ')[0] : string.Empty,
                                                order.CollectionTime != null ? order.CollectionTime.ToString().Split(' ')[1] : string.Empty,
                                                order.BentoCode,
                                                order.ReturnTime != null ? order.ReturnTime.ToString().Split(' ')[0] : string.Empty,
                                                order.ReturnTime != null ? order.ReturnTime.ToString().Split(' ')[1] : string.Empty,
                                            };

                                            row = sheet.CreateRow(++rowCount);
                                            CreateCellValues(row, 1, orderValues, contentStyle);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region old way
                    /*
                    orders.ForEach(dt =>
                    {
                        foreach (var t in dt.Tokens)
                        {
                            var selectedDishes = t.SelectedDishes.ToArray();

                            int j = 0, c = 0;
                            foreach (var tod in selectedDishes)
                            {
                                for (j = 0; j < tod.Qty; j++)
                                {
                                    c = 0;
                                    row = sheet.CreateRow(++rowCount);

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.ClassName);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.StudentName);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.StudentEmail);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.IsFASDisplay);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.DeliveryDate.ToString("dd/MM/yyyy hh:mm:ss"));
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.TransactionTime.ToString("dd/MM/yyyy hh:mm:ss"));
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.MealSessionDetailName);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(tod.DishTypeName);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(tod.DishLabel);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.Status);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.Remarks);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.PaymentTypeName);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.PaymentNumber);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.FomoId);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.InvoiceNumber);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.PaymentSubtotal.ToString("0.##"));
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.Discount.ToString("0.##"));
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.PaymentGst.ToString("0.##"));
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.PaymentTransactionFee.ToString("0.##"));
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.PaymentFixedTransactionFee.ToString("0.##"));
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.PaymentTotalAmount.ToString("0.##"));
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.VoucherCode);
                                    cell.CellStyle = contentStyle;

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.ProcessedBy);
                                    cell.CellStyle = contentStyle;

                                    if (dt.CollectionTime != null)
                                    {
                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(dt.CollectionTime.ToString().Split(' ')[0]);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(dt.CollectionTime.ToString().Split(' ')[1]);
                                        cell.CellStyle = contentStyle;
                                    }
                                    else
                                    {
                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(" ");
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(" ");
                                        cell.CellStyle = contentStyle;
                                    }

                                    cell = row.CreateCell(c++);
                                    cell.SetCellValue(dt.BentoCode);
                                    cell.CellStyle = contentStyle;

                                    if (dt.ReturnTime != null)
                                    {
                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(dt.ReturnTime.ToString().Split(' ')[0]);
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(dt.ReturnTime.ToString().Split(' ')[1]);
                                        cell.CellStyle = contentStyle;
                                    }
                                    else
                                    {
                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(" ");
                                        cell.CellStyle = contentStyle;

                                        cell = row.CreateCell(c++);
                                        cell.SetCellValue(" ");
                                        cell.CellStyle = contentStyle;
                                    }
                                }
                            }

                            //for (int k = j; k < t.Qty; k++)
                            //{
                            //    c = 0;
                            //    row = sheet.CreateRow(++rowCount);

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.ClassName);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.StudentName);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.StudentEmail);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.IsFASDisplay);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.DeliveryDate.ToString("dd/MM/yyyy hh:mm:ss"));
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.TransactionTime.ToString("dd/MM/yyyy hh:mm:ss"));
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.MealSessionDetailName);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(t.TokenDesc);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(t.TokenName);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.TotalAmount);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.Status);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.Remarks);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.PaymentTypeName);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.PaymentNumber);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.FomoId);
                            //    cell.CellStyle = contentStyle;

                            //    cell = row.CreateCell(c++);
                            //    cell.SetCellValue(dt.ProcessedBy);
                            //    cell.CellStyle = contentStyle;
                            //}
                        }
                    });
                    */
                    #endregion

                    for (var i = 0; i < 30; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        private void CreateCellValues(IRow row, int cellStart, List<string> values, ICellStyle style)
        {
            ICell cell;
            for (int i = 0; i < values.Count(); i++)
            {
                cell = row.CreateCell(cellStart++);
                cell.SetCellValue(values[i]);
                cell.CellStyle = style;
            }
        }

        public async Task<PagedEntity<TokenOrderDTO>> GetSalesOrdersAsync(SalesOrderReportFilter filter)
        {
            var salesOrders = await _uow.TokenOrders.GetSalesOrders(filter);
            int total = salesOrders != null && salesOrders.Any() ? salesOrders.FirstOrDefault().Total : 0;
            var result = new PagedEntity<TokenOrderDTO>();
            result.Filter = filter;
            result.PagedData = _mapper.Map<List<TokenOrderDTO>>(salesOrders);
            result.CurrentPage = filter.Page ?? 1;
            result.PageSize = filter.PageSize ?? 10;
            result.PageCount = total / result.PageSize;
            result.TotalCount = total;

            return result;
        }

        public async Task<PagedEntity<VoucherUtilisation>> GetVoucherUtilisationsAsync(VoucherUtilisationReportFilter filter)
        {
            var orders = await _uow.TokenOrders.GetVoucherUtilisations(filter);
            int total = orders != null && orders.Any() ? orders.FirstOrDefault().Total : 0;
            var result = new PagedEntity<VoucherUtilisation>();
            result.Filter = filter;
            result.PagedData = _mapper.Map<List<VoucherUtilisation>>(orders);
            result.CurrentPage = filter.Page ?? 1;
            result.PageSize = filter.PageSize ?? 10;
            result.PageCount = total / result.PageSize;
            result.TotalCount = total;

            return result;
        }

        public async Task<byte[]> GenerateOrderLogXls2(SalesOrderReportFilter filter)
        {
            var orders = await _uow.TokenOrders.GetSalesOrders(filter);
            string dateFormat = "dd/MM/yy";
            string dateTimeFormat = "dd/MM/yy hh:mm tt";
            string decimalFormat = "0.00";

            if (orders != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Orders");
                    var headers = new List<string>
                    {
                        "Student Id",
                        "Invoice No.",
                        "Name",
                        "Outlet",
                        "Class/Department",
                        "Order Date",
                        "Status",
                        "Cancellation Reason",
                        "Discount Code",
                        "Discount",
                        "Total Gst",
                        "Transaction Fee",
                        "Fix Transaction Fee",
                        "Total Amount",
                        "FomoId",
                        "Payment Status",
                        "Delivery Date",
                        "Meal Session",
                        "Meal Type",
                        "Meal Name",
                        "Qty",
                        "Dish Price",
                        "Collected Date/Time",
                        "Bento Code",
                        "Returned Date/Time",
                        "Is FAS"
                    };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.FontName = "Calibri";
                    headerFont.FontHeightInPoints = 11;
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount++);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderRight = BorderStyle.Thin;

                    ICell cell;

                    cell = row.CreateCell(0);
                    cell.SetCellValue("Sales Order Report");
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Start Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateFrom.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("End Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateTo.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    rowCount += 2;
                    row = sheet.CreateRow(rowCount++);
                    for (int headerIndex = 0; headerIndex < headers.Count; headerIndex++)
                    {
                        cell = row.CreateCell(headerIndex);
                        cell.SetCellValue(headers[headerIndex]);
                        cell.CellStyle = borderedHeaderStyle;
                    }

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    var contentFont = wb.CreateFont();
                    contentFont.FontName = "Calibri";
                    contentFont.FontHeightInPoints = 11;
                    contentStyle.SetFont(contentFont);
                    //contentStyle.BorderTop = BorderStyle.Thin;
                    //contentStyle.BorderBottom = BorderStyle.Thin;
                    //contentStyle.BorderLeft = BorderStyle.Thin;
                    //contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    //contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();


                    IDataFormat format = wb.CreateDataFormat();
                    ICellStyle dateCellStyle = wb.CreateCellStyle();
                    dateCellStyle.DataFormat = format.GetFormat(dateFormat);


                    var dateTimeCellStyle = wb.CreateCellStyle();
                    format = wb.CreateDataFormat();
                    dateTimeCellStyle.DataFormat = format.GetFormat(dateTimeFormat);

                    var createHelper = wb.GetCreationHelper();
                    //var format = wb.CreateDataFormat();
                    //var dateCellStyle = wb.CreateCellStyle();
                    //dateCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateFormat);
                    dateCellStyle.Alignment = HorizontalAlignment.Right;
                    dateCellStyle.SetFont(contentFont);

                    //var dateTimeCellStyle = wb.CreateCellStyle();
                    //dateTimeCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateTimeFormat);
                    dateTimeCellStyle.Alignment = HorizontalAlignment.Right;
                    dateTimeCellStyle.SetFont(contentFont);

                    var numericCellStyle = wb.CreateCellStyle();
                    numericCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(decimalFormat);
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.SetFont(contentFont);


                    int col = 0;
                    string invoice = null;
                    string studentName = null;
                    orders = orders.OrderByDescending(e => e.InvoiceNumber).ThenBy(e => e.StudentName).ToList();
                    var grpOrders = orders.GroupBy(e => new { e.InvoiceNumber });
                    foreach (var grp in grpOrders)
                    {
                        var paymentDetails = grp.First();
                        var invoiceNumber = paymentDetails.InvoiceNumber;

                        col = 0;
                        row = sheet.CreateRow(rowCount++);

                        #region Student Orders

                        var studentOrders = grp.GroupBy(e => e.StudentId);
                        int firstStudentRecord = 0;
                        foreach (var o in studentOrders)
                        {
                            col = 0;

                            if (firstStudentRecord > 0)
                            {
                                row = sheet.CreateRow(rowCount++);
                            }

                            firstStudentRecord++;

                            var studentDetails = o.First();

                            #region Payment 
                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.StudentId);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(invoiceNumber);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.StudentName);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.Outlet);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.ClassName);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.OrderDate);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = dateCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.Status);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.CancellationReason);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.DiscountCode);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.Discount);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.PaymentGst);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.PaymentTransactionFee);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.PaymentFixedTransactionFee);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.PaymentTotalAmount);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.FomoId);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(studentDetails.PaymentStatus);
                            cell.CellStyle = contentStyle;

                            #endregion

                            int firstOrder = 0;
                            o.ToList().ForEach(dt =>
                            {
                                col = 15;

                                if (firstOrder > 0)
                                {
                                    row = sheet.CreateRow(rowCount++);
                                }

                                firstOrder++;
                                cell = row.CreateCell(col++);
                                cell.SetCellValue(dt.DeliveryDate);
                                cell.SetCellType(CellType.Numeric);
                                cell.CellStyle = dateCellStyle;

                                cell = row.CreateCell(col++);
                                cell.SetCellValue(dt.MealSessionDetailName);
                                cell.CellStyle = contentStyle;

                                cell = row.CreateCell(col++);
                                cell.SetCellValue(dt.MealTypeName);
                                cell.CellStyle = contentStyle;

                                cell = row.CreateCell(col++);
                                cell.SetCellValue(dt.DishLabel);
                                cell.CellStyle = contentStyle;

                                cell = row.CreateCell(col++);
                                cell.SetCellValue(dt.Quantity);
                                cell.SetCellType(CellType.Numeric);
                                cell.CellStyle = contentStyle;

                                cell = row.CreateCell(col++);
                                cell.SetCellValue(dt.DishPrice);
                                cell.SetCellType(CellType.Numeric);
                                cell.CellStyle = numericCellStyle;

                                cell = row.CreateCell(col++);
                                cell.SetCellValue(dt.CollectionTime);
                                cell.SetCellType(CellType.Numeric);
                                cell.CellStyle = dateTimeCellStyle;

                                cell = row.CreateCell(col++);
                                cell.SetCellValue(dt.BentoCode);
                                cell.CellStyle = contentStyle;

                                cell = row.CreateCell(col++);
                                cell.SetCellValue(dt.ReturnTime);
                                cell.SetCellType(CellType.Numeric);
                                cell.CellStyle = dateTimeCellStyle;

                                cell = row.CreateCell(col++);
                                cell.SetCellValue(dt.IsFAS ? "Y" : "N");
                            });

                        }
                        #endregion

                        //o.ForEach(dt =>
                        //{
                        //    col = 0;
                        //    row = sheet.CreateRow(rowCount++);

                        //    if (dt.InvoiceNumber?.Trim() != invoice?.Trim() ||
                        //        (dt.InvoiceNumber?.Trim() == invoice?.Trim() && dt.StudentName?.Trim() != studentName?.Trim()))
                        //    {
                        //        #region Payment 
                        //        invoice = dt.InvoiceNumber;
                        //        studentName = dt.StudentName;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.InvoiceNumber);
                        //        cell.CellStyle = contentStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.StudentName);
                        //        cell.CellStyle = contentStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.Outlet);
                        //        cell.CellStyle = contentStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.ClassName);
                        //        cell.CellStyle = contentStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.OrderDate.ToString(dateFormat));
                        //        cell.CellStyle = dateCellStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.Status);
                        //        cell.CellStyle = contentStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.CancellationReason);
                        //        cell.CellStyle = contentStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.DiscountCode);
                        //        cell.CellStyle = contentStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.Discount);
                        //        cell.SetCellType(CellType.Numeric);
                        //        cell.CellStyle = numericCellStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.PaymentGst);
                        //        cell.SetCellType(CellType.Numeric);
                        //        cell.CellStyle = numericCellStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.PaymentTransactionFee);
                        //        cell.SetCellType(CellType.Numeric);
                        //        cell.CellStyle = numericCellStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.PaymentFixedTransactionFee);
                        //        cell.SetCellType(CellType.Numeric);
                        //        cell.CellStyle = numericCellStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.PaymentTotalAmount);
                        //        cell.SetCellType(CellType.Numeric);
                        //        cell.CellStyle = numericCellStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.FomoId);
                        //        cell.CellStyle = contentStyle;

                        //        cell = row.CreateCell(col++);
                        //        cell.SetCellValue(dt.PaymentStatus);
                        //        cell.CellStyle = contentStyle;

                        //        #endregion
                        //    }
                        //    else
                        //    {
                        //        col += 15;
                        //    }

                        //    cell = row.CreateCell(col++);
                        //    cell.SetCellValue(dt.DeliveryDate.ToString(dateFormat));
                        //    cell.CellStyle = dateCellStyle;

                        //    cell = row.CreateCell(col++);
                        //    cell.SetCellValue(dt.MealSessionDetailName);
                        //    cell.CellStyle = contentStyle;

                        //    cell = row.CreateCell(col++);
                        //    cell.SetCellValue(dt.MealTypeName);
                        //    cell.CellStyle = contentStyle;

                        //    cell = row.CreateCell(col++);
                        //    cell.SetCellValue(dt.DishLabel);
                        //    cell.CellStyle = contentStyle;

                        //    cell = row.CreateCell(col++);
                        //    cell.SetCellValue(dt.Quantity.ToString());
                        //    cell.CellStyle = contentStyle;

                        //    cell = row.CreateCell(col++);
                        //    cell.SetCellValue(dt.DishPrice);
                        //    cell.SetCellType(CellType.Numeric);
                        //    cell.CellStyle = numericCellStyle;

                        //    cell = row.CreateCell(col++);
                        //    cell.SetCellValue(dt.CollectionTime);
                        //    cell.CellStyle = dateTimeCellStyle;

                        //    cell = row.CreateCell(col++);
                        //    cell.SetCellValue(dt.BentoCode);
                        //    cell.CellStyle = contentStyle;

                        //    cell = row.CreateCell(col++);
                        //    cell.SetCellValue(dt.ReturnTime);
                        //    cell.CellStyle = dateTimeCellStyle;

                        //});
                    }



                    #endregion

                    for (var i = 0; i < 30; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<byte[]> GenerateFlattenOrderLogXls(SalesOrderReportFilter filter)
        {
            var orders = await _uow.TokenOrders.GetSalesOrders(filter);
            string dateFormat = "dd/MM/yy";
            string dateTimeFormat = "dd/MM/yy hh:mm tt";
            string decimalFormat = "0.00";

            if (orders != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Orders");
                    var headers = new List<string>
                    {
                        "Invoice No.",
                        "Name",
                        "Outlet",
                        "Class/Department",
                        "Order Date",
                        "Status",
                        "Cancellation Reason",
                        "Discount Code",
                        "Subtotal",
                        "Discount",
                        "Total Gst",
                        "Transaction Fee",
                        "Fix Transaction Fee",
                        "Total Amount",
                        "FomoId",
                        "Payment Status",
                        "Delivery Date",
                        "Meal Session",
                        "Meal Type",
                        "Meal Name",
                        "Qty",
                        "Dish Price",
                        "Collected Date/Time",
                        "Bento Code",
                        "Returned Date/Time",
                        "IsFAS"
                    };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.FontName = "Calibri";
                    headerFont.FontHeightInPoints = 11;
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount++);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderRight = BorderStyle.Thin;

                    ICell cell;

                    cell = row.CreateCell(0);
                    cell.SetCellValue("Sales Order Report");
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Start Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateFrom.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("End Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateTo.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    rowCount += 2;
                    row = sheet.CreateRow(rowCount++);
                    for (int headerIndex = 0; headerIndex < headers.Count; headerIndex++)
                    {
                        cell = row.CreateCell(headerIndex);
                        cell.SetCellValue(headers[headerIndex]);
                        cell.CellStyle = borderedHeaderStyle;
                    }

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    var contentFont = wb.CreateFont();
                    contentFont.FontName = "Calibri";
                    contentFont.FontHeightInPoints = 11;
                    contentStyle.SetFont(contentFont);
                    //contentStyle.BorderTop = BorderStyle.Thin;
                    //contentStyle.BorderBottom = BorderStyle.Thin;
                    //contentStyle.BorderLeft = BorderStyle.Thin;
                    //contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    //contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();


                    IDataFormat format = wb.CreateDataFormat();
                    ICellStyle dateCellStyle = wb.CreateCellStyle();
                    dateCellStyle.DataFormat = format.GetFormat(dateFormat);


                    var dateTimeCellStyle = wb.CreateCellStyle();
                    format = wb.CreateDataFormat();
                    dateTimeCellStyle.DataFormat = format.GetFormat(dateTimeFormat);

                    var createHelper = wb.GetCreationHelper();
                    //var format = wb.CreateDataFormat();
                    //var dateCellStyle = wb.CreateCellStyle();
                    //dateCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateFormat);
                    dateCellStyle.Alignment = HorizontalAlignment.Right;
                    dateCellStyle.SetFont(contentFont);

                    //var dateTimeCellStyle = wb.CreateCellStyle();
                    //dateTimeCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateTimeFormat);
                    dateTimeCellStyle.Alignment = HorizontalAlignment.Right;
                    dateTimeCellStyle.SetFont(contentFont);

                    var numericCellStyle = wb.CreateCellStyle();
                    numericCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(decimalFormat);
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.SetFont(contentFont);


                    int col = 0;
                    string invoice = null;
                    string studentName = null;
                    orders.ForEach(dt =>
                    {
                        col = 0;
                        row = sheet.CreateRow(rowCount++);

                        #region Payment 
                        invoice = dt.InvoiceNumber;
                        studentName = dt.StudentName;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.InvoiceNumber);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.StudentName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Outlet);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.ClassName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.OrderDate);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Status);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.CancellationReason);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.DiscountCode);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Subtotal);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Discount);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentGst);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentTransactionFee);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentFixedTransactionFee);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentTotalAmount);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.FomoId);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentStatus);
                        cell.CellStyle = contentStyle;

                        #endregion

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.DeliveryDate);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.MealSessionDetailName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.MealTypeName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.DishLabel);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Quantity);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.DishPrice);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.CollectionTime);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateTimeCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.BentoCode);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.ReturnTime);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateTimeCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.IsFAS ? "Y" : "N");
                    });

                    #endregion

                    for (var i = 0; i < 30; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<byte[]> GeneratePaymentReport(SalesOrderReportFilter filter)
        {
            var orders = await _uow.TokenOrders.GetSalesOrders(filter);
            string dateFormat = "dd/MM/yy";
            string dateTimeFormat = "dd/MM/yy hh:mm tt";
            string decimalFormat = "0.00";

            if (orders != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Orders");
                    var headers = new List<string>
                    {
                        "Invoice No.",
                        "Transaction Date",
                        "Outlet",
                        "Name",
                        "Total Gst",
                        "Subtotal",
                        "Transaction Fee",
                        "Fix Transaction Fee",
                        "Total Amount",
                        "Payment Method",
                        "FomoId",
                        "Payment Status",
                        "Order Status"
                    };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.FontName = "Calibri";
                    headerFont.FontHeightInPoints = 11;
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount++);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderRight = BorderStyle.Thin;

                    ICell cell;

                    cell = row.CreateCell(0);
                    cell.SetCellValue("Sales Order Report");
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Start Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateFrom.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("End Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateTo.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    rowCount += 2;
                    row = sheet.CreateRow(rowCount++);
                    for (int headerIndex = 0; headerIndex < headers.Count; headerIndex++)
                    {
                        cell = row.CreateCell(headerIndex);
                        cell.SetCellValue(headers[headerIndex]);
                        cell.CellStyle = borderedHeaderStyle;
                    }

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    var contentFont = wb.CreateFont();
                    contentFont.FontName = "Calibri";
                    contentFont.FontHeightInPoints = 11;
                    contentStyle.SetFont(contentFont);
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    var dataFormatCustom = wb.CreateDataFormat();

                    IDataFormat format = wb.CreateDataFormat();
                    ICellStyle dateCellStyle = wb.CreateCellStyle();
                    dateCellStyle.DataFormat = format.GetFormat(dateFormat);


                    var dateTimeCellStyle = wb.CreateCellStyle();
                    format = wb.CreateDataFormat();
                    dateTimeCellStyle.DataFormat = format.GetFormat(dateTimeFormat);

                    var createHelper = wb.GetCreationHelper();
                    //var format = wb.CreateDataFormat();
                    //var dateCellStyle = wb.CreateCellStyle();
                    //dateCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateFormat);
                    dateCellStyle.Alignment = HorizontalAlignment.Right;
                    dateCellStyle.SetFont(contentFont);

                    //var dateTimeCellStyle = wb.CreateCellStyle();
                    //dateTimeCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateTimeFormat);
                    dateTimeCellStyle.Alignment = HorizontalAlignment.Right;
                    dateTimeCellStyle.SetFont(contentFont);

                    var numericCellStyle = wb.CreateCellStyle();
                    numericCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(decimalFormat);
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.SetFont(contentFont);


                    int col = 0;
                    string invoice = null;
                    string studentName = null;
                    orders.ForEach(dt =>
                    {
                        col = 0;
                        row = sheet.CreateRow(rowCount++);

                        #region Payment 
                        invoice = dt.InvoiceNumber;
                        studentName = dt.StudentName;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.InvoiceNumber);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.OrderDate);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Outlet);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.StudentName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentGst);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Subtotal);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentTransactionFee);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentFixedTransactionFee);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentTotalAmount);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.FomoId);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentMethod);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentStatus);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Status);
                        cell.CellStyle = contentStyle;

                        #endregion
                    });

                    #endregion

                    for (var i = 0; i < 30; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<byte[]> GenerateVoucherUtilisationReport(VoucherUtilisationReportFilter filter)
        {
            var orders = await _uow.TokenOrders.GetVoucherUtilisations(filter);
            string dateFormat = "dd/MM/yy";
            string dateTimeFormat = "dd/MM/yy hh:mm tt";
            string decimalFormat = "0.00";

            if (orders != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Vouchers");
                    var headers = new List<string>
                    {
                        "Voucher Name",
                        "Voucher Code",
                        "Value",
                        "Discount Type",
                        "Validity Start Date",
                        "Validity End Date",
                        "Name",
                        "Class",
                        "Utilized Date",
                        "InvoiceNumber",
                        "Amount",
                        "Voucher Status"
                    };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.FontName = "Calibri";
                    headerFont.FontHeightInPoints = 11;
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount++);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderRight = BorderStyle.Thin;

                    ICell cell;

                    cell = row.CreateCell(0);
                    cell.SetCellValue("Voucher Utilisation Report");
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Start Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateFrom.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("End Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateTo.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    rowCount += 2;
                    row = sheet.CreateRow(rowCount++);
                    for (int headerIndex = 0; headerIndex < headers.Count; headerIndex++)
                    {
                        cell = row.CreateCell(headerIndex);
                        cell.SetCellValue(headers[headerIndex]);
                        cell.CellStyle = borderedHeaderStyle;
                    }

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    var contentFont = wb.CreateFont();
                    contentFont.FontName = "Calibri";
                    contentFont.FontHeightInPoints = 11;
                    contentStyle.SetFont(contentFont);
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    var dataFormatCustom = wb.CreateDataFormat();


                    IDataFormat format = wb.CreateDataFormat();
                    ICellStyle dateCellStyle = wb.CreateCellStyle();
                    dateCellStyle.DataFormat = format.GetFormat(dateFormat);

                    var createHelper = wb.GetCreationHelper();
                    //var format = wb.CreateDataFormat();
                    //var dateCellStyle = wb.CreateCellStyle();
                    //dateCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateFormat);
                    dateCellStyle.Alignment = HorizontalAlignment.Right;
                    dateCellStyle.SetFont(contentFont);

                    var dateTimeCellStyle = wb.CreateCellStyle();
                    format = wb.CreateDataFormat();
                    dateTimeCellStyle.DataFormat = format.GetFormat(dateTimeFormat);

                    //dateTimeCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateTimeFormat);
                    dateTimeCellStyle.Alignment = HorizontalAlignment.Right;
                    dateTimeCellStyle.SetFont(contentFont);

                    var numericCellStyle = wb.CreateCellStyle();
                    numericCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(decimalFormat);
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.SetFont(contentFont);


                    int col = 0;
                    orders.ForEach(dt =>
                    {
                        col = 0;
                        row = sheet.CreateRow(rowCount++);

                        #region 

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.VoucherName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.VoucherCode);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.VoucherAmount);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;


                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.DiscountType);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellType(CellType.Numeric);
                        cell.SetCellValue(dt.ValidityStartDate);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellType(CellType.Numeric);
                        cell.SetCellValue(dt.ValidityEndDate);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.StudentName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.ClassName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellType(CellType.Numeric);
                        if (dt.UtilisedDate.Date != DateTime.MinValue.Date)
                            cell.SetCellValue(dt.UtilisedDate.Date);

                        //cell.SetCellValue(dt.UtilisedDate.Date == DateTime.MinValue.Date ? string.Empty : dt.UtilisedDate.Date);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.InvoiceNumber);

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Discount);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.VoucherStatus);
                        cell.CellStyle = contentStyle;

                        #endregion
                    });

                    #endregion

                    for (var i = 0; i < 30; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<List<SalesOrderCollectionSummary>> GetSalesOrderCollectionSummary(DateTime orderDate, int outletId)
        {
            var orders = _appContext.TokenOrders.Where(e => e.IsActive && e.Status == "paid" &&
                                        e.DeliveryDate.Date == orderDate.Date);

            var mealSessions = _appContext.MealSessions.Where(e => e.IsActive && e.OutletId == outletId)
                                       .OrderBy(e => e.StartDate);

            var results = new List<SalesOrderCollectionSummary>();

            foreach (var mealSession in mealSessions)
            {
                var orderByMealSession = orders.Where(e => e.Session.MealSessionId == mealSession.Id).ToList();
                var orderByMealSessionId = orderByMealSession.Select(t => t.Id).ToList();

                int returnables = orderByMealSession == null ? 0 : _appContext.TokenOrderDishes.Where(e => e.IsActive &&
                                            orderByMealSessionId.Contains(e.TokenOrdered.OrderId) && e.Dish.BentoBoxType.isRFID).Count();

                var summary = new SalesOrderCollectionSummary
                {
                    TotalOrder = orderByMealSession != null ? orderByMealSession.Count : 0,
                    MealSession = mealSession.Name,
                    TotalCollected = orderByMealSession != null ? orderByMealSession.Count(f => f.CollectionTime.HasValue) : 0,
                    TotalReturnable = returnables,
                    TotalReturnedBento = orderByMealSession != null ? orderByMealSession.Count(f => f.ReturnTime.HasValue) : 0
                };

                results.Add(summary);
            }

            return results;
        }
        #endregion

        #region Reports

        #region Cancelled Orders
        public async Task<byte[]> GenerateCancelledOrdersXls(SalesOrderReportFilter filter)
        {
            var orders = await _uow.TokenOrders.GetSalesOrders(filter);
            string dateFormat = "dd/MM/yy";
            string dateTimeFormat = "dd/MM/yy hh:mm tt";
            string decimalFormat = "0.00";

            if (orders != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Order Cancellation Report");
                    var headers = new List<string>
                    {
                        "Order No.",
                        "Name",
                        "Outlet",
                        "Class/Department",
                        "Cancelled Date",
                        "Order Date",
                        "Delivery Date",
                        "Meal Session",
                        "Meal Type",
                        "Meal Name",
                        "Discount Code",
                        "Qty",
                        "Subtotal",
                        "Discount",
                        "Subtotal after discount",
                        "Total Gst",
                        "Transaction Fee",
                        "Fix Transaction Fee",
                        "Total Amount",
                        "Payment Status",
                        "Order Status",
                        "Cancellation Reason"
                    };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.FontName = "Calibri";
                    headerFont.FontHeightInPoints = 11;
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount++);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                    ICell cell;

                    cell = row.CreateCell(0);
                    cell.SetCellValue("Order Cancellation Report");
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Start Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateFrom.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("End Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateTo.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    var contentFont = wb.CreateFont();
                    contentFont.FontName = "Calibri";
                    contentFont.FontHeightInPoints = 11;
                    contentStyle.SetFont(contentFont);
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    var dataFormatCustom = wb.CreateDataFormat();


                    IDataFormat format = wb.CreateDataFormat();
                    ICellStyle dateCellStyle = wb.CreateCellStyle();
                    dateCellStyle.DataFormat = format.GetFormat(dateFormat);

                    var createHelper = wb.GetCreationHelper();
                    //var format = wb.CreateDataFormat();
                    //var dateCellStyle = wb.CreateCellStyle();
                    //dateCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateFormat);
                    dateCellStyle.Alignment = HorizontalAlignment.Right;
                    dateCellStyle.SetFont(contentFont);

                    format = wb.CreateDataFormat();
                    ICellStyle dateTimeCellStyle = wb.CreateCellStyle();
                    dateTimeCellStyle.DataFormat = format.GetFormat(dateTimeFormat);

                    //var dateTimeCellStyle = wb.CreateCellStyle();
                    //dateTimeCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateTimeFormat);
                    dateTimeCellStyle.Alignment = HorizontalAlignment.Right;
                    dateTimeCellStyle.SetFont(contentFont);

                    var numericCellStyle = wb.CreateCellStyle();
                    numericCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(decimalFormat);
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.SetFont(contentFont);


                    int col = 0;
                    string invoice = null;
                    string studentName = null;
                    var grpMealOrders = orders.GroupBy(e => e.InvoiceNumber).Select(e => new { InvoiceNumber = e.Key, Orders = e });

                    var oneMealOrders = new List<spSalesOrderReport>();
                    var wholeMealOrders = new List<spSalesOrderReport>();
                    foreach (var mo in grpMealOrders)
                    {
                        var isWholeOrder = mo.Orders.Count() == mo.Orders.First().OrderCount;
                        if (isWholeOrder)
                        {
                            //this means whole order was cancelled
                            wholeMealOrders.AddRange(mo.Orders);
                        }
                        else
                        {
                            //specific meal was cancelled
                            oneMealOrders.AddRange(mo.Orders);
                        }
                    }

                    rowCount += 2;
                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Cancellation of the 1 meal in an order");
                    cell.CellStyle = contentStyle;

                    row = sheet.CreateRow(rowCount++);
                    for (int headerIndex = 0; headerIndex < headers.Count; headerIndex++)
                    {
                        cell = row.CreateCell(headerIndex);
                        cell.SetCellValue(headers[headerIndex]);
                        cell.CellStyle = borderedHeaderStyle;
                    }

                    oneMealOrders.ForEach(dt =>
                    {
                        col = 0;
                        row = sheet.CreateRow(rowCount++);

                        #region Payment

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentNumber);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.StudentName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Outlet);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.ClassName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.CancelledOn);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.OrderDate);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.DeliveryDate);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.MealSessionDetailName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.MealTypeName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.DishLabel);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.DiscountCode);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Quantity);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Subtotal);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Discount);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.SubDiscTotal);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentGst);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentTransactionFee);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentFixedTransactionFee);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentTotalAmount);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.PaymentStatus);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Status);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.CancellationReason);
                        cell.CellStyle = contentStyle;

                        #endregion
                    });


                    rowCount += 2;
                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Cancellation of the whole order");
                    cell.CellStyle = contentStyle;

                    row = sheet.CreateRow(rowCount++);
                    for (int headerIndex = 0; headerIndex < headers.Count; headerIndex++)
                    {
                        cell = row.CreateCell(headerIndex);
                        cell.SetCellValue(headers[headerIndex]);
                        cell.CellStyle = borderedHeaderStyle;
                    }

                    var grpWholeMealOrders = wholeMealOrders.GroupBy(e => new { e.PaymentNumber, e.StudentId }).
                                            Select(e => new
                                            {
                                                PaymentNumber = e.Key.PaymentNumber,
                                                StudentId = e.Key.StudentId,
                                                Orders = e.OrderBy(f => f.DeliveryDate).ToList()
                                            });

                    row = sheet.CreateRow(rowCount++);
                    foreach (var wmorders in grpWholeMealOrders)
                    {
                        var orderHeader = wmorders.Orders.First();
                        col = 0;
                        invoice = orderHeader.InvoiceNumber;
                        studentName = orderHeader.StudentName;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(orderHeader.PaymentNumber);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(orderHeader.StudentName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(orderHeader.Outlet);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(orderHeader.ClassName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(orderHeader.CancelledOn);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(orderHeader.OrderDate);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateCellStyle;

                        wmorders.Orders.ForEach(dt =>
                        {
                            #region 

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.DeliveryDate);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = dateCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.MealSessionDetailName);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.MealTypeName);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.DishLabel);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.DiscountCode);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.Quantity);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.Subtotal);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.Discount);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.SubDiscTotal);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.PaymentGst);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.PaymentTransactionFee);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.PaymentFixedTransactionFee);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.PaymentTotalAmount);
                            cell.SetCellType(CellType.Numeric);
                            cell.CellStyle = numericCellStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.PaymentStatus);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.Status);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(col++);
                            cell.SetCellValue(dt.CancellationReason);
                            cell.CellStyle = contentStyle;

                            col = 6;
                            row = sheet.CreateRow(rowCount++);
                            #endregion
                        });

                    }
                    #endregion

                    for (var i = 0; i < 30; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        public async Task<byte[]> GenerateOrderCollectionXls(SalesOrderReportFilter filter)
        {
            var orders = await _uow.TokenOrders.GetSalesOrders(filter);
            string dateFormat = "dd/MM/yy";
            string dateTimeFormat = "dd/MM/yy hh:mm tt";
            string decimalFormat = "0.00";

            if (orders != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Orders");
                    var headers = new List<string>
                    {
                        "Group",
                        "Invoice No.",
                        "Name",
                        //"Outlet",
                        "Class/Department",
                        "Order Date",
                        "Status",
                        //"Cancellation Reason",
                        //"Discount Code",
                        //"Subtotal",
                        //"Discount",
                        //"Total Gst",
                        //"Transaction Fee",
                        //"Fix Transaction Fee",
                        //"Total Amount",
                        //"FomoId",
                        //"Payment Status",
                        "Delivery Date",
                        "Meal Session",
                        "Meal Type",
                        "Meal Name",
                        //"Qty",
                        //"Dish Price",
                        "Collected Date/Time",
                        "Bento Code",
                        "Returned Date/Time",
                        "IsFAS"
                    };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.FontName = "Calibri";
                    headerFont.FontHeightInPoints = 11;
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount++);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    //borderedHeaderStyle.BorderRight = BorderStyle.Thin;

                    ICell cell;

                    cell = row.CreateCell(0);
                    cell.SetCellValue("Order Collection Report");
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Start Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateFrom.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("End Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(filter.ReportDateTo.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    rowCount += 2;
                    row = sheet.CreateRow(rowCount++);
                    for (int headerIndex = 0; headerIndex < headers.Count; headerIndex++)
                    {
                        cell = row.CreateCell(headerIndex);
                        cell.SetCellValue(headers[headerIndex]);
                        cell.CellStyle = borderedHeaderStyle;
                    }

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    var contentFont = wb.CreateFont();
                    contentFont.FontName = "Calibri";
                    contentFont.FontHeightInPoints = 11;
                    contentStyle.SetFont(contentFont);
                    //contentStyle.BorderTop = BorderStyle.Thin;
                    //contentStyle.BorderBottom = BorderStyle.Thin;
                    //contentStyle.BorderLeft = BorderStyle.Thin;
                    //contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    //contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();


                    IDataFormat format = wb.CreateDataFormat();
                    ICellStyle dateCellStyle = wb.CreateCellStyle();
                    dateCellStyle.DataFormat = format.GetFormat(dateFormat);


                    var dateTimeCellStyle = wb.CreateCellStyle();
                    format = wb.CreateDataFormat();
                    dateTimeCellStyle.DataFormat = format.GetFormat(dateTimeFormat);

                    var createHelper = wb.GetCreationHelper();
                    //var format = wb.CreateDataFormat();
                    //var dateCellStyle = wb.CreateCellStyle();
                    dateCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateFormat);
                    dateCellStyle.Alignment = HorizontalAlignment.Right;
                    dateCellStyle.SetFont(contentFont);

                    //var dateTimeCellStyle = wb.CreateCellStyle();
                    //dateTimeCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateTimeFormat);
                    dateTimeCellStyle.Alignment = HorizontalAlignment.Right;
                    dateTimeCellStyle.SetFont(contentFont);

                    var numericCellStyle = wb.CreateCellStyle();
                    numericCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(decimalFormat);
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.SetFont(contentFont);


                    int col = 0;
                    string invoice = null;
                    string studentName = null;
                    orders.ForEach(dt =>
                    {
                        col = 0;
                        row = sheet.CreateRow(rowCount++);

                        #region Payment 
                        invoice = dt.InvoiceNumber;

                        invoice = dt.InvoiceNumber;
                        studentName = dt.StudentName;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.StudentGroupName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.InvoiceNumber);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.StudentName);
                        cell.CellStyle = contentStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.Outlet);
                        //cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.ClassName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.OrderDate);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.Status);
                        cell.CellStyle = contentStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.CancellationReason);
                        //cell.CellStyle = contentStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.DiscountCode);
                        //cell.CellStyle = contentStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.Subtotal);
                        //cell.SetCellType(CellType.Numeric);
                        //cell.CellStyle = numericCellStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.Discount);
                        //cell.SetCellType(CellType.Numeric);
                        //cell.CellStyle = numericCellStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.PaymentGst);
                        //cell.SetCellType(CellType.Numeric);
                        //cell.CellStyle = numericCellStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.PaymentTransactionFee);
                        //cell.SetCellType(CellType.Numeric);
                        //cell.CellStyle = numericCellStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.PaymentFixedTransactionFee);
                        //cell.SetCellType(CellType.Numeric);
                        //cell.CellStyle = numericCellStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.PaymentTotalAmount);
                        //cell.SetCellType(CellType.Numeric);
                        //cell.CellStyle = numericCellStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.FomoId);
                        //cell.CellStyle = contentStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.PaymentStatus);
                        //cell.CellStyle = contentStyle;

                        #endregion

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.DeliveryDate);
                        cell.CellStyle = dateCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.MealSessionDetailName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.MealTypeName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.DishLabel);
                        cell.CellStyle = contentStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.Quantity.ToString());
                        //cell.CellStyle = contentStyle;

                        //cell = row.CreateCell(col++);
                        //cell.SetCellValue(dt.DishPrice);
                        //cell.SetCellType(CellType.Numeric);
                        //cell.CellStyle = numericCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.CollectionTime);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateTimeCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.BentoCode);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.ReturnTime);
                        cell.SetCellType(CellType.Numeric);
                        cell.CellStyle = dateTimeCellStyle;

                        cell = row.CreateCell(col++);
                        cell.SetCellValue(dt.IsFAS ? "Y" : "N");
                        cell.CellStyle = dateTimeCellStyle;
                    });

                    #endregion

                    for (var i = 0; i < 30; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region CancelOrderRequest

        public async Task<PagedEntity<CancelOrderRequestDTO>> GetCancelOrderRequestsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<CancelOrderRequestDTO>>(await this._uow.CancelOrderRequests.GetCancelOrderRequestsAsync(filter));
            return result;
        }

        public async Task<CancelOrderRequestDTO> GetCancelOrderRequestByIdAsync(int id)
        {
            return _mapper.Map<CancelOrderRequestDTO>(await this._uow.CancelOrderRequests.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCancelOrderRequestAsync(CancelOrderRequestDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<CancelOrderRequest>(dto);
            result = await this._uow.CancelOrderRequests.CreateAsync(order);

            //var tokenOrder = await GetTokenOrderByIdAsync(dto.OrderId);
            //tokenOrder.CancelRequestStatus = "Submitted";
            //await UpdateTokenOrderAsync(tokenOrder);
            await this._uow.TokenOrders.UpdateOrderCancelRequestStatus(dto.OrderId, "Submitted");
            return result;
        }

        public async Task<BaseOperationResponse> UpdateCancelOrderRequestAsync(CancelOrderRequestDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<CancelOrderRequest>(dto);
            result = await this._uow.CancelOrderRequests.UpdateAsync(order);
            return result;
        }

        public async Task<BaseOperationResponse> DeleteCancelOrderRequestAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.CancelOrderRequests.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> ApprovalCancelOrderRequestAsync(CancelOrderRequestDTO dto)
        {
            var result = new BaseOperationResponse();
            var order = _mapper.Map<CancelOrderRequest>(dto);
            bool isApproved = order.Status == "Approved";
            result = await this._uow.CancelOrderRequests.UpdateAsync(order);
            result = await this._uow.TokenOrders.UpdateCancellationStatus(dto.OrderId, isApproved, dto.Response);

            if (isApproved && result.IsSuccess && result.Data != null)
            {
                var to = (TokenOrder)result.Data;
                var history = _mapper.Map<TokensOrderHistoryDTO>(result.Data);
                if (history != null)
                {
                    history.Status = "cancelled";
                    await CreateTokensOrderHistoryAsync(history, to);
                }
            }

            //if (order.Status == "Approved")
            //{
            //    //delete order
            //    await DeleteTokenOrderAsync(dto.OrderId);
            //}
            //else
            //{
            //    //remove status
            //    await this._uow.TokenOrders.UpdateCancellationStatus(dto.OrderId);
            //}

            return result;
        }

        #endregion
    }
}
