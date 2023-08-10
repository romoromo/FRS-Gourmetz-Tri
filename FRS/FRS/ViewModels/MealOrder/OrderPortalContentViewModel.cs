using FRS.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels.MealOrder
{
    public class OrderPortalContentViewModel
    {
        public OrderPortalContentViewModel()
        {
            Banners = new List<OrderPortalBannerViewModel>();
        }

        public int Id { get; set; }

        public string Announcement { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int? OutletId { get; set; }

        public List<OrderPortalBannerViewModel> Banners { get; set; }
    }

    public class OrderPortalBannerViewModel: Sanitizeable
    {
        public int Id { get; set; }

        [Sanitize]
        public string Url { get; set; }

        [Sanitize]
        public string Title { get; set; }

        [Sanitize]
        public string Subtitle { get; set; }

        public int Order { get; set; }

        public int? ImageId { get; set; }

        public int? OrderPortalContentId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }
    }
}
