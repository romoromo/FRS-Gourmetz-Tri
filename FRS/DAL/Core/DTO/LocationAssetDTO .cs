using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class LocationAssetDTO
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public int AssetId { get; set; }
        public int FileId { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string SerialNumber { get; set; }
        public int AssetModelId { get; set; }
        public string AssetModelName { get; set; }
        public string AssetTypeName { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyStart { get; set; }
        public DateTime? WarrantyEnd { get; set; }
    }
}
