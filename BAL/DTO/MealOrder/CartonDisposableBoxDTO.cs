using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class CartonDisposableBoxDTO
    {
        public int Id { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public int? CartonAssetId { get; set; }

        public string CartonAssetCode { get; set; }

        public int? DishId { get; set; }
        public string DishCode { get; set; }
        public string DishLabel { get; set; }
        public int Qty { get; set; }

        public int? StoreInfoId { get; set; }
        public string StoreInfoCode { get; set; }

        public int? ToStoreInfoId { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? TimeStamp { get; set; }

        public bool reset { get; set; }
    }
}
