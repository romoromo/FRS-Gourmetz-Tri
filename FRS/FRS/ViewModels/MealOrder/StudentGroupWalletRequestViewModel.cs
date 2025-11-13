using DAL.Core;

namespace FRS.ViewModels.MealOrder
{
    public class StudentGroupWalletRequestViewModel
    {
        public int StudentGroupId { get; set; }
        public double Amount { get; set; }
        public int UserId { get; set; }
        public WalletType Type { get; set; }
    }
}
