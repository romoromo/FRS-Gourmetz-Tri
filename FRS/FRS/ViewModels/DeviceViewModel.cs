using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class DeviceViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public string SerialNumber { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? LocationId { get; set; }
        public int? InstitutionId { get; set; }
        public bool IsApproved { get; set; }

        public string InstitutionName { get; set; }
        public string LocationName { get; set; }
        public string LocationColorTheme { get; set; }
        public string LocationGroup { get; set; }
        public int? defaultVolume { get; set; }

        public int? PublicationId { get; set; }
        public string PublicationName { get; set; }

        public string module_path { get; set; }
        public string module_path_value { get; set; }
        public int device_status { get; set; }
        public string device_label { get; set; }
        public DateTime? last_heartbeat { get; set; }

        public string EmergencyMessage1 { get; set; }
        public string EmergencyMessage2 { get; set; }
        public string EmergencyMessage3 { get; set; }
        public string EmergencyMessage4 { get; set; }

        public string customUrl { get; set; }

        public int? CommunicatorMenuId { get; set; }
        public int? ModuleId { get; set; }
        public int? PIBTemplateId { get; set; }
        public string Rotation { get; set; }
        public string Resolution { get; set; }
        public int? Brightness { get; set; }
        public int? Volume { get; set; }
        public bool monitorStatus { get; set; }
        public bool isScreenOn { get; set; }

        public bool rebootWhenUpdate { get; set; }

        public int? ems_id { get; set; }
        public int? media_id { get; set; }
        public int? display_type_id { get; set; }
        public long? external_location_id { get; set; }
        public int? external_ems_id { get; set; }
        public int? external_media_id { get; set; }
        public string Scheme { get; set; }
        public string Port { get; set; }
        public int? DeviceTypeId { get; set; }
        public string DeviceTypeName { get; set; }
        public List<ModuleParameterViewModel> ModuleParameters { get; set; }

        public string Status
        {
            get
            {
                if (!IsApproved || (this.InstitutionId > 0 && this.LocationId > 0))// && this.StartDate.HasValue && this.StartDate.Value > DateTime.MinValue))
                {
                    return IsApproved ? "Approved" : "Pending"; 
                }
                else
                {
                    return "Unknown";
                }
            }
            set
            {

            }
        
        }

        public string status_display
        {
            get
            {
                return device_status == 0 ? "OFFLINE" : "ONLINE";
            }
        }

        public string connection_status_display { get; set; }
    }
}
