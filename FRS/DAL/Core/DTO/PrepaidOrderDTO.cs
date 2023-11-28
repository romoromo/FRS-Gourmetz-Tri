using System;
using System.Collections.Generic;

namespace DAL.Core.DTO
{
    public class PrepaidOrderDTO
    {
        public int ProfileId { get; set; }
        public DateTime DeliveryDate { get; set; }
        //public int? PeriodId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public int Quantity { get; set; }
        public int TotalAmount { get; set; }
        public string Remarks { get; set; }
        public int MealSessionDetailId { get; set; }
        public int? CreatedBy { get; set; }
        public int? StoreId { get; set; }
        public int OutletId { get; set; }
        public List<PrepaidOrderTokenOrderedDTO> Tokens { get; set; }
    }

    public class PrepaidOrderTokenOrderedDTO
    {
        public int? TokenId { get; set; }
        public int? MeaTypeId { get; set; }
        public string TokenDesc { get; set; }
        public int Qty { get; set; }
        public int DishId { get; set; }
    }
}
