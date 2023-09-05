using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DeliveryOrderNewDTO
    {
        public int Id { get; set; }

        public string DONumber { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public string DeliveryAddress { get; set; }

        public int? CatererInfoId { get; set; }

        public string CatererInfoName { get; set; }

        public int? FromStoreId { get; set; }

        public string FromStoreName { get; set; }

        public string FromStoreAddress { get; set; }

        public int? ToStoreId { get; set; }

        public string ToStoreName { get; set; }

        public string ToStoreAddress { get; set; }

        public bool Completed { get; set; }

        public List<DeliveryDetailNewDTO> DeliveryDetails { get; set; }

        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime ClosedDate { get; set; }

        public DateTime FoodExpiryDate { get; set; }
        public int? CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        public int? UpdatedBy { get; set; }

        public int? ClosedBy { get; set; }

        public string ClosedByName { get; set; }

        public int? MealSessionDetailId { get; set; }

        public string MealSessionName { get; set; }

        public int? RouteId { get; set; }

        public string RouteName { get; set; }

        public DateTime? PickupTime { get; set; }

        public string VehicleNumber { get; set; }

        public DateTime LoadingTime { get; set; }
        public DateTime ReceivingTime { get; set; }
    }

    public class DoPrintDTO
    {
        public int Id { get; set; }
        public int dishID { get; set; }
        public string dishCode { get; set; }
        public string dishLabel { get; set; }
        public string categories { get; set; }
        public int dishQty { get; set; }
        public int issQty { get; set; }
        public string bentoList { get; set; }
    }
}
