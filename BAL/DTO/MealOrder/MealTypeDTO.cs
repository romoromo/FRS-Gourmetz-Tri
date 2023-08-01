using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MealTypeDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string ChargeType { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public int? CuisineId { get; set; }

        public string CuisineName { get; set; }

        public string DishNames { get; set; }

        public int? CatererId { get; set; }

        public bool IsSubMenu { get; set; }

        public List<MealTypeDishDTO> Dishes { get; set; }
    }
}
