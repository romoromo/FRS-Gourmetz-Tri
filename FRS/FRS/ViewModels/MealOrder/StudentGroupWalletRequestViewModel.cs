using DAL.Core;

namespace FRS.ViewModels.MealOrder
{
    public class StudentGroupWalletRequestViewModel : BaseWalletRequestViewModel
    {
        public WalletType Type { get; set; }
    }

    public class StudentGroupPointRequestViewModel : BaseWalletRequestViewModel
    {
        public PointType Type { get; set; }
    }

    public class BaseWalletRequestViewModel
    {
        public int StudentGroupId { get; set; }
        public double Amount { get; set; }
        public int UserId { get; set; }
    }
}
