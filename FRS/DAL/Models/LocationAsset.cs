using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class LocationAsset : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        //public string SerialNumber { get; set; }
        //public DateTime? PurchaseDate { get; set; }
        //public DateTime? WarrantyStart { get; set; }
        //public DateTime? WarrantyEnd { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        //public int AssetModelId { get; set; }

        //[ForeignKey("AssetModelId")]
        //public virtual AssetModel AssetModel { get; set; }

        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset Asset { get; set; }
    }
}
