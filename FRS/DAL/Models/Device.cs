using DAL.Core.Audit.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Device: AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [StringLength(250)]
        public string Code { get; set; }
        [StringLength(250)]
        public string IpAddress { get; set; }
        [StringLength(250)]
        public string MacAddress { get; set; }
        [StringLength(250)]
        public string SerialNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? LocationId { get; set; }
        public int? InstitutionId { get; set; }
        public bool IsApproved { get; set; }
        public int? DeviceTypeId { get; set; }

        public string module_path { get; set; }

        public int? PublicationId { get; set; }
        [ForeignKey("PublicationId")]
        public string module_path_value { get; set; }
        [SkipTracking]
        public int device_status { get; set; }
        [SkipTracking]
        public DateTime? last_heartbeat { get; set; }
        [StringLength(250)]
        public string device_label { get; set; }
        public int? CommunicatorMenuId { get; set; }
        public int? ModuleId { get; set; }
        public int? PIBTemplateId { get; set; }
        public virtual SignagePublication Publication { get; set; }

        public string EmergencyMessage1 { get; set; }
        public string EmergencyMessage2 { get; set; }
        public string EmergencyMessage3 { get; set; }
        public string EmergencyMessage4 { get; set; }

        public string customUrl { get; set; }

        public virtual Location Location { get; set; }
        public string Rotation { get; set; }
        public string Resolution { get; set; }
        public int? Brightness { get; set; }
        public int? Volume { get; set; }
        public bool monitorStatus { get; set; }
        public bool isScreenOn { get; set; }

        public int? ems_id { get; set; }
        public int? media_id { get; set; }
        public int? display_type_id { get; set; }
        public long? external_location_id { get; set; }
        public int? external_ems_id { get; set; }
        public int? external_media_id { get; set; }

        public int? defaultVolume { get; set; }

        public string LocationName { get; set; }
        public string InstitutionName { get; set; }
        public string Scheme { get; set; }
        public string Port { get; set; }
        //public virtual Institution Institution { get; set; }
        //public virtual Location Location { get; set; }
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        [ForeignKey("ems_id")]
        public virtual Ems Ems { get; set; }

        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType DeviceType { get; set; }
    }
}
