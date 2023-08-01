using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class CartonType : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Code { get; set; }

        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Details { get; set; }

        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        public int? CatererInfoId { get; set; }

        [ForeignKey("CatererInfoId")]
        public virtual CatererInfo CatererInfo { get; set; }

        public virtual ICollection<CartonAsset> CartonAssets { get; set; }
    }
}
