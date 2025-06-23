using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class UserActivity
    {
        [Key]
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = false)]
        public string Message { get; set; }
        [Sieve(CanFilter = true)]
        public int? CreatedBy { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? CreatedDate { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser CreatedByUser { get; set; }
    }
}
