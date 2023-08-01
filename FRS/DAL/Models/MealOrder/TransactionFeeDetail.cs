using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class TransactionFeeDetail : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public double Amount { get; set; }

        public int TransactionFeeId { get; set; }

        [ForeignKey("TransactionFeeId")]
        public virtual TransactionFee TransactionFee { get; set; }
    }
}
