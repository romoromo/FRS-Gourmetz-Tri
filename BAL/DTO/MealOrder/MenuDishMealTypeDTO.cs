using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MenuDishMealTypeDTO
    {
        public List<MenuListColDTO> Columns { get; set; }
        public List<MenuListRowDTO> Rows { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }

    public class MenuListColDTO
    {
        public int DishId { get; set; }
        public int MealTypeId { get; set; }
        public string ColName { get; set; }
    }

    public class MenuListRowDTO
    {
        public int DishId { get; set; }
        public int MealTypeId { get; set; }
        public string DishLabel { get; set; }
        public List<MenuListCellDTO> Cells { get; set; }
    }

    public class MenuListCellDTO
    {
        public int DishId { get; set; }
        public int MealTypeId { get; set; }
        public bool Checked { get; set; }
    }
}
