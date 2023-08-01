using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class PointDirectoryListing : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? PointId { get; set; }
        [ForeignKey("PointId")]
        public virtual Point Point { get; set; }

        public int? DirectoryListingId { get; set; }
        [ForeignKey("DirectoryListingId")]
        public virtual DirectoryListing DirectoryListing { get; set; }
    }
}
