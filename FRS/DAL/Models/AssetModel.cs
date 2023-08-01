using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class AssetModel : AuditableEntity
    {
        public AssetModel()
        {
        }

        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Model { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Notes { get; set; }

        public int? InstitutionId { get; set; }

        public virtual Institution Institution { get; set; }

        public int? AssetTypeId { get; set; }

        [ForeignKey("AssetTypeId")]
        public virtual AssetType AssetType { get; set; }

        public int? FileId { get; set; }

        [ForeignKey("FileId")]
        public virtual File Photo { get; set; }

    }
}
