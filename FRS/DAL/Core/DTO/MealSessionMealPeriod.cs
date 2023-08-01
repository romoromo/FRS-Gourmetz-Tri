using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class MealSessionMealPeriod
    {
        public MealPeriod MealPeriod { get; set; }
        public List<MealSession> MealSessions { get; set; }
    }
}
