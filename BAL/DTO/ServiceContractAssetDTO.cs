using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class ServiceContractAssetDTO
    {
        public int Id { get; set; }
        public int ServiceContractId { get; set; }
        public int AssetId { get; set; }

        public string SerialNumber { get; set; }
        public int AssetModelId { get; set; }
        public string AssetModelName { get; set; }
        public string AssetTypeName { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyStart { get; set; }
        public DateTime? WarrantyEnd { get; set; }
    }
}
