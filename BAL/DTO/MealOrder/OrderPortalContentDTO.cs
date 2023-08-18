using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OrderPortalContentDTO
    {
        public OrderPortalContentDTO()
        {
            Banners = new List<OrderPortalBannerDTO>();
        }

        public int Id { get; set; }

        public string Announcement { get; set; }

        public string Description { get; set; }

        public DateTime EffectiveStartDate { get; set; }

        public DateTime EffectiveEndDate { get; set; }

        public int? OutletId { get; set; }

        public List<OrderPortalBannerDTO> Banners { get; set; }
    }
}
