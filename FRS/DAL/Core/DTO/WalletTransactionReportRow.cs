namespace DAL.Core.DTO
{
    public class WalletTransactionReportRow
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public string OutletName { get; set; }
        public string ClassLevelName { get; set; }
        public string ClassName { get; set; }

        public bool IsFAS { get; set; }

        // BASIC
        public double TotalTopUpBasic { get; set; }
        public double TotalRefundBasic { get; set; }
        public double TotalBasicRedemption
        {
            get; set;
            //get
            //{
            //    return (TotalTopUpBasic + TotalRefundBasic) - BasicWalletBalance;
            //}
        }
        public double BasicWalletBalance { get; set; }

        // FAS
        public double TotalTopUpFAS { get; set; }
        public double TotalRefundFAS { get; set; }
        public double TotalFASRedemption { get; set; }
        public double FASWalletBalance { get; set; }
        public double TotalAutoDebitFAS { get; set; }

        public double WrongTopupFASCredit { get; set; }
        public string StudentStatus { get; set; }
    }
}
