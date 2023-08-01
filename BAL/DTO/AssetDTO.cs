using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class AssetDTO
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyStart { get; set; }
        public DateTime? WarrantyEnd { get; set; }
        public int AssetModelId { get; set; }
        public string AssetModelName { get; set; }
        public string AssetTypeName { get; set; }

        public int? LocationId { get; set; }
        public int? InstitutionId { get; set; }
        public string LocationName { get; set; }
        public string InstitutionName { get; set; }

        public string poNumber { get; set; }
    }
}
