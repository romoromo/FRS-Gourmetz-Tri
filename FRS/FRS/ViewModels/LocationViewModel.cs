using DAL.Core.DTO;
using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class LocationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Location name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Location name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string LocationTypeName { get; set; }

        public string InstitutionNames { get; set; }

        public string FacilityNames { get; set; }
        public string FacilityTypeNames { get; set; }
        public string Status { get; set; }

        public bool IsBooking { get; set; }

        public string FacePlateNumber { get; set; }
        public List<FacilityViewModel> Facilities { get; set; }
        public List<FacilityTypeViewModel> FacilityTypes { get; set; }

        public List<int> FacilityIds { get; set; }
        public List<int> FacilityTypeIds { get; set; }
        public List<int> InstitutionIds { get; set; }

        //[Required(ErrorMessage = "Capacity is required")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int Capacity { get; set; }

        public int LocationTypeId { get; set; }
        public int? ParentLocationId { get; set; }
        public LocationViewModel ParentLocation { get; set; }

        public int? DirectoryId { get; set; }
        public DirectoryListingViewModel MappedDirectory { get; set; }

        public string DirectoryName { get; set; }

        public string FilePath { get; set; }
        public int InstitutionId { get; set; }

        public string ColorTheme { get; set; }

        public string LocationGroup { get; set; }

        public int? ScheduleId { get; set; }
        public int? LocationId { get; set; }

        public int IsActive { get; set; }

        public List<LocationImageReferenceDTO> ImageReferences { get; set; }
        public List<LocationAssetDTO> LocationAssets { get; set; }

        public int FloorId { get; set; }

        public string ExchangeId { get; set; }

        public string Extension { get; set; }
        public string IP { get; set; }
        public string deviceName { get; set; }
        public string category { get; set; }
        public string remarks { get; set; }
        public int floorX { get; set; }
        public int floorY { get; set; }
        public int seatingCapacity { get; set; }
    }
}
