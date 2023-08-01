using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class AssetType : AuditableEntity
    {
        public AssetType()
        {
        }

        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public int? InstitutionId { get; set; }

        public virtual Institution Institution { get; set; }

        public int? FileId { get; set; }

        [ForeignKey("FileId")]
        public virtual File Icon { get; set; }
    }
}
