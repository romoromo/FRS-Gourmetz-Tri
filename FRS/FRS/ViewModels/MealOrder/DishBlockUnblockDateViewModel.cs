using BAL.DTO.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels.MealOrder
{
    public class DishBlockUnblockDateViewModel
    {
        public bool IsUnblock { get; set; }
        public List<DishCycleBlockedDateDTO> BlockedDates { get; set; }
    }

    public class BlockUnblockOutletDishDateViewModel
    {
        public bool IsUnblock { get; set; }
        public List<OutletDishBlockedDateDTO> BlockedDates { get; set; }
    }
}
