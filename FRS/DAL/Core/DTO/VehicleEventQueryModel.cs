using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class VechicleEventQueryModel
    {
        public List<VehicleQueryModel> Vehicles { get; set; }
    }

    public class VehicleQueryModel: VMSVehicleModel
    {
        public string Type { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class VMSVehiclePostRequestModel
    {
        public string bookingDescription { get; set; }

        public string seasonId { get; set; }

        public string personName { get; set; }

        public string cardType { get; set; }

        public string vehicle { get; set; }

        public string issueDate { get; set; }

        public string expiryDate { get; set; }
    }

    public class VMSVehicleBaseModel
    {
        public string SeasonId { get; set; }

        public string PersonName { get; set; }

        public string CardType { get; set; }

        public string Vehicle { get; set; }

        public string VehicleStatus { get; set; }
    }

    public class VMSVehicleModel : VMSVehicleBaseModel
    {
        public DateTime? IssueDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }

    public class VehicleLogFilter
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }

    public class VMSVehicleLog: VehicleQueryModel
    {
        public string BookingDescription { get; set; }
    }
}
