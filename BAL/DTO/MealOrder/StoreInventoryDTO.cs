using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class StoreInventoryDTO
    {
        public int Id { get; set; }

        public int? StoreInfoId { get; set; }
        public int? DeliveryOrderID { get; set; }
        public int? DeliveryOrderNewID { get; set; }
        public string DeliveredBy { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime TimeReceived { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public StoreInfoDTO Store { get; set; }
        public DeliveryOrderDTO DeliveryOrder { get; set; }
        public DeliveryOrderNewDTO DeliveryOrderNew { get; set; }
        public List<StoreInventoryDetailDTO> StoreInventoryDetails { get; set; }
    }

    public class StoreInventoryDetailDTO
    {
        public int Id { get; set; }

        public int? StoreInventoryId { get; set; }

        public int? StoreInfoId { get; set; }

        public int? DishId { get; set; }

        public int? CartonId { get; set; }

        public int QtyExpected { get; set; }

        public int QtyReceived { get; set; }

        public int IsActive { get; set; }

        public DateTime TimeReceived { get; set; }
        public string Remarks { get; set; }

        public StoreInventoryDTO StoreInventory { get; set; }

        public DishDTO Dish { get; set; }

        public CartonAssetDTO Carton { get; set; }

        public StoreInfoDTO Store { get; set; }
    }


}
