using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class AssetModelDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string Notes { get; set; }

        public int AssetTypeId { get; set; }

        public string AssetTypeName { get; set; }

        public int? FileId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }
    }
}
