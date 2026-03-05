using System;

namespace DAL.Core.DTO
{
    public class DetailedBasicWalletTopUpReport
    {
        public int ID { get; set; }
        public DateTime TransactionDate { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Outlet { get; set; }
        public string ClassLevel { get; set; }
        public double Amount { get; set; }
        public string Source { get; set; }
        public string RefId { get; set; }
        public string ProcessedBy { get; set; }
    }
}
