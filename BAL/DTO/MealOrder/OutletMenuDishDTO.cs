using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletMenuDishDTO
    {
        public int DishId { get; set; }

        public string DishName { get; set; }

        public int MenuId { get; set; }

        public string MenuName { get; set; }

        public int? MealTypeId { get; set; }

        public string MealTypeName { get; set; }

        public int? OutletId { get; set; }

        public string OutletName { get; set; }
    }
}
