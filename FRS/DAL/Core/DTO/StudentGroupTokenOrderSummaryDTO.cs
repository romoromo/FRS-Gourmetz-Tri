using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class MealPlanTokenOrderSummaryDTO
    {
        public MealPlanTokenOrderSummaryDTO()
        {
            this.Cols = new List<string>();
            this.Rows = new List<MealPlanTokenOrderRowDTO>();
        }
        public List<string> Cols { get; set; }
        public List<MealPlanTokenOrderRowDTO> Rows { get; set; }
        public MealPlanTokenOrderRowDTO Total { get; set; }
    }

    public class MealPlanTokenOrderRowDTO
    {
        public MealPlanTokenOrderRowDTO()
        {
            this.Cells = new List<string>();
        }
        public List<string> Cells { get; set; }
    }
}
