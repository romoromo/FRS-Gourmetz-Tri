using DAL.Core.Audit.Attributes;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class UserActivity
    {
        [SkipTracking]
        [Key]
        public int Id { get; set; }
        [SkipTracking]
        [Sieve(CanFilter = true, CanSort = false)]
        public string Message { get; set; }
        [SkipTracking]
        [Sieve(CanFilter = true)]
        public int? CreatedBy { get; set; }
        [SkipTracking]
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? CreatedDate { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser CreatedByUser { get; set; }
    }
}
