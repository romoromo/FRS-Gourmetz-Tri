using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using DAL.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;
using DAL.Core.Audit.Attributes;

namespace DAL.Models
{
    public class AuditableEntity : IAuditableEntity
    {
        [Sieve(CanFilter = true)]
        public bool IsActive { get; set; }

        [Sieve(CanFilter = true)]
        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser CreatedByUser { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual ApplicationUser UpdatedByUser { get; set; }

        [Sieve(CanFilter = true)]
        public int? UpdatedBy { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        [SkipTracking]
        public DateTime UpdatedDate { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedDate { get; set; }
    }
}
