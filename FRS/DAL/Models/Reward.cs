using DAL.Core.Audit.Attributes;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Reward : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public double Balance { get; set; }

        [SkipTracking]
        public byte[] ConcurrencyStamp { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
