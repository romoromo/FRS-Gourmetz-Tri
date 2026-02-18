using System;

namespace DAL.Core.DTO
{
    public class FASMonthlyBillingReportDTO
    {
        public int ID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public int? OutletID { get; set; }
        public string OutletName { get; set; }
        public int ClasslevelID { get; set; }
        public string ClassLevel { get; set; }
        public int ClassID { get; set; }
        public string Class { get; set; }
        public bool FASStudent { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string MealType { get; set; }
        public string MealName { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public string InvoiceNumber { get; set; }
        public string POSInvoiceNumber { get; set; }
        public double Amount { get; set; }
    }
}
