using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DAL.Models
{
    public class Location : AuditableEntity
    {
        public Location()
        {
            this.LocationFacilities = new HashSet<LocationFacility>();
            this.LocationImageReferences = new HashSet<LocationImageReference>();
            this.LocationInstitutions = new HashSet<LocationInstitution>();
            this.Reservations = new HashSet<Reservation>();
            this.ChildLocations = new HashSet<Location>();
            this.LocationFacilityTypes = new HashSet<LocationFacilityType>();
            this.LocationAssets = new HashSet<LocationAsset>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int Capacity { get; set; }
        public int? LocationTypeId { get; set; }
        public int? ParentLocationId { get; set; }
        public int? InstitutionId { get; set; }

        public string ColorTheme { get; set; }

        public string LocationGroup { get; set; }

        public bool IsBooking { get; set; }

        public string FacePlateNumber { get; set; }

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
        public string Access { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public int? FileId { get; set; }

        [ForeignKey("FileId")]
        public virtual File Icon { get; set; }

        [ForeignKey("ParentLocationId")]
        public virtual Location ParentLocation { get; set; }
        public virtual LocationType LocationType { get; set; }

        public int? SmartRoomResourceId { get; set; }

        [ForeignKey("SmartRoomResourceId")]
        public virtual SmartRoomResource SmartRoomResource { get; set; }

        public int? DirectoryId { get; set; }

        [ForeignKey("DirectoryId")]
        public virtual DirectoryListing MappedDirectory { get; set; }

        public virtual ICollection<LocationFacilityType> LocationFacilityTypes { get; set; }
        public virtual ICollection<LocationFacility> LocationFacilities { get; set; }
        public virtual ICollection<LocationImageReference> LocationImageReferences { get; set; }
        public virtual ICollection<LocationInstitution> LocationInstitutions { get; set; }
        public virtual ICollection<Location> ChildLocations { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<LocationAsset> LocationAssets { get; set; }
        public virtual ICollection<UserGroupLocation> UserGroupLocations { get; set; }

        public IEnumerable<Location> GetLocationAndDescendants()
        {
            return new[] { this }.Concat(ChildLocations.SelectMany(child => child.GetLocationAndDescendants()));
        }
    }
}
