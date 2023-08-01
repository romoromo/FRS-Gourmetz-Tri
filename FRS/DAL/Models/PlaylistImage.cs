using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class PlaylistImage : AuditableEntity
    {
        public int imageIndex { get; set; }

        public int? duration { get; set; }
        public string animation { get; set; }

        [ForeignKey("Playlist")]
        public int PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }

        [ForeignKey("ImageFile")]
        public int ImageId { get; set; }
        public virtual ImageFile ImageFile { get; set; }
    }
}
