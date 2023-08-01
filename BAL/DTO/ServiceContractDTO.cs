using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class ServiceContractDTO
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Reference { get; set; }
        public string Coverage { get; set; }
        public string Details { get; set; }
        public int? RenewalAlert { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<ServiceContractAssetDTO> ServiceContractAssets { get; set; }
    }
}
