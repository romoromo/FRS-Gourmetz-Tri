using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MenuDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public string Label { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? InstitutionId { get; set; }
        public int? CatererId { get; set; }
        public string InstitutioName { get; set; }
        public int? CuisineId { get; set; }
        public string CuisineName { get; set; }
        public string MenuDishNames { get { return MenuDishes != null ? string.Join(",", MenuDishes.Select(e => e.DishName)) : string.Empty; } }

        public List<MenuDishDTO> MenuDishes { get; set; }
        public List<OutletMenuDishDTO> OutletMenuDishes { get; set; }
    }
}
