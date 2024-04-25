using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MenuGroupSimpleDTO
    {
        public int Id { get; set; }
        public int OutletId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string OutletName { get; set; }
        public bool IsPublished { get; set; }
        public List<int> DishCycleIds { get; set; }
        public List<int> ClassIds { get; set; }
    }

    public class MenuGroupDTO: MenuGroupSimpleDTO
    {
        public List<MenuGroupDishCycleDTO> MenuGroupDishCycles { get; set; }
        public List<MenuGroupClassDTO> Classes { get; set; }
    }
}
