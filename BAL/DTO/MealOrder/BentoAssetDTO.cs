using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class BentoAssetDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public int? BentoBoxTypeId { get; set; }

        public string BentoBoxTypeCode { get; set; }

        public int? CartonAssetId { get; set; }

        public string CartonAssetCode { get; set; }

        public int? DishId { get; set; }
        public string DishCode { get; set; }
        public string DishLabel { get; set; }

        public int? StoreInfoId { get; set; }
        public string StoreInfoCode { get; set; }

        public bool isActive { get; set; }

        public int? ToStoreInfoId { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? TimeStamp { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime? LastPackingTime { get; set; }

        public DateTime? AssetRegistrationTime { get; set; }

        public DateTime? LastReturnTime { get; set; }

        public string Remarks { get; set; }

        public int? RouteId { get; set; }
        public string RouteLabel { get; set; }
    }
}
