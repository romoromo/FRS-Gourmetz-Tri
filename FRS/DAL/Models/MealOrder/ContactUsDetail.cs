using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class ContactUsDetail : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Label { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? InstitutionId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int ContactUsSubjectId { get; set; }

        [ForeignKey("ContactUsSubjectId")]
        public virtual ContactUsSubject ContactUsSubject { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

    }
}
