using DAL.Core.Audit.Attributes;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class RewardTransaction : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int RewardId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public double Amount { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string TransactionType { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }

        [ForeignKey("RewardId")]
        public virtual Reward Reward { get; set; }
    }
}
