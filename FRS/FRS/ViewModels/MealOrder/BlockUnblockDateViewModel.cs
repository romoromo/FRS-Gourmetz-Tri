using BAL.DTO.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels.MealOrder
{
    public class BlockUnblockDateViewModel
    {
        public bool IsUnblock { get; set; }
        public List<MenuCycleBlockedDateDTO> BlockedDates { get; set; }
    }

    public class BlockUnblockOutletDateViewModel
    {
        public bool IsUnblock { get; set; }
        public List<OutletBlockedDateDTO> BlockedDates { get; set; }
    }
}
