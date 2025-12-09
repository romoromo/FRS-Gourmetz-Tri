using System.ComponentModel.DataAnnotations;

namespace FRS.ViewModels
{
    public class UpdateDailyLimitViewModel
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int DailyLimit { get; set; }
    }

    public class UpdateIsWalletFreezeViewModel
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public bool IsWalletFreeze { get; set; }
    }
}
