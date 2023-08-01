using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core
{
    public class OutletDishViewMenu
    {
        public int OutletId { get; set; }
        public List<OutletDishCyclePeriodMenu> ExcludedMenus { get; set; }
    }
}
