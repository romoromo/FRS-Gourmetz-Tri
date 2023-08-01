using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class DirectoryListing : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public string ExtraField { get; set; }
        public string IconUrl { get; set; }
        public bool AvailableForBooking { get; set; }

        public bool IsInternal { get; set; }

        public string Aliases { get; set; }

        public string Disciplines { get; set; }

        public string Contact { get; set; }

        public string UnitNumber { get; set; }

        public string HighlightIcon { get; set; }
        public string HighlightText { get; set; }
        public bool Pause { get; set; }
        public int? PauseTime { get; set; }

        public string MapDisplayName { get; set; }

        public int? DirectoryListingCategoryId { get; set; }
        [ForeignKey("DirectoryListingCategoryId")]
        public virtual DirectoryListingCategory DirectoryListingCategory { get; set; }
        
        public string OpeningHours { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public int? FloorId { get; set; }
        [ForeignKey("FloorId")]
        public virtual Floor Floor { get; set; }
    }
}
