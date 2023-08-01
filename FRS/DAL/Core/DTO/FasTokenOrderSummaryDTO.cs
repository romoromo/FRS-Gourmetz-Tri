using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class FasTokenOrderSummaryDTO
    {
        public FasTokenOrderSummaryDTO()
        {
            this.Cols = new List<string>();
            this.Rows = new List<FasTokenOrderRowDTO>();
        }
        public List<string> Cols { get; set; }
        public List<FasTokenOrderRowDTO> Rows { get; set; }
        public FasTokenOrderRowDTO Total { get; set; }
    }

    public class FasTokenOrderRowDTO
    {
        public FasTokenOrderRowDTO()
        {
            this.Cells = new List<string>();
        }
        public List<string> Cells { get; set; }
    }
}
