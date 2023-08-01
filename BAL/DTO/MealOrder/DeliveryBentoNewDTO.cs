using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DeliveryBentoNewDTO
    {
        public int Id { get; set; }

        public int? DeliveryDetailId { get; set; }

        public int? BentoAssetId { get; set; }
        public string BentoAssetCode { get; set; }

        public string BentoType { get; set; }

        public int? DishId { get; set; }
        public string DishCode { get; set; }
        public string DishLabel { get; set; }
        public string DishType { get; set; }

        public int Qty { get; set; }

        public int ReceivedQty { get; set; }

        public string UserData { get; set; }

        public int IsActive { get; set; }
    }
}
