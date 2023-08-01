using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DeliveryDetailNewDTO
    {
        public int Id { get; set; }

        public int? DeliveryOrderId { get; set; }

        public int? CartonAssetId { get; set; }
        public string CartonAssetCode { get; set; }
        public string CartonType { get; set; }

        public int? TrackingStatusId { get; set; }
        public string Status { get; set; }

        //public int? DishId { get; set; }
        //public string DishCode { get; set; }

        //public int Qty { get; set; }

        public Boolean IsLoad { get; set; }
        public DateTime LoadingTime { get; set; }

        public List<DeliveryBentoNewDTO> DeliveryBentos { get; set; }

        public int IsActive { get; set; }
    }
}
