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
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Url { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Title { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Subtitle { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int Order { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? ImageId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? OutletId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? OrderPortalContentId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string FileName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string FilePath { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string ImageFileName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string ImageFilePath { get; set; }

        [ForeignKey("OrderPortalContentId")]
        public virtual OrderPortalContent OrderPortalContent { get; set; }

        [ForeignKey("ImageId")]
        public virtual File BannerImage { get; set; }
    }
}
