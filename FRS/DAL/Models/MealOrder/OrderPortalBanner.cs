using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class OrderPortalBanner : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public int Order { get; set; }

        public int? ImageId { get; set; }

        public int? OutletId { get; set; }

        public int? OrderPortalContentId { get; set; }

        [ForeignKey("OrderPortalContentId")]
        public virtual OrderPortalContent OrderPortalContent { get; set; }

        [ForeignKey("ImageId")]
        public virtual File BannerImage { get; set; }
    }
}
