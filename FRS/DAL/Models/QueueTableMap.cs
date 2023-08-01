using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class QueueTableMap : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string QueueId { get; set; }

        public string RoomName { get; set; }

        //public string DeviceId { get; set; }

        public int? DeviceIdentifier { get; set; }
        [ForeignKey("DeviceIdentifier")]
        public virtual Device Device { get; set; }

        public string StationId { get; set; }

        public string CurrentDisplay { get; set; }

        public string LastCall { get; set; }

        public string LastReturn { get; set; }
    }
}
