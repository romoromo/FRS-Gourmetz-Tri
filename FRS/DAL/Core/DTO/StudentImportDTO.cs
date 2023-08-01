using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class StudentImportDTO
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string Batch { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public bool IsFAS { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
    }

    public class StudentCardImportDTO
    {
        public int OutletId { get; set; }
        public int Batch { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public DateTime? IssueDate { get; set; }
        public string CardId { get; set; }
        public string CardNumber { get; set; }
        public string Email { get; set; }
    }
}
