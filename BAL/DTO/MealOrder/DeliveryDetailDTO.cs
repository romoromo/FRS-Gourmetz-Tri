using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DeliveryDetailDTO
    {
        public int Id { get; set; }

        public int? DeliveryOrderId { get; set; }

        public int? CartonAssetId { get; set; }
        public string CartonAssetCode { get; set; }

        public int? TrackingStatusId { get; set; }
        public string Status { get; set; }

        public List<DeliveryBentoDTO> DeliveryBentos { get; set; }

        public int IsActive { get; set; }
    }
}
