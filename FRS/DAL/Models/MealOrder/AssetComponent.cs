using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.MealOrder
{
    public class AssetComponent : AuditableEntity
    {
        [Key]
        [Sieve(CanSort = true)]
        public int Id { get; set; }

        public int? CatererAssetId { get; set; }
        public virtual CatererAsset CatererAsset { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }
        public int Qty { get; set; }
        public string Remarks { get; set; }

        public int? FileId { get; set; }
        [ForeignKey("FileId")]
        public virtual File Icon { get; set; }
    }
}