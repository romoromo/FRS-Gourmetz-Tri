using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EpaperDevice : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string device_code { get; set; }
        public int device_status { get; set; }
        public string device_label { get; set; }

        public string mac_address { get; set; }
        public string ip_address { get; set; }
        public string host_address { get; set; }
        public DateTime? last_heartbeat { get; set; }

        public long? location_id { get; set; }
        public string location_code { get; set; }

        public string epaper_url { get; set; }
    }
}
