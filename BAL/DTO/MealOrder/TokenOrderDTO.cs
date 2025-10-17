using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class TokenOrderDTO
    {

        public int Id { get; set; }

        public DateTime TransactionTime { get; set; }

        public int SchoolId { get; set; }

        public string OrderProfile { get; set; }

        public int? ProfileId { get; set; }

        public DateTime DeliveryDate { get; set; }

        public int? PeriodId { get; set; }

        public int? PaymentId { get; set; }

        public float TotalAmount { get; set; }

        public DateTime PaymentTime { get; set; }

        public float TotalPayment { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public int? StudentGroupId { get; set; }
        public string StudentGroupName { get; set; }
        public List<TokenOrderedDTO> Tokens { get; set; }

        public int? MealSessionDetailId { get; set; }
        public string MealSessionName { get; set; }
        public string MealSessionDetailName { get; set; }
        public string ClassName { get; set; }

        public int? MealPeriodId { get; set; }

        public DateTime MealSessionStartDate { get; set; }

        public int? StoreId { get; set; }

        public int? CancelledById { get; set; }
        public DateTime? CancelledOn { get; set; }
        public string CancellationReason { get; set; }
        public string CancelRequestStatus { get; set; }
        public string ProfileName { get; set; }
        public string PeriodName { get; set; }
        public string PaymentTypeName { get; set; }
        public int? CreatedBy { get; set; }
        public string ProcessedBy { get; set; }
        public string MealDescription { get; set; }
        public string MealOption { get; set; }
        public string PaymentStatus { get; set; }

        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string MealPeriodName { get; set; }
        public string IsFASDisplay { get; set; }
        public bool IsFAS { get; set; }
        public bool IsMealPlan { get; set; }
        public bool IsStudentGroupOrder { get; set; }
        public string PaymentNumber { get; set; }
        public string FomoId { get; set; }
        public string InvoiceNumber { get; set; }
        public string VoucherCode { get; set; }
        public decimal Discount { get; set; }
        public decimal PaymentSubtotal { get; set; }
        public decimal PaymentGst { get; set; }
        public decimal PaymentTransactionFee { get; set; }
        public decimal PaymentFixedTransactionFee { get; set; }
        public decimal PaymentTotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int Quantity { get; set; }
        public int OrderCount { get; set; }
        public DateTime? CollectionTime { get; set; }

        public string BentoCode { get; set; }

        public DateTime? ReturnTime { get; set; }
        public string AmendReason { get; set; }

        public List<TokensOrderHistoryDTO> TokensOrderHistorys { get; set; }
    }

    public class TokenOrderOrderingPortalDTO
    {

        public int Id { get; set; }

        public DateTime TransactionTime { get; set; }

        public int SchoolId { get; set; }

        public string OrderProfile { get; set; }

        public int? ProfileId { get; set; }

        public DateTime DeliveryDate { get; set; }

        public int? PeriodId { get; set; }

        public int? PaymentId { get; set; }

        public float TotalAmount { get; set; }

        public DateTime PaymentTime { get; set; }

        public float TotalPayment { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public int? StudentGroupId { get; set; }
        public string StudentGroupName { get; set; }
        public List<TokenOrderedDTO> Tokens { get; set; }

        public int? MealSessionDetailId { get; set; }
        public string MealSessionName { get; set; }
        public string MealSessionDetailName { get; set; }
        public string ClassName { get; set; }

        public int? MealPeriodId { get; set; }

        public DateTime MealSessionStartDate { get; set; }

        public int? StoreId { get; set; }

        public int? CancelledById { get; set; }
        public DateTime? CancelledOn { get; set; }
        public string CancellationReason { get; set; }
        public string CancelRequestStatus { get; set; }
        public string ProfileName { get; set; }
        public string PeriodName { get; set; }
        public string PaymentTypeName { get; set; }
        public int? CreatedBy { get; set; }
        public string ProcessedBy { get; set; }
        public string MealDescription { get; set; }
        public string MealOption { get; set; }
        public string PaymentStatus { get; set; }

        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string MealPeriodName { get; set; }
        public string IsFASDisplay { get; set; }
        public bool IsFAS { get; set; }
        public bool IsMealPlan { get; set; }
        public bool IsStudentGroupOrder { get; set; }
        public string PaymentNumber { get; set; }
        public string FomoId { get; set; }
        public string InvoiceNumber { get; set; }
        public string VoucherCode { get; set; }
        public decimal Discount { get; set; }
        public decimal PaymentSubtotal { get; set; }
        public decimal PaymentGst { get; set; }
        public decimal PaymentTransactionFee { get; set; }
        public decimal PaymentFixedTransactionFee { get; set; }
        public decimal PaymentTotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int Quantity { get; set; }
        public int OrderCount { get; set; }
        public DateTime? CollectionTime { get; set; }

        public string BentoCode { get; set; }

        public DateTime? ReturnTime { get; set; }
        public string AmendReason { get; set; }
    }

    public class TokenOrderedDTO
    {
        public int Id { get; set; }
        public int TokenId { get; set; }
        public string MealTypeId { get; set; }
        public string TokenDesc { get; set; }
        public string TokenName { get; set; }
        public int Qty { get; set; }
        public int OrderId { get; set; }

        public List<TokenOrderDishDTO> SelectedDishes { get; set; }

        public List<TokenAltDishDTO> TokenAltDishes { get; set; }

        public List<TokenOrderCombinedDishDTO> SelectedCombinedDishes { get; set; }

    }

    public class TokenOrderCombinedDishDTO
    {
        public int Id { get; set; }
        public int TokenOrderedId { get; set; }
        public string CDishLabel { get; set; }
        public string CDishCode { get; set; }
        public int Qty { get; set; }
        public int MenuQty { get; set; }
    }

    public class TokenOrderDishDTO
    {
        public int Id { get; set; }
        public int TokenOrderedId { get; set; }
        public int DishId { get; set; }
        public int Qty { get; set; }
        public string DishLabel { get; set; }
        public string DishTypeName { get; set; }
        public string FilePath { get; set; }
        public string ProductionPicturePath { get; set; }

    }

    public class TokenAltDishDTO
    {
        public int Id { get; set; }
        public int TokenOrderedId { get; set; }
        public int DishId { get; set; }
        public string DishLabel { get; set; }
        public string FilePath { get; set; }
        public string ProductionPicturePath { get; set; }
    }

    public class PackingAllocationDTO
    {
        public int Id { get; set; }
        public DateTime? PackingDate { get; set; }
        public int? RouteId { get; set; }
        public int? OutletId { get; set; }
        public string RouteLabel{ get; set; }
        public int? ToStoreInfoId { get; set; }
        public List<MealAllocationDTO> Allocations { get; set; }
        public List<DishAllocationDTO> Dishes { get; set; }

    }

    public class DishAllocationDTO
    {
        public int Id { get; set; }
        public int? PackingId { get; set; }
        public int? DishId { get; set; }
        public int? Qty { get; set; }
        public int? ScannedQty { get; set; }
        public DishDTO Dish { get; set; }
    }

    public class MealAllocationDTO
    {
        public int Id { get; set; }
        public DateTime? deliveryDate { get; set; }
        public int? periodId { get; set; }
        public int? mealSessionId { get; set; }
        public int? outletId { get; set; }
        public int? routeId { get; set; }
        public List<TokenLabelDTO> tokens { get; set; }
    }

    public class KioskOrderDishDTO : MealAllocationDTO
    {
        public string MealSessionName { get; set; }
    }

    public class DOReportDTO
    {
        public int Id { get; set; }
        public int dishID { get; set; }
        public string dishLabel { get; set; }
        public string tokenLabel { get; set; }
        public int? OrderNumber { get; set; }
        public int totalQty { get; set; }
        public List<DOReportRouteDTO> routes { get; set; }
        public List<DOReportSessionDTO> sessions { get; set; }
    }

    public class DOReportRouteDTO
    {
        public int routeId { get; set; }
        public string routeLabel { get; set; }
        public bool isFas { get; set; }
        public TimeSpan startTime { get; set; }
        public int qty { get; set; }
        public List<DOReportSessionDTO> sessions { get; set; }
    }

    public class DOReportSessionDTO
    {
        public int mealSessionDetailId { get; set; }
        public string name { get; set; }
        public bool isFas { get; set; }
        public string startTime { get; set; }
        public int qty { get; set; }

    }

    public class TokenLabelDTO
    {
        public int Id { get; set; }
        public int meal_allocation_id { get; set; }
        public int order_id { get; set; }
        public int token_id { get; set; }
        public string token_name { get; set; }
        public int qty { get; set; }
        public int qty_dishes { get; set; }
        public int qty_pdishes { get; set; }
        public int qty_tdishes { get; set; }
        public int qty_menus { get; set; }
        public string deliveryDate { get; set; }
        public DateTime? deliveryDate2 { get; set; }
        public DateTime? timePacked { get; set; }
        public string color { get; set; }
        public List<TokenDishLabelDTO> dishes { get; set; }
    }

    public class TokenDishLabelDTO
    {
        public int Id { get; set; }
        public int token_lable_id { get; set; }
        public int token_id { get; set; }
        public string token_name { get; set; }
        public int dish_id { get; set; }
        public string dish_name { get; set; }
        public string dish_code { get; set; }
        public int o_qty { get; set; }
        public int p_qty { get; set; }
        public int a_qty { get; set; }
        public int t_qty { get; set; }
    }

    public class SalesDataDTO
    {
        //public int Id { get; set; }
        public int? orderId { get; set; }
        public int? studentId { get; set; }
        public string studentName { get; set; }
        public string studentClass { get; set; }
        public int? DishId { get; set; }
        public string DishCode { get; set; }
        public string DishName { get; set; }
        public int? DishTypeID { get; set; }
        public string DishType { get; set; }
        public int? DishTypeOrder { get; set; }

        public string cardId { get; set; }
        public DateTime? collectionStart { get; set; }
        public DateTime? collectionEnd { get; set; }
        public int? sessionId { get; set; }
        public string sessionLabel { get; set; }
        public DateTime? orderDate { get; set; }
        public int? mealTypeId { get; set; }

    }

    public class MealSessionDetailByOrderAndClass
    {
        public int? OutletId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int MealSessionId { get; set; }
        public int ClassId { get; set; }
        public MealSessionDetailDTO MealSessionDetail { get; set; }
    }

    public class SummaryOrderMealTypeDTO
    {
        public int Id { get; set; }
        public int OutletId { get; set; }
        public DateTime? deliveryDate { get; set; }
        public List<SummaryOrderDetailDTO> details { get; set; }
    }

    public class SummaryOrderDetailDTO
    {
        public int Id { get; set; }
        public int SummaryOrderId { get; set; }
        public int MealTypeId { get; set; }
        public int qty_total { get; set; }
        public List<SummaryOrderSessionDTO> sdetails { get; set; }
    }

    public class SummaryOrderSessionDTO
    {
        public int Id { get; set; }
        public int SummaryOrderDetailId { get; set; }
        public int SessionId { get; set; }
        public int qty { get; set; }
    }

    public class CreateCancelOrderRequestDTO
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
        public string ImgUrl { get; set; }
        public string FileName { get; set; }
        public string UserName { get; set; }
        public int? StudentId { get; set; }
    }

    public class CancelOrderRequestDTO : CreateCancelOrderRequestDTO
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public DateTime CancellationDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public bool IsApproved { get; set; }
        public string MealSessionName { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public bool IsNotifCancellationRequestStatus { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string OrderNumber { get; set; }
        public List<TokenOrderedDTO> Tokens { get; set; }
    }

    public class CancelOrderRequestApproval
    {
        public int Id { get; set; }
        public bool IsApproved { get; set; }
        public string Response { get; set; }
    }

    public class SalesOrderCollectionSummary
    {
        public string MealSession { get; set; }
        public int TotalOrder { get; set; }
        public int TotalCollected { get; set; }
        public int TotalReturnable { get; set; }
        public int TotalReturnedBento { get; set; }
    }

    public class VoucherUtilisation
    {
        [Key]
        public int Id { get; set; }
        public int Total { get; set; }
        public string VoucherName { get; set; }
        public string VoucherCode { get; set; }
        public float VoucherAmount { get; set; }
        public string DiscountType { get; set; }
        public DateTime ValidityStartDate { get; set; }
        public DateTime ValidityEndDate { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public DateTime? UtilisedDate { get; set; }
        public string InvoiceNumber { get; set; }
        public float Discount { get; set; }
        public string VoucherStatus { get; set; }
    }
}
