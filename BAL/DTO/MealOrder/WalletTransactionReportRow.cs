namespace BAL.DTO.MealOrder
{
    public class WalletTransactionReportRow
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";

        public int OutletId { get; set; }
        public string OutletName { get; set; } = ""; 

        public int ClassLevelId { get; set; }
        public string ClassLevelName { get; set; } = ""; 

        public int ClassId { get; set; }
        public string ClassName { get; set; } = ""; 

        public double TotalTopUpNormalAccount { get; set; }
        public double TotalRedemption { get; set; }
        public double NormalWalletBalance { get; set; }

        public double Difference => TotalTopUpNormalAccount - NormalWalletBalance;
    }
}
