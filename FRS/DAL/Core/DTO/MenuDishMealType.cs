using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class MenuDishMealType
    {
        public List<MenuListCol> Columns { get; set; }
        public List<MenuListRow> Rows { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }

    public class MenuListCol
    {
        public int DishId { get; set; }
        public int MealTypeId { get; set; }
        public string ColName { get; set; }
    }

    public class MenuListRow
    {
        public int DishId { get; set; }
        public int MealTypeId { get; set; }
        public string DishLabel { get; set; }
        public List<MenuListCell> Cells { get; set; }
    }

    public class MenuListCell
    {
        public int DishId { get; set; }
        public int MealTypeId { get; set; }
        public bool Checked { get; set; }
    }
}
