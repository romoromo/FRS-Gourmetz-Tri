using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class ServiceContractAsset : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ServiceContract")]
        public int ServiceContractId { get; set; }
        public virtual ServiceContract ServiceContract { get; set; }

        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset Asset { get; set; }
    }
}
