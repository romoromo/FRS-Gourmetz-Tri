using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MealTypeDishDTO
    {
        public int DishId { get; set; }

        public string DishName { get; set; }

        public int MealTypeId { get; set; }

        public string MealTypeName { get; set; }
    }
}
