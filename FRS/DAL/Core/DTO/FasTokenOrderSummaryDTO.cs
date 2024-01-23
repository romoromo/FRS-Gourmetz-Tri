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
            this.Cols = new List<FasTokenOrderColDTO>();
            this.Rows = new List<FasTokenOrderRowDTO>();
            this.Headers = new List<FasTokenOrderRowDTO>();
        }
        public List<FasTokenOrderColDTO> Cols { get; set; }
        public List<FasTokenOrderRowDTO> Rows { get; set; }
        public List<FasTokenOrderRowDTO> Headers { get; set; }
        public FasTokenOrderRowDTO Total { get; set; }
    }

    public class FasTokenOrderColDTO
    {
        public FasTokenOrderColDTO()
        {
            this.Subheaders = new List<string>();
        }

        public string Header { get; set; }
        public int Rowspan { get; set; }
        public int Colspan { get; set; }
        public List<string> Subheaders { get; set; }
    }

    public class FasTokenOrderRowDTO
    {
        public FasTokenOrderRowDTO()
        {
            this.Cells = new List<FasTokenOrderCellDTO>();
        }
        public List<FasTokenOrderCellDTO> Cells { get; set; }
    }

    public class FasTokenOrderCellDTO
    {
        public string Val { get; set; }
        public int Rowspan { get; set; }
        public int Colspan { get; set; }
    }

    //public class FasTokenOrderSummaryByDateDTO
    //{
    //    public DateTime OrderDate { get; set; }
    //    public List<FasTokenOrderByDishTypeDTO> Summary { get; set; }
    //}

    //public class FasTokenOrderSummaryDTO
    //{
    //    public DateTime OrderDate { get; set; }
    //    public List<FasTokenOrderByDishTypeDTO> Summary { get; set; }
    //}
}
