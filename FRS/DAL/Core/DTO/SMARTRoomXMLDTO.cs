using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DAL.Core.DTO
{
    [XmlRoot("SMARTRoom")]
    public class SMARTRoomXML
    {
        public CallStatusXML CallStatus { get; set; }
        public ApplicationDataXML ApplicationData { get; set; }
    }

    [XmlType("CallStatus")]
    public class CallStatusXML
    {
        public string StatusCode { get; set; }
        public string StatusRemarks { get; set; }
    }

    [XmlType("ApplicationData")]
    public class ApplicationDataXML
    {
        public AppUserTokenXML AppUserToken { get; set; }

        [XmlElement("Rooms")]
        public RoomsXML Rooms { get; set; }

        [XmlElement("Schedules")]
        public SchedulesXML Schedules { get; set; }
    }

    [XmlType("AppUserToken")]
    public class AppUserTokenXML
    {
        [XmlElement("id")]
        public int ID { get; set; }

        public string UserId { get; set; }
        public string AccessToken { get; set; }
    }

    [XmlType("Rooms")]
    public class RoomsXML
    {
        [XmlElement("Room")]
        public List<RoomXML> Room { get; set; }
    }

    [XmlType("Room")]
    public class RoomXML
    {
        [XmlElement("id")]
        public int ID { get; set; }

        [XmlElement("AlternateId")]
        public string AlternateID { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("FloorId")]
        public string FloorIDString { get; set; }

        [XmlIgnore]
        public int? FloorID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FloorIDString))
                {
                    return int.Parse(FloorIDString);
                }

                return null;
            }
        }

        public string Floor { get; set; }
        public string ExchangeID { get; set; }

        [XmlElement("extension")]
        public string Extension { get; set; }

        [XmlElement("ip")]
        public string IP { get; set; }

        public string DeviceName { get; set; }

        [XmlElement("category")]
        public string Category { get; set; }

        [XmlElement("remarks")]
        public string Remarks { get; set; }

        [XmlElement("FloorLocationX")]
        public string FloorLocationXString { get; set; }

        [XmlIgnore]
        public int? FloorLocationX
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FloorLocationXString))
                {
                    return int.Parse(FloorLocationXString);
                }

                return null;
            }
        }

        [XmlElement("FloorLocationY")]
        public string FloorLocationYString { get; set; }

        [XmlIgnore]
        public int? FloorLocationY
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FloorLocationYString))
                {
                    return int.Parse(FloorLocationYString);
                }

                return null;
            }
        }

        public int SeatingCapacity { get; set; }

        [XmlElement("access")]
        public string Access { get; set; }

        [XmlElement("DateCreated")]
        public string DateCreatedString { get; set; }

        [XmlElement("CreatedBy")]
        public string CreatedByString { get; set; }

        [XmlIgnore]
        public int? CreatedBy
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CreatedByString))
                {
                    return int.Parse(CreatedByString);
                }

                return null;
            }
        }

        [XmlIgnore]
        public DateTime? DateCreated
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(DateCreatedString))
                {
                    return DateTime.Parse(DateCreatedString);
                }

                return null;
            }
        }
    }

    [XmlType("Schedules")]
    public class SchedulesXML
    {
        [XmlElement("Schedule")]
        public List<ScheduleXML> Schedule { get; set; }
    }

    public class ScheduleXML
    {
        [XmlElement("id")]
        public int ID { get; set; }

        [XmlElement("AlternateId")]
        public string AlternateID { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("category")]
        public string Category { get; set; }

        [XmlElement("RoomId")]
        public string RoomIDString { get; set; }

        [XmlIgnore]
        public int? RoomID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(RoomIDString))
                {
                    return int.Parse(RoomIDString);
                }

                return null;
            }
        }

        [XmlElement("AlternateRoomId")]
        public string AlternateRoomID { get; set; }

        [XmlElement("UserID")]
        public string UserIDString { get; set; }

        [XmlIgnore]
        public int? UserID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(UserIDString))
                {
                    return int.Parse(UserIDString);
                }

                return null;
            }
        }

        public string UserName { get; set; }
        public string MeetingContactNo { get; set; }

        [XmlElement("StartTime")]
        public string StartTimeString { get; set; }

        [XmlIgnore]
        public DateTime? StartTime
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(StartTimeString))
                {
                    return DateTime.Parse(StartTimeString);
                }

                return null;
            }
        }

        [XmlElement("EndTime")]
        public string EndTimeString { get; set; }

        [XmlIgnore]
        public DateTime? EndTime
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(EndTimeString))
                {
                    return DateTime.Parse(EndTimeString);
                }

                return null;
            }
        }

        public string ReserveSource { get; set; }

        [XmlElement("status")]
        public string Status { get; set; }

        public string StatusRemarks { get; set; }

        [XmlElement("remarks")]
        public string Remarks { get; set; }

        [XmlElement("RequestorId")]
        public string RequestorID { get; set; }

        public string Requestor { get; set; }

        [XmlElement("DateCreated")]
        public string DateCreatedString { get; set; }

        [XmlIgnore]
        public DateTime? DateCreated
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(DateCreatedString))
                {
                    return DateTime.Parse(DateCreatedString);
                }

                return null;
            }
        }

        [XmlElement("StartedBy")]
        public string StartedByString { get; set; }

        [XmlIgnore]
        public int? StartedBy
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(StartedByString))
                {
                    return int.Parse(StartedByString);
                }

                return null;
            }
        }

        [XmlElement("StartedAt")]
        public string StartedAtString { get; set; }

        [XmlIgnore]
        public DateTime? StartedAt
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(StartedAtString))
                {
                    return DateTime.Parse(StartedAtString);
                }

                return null;
            }
        }

        [XmlElement("EndedBy")]
        public string EndedByString { get; set; }

        [XmlIgnore]
        public int? EndedBy
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(EndedByString))
                {
                    return int.Parse(EndedByString);
                }

                return null;
            }
        }

        [XmlElement("EndedAt")]
        public string EndedAtString { get; set; }

        [XmlIgnore]
        public DateTime? EndedAt
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(EndedAtString))
                {
                    return DateTime.Parse(EndedAtString);
                }

                return null;
            }
        }

        [XmlElement("LastUpdatedDateTime")]
        public string LastUpdatedDateTimeString { get; set; }

        [XmlIgnore]
        public DateTime? LastUpdatedDateTime
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(LastUpdatedDateTimeString))
                {
                    return DateTime.Parse(LastUpdatedDateTimeString);
                }

                return null;
            }
        }
    }
}
