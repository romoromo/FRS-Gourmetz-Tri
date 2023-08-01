using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class LocationTimeSlotViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string LocationTypeName { get; set; }

        public string InstitutionNames { get; set; }

        public string FacilityNames { get; set; }

        public string Status { get; set; }

        public List<FacilityViewModel> Facilities { get; set; }

        public List<int> FacilityIds { get; set; }
        public List<int> InstitutionIds { get; set; }
        public int Capacity { get; set; }

        public int LocationTypeId { get; set; }
        public int? ParentLocationId { get; set; }
        public LocationViewModel ParentLocation { get; set; }
        public string FilePath { get; set; }

        public string AvailableFrom { get; set; }
        public string AvailableTo { get; set; }
        public string AvailableTimeDisplay { get; set; }

        public List<ReservationViewModel> Reservations { get; set; }
    }
}
