using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class LocationTreeImportDTO
    {
        public string Institution { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }
        public string Capacity { get; set; }
        public bool IsBooking { get; set; }
        public string FacePlateNumber { get; set; }
        public List<string> AssignedInstitutions { get; set; }
        public List<string> Facilities { get; set; }
    }
}
