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
        public string Qty { get; set; }
        public int QtyNumber
        {
            get
            {
                return int.TryParse(Qty.Replace(".00", "").Replace(",00", ""), out var p) ? p : 0;
            }
        }
        public string Price { get; set; }
        public string InvoiceNumber { get; set; }
        public string POSInvoiceNumber { get; set; }
        public double Amount { 
            get
            {
                if (string.IsNullOrWhiteSpace(Price)) return 0;
                string normalizedPrice = Price.Replace(",", ".").Trim();
                int lastDotIndex = normalizedPrice.LastIndexOf('.');
                if (lastDotIndex != -1)
                {
                    // Remove all dots except the last one (treating others as thousand separators)
                    string beforeDot = normalizedPrice.Substring(0, lastDotIndex).Replace(".", "");
                    string afterDot = normalizedPrice.Substring(lastDotIndex);
                    normalizedPrice = beforeDot + afterDot;
                }

                if (double.TryParse(normalizedPrice, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var p))
                {
                    return QtyNumber * p;
                }
                return 0;
            }
        }
    }
}
