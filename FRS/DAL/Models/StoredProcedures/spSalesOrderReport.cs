using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace DAL.Models.StoredProcedures
{
    [NotMapped]
    public class spSalesOrderReport
    {
        [Key]
        public int Id { get; set; }
        public int Total { get; set; }
        public int? PaymentId { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentNumber { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentGroupName { get; set; }
        public string Outlet { get; set; }
        public string ClassName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string DiscountCode { get; set; }
        public float Subtotal { get; set; }
        public float SubDiscTotal { get; set; }
        public float Discount { get; set; }
        public float PaymentGst { get; set; }
        public float PaymentTransactionFee { get; set; }
        public float PaymentFixedTransactionFee { get; set; }
        public float PaymentTotalAmount { get; set; }
        public string FomoId { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string MealSessionDetailName { get; set; }
        public string MealTypeName { get; set; }
        public string DishLabel { get; set; }
        public int Quantity { get; set; }
        public float DishPrice { get; set; }
        public string CollectionTime { get; set; }
        public string BentoCode { get; set; }
        public string ReturnTime { get; set; }
        public bool IsFAS { get; set; }
        public string CancellationReason { get; set; }
        public string CancelledOn { get; set; }
        public string CancelRequestStatus { get; set; }
        public int OrderCount { get; set; }
        //public string ProcessedBy { get; set; }
    }
}
