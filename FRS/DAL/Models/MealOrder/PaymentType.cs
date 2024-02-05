using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class PaymentType : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public int? InstitutionId { get; set; }

        public bool IsSystem { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public virtual ICollection<TransactionFee> TransactionFees { get; set; }
    }
}
