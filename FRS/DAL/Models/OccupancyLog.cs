using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class OccupancyLog : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? Datetime { get; set; }
        public string DeviceId { get; set; }
        public string SensorId { get; set; }
        public string Status { get; set; }
    }
}
