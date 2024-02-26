using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class FaqDetail : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Label { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? InstitutionId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int FaqSubjectId { get; set; }

        [ForeignKey("FaqSubjectId")]
        public virtual FaqSubject FaqSubject { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

    }
}
