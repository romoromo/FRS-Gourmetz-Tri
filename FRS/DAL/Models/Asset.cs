using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Asset : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyStart { get; set; }
        public DateTime? WarrantyEnd { get; set; }

        public int AssetModelId { get; set; }
        [ForeignKey("AssetModelId")]
        public virtual AssetModel AssetModel { get; set; }

        public int? LocationId { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? InstitutionId { get; set; }
        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public string poNumber { get; set; }
    }
}
