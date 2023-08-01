using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DeliveryBentoDTO
    {
        public int Id { get; set; }

        public int? DeliveryDetailId { get; set; }

        public int? BentoAssetId { get; set; }
        public string BentoAssetCode { get; set; }

        public int? DishId { get; set; }
        public string DishCode { get; set; }

        public string UserData { get; set; }

        public int IsActive { get; set; }
    }
}
