using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class ServiceContract : AuditableEntity
    {
        public ServiceContract()
        {
            this.ServiceContractAssets = new HashSet<ServiceContractAsset>();
        }

        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Identifier { get; set; }
        public string Reference { get; set; }
        public string Coverage { get; set; }
        public string Details { get; set; }
        public int? RenewalAlert { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual ICollection<ServiceContractAsset> ServiceContractAssets { get; set; }
    }
}
