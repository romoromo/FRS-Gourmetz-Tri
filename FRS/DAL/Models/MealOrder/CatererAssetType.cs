using Sieve.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.MealOrder
{
    public class CatererAssetType: AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Code { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }

        public int? FileId { get; set; }
        [ForeignKey("FileId")]
        public virtual File Icon { get; set; }

        public int? CatererInfoId { get; set; }
        [ForeignKey("CatererInfoId")]
        public virtual CatererInfo CatererInfo { get; set; }

        public virtual ICollection<CartonAsset> CartonAssets { get; set; }
    }
}
