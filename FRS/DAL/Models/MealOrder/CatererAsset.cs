using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.MealOrder
{
    public class CatererAsset : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string assetQRCode { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }


        public int? CatererAssetTypeId { get; set; }

        [ForeignKey("CatererAssetTypeId")]
        public virtual CatererAssetType CatererAssetType { get; set; }

        public int? FileId { get; set; }
        [ForeignKey("FileId")]
        public virtual File Icon { get; set; }
    }
}
