using DAL.Models.StoredProcedures;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class spGetUserActivityLogDTO
    {
        public spGetUserActivityLogDTO()
        {
            Headers = new List<spGetUserActivityLogHeader>();
            Details = new List<spGetUserActivityLogDetail>();
        }

        public List<spGetUserActivityLogHeader> Headers { get; set; }
        public List<spGetUserActivityLogDetail> Details { get; set; }
    }
}
