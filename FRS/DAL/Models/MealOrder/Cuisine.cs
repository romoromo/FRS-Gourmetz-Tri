using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class Cuisine : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public int? FileId { get; set; }

        [ForeignKey("FileId")]
        public virtual File Icon { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string LicenseCode { get; set; }

        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }
        public string KitchenLabel { get; set; }

    }
}
