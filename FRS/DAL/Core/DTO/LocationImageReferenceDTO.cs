using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class LocationImageReferenceDTO
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public int FileId { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string Remarks { get; set; }
        public int? ImageReferenceTypeId { get; set; }
        public int? ImageReferenceColorId { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string ImageReferenceColorCode { get; set; }
    }
}
